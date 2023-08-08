using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimSyncCtrl : MonoBehaviour
{
    public GameDataManager.GameType gameType;
    public Animator anim_h;
    public Animator[] anim_g;

    private bool isPlay = false;
    private bool isCenter = false;
    private bool isMale = false;

    private void OnEnable()
    {
        SetAnim();
    }

    public void Init(bool _isMale)
    {
        isMale = _isMale;
        SetAnim();
    }

    private void SetAnim()
    {
        for (int i = 0; i < anim_g.Length; i++)
        {
            anim_g[i].SetBool("IsMale", isMale);
            anim_g[i].SetInteger("StateNum", (int)gameType);
            anim_g[i].SetTrigger("OnState");
            anim_g[i].SetBool("IsPlay", isPlay);
            anim_g[i].SetBool("IsCenter", isCenter);
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        for (int i = 0; i < anim_g.Length; i++)
        {
            if (anim_h.GetBool("IsCenter"))
            {
                if (!isCenter)
                {
                    if (i == anim_g.Length - 1)
                    {
                        isCenter = true;
                    }
                    anim_g[i].SetBool("IsCenter", true);
                }
            }
            else
            {
                if (isCenter)
                {
                    if (i == anim_g.Length - 1)
                    {
                        isCenter = false;
                    }
                    anim_g[i].SetBool("IsCenter", false);
                }
            }

            if (anim_h.GetBool("IsPlay"))
            {
                if (!isPlay)
                {
                    if (i == anim_g.Length-1)
                    {
                        isPlay = true;
                    }
                    anim_g[i].SetBool("IsPlay", true);
                }
            }
            else
            {
                if (isPlay)
                {
                    if (i == anim_g.Length - 1)
                    {
                        isPlay = false;
                    }
                    anim_g[i].SetBool("IsPlay", false);
                }
            }

            if (isPlay || isCenter)
            {
                anim_g[i].SetFloat("time", anim_h.GetCurrentAnimatorStateInfo(0).normalizedTime);
            }
        }
    }
}
