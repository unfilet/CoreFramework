using System;

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class MinMaxRangeAttribute : Attribute
{
    public MinMaxRangeAttribute() : this(0, 1) { }

    public MinMaxRangeAttribute(float min, float max, bool wholeNumber = false)
    {
        Min = min;
        Max = max;
        WholeNumber = wholeNumber;
    }
    public bool WholeNumber { get; private set; }
    public float Min { get; private set; }
    public float Max { get; private set; }
}