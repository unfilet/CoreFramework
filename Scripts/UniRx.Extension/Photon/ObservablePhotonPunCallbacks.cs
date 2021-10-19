using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Photon.Pun;
using Photon.Realtime;

namespace UniRx.Triggers
{
    public class PhotonException<T> : Exception
    {
        public readonly T Status;
        public PhotonException(T status, string message) : base(message) 
            => this.Status = status;
    }
    
    public class PhotonDisconnectException : PhotonException<DisconnectCause>
    {
        public PhotonDisconnectException(DisconnectCause status, string message = null) : base(status, message)
        { }
    }
    
    public class PhotonJoinRoomException : PhotonException<short>
    {
        public PhotonJoinRoomException(short status, string message) : base(status, message)
        { }
    }

    public class PhotonCreateRoomException : PhotonException<short>
    {
        public PhotonCreateRoomException(short status, string message) : base(status, message)
        { }
    }
    
    public class ObservablePhotonPunCallbacks : MonoBehaviourPunCallbacks
    {
        #region IConnectionCallbacks
        private Subject<Unit> _onConnectedToMaster;
        public IObservable<Unit> OnConnectedToMasterAsObservable() 
            => _onConnectedToMaster ??= new Subject<Unit>();
        public override void OnConnectedToMaster()
            => _onConnectedToMaster?.OnNext(Unit.Default);
        
        private Subject<PhotonDisconnectException> _onDisconnected;
        public IObservable<PhotonDisconnectException> OnDisconnectedAsObservable() 
            => _onDisconnected ??= new Subject<PhotonDisconnectException>();
        public override void OnDisconnected(DisconnectCause cause)
            => _onDisconnected?.OnNext(new PhotonDisconnectException(cause));
        #endregion
        
        #region IMatchmakingCallbacks
        private Subject<Unit> _onLeftRoom;
        public IObservable<Unit> OnLeftRoomAsObservable() 
            => _onLeftRoom ??= new Subject<Unit>();
        public override void OnLeftRoom()
            => _onLeftRoom?.OnNext(Unit.Default);
        
        
        private Subject<Unit> _onCreatedRoom;
        public IObservable<Unit> OnCreatedRoomAsObservable() 
            => _onCreatedRoom ??= new Subject<Unit>();
        public override void OnCreatedRoom()
            => _onCreatedRoom?.OnNext(Unit.Default);
        
        
        private Subject<PhotonCreateRoomException> _onCreateRoomFailed;
        public IObservable<PhotonCreateRoomException> OnCreateRoomFailedAsObservable() 
            => _onCreateRoomFailed ??= new Subject<PhotonCreateRoomException>();
        public override void OnCreateRoomFailed(short returnCode, string message)
            => _onCreateRoomFailed?.OnNext(new PhotonCreateRoomException(returnCode, message));
            
        
        private Subject<Unit> _onJoinedRoom;
        public IObservable<Unit> OnJoinedRoomAsObservable()
            => _onJoinedRoom ??= new Subject<Unit>();
        public override void OnJoinedRoom()
            => _onJoinedRoom?.OnNext(Unit.Default);

        
        private Subject<PhotonJoinRoomException> _onJoinRoomFailed;
        public IObservable<PhotonJoinRoomException> OnJoinRoomFailedAsObservable() 
            => _onJoinRoomFailed ??= new Subject<PhotonJoinRoomException>();
        public override void OnJoinRoomFailed(short returnCode, string message)
            => _onJoinRoomFailed?.OnNext(new PhotonJoinRoomException(returnCode, message));
        
        
        private Subject<PhotonJoinRoomException> _onJoinRandomFailed;
        public IObservable<PhotonJoinRoomException> OnJoinRandomFailedAsObservable() 
            => _onJoinRandomFailed ??= new Subject<PhotonJoinRoomException>();
        public override void OnJoinRandomFailed(short returnCode, string message)
            => _onJoinRandomFailed?.OnNext(new PhotonJoinRoomException(returnCode, message));
        #endregion

        #region IInRoomCallbacks
        private Subject<Player> _onPlayerEnteredRoom;
        public IObservable<Player> OnPlayerEnteredRoomAsObservable() 
            => _onPlayerEnteredRoom ??= new Subject<Player>();
        public override void OnPlayerEnteredRoom(Player newPlayer)
            => _onPlayerEnteredRoom?.OnNext(newPlayer);
        
        
        private Subject<Player> _onPlayerLeftRoom;
        public IObservable<Player> OnPlayerLeftRoomAsObservable() 
            => _onPlayerLeftRoom ??= new Subject<Player>();
        public override void OnPlayerLeftRoom(Player otherPlayer)
            => _onPlayerLeftRoom?.OnNext(otherPlayer);
        #endregion

        #region ILobbyCallbacks
        
        private Subject<Unit> _onJoinedLobby;
        public IObservable<Unit> OnJoinedLobbyAsObservable()
            => _onJoinedLobby ??= new Subject<Unit>();
        public override void OnJoinedLobby()
            => _onJoinedLobby?.OnNext(Unit.Default);

        
        private Subject<List<RoomInfo>> _onRoomListUpdate;
        public IObservable<List<RoomInfo>> OnRoomListUpdateAsObservable() 
            => _onRoomListUpdate ??= new Subject<List<RoomInfo>>();
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
            => _onRoomListUpdate?.OnNext(roomList);

        #endregion
        
        private void OnDestroy()
        {
            _onConnectedToMaster?.OnCompleted();
            _onCreatedRoom?.OnCompleted();
            _onLeftRoom?.OnCompleted();
            _onPlayerLeftRoom?.OnCompleted();
            _onPlayerEnteredRoom?.OnCompleted();
        }


        
    }
}