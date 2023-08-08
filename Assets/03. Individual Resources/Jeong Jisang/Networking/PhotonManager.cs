using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Billiards;
using Photon.Realtime;
using Photon.Pun;
using System;
using System.Linq;
using Appnori.Util;
using MJ;

namespace Appnori.Photon
{

    public class PhotonSingleton<T> : MonoBehaviourPunCallbacks , IPunObservable, IDisposable where T : PhotonSingleton<T> 
    {
        protected static T _instance;

        private static object _lock = new object();

        public static T Instance
        {
            get
            {
                if (applicationIsQuitting)
                {
                    Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                        "' already destroyed on application quit." +
                        " Won't create again - returning null.");
                    return null;
                }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = (T)FindObjectOfType(typeof(T));

                        if (FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            Debug.LogError("[Singleton] Something went really wrong " +
                                " - there should never be more than 1 singleton!" +
                                " Reopening the scene might fix it.");
                            return _instance;
                        }

                        if (_instance == null)
                        {
                            GameObject singleton = new GameObject();
                            _instance = singleton.AddComponent<T>();
                            singleton.name = "(singleton) " + typeof(T).ToString();

                            DontDestroyOnLoad(singleton);
                            Debug.Log("[Singleton] An instance of " + typeof(T) +
                                " is needed in the scene, so '" + singleton +
                                "' was created with DontDestroyOnLoad.");
                        }
                        else
                        {
                            Debug.Log("[Singleton] Using instance already created: " +
                                _instance.gameObject.name);
                        }
                    }

                    return _instance;
                }
            }
        }

        private static event Action<T> onInitialized;
        public static event Action<T> OnInitialized
        {
            add
            {
                if (_instance != null)
                {
                    value?.Invoke(_instance);
                    return;
                }

                onInitialized += value;
            }
            remove
            {
                onInitialized -= value;
            }
        }
        public static bool isInitialized { get; protected set; }
        private static bool applicationIsQuitting = false;
        /// <summary>
        /// When Unity quits, it destroys objects in a random order.
        /// In principle, a Singleton is only destroyed when application quits.
        /// If any script calls Instance after it have been destroyed, 
        ///   it will create a buggy ghost object that will stay on the Editor scene
        ///   even after stopping playing the Application. Really bad!
        /// So, this was made to be sure we're not creating that buggy ghost object.
        /// </summary>
        public virtual void OnDestroy()
        {
            if (_instance == this)
            {
                applicationIsQuitting = true;
            }
        }

        public virtual void OnApplicationQuit()
        {
            applicationIsQuitting = true;
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);

                //invoke added handler
                isInitialized = true;

                onInitialized?.Invoke(_instance);
                onInitialized = null;

            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }

        public static bool TryGetInstance(out T instance)
        {
            instance = _instance;
            return instance != null;
        }

        public void Dispose()
        {
            if (_instance == this)
            {
                _instance = null;
                isInitialized = false;
                DestroyImmediate(gameObject);
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {

        }
    }
    // 포톤 앱 아이디는 같더라도 앱 버전이 다르면 됨 
    public partial class PhotonManager : PhotonSingleton<PhotonManager>
    {
        public Notifier<int> ConnectedPlayerCount { get; private set; } = new Notifier<int>();
        public Notifier<string> ConnectionState { get; private set; } = new Notifier<string>();

        public string roomCode { get; set; } = "";
        public enum PhotonMatchState { None, Single, Random, Friend }

        public PhotonMatchState photonMatchState = PhotonMatchState.None;

        private void Start()
        {
            PhotonNetwork.SerializationRate = 10;
            PhotonNetwork.SendRate = 20;

            var photonView = PhotonView.Get(this);
            photonView = gameObject.AddComponent<PhotonView>();
            photonView.ObservedComponents = new List<Component>(0);
            photonView.ObservedComponents.Add(this);
            photonView.ViewID = 12;
            //PhotonNetwork.AddCallbackTarget(this);

            PhotonNetwork.MinimalTimeScaleToDispatchInFixedUpdate = 1f;

            ConnectionState.OnDataChanged += ConnectionState_OnDataChanged;
           
        }


        private void ConnectionState_OnDataChanged(string obj)
        {
            Debug.Log("ConnectionState : " + obj);
        }


        public void StartConnect()
        {
            //PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "us";

            PhotonNetwork.ConnectUsingSettings();
          
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            /*  foreach (var room in roomList)
              {
                  if (room.PlayerCount == room.MaxPlayers)
                      continue;

                  PhotonNetwork.JoinRoom(room.Name);
                  return;
              }


              CreateRoom();*/
            if (photonMatchState == PhotonMatchState.Random)
            {
                for (int i = 0; i < roomList.Count; i++)
                {
                    if (roomList[i].CustomProperties == null || !roomList[i].CustomProperties.ContainsKey("Code") || !roomList[i].IsOpen)
                    {
                        continue;
                    }
                    if ((GameType)roomList[i].CustomProperties["GameType"] == LevelLoader.CurrentMatchInfo.gameType
                      && (string)roomList[i].CustomProperties["Code"] == ""
                      && roomList[i].PlayerCount != roomList[i].MaxPlayers)
                    {
                        PhotonNetwork.JoinRoom(roomList[i].Name);
                        return;
                    }
                }
            }
            else
            {
                for (int i = 0; i < roomList.Count; i++)
                {
                    if (roomList[i].CustomProperties == null || !roomList[i].CustomProperties.ContainsKey("GameType") || !roomList[i].CustomProperties.ContainsKey("Code"))
                    {
                        continue;
                    }
                    if ((string)roomList[i].CustomProperties["Code"] == roomCode && roomList[i].PlayerCount != roomList[i].MaxPlayers)
                    {
                        PhotonNetwork.JoinRoom(roomList[i].Name);
                        return;
                    }
                }


            }
            CreateRoom();
        }

        private void CreateRoom()
        {
            /*  RoomOptions roomOptions = new RoomOptions
              {
                  PublishUserId = true,
                  MaxPlayers = 2
              };

              PhotonNetwork.CreateRoom("Lobby" + Time.time, roomOptions);*/
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.PublishUserId = true;
            roomOptions.MaxPlayers = 2;
            roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
            roomOptions.CleanupCacheOnLeave = false;
            roomOptions.CustomRoomProperties.Add("GameType", LevelLoader.CurrentMatchInfo.gameType);

            if (photonMatchState == PhotonMatchState.Friend)
            {
                roomOptions.CustomRoomProperties.Add("Code", roomCode);
            }
            else
            {
                roomOptions.CustomRoomProperties.Add("Code", "");
            }
            roomOptions.CustomRoomProperties.Add("GameReady", false);
            roomOptions.CustomRoomProperties.Add("StartTime", PhotonNetwork.ServerTimestamp); // 2~5 동일하게 시작

            roomOptions.CustomRoomPropertiesForLobby = new string[] { "Code", "StartTime", "GameReady" };

            PhotonNetwork.CreateRoom(null, roomOptions);
        }

        public void StartDisconnect()
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Disconnect();
            }
        }

        private void CheckMatchableCondition()
        {
            if (PhotonNetwork.IsSyncScene && PhotonNetwork.isLoadLevel)
                return;

            int playerCount = PhotonNetwork.CurrentRoom.Players.Count;

            foreach (var player in PhotonNetwork.CurrentRoom.Players.OrderBy(i => i.Value.ActorNumber))
            {

                if (PhotonNetwork.LocalPlayer == player.Value)
                    LobbyManager.instance.SetNickPlayer(player.Value.NickName, 0);
                else
                    LobbyManager.instance.SetNickPlayer(player.Value.NickName, 1);
            }

            ConnectedPlayerCount.Value = playerCount;

            switch (playerCount)
            {
                case 1:
                    {


                        break;
                    }

                case 2:
                    {



                        break;
                    }

                default:
                    break;
            }
        }

        //callbacks
        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinLobby();
            ConnectionState.Value = "StartConnect";
        }

        public override void OnJoinedLobby()
        {
            ConnectionState.Value = "StartConnect";
        }

        public override void OnJoinedRoom()
        {
            ConnectionState.Value = "JoinedRoom";
            CheckMatchableCondition();
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            CheckMatchableCondition();
        }
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            CheckMatchableCondition();
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            CheckMatchableCondition();
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            PhotonNetwork.JoinLobby();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            ConnectionState.Value = "Disconnected : " + cause;
        }

    }
}