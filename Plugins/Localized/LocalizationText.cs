using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace Localized
{
    [ExecuteInEditMode]
    public class LocalizationText : MonoBehaviourExt
    {
        [HideInInspector]
        public string keyForLocalization = string.Empty;
        [SerializeField] Text label = null;
        [SerializeField] TMP_Text tm_label = null;

        public Func<string, string> RequestForParams = null;

        protected override void Awake ()
        {
            base.Awake ();
            if (label == null)
                label = this.GetComponent<Text> ();
            else if (tm_label == null)
                tm_label = this.GetComponent<TMP_Text> ();
        }

        protected override void OnEnable ()
        {
            SetLocalText ();
            LocalizationManager.Instance.OnLanguageChange += SetLocalText;
        }

        protected override void OnDisable ()
        {
            LocalizationManager.Instance.OnLanguageChange -= SetLocalText;
        }

        public void SetLocalText ()
        {
            string text = LocalizationManager.Instance.GetLocalValue (keyForLocalization);

            if (RequestForParams != null)
                text = RequestForParams (text);

            if (string.IsNullOrEmpty (text))
                return;

            if (label != null)
                label.text = text;
            else if (tm_label != null)
                tm_label.text = text;
        }

    }
}