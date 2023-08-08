using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public enum ColorUIPart
{
    Skin, Eye, Eyebrow, Hair, Upper, Lower, Foot, Pattern, Acc
}

public class ColorPaletteCtrl : MonoBehaviour
{
    private ButtonRatio buttonRatio;

    public Color colorVelue;

    public string hexColor;

    public ColorUIPart color_part;

    public LayerMask layerMask;

    private float halfExtents;

    private float ray_dist;

    private Vector3 center;

    public MeshRenderer renderer_palette;

    public Transform transform_h;
    public TextMesh text_h;
    public MeshRenderer renderer_h;

    public Transform transform_sv;
    public TextMesh text_sv;
    public MeshRenderer renderer_sv;

    private float colorSumVelue;
    private float colorMulVelue;

    private Vector3 hsv = new Vector3(0.5f, 0.5f, 0.5f);
    private Vector3 keep_hsv = new Vector3(0.5f, 0.5f, 0.5f);
    public MeshButtonCtrl[] buttons_part;

    //private float keepP_h;
    //private Vector2 keepP_sv;

    private float delayTIme_sound;

    // Start is called before the first frame update
    void Start()
    {
        BoxCollider collider = transform.GetComponent<BoxCollider>();
        center = transform.TransformPoint(collider.center);

        Vector3 temp_size = collider.size;

        colorSumVelue = temp_size.y * 0.5f;

        colorMulVelue = 1 / temp_size.y;

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

        CustomizeManager.GetInstance.InitColorSlot(this);
    }

    private void Update()
    {
        if (delayTIme_sound > 0f)
        {
            delayTIme_sound -= Time.deltaTime;
        }
    }

    public void SetSlot()
    {
        string hexColor = CustomizeManager.GetInstance.GetColorInfo(color_part);

        if (!ColorUtility.TryParseHtmlString("#" + hexColor, out colorVelue))
        {
            return;
        }
        Color.RGBToHSV(colorVelue, out hsv.x, out hsv.y, out hsv.z);
        //1f - Mathf.Clamp01((transform_h.localPosition.y + colorSumVelue) * colorMulVelue)
        transform_h.localPosition = new Vector3(0f, ((1f - hsv.x) / colorMulVelue) - colorSumVelue, 0f);
        transform_sv.localPosition = new Vector3(((1f - hsv.y) / colorMulVelue) - colorSumVelue, (hsv.z / colorMulVelue) - colorSumVelue, 0f);
        keep_hsv = hsv;

        renderer_h.material.SetColor("_Color", Color.HSVToRGB(hsv.x, 1f, 1f));
        renderer_sv.material.SetColor("_Color", colorVelue);
        renderer_palette.material.SetFloat("_Hue", hsv.x);
        text_h.text = string.Format("H : {0:F2}", hsv.x);
        text_sv.text = string.Format("S : {0:F2} / V : {1:F2}", hsv.y, hsv.z);

        switch (color_part)      
        {
            case ColorUIPart.Skin:
                {
                    for (int i = 0; i < buttons_part.Length; i++)
                    {
                        if (i == 0)
                        {
                            buttons_part[i].SetMaterial(CustomizeManager.GetInstance.materials_customUI[1]);
                            buttons_part[i].SetInteractable(false);
                        }
                        else
                        {
                            buttons_part[i].SetMaterial(CustomizeManager.GetInstance.materials_customUI[0]);
                            buttons_part[i].SetInteractable(true);
                        }
                    }
                }
                break;
            case ColorUIPart.Eye:
                {
                    for (int i = 0; i < buttons_part.Length; i++)
                    {
                        if (i == 1)
                        {
                            buttons_part[i].SetMaterial(CustomizeManager.GetInstance.materials_customUI[1]);
                            buttons_part[i].SetInteractable(false);
                        }
                        else
                        {
                            buttons_part[i].SetMaterial(CustomizeManager.GetInstance.materials_customUI[0]);
                            buttons_part[i].SetInteractable(true);
                        }
                    }
                }
                break;
            case ColorUIPart.Eyebrow:
                {
                    for (int i = 0; i < buttons_part.Length; i++)
                    {
                        if (i == 2)
                        {
                            buttons_part[i].SetMaterial(CustomizeManager.GetInstance.materials_customUI[1]);
                            buttons_part[i].SetInteractable(false);
                        }
                        else
                        {
                            buttons_part[i].SetMaterial(CustomizeManager.GetInstance.materials_customUI[0]);
                            buttons_part[i].SetInteractable(true);
                        }
                    }
                }
                break;
            case ColorUIPart.Hair:
                {
                    for (int i = 0; i < buttons_part.Length; i++)
                    {
                        if (i == 3)
                        {
                            buttons_part[i].SetMaterial(CustomizeManager.GetInstance.materials_customUI[1]);
                            buttons_part[i].SetInteractable(false);
                        }
                        else
                        {
                            buttons_part[i].SetMaterial(CustomizeManager.GetInstance.materials_customUI[0]);
                            buttons_part[i].SetInteractable(true);
                        }
                    }
                }
                break;
            case ColorUIPart.Upper:
                {
                    for (int i = 0; i < buttons_part.Length; i++)
                    {
                        if (i == 4)
                        {
                            buttons_part[i].SetMaterial(CustomizeManager.GetInstance.materials_customUI[1]);
                            buttons_part[i].SetInteractable(false);
                        }
                        else
                        {
                            buttons_part[i].SetMaterial(CustomizeManager.GetInstance.materials_customUI[0]);
                            buttons_part[i].SetInteractable(true);
                        }
                    }
                }
                break;
            case ColorUIPart.Lower:
                {
                    for (int i = 0; i < buttons_part.Length; i++)
                    {
                        if (i == 5)
                        {
                            buttons_part[i].SetMaterial(CustomizeManager.GetInstance.materials_customUI[1]);
                            buttons_part[i].SetInteractable(false);
                        }
                        else
                        {
                            buttons_part[i].SetMaterial(CustomizeManager.GetInstance.materials_customUI[0]);
                            buttons_part[i].SetInteractable(true);
                        }
                    }
                }
                break;
            case ColorUIPart.Foot:
                {
                    for (int i = 0; i < buttons_part.Length; i++)
                    {
                        if (i == 6)
                        {
                            buttons_part[i].SetMaterial(CustomizeManager.GetInstance.materials_customUI[1]);
                            buttons_part[i].SetInteractable(false);
                        }
                        else
                        {
                            buttons_part[i].SetMaterial(CustomizeManager.GetInstance.materials_customUI[0]);
                            buttons_part[i].SetInteractable(true);
                        }
                    }
                }
                break;
            case ColorUIPart.Pattern:
                {
                    for (int i = 0; i < buttons_part.Length; i++)
                    {
                        if (i == 7)
                        {
                            buttons_part[i].SetMaterial(CustomizeManager.GetInstance.materials_customUI[1]);
                            buttons_part[i].SetInteractable(false);
                        }
                        else
                        {
                            buttons_part[i].SetMaterial(CustomizeManager.GetInstance.materials_customUI[0]);
                            buttons_part[i].SetInteractable(true);
                        }
                    }
                }
                break;
            case ColorUIPart.Acc:
                {
                    for (int i = 0; i < buttons_part.Length; i++)
                    {
                        if (i == 8)
                        {
                            buttons_part[i].SetMaterial(CustomizeManager.GetInstance.materials_customUI[1]);
                            buttons_part[i].SetInteractable(false);
                        }
                        else
                        {
                            buttons_part[i].SetMaterial(CustomizeManager.GetInstance.materials_customUI[0]);
                            buttons_part[i].SetInteractable(true);
                        }
                    }
                }
                break;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        RaycastHit hit_1 = new RaycastHit();
        RaycastHit hit_2 = new RaycastHit();

        bool isHit_1 = false;
        bool isHit_2 = false;

        switch (buttonRatio)
        {
            case ButtonRatio.Square:
                {
                    isHit_1 = Physics.BoxCast(center + transform.forward * -1f, Vector3.one * halfExtents, transform.forward, out hit_1, transform.rotation, 2f, layerMask);
                }
                break;
            case ButtonRatio.Height:
                {
                    isHit_1 = Physics.BoxCast(center + transform.forward * -1f + transform.up * -ray_dist, Vector3.one * halfExtents, transform.forward, out hit_1, transform.rotation, 2f, layerMask);
                    isHit_2 = Physics.BoxCast(center + transform.forward * -1f + transform.up * ray_dist, Vector3.one * halfExtents, transform.forward, out hit_2, transform.rotation, 2f, layerMask);

                }
                break;
            case ButtonRatio.Width:
                {
                    isHit_1 = Physics.BoxCast(center + transform.forward * -1f + transform.right * -ray_dist, Vector3.one * halfExtents, transform.forward, out hit_1, transform.rotation, 2f, layerMask);
                    isHit_2 = Physics.BoxCast(center + transform.forward * -1f + transform.right * ray_dist, Vector3.one * halfExtents, transform.forward, out hit_2, transform.rotation, 2f, layerMask);
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
            if (pos.z > 0f || (pos.x > 0.11f && pos.x < 0.14f))
            {
                return;
            }

            pos.z = 0f;

            if (pos.x > 0.125f) // 색상.
            {
                pos.x = 0f;
                transform_h.localPosition = pos;

                hsv.x = 1f - Mathf.Clamp01((transform_h.localPosition.y + colorSumVelue) * colorMulVelue);

                if (Mathf.Abs(keep_hsv.x - hsv.x) >= 0.05f && delayTIme_sound <= 0f)
                {
                    delayTIme_sound = 0.05f;
                    LobbySoundManager.GetInstance.Play(4);
                    keep_hsv.x = hsv.x;
                }
                Color getColor = Color.HSVToRGB(hsv.x, hsv.y, hsv.z);

                renderer_h.material.SetColor("_Color", Color.HSVToRGB(hsv.x, 1f, 1f));
                renderer_sv.material.SetColor("_Color", getColor);
                renderer_palette.material.SetFloat("_Hue", hsv.x);
                text_h.text = string.Format("H : {0:F2}", hsv.x);
                //Debug.LogError(ColorUtility.ToHtmlStringRGB(getColor));
                CustomizeManager.GetInstance.characterCtrl.SetColor(color_part, ColorUtility.ToHtmlStringRGB(getColor));
            }
            else // 명도, 채도.
            {
                transform_sv.localPosition = pos;

                hsv.y = 1f - Mathf.Clamp01((transform_sv.localPosition.x + colorSumVelue) * colorMulVelue);
                hsv.z = Mathf.Clamp01((transform_sv.localPosition.y + colorSumVelue) * colorMulVelue);

                if ((new Vector2(keep_hsv.y, keep_hsv.z) - new Vector2(hsv.y, hsv.z)).sqrMagnitude >= 0.005f && delayTIme_sound <= 0f)
                {
                    delayTIme_sound = 0.05f;
                    LobbySoundManager.GetInstance.Play(4);
                    keep_hsv.y = hsv.y;
                    keep_hsv.z = hsv.z;
                }

                Color getColor = Color.HSVToRGB(hsv.x, hsv.y, hsv.z);

                renderer_sv.material.SetColor("_Color", getColor);

                text_sv.text = string.Format("S : {0:F2} / V : {1:F2}", hsv.y, hsv.z);
                //Debug.LogError(ColorUtility.ToHtmlStringRGB(getColor));
                CustomizeManager.GetInstance.characterCtrl.SetColor(color_part, ColorUtility.ToHtmlStringRGB(getColor));
            }
        }
    }

    public int GetPaletteState(Vector3 point)
    {
        Vector3 pos = transform.InverseTransformPoint(point);

        if (pos.x > 0.11f && pos.x < 0.14f)
        {
            return -1;
        }

        if (pos.x > 0.125f) // 색상.
        {
            return 1;
        }
        else // 명도, 채도.
        {
            return 2;
        }

    }

    public void Click_Palette(bool isHue, Vector3 point)
    {
        Vector3 pos = transform.InverseTransformPoint(point);

        pos.z = 0f;

        if (isHue) // 색상.
        {
            pos.x = 0f;
            transform_h.localPosition = pos;

            hsv.x = 1f - Mathf.Clamp01((transform_h.localPosition.y + colorSumVelue) * colorMulVelue);

            transform_h.localPosition = new Vector3(0f, ((1f - hsv.x) / colorMulVelue) - colorSumVelue, 0f);

            if (Mathf.Abs(keep_hsv.x - hsv.x) >= 0.05f && delayTIme_sound <= 0f)
            {
                delayTIme_sound = 0.05f;
                LobbySoundManager.GetInstance.Play(4);
                keep_hsv.x = hsv.x;
            }
            Color getColor = Color.HSVToRGB(hsv.x, hsv.y, hsv.z);

            renderer_h.material.SetColor("_Color", Color.HSVToRGB(hsv.x, 1f, 1f));
            renderer_sv.material.SetColor("_Color", getColor);
            renderer_palette.material.SetFloat("_Hue", hsv.x);
            text_h.text = string.Format("H : {0:F2}", hsv.x);
            //Debug.LogError(ColorUtility.ToHtmlStringRGB(getColor));
            CustomizeManager.GetInstance.characterCtrl.SetColor(color_part, ColorUtility.ToHtmlStringRGB(getColor));

            return;
        }
        else // 명도, 채도.
        {
            transform_sv.localPosition = pos;

            hsv.y = 1f - Mathf.Clamp01((transform_sv.localPosition.x + colorSumVelue) * colorMulVelue);
            hsv.z = Mathf.Clamp01((transform_sv.localPosition.y + colorSumVelue) * colorMulVelue);

            transform_sv.localPosition = new Vector3(((1f - hsv.y) / colorMulVelue) - colorSumVelue, (hsv.z / colorMulVelue) - colorSumVelue, 0f);

            if ((new Vector2(keep_hsv.y, keep_hsv.z) - new Vector2(hsv.y, hsv.z)).sqrMagnitude >= 0.005f && delayTIme_sound <= 0f)
            {
                delayTIme_sound = 0.05f;
                LobbySoundManager.GetInstance.Play(4);
                keep_hsv.y = hsv.y;
                keep_hsv.z = hsv.z;
            }

            Color getColor = Color.HSVToRGB(hsv.x, hsv.y, hsv.z);

            renderer_sv.material.SetColor("_Color", getColor);

            text_sv.text = string.Format("S : {0:F2} / V : {1:F2}", hsv.y, hsv.z);
            //Debug.LogError(ColorUtility.ToHtmlStringRGB(getColor));
            CustomizeManager.GetInstance.characterCtrl.SetColor(color_part, ColorUtility.ToHtmlStringRGB(getColor));

            return;
        }
    }

    public void Click_InputKey(string key)
    {
        LobbySoundManager.GetInstance.Play(3);
        switch (key)
        {
            case "Skin":
                {
                    color_part = ColorUIPart.Skin;
                }
                break;
            case "Eye":
                {
                    color_part = ColorUIPart.Eye;
                }
                break;
            case "Eyebrow":
                {
                    color_part = ColorUIPart.Eyebrow;
                }
                break;
            case "Hair":
                {
                    color_part = ColorUIPart.Hair;
                }
                break;
            case "Upper":
                {
                    color_part = ColorUIPart.Upper;
                }
                break;
            case "Lower":
                {
                    color_part = ColorUIPart.Lower;
                }
                break;
            case "Foot":
                {
                    color_part = ColorUIPart.Foot;
                }
                break;
            case "Pattern":
                {
                    color_part = ColorUIPart.Pattern;
                }
                break;
            case "Acc":
                {
                    color_part = ColorUIPart.Acc;
                }
                break;
        }
        SetSlot();
    }


    public void CreateColorPalette()
    {
        Texture2D tex = new Texture2D(16, 256);

        for (int w = 0; w < tex.width; w++)
        {
            for (int h = 0; h < tex.height; h++)
            {
                tex.SetPixel(w, h, Color.HSVToRGB(1f - (h / (tex.height - 1f)), 1f, 1f));
            }
        }

        System.IO.File.WriteAllBytes("Assets/03. Individual Resources/Lee Jaeheung/Images/Image_ColorPalette_H.png", tex.EncodeToPNG());
    }


}
