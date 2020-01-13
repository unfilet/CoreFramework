using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using AspectMode = UnityEngine.UI.AspectRatioFitter.AspectMode;

[ExecuteAlways]
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class SkeletoneAspectRatioFitter : UIBehaviour, ILayoutSelfController, ILayoutController
{ 
    [SerializeField] 
    private Vector2 m_ViewportResolution = Vector2.zero;
    [SerializeField]
    private AspectMode m_AspectMode = AspectMode.None;
    [SerializeField]
    private Vector2 m_offset = Vector2.zero;
    [NonSerialized]
    private RectTransform m_Rect;

    /// <summary>
    ///   <para>The mode to use to enforce the aspect ratio.</para>
    /// </summary>
    public AspectMode aspectMode
    {
        get => this.m_AspectMode;
        set
        {
            if (m_AspectMode.Equals(value)) return;
            this.SetDirty();
        }
    }

    public float aspectRatio
        => m_ViewportResolution.x/m_ViewportResolution.y;

    private RectTransform rectTransform 
        => m_Rect ?? (m_Rect = GetComponent<RectTransform>());


    protected override void OnEnable()
    {
        base.OnEnable();
        this.SetDirty();
    }

    protected override void OnDisable()
    {
        LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
        base.OnDisable();
    }

    protected override void OnRectTransformDimensionsChange()
        => this.UpdateRect();

    private void UpdateRect()
    {
        if (!this.IsActive())
            return;

        switch (this.m_AspectMode)
        {
            case AspectMode.None:
            case AspectMode.WidthControlsHeight:
            case AspectMode.HeightControlsWidth:
                break;
            case AspectMode.FitInParent:
            case AspectMode.EnvelopeParent:

                Func<float, float, float> action = Mathf.Max;
                if (m_AspectMode == AspectMode.FitInParent)
                    action = Mathf.Min;

                Vector2 parentSize = this.GetParentSize();
                float s = action(
                    parentSize.x / m_ViewportResolution.x,
                    parentSize.y / m_ViewportResolution.y
                    );

                this.rectTransform.anchoredPosition = m_offset * s;
                this.rectTransform.localScale = Vector2.one * s;
                this.rectTransform.anchorMin = Vector2.zero;
                this.rectTransform.anchorMax = Vector2.one;
                this.rectTransform.sizeDelta = Vector2.zero;
                break;
        }
    }

    private Vector2 GetParentSize()
    {
        RectTransform parent = this.rectTransform.parent as RectTransform;
        if (!(bool)((UnityEngine.Object)parent))
            return Vector2.zero;
        return parent.rect.size;
    }

    public void SetLayoutHorizontal() {}
    public void SetLayoutVertical() {}

    protected void SetDirty() => UpdateRect();
#if UNITY_EDITOR
    protected override void OnValidate() => this.SetDirty();
#endif
}
