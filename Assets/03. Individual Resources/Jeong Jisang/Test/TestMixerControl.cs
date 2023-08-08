using Billiards;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


namespace Appnori.Util
{

    public class TestMixerControl : MonoSingleton<TestMixerControl>
    {
        [SerializeField]
        private List<AudioMixerGroup> targetGroup;

        protected override void Awake()
        {
            base.Awake();
            if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                foreach (var g in targetGroup)
                {
                    if (g.audioMixer.GetFloat("BGMVolume", out var bgm))
                        g.audioMixer.SetFloat("BGMVolume", bgm + 1);

                    if (g.audioMixer.GetFloat("FXVolume", out var fx))
                        g.audioMixer.SetFloat("FXVolume", fx + 1);
                }
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                foreach (var g in targetGroup)
                {
                    if (g.audioMixer.GetFloat("BGMVolume", out var bgm))
                        g.audioMixer.SetFloat("BGMVolume", bgm - 1);

                    if (g.audioMixer.GetFloat("FXVolume", out var fx))
                        g.audioMixer.SetFloat("FXVolume", fx - 1);
                }
            }
        }
    }

}