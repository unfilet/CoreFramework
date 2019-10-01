using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
using TMPro;
#endif

namespace UISystem
{
    [ExecuteInEditMode]
    public class UILabelFont : MonoBehaviour {

        [SerializeField] UIFontData font = null;
        void Awake() => Apply();
        public void Apply () => font?.Apply(this.gameObject);

#if UNITY_EDITOR
        private void OnValidate() => Apply();
#endif

    }


#if UNITY_EDITOR

    [CanEditMultipleObjects]
    [CustomEditor(typeof(UILabelFont))]
    public class UILabelFontEditor : Editor
    {
        private void OnEnable() => ((UILabelFont)target).Apply();

        public override void OnInspectorGUI()
        {
            if (DrawDefaultInspector() || GUILayout.Button("Apply"))
                foreach (var target in targets)
                    ((UILabelFont)target).Apply();

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        /**/
        [MenuItem("CONTEXT/TextMeshProUGUI/Convert to text")]
        public static void ConvertTotext(MenuCommand menuCommand)
        {
            TextMeshProUGUI mf = menuCommand.context as TextMeshProUGUI;

            string text = mf.text;
            Color color = mf.color;
            float size = mf.fontSize;

            var go = mf.gameObject;
            DestroyImmediate(mf);

            var lbl = go.AddComponent<Text>();
            lbl.text = text;
            lbl.color = color;
            lbl.fontSize = (int)size;
        }
        /**/
    }

#endif

}