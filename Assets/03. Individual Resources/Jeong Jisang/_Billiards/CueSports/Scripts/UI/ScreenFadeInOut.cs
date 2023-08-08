using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Billiards
{

    public class ScreenFadeInOut : MonoBehaviour
    {
        [SerializeField]
        private Image FadeImage;

        private void Awake()
        {
            BilliardsDataContainer.Instance.NormalizedFadeTime.OnDataChanged += NormalizedFadeValue_OnDataChanged;
        }

        private void NormalizedFadeValue_OnDataChanged(float obj)
        {
            var color = FadeImage.color;
            color.a = obj;
            FadeImage.color = color;
        }

        private void OnDestroy()
        {
            BilliardsDataContainer.Instance.NormalizedFadeTime.OnDataChanged -= NormalizedFadeValue_OnDataChanged;
        }
    }
}