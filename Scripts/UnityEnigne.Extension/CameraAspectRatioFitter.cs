using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.AspectRatioFitter;

[ExecuteAlways]
public class CameraAspectRatioFitter : ScaleRelativeToCameraBase
{
    [SerializeField] private int _pixelPerMeter = 100;
    [SerializeField] private Vector2 _referenceSize = Vector2.one;
    public Vector2 ReferenceSize
    {
        get { return _referenceSize; }
        set
        {
            if (SetStruct(ref _referenceSize, value))
                SetDirty();
        }
    }

    protected override Vector2 GetParentSize()
    {
        return  new Vector2(Screen.width, Screen.height) * 1f/_pixelPerMeter;
    }

    protected override void UpdateRect()
    {
        if (!cam.orthographic)
            return;


        Vector2 sizeDelta = _referenceSize;
        Vector2 parentSize = GetParentSize();
        var screenAspect = Mathf.Clamp(parentSize.x / parentSize.y, 0.001f, 1000f);
        var aspectRatio = Mathf.Clamp(sizeDelta.x / sizeDelta.y, 0.001f, 1000f);
        var orthographicSize = parentSize.y / 2f;

        switch (aspectMode)
        {
            case AspectMode.FitInParent:
            case AspectMode.EnvelopeParent:
                {
                    Vector2 aspect = new Vector2(parentSize.x / sizeDelta.x, parentSize.y / sizeDelta.y);

                    if ((parentSize.y * aspectRatio < parentSize.x) ^ (aspectMode == AspectMode.FitInParent))
                    {
                        orthographicSize /= aspect.x;
                    }
                    else
                    {
                        orthographicSize /= aspect.y;
                    }


                    break;
                }
        }

        cam.orthographicSize = orthographicSize;
    }
}
