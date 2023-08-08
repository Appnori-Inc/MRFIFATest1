using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BallPool;

/// <summary>
/// The balls sorting manager, select the gameobject with component BallsSortingManager and sort balls.
/// </summary>
public class BallPoolSortingManagerEditor : EditorWindow
{
    [MenuItem ("Window/Ball Pool/Sort balls")]
    static void Init () 
    {
        GameObject activeGameObject = Selection.activeGameObject;
        if (activeGameObject)
        {
            BallPoolBallsSortingManager sortingManager = activeGameObject.GetComponent<BallPoolBallsSortingManager>();
            if (sortingManager != null)
            {
                sortingManager.SortBalls();
            }
            else
            {
                Debug.LogWarning("Please select the gameobject with component CmBallsSortingManager");
            }
        }
        else
        {
            Debug.LogWarning("Please select the gameobject with component CmBallsSortingManager");
        }
    }
}
