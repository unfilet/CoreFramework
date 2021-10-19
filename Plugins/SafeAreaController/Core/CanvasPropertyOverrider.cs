using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

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

        [SerializeField] private bool revers = false;
        [SerializeField] private Direction ignore = 0;
        
        private SafeAreaController _safeAreaController;
        private  RectTransform rectTransform => transform as RectTransform;
        private Vector2 _cachedMin;
        private Vector2 _cachedMax;
        
        [Inject]
        private void Construct(SafeAreaController controller)
        {
            _safeAreaController = controller;
        }

        private void Awake()
        {
            _cachedMin = rectTransform.anchorMin;
            _cachedMax = rectTransform.anchorMax;
            _safeAreaController.OnSafeAreaChange
                .Subscribe(UpdateCanvasProperty)
                .AddTo(this);
        }

        bool HasFlag(Direction @this, Direction mask) => ((int)@this & (int)mask) != 0;
        Vector2 Divide(Vector2 a, Vector2 b) => new Vector2(a.x / b.x, a.y / b.y);

        // Update Method
        private void UpdateCanvasProperty(Rect safeArea)
        {
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
                    HasFlag(ignore, Direction.Left)   ? _cachedMin.x : _saAnchorMin.x,
                    HasFlag(ignore, Direction.Bottom) ? _cachedMin.y : _saAnchorMin.y
                );

                _saAnchorMax = new Vector2(
                    HasFlag(ignore, Direction.Right) ? _cachedMax.x : _saAnchorMax.x,
                    HasFlag(ignore, Direction.Top)   ? _cachedMax.y : _saAnchorMax.y
                );
            }

            rectTransform.anchorMin = _saAnchorMin;
            rectTransform.anchorMax = _saAnchorMax;
        }
    }
}