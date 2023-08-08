using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SingletonPunBase;

public class RootPhysics : Singleton<RootPhysics>
{
    public struct PosTime
    {
        public Vector3 position;
        public float time;
    }

    public static Vector3 GetVelocity(Vector3 Departure, Vector3 Arrival, float time, bool isGravity = true)
    {
        Vector3 Velocity = (Arrival - Departure) / time;

        if (isGravity)
            Velocity.y -= Physics.gravity.y * time * 0.5f;

        return Velocity;
    }

    public static Vector3 GetVelocity(Vector3 Departure, Vector3 Arrival, Vector3 force, float drag, float time)
    {
        Vector3 Velocity = (Arrival - Departure) / time;

        Velocity.y -= (force.y) * time * 0.5f;
        //Velocity.x += force.x * time * 0.5f;

        Velocity *= (1f + drag * time);

        return Velocity;
    }

    public static Vector3 GetPreviewHitPos(Rigidbody rigid, ConstantForce constatForce = null, string hitColliderTag = "Untagged", bool isUpToDouwn = true)
    {
        List<Vector3> list = new List<Vector3>();
         
        float time = 0.0f;
        bool isRunning = true;
 
        Vector3 pos = rigid.position;
        float timestep = Time.fixedDeltaTime * Physics.defaultSolverVelocityIterations;
        Vector3 gravityAccel = Physics.gravity * Mathf.Pow(timestep , 2);
        Vector3 addforceAccel = new Vector3();
        if (constatForce != null)
        {
            addforceAccel = (constatForce.force / rigid.mass) * Mathf.Pow(timestep, 2);
        }

        float drag = 1f - timestep * rigid.drag; 
        Vector3 moveStep = rigid.velocity * timestep;

        while (isRunning)
        {
            time += Time.fixedDeltaTime;

            if (constatForce != null)
                moveStep += addforceAccel;
            moveStep += gravityAccel;
            moveStep *= drag;
            pos += moveStep;

            list.Add(pos);

            if (list.Count < 2)
                continue;

            Vector3 origin = list[list.Count - 2];
            Vector3 direction = list[list.Count - 1] - origin;

            //Debug.DrawRay(origin, direction, Color.red, 10);
            //Instantiate(Resources.Load<GameObject>("RedSphere"), origin, Quaternion.identity);
            RaycastHit hit;

            if (Physics.Raycast(origin, direction, out hit, direction.magnitude))
            {
                if(hit.collider.CompareTag(hitColliderTag))
                {
                    if (isUpToDouwn)
                    {
                        if (direction.y < 0)
                            return hit.point;
                    }
                    else
                        return hit.point;
                }
            }
            if (list.Count > 200)
                break;
        }
        return Vector3.zero;
    }

    public static PosTime GetPreviewHitPosTime(Rigidbody rigid, ConstantForce constatForce = null, string hitColliderTag = "Untagged", bool isUpToDouwn = true)
    {
        List<Vector3> list = new List<Vector3>();

        PosTime posTime = new PosTime();
        float time = 0.0f;
        bool isRunning = true;

        Vector3 pos = rigid.position;
        float timestep = Time.fixedDeltaTime * Physics.defaultSolverVelocityIterations;
        Vector3 gravityAccel = Physics.gravity * Mathf.Pow(timestep, 2);
        Vector3 addforceAccel = new Vector3();
        if (constatForce != null)
        {
            addforceAccel = (constatForce.force / rigid.mass) * Mathf.Pow(timestep, 2);
        }

        float drag = 1f - timestep * rigid.drag;
        Vector3 moveStep = rigid.velocity * timestep;

        while (isRunning)
        {
            time += Time.fixedDeltaTime;

            if (constatForce != null)
                moveStep += addforceAccel;
            moveStep += gravityAccel;
            moveStep *= drag;
            pos += moveStep;

            list.Add(pos);

            if (list.Count < 2)
                continue;

            Vector3 origin = list[list.Count - 2];
            Vector3 direction = list[list.Count - 1] - origin;

            //Debug.DrawRay(origin, direction, Color.red, 10);
            //Instantiate(Resources.Load<GameObject>("RedSphere"), origin, Quaternion.identity);
            RaycastHit hit;

            if (Physics.Raycast(origin, direction, out hit, direction.magnitude))
            {
                if (hit.collider.CompareTag(hitColliderTag))
                {
                    if (isUpToDouwn)
                    {
                        if (direction.y < 0)
                        {
                            posTime.position = hit.point;
                            posTime.time = time;
                            //Instantiate(Resources.Load<GameObject>("BlueSphere"), hit.point, Quaternion.identity);
                            return posTime;
                        }

                    }
                    else
                    {
                        posTime.position = hit.point;
                        posTime.time = time;
                        return posTime;
                    }
                }
            }
            if (list.Count > 200)
                break;
        }

        posTime.position = Vector3.zero;
        posTime.time = 0.0f;
        return posTime;
    }


    // proper following duration is larger then 0.02 second, depends on the update rate
    public static void SetRigidbodyVelocity(Rigidbody rigidbody, Vector3 from, Vector3 to, float duration)
    {
        var diffPos = to - from;
        if (Mathf.Approximately(diffPos.sqrMagnitude, 0f))
        {
            rigidbody.velocity = Vector3.zero;
        }
        else
        {
            rigidbody.velocity = diffPos / duration;
        }
    }

    // proper folloing duration is larger then 0.02 second, depends on the update rate
    public static void SetRigidbodyAngularVelocity(Rigidbody rigidbody, Quaternion from, Quaternion to, float duration, bool overrideMaxAngularVelocity = true)
    {
        float angle;
        Vector3 axis;
        (to * Quaternion.Inverse(from)).ToAngleAxis(out angle, out axis);
        while (angle > 180f) { angle -= 360f; }

        if (Mathf.Approximately(angle, 0f) || float.IsNaN(axis.x) || float.IsNaN(axis.y) || float.IsNaN(axis.z))
        {
            rigidbody.angularVelocity = Vector3.zero;
        }
        else
        {
            angle *= Mathf.Deg2Rad / duration; // convert to radius speed
            if (overrideMaxAngularVelocity && rigidbody.maxAngularVelocity < angle) { rigidbody.maxAngularVelocity = angle; }
            rigidbody.angularVelocity = axis * angle;
        }
    }
    public static Vector3 SetAngularVelocity(Quaternion from, Quaternion to, float duration, bool overrideMaxAngularVelocity = true)
    {
        float angle;
        Vector3 axis;
        (to * Quaternion.Inverse(from)).ToAngleAxis(out angle, out axis);
        while (angle > 180f) { angle -= 360f; }

        if (Mathf.Approximately(angle, 0f) || float.IsNaN(axis.x) || float.IsNaN(axis.y) || float.IsNaN(axis.z))
        {
            return Vector3.zero;
        }
        else
        {
            angle *= Mathf.Deg2Rad / duration; // convert to radius speed
            return axis * angle;
        }
    }

    public struct Pose
    {
        public float time;
        public RigidPose pose;
    }
    private System.Collections.Generic.Queue<Pose> m_poseSamples = new Queue<Pose>();
    public void RecordLatestPosesForDrop(Transform currentBall, float currentTime, float recordLength)
    {
        while (m_poseSamples.Count > 0 && (currentTime - m_poseSamples.Peek().time) > recordLength)
        {
            m_poseSamples.Dequeue();
        }

        m_poseSamples.Enqueue(new Pose()
        {
            time = currentTime,
            pose = new RigidPose(currentBall),
        });
    }

    public Pose GetPoseQueue()
    {
        return m_poseSamples.Dequeue();
    }

    public void ClearPoseQueue()
    {
        m_poseSamples.Clear();
    }


}
