using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceMatCtrl : MonoBehaviour
{
    public float speed_surface = 0.03f;
    public float speed_pos = 30f;
    public float speed_wave = 0.13f;
    public float speed_stagnant = 0.13f;
    public float speed_waterfall = 0.13f;

    private float timeP;
    private float posP;
    private float waveP;
    private float stagnantP;
    private bool isTurnPos;
    private float waterfallP;

    public Material[] mat_surface;
    public Material mat_wave;
    public Material mat_stagnant;
    public Material mat_waterfall;

    public AnimationCurve animCurve_waveA;
    public AnimationCurve animCurve_waveP;
    public AnimationCurve animCurve_waveS;

    private void Update()
    {
        timeP += Time.deltaTime * speed_surface;

        if (timeP >= 1f)
        {
            timeP -= 1f;
        }

        if (isTurnPos)
        {
            posP -= Time.deltaTime * speed_pos;

            if (posP <= 0f)
            {
                isTurnPos = !isTurnPos;
                //posP += 100f;
            }
        }
        else
        {
            posP += Time.deltaTime * speed_pos;

            if (posP >= 10000f)
            {
                isTurnPos = !isTurnPos;
                //posP -= 100f;
            }
        }

        for (int i = 0; i < mat_surface.Length; i++)
        {
            mat_surface[i].SetFloat("_SurfaceTime", timeP);
        }      

        if (mat_wave != null)
        {
            mat_wave.SetFloat("_SurfaceTime", timeP);

            waveP += Time.deltaTime * speed_wave;
            if (waveP >= 1f)
            {
                waveP -= 1f;
            }
            mat_wave.SetVector("_WaveOffset", new Vector2(0f, animCurve_waveP.Evaluate(waveP)));
            mat_wave.SetVector("_WaveTiling", new Vector2(2f, animCurve_waveS.Evaluate(waveP)));
            mat_wave.SetFloat("_Alpha1", animCurve_waveA.Evaluate(waveP));
            float waveP2 = waveP + 0.3333f;
            if (waveP2 >= 1f)
            {
                waveP2 -= 1f;
            }

            mat_wave.SetVector("_WaveOffset2", new Vector2(0f, animCurve_waveP.Evaluate(waveP2)));
            mat_wave.SetVector("_WaveTiling2", new Vector2(2f, animCurve_waveS.Evaluate(waveP2)));
            mat_wave.SetFloat("_Alpha2", animCurve_waveA.Evaluate(waveP2));
            float waveP3 = waveP + 0.6667f;
            if (waveP3 >= 1f)
            {
                waveP3 -= 1f;
            }

            mat_wave.SetVector("_WaveOffset3", new Vector2(0f, animCurve_waveP.Evaluate(waveP3)));
            mat_wave.SetVector("_WaveTiling3", new Vector2(2f, animCurve_waveS.Evaluate(waveP3)));
            mat_wave.SetFloat("_Alpha3", animCurve_waveA.Evaluate(waveP3));
        }

        if (mat_stagnant != null)
        {
            mat_stagnant.SetFloat("_SurfaceTime", timeP);

            stagnantP += Time.deltaTime * speed_stagnant;
            if (stagnantP >= 1f)
            {
                stagnantP -= 1f;
            }

            mat_stagnant.SetVector("_WaveOffset", new Vector2(stagnantP, 0f));
            mat_stagnant.SetFloat("_PosTime", posP);
        }

        if (mat_waterfall != null)
        {
            mat_waterfall.SetFloat("_SurfaceTime", timeP);

            waterfallP += Time.deltaTime * speed_waterfall;
            if (waterfallP >= 1f)
            {
                waterfallP -= 1f;
            }
            mat_waterfall.SetVector("_WaterfallOffset", new Vector2(0f, -waterfallP));
        }
    }

#if UNITY_EDITOR

    private void OnDestroy()
    {
        //Debug.Log("OnDestroy");
        SetDefaultData();
    }

    private void OnApplicationQuit()
    {
        //Debug.Log("OnApplicationQuit");
        SetDefaultData();
    }

    public void SetDefaultData()
    {
        for (int i = 0; i < mat_surface.Length; i++)
        {

            if (mat_surface[i] != null)
            {
                mat_surface[i].SetFloat("_SurfaceTime", 0f);
            }
        }

        if (mat_wave != null)
        {
            mat_wave.SetFloat("_SurfaceTime", 0f);

            mat_wave.SetVector("_WaveOffset", new Vector2(0f, 0f));
            mat_wave.SetVector("_WaveTiling", new Vector2(2f, 1f));
            mat_wave.SetFloat("_Alpha1", 1f);

            mat_wave.SetVector("_WaveOffset2", new Vector2(0f, 0f));
            mat_wave.SetVector("_WaveTiling2", new Vector2(2f, 1f));
            mat_wave.SetFloat("_Alpha2", 0f);

            mat_wave.SetVector("_WaveOffset3", new Vector2(0f, 0f));
            mat_wave.SetVector("_WaveTiling3", new Vector2(2f, 1f));
            mat_wave.SetFloat("_Alpha3", 0f);
        }

        if (mat_stagnant != null)
        {
            mat_stagnant.SetFloat("_SurfaceTime", 0f);
            mat_stagnant.SetVector("_WaveOffset", new Vector2(0f, 0f));
            mat_stagnant.SetFloat("_PosTime", 0f);
        }

        if (mat_waterfall != null)
        {
            mat_waterfall.SetFloat("_SurfaceTime", 0f);
            mat_waterfall.SetVector("_WaterfallOffset", new Vector2(0f, 0f));
        }
    }

#endif
}
