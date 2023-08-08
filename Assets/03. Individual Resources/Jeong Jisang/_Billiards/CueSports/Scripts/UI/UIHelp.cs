using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Billiards
{

    public class UIHelp : MonoBehaviour
    {

        [SerializeField]
        private Transform Root;

        [SerializeField]
        private RectTransform LayoutRoot;


        private void Awake()
        {
            if (BallPool.BallPoolGameLogic.playMode != BallPool.PlayMode.OnLine)
            {
                BilliardsDataContainer.Instance.GameState.OnDataChanged += GameState_OnDataChanged;
                BilliardsDataContainer.Instance.XRRightControllerState[UnityEngine.XR.Interaction.Toolkit.InputHelpers.Button.MenuButton].OnDataChanged += ControllerBackButton_OnDataChanged;
            }

            Root.gameObject.SetActive(false);
            //InitializeConfirm();
        }

        private void GameState_OnDataChanged(BallPool.ShotController.GameStateType obj)
        {
            switch (obj)
            {
                case BallPool.ShotController.GameStateType.SelectShotDirection:
                    Show();
                    break;

                case BallPool.ShotController.GameStateType.WaitingForOpponent:
                    Close();
                    break;

                case BallPool.ShotController.GameStateType.CameraFixAndWaitShot:
                    BilliardsDataContainer.Instance.GameState.OnDataChanged -= GameState_OnDataChanged;
                    Close();
                    break;

                default:
                    break;
            }
        }

        private void ControllerBackButton_OnDataChanged(bool isDown)
        {
            //if (isDown && !Root.gameObject.activeInHierarchy)
            //{
            //    Show();
            //}
            //else 
            if (isDown && Root.gameObject.activeInHierarchy)
            {
                Close();
            }
        }

        public void Show()
        {
            //UI_PointerManager.instance.RequestShow(true, this);

            //HandLineManager.Instance.MainHand.RequestShow(true, this);
            //HandLineManager.Instance.SubHand.RequestShow(true, this);

            //var dir = (BilliardsDataContainer.Instance.MainCamera.CurrentData.transform.position.ToXZ() -
            //    BilliardsDataContainer.Instance.CueBallCameraRootFollower.CurrentData.TargetPosition.ToXZ()).normalized * 0.2f;

            //transform.position = (BilliardsDataContainer.Instance.CueBallCameraRootFollower.CurrentData.TargetPosition.ToXZ() + dir).ToVector3FromXZ(0.65f);

            //SounBilliardsDataContainerdManager.PlaySound(SoundManager.AudioClipType.Popup);
            //.add Animation here
            Root.gameObject.SetActive(true);
            //InitializeConfirm();
            RebuildLayout();

            BallPool.BallPoolGameManager.instance.OnShowHelp();
        }

        public void Close()
        {
            //.add Animation here
            Root.gameObject.SetActive(false);

            BallPool.BallPoolGameManager.instance.OnHideHelp();
            //HandLineManager.Instance.MainHand.RequestShow(false, this);
            //HandLineManager.Instance.SubHand.RequestShow(false, this);
            //UI_PointerManager.instance.RequestShow(false, this);
        }

        private void OnDestroy()
        {
            BilliardsDataContainer.Instance.XRRightControllerState[UnityEngine.XR.Interaction.Toolkit.InputHelpers.Button.MenuButton].OnDataChanged -= ControllerBackButton_OnDataChanged;
            BilliardsDataContainer.Instance.GameState.OnDataChanged -= GameState_OnDataChanged;
        }

        private void RebuildLayout()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(LayoutRoot);
            LayoutRebuilder.ForceRebuildLayoutImmediate(LayoutRoot);
            LayoutRebuilder.ForceRebuildLayoutImmediate(LayoutRoot);
        }

    }
}