using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using NetworkManagement;

#if UNITY_EDITOR
using UnityEditor.Events;
using UnityEditor;
#endif

namespace Billiards
{

    public class SoundManager : MonoSingleton<SoundManager>
    {
        
        public enum AudioClipType
        {
            None,
            Button,
            Win,
            Lose,
            Popup,
            TimeLimit,

            LobbyBGM,
            GameBGM,

            Intro,
        }

        [Serializable]
        private class AudioDatum
        {
            public AudioClipType type;
            public AudioClip clip;

            [Range(0, 1)]
            public float volume = 0.5f;
        }

        [SerializeField]
        private AudioDatum LobbyDatum;
        private AudioSource LobbySource;

        [SerializeField]
        private AudioDatum IngameDatum;
        private AudioSource IngameSource;

        [Space(15)]
        [SerializeField]
        private AudioDatum[] AudioData;

        [Space(15)]
        [SerializeField]
        private AnimationCurve FadeInCurve;
        [SerializeField]
        private AnimationCurve FadeOutCurve;

        [SerializeField]
        private List<AudioClip> CrowdCheerSounds;

        private Dictionary<AudioClipType, AudioDatum> AudioDictionary = new Dictionary<AudioClipType, AudioDatum>();

        private List<AudioSource> AudioSources = new List<AudioSource>();

        private List<AudioSource> CachedAudioSource = new List<AudioSource>();

        private float defaultBgmVolume;
        public bool isInitialized { get; private set; }

        private float CachedFXVolume = float.MinValue;
        private float SavedFXVolume
        {
            get => DataManager.GetFloat("FXVolume", 1f);
            set => DataManager.SetFloat("FXVolume", value);
        }

        private float CachedBGMVolume = float.MinValue;
        private float SavedBGMVolume
        {
            get => DataManager.GetFloat("BGMVolume", 1f);
            set => DataManager.SetFloat("BGMVolume", value);
        }

        public float FXVolume
        {
            get
            {
                if(CachedFXVolume == float.MinValue)
                {
                    CachedFXVolume = SavedFXVolume;
                }

                return CachedFXVolume;
            }
            set
            {
                if(SavedFXVolume != value)
                {
                    SavedFXVolume = value;
                    CachedFXVolume = value;

                    UpdateVolume();
                }
            }
        }


        public float BGMVolume
        {
            get
            {
                if (CachedBGMVolume == float.MinValue)
                {
                    CachedBGMVolume = SavedBGMVolume;
                }

                return CachedBGMVolume;
            }
            set
            {
                if (SavedBGMVolume != value)
                {
                    SavedBGMVolume = value;
                    CachedBGMVolume = value;

                    UpdateVolume();
                }
            }
        }

        private int SceneChangedCount;

        protected override void Awake()
        {
            if ((object)_instance != null)
            {
                DestroyImmediate(gameObject);
                return;
            }

            Initialze();
            base.Awake();
        }

        private void Initialze()
        {
            for (int i = 0; i < 10; ++i)
            {
                var component = gameObject.AddComponent<AudioSource>();

                InitializeAudioSource(component);
                AudioSources.Add(component);
                component.outputAudioMixerGroup = GameSettingCtrl.GetAudioMixerGroup("Effect"); 

                //Appnori.Util.AudioMixerControl.OnInitialized += (instance) =>
                //{
                //    var targetGroup = instance.GetGroup((group) => group.name == GameConfig.FxSoundGroupName);
                //    component.outputAudioMixerGroup = targetGroup;
                //};
            }

            LobbySource = gameObject.AddComponent<AudioSource>();
            LobbySource.clip = LobbyDatum.clip;
            LobbySource.volume = LobbyDatum.volume * BGMVolume;
            LobbySource.loop = true;
            LobbySource.playOnAwake = false;
            LobbySource.Stop();

            IngameSource = gameObject.AddComponent<AudioSource>();
            IngameSource.clip = IngameDatum.clip;
            IngameSource.volume = IngameDatum.volume * BGMVolume;
            IngameSource.loop = true;
            IngameSource.playOnAwake = false;
            IngameSource.Stop();

            for (int i = 0; i < AudioData.Length; ++i)
            {
                AudioDictionary[AudioData[i].type] = AudioData[i];
            }

            isInitialized = true;

            defaultBgmVolume = LobbyDatum.volume;
            SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        }

        private void SceneManager_activeSceneChanged(Scene prev, Scene next)
        {
            if (SceneChangedCount++ == 0)
                return;

            StopAllCoroutines();
            if (next.buildIndex == 2)
            {
                //game
                StartCoroutine(FadeBGM(LobbySource, IngameSource, 2f));
            }
            else
            {
                StartCoroutine(FadeBGM(IngameSource, LobbySource, 2f));
            }
        }

        private IEnumerator FadeBGM(AudioSource prev, AudioSource next, float runtime)
        {
            float t = 0;

            next.volume = 0;

            next.Play();
            while (t < runtime)
            {
                t += Time.deltaTime;

                prev.volume = defaultBgmVolume * FadeOutCurve.Evaluate(t / runtime) * BGMVolume;
                next.volume = defaultBgmVolume * FadeInCurve.Evaluate(t / runtime) * BGMVolume;

                yield return null;
            }
            prev.Stop();

            prev.volume = defaultBgmVolume * FadeOutCurve.Evaluate(1) * BGMVolume;
            next.volume = defaultBgmVolume * FadeInCurve.Evaluate(1) * BGMVolume;

        }

        private void InitializeAudioSource(AudioSource source)
        {
            source.playOnAwake = false;
        }

        private void UpdateVolume()
        {
            LobbySource.volume = LobbyDatum.volume * BGMVolume;
            IngameSource.volume = IngameDatum.volume * BGMVolume;

            for (int i = 0; i < AudioSources.Count; ++i)
            {
                if (AudioSources[i].isPlaying)
                {
                    var current = AudioDictionary.Values.First((datum) => datum.clip == AudioSources[i].clip);
                    AudioSources[i].volume = current.volume * FXVolume;
                }
            }
        }

        private void PlayRandomCheerSound()
        {
            for (int i = 0; i < AudioSources.Count; ++i)
            {
                if (!AudioSources[i].isPlaying)
                {
                    AudioSources[i].clip = CrowdCheerSounds.GetRandom();
                    AudioSources[i].volume = 0.5f * FXVolume;
                    AudioSources[i].loop = false;
                    AudioSources[i].Play();
                    break;
                }
            }
        }

        private void PlaySoundInternal(AudioClipType type)
        {
            if (type == AudioClipType.LobbyBGM)
            {
                LobbySource.Play();
                return;
            }

            if (type == AudioClipType.GameBGM)
            {
                IngameSource.Play();
                return;
            }

            for (int i = 0; i < AudioSources.Count; ++i)
            {
                if (!AudioSources[i].isPlaying)
                {
                    AudioSources[i].clip = AudioDictionary[type].clip;
                    AudioSources[i].volume = AudioDictionary[type].volume * FXVolume;
                    AudioSources[i].loop = false;
                    AudioSources[i].Play();
                    break;
                }
            }
        }

        private void StopSoundInternal(AudioClipType type)
        {
            if (type == AudioClipType.LobbyBGM)
            {
                LobbySource.Stop();
                return;
            }

            if (type == AudioClipType.GameBGM)
            {
                IngameSource.Stop();
                return;
            }

            for (int i = 0; i < AudioSources.Count; ++i)
            {
                if (AudioSources[i].isPlaying && AudioSources[i].clip == AudioDictionary[type].clip)
                {
                    AudioSources[i].Stop();
                }
            }
        }

        private void PlayingSoundModifyInternal(AudioClipType type, Action<AudioSource> target)
        {
            if (type == AudioClipType.LobbyBGM)
            {
                target?.Invoke(LobbySource);
                return;
            }

            if (type == AudioClipType.GameBGM)
            {
                target?.Invoke(IngameSource);
                return;
            }

            for (int i = 0; i < AudioSources.Count; ++i)
            {
                if (AudioSources[i].isPlaying && AudioSources[i].clip == AudioDictionary[type].clip)
                {
                    target?.Invoke(AudioSources[i]);
                }
            }
        }

        private bool isPlayingInternal(AudioClipType type)
        {
            if (type == AudioClipType.LobbyBGM)
            {
                return LobbySource.isPlaying;
            }

            if (type == AudioClipType.GameBGM)
            {
                return IngameSource.isPlaying;
            }

            for (int i = 0; i < AudioSources.Count; ++i)
            {
                if (AudioSources[i].isPlaying && AudioSources[i].clip == AudioDictionary[type].clip)
                {
                    return AudioSources[i].isPlaying;
                }
            }

            return false;
        }

        private void PauseSoundInternal(bool isPause)
        {
            if (isPause)
            {
                for (int i = 0; i < AudioSources.Count; ++i)
                {
                    if (AudioSources[i].isPlaying)
                    {
                        CachedAudioSource.Add(AudioSources[i]);
                        AudioSources[i].Pause();
                    }
                }
            }
            else
            {
                var e = CachedAudioSource.GetEnumerator();
                while (e.MoveNext())
                {
                    e.Current.Play();
                }
                CachedAudioSource.Clear();
            }
        }


        public static bool isPlaying(AudioClipType type)
        {
            if (!Instance.isInitialized)
                return false;

            return Instance.isPlayingInternal(type);
        }

        public static void PlaySound(AudioClipType type)
        {
            if (!Instance.isInitialized)
                return;

            Instance.PlaySoundInternal(type);
        }

        public static void PlayCheerSound()
        {

            if (!Instance.isInitialized)
                return;

            Instance.PlayRandomCheerSound();
        }

        public static void StopSound(AudioClipType type)
        {
            if (!Instance.isInitialized)
                return;

            Instance.StopSoundInternal(type);
        }

        public static void PlayingSoundModify(AudioClipType type, Action<AudioSource> target)
        {
            Instance.PlayingSoundModifyInternal(type, target);
        }

        public static float GetDefaultVolume(AudioClipType type)
        {
            return Instance.AudioDictionary[type].volume;
        }

        public static void Pause(bool isPause)
        {
            if (!Instance.isInitialized)
                return;

            Instance.PauseSoundInternal(isPause);
        }

        public override void OnDestroy()
        {
            if (_instance == this)
            {
                base.OnDestroy();
            }
        }


#if UNITY_EDITOR
        [MenuItem("Sound/UI Button Initialize")]
        public static void initSound()
        {
            var buttons = Resources.FindObjectsOfTypeAll<Button>().ToList();
            foreach (var button in buttons)
            {
                var invoker = button.GetComponent<SoundInvoker>();
                if (invoker == null)
                {
                    invoker = button.gameObject.AddComponent<SoundInvoker>();
                }

                var count = button.onClick.GetPersistentEventCount();
                for (int i = 0; i < count; ++i)
                {
                    var targetMethodName = button.onClick.GetPersistentMethodName(i);
                    var target = button.onClick.GetPersistentTarget(i);

                    if (targetMethodName == "OnClickButton" && target.GetType() == invoker.GetType())
                    {
                        UnityEventTools.RemovePersistentListener(button.onClick, invoker.OnClickButton);
                        break;
                    }
                }

                UnityEventTools.AddVoidPersistentListener(button.onClick, invoker.OnClickButton);
            }
        }

        [MenuItem("Sound/Button Reactor Initialize")]
        public static void initReactorSound()
        {
            var buttons = Resources.FindObjectsOfTypeAll<InteractableButton>().ToList();
            foreach (var button in buttons)
            {
                var invoker = button.GetComponent<SoundInvoker>();
                if (invoker == null)
                {
                    invoker = button.gameObject.AddComponent<SoundInvoker>();
                }

                var count = button.OnClick.GetPersistentEventCount();
                for (int i = 0; i < count; ++i)
                {
                    var targetMethodName = button.OnClick.GetPersistentMethodName(i);
                    var target = button.OnClick.GetPersistentTarget(i);

                    if (targetMethodName == "OnClickButton" && target.GetType() == invoker.GetType())
                    {
                        UnityEventTools.RemovePersistentListener(button.OnClick, invoker.OnClickButton);
                        break;
                    }
                }

                UnityEventTools.AddVoidPersistentListener(button.OnClick, invoker.OnClickButton);
            }
        }


#endif
    }

}