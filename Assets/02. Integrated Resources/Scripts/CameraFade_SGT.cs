using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SingletonBase;
using System;

public class CameraFade_SGT : Singleton<CameraFade_SGT>
{
    private Material fadeMaterial;
    // Start is called before the first frame update


    private bool isFadeIn;
    private bool isFadeOut;

    private Coroutine fadeIn_C;
    private Coroutine fadeOut_C;

    protected override void Awake()
    {
        base.Awake();
        fadeMaterial = GetComponent<MeshRenderer>().material;

        isFadeIn = false;
        isFadeOut = false;
    }

    //밝아짐(파라미터 = 대리자)
    public void StartFadeIn(Action DLG = null)
    {
        if (fadeIn_C != null)
            StopCoroutine(fadeIn_C);

        if (fadeOut_C != null)
            StopCoroutine(fadeOut_C);

        fadeIn_C = StartCoroutine(StartFadeIn_C(0.01f));

        if (DLG != null)
            DLG();
    }

 
    //어두워짐(파라미터 = 대리자)
    public void StartFadeOut(Action DLG = null)
    {
        if (fadeIn_C != null)
            StopCoroutine(fadeIn_C);

        if (fadeOut_C != null)
            StopCoroutine(fadeOut_C);

        fadeOut_C = StartCoroutine(StartFadeOut_C(0.01f));

        if (DLG != null)
            DLG();
    }


    IEnumerator StartFadeIn_C(float fadeSpeed)
    {
        isFadeIn = true;

        Color color = new Color();
        color = Color.black;
        color.a = 1;

        fadeMaterial.SetColor("_BaseColor", color);
        while (isFadeIn)
        {
            color.a -= fadeSpeed;
            fadeMaterial.SetColor("_BaseColor", color);

            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);

            if(color.a <= 0)
            {
                isFadeIn = false;

                color.a = 0;
                fadeMaterial.SetColor("_BaseColor", color);
                break;
            }
        }
    }


    IEnumerator StartFadeOut_C(float fadeSpeed)
    {
        isFadeOut = true;

        Color color = new Color();
        color = Color.black;
        color.a = 0;

        fadeMaterial.SetColor("_BaseColor", color);
        while (isFadeOut)
        {
            color.a += fadeSpeed;
            fadeMaterial.SetColor("_BaseColor", color);

            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);

            if (color.a >= 1)
            {
                isFadeOut = false;

                color.a = 1;
                fadeMaterial.SetColor("_BaseColor", color);
                break;
            }
        }
    }

}
