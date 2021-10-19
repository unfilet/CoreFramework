using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.UI.AspectRatioFitter;

public abstract class ScaleRelativeToCameraBase : UIBehaviour
{
    [SerializeField] protected Camera cam;
    [SerializeField] protected Vector3 offset;
    [SerializeField] private AspectMode m_AspectMode = AspectMode.None;
    public AspectMode aspectMode
    {
        get { return m_AspectMode; }
        set
        {
            if (SetStruct(ref m_AspectMode, value))
                SetDirty();
        }
    }

    private bool m_DelayedSetDirty = false;

    protected abstract void UpdateRect();

    protected virtual Vector2 GetParentSize()
    {
        Vector2 parentSize = Vector2.zero;

        if (cam.orthographic)
            parentSize = OrthographicSize(cam);
        else
            parentSize = new Vector2(
                FrustumWidthAtDistance(cam, offset.z),
                FrustumHeightAtDistance(cam, offset.z));

        return parentSize;
    }

    protected Rect GetParentRect()
    {
        Rect parentRect = new Rect();

        if (cam.orthographic)
            parentRect.size = OrthographicSize(cam);
        else
            parentRect.size = new Vector2(
                FrustumWidthAtDistance(cam, offset.z),
                FrustumHeightAtDistance(cam, offset.z));

        parentRect.center = cam.transform.position;

        return parentRect;
    }
   

    protected override void Awake()
    {
        base.Awake();
        if (!cam) cam = GetComponent<Camera>();
    }

    protected override void Start()
    {
        base.Start();
        SetDirty();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        SetDirty();
    }

    protected virtual void Update()
    {
        if (m_DelayedSetDirty)
        {
            m_DelayedSetDirty = false;
            SetDirty();
        }
    }

    protected void SetDirty()
    {
        if (!cam || !gameObject.activeInHierarchy)
            return;
        UpdateRect();
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        m_DelayedSetDirty = true;
    }
#endif

    protected static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
    {
        if (EqualityComparer<T>.Default.Equals(currentValue, newValue))
            return false;

        currentValue = newValue;
        return true;
    }

    #region CameraUtils

    protected Vector2 OrthographicSize(Camera camera)
        => new Vector2(
            2.0f * camera.orthographicSize * camera.aspect,
            2.0f * camera.orthographicSize);

    // Calculate the frustum height at a given distance from the camera.
    protected float FrustumHeightAtDistance(Camera camera, float distance)
        => 2.0f * distance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);

    protected float FrustumWidthAtDistance(Camera camera, float distance)
        => FrustumHeightAtDistance(camera, distance) * camera.aspect;

    // Calculate the FOV needed to get a given frustum height at a given distance.
    protected float FOVForHeightAndDistance(float height, float distance)
    {
        return 2.0f * Mathf.Atan(height * 0.5f / distance) * Mathf.Rad2Deg;
    }

    #endregion
}
