using System;
using Photon.Realtime;
using UnityEngine;

namespace UniRx.Triggers
{
    public static partial class ObservableTriggerExtensions
    {
        #region IConnectionCallbacks
        public static IObservable<Unit> OnConnectedToMasterAsObservable(this Component component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<Unit>();
            return component.GetOrAddComponent<ObservablePhotonPunCallbacks>()
                .OnConnectedToMasterAsObservable();
        }
        #endregion
        
        #region IMatchmakingCallbacks
        public static IObservable<Unit> OnLeftRoomAsObservable(this Component component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<Unit>();
            return component.GetOrAddComponent<ObservablePhotonPunCallbacks>()
                .OnLeftRoomAsObservable();
        }
        #endregion

        #region IInRoomCallbacks
        public static IObservable<Player> OnPlayerLeftRoomAsObservable(this Component component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<Player>();
            return component.GetOrAddComponent<ObservablePhotonPunCallbacks>()
                .OnPlayerLeftRoomAsObservable();
        }
        
        public static IObservable<Player> OnPlayerEnteredRoomAsObservable(this Component component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<Player>();
            return component.GetOrAddComponent<ObservablePhotonPunCallbacks>()
                .OnPlayerEnteredRoomAsObservable();
        }
        #endregion
    }
}