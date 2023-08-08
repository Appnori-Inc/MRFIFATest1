using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Billiards
{

    public class LocalFunctionTest : MonoBehaviour
    {
        public void Awake()
        {
            float t = 0;
            //var lambda = new Action(() => { t = 15; });

            LocalFunction(ref t);
            //lambda();

            Debug.Log("T : " + t);

            StartCoroutine(RunRoutine());

            return;
            //localfunctions 

            IEnumerator RunRoutine()
            {
                while (true)
                {
                    yield return new WaitForSeconds(1f);
                    t += 1f;
                    Debug.Log("In Loop : " + t);
                }

                LocalFunction(ref t);

                Debug.Log("Loop Break : " + t);
            }


            void LocalFunction(ref float temp)
            {
                temp = 15;
            }
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        public void Update()
        {
            
        }
    }

}