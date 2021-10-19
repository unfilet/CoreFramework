using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class PlatformChecker : MonoBehaviour
{
    [SerializeField] RuntimePlatform platform;
    [Space]
    public UnityEvent onCall;

    void Awake()
    {
        if (platform.HasFlag(Application.platform))
            onCall?.Invoke();
    }
}
