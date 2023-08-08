using UnityEngine;
using System.Collections;

namespace Billiards
{
    using BallPool;
    using System.Collections.Generic;
    using System.Linq;

    public class PlayerHand : MonoBehaviour
    {
        [SerializeField]
        private Material OpaqueHandMaterial;

        [SerializeField]
        private Material TransparentHandMaterial;
        [SerializeField]
        private Material TransparentHandMaterial_left;

        [SerializeField]
        private Renderer leftCueBridgeHand;
        [SerializeField]
        private Renderer leftCueGripHand;
        [SerializeField]
        private GameObject leftHandRoot;
        [SerializeField]
        private GameObject rightHandRoot;

        [SerializeField]
        private Renderer rightCueHand;

        [SerializeField]
        private Renderer freeLeftHand;
        [SerializeField]
        private Renderer freeRightHand;


        public Renderer LeftCueBridgeHand { get => leftCueBridgeHand; }
        public Renderer RightCueHand { get => rightCueHand; }

        public Renderer LeftFreeHand { get => freeLeftHand; }
        public Renderer RightFreeHand { get => freeRightHand; }

        private Color PlayerSkinColor;
        private Color TransparentBlueColor;

        private bool isMyturn { get => BallPool.AightBallPoolPlayer.mainPlayer.myTurn; }
        private bool isNetworkControl { get => BallPool.AightBallPoolGameLogic.playMode == BallPool.PlayMode.OnLine && !BallPool.AightBallPoolPlayer.mainPlayer.myTurn; }

        private void Awake()
        {
            BilliardsDataContainer.Instance.GameState.OnDataChanged += GameState_OnDataChanged;
            BilliardsDataContainer.Instance.OpponentGameState.OnDataChanged += OpponentGameState_OnDataChanged;
            BilliardsDataContainer.Instance.CueSnapState.OnDataChanged += CueSnapState_OnDataChanged;

            //SetActive(false);

            PlayerSkinColor = GetSkinColor();

            SetOtherPlayerSkin();

            if (ColorUtility.TryParseHtmlString("#3B7FFF", out var blueColor))
                TransparentBlueColor = blueColor;

            //scale for mainHanded

            //var IsRightHanded = GameSettingCtrl.IsRightHanded();
            //GameSettingCtrl.AddHandChangedEvent(OnMainHandChanged);

            //if (isMyturn)
            //    OnMainHandChanged(IsRightHanded);

            //turn Changed Event listening
            BallPoolPlayer.OnTurnChanged += BallPoolPlayer_OnTurnChanged;
        }

        private void BallPoolPlayer_OnTurnChanged()
        {
            if (BallPool.AightBallPoolGameLogic.playMode == BallPool.PlayMode.PlayerAI && !isMyturn)
            {
                OnMainHandChanged(true);
            }
        }

        //왼손/오른손잡이 관련
        private void OnMainHandChanged(bool isRightHanded)
        {
            if (isRightHanded)
            {
                leftHandRoot.transform.localScale = Vector3.one;
                rightHandRoot.transform.localScale = Vector3.one;
            }
            else
            {
                leftHandRoot.transform.localScale = new Vector3(-1, 1, 1);
                rightHandRoot.transform.localScale = new Vector3(-1, 1, 1);
            }
        }

        private void SetOtherPlayerSkin()
        {
            return;

            var opponentIndex = GameDataManager.instance.userInfos.Select((info) => info.nick).ToList().IndexOf(BallPoolPlayer.players[1].name);

            if (BallPoolGameLogic.playMode == PlayMode.PlayerAI)
                opponentIndex = 1;

            var opponentData = GameDataManager.instance.GetCustomModelData(opponentIndex);

            if (ColorUtility.TryParseHtmlString("#" + opponentData.Hex_Skin_C, out var otherSkinColor))
                OpaqueHandMaterial.SetColor("_BaseColor", otherSkinColor);
        }

        private Color GetSkinColor()
        {
            //TODO: Remove when develop complete
            return Color.white;

            var index = GameDataManager.instance.userInfos.Select((info) => info.nick).ToList().IndexOf(BallPoolPlayer.mainPlayer.name);

            var data = GameDataManager.instance.GetCustomModelData(index);

            if (ColorUtility.TryParseHtmlString("#" + data.Hex_Skin_C, out var skinColor))
                return skinColor;

            else if (ColorUtility.TryParseHtmlString("#F3A39B", out skinColor))
                return skinColor;

            return Color.white;
        }


        private IEnumerator Start()
        {
            yield return new WaitForSeconds(1f);
            OpponentGameState_OnDataChanged(BilliardsDataContainer.Instance.OpponentGameState.Value);
            GameState_OnDataChanged(BilliardsDataContainer.Instance.GameState.Value);
        }

        private void CueSnapState_OnDataChanged(bool isSnap)
        {
            SetColor();
        }

        private void OpponentGameState_OnDataChanged(BallPool.ShotController.GameStateType obj)
        {
            if (!isNetworkControl)
                return;

            OnMainHandChanged(BilliardsDataContainer.Instance.OpponentMainHanded.Value);

            switch (obj)
            {
                case BallPool.ShotController.GameStateType.SetBallPosition:
                case BallPool.ShotController.GameStateType.WaitingForOpponent:
                    UpdateHandActive(false);
                    break;
                case BallPool.ShotController.GameStateType.SelectShotDirection:
                    leftHandRoot.transform.GetChild(0).gameObject.SetActive(false);
                    leftHandRoot.transform.GetChild(1).gameObject.SetActive(true);
                    UpdateHandActive(true);
                    break;
                case BallPool.ShotController.GameStateType.CameraFixAndWaitShot:

                    leftHandRoot.transform.GetChild(0).gameObject.SetActive(true);
                    leftHandRoot.transform.GetChild(1).gameObject.SetActive(false);
                    UpdateHandActive(true);
                    break;

                default:
                    UpdateHandActive(true);
                    break;
            }
        }

        private void GameState_OnDataChanged(BallPool.ShotController.GameStateType obj)
        {
            if (isNetworkControl)
                return;

            //해당타이밍에 바꾸는것은 BallpoolPlayer.OnTurnChanged에서 수정
            if (obj == ShotController.GameStateType.WaitingForOpponent)
                return;

            OnMainHandChanged(ShotController.IsRightHanded);

            switch (obj)
            {

                case BallPool.ShotController.GameStateType.SetBallPosition:
                    UpdateHandActive(false);
                    break;

                case BallPool.ShotController.GameStateType.SelectShotDirection:
                    UpdateHandActive(true);
                    break;

                case BallPool.ShotController.GameStateType.CameraFixAndWaitShot:
                    UpdateHandActive(true);
                    //LeftCueBridgeHand.material.color = PlayerSkinColor;
                    break;

            }
        }

        public void UpdateHandActive(bool cueActive)
        {
            SetMaterial();
            SetColor();

            leftHandRoot.SetActive(cueActive);
            LeftCueBridgeHand.gameObject.SetActive(cueActive);
            RightCueHand.gameObject.SetActive(cueActive);

            if (isMyturn)
            {
                LeftFreeHand.gameObject.SetActive(!cueActive);
                RightFreeHand.gameObject.SetActive(!cueActive);
            }

            //if cue disabled or my turn, set active.
            PlayerManager.OnInitialized += (instance) => instance.ForceSetOtherPlayerHand(!cueActive || isMyturn);
        }

        public void SetColor()
        {
            if (!isMyturn)
                return;

            if (BilliardsDataContainer.Instance.CueSnapState.Value == true)
            {
                LeftCueBridgeHand.sharedMaterial.color = PlayerSkinColor;

                if (BilliardsDataContainer.Instance.GameState.Value == BallPool.ShotController.GameStateType.CameraFixAndWaitShot)
                    RightCueHand.sharedMaterial.color = PlayerSkinColor;
                else
                    RightCueHand.sharedMaterial.color = TransparentBlueColor;

                return;
            }
            else
            {
                LeftCueBridgeHand.sharedMaterial.color = TransparentBlueColor;
                RightCueHand.sharedMaterial.color = TransparentBlueColor;
            }
        }

        public void SetMaterial()
        {
            if (isMyturn)
            {
                LeftCueBridgeHand.sharedMaterial = TransparentHandMaterial_left;
                leftCueGripHand.sharedMaterial = TransparentHandMaterial;
                RightCueHand.sharedMaterial = TransparentHandMaterial;
            }
            else
            {
                LeftCueBridgeHand.sharedMaterial = OpaqueHandMaterial;
                leftCueGripHand.sharedMaterial = OpaqueHandMaterial;
                RightCueHand.sharedMaterial = OpaqueHandMaterial;
            }
        }

        //public void SetColor(Color color)
        //{
        //    LeftCueBridgeHand.sharedMaterial.SetColor("_Tint", color);
        //    RightCueHand.sharedMaterial.SetColor("_Tint", color);
        //}

        private void OnDestroy()
        {
            BallPoolPlayer.OnTurnChanged -= BallPoolPlayer_OnTurnChanged;

            BilliardsDataContainer.Instance.CueSnapState.OnDataChanged -= CueSnapState_OnDataChanged;
            BilliardsDataContainer.Instance.OpponentGameState.OnDataChanged -= OpponentGameState_OnDataChanged;
            BilliardsDataContainer.Instance.GameState.OnDataChanged -= GameState_OnDataChanged;
        }
    }

}