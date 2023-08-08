using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MeshFadeCtrl : MonoBehaviour
{
    public static MeshFadeCtrl instance;

    public bool FadeOnAwake = false;

    private MeshRenderer render;
    private FadeState fadeState;

    private bool isFadeIn = false;
    private float fadeTime;
    private static int sceneIndex;
    private static string sceneName;

    private Action action = null;

    private bool isInit = false;

    public enum FadeState
    {
        Normal, LoadScene
    }

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        InitData();
    }

    void InitData()
    {
        if (isInit)
        {
            return;
        }
        isInit = true;
        render = gameObject.AddComponent<MeshRenderer>();
        render.materials = new Material[1] { new Material(Shader.Find("Oculus/Unlit Transparent Color")) };
        render.materials[0].renderQueue = 5000;
        render.materials[0].SetColor("_Color", Color.black);

        MeshFilter fadeMesh = gameObject.AddComponent<MeshFilter>();

        Mesh mesh = new Mesh();
        fadeMesh.mesh = mesh;

        Vector3[] vertices = new Vector3[4];

        float width = 2f;
        float height = 2f;
        float depth = 1f;

        vertices[0] = new Vector3(-width, -height, depth);
        vertices[1] = new Vector3(width, -height, depth);
        vertices[2] = new Vector3(-width, height, depth);
        vertices[3] = new Vector3(width, height, depth);

        mesh.vertices = vertices;

        int[] tri = new int[6];

        tri[0] = 0;
        tri[1] = 2;
        tri[2] = 1;

        tri[3] = 2;
        tri[4] = 3;
        tri[5] = 1;

        mesh.triangles = tri;

        Vector3[] normals = new Vector3[4];

        normals[0] = -Vector3.forward;
        normals[1] = -Vector3.forward;
        normals[2] = -Vector3.forward;
        normals[3] = -Vector3.forward;

        mesh.normals = normals;

        Vector2[] uv = new Vector2[4];

        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(0, 1);
        uv[3] = new Vector2(1, 1);

        mesh.uv = uv;

        render.enabled = FadeOnAwake;
    }

    public void StartFade(bool _isFadeIn, float _fadeTime = 1.5f, Action _action = null)
    {
        InitData();

        if (cor_fade != null)
        {
            StopCoroutine(cor_fade);
        }

        fadeState = FadeState.Normal;
        isFadeIn = _isFadeIn;
        fadeTime = _fadeTime;
        action = _action;

        render.materials[0].SetColor("_Color", _isFadeIn ? Color.black : Color.clear);
        render.enabled = true;

        //GameDataManager.instance.Temp_Fade();
        cor_fade = StartCoroutine(FadeCoroutine());
    }

    public void LoadScene(int _sceneIndex, float _fadeTime = 1.5f)
    {
        if (cor_fade != null)
        {
            StopCoroutine(cor_fade);
        }

        fadeState = FadeState.LoadScene;
        isFadeIn = false;
        action = null;

        if (_sceneIndex >= 0)
        {
            sceneIndex = _sceneIndex;
        }
        fadeTime = _fadeTime;

        render.materials[0].SetColor("_Color", Color.clear);
        render.enabled = true;

        //GameDataManager.instance.Temp_Fade();
        cor_fade = StartCoroutine(FadeCoroutine());
    }

    public void LoadScene(string _sceneName, float _fadeTime = 1.5f)
    {
        if (cor_fade != null)
        {
            StopCoroutine(cor_fade);
        }

        fadeState = FadeState.LoadScene;
        isFadeIn = false;
        action = null;

        if (!string.IsNullOrWhiteSpace(_sceneName))
        {
            sceneIndex = -1;
            sceneName = _sceneName;
        }
        fadeTime = _fadeTime;

        /*if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Lobby")
        {
            LobbyUIManager.GetInstance.gameSetting.SaveData();
        }*/

        render.materials[0].SetColor("_Color", Color.clear);
        render.enabled = true;

        //GameDataManager.instance.Temp_Fade();
        cor_fade = StartCoroutine(FadeCoroutine());
    }

    Coroutine cor_fade;

    public IEnumerator FadeCoroutine()
    {
        float valueP = 0f;
        float speed = 1f;
        if (fadeTime > 0f)
        {
            speed = 1 / fadeTime;
        }

        yield return null;

        if (isFadeIn)
        {
            while (valueP < 1f)
            {
                valueP = Mathf.Clamp01(valueP + Time.deltaTime * speed);
                render.materials[0].SetColor("_Color", Color.Lerp(Color.black, Color.clear, valueP));

                yield return null;
            }

            if (action != null)
            {
                action.Invoke();
            }

            render.enabled = false;
        }
        else
        {
            while (valueP < 1f)
            {
                //Debug.Log(valueP);
                valueP = Mathf.Clamp01(valueP + Time.deltaTime * speed);
                render.materials[0].SetColor("_Color", Color.Lerp(Color.clear, Color.black, valueP));
                yield return null;
            }

            if (action != null)
            {
                action.Invoke();
            }

            if (fadeState == FadeState.LoadScene)
            {
                if (sceneIndex == -1)
                {
                    if (GameDataManager.instance.playType == GameDataManager.PlayType.Multi)
                    {
                        PhotonNetwork.LoadLevel(sceneName);
                    }
                    else
                    {
                        if (sceneName == "Lobby")
                        {
                            if (PhotonNetwork.IsConnected)
                            {
                                PhotonNetwork.IsSyncScene = false;
                                PhotonNetwork.Disconnect();
                            }
                        }
                        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
                    }
                }
                else
                {
                    if (GameDataManager.instance.playType == GameDataManager.PlayType.Multi)
                    {
                        PhotonNetwork.LoadLevel(sceneIndex);
                    }
                    else
                    {
                        if (sceneIndex == 0)
                        {
                            if (PhotonNetwork.IsConnected)
                            {
                                PhotonNetwork.IsSyncScene = false;
                                PhotonNetwork.Disconnect();
                            }
                        }
                        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
                    }
                }
            }
            else if (GameDataManager.instance.playType == GameDataManager.PlayType.Multi)
            {
                yield return new WaitForSeconds(6f);
                if (PhotonNetwork.IsConnected)
                {
                    PhotonNetwork.Disconnect();
                }
                render.materials[0].SetColor("_Color", Color.clear);
                render.enabled = false;

                LobbyUIManager.GetInstance.isStartGame = false;
                LobbyUIManager.GetInstance.SetLobbyUI(LobbyUIManager.LobbyUIState.RoomOut);
            }
        }
    }
}
