using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

using BallPool;
using NetworkManagement;
using System;

namespace Billiards
{
    public struct BilliardsMatchInfo
    {
        public string userId;
        public string userName;

        public string otherId;
        public string otherName;

        public PlayType playingType;
        public GameType gameType;
        public int level;
    }

    public enum GameType
    {
        None = 0,
        PocketBilliards,    //포켓볼
        CaromThree,         //3구
        CaromFour,          //4구
    }

    public enum PlayType
    {
        None = 0,
        Single,
        Multi,
    }

    public class LevelLoader : MonoBehaviour
    {

        private static BilliardsMatchInfo currentMatchInfo;
        public static BilliardsMatchInfo CurrentMatchInfo { get => currentMatchInfo; }

        [SerializeField]
        private bool useManual;

        [SerializeField]
        private GameType gameType = GameType.None;

        [SerializeField]
        private PlayType playingType = PlayType.Single;

        [Range(1, 5)]
        [SerializeField]
        private int Level = 1;

        private void Awake()
        {
            if (useManual)
                return;

            if (playingType == PlayType.Multi)
                return;

            InitializeAndLoad(BuildDevelopInfo());
        }

        public BilliardsMatchInfo BuildDevelopInfo() => new BilliardsMatchInfo()
        {
            userId = "userID",
            userName = "_userName",
            otherId = "otherId",
            otherName = "_otherName",

            playingType = playingType,
            gameType = gameType,
            level = Level,
        };


        public static void InitializeAndLoad(in BilliardsMatchInfo loadinfo)
        {
            currentMatchInfo = loadinfo;

            PlayerInitialize(loadinfo);

            GameInitialize(loadinfo);

            switch (loadinfo.gameType)
            {
                case GameType.None:
                    break;

                case GameType.PocketBilliards:
                    SceneManager.LoadScene("AightBallPool");
                    break;

                case GameType.CaromThree:
                    SceneManager.LoadScene("CaromBilliards");
                    break;

                case GameType.CaromFour:
                    SceneManager.LoadScene("CaromBilliardsFour");
                    break;
            }
            GameDataManager.instance.SetVRDevice();
            SceneManager.LoadScene(GameConfig.TableSceneName, LoadSceneMode.Additive);
        }

        private static void PlayerInitialize(in BilliardsMatchInfo info)
        {
            //player initialize
            BallPoolPlayer.players = new BallPoolPlayer[2];
            BallPoolPlayer.players[0] = new AightBallPoolPlayer(0, info.userName, 1, info.userId);
            BallPoolPlayer.players[1] = new AightBallPoolPlayer(1, info.otherName, 1, info.otherId);
            BallPoolPlayer.playersCount = 2;

            NetworkManager.initialized = true;
            NetworkManager.InitializeMainPlayer();
        }

        private static void GameInitialize(in BilliardsMatchInfo info)
        {
            switch (info.playingType)
            {
                case PlayType.None:
                case PlayType.Single when info.gameType != GameType.PocketBilliards:
                    BallPoolAIExtension.DumbLevel = 100;
                    BallPoolGameLogic.playMode = BallPool.PlayMode.Solo;
                    break;

                case PlayType.Single when info.gameType == GameType.PocketBilliards:
                    BallPoolAIExtension.DumbLevel = 6 - Mathf.Clamp(info.level, 1, 5);
                    BallPoolGameLogic.playMode = BallPool.PlayMode.PlayerAI;
                    break;

                case PlayType.Multi:
                    BallPoolGameLogic.playMode = BallPool.PlayMode.OnLine;
                    NetworkManager.network.SetAdapter(new AightBallPoolNetworkGameAdapter());
                    NetworkManager.network.adapter.SetTurn((NetworkManager.network as PunNetwork).isMasterClient() ? 1 : 0);
                    break;
            }
        }


        public static bool TryGetPlayerInfos(out Photon.Realtime.Player localPlayer, out Photon.Realtime.Player opponent)
        {
            var others = Photon.Pun.PhotonNetwork.CurrentRoom.Players.Values.Except(new List<Photon.Realtime.Player>() { Photon.Pun.PhotonNetwork.LocalPlayer });
            if (others == null || others.Count() < 1)
            {
                localPlayer = null;
                opponent = null;

                return false;
            }

            localPlayer = Photon.Pun.PhotonNetwork.LocalPlayer;
            opponent = others.First();
            return true;
        }

    }
}