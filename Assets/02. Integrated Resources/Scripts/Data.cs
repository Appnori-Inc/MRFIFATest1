using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class Data : MonoBehaviour
    {
    }

public class LumpsData<T> : SerializableClass
{
    public T lumps;

    public override void DeSerialize(string data)
    {
        var instance = JsonUtility.FromJson<LumpsData<T>>(data);
        lumps = instance.lumps;
    }

    public override string Serialize()
    {
        return JsonUtility.ToJson(this);
    }
}

public class LumpsData<T1, T2> : SerializableClass
{
    public T1 lumps1;
    public T2 lumps2;

    public override void DeSerialize(string data)
    {
        var instance = JsonUtility.FromJson<LumpsData<T1, T2>>(data);
        lumps1 = instance.lumps1;
        lumps2 = instance.lumps2;
    }

    public override string Serialize()
    {
        return JsonUtility.ToJson(this);
    }
}

public class LumpsData<T1, T2, T3> : SerializableClass
{
    public T1 lumps1;
    public T2 lumps2;
    public T3 lumps3;
    public override void DeSerialize(string data)
    {
        var instance = JsonUtility.FromJson<LumpsData<T1, T2, T3>>(data);
        lumps1 = instance.lumps1;
        lumps2 = instance.lumps2;
        lumps3 = instance.lumps3;
    }

    public override string Serialize()
    {
        return JsonUtility.ToJson(this);
    }
}

public class LumpsData<T1, T2, T3, T4> : SerializableClass
{
    public T1 lumps1;
    public T2 lumps2;
    public T3 lumps3;
    public T4 lumps4;
    public override void DeSerialize(string data)
    {
        var instance = JsonUtility.FromJson<LumpsData<T1, T2, T3, T4>>(data);
        lumps1 = instance.lumps1;
        lumps2 = instance.lumps2;
        lumps3 = instance.lumps3;
        lumps4 = instance.lumps4;
    }

    public override string Serialize()
    {
        return JsonUtility.ToJson(this);
    }
}

public class PinData : SerializableClass
{
    public int playerNum;
    public bool[] active = new bool[10];
    public Vector3[] pos = new Vector3[10];
    public Vector3[] velocity = new Vector3[10];
    public Vector3[] angularVelocity = new Vector3[10];
    public Quaternion[] rot = new Quaternion[10];

    public override void DeSerialize(string data)
    {
        var instance = JsonUtility.FromJson<PinData>(data);
        playerNum = instance.playerNum;
        active = instance.active;
        pos = instance.pos;
        rot = instance.rot;
        velocity = instance.velocity;
        angularVelocity = instance.angularVelocity;
    }

    public override string Serialize()
    {
        return JsonUtility.ToJson(this);
    }
}

public class BallData : SerializableClass
    {
        public Vector3 position;
        public Vector3 velocity;
        public Vector3 angularVelocity;
        public double photonTime;

        public override void DeSerialize(string data)
        {
            var instance = JsonUtility.FromJson<BallData>(data);
            position = instance.position;
            velocity = instance.velocity;
            angularVelocity = instance.angularVelocity;
            photonTime = instance.photonTime;
    }

        public override string Serialize()
        {
            return JsonUtility.ToJson(this);
        }
    }

