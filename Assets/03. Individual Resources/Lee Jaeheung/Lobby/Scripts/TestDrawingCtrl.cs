using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDrawingCtrl : MonoBehaviour
{
    public Transform rayTr;
    public LayerMask layerMask;

    //private Texture2D drawTex;
    private RenderTexture drawTex;

    public Texture2D brushTex;
    public float brushSize = 1f;
    // Start is called before the first frame update
    void Start()
    {

        transform.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        //drawTex = new Texture2D(64,64);
        //drawTex.Apply();
        //transform.GetComponent<MeshRenderer>().material.SetTexture("_BaseMap", drawTex);
        drawTex = new RenderTexture(64, 64, 32);
        Graphics.Blit(Texture2D.whiteTexture, drawTex);
        transform.GetComponent<MeshRenderer>().material.SetTexture("_BaseMap", drawTex);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(rayTr.position, rayTr.forward, out hit, 0.5f, layerMask))
        {
            Debug.LogError(hit.textureCoord.x +"  "+ hit.textureCoord.y);
            DrawTexture(drawTex, (hit.textureCoord.x * 64), (hit.textureCoord.y * 64));
            //drawTex.SetPixel((int)(hit.textureCoord.x * 50), (int)(hit.textureCoord.y * 50), Color.green);
            //drawTex.Apply();
        }
    }

    void DrawTexture(RenderTexture rt, float posX, float posY)
    {
        RenderTexture.active = rt;
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, 64, 64, 0);

        Rect rect = new Rect(posX - brushTex.width / brushSize,
            (rt.height - posY) - brushTex.height / brushSize,
            brushTex.width / (brushSize * 0.5f),
            brushTex.height / (brushSize * 0.5f));

        Graphics.DrawTexture(rect, brushTex);

        GL.PopMatrix();
        RenderTexture.active = null;
    }
}
