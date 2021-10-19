using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(AudioEvent), true)]
[CanEditMultipleObjects]
public class AudioEventEditor : Editor
{

	[SerializeField] private AudioSource _previewer;

	public void OnEnable()
	{
		_previewer = EditorUtility.CreateGameObjectWithHideFlags("Audio preview", HideFlags.HideAndDontSave, typeof(AudioSource)).GetComponent<AudioSource>();
	}

	public void OnDisable()
	{
        if (_previewer)
    		DestroyImmediate(_previewer.gameObject);
	}

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);
		if (GUILayout.Button("Preview"))
		{
			((AudioEvent) target).Play(_previewer);
		}
		EditorGUI.EndDisabledGroup();
	}
}
