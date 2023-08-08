using System.Collections;
using System.Collections.Generic;
using System;
using BallPool.Mechanics;
using NetworkManagement;

namespace BallPool
{
    public class AightBallPoolGameManager : BallPoolGameManager
    {
        private bool onBallHitPocket = false;
        private bool playAgainMenuIsActive;

        public AightBallPoolGameLogic gameLogic { get; set; }

        public AightBallPoolGameManager()
        {
            gameLogic = new AightBallPoolGameLogic();
        }

        public override void Start()
        {
            playAgainMenuIsActive = false;
            physicsManager.OnBallHitBall += PhysicsManager_OnBallHitBall;
            physicsManager.OnBallHitBoard += PhysicsManager_OnBallHitBoard;
            physicsManager.OnBallHitPocket += PhysicsManager_OnBallHitPocket;

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
            AightBallPoolGameLogic.gameState.gameIsComplete = true;
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
            gameLogic.OnEndShot(AightBallPoolGameLogic.GetBlackBall(balls).inPocket, out gameIsEnd);

            if (gameIsEnd)
            {
                OpenPlayAgainMenu();
            }
            else if (!playAgainMenuIsActive)
            {
                CallOnEndShot();
                if (AightBallPoolGameLogic.gameState.needToChangeTurn)
                {
                    BallPoolPlayer.ChangeTurn();

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
            bool isCueBall = AightBallPoolGameLogic.isCueBall(ball.id);

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

        void PhysicsManager_OnBallHitPocket(BallListener ball, PocketListener pocket, bool inMove)
        {
            if (!inMove)
            {
                return;
            }
            balls[ball.id].OnState(BallState.EnterInPocket);
            bool cueBallInPocket = false;
            gameLogic.OnBallInPocket(ball.id, ref cueBallInPocket, pocket);
            if (!cueBallInPocket)
            {
                onBallHitPocket = true;
            }
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
