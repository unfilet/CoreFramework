using System;
using System.Collections.Generic;
using UnityEngine;

using AspectMode = UnityEngine.UI.AspectRatioFitter.AspectMode;

[RequireComponent(typeof(SpriteRenderer))]
public class ScaleRelativeToCamera : ScaleRelativeToCameraBase
{
    private SpriteRenderer m_renderer;
    private new SpriteRenderer renderer
            => m_renderer ?? (m_renderer = GetComponent<SpriteRenderer>());
  
    protected override void UpdateRect ()
    {
        if (renderer == null || renderer.sprite == null)
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

        switch (aspectMode)
        {
            case AspectMode.None:
            case AspectMode.WidthControlsHeight:
            case AspectMode.HeightControlsWidth:
                break;
            case AspectMode.FitInParent:
            case AspectMode.EnvelopeParent:

                Vector2 parentSize = GetParentSize();
                var aspectRatio = Mathf.Clamp(size.x / size.y, 0.001f, 1000f);

                Vector2 aspect = new Vector2(
                    parentSize.x / size.x,
                    parentSize.y / size.y);

                var scale = Vector3.one;
                
                if ((parentSize.y * aspectRatio < parentSize.x) ^ (aspectMode == AspectMode.FitInParent))
                    scale *= aspect.x;
                else
                    scale *= aspect.y;

                renderer.transform.localScale = scale;
                renderer.transform.position += Vector3.Scale(pivot, parentSize);
                break;
        }
    }

}