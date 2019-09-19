#if UNITY_PURCHASING

using UnityEngine;
using UnityEngine.Purchasing.Security;
using System.Linq;
using System.Collections.Generic;

namespace UnityEngine.Purchasing
{
    public partial class InAppsManager
    {
        private class InAppsManagerUnityIAP : InAppsManager, IStoreListener
        {
            protected IStoreController controller;
            protected IExtensionProvider extensions;
            protected ProductCatalog catalog;

            private static readonly bool consumePurchase = true;
            private static bool unityPurchasingInitialized;

            public InAppsManagerUnityIAP() : base()
            {
                catalog = ProductCatalog.LoadDefaultCatalog();

                InitializePurchasing();
            }

            private void InitializePurchasing()
            {
                if (unityPurchasingInitialized) return;

                //var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
                //foreach (var item in consumablePurchases)
                //    builder.AddProduct(item, ProductType.Consumable);
                //foreach (var item in nonConsumablePurchases)
                //    builder.AddProduct(item, ProductType.NonConsumable);

                //UnityPurchasing.Initialize(this, builder);

                StandardPurchasingModule module = StandardPurchasingModule.Instance();
                module.useFakeStoreUIMode = FakeStoreUIMode.StandardUser;

                ConfigurationBuilder builder = ConfigurationBuilder.Instance(module);
//                builder.Configure<IGooglePlayConfiguration>().SetPublicKey("MIIBIjA...j");

                IAPConfigurationHelper.PopulateConfigurationBuilder(ref builder, catalog);
                UnityPurchasing.Initialize(this, builder);

                unityPurchasingInitialized = true;
            }

            #region IInAppPurchaseManager

            public override void Purchase(string productIdentifier)
            {
                if (internetVerifier != null && !internetVerifier.Reachable)
                {
                    ShowAlert(message: "Please, check your internet connection");
                    return;
                }

                if (!HasProductInCatalog(productIdentifier))
                {
                    Debug.LogWarning("The product catalog has no product with the ID \"" + productIdentifier + "\"");
                    return;
                }

                preloader(show: true);
                if (!IsInited)
                {
                    void OnInitialized(bool status) {
                        OnInited -= OnInitialized;
                        if (status)
                            Purchase(productIdentifier);
                        else
                            Debug.Log("BuyProductID FAIL. Not initialized.");
                    }

                    OnInited += OnInitialized;
                    InitializePurchasing();
                }
                else try
                    {
                        Product product = GetProduct(productIdentifier);
                        if (product != null && product.availableToPurchase)
                        {
                            Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                            controller.InitiatePurchase(product);
                        }
                        else
                        {
                            preloader(show: false);
                            Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                        }
                    }
                    catch (System.Exception e)
                    {
                        preloader(show: false);
                        Debug.Log("BuyProductID: FAIL. Exception during purchase. " + e);
                    }
            }

            public override void Restore()
            {
                if (internetVerifier != null && !internetVerifier.Reachable)
                {
                    ShowAlert(message: "Please, check your internet connection");
                    return;
                }

                preloader(show: true);
                if (!IsInited)
                {
                    void OnInitialized(bool status)
                    {
                        OnInited -= OnInitialized;
                        if (status)
                            Restore();
                        else
                            Debug.Log("RestorePurchases FAIL. Not initialized.");
                    }
                    OnInited += OnInitialized;
                    InitializePurchasing();
                }
                else if (Application.platform == RuntimePlatform.WSAPlayerX86 ||
                    Application.platform == RuntimePlatform.WSAPlayerX64 ||
                    Application.platform == RuntimePlatform.WSAPlayerARM)
                {
                    extensions.GetExtension<IMicrosoftExtensions>()
                        .RestoreTransactions();
                }
                else if (Application.platform == RuntimePlatform.IPhonePlayer ||
                         Application.platform == RuntimePlatform.OSXPlayer ||
                         Application.platform == RuntimePlatform.tvOS)
                {
                    extensions.GetExtension<IAppleExtensions>()
                        .RestoreTransactions(OnTransactionsRestored);
                }
                else if (Application.platform == RuntimePlatform.Android &&
                         StandardPurchasingModule.Instance().appStore == AppStore.SamsungApps)
                {
                    extensions.GetExtension<ISamsungAppsExtensions>()
                        .RestoreTransactions(OnTransactionsRestored);
                }
                else if (Application.platform == RuntimePlatform.Android &&
                         StandardPurchasingModule.Instance().appStore == AppStore.CloudMoolah)
                {
                    extensions.GetExtension<IMoolahExtension>()
                        .RestoreTransactionID((restoreTransactionIDState) =>
                        {
                            OnTransactionsRestored(
                                restoreTransactionIDState != RestoreTransactionIDState.RestoreFailed &&
                                restoreTransactionIDState != RestoreTransactionIDState.NotKnown);
                        });
                }
                else
                {
                    preloader(show: false);
                    Debug.LogWarning(Application.platform.ToString() +
                                     " is not a supported platform for the Codeless IAP restore button");
                }
            }

            public override string GetPrice(string SKU)
            {
                if (!IsInited || (internetVerifier != null && !internetVerifier.Reachable))
                    return "—"; //base.GetPrice(SKU);//string.Empty;

                var tpl = GetProduct(SKU); ;
                return tpl.metadata.localizedPriceString;
            }

            #endregion

            #region Help function

            public bool HasProductInCatalog(string productID)
            {
                foreach (var product in catalog.allProducts)
                    if (product.id == productID)
                        return true;
                return false;
            }

            public override Product GetProduct(string productID)
            {
                if (controller != null && controller.products != null && !string.IsNullOrEmpty(productID))
                    return controller.products.WithID(productID);
                Debug.LogError("CodelessIAPStoreListener attempted to get unknown product " + productID);
                return null;
            }

            #endregion

            private void UnlockProducts(Product product, bool restored = false)
            {
                if (!IsInited) return;

                ShowAlert("Success", "The " + product.metadata.localizedTitle + " was " + (restored ? "restored" : "purchased"));
                OnInAppPurchaseComplete?.Invoke(product.definition.id);
            }

            private void OnTransactionsRestored(bool IsSucceeded)
            {
                preloader(show: false);
                if (IsSucceeded)
                {
                    ShowAlert("Success", "Restore Compleated");
                }
                else
                {
                    ShowAlert("", "Transaction Failed");
                }
                Debug.Log("Transactions restored.");
            }

            private void OnDeferred(Product item)
            {
                Debug.Log("Purchase deferred: " + item.definition.id);
            }

            #region IStoreListener

            public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
            {
                preloader(show: false);

                this.controller = controller;
                this.extensions = extensions;

                IsInited = controller != null && extensions != null;
                if (!IsInited) return;

                extensions.GetExtension<IAppleExtensions>().RegisterPurchaseDeferredListener(OnDeferred);

                string str = "Available items:\n";
                foreach (var item in controller.products.all)
                {
                    if (item.availableToPurchase)
                    {
                        str += string.Join(" - ",
                            item.metadata.localizedTitle,
                            item.metadata.localizedDescription,
                            item.metadata.isoCurrencyCode,
                            item.metadata.localizedPrice.ToString(),
                            item.metadata.localizedPriceString
                            );
                        str += "\n";
                    }
                }
                Debug.Log(str);
            }

            public void OnInitializeFailed(InitializationFailureReason error)
            {
                preloader(show: false);

                Debug.Log("Billing failed to initialize!");
                switch (error)
                {
                    case InitializationFailureReason.AppNotKnown:
                        Debug.LogError("Is your App correctly uploaded on the relevant publisher console?");
                        break;
                    case InitializationFailureReason.PurchasingUnavailable:
                        // Ask the user if billing is disabled in device settings.
                        Debug.Log("Billing disabled!");
                        break;
                    case InitializationFailureReason.NoProductsAvailable:
                        // Developer configuration error; check product metadata.
                        Debug.Log("No products available for purchase!");
                        break;
                }
                IsInited = false;
                unityPurchasingInitialized = false;
            }

            public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
            {
                preloader(show: false);

                if (failureReason != PurchaseFailureReason.UserCancelled)
                    ShowAlert("Something wrong", "Please try again later");
                Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
                OnInAppPurchaseFailed?.Invoke(product.definition.id);
            }

            public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
            {
                preloader(show: false);

                bool validPurchase = true; // Presume valid for platforms with no R.V.

                // Unity IAP's validation logic is only included on these platforms.
#if (UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX) && !UNITY_EDITOR && false
            // Prepare the validator with the secrets we prepared in the Editor
            // obfuscation window.
            var validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
                AppleTangle.Data(), Application.identifier);

            try {
                // On Google Play, result will have a single product Id.
                // On Apple stores receipts contain multiple products.
                var result = validator.Validate(args.purchasedProduct.receipt);
                // For informational purposes, we list the receipt(s)
                Debug.Log("Receipt is valid. Contents:");
                foreach (IPurchaseReceipt productReceipt in result) {
                    Debug.Log(productReceipt.productID);
                    Debug.Log(productReceipt.purchaseDate);
                    Debug.Log(productReceipt.transactionID);
                }
            } catch (IAPSecurityException) {
                Debug.Log("Invalid receipt, not unlocking content");
                validPurchase = false;
            }
#endif
                if (validPurchase) UnlockProducts(args.purchasedProduct);
                return (consumePurchase) ? PurchaseProcessingResult.Complete : PurchaseProcessingResult.Pending;
            }

            #endregion

        }
    }

    public static class ProductCatalogExt 
    {
        private static Dictionary<string, ProductCatalogItem> productsDict;

        public static ProductCatalogItem WithID(this ProductCatalog @this, string productId)
        {
            var prod = @this.allProducts;
            if (productsDict == null || productsDict.Count != prod.Count)
                productsDict = prod.ToDictionary(k => k.id);

            productsDict.TryGetValue(productId, out ProductCatalogItem item);
            return item;
        }
    }
}
#endif