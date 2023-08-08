using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BallPool.Mechanics;
using Billiards;
using Oculus.Platform.Samples.VrHoops;
using UnityEngine;

namespace BallPool
{
    using FoulType = AightBallPoolGameLogic.FoulType;

    public class CaromFourGameLogic : BallPoolGameLogic
    {
        private static AightBallPoolGameState _gameState;

        public static AightBallPoolGameState gameState
        {
            get
            {
                if (_gameState == null)
                {
                    _gameState = new AightBallPoolGameState();
                }
                return _gameState;
            }
        }

        public CaromFourGameLogic() : base()
        {
            OnFoul += SetFoulCalled;

            void SetFoulCalled(FoulType type, (BallListener, PocketListener) info)
            {
                if (AightBallPoolGameLogic.playMode == PlayMode.OnLine && !isFoulCalled && BallPoolPlayer.mainPlayer.myTurn)
                {
                    int ballId = info.Item1 == null ? -1 : info.Item1.id;
                    int pocketId = info.Item2 == null ? -1 : info.Item2.id;

                    NetworkManagement.NetworkManager.network.SendRemoteMessage("SendFoulInfo", (int)type, ballId, pocketId);
                }

                isFoulCalled = true;
            }
        }

        //public enum FoulType
        //{
        //    None = 0,
        //    Scratch = 1, // PocketListner
        //    BadHit = 2, // BallListener
        //    NoHit = 3, //BallListener
        //    WeakBreak = 4,//BallListener

        //    //not implemented
        //    NoRail = 4,
        //    OutOfTable = 5,
        //}

        public static bool isFoulCalled { get; private set; }
        public static event System.Action<FoulType, (BallListener, PocketListener), float> OnFoulWithDelay;
        public static event System.Action<FoulType, (BallListener, PocketListener)> OnFoul;

        public static void SetFoulFormNetwork(int foulType, int ballId, int pocketId)
        {
            BallListener targetBall = null;
            PocketListener pocketListener = null;
            if (ballId != -1)
            {
                targetBall = CaromFourGameManager.instance.balls[ballId].listener;
            }
            if (pocketId != -1)
            {
                pocketListener = CaromFourGameManager.instance.physicsManager.pocketListeners[pocketId];
            }

            switch ((FoulType)foulType)
            {
                case FoulType.BadHit:
                case FoulType.Scratch:
                    OnFoulWithDelay?.Invoke((FoulType)foulType, (targetBall, pocketListener), 1f); //network sync delayTime = 1f
                    break;

                default:
                    OnFoul?.Invoke((FoulType)foulType, (targetBall, pocketListener));
                    Debug.LogError("FoulCalled. Type is Called By Network");
                    break;
            }
        }

        public void ResetState()
        {
            gameState.gameIsComplete = false;
            gameState.needToChangeTurn = false;
            gameState.cueBallHasHitRightBall = false;
            gameState.cueBallHasHitSomeBall = false;
            gameState.hasRightBallInPocket = false;
            gameState.ballsHitBoardCount = 0;
            gameState.cueBallInHand = false;
            gameState.cueBallInPocket = false;
            gameState.cueBallMoved = false;

            AightBallPoolPlayer.mainPlayer.isCueBall = false;
            AightBallPoolPlayer.otherPlayer.isCueBall = false;

            isFoulCalled = false;

            //carom
            gameState.cueballHitBoardCount = 0;
            gameState.HittedBallsIds.Clear();
            gameState.cueballhitTime.Clear();

            //achievement
            gameState.BankShotStack = 0;
            gameState.PocketedBallIds.Clear();
            gameState.BankShotContactBallIds.Clear();
        }

        public void CheckAchievement(int state, int ballId = -1)
        {
            if ((AightBallPoolPlayer)BallPoolPlayer.currentPlayer == AightBallPoolPlayer.otherPlayer)
                return;

            switch (state)
            {
                //init hatTrick
                case -1:
                    gameState.HatTrickReported = false;
                    gameState.HatTrickStack = 0;
                    break;

                //The cue ball hits the board without hitting any ball.
                case 0:
                    if (gameState.cueBallHasHitSomeBall)
                        return;

                    gameState.BankShotStack = 1;
                    break;

                // the cueBall was hit Right ball and bank state > 0
                case 1:
                    if (gameState.BankShotStack == 0)
                        return;

                    gameState.BankShotContactBallIds.Add(ballId);
                    break;

                //the hitten ball was pocketed
                case 2:
                    //HatTrick check
                    gameState.HatTrickStack++;
                    if (gameState.HatTrickStack >= 3 && !gameState.HatTrickReported)
                    {
                        gameState.HatTrickReported = true;

                        //achievement
                        //GameDataManager.instance.UnlockAchieve("Ach23", 1);
                        //CueSports.AchievementWrapper.Report(CueSports.GameConfig.AchievementHatTrickCode);
                    }

                    if (gameState.BankShotStack == 0)
                        return;

                    if (!gameState.hasRightBallInPocket)
                        return;

                    if (!gameState.BankShotContactBallIds.Contains(ballId))
                        return;

                    //achievement
                    //CueSports.AchievementWrapper.Report(CueSports.GameConfig.AchievementBankShotCode);
                    break;
            }

        }

        public void OnBallHitBoard(int ballId)
        {
            var isControlFromNetwork = controlFromNetwork;
            var cueball = isCueBall(ballId, true);
            var HittedBallCount = gameState.HittedBallsIds.Count;
            var cueballHitBoardCount = gameState.cueballHitBoardCount;

            Debug.Log($"[Test] " +
                $"{nameof(isControlFromNetwork)} : {isControlFromNetwork}" +
                $", {nameof(cueball)} : {cueball}" +
                $", {nameof(HittedBallCount)} : {HittedBallCount} " +
                $", {nameof(cueballHitBoardCount)} : {cueballHitBoardCount} ");

            if (!isCueBall(ballId, true))
                return;

            //achievement - bankshot
            CheckAchievement(0);

            //caromLogic - Three cushion
            if (gameState.HittedBallsIds.Count < 2)
                gameState.cueballHitBoardCount++;
        }

        public void OnCueBallHitBall(int cueBallId, int ballId)
        {
            var time = BallPoolGameManager.instance.physicsManager.moveTime;
            if (CaromFourGameLogic.isOpponentCueball(ballId, true))
            {
                //hit opponent cueball
                gameState.needToChangeTurn = true;

                //Event : Foul
                if (!isFoulCalled)
                {
                    OnFoul?.Invoke(FoulType.BadHit, (BallPoolGameManager.instance.balls[ballId].listener, null));
                    Debug.LogError("FoulCalled. Type is isOpponentCueball");
                }
            }
            else if (gameState.HittedBallsIds.Contains(ballId))
            {
                Debug.Log($"[Turn] Twotouch time check. time is {time}, and gameState.cueballhitTime[{ballId}] is {gameState.cueballhitTime[ballId]} ");
                //two touch
                if (time - gameState.cueballhitTime[ballId] > GameConfig.IgnoreTwoTouchTime)
                {
                    gameState.needToChangeTurn = true;

                    //Event : Foul
                    if (!isFoulCalled)
                    {
                        OnFoul?.Invoke(FoulType.BadHit, (BallPoolGameManager.instance.balls[ballId].listener, null));
                        Debug.LogError($"FoulCalled. Type is twoTouch. time is {time},gameState.cueballhitTime[{ballId}] is {gameState.cueballhitTime[ballId]} ");
                    }
                }
            }
            else
            {
                gameState.HittedBallsIds.Add(ballId);
                gameState.cueballhitTime[ballId] = time;
                Debug.Log($"[Turn]gameState.cueballhitTime[{ballId}] is {time}");
            }

            //Add Hitted data
            int value = BilliardsDataContainer.Instance.HittedBallIds.Value;
            value |= 1 << ballId;
            BilliardsDataContainer.Instance.HittedBallIds.Value = value;
        }

        public void OnEndShot(out bool gameIsEnd)
        {
            //데이터 초기화
            BilliardsDataContainer.Instance.HittedBallIds.Value = 0;

            gameState.needToChangeTurn = checkNeedToChangeTurn(out var isFinal);
            gameState.tableIsOpened = false;

            if(gameState.needToChangeTurn == true)
            {
                if (LevelLoader.CurrentMatchInfo.playingType == PlayType.Single)
                {
                    BilliardsDataContainer.Instance.CaromLife.Value--;

                    //case 1. SinglePlayer Life
                    if (BilliardsDataContainer.Instance.CaromLife.Value <= 0)
                    {
                        AightBallPoolPlayer.mainPlayer.isWinner = false;
                        AightBallPoolPlayer.otherPlayer.isWinner = true;
                        gameState.needToChangeTurn = false;
                        gameIsEnd = true;
                        BallPoolGameManager.instance.SetGameInfo(string.Empty);
                        return;
                    }
                }
            }
            else
            {
                AightBallPoolPlayer.CurrentTurnPlayer.CaromScore++;
                AightBallPoolPlayer.CurrentTurnPlayer.UpdatePlayerState();

                var targetScore = AightBallPoolPlayer.CurrentTurnPlayer.CaromTargetScore;
                var turnPlayerScore = AightBallPoolPlayer.CurrentTurnPlayer.CaromScore;

                //case 2. reach target score in single/multi
                //when player success 3cushion ball
                if (turnPlayerScore > targetScore && isFinal)
                {
                    AightBallPoolPlayer.mainPlayer.isWinner = AightBallPoolPlayer.mainPlayer.myTurn;
                    AightBallPoolPlayer.otherPlayer.isWinner = AightBallPoolPlayer.otherPlayer.myTurn;
                    gameState.needToChangeTurn = false;
                    gameIsEnd = true;
                    BallPoolGameManager.instance.SetGameInfo(string.Empty);
                    return;
                }
            }

            gameIsEnd = false;
            return;

            //functions
            bool checkNeedToChangeTurn(out bool isFinal)
            {
                isFinal = false;

                //already foul
                if (gameState.needToChangeTurn)
                {
                    Debug.Log($"[Turn] gameState.needToChangeTurn is alreay true.");
                    return true;
                }

                //checking Final shot
                var targetScore = AightBallPoolPlayer.CurrentTurnPlayer.CaromTargetScore;
                var turnPlayerScore = AightBallPoolPlayer.CurrentTurnPlayer.CaromScore;

                if (targetScore <= turnPlayerScore)
                {
                    isFinal = true;

                    if (gameState.cueballHitBoardCount < 3)
                    {
                        Debug.Log($"[Turn] gameState.cueballHitBoardCount < 3. count is {gameState.cueballHitBoardCount}");
                        return true;
                    }
                }

                //need to change three-ball and four ball
                if (gameState.HittedBallsIds.Count < 2)
                {
                    Debug.Log($"[Turn] gameState.HittedBallsIds.Count < 2. hittedBallsIds is {string.Join(",", gameState.HittedBallsIds.Select(id => id.ToString()))}");
                    return true;
                }

                Debug.Log($"[Turn] gameState.needToChangeTurn is false");
                return false;
            }
        }

        public override void OnEndTime()
        {
            UnityEngine.Debug.Log("OnEndPlayTime");
            gameState.cueBallInHand = true;
            string info = AightBallPoolPlayer.mainPlayer.myTurn ? "You run out of time\n" + AightBallPoolPlayer.otherPlayer.name + " has cue ball in hand" : AightBallPoolPlayer.otherPlayer.name + " run out of time, \nYou have cue ball in hand ";
            BallPoolGameManager.instance.SetGameInfo(info);
        }

        public override void Deactivate()
        {
            base.Deactivate();

            OnFoul = null;
            _gameState = null;
        }

        public static Ball GetCueBall(Ball[] balls)
        {
            foreach (Ball ball in balls)
            {
                if (isCueBall(ball.id))
                {
                    return ball;
                }
            }
            return null;
        }

        public static bool isControllingCueball(int id)
        {
            if (LevelLoader.CurrentMatchInfo.playingType != PlayType.Multi)
                return id == 0;

            if (controlFromNetwork)
                return isOpponentCueball(id);

            return isCueBall(id);
        }

        public static bool isCueBall(int id, bool useNetworkCondition = false)
        {
            if (LevelLoader.CurrentMatchInfo.playingType != PlayType.Multi)
                return id == 0;

            //if Multiplayer
            if (useNetworkCondition)
            {
                if (controlFromNetwork)
                {
                    //reverse
                    return Photon.Pun.PhotonNetwork.LocalPlayer.IsMasterClient ? id == 0 : id == 1;
                }
                else
                {
                    return Photon.Pun.PhotonNetwork.LocalPlayer.IsMasterClient ? id == 1 : id == 0;
                }
            }
            else
            {
                //default
                return Photon.Pun.PhotonNetwork.LocalPlayer.IsMasterClient ? id == 1 : id == 0;
            }
        }

        public static bool isOpponentCueball(in int id, bool useNetworkCondition = false)
        {
            if (LevelLoader.CurrentMatchInfo.playingType != PlayType.Multi)
                return id == 1;

            if (useNetworkCondition)
            {
                if (controlFromNetwork)
                {
                    //reverse
                    return Photon.Pun.PhotonNetwork.LocalPlayer.IsMasterClient ? id == 1 : id == 0;
                }
                else
                {
                    return Photon.Pun.PhotonNetwork.LocalPlayer.IsMasterClient ? id == 0 : id == 1;
                }
            }
            else
            {

                //default
                return Photon.Pun.PhotonNetwork.LocalPlayer.IsMasterClient ? id == 0 : id == 1;
            }
        }
    }
}
