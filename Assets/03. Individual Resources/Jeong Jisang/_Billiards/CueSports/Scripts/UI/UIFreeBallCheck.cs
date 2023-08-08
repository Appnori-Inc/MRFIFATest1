using UnityEngine;
using System.Collections;

namespace Billiards
{
    using UnityEngine.UI;

    public class UIFreeBallCheck : MonoBehaviour
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
        private MeshRenderer CheckerRenderer;

        public void OnEnable()
        {
            Root.rotation = Quaternion.identity;

            //TODO : Localization
            DescriptionText.text = GameSettingCtrl.GetLocalizationText("0087") /*"Set"*/;
        }

        public void Update()
        {
            //localPosition in 0.001 scaled space
            Cursor.Rotate(Vector3.up, 3f, Space.Self);

            DescriptionTransform.localPosition = new Vector3(0, Mathf.PingPong(Time.time * 100f, 200) - 100f, 0);
        }

        public void SetInvalid()
        {
            DescriptionText.text = GameSettingCtrl.GetLocalizationText("0055") /*"Invalid"*/;

            var color = CheckerRenderer.material.color;
            color = Color.red * new Color(1, 1, 1, color.a);
            CheckerRenderer.material.color = color;

        }

        public void SetValid()
        {
            DescriptionText.text = GameSettingCtrl.GetLocalizationText("0087")/*"Set"*/;

            var color = CheckerRenderer.material.color;
            color = Color.white * new Color(1, 1, 1, color.a);
            CheckerRenderer.material.color = color;
        }
    }
}