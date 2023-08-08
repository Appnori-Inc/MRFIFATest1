using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Billiards
{

    public class CoroutineTest : MonoBehaviour
    {
        private CoroutineWrapper wrapper;
        private void Awake()
        {
            wrapper = CoroutineWrapper.Generate(this)
                .StartSingleton(Test())
                .SetOnComplete(()=> { Debug.Log("End"); });


            void Logger()
            {
                Debug.Log("End");
            }
            
            IEnumerator Test()
            {
                Debug.Log("start");

                yield return new WaitForSeconds(1.5f);

                Debug.Log("something");
            }

        }

        private IEnumerator Start()
        {
            StartCoroutine("Test");
            StartCoroutine("Test");
            yield return new WaitForSeconds(1f);
            //StopCoroutine("Test");
            //wrapper.Stop();
            //wrapper.Start(Test());

            //IEnumerator Test()
            //{
            //    Debug.Log("a");

            //    yield return null;

            //    Debug.Log("b");
            //}


            IEnumerator LocalTest()
            {
                yield break;
            }

        }


        private IEnumerator Test()
        {
            Debug.Log("Test 01");
            yield return new WaitForSeconds(2f);
            Debug.Log("Test 02");

        }
    }

}