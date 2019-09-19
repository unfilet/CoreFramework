using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Localized
{
    [Serializable]
    public class LocalizationData : ISerializationCallbackReceiver
    {
        public List<LocaleTextData> items;
        [NonSerialized]
        private Dictionary<string, string> localizedText;

        public string GetValue (string key)
        {
            string result = string.Empty;
            if (string.IsNullOrEmpty(key) || !localizedText.TryGetValue (key, out result))
                Debug.LogError ("Key not found in localization: " + key);
            return result;
        }

        public string[] GetAllKeys() {
            return localizedText.Keys.ToArray ();
        }

        public void OnBeforeSerialize ()
        {
        }

        public void OnAfterDeserialize ()
        {
            localizedText = items.ToDictionary (obj => obj.key, obj => obj.value);
        }
    }

    [Serializable]
    public class LocaleTextData
    {
        public string key;
        [TextArea (3, 10)]
        public string value;
    }

    [Serializable]
    public class LocaleObjectData
    {
        public string key;
        public GameObject value;
    }
}