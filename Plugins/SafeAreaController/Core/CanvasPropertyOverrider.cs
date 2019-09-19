using System;
using UnityEngine;

namespace UI.SafeArea
{
    [RequireComponent(typeof(RectTransform))]
    public class CanvasPropertyOverrider : MonoBehaviour
    {
        [Flags]
        internal enum Direction
        {
            Left   = 1 << 0,
            Right  = 1 << 1,
            Top    = 1 << 2,
            Bottom = 1 << 3
        }

        bool HasFlag(Direction @this, Direction mask) => ((int)@this & (int)mask) != 0;
        Vector2 Divide(Vector2 a, Vector2 b) => new Vector2(a.x / b.x, a.y / b.y);

        [SerializeField] bool revers = false;
        [EnumMask, SerializeField] Direction ignore = 0;

        private void OnEnable()
        {
            SafeAreaController.UpdateArea.AddListener(UpdateCanvasProperty);
            UpdateCanvasProperty();
        }

        private void OnDisable()
        {
            SafeAreaController.UpdateArea.RemoveListener(UpdateCanvasProperty);
        }

        // Update Method
        public void UpdateCanvasProperty()
        {
            RectTransform myTransform = GetComponent<RectTransform>();
            Rect safeArea = SafeAreaController.GetSafeArea();
            Vector2 screen = new Vector2(Screen.width, Screen.height);


            Vector2 _saAnchorMin = Divide(safeArea.min, screen);
            Vector2 _saAnchorMax = Divide(safeArea.max, screen);

            if (revers)
            {
                var tt = Divide(screen, safeArea.size);
                _saAnchorMin = Vector2.zero + (Vector2.zero - _saAnchorMin) * tt;
                _saAnchorMax = Vector2.one + (Vector2.one - _saAnchorMax) * tt;
            }
            else 
            {
                _saAnchorMin = new Vector2(
                    HasFlag(ignore, Direction.Left) ? myTransform.anchorMin.x : _saAnchorMin.x,
                    HasFlag(ignore, Direction.Bottom) ? myTransform.anchorMin.y : _saAnchorMin.y
                    );

                _saAnchorMax = new Vector2(
                    HasFlag(ignore, Direction.Right) ? myTransform.anchorMax.x : _saAnchorMax.x,
                    HasFlag(ignore, Direction.Top) ? myTransform.anchorMax.y : _saAnchorMax.y
                    );
            }

            myTransform.anchorMin = _saAnchorMin;
            myTransform.anchorMax = _saAnchorMax;
        }
    }
}