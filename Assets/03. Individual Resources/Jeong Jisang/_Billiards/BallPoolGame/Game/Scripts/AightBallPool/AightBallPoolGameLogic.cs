using System.Collections;
using System.Collections.Generic;
using BallPool.Mechanics;
using UnityEngine;

namespace BallPool
{
    public class AightBallPoolGameState : GameState
    {
        /// <summary>
        /// The table will close after first shot.
        /// </summary>
        public bool tableIsOpened = true;
        /// <summary>
        /// The type of the players is e stripes or solids.
        /// </summary>
        public bool playersHasBallType = false;
        /// <summary>
        /// Balls hit board count in first shot.
        /// </summary>
        public int ballsHitBoardCount = 0;
        /// <summary>
        /// The cue ball has hit right ball, stripes, solids or black.
        /// </summary>
        public bool cueBallHasHitRightBall = false;
        /// <summary>
        /// The cue ball has hit some ball at first hit.
        /// </summary>
        public bool cueBallHasHitSomeBall = false;
        /// <summary>
        /// The cue ball has right ball in pocket, stripes, solids or black.
        /// </summary>
        public bool hasRightBallInPocket = false;
        /// <summary>
        /// The current player has cue ball in his hand
        /// </summary>
        public bool cueBallInHand = true;
        /// <summary>
        /// The cue ball in pocket.
        /// </summary>
        public bool cueBallInPocket = false;

        /// <summary>
        /// The cue ball was moved in this turn.
        /// </summary>
        public bool cueBallMoved = false;

        /// <summary>
        /// The cue ball was hit board and hit right ball then right ball pocketed
        /// </summary>
        public int BankShotStack = 0;

        public bool HatTrickReported = false;
        public int HatTrickStack = 0;

        public List<int> BankShotContactBallIds;
        public List<int> PocketedBallIds;

        //for carom

        public int cueballHitBoardCount = 0;
        public List<int> HittedBallsIds;
        public Dictionary<int, float> cueballhitTime;



        public AightBallPoolGameState()
            : base()
        {
            tableIsOpened = true;
            playersHasBallType = false;
            ballsHitBoardCount = 0;
            cueBallHasHitRightBall = false;
            cueBallHasHitSomeBall = false;
            hasRightBallInPocket = false;
            cueBallInHand = false;
            cueBallInPocket = false;
            cueBallMoved = false;

            BankShotStack = 0;
            BankShotContactBallIds = new List<int>();
            PocketedBallIds = new List<int>();

            HatTrickReported = false;
            HatTrickStack = 0;

            cueballHitBoardCount = 0;
            HittedBallsIds = new List<int>();
            cueballhitTime = new Dictionary<int, float>();
        }
    }

    public class AightBallPoolGameLogic : BallPoolGameLogic
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

        public AightBallPoolGameLogic() : base()
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

        public enum FoulType
        {
            None = 0,
            Scratch = 1, // PocketListner
            BadHit = 2, // BallListener
            NoHit = 3, //BallListener
            WeakBreak = 4,//BallListener

            //not implemented
            NoRail = 4,
            OutOfTable = 5,
        }

        public static bool isFoulCalled { get; private set; }
        public static event System.Action<FoulType, (BallListener, PocketListener), float> OnFoulWithDelay;
        public static event System.Action<FoulType, (BallListener, PocketListener)> OnFoul;

        public static void SetFoulFormNetwork(int foulType, int ballId, int pocketId)
        {
            BallListener targetBall = null;
            PocketListener pocketListener = null;
            if (ballId != -1)
            {
                targetBall = AightBallPoolGameManager.instance.balls[ballId].listener;
            }
            if (pocketId != -1)
            {
                pocketListener = AightBallPoolGameManager.instance.physicsManager.pocketListeners[pocketId];
            }

            switch ((FoulType)foulType)
            {
                case FoulType.BadHit:
                case FoulType.Scratch:
                    OnFoulWithDelay?.Invoke((FoulType)foulType, (targetBall, pocketListener), 1f); //network sync delayTime = 1f
                    break;

                default:
                    OnFoul?.Invoke((FoulType)foulType, (targetBall, pocketListener));
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
            if (gameState.tableIsOpened)
            {
                gameState.ballsHitBoardCount++;
            }
            else
            {
                //achievement - bankshot
                if (isCueBall(ballId))
                    CheckAchievement(0);
            }
        }

        public void OnCueBallHitBall(int cueBallId, int ballId)
        {
            if (gameState.cueBallHasHitSomeBall)
            {
                return;
            }
            gameState.cueBallHasHitSomeBall = true;

            if (isBlackBall(ballId))
            {
                if (((AightBallPoolPlayer)BallPoolPlayer.currentPlayer).isBlack)
                {
                    gameState.cueBallHasHitRightBall = true;
                }
            }
            else if (gameState.tableIsOpened)
            {
                gameState.cueBallHasHitRightBall = true;
            }
            else if (!gameState.playersHasBallType)
            {
                gameState.cueBallHasHitRightBall = true;
            }
            else if (AightBallPoolPlayer.PlayerHasSomeBallType((AightBallPoolPlayer)BallPoolPlayer.currentPlayer, ballId))
            {
                gameState.cueBallHasHitRightBall = true;
            }
            if (!gameState.cueBallHasHitRightBall)
            {
                gameState.cueBallInHand = true;
                gameState.needToChangeTurn = true;

                //Event : Foul
                if (!isFoulCalled)
                {
                    OnFoul?.Invoke(FoulType.BadHit, (AightBallPoolGameManager.instance.balls[ballId].listener, null));
                }
            }

            //achievement - bankshot
            if (gameState.cueBallHasHitRightBall && !gameState.tableIsOpened)
                CheckAchievement(1, ballId);
        }

        public void OnBallInPocket(int ballId, ref bool cueBallInPocket, PocketListener pocket)
        {
            if (isCueBall(ballId))
            {
                if (AightBallPoolPlayer.mainPlayer.myTurn)
                {
                    AightBallPoolPlayer.mainPlayer.isCueBall = true;
                }
                else if (AightBallPoolPlayer.otherPlayer.myTurn)
                {
                    AightBallPoolPlayer.otherPlayer.isCueBall = true;
                }
                gameState.needToChangeTurn = true;
                gameState.cueBallInHand = true;
                cueBallInPocket = true;
                gameState.cueBallInPocket = true;

                //Event : Foul
                if (!isFoulCalled)
                {
                    OnFoul?.Invoke(FoulType.Scratch,(null,pocket));
                }

                return;
            }
            if (gameState.tableIsOpened)
            {
                if (!isBlackBall(ballId))
                {
                    gameState.hasRightBallInPocket = true;
                }

                gameState.PocketedBallIds.Add(ballId);
            }
            else
            {
                if (!gameState.playersHasBallType)
                {
                    gameState.playersHasBallType = true;
                    if (!isBlackBall(ballId))
                    {
                        gameState.hasRightBallInPocket = true;
                    }
                    if (AightBallPoolPlayer.mainPlayer.myTurn)
                    {
                        if (AightBallPoolGameLogic.isStripesBall(ballId))
                        {
                            AightBallPoolPlayer.mainPlayer.isStripes = true;
                            AightBallPoolPlayer.otherPlayer.isSolids = true;
                        }
                        else if (AightBallPoolGameLogic.isSolidsBall(ballId))
                        {
                            AightBallPoolPlayer.mainPlayer.isSolids = true;
                            AightBallPoolPlayer.otherPlayer.isStripes = true;
                        }
                    }
                    else if (AightBallPoolPlayer.otherPlayer.myTurn)
                    {
                        if (AightBallPoolGameLogic.isStripesBall(ballId))
                        {
                            AightBallPoolPlayer.otherPlayer.isStripes = true;
                            AightBallPoolPlayer.mainPlayer.isSolids = true;
                        }
                        else if (AightBallPoolGameLogic.isSolidsBall(ballId))
                        {
                            AightBallPoolPlayer.otherPlayer.isSolids = true;
                            AightBallPoolPlayer.mainPlayer.isStripes = true;
                        }
                    }
                }
                else if (AightBallPoolPlayer.PlayerHasSomeBallType((AightBallPoolPlayer)BallPoolPlayer.currentPlayer, ballId))
                {
                    gameState.hasRightBallInPocket = true;

                    //if ((AightBallPoolPlayer)BallPoolPlayer.currentPlayer == AightBallPoolPlayer.mainPlayer)
                    {
                        //crowdSound
                        Billiards.SoundManager.PlayCheerSound();
                    }

                    //achievement - bankshot
                    CheckAchievement(2, ballId);
                }
            }
        }

        public void OnEndShot(bool blackBallInPocket, out bool gameIsEnd)
        {
            gameIsEnd = false;
            if (BallPoolGameManager.instance == null)
            {
                return;
            }
            string info = "";
           
            bool canSetInfo = true;

            if (gameState.cueBallInPocket && !blackBallInPocket)
            {
                if (BallPoolPlayer.mainPlayer.myTurn)
                {
                    info = "You pocket the cue ball, \n" + AightBallPoolPlayer.otherPlayer.name + " has cue ball in hand";
                }
                else
                {
                    info = AightBallPoolPlayer.otherPlayer.name + " pocket the cue ball, \n" + " You have cue ball in hand";
                }
                BallPoolGameManager.instance.SetGameInfo(info);
                canSetInfo = false;
            }
            if (gameState.tableIsOpened)
            {
                info = " ";

                if(gameState.cueBallHasHitRightBall && gameState.hasRightBallInPocket)
                {
                    gameState.playersHasBallType = true;

                    int stripesCount = 0;
                    int SolidsCount = 0;
                    foreach(var ball in AightBallPoolGameManager.instance.balls)
                    {
                        if (!ball.inPocket)
                        {
                            if (isStripesBall(ball.id))
                            {
                                stripesCount += 1;
                            }
                            else if (isSolidsBall(ball.id))
                            {
                                SolidsCount += 1;
                            }
                        }
                    }

                    if (AightBallPoolPlayer.mainPlayer.myTurn)
                    {
                        if (stripesCount < SolidsCount)
                        {
                            AightBallPoolPlayer.mainPlayer.isStripes = true;
                            AightBallPoolPlayer.otherPlayer.isSolids = true;
                        }
                        else if (stripesCount > SolidsCount)
                        {
                            AightBallPoolPlayer.mainPlayer.isSolids = true;
                            AightBallPoolPlayer.otherPlayer.isStripes = true;
                        }
                        else if (stripesCount == SolidsCount && gameState.PocketedBallIds.Count > 0)
                        {
                            var ball_id = gameState.PocketedBallIds[0];
                            if (isSolidsBall(ball_id))
                            {
                                AightBallPoolPlayer.mainPlayer.isSolids = true;
                                AightBallPoolPlayer.otherPlayer.isStripes = true;
                            }
                            else if (isStripesBall(ball_id))
                            {
                                AightBallPoolPlayer.mainPlayer.isStripes = true;
                                AightBallPoolPlayer.otherPlayer.isSolids = true;
                            }

                        }
                    }
                    else if (AightBallPoolPlayer.otherPlayer.myTurn)
                    {
                        if (stripesCount < SolidsCount)
                        {
                            AightBallPoolPlayer.otherPlayer.isStripes = true;
                            AightBallPoolPlayer.mainPlayer.isSolids = true;
                        }
                        else if (stripesCount > SolidsCount)
                        {
                            AightBallPoolPlayer.otherPlayer.isSolids = true;
                            AightBallPoolPlayer.mainPlayer.isStripes = true;
                        }
                        else if (stripesCount == SolidsCount && gameState.PocketedBallIds.Count > 0)
                        {
                            var ball_id = gameState.PocketedBallIds[0];
                            if (isSolidsBall(ball_id))
                            {
                                AightBallPoolPlayer.otherPlayer.isSolids = true;
                                AightBallPoolPlayer.mainPlayer.isStripes = true;
                            }
                            else if (isStripesBall(ball_id))
                            {
                                AightBallPoolPlayer.otherPlayer.isStripes = true;
                                AightBallPoolPlayer.mainPlayer.isSolids = true;
                            }
                        }
                    }

                    (AightBallPoolGameManager.instance as AightBallPoolGameManager).UpdateActiveBalls();
                }
                else 
                {
                    gameState.needToChangeTurn = true;
                    if (gameState.ballsHitBoardCount < 4)
                    {
                        gameState.cueBallInHand = true;
                        //Event : Foul
                        if (!isFoulCalled)
                        {
                            OnFoul?.Invoke(FoulType.WeakBreak, (AightBallPoolGameManager.instance.balls[8].listener, null));
                        }
                        info = "Break up of balls was weak, \n" + (BallPoolPlayer.mainPlayer.myTurn ? AightBallPoolPlayer.otherPlayer.name + " has cue ball in hand" : "You have cue ball in hand");
                    }
                }
            }
            else
            {

            }
            gameState.tableIsOpened = false;
            if (blackBallInPocket)
            {
                if (AightBallPoolPlayer.mainPlayer.myTurn)
                {
                    AightBallPoolPlayer.mainPlayer.isWinner = AightBallPoolPlayer.mainPlayer.isBlack && !AightBallPoolPlayer.mainPlayer.isCueBall;
                    AightBallPoolPlayer.otherPlayer.isWinner = !AightBallPoolPlayer.mainPlayer.isWinner;
                    info = AightBallPoolPlayer.otherPlayer.isWinner ? ("You poked the black ball " + (AightBallPoolPlayer.mainPlayer.isCueBall ? "with cue ball" : "")) : "";
                }
                else if (AightBallPoolPlayer.otherPlayer.myTurn)
                {
                    AightBallPoolPlayer.otherPlayer.isWinner = AightBallPoolPlayer.otherPlayer.isBlack && !AightBallPoolPlayer.otherPlayer.isCueBall;
                    AightBallPoolPlayer.mainPlayer.isWinner = !AightBallPoolPlayer.otherPlayer.isWinner;
                    info = AightBallPoolPlayer.mainPlayer.isWinner ? (AightBallPoolPlayer.otherPlayer.name + " pocket the black ball " + (AightBallPoolPlayer.otherPlayer.isCueBall ? "with cue ball" : "")) : "";
                } 
                gameState.needToChangeTurn = false;
                gameIsEnd = true;
                BallPoolGameManager.instance.SetGameInfo(info);
                return;
            }

            if (AightBallPoolPlayer.mainPlayer.checkIsBlackInEnd)
            {
                AightBallPoolPlayer.mainPlayer.isBlack = true;
            }
            if (AightBallPoolPlayer.otherPlayer.checkIsBlackInEnd)
            {
                AightBallPoolPlayer.otherPlayer.isBlack = true;
            }

            if (!gameState.cueBallHasHitRightBall)
            {
                gameState.cueBallInHand = true;
                gameState.needToChangeTurn = true;

                //Event : Foul
                if (!isFoulCalled)
                {
                    OnFoul?.Invoke(FoulType.NoHit, (AightBallPoolGameManager.instance.balls[0].listener, null));
                }

                if (AightBallPoolPlayer.mainPlayer.myTurn)
                {
                    if (info == "")
                        info = AightBallPoolPlayer.mainPlayer.isBlack ? "You need to hit black ball" : (AightBallPoolPlayer.mainPlayer.isSolids ? "You need to hit solids ball" : (AightBallPoolPlayer.mainPlayer.isStripes ? "You need to hit stripes ball" : "You need to hit solids or stripes ball")) +
                        "\n" + AightBallPoolPlayer.otherPlayer.name + " has cue ball in hand";
                }
                else
                {
                    if (info == "")
                        info = AightBallPoolPlayer.otherPlayer.name + ((AightBallPoolPlayer.otherPlayer.isBlack ? " need to hit black ball" : (AightBallPoolPlayer.otherPlayer.isSolids ? " need to hit solids ball" : (AightBallPoolPlayer.otherPlayer.isStripes ? " need to hit stripes ball" : " need to hit solids or stripes ball")))) +
                        ", \nYou have cue ball in hand";
                }
            }
            else if (!gameState.hasRightBallInPocket)
            {
                gameState.needToChangeTurn = true;
                //gameState.cueBallInHand = true;

                if (AightBallPoolPlayer.mainPlayer.myTurn)
                {
                    if (info == "")
                        info = AightBallPoolPlayer.mainPlayer.isBlack ? "You need to pocket solids ball" : (AightBallPoolPlayer.mainPlayer.isSolids ? "You need to pocket solids ball" : (AightBallPoolPlayer.mainPlayer.isStripes ? "You need to pocket stripes ball" : "You need to pocket solids or stripes ball"));
                }
                else
                {
                    if (info == "")
                        info = AightBallPoolPlayer.otherPlayer.name + (AightBallPoolPlayer.otherPlayer.isBlack ? " need to pocket black ball" : ((AightBallPoolPlayer.otherPlayer.isSolids ? " need to pocket solids ball" : (AightBallPoolPlayer.otherPlayer.isStripes ? " need to pocket stripes ball" : " need to pocket solids or stripes ball"))));
                }
            }
            if (canSetInfo)
            {
                BallPoolGameManager.instance.SetGameInfo(info);
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

        /// <summary>
        /// Is the ball in pocket?, not for the cue ball, the cue ball will be resetted
        /// </summary>
        public static bool ballInPocket(Ball ball)
        {
            return ball.inPocket;
        }

        public static Ball GetBlackBall(Ball[] balls)
        {
            foreach (Ball ball in balls)
            {
                if (isBlackBall(ball.id))
                {
                    return ball;
                }
            }
            return null;
        }

        public static bool isCueBall(int id)
        {
            return id == 0;
        }

        public static bool isBlackBall(int id)
        {
            return id == 8;
        }

        public static bool isStripesBall(int id)
        {
            return id > 8 && id < 16;
        }

        public static bool isSolidsBall(int id)
        {
            return id > 0 && id < 8;
        }
    }
}
