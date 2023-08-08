using Billiards;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Appnori
{

    public class SceneManagement : MonoSingleton<SceneManagement>
    {
        protected override void Awake()
        {
            base.Awake();

            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        }

        private void SceneManager_activeSceneChanged(UnityEngine.SceneManagement.Scene prev, UnityEngine.SceneManagement.Scene next)
        {
            Debug.Log($"Scene changed : {prev} -> {next}");
        }

        private void Update()
        {
            for (int i = 0; i < 10; ++i)
            {
                if (Input.GetKeyDown((KeyCode)((int)KeyCode.Alpha0 + i)))
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(i);
                }
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
        }
    }
}