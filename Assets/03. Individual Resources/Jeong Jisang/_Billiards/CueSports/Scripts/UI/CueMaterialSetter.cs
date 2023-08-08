using UnityEngine;
using System.Collections;

namespace Billiards
{
    using System;
    using System.Collections.Generic;

    public class CueMaterialSetter : MonoBehaviour
    {

        private enum MaterialType
        {
            WhiteTransparent = 0,
            OriginalColor = 1,
            OriginalTransparent = 2,
            Transparent = 3,
        }


        [Serializable]
        public class CueMaterialPair
        {
            [SerializeField] public Material material_1;
            [SerializeField] public Material material_2;

            public Material[] ToArray()
            {
                return new Material[] { material_1, material_2 };
            }
        }

        [SerializeField]
        private List<CueMaterialPair> cueMaterials = new List<CueMaterialPair>();

        [SerializeField]
        private Renderer CueRenderer;

        private BallPool.ShotController.GameStateType currentState;

        private bool isNetworkControl { get => BallPool.AightBallPoolGameLogic.playMode == BallPool.PlayMode.OnLine && !BallPool.AightBallPoolPlayer.mainPlayer.myTurn; }


        private void OnEnable()
        {
            BilliardsDataContainer.Instance.GameState.OnDataChanged += GameState_OnDataChanged;
            BilliardsDataContainer.Instance.OpponentGameState.OnDataChangedDelta += OpponentGameState_OnDataChangedDelta;

            GameState_OnDataChanged(BilliardsDataContainer.Instance.GameState.Value);
        }

        private void OpponentGameState_OnDataChangedDelta(BallPool.ShotController.GameStateType prev, BallPool.ShotController.GameStateType next)
        {
            if (!isNetworkControl)
                return;

            switch (prev)
            {
                case BallPool.ShotController.GameStateType.SetBallPosition:
                    CueRenderer.materials = cueMaterials[(int)MaterialType.OriginalColor].ToArray();
                    break;
            }

            switch (next)
            {
                case BallPool.ShotController.GameStateType.SetBallPosition:
                    CueRenderer.materials = cueMaterials[(int)MaterialType.Transparent].ToArray();
                    break;

                default:
                    CueRenderer.materials = cueMaterials[(int)MaterialType.OriginalColor].ToArray();
                    break;
            }
        }

        private void GameState_OnDataChanged(BallPool.ShotController.GameStateType obj)
        {
            if (isNetworkControl)
                return;

            switch (obj)
            {
                case BallPool.ShotController.GameStateType.SetBallPosition:
                    CueRenderer.materials = cueMaterials[(int)MaterialType.Transparent].ToArray();
                    break;

                case BallPool.ShotController.GameStateType.WaitingForOpponent:
                    CueRenderer.materials = cueMaterials[(int)MaterialType.OriginalColor].ToArray();
                    break;

                case BallPool.ShotController.GameStateType.SelectShotDirection:
                case BallPool.ShotController.GameStateType.CameraFixAndWaitShot:
                    /*DO NOTHING*/
                    break;

                case BallPool.ShotController.GameStateType.Shot:
                    CueRenderer.materials = cueMaterials[(int)MaterialType.OriginalTransparent].ToArray();
                    break;

                default:
                    CueRenderer.materials = cueMaterials[(int)MaterialType.WhiteTransparent].ToArray();
                    break;
            }

            currentState = obj;
        }

        private void Update()
        {
            switch (currentState)
            {
                case BallPool.ShotController.GameStateType.SelectShotDirection:
                {
                    if (BallPool.ShotController.SubHandController[UnityEngine.XR.Interaction.Toolkit.InputHelpers.Button.Trigger].Value)
                    {
                        CueRenderer.materials = cueMaterials[(int)MaterialType.OriginalTransparent].ToArray();
                    }
                    else
                    {
                        CueRenderer.materials = cueMaterials[(int)MaterialType.WhiteTransparent].ToArray();
                    }
                    break;
                }

                case BallPool.ShotController.GameStateType.CameraFixAndWaitShot:
                {
                    if (BilliardsDataContainer.Instance.XRLeftControllerState[UnityEngine.XR.Interaction.Toolkit.InputHelpers.Button.Trigger].Value == true &&
                    BilliardsDataContainer.Instance.XRRightControllerState[UnityEngine.XR.Interaction.Toolkit.InputHelpers.Button.Trigger].Value == true &&
                    BilliardsDataContainer.Instance.AllowedSetCuePositionState.Value == true)
                    {
                        CueRenderer.materials = cueMaterials[(int)MaterialType.OriginalTransparent].ToArray();
                    }
                    else
                    {
                        CueRenderer.materials = cueMaterials[(int)MaterialType.OriginalColor].ToArray();
                    }
                    break;
                }

                default:
                    break;
            }
        }

        private void OnDisable()
        {
            BilliardsDataContainer.Instance.OpponentGameState.OnDataChangedDelta -= OpponentGameState_OnDataChangedDelta;
            BilliardsDataContainer.Instance.GameState.OnDataChanged -= GameState_OnDataChanged;
        }



    }

}