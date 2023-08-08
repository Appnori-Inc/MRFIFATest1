using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeAnimationCtrl : MonoBehaviour
{
    private int stateNum = 0;
    private float playTime = 0f;
    private float endTime = 0f;
    private float setTime = 0f;
    private Vector2 offset = new Vector2();
    private Vector2 offset_idle = new Vector2(0.5f,0f);

    private MeshRenderer renderer;
    private bool isOpenEye = false;

    public bool isSingleton = false;

    private static EyeAnimationCtrl Instance = null;
    public static EyeAnimationCtrl GetInstance
    {
        get
        {
            return Instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (isSingleton)
        {
            Instance = this;
        }

        renderer = transform.GetComponent<MeshRenderer>();
        SetExpression(0, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        playTime -= Time.unscaledDeltaTime;

        if (playTime <= 0f)
        {
            playTime = Random.Range(1f, 3.5f);
        }


        if (!isOpenEye && playTime > 0.1f)
        {
            isOpenEye = true;
            renderer.material.SetVector("_Offset_Eye", offset);
        }
        else if (isOpenEye && playTime <= 0.1f)
        {
            isOpenEye = false;
            renderer.material.SetVector("_Offset_Eye", offset_idle);
        }

        if (stateNum != 0)
        {
            endTime -= Time.unscaledDeltaTime;
            if (endTime <= 0f)
            {
                if (stateNum == 15)
                {
                    SetExpression(16, setTime);
                }
                else if (stateNum == 16)
                {
                    SetExpression(17, setTime);
                }
                else
                {
                    SetExpression(0, 0f);
                }
            }
        }
    }

    public void SetExpression(int setState, float _setTime)
    {
        stateNum = setState;
        setTime = _setTime;
        switch (setState)
        {
            case 0: // 기본.
                {
                    offset = new Vector2(0f, 0f);
                    renderer.material.SetVector("_Offset_Eye", offset);
                    renderer.material.SetVector("_Offset_Eyebrow", new Vector2(0f, 0f));
                    renderer.material.SetVector("_Offset_Mouth", new Vector2(0f, 0f));
                }
                break;
            case 1: // 기쁨.
                {
                    offset = new Vector2(0f, 0f);
                    renderer.material.SetVector("_Offset_Eye", offset);
                    renderer.material.SetVector("_Offset_Eyebrow", new Vector2(0.5f, 0.5f));
                    renderer.material.SetVector("_Offset_Mouth", new Vector2(0.5f, 0f));
                }
                break;
            case 2: // 슬픔.
                {
                    offset = new Vector2(0.5f, 0.5f);
                    renderer.material.SetVector("_Offset_Eye", offset);
                    renderer.material.SetVector("_Offset_Eyebrow", new Vector2(0.5f, 0f));
                    renderer.material.SetVector("_Offset_Mouth", new Vector2(0.5f, 0.5f));
                }
                break;
            case 3: // 화남.
                {
                    offset = new Vector2(0f, 0.5f);
                    renderer.material.SetVector("_Offset_Eye", offset);
                    renderer.material.SetVector("_Offset_Eyebrow", new Vector2(0f, 0.5f));
                    renderer.material.SetVector("_Offset_Mouth", new Vector2(0f, 0.5f));
                }
                break;
            case 4: // 복싱_데미지.
                {
                    float rand = Random.Range(0f, 1f);

                    if (rand >= 0.667f)
                    {
                        offset = new Vector2(0.5f, 0f);
                    }
                    else if (rand >= 0.333f)
                    {
                        offset = new Vector2(0f, 0.5f);
                    }
                    else
                    {
                        offset = new Vector2(0.5f, 0.5f);
                    }

                    renderer.material.SetVector("_Offset_Eye", offset);
                    if (Random.Range(0f, 1f) >= 0.5f)
                    {
                        renderer.material.SetVector("_Offset_Eyebrow", new Vector2(0.5f, 0f));
                    }
                    else
                    {
                        renderer.material.SetVector("_Offset_Eyebrow", new Vector2(0f, 0.5f));
                    }
                    renderer.material.SetVector("_Offset_Mouth", new Vector2(0f, 0.5f));
                }
                break;
            case 15: // 심판 외치기
                {
                    offset = new Vector2(0f, 0.5f);
                    renderer.material.SetVector("_Offset_Eye", offset);
                    renderer.material.SetVector("_Offset_Eyebrow", new Vector2(0f, 0.5f));
                    renderer.material.SetVector("_Offset_Mouth", new Vector2(0f, 0.5f));

                    endTime = 0.15f;
                }
                return;
            case 16: // 심판 외치기2
                {
                    offset = new Vector2(0f, 0.5f);
                    renderer.material.SetVector("_Offset_Eye", offset);
                    renderer.material.SetVector("_Offset_Eyebrow", new Vector2(0f, 0.5f));
                    renderer.material.SetVector("_Offset_Mouth", new Vector2(0.5f, 0f));

                    endTime = 0.35f;
                }
                return;
            case 17: // 심판 외치기3
                {
                    offset = new Vector2(0f, 0.5f);
                    renderer.material.SetVector("_Offset_Eye", offset);
                    renderer.material.SetVector("_Offset_Eyebrow", new Vector2(0f, 0.5f));
                    renderer.material.SetVector("_Offset_Mouth", new Vector2(0.5f, 0.5f));

                    endTime = setTime;
                }
                return;
            default:
                {
                    offset = new Vector2((Random.Range(0f,1f) >= 0.5f ? 0 : 0.5f), (Random.Range(0f, 1f) >= 0.5f ? 0 : 0.5f));
                    renderer.material.SetVector("_Offset_Eye", offset);
                    renderer.material.SetVector("_Offset_Eyebrow", new Vector2((Random.Range(0f, 1f) >= 0.5f ? 0 : 0.5f), (Random.Range(0f, 1f) >= 0.5f ? 0 : 0.5f)));
                    renderer.material.SetVector("_Offset_Mouth", new Vector2((Random.Range(0f, 1f) >= 0.5f ? 0 : 0.5f), (Random.Range(0f, 1f) >= 0.5f ? 0 : 0.5f)));
                }
                break;
        }
        endTime = setTime;
    }

}
