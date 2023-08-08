using UnityEngine;
using System.Collections;
using BallPool;
using UnityEngine.UI;

namespace Billiards
{

    public class UIPause : MonoBehaviour
    {
        [SerializeField]
        private Transform Root;

        [SerializeField]
        private RectTransform LayoutRoot;

        [SerializeField]
        private GameUIController controller;

        private void Awake()
        {
            //BilliardsDataContainer.Instance.XRRightControllerState[InputHelpers.Button.MenuButton].OnDataChanged += ControllerBackButton_OnDataChanged;

            Root.gameObject.SetActive(false);
        }

        private void ControllerBackButton_OnDataChanged(bool isDown)
        {
            if (isDown && !Root.gameObject.activeInHierarchy)
            {
                //Show();
            }
            else if (isDown)
            {
                //Close();
            }
        }

        private void Update()
        {
            if ((Input.GetKeyDown(KeyCode.Mouse1) || Input.GetButtonDown("Right Menu") || Input.GetButtonDown("Left Menu")))
            {
                ControllerBackButton_OnDataChanged(true);
            }
        }

        public void Show()
        {
            if (AightBallPoolGameLogic.playMode != BallPool.PlayMode.OnLine)
            {
                Time.timeScale = 0;
                SoundManager.Pause(true);
            }
            var dir = (BilliardsDataContainer.Instance.MainCamera.Value.transform.position.ToXZ() -
                BilliardsDataContainer.Instance.CueBallCameraRootFollower.Value.TargetPosition.ToXZ()).normalized * 0.2f;

            transform.position = (BilliardsDataContainer.Instance.CueBallCameraRootFollower.Value.TargetPosition.ToXZ() + dir).ToVector3FromXZ(0.15f);

            //.add Animation here
            Root.gameObject.SetActive(true);
            RebuildLayout();
        }

        public void OnClickGoHome()
        {
            //. Action
            controller.ForceGoHome();

            Close();
        }

        public void Close()
        {
            if (Time.timeScale != 1)
            {
                Time.timeScale = 1;
                SoundManager.Pause(true);
            }

            //.add Animation here
            Root.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            //BilliardsDataContainer.Instance.XRRightControllerState[InputHelpers.Button.MenuButton].OnDataChanged -= ControllerBackButton_OnDataChanged;
        }
        private void RebuildLayout()
        {
            try
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(LayoutRoot);
                LayoutRebuilder.ForceRebuildLayoutImmediate(LayoutRoot);
                LayoutRebuilder.ForceRebuildLayoutImmediate(LayoutRoot);
            }
            catch(System.Exception e)
            {
                Debug.Log("Exception in LayoutRebuilder.  " + e.Message + e.StackTrace);
                Debug.Log("LayoutRoot is " + LayoutRoot);
            }
        }

    }
}