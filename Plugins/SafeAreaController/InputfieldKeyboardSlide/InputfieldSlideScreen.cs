using UnityEngine;
using System.Collections;

public class InputfieldSlideScreen : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private float addKeyboardHeight = 100f; 
    public bool InputFieldActive { get; set; }
    public RectTransform childRectTransform { get; set; }

    private RectTransform rectTransform => transform as RectTransform;

    void LateUpdate()
    {
        if (InputFieldActive)// && TouchScreenKeyboard.visible)
        {
            rectTransform.anchoredPosition = Vector2.zero;

            Rect rect = RectTransformExtension.GetScreenRect(childRectTransform, canvas);

            float heightPercentOfKeyboard = GetKeyboardHeightRatio() * 100f;
            float heightPercentOfInput = (Screen.height - (rect.y + rect.height)) / Screen.height * 100f;


            if (heightPercentOfKeyboard > heightPercentOfInput)
            {
                // keyboard covers input field so move screen up to show keyboard
                float differenceHeightPercent = heightPercentOfKeyboard - heightPercentOfInput;
                float newYPos = rectTransform.rect.height / 100f * differenceHeightPercent;

                Vector2 newAnchorPosition = Vector2.zero;
                newAnchorPosition.y = newYPos;
                rectTransform.anchoredPosition = newAnchorPosition;
            }
            else
            {
                // Keyboard top is below the position of the input field, so leave screen anchored at zero
                rectTransform.anchoredPosition = Vector2.zero;
            }
        }
        else
        {
            // No focus or touchscreen invisible, set screen anchor to zero
            rectTransform.anchoredPosition = Vector2.zero;
        }
        InputFieldActive = false;
    }

    private float GetKeyboardHeightRatio()
    {
#if UNITY_EDITOR
        return 0.4f + addKeyboardHeight / Screen.height; // fake TouchScreenKeyboard height ratio for debug in editor        
#elif UNITY_ANDROID        
        using (AndroidJavaClass UnityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
            AndroidJavaObject View = UnityClass.GetStatic<AndroidJavaObject>("currentActivity").Get<AndroidJavaObject>("mUnityPlayer").Call<AndroidJavaObject>("getView");
            using (AndroidJavaObject rect = new AndroidJavaObject("android.graphics.Rect")) {
                View.Call("getWindowVisibleDisplayFrame", rect);
                return (float)(Screen.height - rect.Call<int>("height") + addKeyboardHeight) / Screen.height;
            }
        }
#else
        return (TouchScreenKeyboard.area.height + addKeyboardHeight) / Screen.height;
#endif
    }
}

internal static class RectTransformExtension
{

    public static Rect GetScreenRect(RectTransform rectTransform, Canvas canvas)
    {

        Vector3[] corners = new Vector3[4];
        Vector3[] screenCorners = new Vector3[2];

        rectTransform.GetWorldCorners(corners);

        if (canvas.renderMode == RenderMode.ScreenSpaceCamera || canvas.renderMode == RenderMode.WorldSpace)
        {
            screenCorners[0] = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners[1]);
            screenCorners[1] = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners[3]);
        }
        else
        {
            screenCorners[0] = RectTransformUtility.WorldToScreenPoint(null, corners[1]);
            screenCorners[1] = RectTransformUtility.WorldToScreenPoint(null, corners[3]);
        }

        screenCorners[0].y = Screen.height - screenCorners[0].y;
        screenCorners[1].y = Screen.height - screenCorners[1].y;

        return new Rect(screenCorners[0], screenCorners[1] - screenCorners[0]);
    }

}