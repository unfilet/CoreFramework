using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

namespace UI.SafeArea
{
    [RequireComponent(typeof(Camera))]
    public class CameraPropertyOverrider : MonoBehaviour
    {
        public bool isSafeAreaCamera = true;
        public bool useFullSize = true;

        private SafeAreaController _safeAreaController;
        private Camera _camera;

        [Inject]
        private void Construct(SafeAreaController controller)
        {
            _camera = GetComponent<Camera>();
            _safeAreaController = controller;
        }

        private void Awake()
        {
            _safeAreaController.OnSafeAreaChange
                .Subscribe(UpdateCameraProperty)
                .AddTo(this);
        }

        private Rect GetSafeSize(Rect safeArea)
        {
            float safeX = safeArea.x / Screen.width;
            float safeY = safeArea.y / Screen.height;
            float safeW = safeArea.width / Screen.width;
            float safeH = safeArea.height / Screen.height;

            Rect originalRect = useFullSize ? new Rect(0, 0, 1, 1) : _camera.rect;

            return new Rect(safeX + originalRect.x / safeW,
                            safeY + originalRect.y / safeH,
                            safeW / originalRect.width,
                            safeH / originalRect.height);
        }

        // Update Method
        private void UpdateCameraProperty(Rect safeArea)
        {
            if (isSafeAreaCamera)
                _camera.rect = GetSafeSize(safeArea);
            else
                _camera.rect = useFullSize ? new Rect(0, 0, 1, 1) : _camera.rect;
        }
    }
}
