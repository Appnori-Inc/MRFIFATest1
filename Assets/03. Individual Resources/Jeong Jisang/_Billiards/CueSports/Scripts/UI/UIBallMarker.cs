using UnityEngine;
using System.Collections;

namespace Billiards
{
    using BallPool.Mechanics;
    using System;

    [RequireComponent(typeof(Ball))]
    public class UIBallMarker : MonoBehaviour
    {
        [SerializeField]
        private Transform Root;

        [SerializeField]
        private Ball cachedBall = null;

        [SerializeField]
        private GameObject CueballMarker;

        [SerializeField]
        private GameObject defaultMarker;

        private int id { get => cachedBall.id; }
        private bool isTarget = false;

        private void Awake()
        {
            if (cachedBall == null)
                cachedBall = GetComponent<Ball>();

            Root.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            BallPollSetting();
            CaromSetting();
        }

        private void BallPollSetting()
        {
            if (LevelLoader.CurrentMatchInfo.gameType != GameType.PocketBilliards)
                return;

            BilliardsDataContainer.Instance.GameState.OnDataChanged += GameState_OnDataChanged;
            BilliardsDataContainer.Instance.TargetBallIds.OnDataChanged += TargetBallIds_OnDataChanged;

            if (BallPool.AightBallPoolGameLogic.isCueBall(id))
            {
                BilliardsDataContainer.Instance.CueSnapState.OnDataChanged += CueSnapState_OnDataChanged;
            }
        }

        private void CaromSetting()
        {
            if (LevelLoader.CurrentMatchInfo.gameType == GameType.PocketBilliards)
                return;

            if (BallPool.CaromGameLogic.isCueBall(id))
            {
                if (CueballMarker != null) CueballMarker.SetActive(true);
                if (defaultMarker != null) defaultMarker.SetActive(false);

                BilliardsDataContainer.Instance.GameState.OnDataChanged += GameState_OnDataChanged;
                BilliardsDataContainer.Instance.CueSnapState.OnDataChanged += CueSnapState_OnDataChanged;
            }
            else
            {
                BilliardsDataContainer.Instance.HittedBallIds.OnDataChanged += Carom_HittedBallIds_OnDataChanged;

                if (CueballMarker != null) CueballMarker.SetActive(false);
                if (defaultMarker != null) defaultMarker.SetActive(true);
            }
        }


        private void Carom_HittedBallIds_OnDataChanged(int totalData)
        {
            //carom Only
            if (BallPool.CaromGameLogic.isCueBall(id))
                return;

            isTarget = Contains(totalData, id);

            Root.gameObject.SetActive(isTarget);
        }

        private void CueSnapState_OnDataChanged(bool isSnapped)
        {
            if(isSnapped)
            {
                Root.gameObject.SetActive(false);
            }
            else
            {
                //renew
                GameState_OnDataChanged(BilliardsDataContainer.Instance.GameState.Value);
            }
        }

        private void TargetBallIds_OnDataChanged(int totalData)
        {
            isTarget = Contains(totalData, id);

            //renew
            GameState_OnDataChanged(BilliardsDataContainer.Instance.GameState.Value);
        }

        private void GameState_OnDataChanged(BallPool.ShotController.GameStateType obj)
        {
            switch (obj)
            {
                //case BallPool.ShotController.GameStateType.MoveAroundTable when LevelLoader.CurrentMatchInfo.gameType is not GameType.PocketBilliards:
                //case BallPool.ShotController.GameStateType.WaitingForOpponent when LevelLoader.CurrentMatchInfo.gameType is not GameType.PocketBilliards:
                //    Root.gameObject.SetActive(false);
                //    break;


                case BallPool.ShotController.GameStateType.SetBallPosition:
                    Root.gameObject.SetActive(isTarget);
                    break;

                case BallPool.ShotController.GameStateType.SelectShotDirection:
                    if (BallPool.AightBallPoolGameLogic.isCueBall(id))
                        Root.gameObject.SetActive(BilliardsDataContainer.Instance.CueSnapState.Value == false);
                    else
                        Root.gameObject.SetActive(isTarget);
                    break;


                case BallPool.ShotController.GameStateType.CameraFixAndWaitShot:
                    if (!BallPool.AightBallPoolGameLogic.isCueBall(id))
                        Root.gameObject.SetActive(false);
                    break;

                default:
                    break;
            }
        }

        private void OnDisable()
        {
            if (BallPool.AightBallPoolGameLogic.isCueBall(id))
            {
                BilliardsDataContainer.Instance.CueSnapState.OnDataChanged -= CueSnapState_OnDataChanged;
            }

            BilliardsDataContainer.Instance.HittedBallIds.OnDataChanged -= Carom_HittedBallIds_OnDataChanged;
            BilliardsDataContainer.Instance.TargetBallIds.OnDataChanged -= TargetBallIds_OnDataChanged;
            BilliardsDataContainer.Instance.GameState.OnDataChanged -= GameState_OnDataChanged;
        }

        private bool Contains(int value, int id)
        {
            return (value & 1 << id) != 0;
        }
    }
}