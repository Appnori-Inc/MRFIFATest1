using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;


namespace Appnori.Util
{

    public class AudioMixerControl : MonoBehaviour
    {
        public static AudioMixerControl Instance { get ; private set; }

        private static event Action<AudioMixerControl> onInitialized;
        public static event Action<AudioMixerControl> OnInitialized
        {
            add
            {
                if (Instance != null)
                {
                    value?.Invoke(Instance);
                    return;
                }

                onInitialized += value;
            }
            remove
            {
                onInitialized -= value;
            }
        }


        [Serializable]
        public class Pair
        {
            public string originFieldName;
            public AudioMixerGroup origin;
            public string destFieldName;
            public AudioMixerGroup destination;
        }

        [SerializeField]
        private List<Pair> list;
        [SerializeField]
        private List<AudioMixerGroup> DestinationGroups;

        public Notifier<float> Voulume { get; private set; } = new Notifier<float>();

        public AudioMixerGroup GetGroup(Predicate<AudioMixerGroup> pred)
        {
            return DestinationGroups.Find(pred);
        }

        private void Awake()
        {
            Instance = this;

            onInitialized?.Invoke(this);
            onInitialized = null;
        }

        private void Update()
        {
            foreach (var pair in list)
            {
                if (pair.origin.audioMixer.GetFloat(pair.originFieldName, out var value))
                {
                    pair.destination.audioMixer.SetFloat(pair.destFieldName, value);
                    Voulume.Value = value;
                }
            }
        }

        public static float DecibelToLinear(float dB)
        {
            float linear = Mathf.Pow(10.0f, dB / 20.0f);

            return linear;
        }

    }

}