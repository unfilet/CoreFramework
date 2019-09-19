using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;

namespace Localized
{
    public class LocalizationManager : System.Object
    {
        private static LocalizationManager _instance;
        public static LocalizationManager Instance { 
            get { return _instance ?? (_instance = new LocalizationManager ()); }
        }

        private static readonly string defaultLanguage = "en";
        private LocalizationData currentLocalization;

        public bool IsReady { get; private set; }

        public event Action OnLanguageChange = null;

        public static string UsedLanguage {
            get { return PlayerPrefs.GetString ("cws_language", GetSystemLanguage ()); }
            private set {
                if (UsedLanguage == value)
                    return;
                PlayerPrefs.SetString ("cws_language", value); 
            }
        }

        private LocalizationManager ()
        {
            LoadLocalizedText (UsedLanguage);
        }

        public void ChangeLanguage ()
        {
            UsedLanguage = UsedLanguage == "en" ? "ru" : "en"; 
            LoadLocalizedText (UsedLanguage);
        }

        public bool LoadLocalizedText(string fileName)
        {
            IsReady = false;

            currentLocalization = null;

            TextAsset asset = Resources.Load<TextAsset>($"localizedText.{fileName}");

            /**
            string filePath = Path.Combine(
                Application.streamingAssetsPath,
                string.Format("localizedText.{0}.json", fileName)
            );
            if (File.Exists(filePath)) {
                string dataAsJson = File.ReadAllText(filePath);
                currentLocalization = JsonUtility.FromJson<LocalizationData>(dataAsJson);
            } else {
                Debug.LogError("Cannot find file!");
                return LoadLocalizedText(defaultLanguage);
            }
            /**/
            if (asset == null || string.IsNullOrEmpty(asset.text)) {
                Debug.LogError("Cannot find file!");
                return LoadLocalizedText(defaultLanguage);
            }
            else {
                currentLocalization = JsonUtility.FromJson<LocalizationData>(asset.text);
            }


            OnLanguageChange?.Invoke();

            return (IsReady = currentLocalization != null);
        }

        public string GetLocalValue (string key)
        {
            return currentLocalization.GetValue (key);
        }

        public string[] GetAllKeys ()
        {
            return currentLocalization.GetAllKeys ();
        }

        public static string GetSystemLanguage ()
        {
            switch (Application.systemLanguage) {
            default:
                return defaultLanguage;
            }
        }

        public string this[string key]
        {   
            get { return GetLocalValue(key); }
        }
    }
}