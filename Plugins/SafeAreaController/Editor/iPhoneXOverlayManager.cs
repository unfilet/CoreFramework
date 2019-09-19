using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace UI.SafeArea
{
    public static class iPhoneXOverlayManager
    {
        const GameViewSizeGroupType GROUP_TYPE = GameViewSizeGroupType.iOS;
        const float LANDSCAPE_ASPECT = 2436f / 1125f;
        const float PORTRAIT_ASPECT = 1125f / 2436f;

        struct OrientationMetadata
        {
            internal string sizeName;
            internal string textureName;
            internal int width, height;
        }

        /// <summary>
        /// Defines metadata for creating custom game view sizes.
        /// Texture names corespond to textures that are used to mask areas of the game view.
        /// </summary>
        static List<OrientationMetadata> metadata = new List<OrientationMetadata> {
        new OrientationMetadata {
            sizeName = "iPhone X Wide",
            textureName = "iPhoneX-landscape.psd",
            width = 2436,
            height = 1125
        },
        new OrientationMetadata {
            sizeName = "iPhone X Tall",
            textureName = "iPhoneX-portrait.psd",
            width = 1125,
            height = 2436
        }
    };

        #region Lifecycle
        [InitializeOnLoadMethod]
        static void InitializeOnLoad()
        {
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
            {
                metadata.ForEach(EnsureSizeExists);
                EnsureOverlayExists();
            }
            else
            {
                EnsureOverlayRemoved();
            }
        }

        [RuntimeInitializeOnLoadMethod]
        static void RuntimeInitializeOnLoad()
        {
            EnsureOverlayExists();
        }
        #endregion

        #region Private
        static Texture LandscapeTexture { get { return (Texture)EditorGUIUtility.Load(metadata[0].textureName); } }
        static Texture PortraitTexture { get { return (Texture)EditorGUIUtility.Load(metadata[1].textureName); } }

        static bool IsLandscapeX { get { return Mathf.Abs(GameViewUtils.AspectRatio - LANDSCAPE_ASPECT) < float.Epsilon; } }
        static bool IsPortraitX { get { return Mathf.Abs(GameViewUtils.AspectRatio - PORTRAIT_ASPECT) < float.Epsilon; } }

        static Rect GameViewRect { get { return new Rect(0, 0, Screen.width, Screen.height); } }

        static string DispatcherName { get { return typeof(OnGUIDispatcher).Name; } }
        static GameObject DispatcherGameObject { get { return GameObject.Find(DispatcherName); } }
        static OnGUIDispatcher Dispatcher { get { return DispatcherGameObject.GetComponent<OnGUIDispatcher>(); } }

        static private Vector2 resolution;

        static void DrawOverlay()
        {
            if (IsLandscapeX) GUI.DrawTexture(GameViewRect, LandscapeTexture);
            else if (IsPortraitX) GUI.DrawTexture(GameViewRect, PortraitTexture);

            var current = new Vector2(Screen.width, Screen.height);
            if (resolution != current)
            {
                resolution = current;
                GameViewUtils.SetMinGameViewScale();
            }
        }

        [MenuItem("Edit/Ensure iPhone X Overlay Exists")]
        static void EnsureOverlayExists()
        {
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
            {
                if (DispatcherGameObject == null)
                {
                    var gameObject = new GameObject("OnGUIDispatcher", typeof(OnGUIDispatcher));
                    gameObject.hideFlags = HideFlags.DontSave | HideFlags.HideInInspector;
                }
                Dispatcher.OnGUIEvent += DrawOverlay;
            }
        }

        static void EnsureOverlayRemoved()
        {
            if (DispatcherGameObject == null) return;
            Object.DestroyImmediate(DispatcherGameObject);
        }

        static void EnsureSizeExists(OrientationMetadata orientationMetadata)
        {
            if (GameViewUtils.SizeExists(GROUP_TYPE, orientationMetadata.width, orientationMetadata.height)) return;
            GameViewUtils.AddCustomSize(GameViewUtils.GameViewSizeType.FixedResolution, GROUP_TYPE,
                                        orientationMetadata.width,
                                        orientationMetadata.height,
                                        orientationMetadata.sizeName);
        }
        #endregion
    }
}