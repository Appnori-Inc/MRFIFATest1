using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;
using System;

namespace Billiards
{
    public class HandLineManager : Singleton<HandLineManager>
    {
        public HandLineManager() : base()
        {
            SubHand = new Container();
            SubHand.onEnableEvent += SubHandLineActive;
            MainHand = new Container();
            MainHand.onEnableEvent += MainHandLineActive;

            void MainHandLineActive(bool value) => BilliardsDataContainer.Instance.MainHandLineActivation.Value = value;
            void SubHandLineActive(bool value) => BilliardsDataContainer.Instance.SubHandLineActivation.Value = value;
        }

        public Container SubHand { get; private set; }
        public Container MainHand { get; private set; }

        public class Container
        {
            private List<MonoBehaviour> Requesters = new List<MonoBehaviour>();

            public event Action<bool> onEnableEvent;

            public void RequestShow(bool enable, MonoBehaviour requester)
            {

                if (enable)
                {
                    if (!Requesters.Contains(requester))
                        Requesters.Add(requester);
                   
                }
                else
                {
                    if (Requesters.Contains(requester))
                        Requesters.Remove(requester);
                }

                SetEnable(CheckEnable());
            }

            private bool CheckEnable()
            {
                Requesters.RemoveAll((target) => target == null);

                if (Requesters.Count > 0)
                    return true;

                return false;
            }

            private void SetEnable(bool value)
            {
                onEnableEvent?.Invoke(value);
            }
        }
    }

    public class HandLineControl : MonoBehaviour
    {
        [SerializeField]
        private bool isMain;

        [SerializeField]
        private XRInteractorLineVisual mainHandLineVisual;
     

        [SerializeField]
        private Gradient DefaultValidColor;
        [SerializeField]
        private Gradient DefaultInvalidColor;


        [SerializeField]
        private Gradient ActiveValidColor;
        [SerializeField]
        private Gradient ActiveInvalidColor;

        [SerializeField]
        private Animator handani;

        private void SetMainHandEvent()
        {
            BilliardsDataContainer.Instance.MainHandLineActivation.OnDataChanged += HandLineActivation_OnDataChanged;
            HandLineActivation_OnDataChanged(BilliardsDataContainer.Instance.MainHandLineActivation.Value);
        }

        private void SetSubHandEvent()
        {
            BilliardsDataContainer.Instance.SubHandLineActivation.OnDataChanged += HandLineActivation_OnDataChanged;
            HandLineActivation_OnDataChanged(BilliardsDataContainer.Instance.SubHandLineActivation.Value);
        }


        private void Awake()
        {
            //SetHandEvent(GameSettingCtrl.IsRightHanded());
            //GameSettingCtrl.AddHandChangedEvent(SetHandEvent);

            mainHandLineVisual.invalidColorGradient = DefaultInvalidColor;

        }

        //왼손/오른손잡이 관련
        private void SetHandEvent(bool isRightHanded)
        {
            //remove event
            BilliardsDataContainer.Instance.MainHandLineActivation.OnDataChanged -= HandLineActivation_OnDataChanged;
            BilliardsDataContainer.Instance.SubHandLineActivation.OnDataChanged -= HandLineActivation_OnDataChanged;

            if (isMain)
            {
                if (isRightHanded)
                {
                    SetMainHandEvent();
                }
                else
                {
                    SetSubHandEvent();
                }
            }
            else // if not main or main but not righthanded
            {
                if (isRightHanded)
                {
                    SetSubHandEvent();
                }
                else
                {
                    SetMainHandEvent();
                }
            }
        }

        private void HandLineActivation_OnDataChanged(bool isActive)
        {
            if (isActive)
            {
                mainHandLineVisual.validColorGradient = ActiveValidColor;
                mainHandLineVisual.invalidColorGradient = ActiveInvalidColor;
            }
            else
            {
                mainHandLineVisual.validColorGradient = DefaultValidColor;
                mainHandLineVisual.invalidColorGradient = DefaultInvalidColor;
            }
        }

        public void SetLine()
        {
            handani.SetBool("IsPoint",true);
            mainHandLineVisual.invalidColorGradient = ActiveInvalidColor;
            mainHandLineVisual.validColorGradient = ActiveValidColor;
           
           /* GameObject hand = GameObject.Find("PlayerHandTracker");
            hand.SetActive(false);*/
        }

        private void OnDestroy()
        {
            BilliardsDataContainer.Instance.MainHandLineActivation.OnDataChanged -= HandLineActivation_OnDataChanged;
            BilliardsDataContainer.Instance.SubHandLineActivation.OnDataChanged -= HandLineActivation_OnDataChanged;
        }
    }

}