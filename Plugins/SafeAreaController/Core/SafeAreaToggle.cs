using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UI.SafeArea
{
    public class SafeAreaToggle : MonoBehaviour
    {
        [SerializeField] KeyCode KeySafeArea = KeyCode.A;

#if UNITY_EDITOR

        SimDevice[] Sims;
        int SimIdx;

        void Awake ()
        {
            if (!Application.isEditor)
                Destroy (this);

            Sims = (SimDevice[])Enum.GetValues (typeof (SimDevice));
        }

        void Update ()
        {
            if (Input.GetKeyDown (KeySafeArea))
                ToggleSafeArea ();
        }

        /// <summary>
        /// Toggle the safe area simulation device.
        /// </summary>
        void ToggleSafeArea ()
        {
            SimIdx = ++SimIdx % Sims.Length;
            SafeAreaController.Sim = Sims[SimIdx];
            SafeAreaController.Instance.UpdateSafeArea();
        }

        [MenuItem("Hexart/Toggle SafeArea")]
        static void static_toggleSafeArea()
        {
            var Sims = (SimDevice[])Enum.GetValues(typeof(SimDevice));
            int SimIdx = Array.IndexOf(Sims, SafeAreaController.Sim);
            SimIdx = ++SimIdx % Sims.Length;
            SafeAreaController.Sim = Sims[SimIdx];
            SafeAreaController.Instance.UpdateSafeArea();
        }
#endif
    }
}
