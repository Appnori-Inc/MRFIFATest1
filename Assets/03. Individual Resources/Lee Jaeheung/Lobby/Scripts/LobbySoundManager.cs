using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SingletonBase;
using System.Xml;

public class LobbySoundManager : Singleton<LobbySoundManager>
{
    public AudioClip audio_bgm;

    public AudioClip[] audios_voice_op;
    public AudioClip[] audios_effect;
    public AudioClip[] audios_voice;

    private AudioSource audioSource_bgm;
    private AudioSource audioSource_effect;
    private AudioSource audioSource_voice;
    void Start()
    {
        audioSource_bgm = gameObject.AddComponent<AudioSource>();
        audioSource_bgm.loop = false;
        audioSource_bgm.volume = 0.7f;
        audioSource_bgm.playOnAwake = false;
        audioSource_bgm.outputAudioMixerGroup = GameSettingCtrl.GetAudioMixerGroup("BGM");

        audioSource_effect = gameObject.AddComponent<AudioSource>();
        audioSource_effect.loop = false;
        audioSource_effect.volume = 1f;
        audioSource_effect.playOnAwake = false;
        audioSource_effect.outputAudioMixerGroup = GameSettingCtrl.GetAudioMixerGroup("Effect");

        audioSource_voice = gameObject.AddComponent<AudioSource>();
        audioSource_voice.loop = false;
        audioSource_voice.volume = 1f;
        audioSource_voice.playOnAwake = false;
        audioSource_voice.outputAudioMixerGroup = GameSettingCtrl.GetAudioMixerGroup("Effect");

        StartCoroutine(StartPlaySound());
    }
    
    IEnumerator StartPlaySound()
    {
        yield return new WaitForSeconds(1f);
        if (audios_voice_op.Length > 0)
        {
            audioSource_bgm.clip = audios_voice_op[(int)Random.Range(0f,2.9999f)];
            audioSource_bgm.Play();
            yield return new WaitForSeconds(1f);
            while (audioSource_bgm.isPlaying)
            {
                yield return null;
            }
        }
        audioSource_bgm.loop = true;
        audioSource_bgm.clip = audio_bgm;
        audioSource_bgm.Play();
    }

    public void Play(int index, bool isLoop = false)
    {
        audioSource_effect.loop = isLoop;
        audioSource_effect.clip = audios_effect[index];
        audioSource_effect.Play();
    }
    public void Stop()
    {
        audioSource_effect.Stop();
    }

    public void Play_Voice(int index, bool isLoop = false)
    {
        audioSource_voice.loop = isLoop;
        audioSource_voice.clip = audios_voice[index];
        audioSource_voice.Play();
    }
    public void Stop_Voice()
    {
        audioSource_voice.Stop();
    }
}
