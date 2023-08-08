using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Appnori.Util
{
    public class BakeCustomTex : MonoBehaviour
    {
        RenderTexture renderTexture;

        public enum InitState
        {
            Manual, Auto
        }

        public InitState state_init = InitState.Auto;

        private void Start()
        {
            if (state_init == InitState.Auto)
            {
                SetBake();
            }
        }

        public void SetBake()
        {
            renderTexture = BakeTextureManager.SetBakeTexture(transform.GetComponent<Renderer>());
            enabled = false;
        }

        private void OnDestroy()
        {
            if (renderTexture != null)
            {
                RenderTexture.ReleaseTemporary(renderTexture);
            }
        }
    }
}
