using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
using System.Collections.Generic;
using Photon.Pun;
using System.Runtime.InteropServices;
using Photon.Pun.UtilityScripts;
using System.Runtime.Serialization;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using SingletonPunBase;

//for argument serialization. implement serialize / deserialize
[Serializable]
public abstract class SerializableClass
{
    public abstract string Serialize();
    public abstract void DeSerialize(string data);
}

public class RPC_SGT : Singleton<RPC_SGT>
{
    private PhotonView pv;

    private Dictionary<string, object> registeredInstanceDict = new Dictionary<string, object>();

    protected override void Awake()
    {
        base.Awake();
        pv = GetComponent<PhotonView>();
    }


    //[Serializable]
    //public class Data : SerializableClass
    //{
    //    public int valueInt;
    //    public string valueString;
    //    public Vector3 valueVector3;
    //    public Rigidbody valueRigidbody;

    //    public Data SetValueInt(int v) { valueInt = v; return this; }
    //    public Data SetValueString(string v) { valueString = v; return this; }
    //    public Data SetValueVector3(Vector3 v) { valueVector3 = v; return this; }
    //    public Data SetValueRigidbody(Rigidbody v) { valueRigidbody = v; return this; }

    //    public override void DeSerialize(string data)
    //    {
    //        var instance = JsonUtility.FromJson<Data>(data);
    //        valueInt = instance.valueInt;
    //        valueString = instance.valueString;
    //        valueVector3 = instance.valueVector3;
    //        valueRigidbody = instance.valueRigidbody;
    //    }

    //    public override string Serialize()
    //    {
    //        return JsonUtility.ToJson(this);
    //    }
    //}

    public void PreRegisterTarget(Action<SerializableClass> invocationTarget)
    {
        var name = invocationTarget.GetMethodInfo().ReflectedType.ToString();
        var instance = invocationTarget.Target;

        registeredInstanceDict[name] = instance;
    }

    public bool TrySend(Action<SerializableClass> invocationTarget, SerializableClass data, RpcTarget target)
    {
        var packet = CreatePacket(invocationTarget, data);
        if (packet == null)
            return false;

        Send(packet.Serialize(), target);
        return true;
    }

    public PacketInfo<Action<SerializableClass>, SerializableClass> CreatePacket(Action<SerializableClass> invocationTarget, SerializableClass data)
    {
        if (!registeredInstanceDict.ContainsKey(invocationTarget.GetMethodInfo().ReflectedType.ToString()))
        {
            Debug.LogError("invocationTarget is NOT Registered.");
            return null;
        }

        PacketInfo<Action<SerializableClass>, SerializableClass> packet = new PacketInfo<Action<SerializableClass>, SerializableClass>();
        packet.Set(invocationTarget, data);
        return packet;
    }

    public void Send(string data, RpcTarget target)
    {
        //implement send logic instead.

        pv.RPC("Receive", target, data);
        //Receive(data);
    }

    [PunRPC]
    void Receive(string data)
    {
        var received = PacketInfo<Action<SerializableClass>, SerializableClass>.ToInfo(data);
        var action = received.Get((name) => registeredInstanceDict[name]);
        action.Invoke(received.argsData);
    }

}

[Serializable]
public class PacketInfo<T, Args>
    where T : Delegate
    where Args : SerializableClass
{
    //action info
    public string typeName;
    public string assem;
    public string functionName;

    //args info
    public string ArgsType;
    public string ArgsAssem;
    public string serializedArgs;

    [NonSerialized]
    public Args argsData;

    public void Set(T caller, Args data)
    {
        functionName = caller.GetMethodInfo().Name;
        assem = caller.GetMethodInfo().ReflectedType.Assembly.ToString();
        typeName = caller.GetMethodInfo().ReflectedType.ToString();

        ArgsAssem = data.GetType().Assembly.ToString();
        ArgsType = data.GetType().FullName;
        argsData = data;
    }

    public string Serialize()
    {
        serializedArgs = argsData.Serialize();
        return JsonUtility.ToJson(this);
    }

    public static PacketInfo<T, Args> ToInfo(string json)
    {
        var info = JsonUtility.FromJson<PacketInfo<T, Args>>(json);
        Type argsType = Type.GetType($"{info.ArgsType}, {info.ArgsAssem}");
        info.argsData = Activator.CreateInstance(argsType) as Args;
        info.argsData.DeSerialize(info.serializedArgs);
        return info;
    }

    public T Get(Func<string, object> predicate)
    {
        Type currentType = Type.GetType($"{typeName}, {assem}");

        var methodInfo = currentType.GetMethod(functionName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        var func = (T)Delegate.CreateDelegate(typeof(T), predicate(typeName), methodInfo);

        return func;
    }
}

    //private PhotonView pv;

    //protected override void Awake()
    //{
    //    base.Awake();
    //    pv = GetComponent<PhotonView>();
    //}

    //public void SendRPC(Action action, RpcTarget target)
    //{
    //    string str = "안녕하세요";
    //    Debug.Log(str);
    //    pv.RPC("RPC", target, str);
    //}

    //[PunRPC]
    //public void RPC(string info)
    //{
    //    Debug.Log(info);
    //    //DLG();
    //}