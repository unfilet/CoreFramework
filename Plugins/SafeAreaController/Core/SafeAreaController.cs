using System;
using UniRx;
using UnityEngine;
using Zenject;

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

    public class SafeAreaController : MonoBehaviour
    {
        public IObservable<Rect> OnSafeAreaChange => _safeArea;
        
        [SerializeField]
        private AreaUpdateTiming UpdateTimming = AreaUpdateTiming.Awake;
        
        private RectReactiveProperty _safeArea;

        [Inject]
        private void Construct()
        {
            _safeArea = new RectReactiveProperty(Screen.safeArea);
        }

        // Update Function
        private void UpdateSafeArea(AreaUpdateTiming mask)
        {
            if (haveMask(mask))
            {
                var safeArea = Screen.safeArea;
#if UNITY_IOS
                if (!Mathf.Approximately(Screen.height, safeArea.height))
                    safeArea.height += 40; //status bar height
#endif   
                _safeArea.SetValueAndForceNotify(safeArea);
            }
        }
        
        protected void Awake() 
            => UpdateSafeArea(AreaUpdateTiming.Awake);

        protected void OnEnable() 
            => UpdateSafeArea(AreaUpdateTiming.OnEnable);

        protected void Start() 
            => UpdateSafeArea(AreaUpdateTiming.Start);

        private void Update() 
            => UpdateSafeArea(AreaUpdateTiming.Update);

        private void FixedUpdate() 
            => UpdateSafeArea(AreaUpdateTiming.FixedUpdate);

        private bool haveMask(AreaUpdateTiming mask)
        {
            return ((int)UpdateTimming & (int)mask) != 0;
        }
    }
}