﻿using System;
using System.Collections.Generic;
using UnityEngine;

using AspectMode = UnityEngine.UI.AspectRatioFitter.AspectMode;

[RequireComponent(typeof(SpriteRenderer))]
public class ScaleRelativeToCamera : MonoBehaviour 
{
    [SerializeField] Camera cam;
    [SerializeField] tk2dCamera _tk2dCamera;

    [SerializeField] AspectMode m_AspectMode = AspectMode.None;
    [SerializeField] Vector3 offset;

    private SpriteRenderer m_renderer;
    private new SpriteRenderer renderer
            => m_renderer ?? (m_renderer = GetComponent<SpriteRenderer>());
  
    private void UpdateRect ()
    {
        if (!this.gameObject.activeInHierarchy || renderer == null || renderer.sprite == null)
            return;

        Vector2 size = renderer.sprite.bounds.size;
        Vector2 pivot = new Vector2(
            renderer.sprite.pivot.x / renderer.sprite.rect.width,
            renderer.sprite.pivot.y / renderer.sprite.rect.height);
        pivot -= Vector2.one * 0.5f;

        Vector3 position = cam.transform.position
            + cam.transform.rotation * offset;

        renderer.transform.position = position;
        renderer.transform.rotation = Quaternion.LookRotation(
            position - cam.transform.position,
            Vector3.up
            );

        switch (this.m_AspectMode)
        {
            case AspectMode.None:
            case AspectMode.WidthControlsHeight:
            case AspectMode.HeightControlsWidth:
                break;
            case AspectMode.FitInParent:
            case AspectMode.EnvelopeParent:

                Vector2 parentSize = Vector2.zero;

                if (cam.orthographic)
                {
                    if (_tk2dCamera != null)
                        parentSize = OrthographicSize(_tk2dCamera);
                    else
                        parentSize = OrthographicSize(cam);
                }
                else
                    parentSize = new Vector2(
                        FrustumWidthAtDistance(cam, offset.z),
                        FrustumHeightAtDistance(cam, offset.z));

                Vector2 aspect = new Vector2(
                    parentSize.x / size.x,
                    parentSize.y / size.y);

                float scale = 1f;
                if (m_AspectMode == AspectMode.FitInParent)
                    scale = Mathf.Min(aspect.x, aspect.y);
                else
                    scale = Mathf.Max(aspect.x, aspect.y);

                renderer.transform.localScale = Vector3.one * scale;
                renderer.transform.position += Vector3.Scale(pivot, parentSize);
                

                break;
        }
    }


    Vector2 OrthographicSize(tk2dCamera camera)
        => camera.ScreenExtents.size;

    Vector2 OrthographicSize(Camera camera)
        => new Vector2(
            2.0f * camera.orthographicSize * camera.aspect,
            2.0f * camera.orthographicSize);

    // Calculate the frustum height at a given distance from the camera.
    float FrustumHeightAtDistance(Camera camera, float distance)
        => 2.0f * distance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);

    float FrustumWidthAtDistance(Camera camera, float distance)
        => FrustumHeightAtDistance(camera, distance) * camera.aspect;

    // Calculate the FOV needed to get a given frustum height at a given distance.
    float FOVForHeightAndDistance(float height, float distance) {
        return 2.0f * Mathf.Atan(height * 0.5f / distance) * Mathf.Rad2Deg;
    }


    protected void OnEnable() => this.SetDirty();
    protected void SetDirty() => UpdateRect();
#if UNITY_EDITOR
    protected void OnValidate() => this.SetDirty();
#endif
}