using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Appnori.Util
{
    public class BakeTextureManager : MonoBehaviour
    {
        private static Camera cam;
        private static Material mat;

        private void Awake()
        {
            cam = transform.GetComponent<Camera>();
            mat = transform.GetComponent<MeshRenderer>().sharedMaterial;
            gameObject.SetActive(false);
        }

        public static RenderTexture SetBakeTexture(Renderer renderer)
        {
            if (cam == null)
            {
                return null;
            }

            Material mat_origin = renderer.material;
            Material mat_optimize = new Material(Shader.Find("Shader Graphs/CustomizeShader_Opt"));

            Texture2D channelMap = (mat_origin.GetTexture("_ChannelMap") != null ? (Texture2D)mat_origin.GetTexture("_ChannelMap") : null);

            int size = 2;
            if (channelMap != null)
            {
                size = channelMap.width;
            }

            Texture2D patternTex = (mat_origin.GetTexture("_PatternTex") != null ? (Texture2D)mat_origin.GetTexture("_PatternTex") : null);
            Texture2D emissionMap = (mat_origin.GetTexture("_EmissionMap") != null ? (Texture2D)mat_origin.GetTexture("_EmissionMap") : null);
            Texture2D normalMap = (mat_origin.GetTexture("_BumpMap") != null ? (Texture2D)mat_origin.GetTexture("_BumpMap") : null);
            Texture2D specularMap = (mat_origin.GetTexture("_SpecGlossMap") != null ? (Texture2D)mat_origin.GetTexture("_SpecGlossMap") : null);


            mat.shader = Shader.Find("Shader Graphs/BakeTextureShader");
            mat.SetTexture("_ChannelMap", channelMap);
            mat.SetTexture("_PatternTex", patternTex);

            mat.SetColor("_BaseColor", mat_origin.GetColor("_BaseColor"));
            mat.SetColor("_Color_R", mat_origin.GetColor("_Color_R"));
            mat.SetColor("_Color_G", mat_origin.GetColor("_Color_G"));
            mat.SetColor("_Color_B", mat_origin.GetColor("_Color_B"));
            mat.SetColor("_PatternColor", mat_origin.GetColor("_PatternColor"));

            RenderTexture renderTexture = RenderTexture.GetTemporary(
                                size,
                                size,
                                0,
                                RenderTextureFormat.RGB565);

            renderTexture.useMipMap = true;

            cam.targetTexture = renderTexture;
            cam.gameObject.SetActive(true);
            cam.Render();
            cam.gameObject.SetActive(false);
            mat_optimize.SetTexture("_BaseMap", renderTexture);

            mat_optimize.SetTexture("_EmissionMap", emissionMap);
            mat_optimize.SetColor("_EmissionColor", mat_origin.GetColor("_EmissionColor"));

            mat_optimize.SetTexture("_BumpMap", normalMap);
            mat_optimize.SetTexture("_SpecGlossMap", specularMap);

            mat_optimize.SetFloat("_Smoothness", mat_origin.GetFloat("_Smoothness"));
            mat_optimize.SetFloat("_ToneP", mat_origin.GetFloat("_ToneP"));

            renderer.sharedMaterial = mat_optimize;

            return renderTexture;
        }

        public static RenderTexture GetMainAtlasTexture(Texture[] textures, Color color, int size = 2048)
        {
            mat.shader = Shader.Find("Shader Graphs/CharAtlasShader");

            for (int i = 0; i < textures.Length || i < 4; i++)
            {
                mat.SetTexture("_Map0" + i, i < textures.Length ? textures[i] : null);
            }
            mat.SetColor("_Color", color);

            RenderTexture renderTexture = RenderTexture.GetTemporary(
                                size,
                                size,
                                0,
                                RenderTextureFormat.RGB565);

            renderTexture.useMipMap = true;

            cam.targetTexture = renderTexture;
            cam.gameObject.SetActive(true);
            cam.Render();
            cam.gameObject.SetActive(false);

            return renderTexture;
        }

        public static RenderTexture GetOtherAtlasTexture(Texture[] textures, int size = 1024)
        {
            mat.shader = Shader.Find("Shader Graphs/CharAtlasShader");

            for (int i = 0; i < textures.Length || i < 4; i++)
            {
                mat.SetTexture("_Map0" + i, i < textures.Length ? textures[i] : null);
            }
            mat.SetColor("_Color", new Color(0.5f, 0.5f, 1f, 1f));

            RenderTexture renderTexture = RenderTexture.GetTemporary(
                                size,
                                size,
                                0,
                                RenderTextureFormat.RGB565);

            renderTexture.useMipMap = true;
            //renderTexture.wrapMode = TextureWrapMode.Repeat;
            //renderTexture.filterMode = FilterMode.Point;
            cam.targetTexture = renderTexture;
            cam.gameObject.SetActive(true);
            cam.Render();
            cam.gameObject.SetActive(false);

            return renderTexture;
        }
    }
}

    //private static Texture2D CopyTexture(Texture2D texture)
    //{
    //    if (texture == null)
    //    {
    //        return null;
    //    }
    //    // Create a temporary RenderTexture of the same size as the texture
    //    RenderTexture tmp = RenderTexture.GetTemporary(
    //                        texture.width,
    //                        texture.height,
    //                        0,
    //                        RenderTextureFormat.Default,
    //                        RenderTextureReadWrite.Linear);

    //    // Blit the pixels on texture to the RenderTexture
    //    Graphics.Blit(texture, tmp);

    //    //return tmp;

    //    // Backup the currently set RenderTexture
    //    RenderTexture previous = RenderTexture.active;

    //    // Set the current RenderTexture to the temporary one we created
    //    RenderTexture.active = tmp;

    //    // Create a new readable Texture2D to copy the pixels to it
    //    Texture2D myTexture2D = new Texture2D(texture.width, texture.height);

    //    // Copy the pixels from the RenderTexture to the new Texture
    //    myTexture2D.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
    //    myTexture2D.Apply();

    //    // Reset the active RenderTexture
    //    RenderTexture.active = previous;

    //    // Release the temporary RenderTexture
    //    RenderTexture.ReleaseTemporary(tmp);

    //    // "myTexture2D" now has the same pixels from "texture" and it's readable.

    //    return myTexture2D;
    //}
