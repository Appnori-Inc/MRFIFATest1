using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollCreator : MonoBehaviour
{
    public void CreateRagdoll()
    {
        Transform parentTr = new GameObject(gameObject.name).transform;
        parentTr.SetPositionAndRotation(transform.position, transform.rotation);
        Transform orizin = Instantiate<Transform>(transform, transform.position, transform.rotation, parentTr);
        orizin.name = "Orizin";
        DestroyImmediate(orizin.GetComponent<RagdollCreator>());
        Transform ragdoll = Instantiate<Transform>(transform, transform.position, transform.rotation,parentTr);
        ragdoll.name = "Ragdoll";
        DestroyImmediate(ragdoll.GetComponent<RagdollCreator>());
        DestroyImmediate(ragdoll.GetComponent<Animator>());
        Debug.Log("복제 완료.");
        Renderer[] meshes = ragdoll.GetComponentsInChildren<Renderer>(true);

        for (int i = meshes.Length - 1; i >= 0; i--)
        {
            DestroyImmediate(meshes[i].gameObject);
        }
        Debug.Log("메시 오브젝트 제거.");
        Transform[] childTrs = ragdoll.GetComponentsInChildren<Transform>();
        List<GameObject> destroyGOs = new List<GameObject>();
        for (int i = 0; i < childTrs.Length; i++)
        {
            if (childTrs[i].name.Contains("Footstep") || childTrs[i].name.Contains("Twist")
               || childTrs[i].name.Contains("Clavicle") || childTrs[i].name.Contains("Neck")
               || childTrs[i].name.Contains("Finger") || childTrs[i].name.Contains("HeadNub")
               || (childTrs[i].name.Contains("Spine") && !childTrs[i].name.Contains("Spine2"))
               || childTrs[i].name.Contains("Toe") || childTrs[i].name.Contains("Eye")
               || childTrs[i].name.Equals("Bip001") )
            {
                destroyGOs.Add(childTrs[i].gameObject);
            }
        }

        for (int i = destroyGOs.Count - 1; i >= 0; i--)
        {
            if (destroyGOs[i].name.Equals("Bip001") || destroyGOs[i].name.Contains("Clavicle") || destroyGOs[i].name.Contains("Neck") || destroyGOs[i].name.Contains("Spine"))
            {
                destroyGOs[i].transform.GetChild(0).parent = destroyGOs[i].transform.parent;
            }

            DestroyImmediate(destroyGOs[i]);
        }

        Debug.Log("필요없는 Bone 제거.");
        Transform[] ragdollTrs = ragdoll.GetComponentsInChildren<Transform>();

        for (int i = 0; i < ragdollTrs.Length; i++)
        {
            if (ragdollTrs[i].name.Equals("Ragdoll"))
            {
                continue;
            }
            Rigidbody rb = ragdollTrs[i].gameObject.AddComponent<Rigidbody>();
            rb.drag = 5f;
            rb.angularDrag = 1f;
            if (ragdollTrs[i].name.Contains("Pelvis"))
            {
                rb.mass = 50f;
                //rb.tag = "Enemy_Body";
            }
            else if (ragdollTrs[i].name.Contains("Thigh"))
            {
                rb.mass = 17f;
            }
            else if (ragdollTrs[i].name.Contains("Calf"))
            {
                rb.mass = 13f;
            }
            else if (ragdollTrs[i].name.Contains("Foot"))
            {
                rb.mass = 12f;
            }
            else if (ragdollTrs[i].name.Contains("Spine"))
            {
                rb.mass = 12f;
                //rb.tag = "Enemy_Neck";
            }
            else if (ragdollTrs[i].name.Contains("UpperArm"))
            {
                rb.mass = 0.5f;
                if (ragdollTrs[i].name.Contains(" L "))
                {
                    //rb.tag = "Enemy_ArmL";
                }
                else
                {
                    //rb.tag = "Enemy_ArmR";
                }
            }
            else if (ragdollTrs[i].name.Contains("Forearm"))
            {
                rb.mass = 0.5f;
            }
            else if (ragdollTrs[i].name.Contains("Hand"))
            {
                rb.mass = 0.5f;
            }
            else if (ragdollTrs[i].name.Contains("Head"))
            {
                rb.mass = 5f;
                //rb.tag = "Enemy_Head";
            }
        }
        Debug.Log("리지드바디/태그 추가.");
        for (int i = 0; i < ragdollTrs.Length; i++)
        {
            if (ragdollTrs[i].name.Equals("Ragdoll"))
            {
                continue;
            }
            ConfigurableJoint joint = ragdollTrs[i].gameObject.AddComponent<ConfigurableJoint>();
            joint.axis = Vector3.right;
            joint.secondaryAxis = Vector3.up;
            if (ragdollTrs[i].name.Contains("Pelvis"))
            {
                joint.xMotion = ConfigurableJointMotion.Free;
                joint.yMotion = ConfigurableJointMotion.Free;
                joint.zMotion = ConfigurableJointMotion.Free;

            }
            else
            {
                joint.connectedBody = ragdollTrs[i].parent.GetComponent<Rigidbody>();

                joint.xMotion = ConfigurableJointMotion.Locked;
                joint.yMotion = ConfigurableJointMotion.Locked;
                joint.zMotion = ConfigurableJointMotion.Locked;      
            }       
            joint.angularXMotion = ConfigurableJointMotion.Free;
            joint.angularYMotion = ConfigurableJointMotion.Free;
            joint.angularZMotion = ConfigurableJointMotion.Free;
            joint.rotationDriveMode = RotationDriveMode.Slerp;

            if (ragdollTrs[i].name.Contains("Spine"))
            {
                joint.anchor = Vector3.right * 0.15f;
            }
            else if (ragdollTrs[i].name.Contains("Bip001 Head"))
            {
                joint.anchor = Vector3.right * 0.15f;
            }
        }
        Debug.Log("조인트 추가.");
        for (int i = 0; i < ragdollTrs.Length; i++)
        {
            if (ragdollTrs[i].name.Contains("Pelvis"))
            {
                CapsuleCollider coll = ragdollTrs[i].gameObject.AddComponent<CapsuleCollider>();
                coll.center = new Vector3(-0.13f, 0.01f, 0f);
                coll.radius = 0.13f;
                coll.height = 0.3f;
                coll.direction = 2;
            }
            else if (ragdollTrs[i].name.Contains("Thigh"))
            {
                CapsuleCollider coll = ragdollTrs[i].gameObject.AddComponent<CapsuleCollider>();
                coll.center = new Vector3(-0.35f, 0f, 0f);
                coll.radius = 0.1f;
                coll.height = 0.35f;
                coll.direction = 0;
            }
            else if (ragdollTrs[i].name.Contains("Calf"))
            {
                CapsuleCollider coll = ragdollTrs[i].gameObject.AddComponent<CapsuleCollider>();
                coll.center = new Vector3(-0.2f, 0f, 0f);
                coll.radius = 0.08f;
                coll.height = 0.25f;
                coll.direction = 0;
            }
            else if (ragdollTrs[i].name.Contains("Foot"))
            {
                BoxCollider coll = ragdollTrs[i].gameObject.AddComponent<BoxCollider>();
                coll.center = new Vector3(-0.03f, 0.1f, 0f);
                coll.size = new Vector3(0.1f, 0.3f, 0.1f);
            }
            else if (ragdollTrs[i].name.Contains("Spine"))
            {
                CapsuleCollider coll = ragdollTrs[i].gameObject.AddComponent<CapsuleCollider>();
                coll.center = new Vector3(0.02f, -0.02f, 0f);
                coll.radius = 0.1f;
                coll.height = 0.33f;
                coll.direction = 2;
            }
            else if (ragdollTrs[i].name.Contains("UpperArm"))
            {
                CapsuleCollider coll = ragdollTrs[i].gameObject.AddComponent<CapsuleCollider>();
                coll.center = new Vector3(-0.2f, 0f, 0f);
                coll.radius = 0.05f;
                coll.height = 0.3f;
                coll.direction = 0;
            }
            else if (ragdollTrs[i].name.Contains("Forearm"))
            {
                CapsuleCollider coll = ragdollTrs[i].gameObject.AddComponent<CapsuleCollider>();
                coll.center = new Vector3(-0.15f, 0f, 0f);
                coll.radius = 0.05f;
                coll.height = 0.2f;
                coll.direction = 0;
            }
            else if (ragdollTrs[i].name.Contains("Hand"))
            {
                SphereCollider coll = ragdollTrs[i].gameObject.AddComponent<SphereCollider>();
                coll.center = new Vector3(-0.07f, 0.035f, 0f);
                coll.radius = 0.1f;
            }
            else if (ragdollTrs[i].name.Contains("Head"))
            {
                CapsuleCollider coll = ragdollTrs[i].gameObject.AddComponent<CapsuleCollider>();
                coll.center = new Vector3(-0.05f, 0.05f, 0f);
                coll.radius = 0.1f;
                coll.height = 0.3f;
                coll.direction = 0;
            }
        }
        Debug.Log("콜라이더 추가.");
 
        Transform[] orizinTrs = orizin.GetComponentsInChildren<Transform>();
        BoxerCtrl boxerCtrl = orizin.gameObject.AddComponent<BoxerCtrl>();
        Debug.Log("BoxerCtrl 스크립트 추가.");

        boxerCtrl.bones = new BoxerCtrl.Bone[15];

        string findName;
        findName = "Pelvis";
        boxerCtrl.bones[0].orizin = FindObject(findName, orizinTrs);
        boxerCtrl.bones[0].ragdoll_joint = FindObject(findName, ragdollTrs).GetComponent<ConfigurableJoint>();
        boxerCtrl.bones[0].ragdoll_rb = FindObject(findName, ragdollTrs).GetComponent<Rigidbody>();
        findName = "L Thigh";
        boxerCtrl.bones[1].orizin = FindObject(findName, orizinTrs);
        boxerCtrl.bones[1].ragdoll_joint = FindObject(findName, ragdollTrs).GetComponent<ConfigurableJoint>();
        boxerCtrl.bones[1].ragdoll_rb = FindObject(findName, ragdollTrs).GetComponent<Rigidbody>();
        findName = "L Calf";
        boxerCtrl.bones[2].orizin = FindObject(findName, orizinTrs);
        boxerCtrl.bones[2].ragdoll_joint = FindObject(findName, ragdollTrs).GetComponent<ConfigurableJoint>();
        boxerCtrl.bones[2].ragdoll_rb = FindObject(findName, ragdollTrs).GetComponent<Rigidbody>();
        findName = "L Foot";
        boxerCtrl.bones[3].orizin = FindObject(findName, orizinTrs);
        boxerCtrl.bones[3].ragdoll_joint = FindObject(findName, ragdollTrs).GetComponent<ConfigurableJoint>();
        boxerCtrl.bones[3].ragdoll_rb = FindObject(findName, ragdollTrs).GetComponent<Rigidbody>();
        findName = "R Thigh";
        boxerCtrl.bones[4].orizin = FindObject(findName, orizinTrs);
        boxerCtrl.bones[4].ragdoll_joint = FindObject(findName, ragdollTrs).GetComponent<ConfigurableJoint>();
        boxerCtrl.bones[4].ragdoll_rb = FindObject(findName, ragdollTrs).GetComponent<Rigidbody>();
        findName = "R Calf";
        boxerCtrl.bones[5].orizin = FindObject(findName, orizinTrs);
        boxerCtrl.bones[5].ragdoll_joint = FindObject(findName, ragdollTrs).GetComponent<ConfigurableJoint>();
        boxerCtrl.bones[5].ragdoll_rb = FindObject(findName, ragdollTrs).GetComponent<Rigidbody>();
        findName = "R Foot";
        boxerCtrl.bones[6].orizin = FindObject(findName, orizinTrs);
        boxerCtrl.bones[6].ragdoll_joint = FindObject(findName, ragdollTrs).GetComponent<ConfigurableJoint>();
        boxerCtrl.bones[6].ragdoll_rb = FindObject(findName, ragdollTrs).GetComponent<Rigidbody>();
        findName = "Spine2";
        boxerCtrl.bones[7].orizin = FindObject(findName, orizinTrs);
        boxerCtrl.bones[7].ragdoll_joint = FindObject(findName, ragdollTrs).GetComponent<ConfigurableJoint>();
        boxerCtrl.bones[7].ragdoll_rb = FindObject(findName, ragdollTrs).GetComponent<Rigidbody>();
        findName = "L UpperArm";
        boxerCtrl.bones[8].orizin = FindObject(findName, orizinTrs);
        boxerCtrl.bones[8].ragdoll_joint = FindObject(findName, ragdollTrs).GetComponent<ConfigurableJoint>();
        boxerCtrl.bones[8].ragdoll_rb = FindObject(findName, ragdollTrs).GetComponent<Rigidbody>();
        findName = "L Forearm";
        boxerCtrl.bones[9].orizin = FindObject(findName, orizinTrs);
        boxerCtrl.bones[9].ragdoll_joint = FindObject(findName, ragdollTrs).GetComponent<ConfigurableJoint>();
        boxerCtrl.bones[9].ragdoll_rb = FindObject(findName, ragdollTrs).GetComponent<Rigidbody>();
        findName = "L Hand";
        boxerCtrl.bones[10].orizin = FindObject(findName, orizinTrs);
        boxerCtrl.bones[10].ragdoll_joint = FindObject(findName, ragdollTrs).GetComponent<ConfigurableJoint>();
        boxerCtrl.bones[10].ragdoll_rb = FindObject(findName, ragdollTrs).GetComponent<Rigidbody>();
        findName = "Head";
        boxerCtrl.bones[11].orizin = FindObject(findName, orizinTrs);
        boxerCtrl.bones[11].ragdoll_joint = FindObject(findName, ragdollTrs).GetComponent<ConfigurableJoint>();
        boxerCtrl.bones[11].ragdoll_rb = FindObject(findName, ragdollTrs).GetComponent<Rigidbody>();
        findName = "R UpperArm";
        boxerCtrl.bones[12].orizin = FindObject(findName, orizinTrs);
        boxerCtrl.bones[12].ragdoll_joint = FindObject(findName, ragdollTrs).GetComponent<ConfigurableJoint>();
        boxerCtrl.bones[12].ragdoll_rb = FindObject(findName, ragdollTrs).GetComponent<Rigidbody>();
        findName = "R Forearm";
        boxerCtrl.bones[13].orizin = FindObject(findName, orizinTrs);
        boxerCtrl.bones[13].ragdoll_joint = FindObject(findName, ragdollTrs).GetComponent<ConfigurableJoint>();
        boxerCtrl.bones[13].ragdoll_rb = FindObject(findName, ragdollTrs).GetComponent<Rigidbody>();
        findName = "R Hand";
        boxerCtrl.bones[14].orizin = FindObject(findName, orizinTrs);
        boxerCtrl.bones[14].ragdoll_joint = FindObject(findName, ragdollTrs).GetComponent<ConfigurableJoint>();
        boxerCtrl.bones[14].ragdoll_rb = FindObject(findName, ragdollTrs).GetComponent<Rigidbody>();

        boxerCtrl.spine0Tr = FindObject("Spine", orizinTrs);
        boxerCtrl.spine1Tr = FindObject("Spine1", orizinTrs);
        boxerCtrl.neckTr = FindObject("Neck", orizinTrs);
        boxerCtrl.clavicleLTr = FindObject("L Clavicle", orizinTrs);
        boxerCtrl.clavicleRTr = FindObject("R Clavicle", orizinTrs);
        Debug.Log("BoxerCtrl Bone 등록");
    }

    Transform FindObject(string name, Transform[] transforms)
    {
        for (int i = 0; i < transforms.Length; i++)
        {
            if (transforms[i].name.Contains(name))
            {
                if (name == "Spine")
                {
                    if (transforms[i].name.Contains("1") || transforms[i].name.Contains("2"))
                    {
                        continue;
                    }
                }

                return transforms[i];
            }
        }
        return null;
    }

}
