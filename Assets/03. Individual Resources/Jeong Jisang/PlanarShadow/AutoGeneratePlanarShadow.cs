using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AutoGeneratePlanarShadow : MonoBehaviour
{
    private class Pair
    {
        public Renderer renderer;
        public GameObject Target;


        public Renderer planarRenderer;
        public GameObject planarInstance;

        public bool Initialize(in Renderer target, in Material material, in Transform root)
        {
            Target = target.gameObject;
            renderer = target;

            if (!RendererInstantiate(target, out planarInstance))
                return false;
            
            planarInstance.transform.SetParent(Target.transform, false);

            planarRenderer = planarInstance.GetComponent<Renderer>();
            planarRenderer.material = material;

            return true;
        }


        public void Clear()
        {
            if (planarInstance != null)
                Destroy(planarInstance);
        }
    }

    [SerializeField]
    private Material PlanarShadowMaterial;
    private Material OverrideShadowMaterial;

    [SerializeField]
    private float Height = 0.005f;
    private const float defaultPlanarHeight = 0.005f;

    [SerializeField]
    private bool useAreaControlHeight;
    [SerializeField]
    private Bounds AreaBound;
    [SerializeField]
    private float InnerHeight;
    [SerializeField]
    private float OutterHeight;
    [SerializeField]
    private Light MainLight;


    private readonly List<Pair> pairs = new List<Pair>();

    private void Awake()
    {
#if !UNITY_ANDROID
        return;
#endif
        if(useAreaControlHeight)
        {
            OverrideShadowMaterial = Instantiate(PlanarShadowMaterial);
            Initialize(OverrideShadowMaterial);
        }
        else if (Height != defaultPlanarHeight)
        {
            OverrideShadowMaterial = Instantiate(PlanarShadowMaterial);
            OverrideShadowMaterial.SetFloat("_PlaneHeight", Height);
            Initialize(OverrideShadowMaterial);
        }
        else
        {
            Initialize(PlanarShadowMaterial);
        }

    }

    private void Update()
    {
        if (!useAreaControlHeight)
            return;
        if (pairs == null || pairs.Count <= 0 || pairs.Count > 2)
            return;

        var pair = pairs.First();

        var worldPos = pair.planarInstance.transform.position;
        var lightDirection = MainLight.transform.forward;

        //test
        var opposite = worldPos.y - InnerHeight;
        var cosTheta = -lightDirection.y;
        var hypotenuse = opposite / cosTheta;
        var vPos = worldPos + (lightDirection * hypotenuse);

        var currentHeight = AreaBound.Contains(vPos) ? InnerHeight : OutterHeight;
        OverrideShadowMaterial.SetFloat("_PlaneHeight", currentHeight);
    }

    private static bool RendererInstantiate(in Renderer originRenderer, out GameObject instance)
    {
        if (!originRenderer.enabled || !originRenderer.gameObject.activeInHierarchy)
        {
            instance = null;

            return false;
        }

        instance = new GameObject();
        instance.name = originRenderer.transform.name + " (planarShadow)";

        var instanceRenderer = instance.AddComponent(originRenderer.GetType());
        switch (instanceRenderer)
        {
            case MeshRenderer mesh:
                var originFilter = originRenderer.GetComponent<MeshFilter>();
                if (originFilter.sharedMesh == null || originFilter.mesh == null)
                {
                    Destroy(instance);
                    instance = null;

                    return false;
                }

                var instanceFilter = instance.AddComponent<MeshFilter>();
                instanceFilter.mesh = originFilter.sharedMesh;
                instanceFilter.sharedMesh.bounds = new Bounds(Vector3.zero, Vector3.one * 10);

                mesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                mesh.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                mesh.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
                break;

            case SkinnedMeshRenderer skin:
                var origin = originRenderer as SkinnedMeshRenderer;
                if (origin.sharedMesh == null || origin.rootBone == null)
                {
                    Destroy(instance);
                    instance = null;

                    return false;
                }

                skin.bones = origin.bones;
                skin.sharedMesh = origin.sharedMesh;
                skin.rootBone = origin.rootBone;
                skin.localBounds = new Bounds(Vector3.zero, Vector3.one * 10);

                skin.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                skin.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                skin.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
                break;

            default:
                Destroy(instance);
                instance = null;

                return false;
        }

        return true;
    }

    public void Initialize(in Material shadowMaterial)
    {
        foreach (var pair in pairs)
        {
            pair.Clear();
        }
        pairs.Clear();

        var targetList = GetComponentsInChildren<Renderer>(false);

        foreach (var target in targetList)
        {
            var pair = new Pair();
            if (pair.Initialize(target, shadowMaterial, transform))
                pairs.Add(pair);
        }
    }


}
