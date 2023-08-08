//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;


////public enum ColorUIPart
////{
////    Face, Hair, Wear
////}


//public class CustomizeColorCtrl : CustomizeUIProp
//{
//    public ColorSlotInfo slotInfo;

//    public Color colorVelue;

//    public ColorUIPart color_part;



//    private Transform moveButtonTr;

//    public LayerMask layerMask;

//    private float halfExtents;

//    public float limitPos = 0.15f;

//    private bool isTrigger = false;

//    private float startPosY;

//    void Start()
//    {
//        halfExtents = transform.GetComponent<BoxCollider>().size.x * 0.5f;
//        moveButtonTr = transform.Find("Button");
//        startPosY = moveButtonTr.localPosition.y;


//        //CustomizeManager.GetInstance.InitColorSlot(this);
//    }

//    public void SetSlot(ColorSlotInfo _slotInfo)
//    {
//        slotInfo = _slotInfo;

//        switch (color_part)
//        {
//            case ColorUIPart.Face:
//                {
//                    if (ColorUtility.TryParseHtmlString("#" + CustomizeManager.GetInstance.GetColorSlotInfo(slotInfo.id).hex_color, out colorVelue))
//                    {
//                        moveButtonTr.GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_BaseColor", colorVelue);
//                    }

//                    if (CustomizeManager.GetInstance.select_id_faceColor == slotInfo.id)
//                    {
//                        SetButtonPush();
//                    }
//                }
//                break;
//            case ColorUIPart.Hair:
//                {
//                    if (ColorUtility.TryParseHtmlString("#" + CustomizeManager.GetInstance.GetColorSlotInfo(slotInfo.id).hex_color, out colorVelue))
//                    {
//                        moveButtonTr.GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_BaseColor", colorVelue);
//                    }


//                    if (CustomizeManager.GetInstance.select_id_hairColor == slotInfo.id)
//                    {
//                        SetButtonPush();
//                    }
//                }
//                break;
//            case ColorUIPart.Wear:
//                {

//                }
//                break;
//        }
//    }

//    public void SetColor()
//    {
//        CustomizeManager.GetInstance.characterCtrl.SetColor(slotInfo);
//    }

//    public override Vector3 GetCenterPos()
//    {
//        return transform.position;
//    }

//    public override void SetSize(bool isSelect)
//    {
//        //if (isSelect)
//        //{
//        //    transform.localScale = Vector3.one * 1.2f;
//        //}
//        //else
//        //{
//        //    transform.localScale = Vector3.one;
//        //}
//    }


//    private void FixedUpdate()
//    {
//        switch (color_part)
//        {
//            case ColorUIPart.Face:
//                {
//                    if (CustomizeManager.GetInstance.select_id_faceColor == slotInfo.id)
//                    {
//                        return;
//                    }
//                }
//                break;
//            case ColorUIPart.Hair:
//                {
//                    if (CustomizeManager.GetInstance.select_id_hairColor == slotInfo.id)
//                    {
//                        return;
//                    }
//                }
//                break;
//            case ColorUIPart.Wear:
//                {

//                }
//                break;
//        }

//        if (!isTrigger)
//        {
//            moveButtonTr.localPosition = Vector3.up * startPosY;
//        }
//        isTrigger = false;
//    }

//    private void OnTriggerStay(Collider other)
//    {
//        switch (color_part)
//        {
//            case ColorUIPart.Face:
//                {
//                    if (CustomizeManager.GetInstance.select_id_faceColor == slotInfo.id)
//                    {
//                        return;
//                    }
//                }
//                break;
//            case ColorUIPart.Hair:
//                {
//                    if (CustomizeManager.GetInstance.select_id_hairColor == slotInfo.id)
//                    {
//                        return;
//                    }
//                }
//                break;
//            case ColorUIPart.Wear:
//                {

//                }
//                break;
//        }


//        RaycastHit hit;
//        //Debug.LogError("sdfsdf");
//        if (Physics.BoxCast(transform.position + transform.up * -1f, Vector3.one * halfExtents, transform.up, out hit, transform.rotation, 2f, layerMask))
//        {
//            Vector3 pos = transform.InverseTransformPoint(hit.point);
//            moveButtonTr.localPosition = Vector3.up * Mathf.Clamp(pos.y, limitPos, startPosY);

//            if (moveButtonTr.localPosition.y == limitPos)
//            {
//                switch (color_part)
//                {
//                    case ColorUIPart.Face:
//                        {
//                            if (CustomizeManager.GetInstance.select_id_faceColor != slotInfo.id)
//                            {
//                                CustomizeManager.GetInstance.characterCtrl.SetColor(slotInfo);
//                            }
//                        }
//                        break;
//                    case ColorUIPart.Hair:
//                        {
//                            if (CustomizeManager.GetInstance.select_id_hairColor != slotInfo.id)
//                            {
//                                CustomizeManager.GetInstance.characterCtrl.SetColor(slotInfo);
//                            }
//                        }
//                        break;
//                    case ColorUIPart.Wear:
//                        {

//                        }
//                        break;
//                }
//            }

//            isTrigger = true;
//        }
//    }

//    public void SetButtonPush()
//    {
//        moveButtonTr.localPosition = Vector3.up * limitPos;
//    }

//    public void CreateColorPalette()
//    {
//        Texture2D tex = new Texture2D(512, 512);

//        for (int w = 0; w < tex.width; w++)
//        {
//            for (int h = 0; h < tex.height; h++)
//            {
//                //2f - ((1f / ((tex.height - 1f) * 0.5f)) * h)
//                //(1f / ((tex.height * 0.5f) - 1f)) * h
//                //(1f/ (tex.width - 1))*w.
//                tex.SetPixel(w, h, Color.HSVToRGB((1f / (tex.width - 1)) * w, Mathf.Clamp01(2f - ((1f / ((tex.height - 1f) * 0.5f)) * h)), Mathf.Clamp01((1f / ((tex.height - 1f) * 0.5f)) * h)));
//            }
//        }

//        System.IO.File.WriteAllBytes("Assets/03. Individual Resources/Lee Jaeheung/Images/Image_ColorPalette.png", tex.EncodeToPNG());
//    }
//}
