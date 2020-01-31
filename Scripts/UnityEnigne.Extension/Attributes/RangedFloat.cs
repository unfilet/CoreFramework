using System;
using UnityEngine;

[Serializable]
public struct RangedFloat
{
    public static RangedFloat Value(float min = float.MinValue, float max = float.MaxValue)
        => new RangedFloat(min,max);

    public float minValue;
    public float maxValue;

    public float Length => maxValue - minValue;

    public RangedFloat(float min = 1, float max = 1)
    {
        minValue = min; maxValue = max;
    }

    public bool InRange(float value)
        => minValue <= value && value <= maxValue;

    public float Lerp(float t)
        => Mathf.Lerp(minValue, maxValue, t);

    public float Random()
        => UnityEngine.Random.Range(minValue, maxValue);
}