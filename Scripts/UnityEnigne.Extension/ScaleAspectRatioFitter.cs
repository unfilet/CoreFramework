using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using static UnityEngine.UI.AspectRatioFitter;

namespace UnityEngine.UI
{
    [AddComponentMenu("Layout/Sale Aspect Ratio Fitter", 143)]
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    
    public class ScaleAspectRatioFitter : UIBehaviour, ILayoutSelfController
    {
        [SerializeField] private AspectMode m_AspectMode = AspectMode.None;
        public AspectMode aspectMode
        {
            get { return m_AspectMode; }
            set
            {
                if (SetStruct(ref m_AspectMode, value)) 
                    SetDirty();
            }
        }

        [SerializeField] private Vector2 _referenceSize = Vector2.one;
        public Vector2 ReferenceSize 
        { 
            get { return _referenceSize; }
            set
            {
                if (SetStruct(ref _referenceSize, value)) 
                    SetDirty();
            } 
        }

        [System.NonSerialized]
        private RectTransform m_Rect;
        private RectTransform rectTransform
        {
            get
            {
                if (m_Rect == null)
                    m_Rect = GetComponent<RectTransform>();
                return m_Rect;
            }
        }
       
        private bool m_DelayedSetDirty = false;

        private DrivenRectTransformTracker m_Tracker;

        protected ScaleAspectRatioFitter() {}

        protected override void Start()
        {
            base.Start();
            SetDirty();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetDirty();
        }

        protected override void OnDisable()
        {
            m_Tracker.Clear();
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
            base.OnDisable();
        }

        protected virtual void Update()
        {
            if (m_DelayedSetDirty)
            {
                m_DelayedSetDirty = false;
                SetDirty();
            }
        }
        
        protected override void OnRectTransformDimensionsChange()
        {
            UpdateRect();
        }
        
        protected override void OnTransformParentChanged()
        {
            UpdateRect();
        }

        private void UpdateRect()
        {
            if (!IsActive())
                return;

            m_Tracker.Clear();

            m_Tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDelta);
            
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal ,_referenceSize.x);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical ,_referenceSize.y);
            
            var aspectRatio = Mathf.Clamp(_referenceSize.x / _referenceSize.y, 0.001f, 1000f);
            
            switch (m_AspectMode)
            {
                case AspectMode.HeightControlsWidth:
                {
                    m_Tracker.Add(this, rectTransform, DrivenTransformProperties.Scale);
                    rectTransform.localScale = new Vector3(aspectRatio, 1,1);
                    break;
                }
                case AspectMode.WidthControlsHeight:
                {
                    m_Tracker.Add(this, rectTransform, DrivenTransformProperties.Scale);
                    rectTransform.localScale = new Vector3(1, 1f / aspectRatio, 1);
                    break;
                }
                case AspectMode.FitInParent:
                case AspectMode.EnvelopeParent:
                {
                    m_Tracker.Add(this, rectTransform, DrivenTransformProperties.Scale);

                    Vector2 sizeDelta = _referenceSize;
                    Vector2 parentSize = GetParentSize();
                    Vector2 aspect = new Vector2(parentSize.x / sizeDelta.x, parentSize.y / sizeDelta.y);
                    
                    if ((parentSize.y * aspectRatio < parentSize.x) ^ (m_AspectMode == AspectMode.FitInParent))
                    {
                        rectTransform.localScale = Vector3.one * aspect.x;
                    }
                    else
                    {
                        rectTransform.localScale = Vector3.one * aspect.y;
                    }
                    break;
                }
            }
        }

        private Vector2 GetParentSize()
        {
            RectTransform parent = rectTransform.parent as RectTransform;
            if (!parent)
                return Vector2.zero;
            return parent.rect.size;
        }

        public virtual void SetLayoutHorizontal() {}

        public virtual void SetLayoutVertical() {}

        protected void SetDirty()
        {
            UpdateRect();
        }

    #if UNITY_EDITOR
        protected override void OnValidate()
        {
            m_DelayedSetDirty = true;
        }
    #endif
        
        private static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
        {
            if (EqualityComparer<T>.Default.Equals(currentValue, newValue))
                return false;

            currentValue = newValue;
            return true;
        }
    }
}