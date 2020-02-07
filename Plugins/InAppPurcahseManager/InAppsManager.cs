using System.Linq;
using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

namespace UnityEngine.Purchasing
{
    public interface IInternetVerifier { 
        bool Reachable { get; }
    }

    public interface IAlerPopUp
    {
        void Show(string title, string message);
    }

    public interface IInAppPurchaseManager
    {
        event Action<string> OnInAppPurchaseComplete;
        event Action<string> OnInAppPurchaseFailed;
        event Action<bool> OnInited;

        bool IsInited { get; }
        void Restore();
        void Purchase(string productID);
        string GetPrice(string SKU);
#if UNITY_PURCHASING
        Product GetProduct(string productID);
#endif
        void Construct(IInternetVerifier v, IAlerPopUp a);
    }

    public partial class InAppsManager : IInAppPurchaseManager
    {
        private static IInAppPurchaseManager instance;
        public static IInAppPurchaseManager Instance
        {
            [RuntimeInitializeOnLoadMethod]
            get
            {
                return instance ?? (
#if UNITY_PURCHASING //&& !UNITY_EDITOR
                        instance = new InAppsManagerUnityIAP ()
#else
                        instance = new InAppsManager()
#endif
                    );
            }
        }

        private IInternetVerifier internetVerifier;
        private IAlerPopUp alerPopUp;

#pragma warning disable 414
        public event Action<string> OnInAppPurchaseComplete = null;
        public event Action<string> OnInAppPurchaseFailed = null;
        public event Action<bool> OnInited = null;
#pragma warning restore 414

        private bool _IsInited = false;
        public virtual bool IsInited
        {
            get => _IsInited;
            protected set
            {
                _IsInited = value;
                OnInited?.Invoke(_IsInited);
            }
        }

        protected InAppsManager()
        {
            IsInited = true;
#if UNITY_PURCHASING
            purchasesIds.AddRange(ProductCatalog.LoadDefaultCatalog().allProducts.Select(o => o.id));
#endif
        }

        protected virtual void preloader(bool show = true)
        {
#if !UNITY_WEBGL
#if UNITY_IOS
            Handheld.SetActivityIndicatorStyle(UnityEngine.iOS.ActivityIndicatorStyle.WhiteLarge);
#elif UNITY_ANDROID
            Handheld.SetActivityIndicatorStyle (AndroidActivityIndicatorStyle.Small);
#endif
            if (show)
                Handheld.StartActivityIndicator();
            else
                Handheld.StopActivityIndicator();
#endif
        }

        protected void ShowAlert(string title = "", string message = "")
            => alerPopUp?.Show(title,message);

        public virtual void Restore()
        {
            if (internetVerifier != null && !internetVerifier.Reachable)
            {
                ShowAlert(message: "Please, check your internet connection");
                return;
            }

            foreach (var item in purchasesIds)
                OnInAppPurchaseComplete?.Invoke(item);
        }

        public virtual void Purchase(string productIdentifier)
        {
            if (internetVerifier != null && !internetVerifier.Reachable)
            {
                ShowAlert(message: "Please, check your internet connection");
                return;
            }

            if (purchasesIds.Contains(productIdentifier))
                OnInAppPurchaseComplete?.Invoke(productIdentifier);
        }

        public virtual string GetPrice(string SKU)
        {
            return "$10.99";
        }

#if UNITY_PURCHASING
        public virtual Product GetProduct(string productID)
        {
            ProductCatalogItem t = ProductCatalog.LoadDefaultCatalog().WithID(productID);
            Product obj = CreateInstance<Product>(
                new ProductDefinition(
                    t.id,
                    t.id,
                    t.type,
                    true,
                    t.Payouts.Select(payout => new PayoutDefinition(payout.typeString, payout.subtype, payout.quantity, payout.data))
                    ),
                new ProductMetadata()
            );
            return obj;
        }
#endif

        public readonly List<string> purchasesIds = new List<string>();

        internal static T CreateInstance<T>(params object[] args)
        {
            var type = typeof(T);
            var instance = type.Assembly.CreateInstance(
                type.FullName, false,
                BindingFlags.Instance | BindingFlags.NonPublic,
                null, args, null, null);
            return (T)instance;
        }

        public void Construct(IInternetVerifier v, IAlerPopUp a)
        {
            internetVerifier = v;
            alerPopUp = a;
        }
    }
}