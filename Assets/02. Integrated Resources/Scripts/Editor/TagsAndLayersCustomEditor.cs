using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Appnori
{
    //[CustomEditor(typeof(TagsAndLayers))]
    public class TagsAndLayersCustomEditor : Editor
    {
        //string tagname = "";
        //string layername = "";

        //private bool[] folds = new bool[32];
        //private Vector2 scrollPos = Vector2.zero;

        //public override void OnInspectorGUI()
        //{
        //    DrawDefaultInspector();
        //    /// Actual GUI
        //    scrollPos = GUILayout.BeginScrollView(scrollPos);
        //    for (int one = 0; one != 32; one++)
        //    {
        //        /// Get layer Name (un-named layers are unused)
        //        string name1 = LayerMask.LayerToName(one);
        //        if (string.IsNullOrEmpty(name1)) continue;

        //        /// Separate each layer with a horizontal bar
        //        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        //        GUILayout.Space(5);

        //        /// Display a Foldout for each layer
        //        folds[one] = EditorGUILayout.Foldout(folds[one], name1);

        //        /// Group in categories
        //        if (folds[one])
        //        {
        //            /// Hacked For-loop to make current Layer1 be the first
        //            int layersRead = 0;
        //            for (int two = one; layersRead != 32; /*Loop is manually manipulated*/)
        //            {
        //                string name2 = LayerMask.LayerToName(two);
        //                if (!string.IsNullOrEmpty(name2))
        //                {
        //                    /// Matching layer in the matrix is put first
        //                    /// and re-named "self"
        //                    if (two == one) name2 = "Self";

        //                    GUILayout.BeginHorizontal();
        //                    GUILayout.Space(20);

        //                    /// Get collision value, and change it if needed
        //                    /// Values are inverted because fuck Unity -.-
        //                    var value = !Physics.GetIgnoreLayerCollision(one, two);
        //                    var newValue = EditorGUILayout.ToggleLeft(name2, value);
        //                    if (value != newValue) Physics.IgnoreLayerCollision(one, two, !newValue);

        //                    GUILayout.EndHorizontal();
        //                }

        //                /// Manually continue For-loop
        //                layersRead++;
        //                if (two == 31) two = 0;
        //                else two++;
        //            }
        //        }

        //        /// Add some space before next layer
        //        GUILayout.Space(5);
        //    }
        //    GUILayout.EndScrollView();
        //}

        //DrawDefaultInspector();
        //TagsAndLayers myTarget = (TagsAndLayers)target;

        //tagname = EditorGUILayout.TextField("", tagname);

        //if (tagname == "" || tagname == null)
        //{
        //    GUI.enabled = false;
        //}
        //else
        //{
        //    GUI.enabled = true;
        //}
        //if (GUILayout.Button("Create Tag"))
        //{
        //    myTarget.AddNewTag(tagname);
        //}

        //if (GUILayout.Button("Remove Tag"))
        //{
        //    myTarget.DeleteTag(tagname);
        //}

        //GUILayout.Space(50);

        //layername = EditorGUILayout.TextField("", layername);

        //if (layername == "" || layername == null)
        //{
        //    GUI.enabled = false;
        //}
        //else
        //{
        //    GUI.enabled = true;
        //}
        //if (GUILayout.Button("Create Layer"))
        //{
        //    myTarget.AddNewLayer(layername);
        //}

        //if (GUILayout.Button("Remove Layer"))
        //{
        //    myTarget.DeleteLayer(layername);
        //}
    }
}


