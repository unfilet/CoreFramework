#if UNITY_PURCHASING
using UnityEngine.Events;
using UnityEngine.UI;
using System.IO;

namespace UnityEngine.Purchasing
{
    using ButtonType = IAPButton.ButtonType;

    [RequireComponent(typeof(Button))]
    [AddComponentMenu("Unity IAP/NIAP Button")]
  
    public class NIAPButton : MonoBehaviour
    {
        [HideInInspector]
        public string productId;

        [Tooltip("The type of this button, can be either a purchase or a restore button")]
        public ButtonType buttonType = ButtonType.Purchase;

        [Space]

        [Tooltip("[Optional] Displays the localized title from the app store")]
        public Text titleText;

        [Tooltip("[Optional] Displays the localized description from the app store")]
        public Text descriptionText;

        [Tooltip("[Optional] Displays the localized price from the app store")]
        public Text priceText;

        void Start()
        {
            Button button = GetComponent<Button>();
            if (!button) return;



            if (buttonType == ButtonType.Purchase)
            {
                    button.onClick.AddListener(() => 
                        InAppsManager.Instance.Purchase(productId)
                    );

                if (string.IsNullOrEmpty(productId))
                    Debug.LogError("IAPButton productId is empty");
            }
            else if (buttonType == ButtonType.Restore)
            {
                if (Application.platform == RuntimePlatform.Android)
                    gameObject.SetActive(false);
                button.onClick.AddListener(InAppsManager.Instance.Restore);
            }
        }

        void OnEnable()
        {

            if (buttonType == ButtonType.Purchase)
                InAppsManager.Instance.OnInited += UpdateText;
        }

        void OnDisable()
        {
            if (buttonType == ButtonType.Purchase)
                InAppsManager.Instance.OnInited -= UpdateText;
        }

     
        internal void UpdateText(bool inited)
        {
            var product = InAppsManager.Instance.GetProduct(productId);
            if (inited && product != null)
            {
                if (titleText != null)
                    titleText.text = product.metadata.localizedTitle;

                if (descriptionText != null)
                    descriptionText.text = product.metadata.localizedDescription;

                if (priceText != null)
                    priceText.text = product.metadata.localizedPriceString;
            }
        }
    }
}
#endif
