using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

namespace TMPro
{
    [CreateAssetMenu(menuName = "TextMeshPro/TMP_DateYearInputValidator")]
    [Serializable]
    public class TMP_DateYearInputValidator : TMP_InputValidator
    {
        public override char Validate(ref string text, ref int pos, char ch)
        {
            if (ch < '0' && ch > '9') return (char)0;
            if (text.Length >= 4) return (char)0;

            string replaceString = ch.ToString();
            string tmp = text.Insert(pos, replaceString);

            int length = tmp.Length;
            int value = int.Parse(tmp);

            int startDate = (int) (1970 / Math.Pow(10, 4 - length));
            int endDate = (int) (DateTime.Now.Year / Math.Pow(10, 4 - length));

            if (value < startDate || value > endDate)
                return (char)0;
#if UNITY_EDITOR
            text = tmp;
#endif
            pos++;

            return ch;
        }
    }
}