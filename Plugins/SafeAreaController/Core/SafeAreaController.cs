using System;
using UnityEngine;
using UnityEngine.Events;

namespace UI.SafeArea
{
    [Flags]
    public enum AreaUpdateTiming
    {
        OnReciveMessage = (1 << 0),
        Awake = (1 << 1),
        OnEnable = (1 << 2),
        Start = (1 << 3),
        Update = (1 << 4),
        FixedUpdate = (1 << 5),
    };

    /// <summary>
    /// Simulation device that uses safe area due to a physical notch or software home bar. For use in Editor only.
    /// </summary>
    public enum SimDevice
    {
        None,
        iPhoneX,
        iPhoneXsMax
    }

    public class SafeAreaController : Singleton<SafeAreaController>
    {

        [EnumMask]
        public AreaUpdateTiming UpdateTimming = AreaUpdateTiming.Awake;

        #region Simulation

        public static SimDevice Sim = SimDevice.None;

        static Rect[] NSA_iPhoneX = new Rect[]
            {
            new Rect (0f, 102f / 2436f, 1f, 2202f / 2436f),  // Portrait
            new Rect (132f / 2436f, 63f / 1125f, 2172f / 2436f, 1062f / 1125f)  // Landscape
            };

        static Rect[] NSA_iPhoneXsMax = new Rect[]
            {
            new Rect (0f, 102f / 2688f, 1f, 2454f / 2688f),  // Portrait
            new Rect (132f / 2688f, 63f / 1242f, 2424f / 2688f, 1179f / 1242f)  // Landscape
            };

        Rect LastSafeArea = new Rect(0, 0, 0, 0);

        #endregion

        public static UnityEvent UpdateArea = new UnityEvent();

        // Update Function
        public void UpdateSafeArea()
        {

            Rect safeArea = GetSafeArea();
            if (safeArea == LastSafeArea)
                return;
            LastSafeArea = safeArea;

            UpdateArea?.Invoke();
        }

        public static Rect GetSafeArea()
        {
            Rect safeArea = Screen.safeArea;

            if (Application.isEditor && Sim != SimDevice.None)
            {
                Rect nsa = new Rect(0, 0, Screen.width, Screen.height);

                switch (Sim)
                {
                    case SimDevice.iPhoneX:
                        if (Screen.height > Screen.width)  // Portrait
                            nsa = NSA_iPhoneX[0];
                        else  // Landscape
                            nsa = NSA_iPhoneX[1];
                        break;
                    case SimDevice.iPhoneXsMax:
                        if (Screen.height > Screen.width)  // Portrait
                            nsa = NSA_iPhoneXsMax[0];
                        else  // Landscape
                            nsa = NSA_iPhoneXsMax[1];
                        break;
                    default:
                        break;
                }

                safeArea = new Rect(Screen.width * nsa.x, Screen.height * nsa.y, Screen.width * nsa.width, Screen.height * nsa.height);
            }

            return safeArea;
        }

        // Life cycle function
        protected override void Awake()
        {
            if (haveMask(AreaUpdateTiming.Awake))
                UpdateSafeArea();
        }

        protected override void OnEnable()
        {
            if (haveMask(AreaUpdateTiming.OnEnable))
                UpdateSafeArea();
        }

        protected override void Start()
        {
            if (haveMask(AreaUpdateTiming.Start))
                UpdateSafeArea();
        }

        private void Update()
        {
            if (haveMask(AreaUpdateTiming.Update))
                UpdateSafeArea();
        }

        private void FixedUpdate()
        {
            if (haveMask(AreaUpdateTiming.FixedUpdate))
                UpdateSafeArea();
        }

        // Utility
        private bool haveMask(AreaUpdateTiming mask)
        {
            return ((int)UpdateTimming & (int)mask) != 0;
        }

        // =================================================================
        // 		Functions 4 Editor
        // =================================================================

    }
}