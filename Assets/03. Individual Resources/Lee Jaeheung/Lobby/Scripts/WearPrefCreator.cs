using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(WearPrefCreator))]
public class Edit_WearPrefCreator : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        WearPrefCreator wearPrefCreator = (WearPrefCreator)target;
        if (GUILayout.Button("Create"))
        {
            wearPrefCreator.CreateWearPref();
        }
    }
}
#endif

public class WearPrefCreator : MonoBehaviour
{
    public string pref_name;

    public void CreateWearPref()
    {
        List<Transform> list_bone = new List<Transform>();
        Transform prefParent = new GameObject(pref_name).transform;
        Transform boneParent = new GameObject("Bip001").transform;
        boneParent.parent = prefParent;

        Transform boneParent_orizin = transform.parent.Find("Bip001");
        boneParent.localPosition = boneParent_orizin.localPosition;
        boneParent.localRotation = boneParent_orizin.localRotation;

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform prefChild = new GameObject(pref_name + "_" + i).transform;
            prefChild.parent = prefParent;

            SkinnedMeshRenderer skin_orizin = transform.GetChild(i).GetComponent<SkinnedMeshRenderer>();
            SkinnedMeshRenderer skin_copy = prefChild.gameObject.AddComponent<SkinnedMeshRenderer>();

            skin_copy.sharedMesh = skin_orizin.sharedMesh;
            skin_copy.sharedMaterial = skin_orizin.sharedMaterial;

            skin_copy.localBounds = skin_orizin.localBounds;

            if (!list_bone.Exists(bone => bone.name == skin_orizin.rootBone.name))
            {
                Transform boneTr = new GameObject(skin_orizin.rootBone.name).transform;
                boneTr.parent = boneParent;
                boneTr.localPosition = boneParent_orizin.InverseTransformPoint(skin_orizin.rootBone.position);
                boneTr.localRotation = Quaternion.Inverse(boneParent_orizin.rotation) * skin_orizin.rootBone.rotation;
                list_bone.Add(boneTr);
            }

            skin_copy.rootBone = list_bone.Find(bone => bone.name == skin_orizin.rootBone.name);

            Transform[] boneTrs_copy = new Transform[skin_orizin.bones.Length];

            for (int j = 0; j < skin_orizin.bones.Length; j++)
            {
                if (!list_bone.Exists(bone => bone.name == skin_orizin.bones[j].name))
                {
                    Transform boneTr = new GameObject(skin_orizin.bones[j].name).transform;
                    boneTr.parent = boneParent;
                    boneTr.localPosition = boneParent_orizin.InverseTransformPoint(skin_orizin.bones[j].position);
                    boneTr.localRotation = Quaternion.Inverse(boneParent_orizin.rotation) * skin_orizin.bones[j].rotation;
                    list_bone.Add(boneTr);
                }

                boneTrs_copy[j] = list_bone.Find(bone => bone.name == skin_orizin.bones[j].name);
            }

            skin_copy.bones = boneTrs_copy;
        }
    }
}
