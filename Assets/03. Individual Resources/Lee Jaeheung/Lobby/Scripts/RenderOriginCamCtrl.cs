using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderOriginCamCtrl : MonoBehaviour
{
    private Camera cam_renderTex;
    private Transform cam_orizin;
    private Transform cam_overlay;
    public Transform plane;

    private RenderTexture renderTexture;
    private MeshRenderer meshRenderer;
    private bool isInit = false;

    public void Init(Transform _cam_orizin, Transform _cam_overlay)
    {
        cam_renderTex = transform.GetComponent<Camera>();
        renderTexture = cam_renderTex.targetTexture;

        cam_orizin = _cam_orizin;
        cam_overlay = _cam_overlay;

        cam_renderTex.cullingMask = cam_orizin.GetComponent<Camera>().cullingMask;

        plane.gameObject.layer = 31;

        meshRenderer = plane.GetComponent<MeshRenderer>();

        isInit = true;
    }

    public void Init(Transform _cam_orizin)
    {
        cam_orizin = _cam_orizin;

        cam_renderTex.cullingMask = cam_orizin.GetComponent<Camera>().cullingMask;
    }


    // Update is called once per frame
    void LateUpdate()
    {
        if (!isInit || cam_orizin == null || !PublicGameUIManager.GetInstance.IsOverlay())
        {
            return;
        }
        //Debug.LogError("LateUpdate");
        transform.SetPositionAndRotation(cam_orizin.position, cam_orizin.rotation);
        plane.SetPositionAndRotation(cam_overlay.position + cam_overlay.forward * 20f, cam_overlay.rotation);

        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = renderTexture;

        cam_renderTex.Render();

        RenderTexture.active = currentRT;

        Vector3 dir = cam_overlay.localPosition;
        dir.y = 0f;


        meshRenderer.sharedMaterial.SetFloat("_Black", Mathf.Lerp(0.05f, 0f, dir.sqrMagnitude - 3));
    }
}
