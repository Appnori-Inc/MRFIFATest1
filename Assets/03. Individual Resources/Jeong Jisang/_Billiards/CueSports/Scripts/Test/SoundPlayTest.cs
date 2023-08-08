using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Appnori.Util;

public class SoundPlayTest : MonoBehaviour
{
    [SerializeField]
    private AudioSource Foul;

    [SerializeField]
    private AudioSource FreeBall;

    private Notifier<bool> isFoulPlaying = new Notifier<bool>();
    private CoroutineWrapper wrapper;

    private void Awake()
    {
        wrapper = CoroutineWrapper.Generate(this);
    }

    private void OnEnable()
    {
        Foul.Play();
        isFoulPlaying.Value = true;
        StartCoroutine(WaitWhilePlay(Foul, () => isFoulPlaying.Value = false));

        if(isFoulPlaying.Value == true)
        {
            isFoulPlaying.OnDataChangedOnce += IsFoulPlaying_OnDataChangedOnce;
        }
        else
        {
            FreeBall.Play();
        }
    }

    private void IsFoulPlaying_OnDataChangedOnce(bool obj)
    {
        if (obj)
            return;

        FreeBall.Play();
    }

    private IEnumerator WaitWhilePlay(AudioSource target, System.Action onComplete)
    {
        yield return new WaitWhile(() => target.isPlaying);
        onComplete?.Invoke();
    }

}
