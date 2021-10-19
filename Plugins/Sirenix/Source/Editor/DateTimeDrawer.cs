using System;
using System.Globalization;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;

public class DateTimeDrawer : OdinValueDrawer<DateTime>
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        var _dateTime = ValueEntry.SmartValue;

        var _rect = EditorGUILayout.GetControlRect();

        if (label != null)
        {
            _rect = EditorGUI.PrefixLabel(_rect, label);
        }

        var _result = EditorGUI.TextField(_rect, _dateTime.ToString("u"));
        if (DateTime.TryParse(_result, null, DateTimeStyles.RoundtripKind, out _dateTime))
        {
            ValueEntry.SmartValue = _dateTime;
        }
    }
}