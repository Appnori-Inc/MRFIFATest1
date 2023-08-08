using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using Billiards;
using Appnori.Util;

public class PlatformCameraSetting : MonoSingleton<PlatformCameraSetting>
{

    private readonly Notifier<Camera> MainCamera = new Notifier<Camera>();

    private const int CheckFrameCount = 10;
    private int frameCount = 0;
    private bool isChecked = false;

    [Header("기억용 프로파일들")]
    [SerializeField]
    private List<UnityEngine.Rendering.VolumeProfile> profiles;
    protected override void Awake()
    {
        base.Awake();

        if (_instance != this)
        {
            DestroyImmediate(gameObject);
            return;
        }

        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        MainCamera.OnDataChanged += MainCamera_OnDataChanged;
    }

    private void MainCamera_OnDataChanged(Camera mainCamera)
    {
        if (mainCamera == null)
            return;

        isChecked = true;
        if (mainCamera.TryGetComponent<UniversalAdditionalCameraData>(out var cameraData))
        {
#if UNITY_ANDROID
            cameraData.antialiasing = AntialiasingMode.None;
            cameraData.renderPostProcessing = false;
            cameraData.renderShadows = false;
            mainCamera.allowHDR = false;
#elif UNITY_STANDALONE
            cameraData.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
            cameraData.renderPostProcessing = true;            
            cameraData.renderShadows = true;
            mainCamera.allowHDR = true;
#endif
        }
    }

    private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
    {
        isChecked = false;
    }


    void Update()
    {
        if (isChecked)
            return;

        if (++frameCount > CheckFrameCount)
        {
            MainCamera.Value = Camera.main;
        }
    }
}
