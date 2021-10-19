using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

namespace TMPro
{
    [RequireComponent(typeof(LayoutGroup))]
    public class TMP_ConsistantTextSize : UIBehaviour
    {
        [MinMaxRange(0, 1000)]
        [SerializeField] RangedFloat _fontSize = new RangedFloat(0,170);

        public TMP_Text[] linkedLabels;

        private float preferredWidth;
        private LayoutGroup layoutGroup;

        protected override void Awake()
        {
            base.Awake();

            layoutGroup = GetComponent<LayoutGroup>();
            linkedLabels = GetComponentsInChildren<TMP_Text>(true);

            this.transform.OnTransformChildrenChangedAsObservable()
                .Subscribe(_ =>
                {
                    linkedLabels = GetComponentsInChildren<TMP_Text>();
                })
                .AddTo(this);
        }

        private void LateUpdate()
        {
            float pw = linkedLabels.Sum(l => l.preferredWidth);
            if (preferredWidth == pw) return;

            preferredWidth = pw;

            if (preferredWidth <= 0) return;

            Apply();
        }

        private void Apply()
        {
            if (linkedLabels == null || linkedLabels.Length < 1) return;

            TMP_Text candidate = linkedLabels[0];

            foreach (var item in linkedLabels)
            {
                if (!item.gameObject.activeSelf)
                    continue;

                if (item.preferredWidth > candidate.preferredWidth)
                    candidate = item;
            }

            if (!candidate) return;

            candidate.enableAutoSizing = true;
            candidate.fontSizeMin = _fontSize.minValue;
            candidate.fontSizeMax = _fontSize.maxValue;
            candidate.ForceMeshUpdate();
            float optimumPointSize = candidate.fontSize;

            foreach (var item in linkedLabels)
            {
                item.enableAutoSizing = false;
                item.fontSize = optimumPointSize;
            }
        }
    }
}