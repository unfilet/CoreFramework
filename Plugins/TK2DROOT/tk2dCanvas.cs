using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[ExecuteInEditMode]
public class tk2dCanvas : MonoBehaviour
{
    [SerializeField]
    tk2dCamera tk2dCamera = null;

    private new RectTransform transform => base.transform as RectTransform;

    void Start() => UpdateTransform();
    public void ForceUpdateTransform() => UpdateTransform();
    void LateUpdate() => UpdateTransform();

    void UpdateTransform()
    {
        // Break out if anchor camera is not bound
        if (tk2dCamera == null)
            return;

        Rect rect = tk2dCamera.ScreenExtents;

        transform.anchorMin = Vector2.zero;
        transform.anchorMax = Vector2.one;
        transform.anchoredPosition = Vector2.zero;
        transform.sizeDelta = rect.size;

        transform.position =  new Vector3(rect.center.x, rect.center.y, transform.position.z);
    }

#if UNITY_EDITOR
    private void OnValidate() => UpdateTransform();
#endif
}
