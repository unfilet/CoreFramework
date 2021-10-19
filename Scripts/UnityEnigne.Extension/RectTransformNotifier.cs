using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Core.Scripts
{
    using Utils;


    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public class RectTransformNotifier : UIBehaviour
    {
        public UnityEvent OnEnableCall = default;
        public UnityEvent OnDisableCall = default;

        public UnityEventFloat OnRectHeightChange = default;
        public UnityEventFloat OnRectWidthChange = default;

        [NonSerialized]
        private RectTransform m_Rect;
        private RectTransform rectTransform => m_Rect ?? (m_Rect = this.GetComponent<RectTransform>());

        /// <summary>
        ///   <para>Mark the ContentSizeFitter as dirty.</para>
        /// </summary>
        protected void SetDirty()
        {
            if (IsActive())
            {
                OnRectWidthChange?.Invoke(rectTransform.rect.size.x);
                OnRectHeightChange?.Invoke(rectTransform.rect.size.y);
            }
        }


        protected override void OnEnable()
        {
            base.OnEnable();
            OnEnableCall?.Invoke();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            OnDisableCall?.Invoke();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            SetDirty();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            SetDirty();
        }
#endif
    }
}