using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum ButtonRatio
{
    Square, Height, Width
}

public class MeshButtonCtrl : CustomizeProp
{
    public enum ButtonType
    {
        Normal, Keyboard
    }

    public ButtonType buttonType;

    private ButtonRatio buttonRatio;

    private Transform moveButtonTr;

    public LayerMask layerMask;

    private float halfExtents;

    public float limitPos = 0.15f;

    private bool isTrigger = false;

    private float startPosZ;

    private float ray_dist;

    private bool isDown = false;
    private float delayTime = 0;

    private bool isInteractable = true;
    private float interactableTime = 0;

    private bool isInit = false;

    public UnityEvent event_click;

    private MeshRenderer renderer_button;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {
        if (isInit)
        {
            return;
        }
        Vector3 temp_size = transform.GetComponent<BoxCollider>().size;

        if (Mathf.Abs(temp_size.x - temp_size.y) >= 0.001f)
        {
            if (temp_size.x - temp_size.y < 0f)
            {
                buttonRatio = ButtonRatio.Height;
                halfExtents = temp_size.x * 0.5f;
                ray_dist = (temp_size.y - temp_size.x) * 0.5f;
            }
            else
            {
                buttonRatio = ButtonRatio.Width;
                halfExtents = temp_size.y * 0.5f;
                ray_dist = (temp_size.x - temp_size.y) * 0.5f;
            }
        }
        else
        {
            buttonRatio = ButtonRatio.Square;
            halfExtents = temp_size.x * 0.5f;
        }

        moveButtonTr = transform.Find("Button");
        startPosZ = moveButtonTr.localPosition.z;

        renderer_button = moveButtonTr.GetComponentInChildren<MeshRenderer>();

        isInit = true;
    }

    void FixedUpdate()
    {
        if (!isInteractable)
        {
            return;
        }

        if (!Physics.autoSimulation)
        {
            MoveButton();
        }

        if (!isTrigger)
        {
            moveButtonTr.localPosition = Vector3.forward * startPosZ;
            isDown = false;
            delayTime = 0;
            interactableTime = 0.1f;
        }
        isTrigger = false;

        delayTime = Mathf.Clamp01(delayTime - Time.fixedUnscaledDeltaTime); 
    }

    public bool IsInteractable()
    {
        return isInteractable;
    }

    public void SetInteractable(bool _isInteractable)
    {
        Init();
        isInteractable = _isInteractable;
        if (!isInteractable)
        {
            moveButtonTr.localPosition = Vector3.forward * limitPos;
        }
    }

    private void OnEnable()
    {
        Init();
        if (isInteractable)
        {
            moveButtonTr.localPosition = Vector3.forward * startPosZ;
        }
        else
        {
            moveButtonTr.localPosition = Vector3.forward * limitPos;
        }
    }

    public void SetMaterial(Material mat)
    {
        Init();
        renderer_button.sharedMaterial = mat;
    }

    private void OnTriggerStay(Collider other)
    {
        MoveButton();
    }

    private void MoveButton()
    {
        if (!isInteractable)
        {
            return;
        }

        RaycastHit hit_1 = new RaycastHit();
        RaycastHit hit_2 = new RaycastHit();
        //Debug.LogError("sdfsdf");

        bool isHit_1 = false;
        bool isHit_2 = false;

        switch (buttonRatio)
        {
            case ButtonRatio.Square:
                {
                    isHit_1 = Physics.BoxCast(transform.position + transform.forward * -0.1f, Vector3.one * halfExtents, transform.forward, out hit_1, transform.rotation, 2f, layerMask);
                }
                break;
            case ButtonRatio.Height:
                {
                    isHit_1 = Physics.BoxCast(transform.position + transform.forward * -0.1f + transform.up * -ray_dist, Vector3.one * halfExtents, transform.forward, out hit_1, transform.rotation, 2f, layerMask);
                    isHit_2 = Physics.BoxCast(transform.position + transform.forward * -0.1f + transform.up * ray_dist, Vector3.one * halfExtents, transform.forward, out hit_2, transform.rotation, 2f, layerMask);

                }
                break;
            case ButtonRatio.Width:
                {
                    isHit_1 = Physics.BoxCast(transform.position + transform.forward * -0.1f + transform.right * -ray_dist, Vector3.one * halfExtents, transform.forward, out hit_1, transform.rotation, 2f, layerMask);
                    isHit_2 = Physics.BoxCast(transform.position + transform.forward * -0.1f + transform.right * ray_dist, Vector3.one * halfExtents, transform.forward, out hit_2, transform.rotation, 2f, layerMask);
                }
                break;
        }

        if (isHit_1 || isHit_2)
        {
            Vector3 pos;

            if (buttonRatio == ButtonRatio.Square)
            {
                pos = transform.InverseTransformPoint(hit_1.point);
            }
            else
            {
                Vector3 pos_1 = transform.InverseTransformPoint(hit_1.point);
                Vector3 pos_2 = transform.InverseTransformPoint(hit_2.point);

                if (!isHit_2)
                {
                    pos = pos_1;
                }
                else if (!isHit_1)
                {
                    pos = pos_2;
                }
                else
                {
                    pos = (pos_1.z < pos_2.z ? pos_1 : pos_2);
                }
            }

            moveButtonTr.localPosition = Vector3.forward * Mathf.Clamp(pos.z, limitPos, startPosZ);
            isTrigger = true;
            if (interactableTime > 0f)
            {
                interactableTime -= Time.fixedDeltaTime;
                return;
            }

            if (moveButtonTr.localPosition.z == limitPos && delayTime <= 0f)
            {
                if (buttonType == ButtonType.Keyboard)
                {
                    if (!isDown)
                    {
                        isDown = true;
                        delayTime = 0.5f;
                    }
                    else
                    {
                        delayTime = 0.1f;
                    }
                    event_click.Invoke();
                }
                else
                {
                    if (!isDown)
                    {
                        isDown = true;
                        event_click.Invoke();
                    }
                }
            }
            else if (isDown)
            {
                if ((moveButtonTr.localPosition.z - limitPos) >= 0.001f)
                {
                    isDown = false;
                    delayTime = 0;
                }
            }
            //isTrigger = true;
        }
    }

    public override Vector3 GetCenterPos()
    {
        return transform.position;
    }
    public override void SetSize(bool isSelect)
    {
        //
    }

}
