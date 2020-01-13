using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.UI;


[CustomEditor(typeof(SkeletoneAspectRatioFitter), true)]
public class SkeletoneAspectRatioFitterEditor : SelfControllerEditor
{
    private SerializedProperty m_ViewportResolution;
    private SerializedProperty m_AspectMode;
    private SerializedProperty m_Offset;
    private SkeletoneAspectRatioFitter m_target;

    protected virtual void OnEnable()
    {
        m_target = target as SkeletoneAspectRatioFitter;
        m_ViewportResolution = this.serializedObject.FindProperty("m_ViewportResolution");
        m_AspectMode = this.serializedObject.FindProperty("m_AspectMode");
        m_Offset = this.serializedObject.FindProperty("m_offset");
    }


    public override void OnInspectorGUI()
    {
        this.serializedObject.Update();
        EditorGUILayout.PropertyField(m_ViewportResolution);
        EditorGUILayout.PropertyField(this.m_AspectMode);
        EditorGUILayout.PropertyField(this.m_Offset);
        this.serializedObject.ApplyModifiedProperties();

        EditorGUILayout.LabelField($"AspectRatio", $"{m_target.aspectRatio}");
    }
}
