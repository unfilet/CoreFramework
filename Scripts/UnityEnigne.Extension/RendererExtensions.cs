using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;

namespace UnityEngine
{
    public static class RendererExtensions
    {
        public static IEnumerable<Vector2> Corners(this Rect rect)
        {
            yield return new Vector2(rect.xMin, rect.yMin);
            yield return new Vector2(rect.xMin, rect.yMax);
            yield return new Vector2(rect.xMax, rect.yMax);
            yield return new Vector2(rect.xMax, rect.yMin);
        }

        /// <summary>
        /// Counts the bounding box corners of the given RectTransform that are visible from the given Camera in screen space.
        /// </summary>
        /// <returns>The amount of bounding box corners that are visible from the Camera.</returns>
        /// <param name="rectTransform">Rect transform.</param>
        /// <param name="camera">Camera.</param>
        private static int CountCornersVisibleFrom(this RectTransform rectTransform, Camera camera)
        {
            Rect screenBounds = new Rect(0f, 0f, Screen.width, Screen.height);
            Vector3[] objectCorners = new Vector3[4];
            rectTransform.GetWorldCorners(objectCorners);
            
            int visibleCorners = 0;
            for (var i = 0; i < objectCorners.Length; i++)
            {
                Vector3 tempScreenSpaceCorner = RectTransformUtility.WorldToScreenPoint(camera, objectCorners[i]);
                
                if (screenBounds.Contains(tempScreenSpaceCorner))
                {
                    visibleCorners++;
                }
            }

            return visibleCorners;
        }
        
        private static int CountCornersVisibleFrom(this RectTransform rectTransform, Canvas canvas)
        {
            Rect screenBounds = new Rect(0f, 0f, Screen.width, Screen.height);
            Rect rect = RectTransformExtension.GetScreenRect(rectTransform, canvas);
            return rect.Corners().Count(screenBounds.Contains);
        }

        /// <summary>
        /// Determines if this RectTransform is fully visible from the specified camera.
        /// Works by checking if each bounding box corner of this RectTransform is inside the cameras screen space view frustrum.
        /// </summary>
        /// <returns><c>true</c> if is fully visible from the specified camera; otherwise, <c>false</c>.</returns>
        /// <param name="rectTransform">Rect transform.</param>
        /// <param name="camera">Camera.</param>
        public static bool IsFullyVisibleFrom(this RectTransform rectTransform, Camera camera)
        {
            if (!rectTransform.gameObject.activeInHierarchy)
                return false;
            return CountCornersVisibleFrom(rectTransform, camera) == 4;
        }

        /// <summary>
        /// Determines if this RectTransform is at least partially visible from the specified camera.
        /// Works by checking if any bounding box corner of this RectTransform is inside the cameras screen space view frustrum.
        /// </summary>
        /// <returns><c>true</c> if is at least partially visible from the specified camera; otherwise, <c>false</c>.</returns>
        /// <param name="rectTransform">Rect transform.</param>
        /// <param name="camera">Camera.</param>
        public static bool IsVisibleFrom(this RectTransform rectTransform, Camera camera)
        {
            if (!rectTransform.gameObject.activeInHierarchy)
                return false;
            return CountCornersVisibleFrom(rectTransform, camera) > 0;
        }
    }
}