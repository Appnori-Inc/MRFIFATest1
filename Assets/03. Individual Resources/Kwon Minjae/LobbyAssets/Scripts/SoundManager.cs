using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.XR;

public enum VoiceState
{
    Title
}

namespace MJ
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager instance;

        public AudioClip audio_title;
        public AudioClip[] audios_bgm;
        public AudioClip[] audios_ui;
        public AudioClip[] audios_effect;
        /* public AudioClip[] audios_voice_go_stop;
         public AudioClip[] audios_voice_go_stop_sub;
         public AudioClip[] audios_voice_achieve;
         public AudioClip[] audios_voice_other;*/
        public AudioClip[] audios_voice_other;
        public AudioClip audio_time;

        private AudioSource audioSource_bgm;

        private AudioSource[] audioSources_effect = new AudioSource[3];
        private AudioSource[] audioSources_voice = new AudioSource[3];

        private AudioSource audioSource_time;

        private int audioSourceIndex_effect = 0;
        private int audioSourceIndex_voice = 0;

        private int time_voice_index = 0;

        private AudioMixer audioMixer;

        public Slider[] sliders_audio;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(this);
            }
        }


        // Start is called before the first frame update
        void Start()
        {
            audioMixer = Resources.Load("MasterAudioMixer") as AudioMixer;
            audioSource_bgm = GetComponent<AudioSource>();

            sliders_audio[0].value = PlayerPrefs.GetFloat("BGM", 1f);

            sliders_audio[1].value = PlayerPrefs.GetFloat("Effect", 1f);

            sliders_audio[2].value = PlayerPrefs.GetFloat("Voice", 1f);

            for (int i = 0; i < 3; i++)
            {
                audioSources_effect[i] = gameObject.AddComponent<AudioSource>();
                audioSources_effect[i].playOnAwake = false;
                audioSources_effect[i].loop = false;
                audioSources_effect[i].outputAudioMixerGroup = audioMixer.FindMatchingGroups("Effect")[0];
            }

            for (int i = 0; i < 3; i++)
            {
                audioSources_voice[i] = gameObject.AddComponent<AudioSource>();
                audioSources_voice[i].playOnAwake = false;
                audioSources_voice[i].loop = false;
                audioSources_voice[i].outputAudioMixerGroup = audioMixer.FindMatchingGroups("Voice")[0];
            }

            audioSource_time = gameObject.AddComponent<AudioSource>();
            audioSource_time.playOnAwake = false;
            audioSource_time.loop = true;
            audioSource_time.clip = audio_time;
            audioSource_time.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Effect")[0];
            //Invoke("PlayTitle", 1f);
            //Invoke("DelayPlayBGM", 2f);
        }
        public void PlayTimeSound()
        {
            audioSource_time.Play();
        }

        public void StopTimeSound()
        {
            audioSource_time.Stop();
        }

        public void PlayUISound(int index)
        {
            audioSourceIndex_effect++;
            if (audioSourceIndex_effect >= 3)
            {
                audioSourceIndex_effect = 0;
            }
            audioSources_effect[audioSourceIndex_effect].clip = audios_ui[index];
            audioSources_effect[audioSourceIndex_effect].Play();
        }

        public void PlayEffectSound(int index)
        {
            audioSourceIndex_effect++;
            if (audioSourceIndex_effect >= 3)
            {
                audioSourceIndex_effect = 0;
            }

            audioSources_effect[audioSourceIndex_effect].clip = audios_effect[index];
            audioSources_effect[audioSourceIndex_effect].Play();
        }

        public void PlayVoiceSound(VoiceState voiceState)
        {
            audioSourceIndex_voice++;
            if (audioSourceIndex_voice >= 3)
            {
                audioSourceIndex_voice = 0;
            }

            switch (voiceState)
            {
                case VoiceState.Title:
                    {
                        audioSources_voice[audioSourceIndex_voice].clip = audio_title;
                    }
                    break;
               /* case VoiceState.Go1:
                    {
                        int ranNum = (int)Random.Range(0f, 2.9999f);
                        //audioSources_voice[audioSourceIndex_voice].clip = audios_voice_go_stop[ranNum];
                        StartCoroutine(PlaySubGoStopVoiceCoroutine(audioSourceIndex_voice, 0));
                    }
                    break;*/
                
            }
            audioSources_voice[audioSourceIndex_voice].Play();
        }

        IEnumerator PlaySubGoStopVoiceCoroutine(int _audioSourceIndex, int subClipIndex)
        {
            yield return null;
            while (audioSources_voice[audioSourceIndex_voice].isPlaying)
            {
                yield return null;
            }
            yield return null;

            audioSourceIndex_voice++;
            if (audioSourceIndex_voice >= 3)
            {
                audioSourceIndex_voice = 0;
            }

            //audioSources_voice[audioSourceIndex_voice].clip = audios_voice_go_stop_sub[subClipIndex];
            audioSources_voice[audioSourceIndex_voice].Play();
        }

        Coroutine bgmCoroutine;

        public void PlayTitle()
        {
            PlayVoiceSound(VoiceState.Title);
        }

        public void DelayPlayBGM()
        {
            if (bgmCoroutine != null)
            {
                StopCoroutine(bgmCoroutine);
            }
            bgmCoroutine = StartCoroutine(BGMCoroutine(0));
        }

        public void PlayBGM(int index)
        {
            if (bgmCoroutine != null)
            {
                StopCoroutine(bgmCoroutine);
            }
            bgmCoroutine = StartCoroutine(BGMCoroutine(index));
        }

        IEnumerator BGMCoroutine(int index)
        {
            while (true)
            {
                audioSource_bgm.volume = Mathf.Clamp01(audioSource_bgm.volume - Time.deltaTime * 5f);

                if (audioSource_bgm.volume == 0f)
                {
                    break;
                }

                yield return null;
            }

            if (index == -1)
            {
                yield break;
            }

            audioSource_bgm.clip = audios_bgm[index];
            audioSource_bgm.Play();

            while (true)
            {
                audioSource_bgm.volume = Mathf.Clamp(audioSource_bgm.volume + Time.deltaTime * 5f, 0f, 0.5f);

                if (audioSource_bgm.volume >= 0.5f)
                {
                    break;
                }

                yield return null;
            }
        }

        public void SetBGMVolume(float value)
        {
            if (value == 0f)
            {
                audioMixer.SetFloat("BGM", -80f);
            }
            else
            {
                audioMixer.SetFloat("BGM", Mathf.Log(value) * 20);
            }
        }

        public void SetEffectVolume(float value)
        {
            if (value == 0f)
            {
                audioMixer.SetFloat("Effect", -80f);
            }
            else
            {
                audioMixer.SetFloat("Effect", Mathf.Log(value) * 20);
            }
        }

        public void SetVoiceVolume(float value)
        {
            if (value == 0f)
            {
                audioMixer.SetFloat("Voice", -80f);
            }
            else
            {
                audioMixer.SetFloat("Voice", Mathf.Log(value) * 20);
            }
        }

        public void SaveSetVolume()
        {
            Debug.Log("Save_Setting");

            PlayerPrefs.SetFloat("BGM", sliders_audio[0].value);
            PlayerPrefs.SetFloat("Effect", sliders_audio[1].value);
            PlayerPrefs.SetFloat("Voice", sliders_audio[2].value);

        }

        public void ConfirmEffectVolume()
        {
            audioSources_effect[0].clip = audios_effect[6];
            audioSources_effect[0].Play();
        }

        public void ConfirmVoiceVolume()
        {
            int ranNum = (int)Random.Range(0f, 5.9999f);
            switch (ranNum)
            {
                case 0:
                    {
                        //audioSources_voice[0].clip = audios_voice_go_stop[0];
                    }
                    break;
                case 1:
                    {
                        //audioSources_voice[0].clip = audios_voice_go_stop[1];
                    }
                    break;
                case 2:
                    {
                        //audioSources_voice[0].clip = audios_voice_go_stop[2];
                    }
                    break;
                case 3:
                    {
                        //audioSources_voice[0].clip = audios_voice_go_stop[27];
                    }
                    break;
                case 4:
                    {
                        //audioSources_voice[0].clip = audios_voice_go_stop[28];
                    }
                    break;
                case 5:
                    {
                        //audioSources_voice[0].clip = audios_voice_go_stop[29];
                    }
                    break;
            }
            audioSources_voice[0].Play();
        }
    }

}