using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

//Original by DYLAN ENGELMAN http://jupiterlighthousestudio.com/custom-inspectors-unity/
//Altered by Brecht Lecluyse http://www.brechtos.com

[CustomPropertyDrawer(typeof(SceneSelectorAttribute))]
public class SceneSelectorPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.String)
        {
            EditorGUI.BeginProperty(position, label, property);

            /**/
            var oldScene = GetSceneObject(property.stringValue);

            EditorGUI.BeginChangeCheck();
            var newScene = EditorGUI.ObjectField(position, label, oldScene, typeof(SceneAsset), false);
            if (EditorGUI.EndChangeCheck())
            {
                if (newScene == null)
                    property.stringValue = "";
                else
                    property.stringValue = AssetDatabase.GetAssetPath(newScene);
            }
            /**
            var attrib = this.attribute as SceneSelectorAttribute;

            var popupList = EditorBuildSettings.scenes
                .Where(s => s.enabled)
                .Select(s => System.IO.Path.GetFileNameWithoutExtension(s.path))
                .ToArray();

            string propertyString = property.stringValue;
            int index = Array.IndexOf(popupList, propertyString);
            index = EditorGUI.Popup(position, label.text, index, popupList);

            property.stringValue = index >= 0 && index < popupList.Length ? popupList[index] : "";
            /**/
            EditorGUI.EndProperty();
        }
        else
        {
            EditorGUI.PropertyField(position, property, label);
        }
    }

    protected SceneAsset GetSceneObject(string sceneObjectName)
    {
        if (string.IsNullOrEmpty(sceneObjectName))
            return null;

        var obj = EditorBuildSettings.scenes
            .FirstOrDefault(s => s.path.Contains(sceneObjectName));

        if (obj != null)
            return AssetDatabase.LoadAssetAtPath(obj.path, typeof(SceneAsset)) as SceneAsset;

        Debug.LogWarning("Scene [" + sceneObjectName + "] cannot be used. Add this scene to the 'Scenes in the Build' in build settings.");
        return null;
    }
}