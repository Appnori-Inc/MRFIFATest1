using BallPool.Mechanics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Billiards
{
    public class UIHitPoint : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer spriteRenderer;

        [SerializeField]
        private Transform Cue;

        private Ball ball;

        private float distance;

        private void Awake()
        {
            distance = Vector3.Distance(spriteRenderer.transform.position, transform.position);

            ball = GetComponentInParent<Ball>();

            BilliardsDataContainer.Instance.GameState.OnDataChanged += GameState_OnDataChanged;
            BilliardsDataContainer.Instance.CueSnapState.OnDataChanged += CueSnapState_OnDataChanged;
            GameState_OnDataChanged(BilliardsDataContainer.Instance.GameState.Value);
        }

        private void CueSnapState_OnDataChanged(bool snap)
        {
            if (snap)
            {
                GameState_OnDataChanged(BilliardsDataContainer.Instance.GameState.Value);
            }
            else
            {
                spriteRenderer.enabled = false;
            }
        }

        private void GameState_OnDataChanged(BallPool.ShotController.GameStateType obj)
        {
            //Exception
            if (LevelLoader.CurrentMatchInfo.gameType != GameType.PocketBilliards)
            {
                if (!BallPool.CaromGameLogic.isCueBall(ball.id))
                {
                    spriteRenderer.enabled = false;
                    return;
                }
            }

            switch (obj)
            {
                case BallPool.ShotController.GameStateType.SelectShotDirection:
                case BallPool.ShotController.GameStateType.CameraFixAndWaitShot:
                    spriteRenderer.enabled = BilliardsDataContainer.Instance.CueSnapState.Value;
                    break;

                default:
                    spriteRenderer.enabled = false;
                    break;
            }
        }

        // Update is called once per frame
        void Update()
        {
            distance = Vector3.Distance(spriteRenderer.transform.position, transform.position);

            var pos = VectorExtension.IntersectionPoint(Cue.position, Cue.position - Cue.forward, transform.position, distance);

            transform.LookAt(pos);
        }


        private void OnDestroy()
        {
            BilliardsDataContainer.Instance.CueSnapState.OnDataChanged -= CueSnapState_OnDataChanged;
            BilliardsDataContainer.Instance.GameState.OnDataChanged -= GameState_OnDataChanged;
        }
    }

}