using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BoxerCtrl : MonoBehaviour
{
    //TEstsststst
    public static BoxerCtrl instance;

    [System.Serializable]
    public struct Bone
    {
        public Transform orizin;
        public ConfigurableJoint ragdoll_joint;
        public Rigidbody ragdoll_rb;
        public int connect_num;
        public Quaternion startQ;
    }

    public Bone[] bones;
    private Animator anim;

    public Transform spine0Tr;
    public Transform spine1Tr;
    public Transform neckTr;
    public Transform clavicleLTr;
    public Transform clavicleRTr;

    public Transform eyeL;
    public Transform eyeR;
    //public Rigidbody pelvisRB;

    private Quaternion[] recoveryQ;

    private Vector3 recoveryP;

    private Quaternion headForce;
    private Vector3 bodyForce;

    public AnimationCurve bodyDamageCurve;
    private float bodyDamageTime;

    private float physicsWeight;
    private float physicsWeight_armL;
    private float physicsWeight_armR;
    public AnimationCurve damageRecoveryCurve;

    private bool isUpdateBone = false;

    private enum State
    {
        Idle, Attack, Guard
    }

    public bool isGuard;

    private float eyeHeight = 1.68f;

    void Awake()
    {
        if (instance == null)
            instance = this;
        if (instance != this)
            Destroy(this.gameObject);
    }

    void Start()
    {
        anim = transform.GetComponent<Animator>();
        JointDrive jointDrive = new JointDrive();
        jointDrive.positionSpring = 500f;
        jointDrive.positionDamper = 10f;
        jointDrive.maximumForce = 500f;

        for (int i = 0; i < bones.Length; i++)
        {
            bones[i].ragdoll_joint.slerpDrive = jointDrive;
            bones[i].ragdoll_joint.autoConfigureConnectedAnchor = false;
            //bones[i].ragdoll.GetComponent<Collider>().isTrigger = true;

            if (bones[i].ragdoll_joint.connectedBody == null)
            {
                bones[i].connect_num = -1;
                continue;
            }

            for (int j = 0; j < i; j++)
            {
                if (bones[i].ragdoll_joint.connectedBody.name == bones[j].orizin.name)
                {
                    bones[i].connect_num = j;
                    Quaternion q = Quaternion.Inverse(bones[bones[i].connect_num].orizin.transform.rotation) * bones[i].orizin.rotation;
                    bones[i].startQ = q;
                    break;
                }
            }
        }

        recoveryQ = new Quaternion[bones.Length];

        MeshRenderer[] renderer = bones[0].ragdoll_rb.transform.parent.GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < renderer.Length; i++)
        {
            renderer[i].enabled = false;
        }
        StartCoroutine(UpdateBoneRecovery());
        //StartCoroutine(LateFixedUpdate());

        //InvokeRepeating("DelayForce1_Head", 1f, 8f);
        //InvokeRepeating("DelayForce2_Head", 2f, 8f);
        //InvokeRepeating("DelayForce3_Head", 3f, 8f);
        //InvokeRepeating("DelayForce5_Head", 4f, 8f);
        //InvokeRepeating("DelayForce1_Body", 5f, 8f);
        //InvokeRepeating("DelayForce2_Body", 6f, 8f);
        //InvokeRepeating("DelayForce3_Body", 7f, 8f);

        //InvokeRepeating("DelayForce_Head", 0.25f, 0.5f);
        //InvokeRepeating("DelayForce_Head", 0.5f, 0.5f);
        //InvokeRepeating("DelayForce_Head", 1f, 1f);

    }

    // stateNum.
    // 0 - head.
    // 1 - body.
    public void SetDamage(int stateNum, Vector3 veloc)
    {
        if (!isGuard)
        {
            physicsWeight_armL = 0f;
            physicsWeight_armR = 0f;
        }
        Vector3 force = veloc;
        if (force.magnitude > 5f)
        {
            force = force.normalized * 5f;
        }
        //Debug.Log(force.magnitude);
        switch (stateNum)
        {
            case 0: // head.
                {
                    bones[11].ragdoll_rb.velocity = force * 3.5f;

                    force = transform.InverseTransformDirection(-force);

                    force.x *= 35f;

                    force.z += 0.0001f;
                    if (Mathf.Abs(force.y) > Mathf.Abs(force.z))
                    {
                        force.z = -force.y;
                    }
                    force.z *= Mathf.Lerp(10f, 20f, Mathf.Abs(force.y) / (Mathf.Abs(force.y) + Mathf.Abs(force.z)));

                    headForce = Quaternion.Euler(force);
                    physicsWeight = 1f;
                }
                break;
            case 1: // body.
                {
                    bones[7].ragdoll_rb.velocity = -force * 1f;
                    bones[11].ragdoll_rb.velocity = -force * 1f;

                    bodyForce = (force * 0.01f) * 5f;
                    bodyDamageTime = 0f;
                    physicsWeight = 1f;
                }
                break;
            case 2: // neck.
                {

                    bones[7].ragdoll_rb.velocity = force * 3f;
                    bones[11].ragdoll_rb.velocity = -force * 2f;

                    physicsWeight = 1f;
                }
                break;
            case 3: // armL.
                {
                    bones[8].ragdoll_rb.velocity = force * 1f;
                    bones[11].ragdoll_rb.velocity = -force * 1f;

                    physicsWeight = 1f;
                }
                break;
            case 4: // armR.
                {
                    bones[12].ragdoll_rb.velocity = force * 1f;
                    bones[11].ragdoll_rb.velocity = -force * 1f;

                    physicsWeight = 1f;
                }
                break;
            case 5: // guardUp.
                {

                    bones[7].ragdoll_rb.velocity = force * 3f;

                    physicsWeight = 1f;
                }
                break;
            case 6: // guardDown.
                {
                    bones[7].ragdoll_rb.velocity = -force * 1f;
                    bones[11].ragdoll_rb.velocity = -force * 1f;

                    bodyForce = (force * 0.01f) * 3f;
                    bodyDamageTime = 0f;
                    physicsWeight = 1f;
                }
                break;
        }
    }

    public void SetArmWeight(bool isRight)
    {
        if (isRight)
        {
            physicsWeight_armR = 1f;
        }
        else
        {
            physicsWeight_armL = 1f;
        }
    }

    void DelayForce_Head()
    {
        SetDamage(0, Vector3.right * 5f);
        //DelayForce1_Body();
    }
    void DelayForce1_Head()
    {
        bones[11].ragdoll_rb.velocity = -transform.forward * 15f;
        headForce = Quaternion.Euler(Vector3.forward * 50f);
        physicsWeight = 1f;
    }

    void DelayForce2_Head()
    {
        bones[11].ragdoll_rb.velocity = -transform.right * 15f;
        headForce = Quaternion.Euler(Vector3.right * 150f);
        physicsWeight = 1f;
    }

    void DelayForce3_Head()
    {
        bones[11].ragdoll_rb.velocity = transform.right * 15f;
        headForce = Quaternion.Euler(-Vector3.right * 120f);
        physicsWeight = 1f;
    }

    void DelayForce5_Head()
    {
        bones[7].ragdoll_rb.velocity = ((transform.up * 2f) + (-transform.forward * 1f)).normalized * 5f;
        bones[11].ragdoll_rb.velocity = transform.up * 10f;
        headForce = Quaternion.Euler(Vector3.forward * 100f);
        physicsWeight = 1f;
    }

    void DelayForce1_Body()
    {
        bones[7].ragdoll_rb.velocity = transform.forward * 5f;
        bones[11].ragdoll_rb.velocity = transform.forward * 5f;
        bodyForce = ((-transform.forward * 0.01f) + (-transform.up * 0.01f)) * 15f;
        bodyDamageTime = 0f;
        physicsWeight = 1f;
    }

    void DelayForce2_Body()
    {
        bones[7].ragdoll_rb.velocity = transform.right * 5f;
        bones[11].ragdoll_rb.velocity = transform.right * 5f;
        bodyForce = ((-transform.right * 0.01f) + (-transform.up * 0.01f)) * 15f;
        bodyDamageTime = 0f;
        physicsWeight = 1f;
    }

    void DelayForce3_Body()
    {
        bones[7].ragdoll_rb.velocity = -transform.right * 5f;
        bones[11].ragdoll_rb.velocity = -transform.right * 5f;
        bodyForce = ((transform.right * 0.01f) + (-transform.up * 0.01f)) * 15f;
        bodyDamageTime = 0f;
        physicsWeight = 1f;
    }

    public Transform lookPoint_eye;
    public Transform lookPoint_head;
    public Transform lookPoint_body;


    void FixedUpdate()
    {
        //lookPoint_head.position = Vector3.Lerp(lookPoint_head.position, lookPoint_eye.position, Time.deltaTime * 5f);
        //lookPoint_body.position = Vector3.Lerp(lookPoint_body.position, lookPoint_head.position, Time.deltaTime * 5f);

        //Vector3 playerDir = lookPoint_body.position - transform.position;
        //playerDir.y = 0f;
        //transform.rotation = Quaternion.LookRotation(playerDir);

        //Vector3 forVec = lookPoint_body.position - transform.position;
        //forVec.y = 0f;
        //Vector3 lookVec = lookPoint_body.position - (transform.position + Vector3.up * eyeHeight);

        //float angleQ = Vector3.Angle(forVec.normalized, lookVec.normalized);
        //if (lookPoint_body.position.y >= eyeHeight)
        //{
        //    angleQ = -angleQ;
        //}

        //float dist = forVec.magnitude;

        //if (angleQ >= 0f)
        //{
        //    angleQ -= Mathf.Lerp(50f, 0f, dist) - Mathf.Lerp(0f, 10f, angleQ * 0.02f);
        //}
        //else
        //{
        //    angleQ -= Mathf.Lerp(50f, 0f, dist) - Mathf.Lerp(0f, 50f, Math.Abs(angleQ) * 0.02f);
        //}

        //spine0Tr.Rotate(transform.right * angleQ * 0.4f, Space.World);
        //spine1Tr.Rotate(transform.right * angleQ * 0.4f, Space.World);
        //bones[7].orizin.Rotate(transform.right * angleQ * 0.2f, Space.World);


        //playerDir = lookPoint_head.position - transform.position;
        //playerDir.y = 0f;
        //Vector3 headFor = bones[11].orizin.up;
        //headFor.y = 0f;
        //bones[11].orizin.Rotate(transform.right * -20f + transform.up * Vector3.Angle(headFor.normalized, playerDir.normalized), Space.World);

        //......
        bones[0].ragdoll_rb.velocity = Vector3.zero;
        bones[0].ragdoll_rb.angularVelocity = Vector3.zero;

        headForce = Quaternion.Lerp(headForce, Quaternion.identity, Time.fixedDeltaTime * 5f);

        bodyDamageTime = Mathf.Clamp01(bodyDamageTime + Time.fixedDeltaTime * 1.5f);

        Vector3 bodyMovePos = Vector3.Lerp(Vector3.zero, bodyForce, bodyDamageCurve.Evaluate(bodyDamageTime));

        bones[0].ragdoll_rb.MovePosition(bones[0].orizin.position + bodyMovePos);
        bones[0].ragdoll_rb.MoveRotation(bones[0].orizin.rotation);

        physicsWeight = Mathf.Clamp01(physicsWeight - Time.fixedDeltaTime * 0.5f);
        physicsWeight_armL = Mathf.Clamp01(physicsWeight_armL - Time.fixedDeltaTime * 0.5f);
        physicsWeight_armR = Mathf.Clamp01(physicsWeight_armR - Time.fixedDeltaTime * 0.5f);


        Quaternion q = Quaternion.identity;

        for (int i = 1; i < bones.Length; i++)
        {
            if (i == 8 || i == 9 || i == 10)
            {
                q = Quaternion.Lerp(bones[i].orizin.transform.rotation, bones[i].ragdoll_rb.transform.rotation, damageRecoveryCurve.Evaluate(physicsWeight_armL));
            }
            else if (i == 12 || i == 13 || i == 14)
            {
                q = Quaternion.Lerp(bones[i].orizin.transform.rotation, bones[i].ragdoll_rb.transform.rotation, damageRecoveryCurve.Evaluate(physicsWeight_armR));
            }
            else
            {
                q = Quaternion.Lerp(bones[i].orizin.transform.rotation, bones[i].ragdoll_rb.transform.rotation, damageRecoveryCurve.Evaluate(physicsWeight));
            }

            bones[i].ragdoll_rb.MoveRotation(q);

            if (i == 7) // Spine2.
            {
                // set Anchor.
                bones[i].ragdoll_joint.anchor = bones[i].orizin.InverseTransformPoint(spine1Tr.position);

                // set Connected Anchor.
                bones[i].ragdoll_joint.connectedAnchor = bones[0].orizin.InverseTransformPoint(spine1Tr.position);
            }
            else if (i == 8) // UpperArm L.
            {
                // set Anchor.
                bones[i].ragdoll_joint.anchor = bones[i].orizin.InverseTransformPoint(clavicleLTr.position);

                // set Connected Anchor.
                bones[i].ragdoll_joint.connectedAnchor = bones[7].orizin.InverseTransformPoint(clavicleLTr.position);
            }
            else if (i == 11) // Head.
            {
                // set Anchor.
                bones[i].ragdoll_joint.anchor = bones[i].orizin.InverseTransformPoint(neckTr.position);

                // set Connected Anchor.
                bones[i].ragdoll_joint.connectedAnchor = bones[7].orizin.InverseTransformPoint(neckTr.position);
            }
            else if (i == 12) // UpperArm R.
            {
                // set Anchor.
                bones[i].ragdoll_joint.anchor = bones[i].orizin.InverseTransformPoint(clavicleRTr.position);

                // set Connected Anchor.
                bones[i].ragdoll_joint.connectedAnchor = bones[7].orizin.InverseTransformPoint(clavicleRTr.position);
            }
            else
            {
                // set Connected Anchor.
                bones[i].ragdoll_joint.connectedAnchor = bones[i].orizin.parent.InverseTransformPoint(bones[i].orizin.transform.position);
            }

            if (i == 11)
            {
                q = (Quaternion.Inverse(bones[bones[i].connect_num].orizin.transform.rotation) * bones[i].orizin.rotation) * headForce;
                bones[i].ragdoll_joint.targetRotation = Quaternion.Inverse(q) * bones[i].startQ;

            }
            else
            {
                q = Quaternion.Inverse(bones[bones[i].connect_num].orizin.transform.rotation) * bones[i].orizin.rotation;
                bones[i].ragdoll_joint.targetRotation = Quaternion.Inverse(q) * bones[i].startQ;
            }
        }
    }

    private void Update()
    {
        isUpdateBone = true;

        Vector3 start_orizin_pos = bones[1].orizin.position;
        Vector3 forword_orizin_pos = bones[2].orizin.position;
        Vector3 end_orizin_pos = bones[3].orizin.position;

        Vector3 start_ragdoll_pos_L = bones[0].ragdoll_rb.transform.position + (start_orizin_pos - bones[0].orizin.position);

        Vector3 leg_orizin_dir = end_orizin_pos - start_orizin_pos;
        float leg_max_dist = (forword_orizin_pos - start_orizin_pos).magnitude + (end_orizin_pos - forword_orizin_pos).magnitude;
        float start2mid_ratio = (forword_orizin_pos - start_orizin_pos).magnitude / leg_max_dist;

        Vector3 mid_orizin_pos = start_orizin_pos + (leg_orizin_dir * start2mid_ratio);
        Vector3 start2mid_dist = mid_orizin_pos - start_orizin_pos;
        Vector3 start2for_dist = forword_orizin_pos - start_orizin_pos;
        float mid2for_dist = Mathf.Sqrt(start2for_dist.sqrMagnitude - start2mid_dist.sqrMagnitude);

        Vector3 mid2for_dir_L = (forword_orizin_pos - mid_orizin_pos).normalized;



        Vector3 ik_leg_dir_L = end_orizin_pos - start_ragdoll_pos_L;

        if (ik_leg_dir_L.magnitude > leg_max_dist)
        {
            ik_leg_dir_L = ik_leg_dir_L.normalized * leg_max_dist;
        }

        Vector3 ik_mid_pos = start_ragdoll_pos_L + (ik_leg_dir_L * start2mid_ratio);


        Vector3 ik_start2mid_dist = mid_orizin_pos - start_ragdoll_pos_L;

        float ik_mid2for_dist = start2for_dist.sqrMagnitude - ik_start2mid_dist.sqrMagnitude + 0.01f;
        if (ik_mid2for_dist > 0f)
        {
            ik_mid2for_dist = Mathf.Sqrt(ik_mid2for_dist);
        }
        else
        {
            ik_mid2for_dist = 0f;
        }

        Vector3 ik_for_pos_L = ik_mid_pos + (mid2for_dir_L * ik_mid2for_dist);

        Vector3 ik_end_pos_L = end_orizin_pos;

        Quaternion ik_foot_rot_L = bones[3].orizin.rotation;

        /////////////////////////////////////

        start_orizin_pos = bones[4].orizin.position;
        forword_orizin_pos = bones[5].orizin.position;
        end_orizin_pos = bones[6].orizin.position;

        Vector3 start_ragdoll_pos_R = bones[0].ragdoll_rb.transform.position + (start_orizin_pos - bones[0].orizin.position);

        leg_orizin_dir = end_orizin_pos - start_orizin_pos;
        leg_max_dist = (forword_orizin_pos - start_orizin_pos).magnitude + (end_orizin_pos - forword_orizin_pos).magnitude;
        start2mid_ratio = (forword_orizin_pos - start_orizin_pos).magnitude / leg_max_dist;

        mid_orizin_pos = start_orizin_pos + (leg_orizin_dir * start2mid_ratio);
        start2mid_dist = mid_orizin_pos - start_orizin_pos;
        start2for_dist = forword_orizin_pos - start_orizin_pos;
        mid2for_dist = Mathf.Sqrt(start2for_dist.sqrMagnitude - start2mid_dist.sqrMagnitude);

        Vector3 mid2for_dir_R = (forword_orizin_pos - mid_orizin_pos).normalized;



        Vector3 ik_leg_dir_R = end_orizin_pos - start_ragdoll_pos_R;

        if (ik_leg_dir_R.magnitude > leg_max_dist)
        {
            ik_leg_dir_R = ik_leg_dir_R.normalized * leg_max_dist;
        }

        ik_mid_pos = start_ragdoll_pos_R + (ik_leg_dir_R * start2mid_ratio);

        ik_start2mid_dist = mid_orizin_pos - start_ragdoll_pos_R;

        ik_mid2for_dist = start2for_dist.sqrMagnitude - ik_start2mid_dist.sqrMagnitude + 0.01f;
        if (ik_mid2for_dist > 0f)
        {
            ik_mid2for_dist = Mathf.Sqrt(ik_mid2for_dist);
        }
        else
        {
            ik_mid2for_dist = 0f;
        }

        Vector3 ik_for_pos_R = ik_mid_pos + (mid2for_dir_R * ik_mid2for_dist);

        Vector3 ik_end_pos_R = end_orizin_pos;

        Quaternion ik_foot_rot_R = bones[6].orizin.rotation;

        //////////////////////////////////////////

        recoveryP = bones[0].orizin.localPosition;
        recoveryQ[0] = bones[0].orizin.localRotation;

        bones[0].orizin.position = bones[0].ragdoll_rb.transform.position;
        bones[0].orizin.rotation = bones[0].ragdoll_rb.transform.rotation;
        for (int i = 1; i < bones.Length; i++)
        {
            if (i == 7)
            {
                recoveryQ[i] = spine1Tr.localRotation;
                spine1Tr.rotation = bones[i].ragdoll_rb.transform.rotation;
            }
            else
            {
                recoveryQ[i] = bones[i].orizin.localRotation;
                bones[i].orizin.rotation = bones[i].ragdoll_rb.transform.rotation;
            }
        }
        Vector3 look_for = Vector3.Cross(-ik_leg_dir_L.normalized, mid2for_dir_L);
        bones[1].orizin.rotation = Quaternion.LookRotation(look_for, Vector3.Cross(look_for, -(ik_for_pos_L - start_ragdoll_pos_L).normalized));
        bones[2].orizin.rotation = Quaternion.LookRotation(look_for, Vector3.Cross(look_for, -(ik_end_pos_L - ik_for_pos_L).normalized));
        bones[3].orizin.rotation = ik_foot_rot_L;

        look_for = Vector3.Cross(-ik_leg_dir_R.normalized, mid2for_dir_R);
        bones[4].orizin.rotation = Quaternion.LookRotation(look_for, Vector3.Cross(look_for, -(ik_for_pos_R - start_ragdoll_pos_R).normalized));
        bones[5].orizin.rotation = Quaternion.LookRotation(look_for, Vector3.Cross(look_for, -(ik_end_pos_R - ik_for_pos_R).normalized));
        bones[6].orizin.rotation = ik_foot_rot_R;

        //////////////////////
        ///

        if (Vector3.Dot(bones[11].orizin.up, (lookPoint_eye.position - eyeL.position).normalized) >= 0.71f && Vector3.Dot(bones[11].orizin.up, (lookPoint_eye.position - eyeR.position).normalized) >= 0.71f)
        {
            eyeL.rotation = Quaternion.Slerp(eyeL.rotation, Quaternion.LookRotation(lookPoint_eye.position - eyeL.position), Time.deltaTime * 10f);
            eyeR.rotation = Quaternion.Slerp(eyeR.rotation, Quaternion.LookRotation(lookPoint_eye.position - eyeR.position), Time.deltaTime * 10f);
        }
    }

    //private void OnGUI()
    IEnumerator UpdateBoneRecovery()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();

            if (!isUpdateBone)
            {
                continue;
            }
            isUpdateBone = false;
            //Debug.Log("OnRenderImage");
            bones[0].orizin.localPosition = recoveryP;
            bones[0].orizin.localRotation = recoveryQ[0];
            for (int i = 1; i < bones.Length; i++)
            {
                if (i == 7)
                {
                    spine1Tr.localRotation = recoveryQ[i];
                }
                else
                {
                    bones[i].orizin.localRotation = recoveryQ[i];
                }
            }
        }
    }

    public void SetGuard(bool isGuard)
    {
        this.isGuard = isGuard;
        anim.SetBool("isGuard", isGuard);
    }

    public void SetAttack()
    {
        anim.SetTrigger("onAttack");
    }
}