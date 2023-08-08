using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System;
using System.Linq;

namespace Jisu.Utils
{

    [InitializeOnLoad]
    public class SceneSwitchLeftButton : MonoBehaviour
    {
        public static string FolderPath { get; private set; }
        private static readonly string fileName = @"/SceneHelperData.json";

        private static readonly Texture tex = EditorGUIUtility.FindTexture(@"UnityEditor.SceneView");

        private static readonly SceneHelperData data;

        private bool test;
        static SceneSwitchLeftButton()
        {
            InitFolderPath();

            ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);

            var fromJson = AssetDatabase.LoadAssetAtPath(FolderPath + fileName, typeof(TextAsset)) as TextAsset;
            if (fromJson == null)
            {
                var newData = new SceneToJson(0);
                var toJson = JsonUtility.ToJson(newData, true);
                System.IO.File.WriteAllText(FolderPath + fileName, toJson);

                data = new SceneHelperData();
            }
            else
            {
                var loadSceneToJson = JsonUtility.FromJson<SceneToJson>(fromJson.ToString());

                var newData = new SceneHelperData()
                {
                    scene = new SceneAsset[loadSceneToJson.scenePath.Length]
                };

                if (loadSceneToJson != null)
                {
                    for (int i = 0; i < loadSceneToJson.scenePath.Length; i++)
                    {
                        newData.scene[i] = AssetDatabase.LoadAssetAtPath(loadSceneToJson.scenePath[i], typeof(SceneAsset)) as SceneAsset;
                    }
                }

                data = newData;
            }
        }

        private static void InitFolderPath([System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
        {
            FolderPath = System.IO.Path.GetDirectoryName(sourceFilePath);
            int rootIndex = FolderPath.IndexOf(@"Assets\");
            if (rootIndex > -1)
            {
                FolderPath = FolderPath.Substring(rootIndex, FolderPath.Length - rootIndex);
            }
        }

        static void OnToolbarGUI()
        {
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(new GUIContent(null, tex, "Scene Helper Setting"), "Command"))
            {
                var window = (SceneHelperSetter)EditorWindow.GetWindow(typeof(SceneHelperSetter));
                window.minSize = new Vector2(400f, 300f);
                window.data = data;
                window.folderPath = FolderPath + fileName;
            }

            for (int i = 0; i < data.scene.Length; i++)
            {
                if (GUILayout.Button(new GUIContent($"{i + 1}", $"Switch Scene {i + 1}"), GUIStyles.ToolbarStyles.commandButtonStyle))
                {
                    SceneHelper.StartScene(data.scene[i]);
                }
            }
        }
    }

    static class SceneHelper
	{
		static SceneAsset sceneToOpen;

		public static void StartScene(SceneAsset scene)
		{
			if(EditorApplication.isPlaying)
			{
				EditorApplication.isPlaying = false;
			}

			sceneToOpen = scene;
			EditorApplication.update += OnUpdate;
		}

		static void OnUpdate()
		{
			if (sceneToOpen == null || EditorApplication.isPlaying || EditorApplication.isPaused || EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
				return;

            if (AssetDatabase.OpenAsset(sceneToOpen.GetInstanceID()) == false)
                Debug.LogWarning("Couldn't find scene file");

            sceneToOpen = null;

            EditorApplication.update -= OnUpdate;
        }
	}

    public class SceneHelperSetter : EditorWindow
    {
        public SceneHelperData data;

        public string folderPath;

        private void OnDestroy()
        {
            var sceneToJson = new SceneToJson(data.scene.Length);
            for(int i =0; i < sceneToJson.scenePath.Length; i++)
            {
                var path = AssetDatabase.GetAssetPath(data.scene[i]);
                sceneToJson.scenePath[i] = path;
            }

            var toJson = JsonUtility.ToJson(sceneToJson, true);
            System.IO.File.WriteAllText(folderPath, toJson);
        }

        void OnGUI()
        {
            GUILayout.Label("Scene Name Settings", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            int length = data.scene.Length;
            EditorGUILayout.LabelField("[ Scene Count - " + length + " ]");

            if (GUILayout.Button("+") && length < 20)
            {
                length++;
                Array.Resize(ref data.scene, length);
            }
            if (GUILayout.Button("-") && length >= 1)
            {
                length--;
                Array.Resize(ref data.scene, length);
            }

            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < data.scene.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();

                if (data.scene[i] == null)
                    data.scene[i] = null;

                EditorGUILayout.LabelField($"Scene {i + 1:D2}");

                data.scene[i] = (SceneAsset)EditorGUILayout.ObjectField(data.scene[i] == null ? null : data.scene[i], typeof(SceneAsset), true);

                EditorGUILayout.EndHorizontal();
            }
        }
    }


    [Serializable]
    public class SceneHelperData
    {
        public SceneAsset[] scene;

        public SceneHelperData()
        {
            scene = new SceneAsset[0];
        }
    }

    [Serializable]
    public class SceneToJson
    {
        public string[] scenePath;

        public SceneToJson(int length)
        {
            scenePath = new string[length];
        }
    }
}