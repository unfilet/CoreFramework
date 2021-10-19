using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomPropertyDrawer(typeof(RangedFloat), true)]
public class RangedFloatDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label = EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, label);

        SerializedProperty minProp = property.FindPropertyRelative("minValue");
        SerializedProperty maxProp = property.FindPropertyRelative("maxValue");

        float minValue = minProp.floatValue;
        float maxValue = maxProp.floatValue;

        float rangeMin = 0;
        float rangeMax = 1;
        bool whole = false;

        var ranges = (MinMaxRangeAttribute[])fieldInfo.GetCustomAttributes(typeof (MinMaxRangeAttribute), true);
        if (ranges.Length > 0)
        {
            rangeMin = ranges[0].Min;
            rangeMax = ranges[0].Max;
            whole = ranges[0].WholeNumber;
        }
        const float rangeBoundsLabelWidth = 40f;

        var rangeBoundsLabel1Rect = new Rect(position);
        rangeBoundsLabel1Rect.width = rangeBoundsLabelWidth;

        if (whole)
        {
            minProp.floatValue = EditorGUI.IntField(rangeBoundsLabel1Rect, new GUIContent(), (int)minValue);
            position.xMin += rangeBoundsLabelWidth + 2f;

            var rangeBoundsLabel2Rect = new Rect(position);
            rangeBoundsLabel2Rect.xMin = rangeBoundsLabel2Rect.xMax - rangeBoundsLabelWidth;

            maxProp.floatValue = EditorGUI.IntField(rangeBoundsLabel2Rect, new GUIContent(), (int)maxValue);
            position.xMax -= rangeBoundsLabelWidth + 2f;
        }
        else
        {
            minProp.floatValue = EditorGUI.FloatField(rangeBoundsLabel1Rect, new GUIContent(), minValue);
            position.xMin += rangeBoundsLabelWidth + 2f;

            var rangeBoundsLabel2Rect = new Rect(position);
            rangeBoundsLabel2Rect.xMin = rangeBoundsLabel2Rect.xMax - rangeBoundsLabelWidth;

            maxProp.floatValue = EditorGUI.FloatField(rangeBoundsLabel2Rect, new GUIContent(), maxValue);
            position.xMax -= rangeBoundsLabelWidth + 2f;
        }

        EditorGUI.BeginChangeCheck();
        EditorGUI.MinMaxSlider(position, ref minValue, ref maxValue, rangeMin, rangeMax);

        if (whole)
        {
            minValue = Mathf.RoundToInt(minValue);
            maxValue = Mathf.RoundToInt(maxValue);
        }

        if (EditorGUI.EndChangeCheck())
        {
            minProp.floatValue = minValue;
            maxProp.floatValue = maxValue;
        }

        EditorGUI.EndProperty();
    }
}