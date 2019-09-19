using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.SafeArea
{
    [RequireComponent(typeof(Camera))]
    public class CameraPropertyOverrider : MonoBehaviour
    {

        public bool isSafeAreaCamera = true;
        public bool useFullSize = true;

        private Camera myCamera;
        private Rect selfSize
        {
            get { return myCamera.rect; }
        }

        private Rect _safeSize
        {
            get
            {

                var safeArea = SafeAreaController.GetSafeArea();

                float safeX = safeArea.x / Screen.width;
                float safeY = safeArea.y / Screen.height;
                float safeW = safeArea.width / Screen.width;
                float safeH = safeArea.height / Screen.height;

                Rect originalRect = useFullSize ? new Rect(0, 0, 1, 1) : selfSize;

                return new Rect(safeX + originalRect.x / safeW,
                                safeY + originalRect.y / safeH,
                                safeW / originalRect.width,
                                safeH / originalRect.height);
            }
        }

        private void OnEnable()
        {
            SafeAreaController.UpdateArea.AddListener(UpdateCameraProperty);
            UpdateCameraProperty();
        }

        private void OnDisable()
        {
            SafeAreaController.UpdateArea.RemoveListener(UpdateCameraProperty);
        }

        // Update Method
        private void UpdateCameraProperty()
        {

            if (myCamera == null)
            {
                myCamera = GetComponent<Camera>();
            }

            if (isSafeAreaCamera)
            {
                myCamera.rect = _safeSize;
            }
            else
            {
                myCamera.rect = useFullSize ? new Rect(0, 0, 1, 1) : selfSize;
            }
        }
    }
}
