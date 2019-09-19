#if UNITY_PURCHASING
using UnityEditor;
using UnityEngine;
using UnityEngine.Purchasing;
using System.IO;
using System.Collections.Generic;

namespace UnityEditor.Purchasing
{
	[CustomEditor(typeof(NIAPButton))]
	[CanEditMultipleObjects]
	public class NIAPButtonEditor : Editor
	{
		private static readonly string[] excludedFields = new string[] { "m_Script" };
		private static readonly string[] restoreButtonExcludedFields = new string[] { "m_Script", "titleText", "descriptionText", "priceText" };
		private const string kNoProduct = "<None>";

		private List<string> m_ValidIDs = new List<string>();
		private SerializedProperty m_ProductIDProperty;

		public void OnEnable()
		{
			m_ProductIDProperty = serializedObject.FindProperty("productId");
		}

		public override void OnInspectorGUI()
		{
            NIAPButton button = (NIAPButton)target;

			serializedObject.Update();

			if (button.buttonType == IAPButton.ButtonType.Purchase) {
				EditorGUILayout.LabelField(new GUIContent("Product ID:", "Select a product from the IAP catalog"));

				var catalog = ProductCatalog.LoadDefaultCatalog();

				m_ValidIDs.Clear();
				m_ValidIDs.Add(kNoProduct);
				foreach (var product in catalog.allProducts) {
					m_ValidIDs.Add(product.id);
				}

				int currentIndex = string.IsNullOrEmpty(button.productId) ? 0 : m_ValidIDs.IndexOf(button.productId);
				int newIndex = EditorGUILayout.Popup(currentIndex, m_ValidIDs.ToArray());
				if (newIndex > 0 && newIndex < m_ValidIDs.Count) {
					m_ProductIDProperty.stringValue = m_ValidIDs[newIndex];
				} else {
					m_ProductIDProperty.stringValue = string.Empty;
				}

				if (GUILayout.Button("IAP Catalog...")) {
					ProductCatalogEditor.ShowWindow();
				}
			}

			DrawPropertiesExcluding(serializedObject, button.buttonType == IAPButton.ButtonType.Restore ? restoreButtonExcludedFields : excludedFields);

			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif
