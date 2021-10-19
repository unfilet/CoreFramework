using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.AspectRatioFitter;

public class ScaleQuadRelativeToCamera : ScaleRelativeToCameraBase
{
    private Renderer m_renderer;
    private new Renderer renderer
            => m_renderer ?? (m_renderer = GetComponent<Renderer>());

    protected override void UpdateRect()
    {
        if (renderer == null)
            return;

        Texture texture = renderer.material.mainTexture;
        float textureAspect = (float)texture.width / texture.height;

        Vector2 size = new Vector2(textureAspect, 1);

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

                Vector2 aspect = new Vector2(
                    parentSize.x / size.x,
                    parentSize.y / size.y);

                float scale = 1f;
                if (aspectMode == AspectMode.FitInParent)
                    scale = Mathf.Min(aspect.x, aspect.y);
                else
                    scale = Mathf.Max(aspect.x, aspect.y);

                renderer.transform.localScale = size * scale;

                break;
        }
    }

   
}
