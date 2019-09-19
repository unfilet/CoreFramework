using System;

[Serializable]
public struct RangedFloat
{
    public float minValue;
    public float maxValue;

    public float Length {
        get { return maxValue - minValue; }
    }

    public RangedFloat(float min = 1, float max = 1)
    {
        minValue = min; maxValue = max;
    }
}