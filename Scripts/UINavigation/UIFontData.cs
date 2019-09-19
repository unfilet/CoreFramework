using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UISystem
{
    [CreateAssetMenu(menuName = "UI System/Font Data")]
    public class UIFontData : ScriptableObject {
     
        public void Apply(GameObject go) {
            Apply (go.GetComponent<Text>());
            Apply (go.GetComponent<TextMesh>());
            Apply (go.GetComponent<TMP_Text>());
        }

        public Font font;
        public void Apply(Text label) {
            if (font != null && label != null) label.font = font;
        }
        public void Apply(TextMesh label) {
            if (font != null && label != null) label.font = font;
        }

        public TMP_FontAsset TMPFont;
        public void Apply (TMP_Text label) {
            if (TMPFont != null && label != null) label.font = TMPFont;
        }
    }
}