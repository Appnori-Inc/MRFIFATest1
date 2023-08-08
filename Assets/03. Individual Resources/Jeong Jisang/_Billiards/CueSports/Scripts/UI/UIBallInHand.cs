using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace Billiards
{
    using UnityEngine.Events;

    public class UIBallInHand : InteractableButton
    {
        [SerializeField]
        private Transform Root;

        [SerializeField]
        private Transform DescriptionTransform;
        [SerializeField]
        private Text DescriptionText;

        [SerializeField]
        private Transform Cursor;

        [SerializeField]
        private AudioSource source;

        protected override void Awake()
        {
            base.Awake();
            base.onClick.AddListener(new UnityAction(OnClick));
        }
        [Obsolete]
        private void OnClick()
        {
            //.submit
            //BilliardsDataContainer.Instance.isFreeBallSubmited.CurrentData = true;
        }

        public void OnEnable()
        {
            Root.rotation = Quaternion.identity;

            //TODO : Localization
            //DescriptionText.text = "Free ball";

            StartCoroutine(WaitAndPlaySound());

            //BilliardsDataContainer.Instance.GameState.OnDataChanged += GameState_OnDataChanged;
        }

        IEnumerator WaitAndPlaySound()
        {
            yield return new WaitForSeconds(0.1f);

            if (BilliardsDataContainer.Instance.isFoulVoicePlaying.Value == true)
            {
                BilliardsDataContainer.Instance.isFoulVoicePlaying.OnDataChangedOnce += IsFoulVoicePlaying_OnDataChangedOnce;
            }
            else
            {
                source.Play();
            }
        }

        private void IsFoulVoicePlaying_OnDataChangedOnce(bool isPlaying)
        {
            if (isPlaying)
                return;

            source.Play();
        }

        [Obsolete]
        private void GameState_OnDataChanged(BallPool.ShotController.GameStateType obj)
        {

        }

        protected override void OnDisable()
        {
            base.OnDisable();

            //BilliardsDataContainer.Instance.GameState.OnDataChanged -= GameState_OnDataChanged;
        }

        public void Update()
        {
            //localPosition in 0.001 scaled space
            Cursor.Rotate(Vector3.up, 3f, Space.Self);

            DescriptionTransform.localPosition = new Vector3(0, Mathf.PingPong(Time.time * 100f, 200) - 100f, 0);
        }
    }

}