using UnityEngine;
using System.Collections;


namespace Billiards
{
    public class SoundInvoker : MonoBehaviour
    {
        private AudioSource source;
        [SerializeField] private AudioClip freeballClip_eng;
        [SerializeField] private AudioClip freeballClip_chn;

        private void Start()
        {
            source = GetComponent<AudioSource>();

            //SetLocalization();

            GameSettingCtrl.AddLocalizationChangedEvent(SetLocalization);
        }

        private void SetLocalization(LanguageState state)
        {
            var isLanguageChn = state == LanguageState.tchinese ||
                state == LanguageState.schinese;

            if (isLanguageChn)
                source.clip = freeballClip_chn;
            else
                source.clip = freeballClip_eng;
        }

        public void OnClickButton()
        {
            SoundManager.PlaySound(SoundManager.AudioClipType.Button);
        }
    }
}