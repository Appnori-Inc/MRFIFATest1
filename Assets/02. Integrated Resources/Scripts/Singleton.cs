using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//싱글톤 생성
namespace SingletonBase
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T Instance = null;

        public static bool IsInstance
        {
            get
            {
                if (Instance == null)
                {
                    return false;
                }
                return true;
            }
        }

        public static T GetInstance
        {
            get
            {
                if (Instance == null)
                {
                    return new GameObject(typeof(T).Name).AddComponent<T>();
                }
                return Instance;
            }
        }

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}

//Pun2 사용(RPC_) 싱글톤 생성
namespace SingletonPunBase
{
    public abstract class Singleton<T> : MonoBehaviourPunCallbacks where T : MonoBehaviourPunCallbacks
    {
        protected static T Instance = null;
        public static T GetInstance
        {
            get
            {
                if (Instance == null)
                {
                    return new GameObject(typeof(T).Name).AddComponent<T>();
                }
                return Instance;
            }
        }

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}

