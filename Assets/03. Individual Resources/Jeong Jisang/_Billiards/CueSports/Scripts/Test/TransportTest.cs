using UnityEngine;
using System.Collections;
using Billiards;
using System;

namespace Billiards
{

    public class TransportTest : MonoBehaviour
    {
        [Serializable]
        public class TestData : SerializableClass
        {
            public float time;

            public override void DeSerialize(string data)
            {
                var instance = JsonUtility.FromJson<TestData>(data);
                time = instance.time;
            }

            public override string Serialize()
            {
                return JsonUtility.ToJson(this);
            }
        }


        private void Awake()
        {
            //1. register all invocation targets.
            Transporter.Instance.RegisterTarget(Call);
        }

        int idx = 0;
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var data_test = new TestData()
                {
                    time = Time.time
                };

                Transporter.Instance.Send(Call, data_test);
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                var data_normal = new Data()
                {
                    value = idx++,
                    value2 = "current idx is {0}"
                };

                Transporter.Instance.Send(Call, data_normal);
            }
        }

        public void Call(SerializableClass packet)
        {
            switch (packet)
            {
                case TestData testData: Debug.Log("current Time : " + testData.time); return;
                case Data data: Debug.Log(string.Format(data.value2, data.value)); return;

                default: Debug.Log("packet Type is NOT supported"); return;

            }
        }
    }

}