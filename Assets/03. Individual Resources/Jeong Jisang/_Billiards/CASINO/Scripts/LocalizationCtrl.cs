using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BallPool
{
    public class LocalizationCtrl : MonoBehaviour
    {
        public GameObject go_Neon_chinese;
        public GameObject go_Neon_english;

        public Material mat_Poster;

        private void Start()
        {
            //SetLocalization();
            GameSettingCtrl.AddLocalizationChangedEvent(SetLocalization);
        }

        private void SetLocalization(LanguageState lang)
        {
            //var lang = GameSettingCtrl.GetLanguageState();
            switch (lang)
            {
                case LanguageState.schinese:
                case LanguageState.tchinese:
                    {
                        go_Neon_chinese.SetActive(true);
                        go_Neon_english.SetActive(false);

                        mat_Poster.SetTexture("_BaseMap", Resources.Load<Texture2D>("Billiard/Texture/poster_chinese"));
                    }
                    break;
                default:
                    {
                        go_Neon_chinese.SetActive(false);
                        go_Neon_english.SetActive(true);

                        mat_Poster.SetTexture("_BaseMap", Resources.Load<Texture2D>("Billiard/Texture/poster_english"));
                    }
                    break;
            }
        }
    }
}