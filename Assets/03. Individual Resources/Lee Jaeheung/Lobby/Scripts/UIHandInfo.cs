using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
//[System.Serializable]
public class UIHandInfo
{
    public XRController controller;

    public bool isControllerOn = false;

    public bool isTriggerOn = false;

    public TriggerEventState buttonState = TriggerEventState.None;
    public enum TriggerEventState
    {
        None, On, Off
    }

    public PointState pointState = PointState.Idle;
    public Animator anim;
    public Transform rayDir;
    public Transform line_transform;
    public Transform ball_transform;

    public Collider hitCollider;
    public Vector3 hitPoint;

    public enum PointState
    {
        Idle, Point, PropLine, UI
    }

    public void InitData()
    {
        buttonState = TriggerEventState.None;
        isTriggerOn = false;

        pointState = PointState.Idle;
        anim.SetBool("IsPoint", false);
        ball_transform.gameObject.SetActive(false);
        line_transform.gameObject.SetActive(false);
    }

    public void SetButtonEventState(TriggerEventState state)
    {
        if (buttonState == state)
        {
            return;
        }
        buttonState = state;

        switch (buttonState)
        {
            case TriggerEventState.On:
                {
                    isTriggerOn = true;
                }
                break;
            case TriggerEventState.Off:
                {
                    isTriggerOn = false;
                }
                break;
        }
    }

    public void SetPointState(PointState state)
    {
        if (pointState == state)
        {
            return;
        }

        pointState = state;

        switch (state)
        {
            case PointState.Idle:
                {
                    anim.SetBool("IsPoint", false);
                    ball_transform.gameObject.SetActive(false);
                    line_transform.gameObject.SetActive(false);
                }
                break;
            case PointState.Point:
                {
                    anim.SetBool("IsPoint", true);
                    ball_transform.gameObject.SetActive(false);
                    line_transform.gameObject.SetActive(false);
                }
                break;
            case PointState.PropLine:
                {
                    anim.SetBool("IsPoint", false);
                    ball_transform.gameObject.SetActive(false);
                    line_transform.gameObject.SetActive(true);
                }
                break;
            case PointState.UI:
                {
                    anim.SetBool("IsPoint", true);
                    ball_transform.gameObject.SetActive(true);
                    line_transform.gameObject.SetActive(true);
                }
                break;
        }
    }

    public void SetUITransform(Vector3 ballPos, Vector3 linePos, Quaternion lineRot, float lineScaleP)
    {
        ball_transform.position = ballPos;
        line_transform.SetPositionAndRotation(linePos, lineRot);
        line_transform.localScale = Vector3.forward * lineScaleP;
    }
}
