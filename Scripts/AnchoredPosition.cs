using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnchoredPosition : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] SpriteAlignment pivot;
    [SerializeField] Vector3 offset;
    [SerializeField] bool useRotate;

    private void Awake()
    {
    }

    private void UpdatePosition()
    {
        if (!this.gameObject.activeInHierarchy)
            return;

        Vector2 parentSize = Vector2.zero;
        if (cam.orthographic)
            parentSize = new Vector2(
                OrthographicWidth(cam),
                OrthographicHeight(cam));
        else
            parentSize = new Vector2(
                FrustumWidthAtDistance(cam, offset.z),
                FrustumHeightAtDistance(cam, offset.z));

        Vector2 scale = Vector2.zero;
        switch (pivot)
        {
            case SpriteAlignment.TopLeft:
            case SpriteAlignment.TopCenter:
            case SpriteAlignment.TopRight:
                scale.y = 0.5f;
                break;
            case SpriteAlignment.BottomLeft:
            case SpriteAlignment.BottomCenter:
            case SpriteAlignment.BottomRight:
                scale.y = -0.5f;
                break;
            default:
                scale.y = 0;
                break;
        }

        switch (pivot)
        {
            case SpriteAlignment.TopLeft:
            case SpriteAlignment.BottomLeft:
            case SpriteAlignment.LeftCenter:
                scale.x = -0.5f;
                break;
            case SpriteAlignment.TopRight:
            case SpriteAlignment.BottomRight:
            case SpriteAlignment.RightCenter:
                scale.x = 0.5f;
                break;
            default:
                scale.x = 0;
                break;
        }


        Vector3 position = cam.transform.position +
            cam.transform.rotation * offset +
            Vector3.Scale(parentSize, scale);
        this.transform.position = position;

        Quaternion rotation = Quaternion.identity;
        if (useRotate)
            rotation = Quaternion.LookRotation(
                position - cam.transform.position,
                Vector3.up
                );
        this.transform.rotation = rotation;
    }


    protected void OnEnable() => this.SetDirty();
    protected void SetDirty() => UpdatePosition();
#if UNITY_EDITOR
    protected void OnValidate() => this.SetDirty();
#endif


    float OrthographicWidth(Camera camera)
        => OrthographicHeight(camera) * camera.aspect;

    // Calculate the frustum height at a given distance from the camera.
    float OrthographicHeight(Camera camera)
        => 2.0f * camera.orthographicSize;

    // Calculate the frustum height at a given distance from the camera.
    float FrustumHeightAtDistance(Camera camera, float distance)
        => 2.0f * distance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);

    float FrustumWidthAtDistance(Camera camera, float distance)
        => FrustumHeightAtDistance(camera, distance) * camera.aspect;

    // Calculate the FOV needed to get a given frustum height at a given distance.
    float FOVForHeightAndDistance(float height, float distance)
    {
        return 2.0f * Mathf.Atan(height * 0.5f / distance) * Mathf.Rad2Deg;
    }

}
