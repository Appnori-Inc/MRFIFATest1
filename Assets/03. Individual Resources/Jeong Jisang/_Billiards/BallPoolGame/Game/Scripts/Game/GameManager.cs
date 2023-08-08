using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BallPool;
using BallPool.AI;
using BallPool.Mechanics;
using NetworkManagement;
using Billiards;
using System;
using System.Linq;
using UnityEngine.SocialPlatforms.Impl;

namespace BallPool
{

    public class GameManager : MonoBehaviour
    {
        [SerializeField] private UIIngameDisplay IngameDisplay;

        [SerializeField] private Text prize;
        [SerializeField] private Text gameInfo;
        [SerializeField] private Text aiCountText;
        public PlayAgainMenu playAgainMenu;
        [SerializeField] private GameUIController gameUIController;
        public ShotController shotController;

        [SerializeField] private PhysicsManager physicsManager;
        [SerializeField] private BallPoolAIManager aiManager;
        public PlayerManager playerManager;

        public Ball[] balls;
        private bool playAgainMenuIsActive;
        private BallPoolGameManager ballPoolGameManager;
        private bool applicationIsPaused;
        private int applicationPauseSeconds;

        public bool RequestRematch { get => gameUIController.RequestRematch; set => gameUIController.RequestRematch = value; }

        void Awake()
        {
            Application.targetFrameRate = 120;
            if (!NetworkManager.initialized)
            {
                Debug.LogWarning("not initialized");
                gameUIController.GoHome();
                enabled = false;
                return;
            }

            UIIngameDisplay.OnInitializedEvent += (instance) => IngameDisplay = instance;

            DataManager.SaveGameData();
            UpdateAICount();
            if (BallPoolGameLogic.playMode == BallPool.PlayMode.Replay)
            {
                enabled = false;
                shotController.enabled = false;
                TriggerPlayAgainMenu(false);

                physicsManager.OnBallMove += (int ballId, Vector3 position, Vector3 velocity, Vector3 angularVelocity) =>
                    {
                        if (balls[ballId].inPocket)
                        {
                            balls[ballId].OnState(BallState.MoveInPocket);
                        }
                        else
                        {
                            balls[ballId].OnState(BallState.Move);
                        }
                    };

                return;
            }
            NetworkManager.network.OnNetwork += NetworkManager_network_OnNetwork;

            switch (LevelLoader.CurrentMatchInfo.gameType)
            {
                case GameType.PocketBilliards:
                    InitializeBallPool();
                    break;


                case GameType.CaromThree:
                    InitializeCarom();
                    break;

                case GameType.CaromFour:
                    InitializeCaromFour();
                    break;
            }

            //PublicGameUIManager.GetInstance.AddLoadLobbyEvent(gameUIController.OnPauseGoHome);
            if (PublicGameUIManager.GetInstance)
            {

            }
        }

        private void InitializeBallPool()
        {
            ballPoolGameManager = new AightBallPoolGameManager();
            ballPoolGameManager.Initialize(physicsManager, aiManager, balls);
            ballPoolGameManager.maxPlayTime = Billiards.GameConfig.MaxTurnTime;
            ballPoolGameManager.OnEndTime += AightBallPoolGameManager_OnEndTime;
            ballPoolGameManager.OnSetGameInfo += AightBallPoolGameManager_OnSetGameInfo;

            gameUIController.OnForceGoHome += GameUIController_OnForceGoHome;
            gameUIController.OnRequestRematch += GameUIController_OnRequestRematch;
        }

        private void InitializeCarom()
        {
            ballPoolGameManager = new CaromGameManager();
            ballPoolGameManager.Initialize(physicsManager, aiManager, balls);

            ballPoolGameManager.maxPlayTime = LevelLoader.CurrentMatchInfo.playingType == PlayType.Single ? float.PositiveInfinity : GameConfig.MaxTurnTime;
            ballPoolGameManager.OnEndTime += AightBallPoolGameManager_OnEndTime;
            ballPoolGameManager.OnSetGameInfo += AightBallPoolGameManager_OnSetGameInfo;

            gameUIController.OnForceGoHome += GameUIController_OnForceGoHome;
            gameUIController.OnRequestRematch += GameUIController_OnRequestRematch;

            var caromThreeTargetScore = LevelLoader.CurrentMatchInfo.playingType == PlayType.Multi ? GameConfig.CaromMultiplayerTargetScore : GameConfig.CurrentCaromSingleTargetScore;

            AightBallPoolPlayer.mainPlayer.CaromTargetScore = caromThreeTargetScore;
            AightBallPoolPlayer.otherPlayer.CaromTargetScore = caromThreeTargetScore;

            AightBallPoolPlayer.mainPlayer.OnPlayerStateChanged += MainPlayer_OnPlayerStateChanged;
            AightBallPoolPlayer.otherPlayer.OnPlayerStateChanged += OtherPlayer_OnPlayerStateChanged;

            BilliardsDataContainer.Instance.CaromLife.Value = GameConfig.CaromSinglePlayerLife;
        }

        private void InitializeCaromFour()
        {
            ballPoolGameManager = new CaromFourGameManager();
            ballPoolGameManager.Initialize(physicsManager, aiManager, balls);

            ballPoolGameManager.maxPlayTime = LevelLoader.CurrentMatchInfo.playingType == PlayType.Single ? float.PositiveInfinity : GameConfig.MaxTurnTime;
            ballPoolGameManager.OnEndTime += AightBallPoolGameManager_OnEndTime;
            ballPoolGameManager.OnSetGameInfo += AightBallPoolGameManager_OnSetGameInfo;

            gameUIController.OnForceGoHome += GameUIController_OnForceGoHome;
            gameUIController.OnRequestRematch += GameUIController_OnRequestRematch;

            var caromFourTargetScore = LevelLoader.CurrentMatchInfo.playingType == PlayType.Multi ? GameConfig.CaromMultiplayerTargetScore : GameConfig.CurrentCaromSingleTargetScore;

            AightBallPoolPlayer.mainPlayer.CaromTargetScore = caromFourTargetScore;
            AightBallPoolPlayer.otherPlayer.CaromTargetScore = caromFourTargetScore;

            AightBallPoolPlayer.mainPlayer.OnPlayerStateChanged += MainPlayer_OnPlayerStateChanged;
            AightBallPoolPlayer.otherPlayer.OnPlayerStateChanged += OtherPlayer_OnPlayerStateChanged;
            BilliardsDataContainer.Instance.CaromLife.Value = GameConfig.CaromSinglePlayerLife;
        }


        void Start()
        {
            MeshFadeCtrl.instance.StartFade(true);

            TriggerPlayAgainMenu(false);

            ballPoolGameManager.OnCalculateAI += AightBallPoolGameManager_OnCalculateAI;
            ballPoolGameManager.OnSetPlayer += AightBallPoolGameManager_OnSetPlayer;
            //aightBallPoolGameManager.OnSetAvatar += AightBallPoolGameManager_OnSetAvatar;
            ballPoolGameManager.OnSetActivePlayer += AightBallPoolGameManager_OnSetActivePlayer;
            ballPoolGameManager.OnGameComplite += AightBallPoolGameManager_OnGameComplite;
            ballPoolGameManager.OnSetActiveBallsIds += AightBallPoolGameManager_OnSetActiveBallsIds;
            ballPoolGameManager.OnEnableControl += shotController.OnEnableControl;

            shotController.OnEndCalculateAI += ShotController_OnEndCalculateAI;
            shotController.OnSelectBall += ShotController_OnSelectBall;
            shotController.OnUnselectBall += ShotController_OnUnselectBall;
            physicsManager.OnSaveEndStartReplay += PhysicsManager_OnSaveEndStartReplay;

            ballPoolGameManager.Start();

            if (BallPoolGameLogic.isOnLine)
            {
                NetworkManager.network.SendRemoteMessage("OnOpponenWaitingForYourTurn");
                NetworkManager.network.SendRemoteMessage("OnOpponenInGameScene");
                StartCoroutine(UpdateNetworkTime());
                StartCoroutine(UpdateNetworkEnvironment());
            }
            gameInfo.text = (BallPoolPlayer.mainPlayer.myTurn ? "Your turn" : "Your opponent turn") + "\nGood lack!";
            StartCoroutine(HideGameInfoText(gameInfo.text));

            Resources.UnloadUnusedAssets();
        }


        void Update()
        {
            ballPoolGameManager.Update(Time.deltaTime);
            IngameDisplay.SetTime(ballPoolGameManager.playTime);

            if (BallPoolGameLogic.controlFromNetwork && !shotController.inMove && !physicsManager.inMove)
            {
                shotController.UpdateFromNetwork();
            }
        }

        public void SetBallsState(int number)
        {
            for (int i = 0; i < balls.Length; i++)
            {
                balls[i].SetMechanicalState(number);
            }
        }
        /// <summary>
        /// 게임에 영향을 미치는 값들 동기화
        /// </summary>
        IEnumerator UpdateNetworkTime()
        {
            while (NetworkManager.network != null)
            {
                if (BallPoolGameLogic.controlInNetwork && !shotController.inMove)
                {
                    yield return new WaitForSeconds(GameConfig.SendGameInterval);
                    SendToNetwork();
                }
                else
                {
                    yield return null;
                }
            }
        }

        /// <summary>
        /// 게임에 영향을 미치지 않는 값들 동기화 (ex : 플레이어 머리 위치, 이모티콘, 채팅 ......)
        /// </summary>
        IEnumerator UpdateNetworkEnvironment()
        {
            while (NetworkManager.network != null)
            {
                if (BallPoolGameLogic.controlInNetwork || BallPoolGameLogic.controlFromNetwork)
                {
                    yield return new WaitForSeconds(GameConfig.SendEnvironmentInterval);
                    SendToNetworkEnvironment();
                }
                else
                {
                    yield return null;
                }
            }

        }
        void PhysicsManager_OnSaveEndStartReplay(string impulse)
        {
            NetworkManager.network.OnMadeTurn();
            if (BallPoolGameLogic.controlInNetwork)
            {
                NetworkManager.network.SendRemoteMessage("StartSimulate", impulse);
                NetworkManager.network.SendRemoteMessage("StartSimulate", impulse);
                NetworkManager.network.SendRemoteMessage("StartSimulate", impulse);
            }
        }
        private void SendToNetwork()
        {
            if (!NetworkManager.network || ballPoolGameManager == null)
            {
                return;
            }
            NetworkManager.network.SendRemoteMessage("OnSendTime", ballPoolGameManager.playTime);
            if (shotController.cueChanged)
            {
                NetworkManager.network.SendRemoteMessage("OnSendCueControl", shotController.cuePivot.localPosition, shotController.cuePivot.localRotation.eulerAngles.y, shotController.cueVertical.localRotation.eulerAngles.x,
                    new Vector2(shotController.cueDisplacement.localPosition.x, shotController.cueDisplacement.localPosition.y), shotController.cueSlider.localPosition.z, shotController.force);
            }
            if (shotController.ballChanged)
            {
                Debug.LogWarning("ballChanged");
                NetworkManager.network.SendRemoteMessage("OnMoveBall", shotController.cueBall.position);
            }

        }
        /// <summary>
        /// head position, imoticon, chat, etc.
        /// </summary>
        private void SendToNetworkEnvironment()
        {
            if (playerManager.Mine.isDirty)
            {
                NetworkManager.network.SendRemoteMessage("SendPlayerTransform", AightBallPoolPlayer.mainPlayer.uuid,
                    playerManager.Mine.worldPosition, playerManager.Mine.worldRotation,
                    playerManager.Mine.LHandPosition, playerManager.Mine.LHandRotation,
                    playerManager.Mine.RHandPosition, playerManager.Mine.RHandRotation);
            }

            NetworkManager.network.SendRemoteMessage("OnSendGameState", (int)BilliardsDataContainer.Instance.GameState.Value);
            NetworkManager.network.SendRemoteMessage("OnSendMainHanded", ShotController.IsRightHanded ? 1 : 0);
        }

        private void SendToNetworkOnEnd()
        {
            Debug.LogWarning("SendToNetworkOnEnd");

            NetworkManager.network.SendRemoteMessage("OnSendTime", ballPoolGameManager.playTime);
            NetworkManager.network.SendRemoteMessage("OnSendTime", ballPoolGameManager.playTime);
            NetworkManager.network.SendRemoteMessage("OnSendTime", ballPoolGameManager.playTime);
        }
        void ShotController_OnSelectBall()
        {
            if (BallPoolGameLogic.controlInNetwork)
            {
                NetworkManager.network.SendRemoteMessage("SelectBallPosition", shotController.cueBall.position);
            }
        }
        void ShotController_OnUnselectBall()
        {
            if (BallPoolGameLogic.isOnLine)
            {
                Debug.LogWarning("ShotController_OnUnselectBall" + BallPoolGameLogic.controlInNetwork);
                NetworkManager.network.SendRemoteMessage("SetBallPosition", shotController.cueBall.position);
            }
        }
        void OnDisable()
        {
            if (ballPoolGameManager != null)
            {
                ballPoolGameManager.OnDisable();
                ballPoolGameManager = null;
            }
            NetworkManager.Disable();
        }
        void NetworkManager_network_OnNetwork(NetworkState state)
        {
            Debug.Log("NetworkManager_network_OnNetwork " + state);
            if (BallPoolGameLogic.playMode == BallPool.PlayMode.OnLine)
            {
                if (state != NetworkState.Connected)
                {
                    shotController.enabled = false;
                    StartCoroutine(WaitAndGoToHome(state));
                }
            }
        }
        IEnumerator WaitAndGoToHome(NetworkState state)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            enabled = false;
            if (!playAgainMenu.wasOpened && state == NetworkState.LeftRoom)
            {
                OpenPlayAgainMenuForMainPlayer();
            }
            else if (playAgainMenu.wasOpened || state == NetworkState.LostConnection)
            {
                gameUIController.GoHome();
            }
        }
        //void OnApplicationPause(bool pauseStatus)
        //{
        //    if (Application.isEditor)
        //    {
        //        return;
        //    }

        //    if (pauseStatus)
        //    {
        //        applicationPauseSeconds = GetTimeInSeconds();
        //    }
        //    else
        //    {
        //        if (applicationIsPaused && Mathf.Abs(applicationPauseSeconds - GetTimeInSeconds()) > 3)
        //        {
        //            gameUIController.ForceGoHome();
        //        }
        //    }
        //    applicationIsPaused = pauseStatus;
        //}
        //private int GetTimeInSeconds()
        //{
        //    return 60 * 60 * System.DateTime.Now.Hour + 60 * System.DateTime.Now.Minute + System.DateTime.Now.Second;
        //}

        public void UpdateAICount()
        {
            //aiCountText.text = ProductAI.aiCount + "";
        }
        private void OpenPlayAgainMenuForMainPlayer()
        {
            shotController.shotBack.enabled = false;
            BallPoolPlayer.SetWinner(BallPoolPlayer.mainPlayer.uuid);
            //var winnerIndex = GameDataManager.instance.userInfos.Select((info) => info.nick).ToList().IndexOf(BallPoolPlayer.mainPlayer.name);
            //PublicGameUIManager.GetInstance.OpenResultBoard(GameSettingCtrl.GetLocalizationText("0034") /*"상대방이 나갔습니다"*/, winnerIndex);

            playAgainMenu.ShowMainPlayer();

            //갱신된 코인정보를 위해 다시 정보 갱신
            IngameDisplay.SetPlayer(AightBallPoolPlayer.mainPlayer);
            IngameDisplay.SetPlayer(AightBallPoolPlayer.otherPlayer);

        }

        void GameUIController_OnForceGoHome()
        {
            if (BallPoolGameLogic.isOnLine)
            {
                NetworkManager.network.SendRemoteMessage("OnOpponentForceGoHome");
            }
            if (BallPoolGameManager.instance != null)
            {
                BallPoolGameManager.instance.OnForceGoHome(AightBallPoolPlayer.otherPlayer.playerId);
            }
        }

        void GameUIController_OnRequestRematch(bool isRequested)
        {
            NetworkManager.network.SendRemoteMessage("SendRequestRematch", isRequested);
        }


        private void MainPlayer_OnPlayerStateChanged()
        {
            UIIngameDisplay.Instance?.SetPlayerScore(AightBallPoolPlayer.mainPlayer.CaromScore, AightBallPoolPlayer.mainPlayer.CaromTargetScore);

            if (LevelLoader.CurrentMatchInfo.playingType != PlayType.Multi)
                return;
            NetworkManager.network.SendRemoteMessage(nameof(PunNetwork.SendPlayerScore), AightBallPoolPlayer.mainPlayer.uuid, AightBallPoolPlayer.mainPlayer.CaromScore, AightBallPoolPlayer.mainPlayer.CaromTargetScore);
        }

        private void OtherPlayer_OnPlayerStateChanged()
        {
            if (LevelLoader.CurrentMatchInfo.playingType != PlayType.Multi)
                return;

            UIIngameDisplay.Instance?.SetPlayerScoreFromNetwork(AightBallPoolPlayer.otherPlayer.CaromScore, AightBallPoolPlayer.otherPlayer.CaromTargetScore);
        }

        //controll form network
        public void SetPlayerScore(in string uuid, in int opponentScore,in int ppponentTargetScore)
        {
            //AightBallPoolPlayer.otherPlayer.CaromScore = opponentScore;
            //AightBallPoolPlayer.otherPlayer.CaromTargetScore = ppponentTargetScore;

            //UIIngameDisplay.Instance?.SetPlayerScoreFromNetwork(opponentScore, ppponentTargetScore);
        }

        private void TriggerPlayAgainMenu(bool isOn)
        {
            if (isOn)
            {
                shotController.shotBack.enabled = false;
                BallPoolPlayer winner = BallPoolPlayer.GetWinner();
                BallPoolPlayer other = BallPoolPlayer.GetNotWinner();
                IngameDisplay.SetPlayer(AightBallPoolPlayer.mainPlayer);
                IngameDisplay.SetPlayer(AightBallPoolPlayer.otherPlayer);
                GameDataManager.instance.EndGame(winner.uuid, other.uuid, 10000);
                //var winnerIndex = GameDataManager.instance.userInfos.Select((info) => info.nick).ToList().IndexOf(BallPoolPlayer.GetWinner().name);
                //var winnerIndex = GameDataManager.instance.userInfos.Select((info) => info.nick).ToList().IndexOf(BallPoolPlayer.GetWinner().name);
                //if (AightBallPoolPlayer.mainPlayer.uuid.Equals(AightBallPoolPlayer.otherPlayer.uuid))
                //{
                //    winnerIndex = winner.playerId;
                //    if (!Photon.Pun.PhotonNetwork.IsMasterClient)
                //    {
                //        winnerIndex = winner.playerId == 0 ? 1 : 0;
                //    }
                //}
                //PublicGameUIManager.GetInstance.OpenResultBoard(new string[] { "", "" }, winnerIndex);

                playAgainMenu.SetNextStage(winner == AightBallPoolPlayer.mainPlayer);
                playAgainMenu.Show(winner);

                if (winner == AightBallPoolPlayer.mainPlayer)
                {
                    Debug.Log("BilliardsDataContainer.Instance.TurnCount.CurrentData : " + BilliardsDataContainer.Instance.TurnCount.Value);
                    if (BilliardsDataContainer.Instance.TurnCount.Value == 1)
                    {
                        //achievement
                        //GameDataManager.instance.UnlockAchieve("Ach24", 1);
                    }


                    if (BallPoolGameLogic.playMode != PlayMode.Solo)
                    {
                        //EyeAnimationCtrl.GetInstance.SetExpression(2, 3.0f);
                    }

                    //SoundManager.PlaySound(SoundManager.AudioClipType.Win);

                    if (BallPoolGameLogic.playMode == BallPool.PlayMode.PlayerAI || BallPoolGameLogic.playMode == PlayMode.Solo)
                    {
                        //GameDataManager.instance.SetClearLevelData();
                    }
                }
                else
                {
                    //EyeAnimationCtrl.GetInstance.SetExpression(1, 3.0f);
                    //SoundManager.PlaySound(SoundManager.AudioClipType.Lose);
                }
            }
            else
            {
                playAgainMenu.Hide();
            }
        }

        void AightBallPoolGameManager_OnEndTime()
        {
            if (BallPoolGameLogic.controlInNetwork)
            {
                SendToNetworkOnEnd();
            }
            shotController.OnEndTime();
            shotController.UndoShot();
        }
        private bool gameInfoInProgress = false;

        void AightBallPoolGameManager_OnSetGameInfo(string info)
        {
            StartCoroutine(HideGameInfoText(info));
        }
        IEnumerator HideGameInfoText(string info)
        {
            while (gameInfoInProgress)
            {
                yield return null;
            }
            gameInfoInProgress = true;
            gameInfo.text = info;
            yield return new WaitForSeconds(5.0f);
            gameInfo.text = "";
            gameInfoInProgress = false;
        }
        void AightBallPoolGameManager_OnSetPlayer(BallPoolPlayer player)
        {
            IngameDisplay.SetPlayer(player);
        }

        [Obsolete]
        private IEnumerator DownloadAndSetAvatar(BallPoolPlayer player)
        {
            //if (player is AightBallPoolPlayer)
            //{
            //    var _player = player as AightBallPoolPlayer;
            //    if (_player.avatar == null)
            //    {
            //        yield return StartCoroutine(_player.DownloadAvatar());
            //    }

            //    if (AightBallPoolPlayer.mainPlayer.avatar != null)
            //    {
            //        IngameDisplay.SetAvatar(_player);
            //    }
            //}
            //else
            //{
            //    Debug.LogError("DownloadAndSetAvatar Error. player is not AightBallPoolPlayer");
            //}
            yield break;
        }

        private IEnumerator WaitAndShotAI()
        {
            yield return new WaitForSeconds(0.3f);
            if (!shotController.inMove && !shotController.activateAfterCalculateAI)
            {
                gameUIController.Shot(false);
            }
        }
        void AightBallPoolGameManager_OnSetActiveBallsIds(BallPoolPlayer player)
        {
            IngameDisplay.SetActiveBallsIds(player);
        }

        void AightBallPoolGameManager_OnGameComplite()
        {
            shotController.enabled = false;
            TriggerPlayAgainMenu(true);
        }

        void AightBallPoolGameManager_OnSetActivePlayer(BallPoolPlayer player, bool value)
        {
            IngameDisplay.SetTurn(player.playerId, value);
            if (player.playerId == 0)
            {
                if (value)
                {
                    shotController.SetState(ShotController.GameStateType.MoveAroundTable);
                }
                else
                {
                    shotController.SetState(ShotController.GameStateType.WaitingForOpponent);
                }

            }
        }

        void AightBallPoolGameManager_OnCalculateAI()
        {
            shotController.enabled = false;
            StopCoroutine("WaitAndCalculateAI");
            StartCoroutine("WaitAndCalculateAI");
        }
        private IEnumerator WaitAndCalculateAI()
        {
            yield return new WaitForSeconds(0.3f);
            aiManager.CalculateAI();
        }

        //void AightBallPoolGameManager_OnSetAvatar(BallPoolPlayer player)
        //{
        //    StartCoroutine(DownloadAndSetAvatar(player));
        //}

        void ShotController_OnEndCalculateAI()
        {
            StartCoroutine(WaitAndShotAI());
        }

        private void OnDestroy()
        {
            AightBallPoolPlayer.mainPlayer.OnPlayerStateChanged -= MainPlayer_OnPlayerStateChanged;
            AightBallPoolPlayer.otherPlayer.OnPlayerStateChanged -= OtherPlayer_OnPlayerStateChanged;
        }
    }
}