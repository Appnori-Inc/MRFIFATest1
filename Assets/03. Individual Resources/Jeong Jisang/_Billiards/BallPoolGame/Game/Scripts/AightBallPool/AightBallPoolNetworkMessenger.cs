using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BallPool;

namespace NetworkManagement
{
    using GameManager = BallPool.GameManager;

    public interface AightBallPoolMessenger
    {
        void OnSendCueControl(Vector3 cuePivotPosition, float cuePivotLocalRotationY, float cueVerticalLocalRotationX, Vector2 cueDisplacementLocalPositionXY, float cueSliderLocalPositionZ, float force);
    }
    public class AightBallPoolNetworkMessenger : NetworkMessenger, AightBallPoolMessenger
    {
        private ShotController _shotController;
        private ShotController shotController
        {
            get
            {
                if (!_shotController)
                {
                    _shotController = ShotController.FindObjectOfType<ShotController>();
                }
                return _shotController;
            }
        }
        private GameManager _gameManager;
        private GameManager gameManager
        {
            get
            {
                if (!_gameManager)
                {
                    _gameManager = GameManager.FindObjectOfType<GameManager>();
                }
                return _gameManager;
            }
        }
        #region sended from network
        public void SetTime(float time01)
        {
            if (BallPoolGameLogic.controlFromNetwork)
            {
                BallPoolGameManager.instance.SetPlayTime(time01);
            }
        }
        public void SetOpponentCueURL(string url)
        {
            shotController.SetOpponentCueURL(url);
        }
        public void SetOpponentTableURLs(string boardURL, string clothURL, string clothColor)
        {
            shotController.SetOpponentTableURLs(boardURL, clothURL, clothColor);
        }

        public IEnumerator OnOpponenInGameScene()
        {
            while (!shotController)
            {
                yield return null;
            }
            shotController.OpponenIsReadToPlay();
        }
        public void OnOpponentForceGoHome()
        {
            Debug.LogWarning("OnOpponentForceGoHome");
            BallPoolGameManager.instance.OnForceGoHome(AightBallPoolPlayer.mainPlayer.playerId);
        }
        public void OnSendGameState(int gameState)
        {
            if (shotController)
            {
                shotController.OpponentStateFormNetwork(gameState);
            }
        }

        public void OnSendMainHanded(bool isRightHanded)
        {
            if (shotController)
            {
                shotController.OpponentMainHandFormNetwork(isRightHanded);
            }
        }
        public void OnSendCueControl(Vector3 cuePivotPosition, float cuePivotLocalRotationY, float cueVerticalLocalRotationX, Vector2 cueDisplacementLocalPositionXY, float cueSliderLocalPositionZ, float force)
        {
            if (shotController)
            {
                shotController.CueControlFromNetwork(cuePivotPosition, cuePivotLocalRotationY, cueVerticalLocalRotationX, cueDisplacementLocalPositionXY, cueSliderLocalPositionZ, force);
            }
        }
        public void OnForceSendCueControl(Vector3 cuePivotPosition, float cuePivotLocalRotationY, float cueVerticalLocalRotationX, Vector2 cueDisplacementLocalPositionXY, float cueSliderLocalPositionZ, float force)
        {
            if (shotController)
            {
                shotController.ForceCueControlFromNetwork(cuePivotPosition, cuePivotLocalRotationY, cueVerticalLocalRotationX, cueDisplacementLocalPositionXY, cueSliderLocalPositionZ, force);
            }
        }
        public void OnMoveBall(Vector3 ballPosition)
        {
            if (shotController)
            {
                shotController.MoveBallFromNetwork(ballPosition);
            }
        }
        public void SelectBallPosition(Vector3 ballPosition)
        {
            shotController.SelectBallPositionFromNetwork(ballPosition);
        }
        public void SetBallPosition(Vector3 ballPosition)
        {
            shotController.SetBallPositionFromNetwork(ballPosition);
        }

        public void SetMechanicalStatesFromNetwork(int ballId, string mechanicalStateData)
        {
            StartCoroutine(gameManager.balls[ballId].SetMechanicalStatesFromNetwork(mechanicalStateData));
        }
        public void WaitAndStopMoveFromNetwork(float time)
        {
            StartCoroutine(shotController.physicsManager.WaitAndStopMoveFromNetwork(time));
        }
        public void StartSimulate(string impulse)
        {
            shotController.physicsManager.StarShotFromNetwork(impulse);
        }
        public void EndSimulate(string data)
        {
            shotController.physicsManager.CheckEndShot(data);
        }

        public void SetPlayerTransform(string uuid, Vector3 worldPosition, Quaternion rotation, Vector3 worldPosition2, Quaternion rotation2, Vector3 worldPosition3, Quaternion rotation3)
        {
            gameManager.playerManager.SetPlayerTransform(uuid,
                new List<Vector3>() { worldPosition, worldPosition2, worldPosition3 },
                new List<Quaternion>() { rotation, rotation2, rotation3 });
        }

        public void SetPlayerScore(in string uuid, in int score, in int targetScore)
        {
            gameManager.SetPlayerScore(uuid, score, targetScore);
        }

        public void SetFoul(int FoulType, int ballId, int pocketId)
        {
            AightBallPoolGameLogic.SetFoulFormNetwork(FoulType, ballId, pocketId);
        }

        public void SetRequestRematch(bool isRequested)
        {
            if (gameManager != null)
                gameManager.RequestRematch = isRequested;
        }
        #endregion
    }
}
