using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using System.Text.RegularExpressions;

namespace Core.Scripts.Utils
{
    [Serializable] public class UnityEventString : UnityEvent<string> { }
    [Serializable] public class UnityEventSprite : UnityEvent<Sprite> { }
    [Serializable] public class UnityEventInt : UnityEvent<int> { }
    [Serializable] public class UnityEventFloat : UnityEvent<float> { }
    [Serializable] public class UnityEventBool : UnityEvent<bool> { }
    [Serializable] public class UnityEventAudioClip : UnityEvent<AudioClip, float> { }
    [Serializable] public class UnityEventTransform : UnityEvent<Transform> { }

    public static class HelperClass
	{
        public static bool IsEmailValid (string email) {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            string pattern =
            @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
            + "@"
            + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))\z";
            return Regex.IsMatch (email, pattern);
        }

        /// <summary>
        /// Ises the date valid for format dd.MM.YYYY.
        /// </summary>
        /// <returns><c>true</c>, if date valid was ised, <c>false</c> otherwise.</returns>
        /// <param name="date">Date.</param>
        public static bool IsDateValid (string date) {
            string pattern = @"^(3[01]|[1-2][0-9]|0?[1-9])\.(1[0-2]|0?[1-9])\.((?:19|20)\d{2})$";
            return Regex.IsMatch(date, pattern);
        }

		public static T GetRandomNumber<T> (IDictionary<T, int> aNumbers)
		{
			int sum = aNumbers.Values.Sum ();
			int weight = UnityEngine.Random.Range (0, sum);
			T[] Keys = aNumbers.Keys.ToArray ();
			for (int i = Keys.Length - 1; i >= 0; i--)
				if ((sum -= aNumbers [Keys [i]]) <= weight)
					return Keys [i];
			return default(T);
		}

		public static T GetRandomNumber<T> (IDictionary<T, float> aNumbers)
		{
			float weight = UnityEngine.Random.value;
			T[] Keys = aNumbers.Keys.ToArray ();
			float sum = 0;
			for (int i = 0; i < Keys.Length; i++)
				if ((sum += aNumbers[Keys[i]]) >= weight)
					return Keys[i];
			return default(T);
		}
            
        public static T GetInactiveOrCreate<T> (ICollection<T> collection, T prefab, Transform parent, bool activeSelf = true) where T : Component
		{
			T retval = null;
			try {
                retval = collection.First ((arg) => arg != null && !(activeSelf ? arg.gameObject.activeSelf : arg.gameObject.activeInHierarchy));
			} catch {
				retval = GameObject.Instantiate<T> (prefab);
				collection.Add (retval);
			}

			retval.transform.SetParent (parent);
			retval.transform.localScale = Vector3.one;
            retval.transform.localPosition = Vector3.zero;
            retval.transform.localRotation = Quaternion.identity;

			retval.gameObject.SetActive (true);
			return retval;
		}

		public static Color MakeGrayscale (this Color c) {
//			float gray = (0.21f * c.r + 0.71f * c.g + 0.071f * c.b);
//			float gray = (c.r + c.g + c.b) / 3.0f;
			float gray = (c.r * 0.3f + c.g * 0.59f + c.b * 0.11f);
			return new Color (gray,gray,gray,c.a);
		}
	}
}
/*

// Returns a random number between 0.0 and 1.0 (inclusive).
#define RANDOM_FLOAT() ((float)arc4random()/0xFFFFFFFFu)

#define RANDOM_INT(n) (arc4random() % (n))


*/