using System.Collections;
using System.Collections.Generic;
using System;
using BallPool.Mechanics;
using NetworkManagement;
using Billiards;

namespace BallPool
{
    public class CaromFourGameManager : BallPoolGameManager
    {
        private bool onBallHitPocket = false;
        private bool playAgainMenuIsActive;


        public CaromFourGameLogic gameLogic { get; set; }


        public CaromFourGameManager()
        {
            gameLogic = new CaromFourGameLogic();
        }

        public override void Start()
        {
            playAgainMenuIsActive = false;
            physicsManager.OnBallHitBall += PhysicsManager_OnBallHitBall;
            physicsManager.OnBallHitBoard += PhysicsManager_OnBallHitBoard;

            BallPoolPlayer.OnTurnChanged += Player_OnTurnChanged;

            if (!BallPoolGameLogic.isOnLine)
            {
                BallPoolPlayer.turnId = 0;
            }

            BallPoolPlayer.SetTurn(BallPoolPlayer.turnId);

            UpdateActiveBalls();

            CallOnSetPlayer(AightBallPoolPlayer.mainPlayer);
            CallOnSetPlayer(AightBallPoolPlayer.otherPlayer);

            CallOnSetAvatar(AightBallPoolPlayer.mainPlayer);
            CallOnSetAvatar(AightBallPoolPlayer.otherPlayer);

        }

        public override void Update(float deltaTime)
        {
            if (onBallHitPocket)
            {
                onBallHitPocket = false;
                UpdateActiveBalls();
            }
            CallOnUpdateTime(deltaTime);
        }

        public override void OnDisable()
        {
            base.OnDisable();
            BallPoolPlayer.Deactivate();
            BallPoolGameLogic.instance.Deactivate();
        }

        private void OpenPlayAgainMenu()
        {
            CaromFourGameLogic.gameState.gameIsComplete = true;
            playAgainMenuIsActive = true;
            CallOnGameComplite();
            if ( !NetworkManager.mainPlayer.canPlayOffline || ( BallPoolGameLogic.isOnLine && !NetworkManager.mainPlayer.canPlayOnLine ) )
            {
                playAgainMenu.HidePlayAgainButton();
            }
        }

        public void UpdateActiveBalls()
        {
            AightBallPoolPlayer.mainPlayer.SetActiveBalls(balls);
            AightBallPoolPlayer.otherPlayer.SetActiveBalls(balls);

            CallOnSetActiveBallsIds(AightBallPoolPlayer.mainPlayer);
            CallOnSetActiveBallsIds(AightBallPoolPlayer.otherPlayer);
        }

        public override void OnStartShot(string data)
        {
            base.OnStartShot(data);
            gameLogic.ResetState();
        }
        public override void OnEndShot(string data)
        {
            base.OnEndShot(data);
            bool gameIsEnd;
            gameLogic.OnEndShot(out gameIsEnd);

            if (gameIsEnd)
            {
                OpenPlayAgainMenu();
            }
            else if (!playAgainMenuIsActive)
            {
                CallOnEndShot();
                if (CaromFourGameLogic.gameState.needToChangeTurn)
                {
                    if (BallPoolGameLogic.playMode == PlayMode.OnLine)
                    {
                        BallPoolPlayer.ChangeTurn();
                    }
                    else
                    {
                        AightBallPoolBallsSortingManager.Sorter.CaromResetBall();
                    }

                    gameLogic.CheckAchievement(-1);
                }
                if (BallPoolGameLogic.playMode == PlayMode.PlayerAI && AightBallPoolPlayer.otherPlayer.myTurn)
                {
                    CallOnCalculateAI();
                }
            }
        }

        private ShotController _shotController;
        public ShotController shotController
        {
            get
            {
                if (_shotController == null)
                {
                    _shotController = ShotController.FindObjectOfType<ShotController>();
                }
                return _shotController;
            }
        }
        void PhysicsManager_OnBallHitBall(BallListener ball, BallListener hitBall, bool inMove)
        {
            if (!inMove)
            {
                return;
            }
            if (shotController.tragetBallListener == hitBall)
            {
                hitBall.body.velocity = hitBall.body.velocity.magnitude * shotController.targetBallSavedDirection;
                shotController.tragetBallListener = null;
            }
            balls[ball.id].OnState(BallState.HitBall);
            bool isCueBall = CaromFourGameLogic.isCueBall(ball.id, true);

            if (isCueBall)
            {
                gameLogic.OnCueBallHitBall(ball.id, hitBall.id);
            }
        }

        void PhysicsManager_OnBallHitBoard(BallListener ball, bool inMove)
        {
            if (!inMove)
            {
                return;
            }
            balls[ball.id].OnState(BallState.HitBoard);
            gameLogic.OnBallHitBoard(ball.id);
        }

        public override void OnForceGoHome(int winnerId)
        {

        }

        private void Player_OnTurnChanged()
        {
            UnityEngine.Debug.Log("Player_OnTurnChanged - Opponent State : " + Billiards.BilliardsDataContainer.Instance.OpponentGameState.Value);

            if (!playAgainMenuIsActive)
            {
                CallOnEnableControl(BallPoolPlayer.mainPlayer.myTurn || BallPoolGameLogic.playMode == PlayMode.HotSeat);

                CallOnSetActivePlayer(AightBallPoolPlayer.mainPlayer, BallPoolPlayer.turnId == AightBallPoolPlayer.mainPlayer.playerId);
                CallOnSetActivePlayer(AightBallPoolPlayer.otherPlayer, BallPoolPlayer.turnId == AightBallPoolPlayer.otherPlayer.playerId);

                if (BallPoolGameLogic.playMode == PlayMode.PlayerAI && AightBallPoolPlayer.otherPlayer.myTurn)
                {
                    CallOnCalculateAI();
                }
            }
        }
    }
}
