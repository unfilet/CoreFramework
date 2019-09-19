using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Localized
{
    [CustomEditor (typeof(LocalizationText))]
    public class LocalizationTextEditor : Editor
    {
        SerializedObject so_target;
        SerializedProperty sp_key;

        List<string> allKeys = new List<string> ();

        private void OnEnable () {
            so_target = new SerializedObject (target);
            sp_key = so_target.FindProperty ("keyForLocalization");

            allKeys.Clear ();
            allKeys.Add ("None");
            allKeys.AddRange (LocalizationManager.Instance.GetAllKeys ());
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI ();
            
            EditorGUILayout.Space ();

            so_target.Update ();

            string propertyString = sp_key.stringValue;

            int index = Mathf.Max(0, allKeys.IndexOf(propertyString));
            index = EditorGUILayout.Popup ("Key For Localization", index, allKeys.ToArray());
            if (index > 0)
                propertyString = allKeys [index];
            else
                propertyString = string.Empty;

            sp_key.stringValue = propertyString;

            if (so_target.ApplyModifiedProperties ()) {
                (so_target.targetObject as LocalizationText).SetLocalText ();
            }
        }
       
    }
}