using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.UI.CoroutineTween;

public static class CanvasGroupExt {

    public static void CrossFadeAlpha (this CanvasGroup group, float alpha, float duration, bool ignoreTimeScale)
    {
        TweenFloat info = new TweenFloat {
            duration = duration,
            startAlpha = group.alpha,
            targetAlpha = alpha,
            ignoreTimeScale = ignoreTimeScale
        };
        info.target = (a) => group.alpha = a;

        MonoBehaviour m_CoroutineContainer = group.GetOrAddComponent<MonoBehaviourExt> ();
//        m_CoroutineContainer.hideFlags = HideFlags.HideInInspector;

        if (m_CoroutineContainer == null)
        {
            Debug.LogWarning("Coroutine container not configured... did you forget to call Init?");
            return;
        }

        m_CoroutineContainer.StopAllCoroutines ();

        if (!m_CoroutineContainer.gameObject.activeInHierarchy)
        {
            info.TweenValue(1.0f);
            return;
        }

        var m_Tween = Start (info);
        m_CoroutineContainer.StartCoroutine(m_Tween);

//        return m_Tween;
    }

    private static IEnumerator Start(TweenFloat tweenInfo)
    {
        if (!tweenInfo.ValidTarget())
            yield break;

        var elapsedTime = 0.0f;
        while (elapsedTime < tweenInfo.duration)
        {
            elapsedTime += tweenInfo.ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
            var percentage = Mathf.Clamp01(elapsedTime / tweenInfo.duration);
            tweenInfo.TweenValue(percentage);
            yield return null;
        }
        tweenInfo.TweenValue(1.0f);
    }
        
    struct TweenFloat
    {
        public UnityAction<float> target;
        public float startAlpha;
        public float targetAlpha;

        public float duration;
        public bool ignoreTimeScale;

        public void TweenValue(float floatPercentage)
        {
            if (!ValidTarget())
                return;
            var newColor = Mathf.Lerp (startAlpha, targetAlpha, floatPercentage);
            target.Invoke (newColor);
        }

        public bool ValidTarget()
        {
            return target != null;
        }
    }
}

