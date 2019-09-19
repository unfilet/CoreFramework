using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Localized
{
    public class LocalizationObject : MonoBehaviour
    {
        [SerializeField] LocaleObjectData[] objects = null;

        void OnEnable ()
        {
            SetLocal ();
            LocalizationManager.Instance.OnLanguageChange += SetLocal;
        }

        void OnDisable ()
        {
            LocalizationManager.Instance.OnLanguageChange -= SetLocal;
        }

        void SetLocal ()
        {
            string text = LocalizationManager.UsedLanguage;
            if (!string.IsNullOrEmpty (text))
                foreach (var item in objects)
                    item.value.SetActive (item.key == text);
        }
    }
}