using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Billiards
{
    //for argument serialization. implement serialize / deserialize
    [Serializable]
    public abstract class SerializableClass
    {
        public abstract string Serialize();
        public abstract void DeSerialize(string data);
    }

    [Serializable]
    public class Data : SerializableClass
    {
        public int value;
        public string value2;

        public Data SetValue(int v) { value = v; return this; }
        public Data SetValue2(string v) { value2 = v; return this; }

        public override void DeSerialize(string data)
        {
            var instance = JsonUtility.FromJson<Data>(data);
            value = instance.value;
            value2 = instance.value2;
        }

        public override string Serialize()
        {
            return JsonUtility.ToJson(this);
        }
    }

    public class Transporter : Billiards.MonoSingleton<Transporter>
    {
        private 
        Dictionary<string, object> registeredInstanceDict = new Dictionary<string, object>();

        public bool Send(Action<SerializableClass> invocationTarget, SerializableClass data)
        {
            var packet = CreatePacket(invocationTarget, data);
            if (packet == null)
                return false;

            SendTo(packet.Serialize());
            return true;
        }

        public void RegisterTarget(Action<SerializableClass> invocationTarget)
        {
            var name = invocationTarget.GetMethodInfo().ReflectedType.ToString();
            var instance = invocationTarget.Target;

            registeredInstanceDict[name] = instance;
        }

        PacketInfo<Action<SerializableClass>, SerializableClass> CreatePacket(Action<SerializableClass> invocationTarget, SerializableClass data)
        {
            if(!registeredInstanceDict.ContainsKey(invocationTarget.GetMethodInfo().ReflectedType.ToString()))
            {
                Debug.LogError("invocationTarget is NOT Registered.");
                return null;
            }

            PacketInfo<Action<SerializableClass>, SerializableClass> packet = new PacketInfo<Action<SerializableClass>, SerializableClass>();
            packet.Set(invocationTarget, data);
            return packet;
        }

        void SendTo(string data)
        {
            //implement send logic instead.

            //test logic
            ReceiveFrom(data);
        }

        void ReceiveFrom(string data)
        {
            var received = PacketInfo<Action<SerializableClass>, SerializableClass>.ToInfo(data);
            var action = received.Get((name) => registeredInstanceDict[name]);
            action.Invoke(received.argsData as SerializableClass);
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

}