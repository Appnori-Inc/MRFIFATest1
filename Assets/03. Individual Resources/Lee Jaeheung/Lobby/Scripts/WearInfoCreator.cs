using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(WearInfoCreator))]
public class Edit_WearInfoCreator : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        WearInfoCreator wearInfoCreator = (WearInfoCreator)target;
        if (GUILayout.Button("Create"))
        {
            wearInfoCreator.CreateWearInfo();
        }
    }
}
#endif

public enum WearState
{
    None, Upper, Upper_Cut, Lower, Lower_Sub, Foot, Hand, Body
}

public class WearInfoCreator : MonoBehaviour
{
    public WearState state;
    public string meshName;

    public void CreateWearInfo()
    {
        if (string.IsNullOrEmpty(meshName))
        {
            Debug.Log("No ID");
            return;
        }
        CustomizeDataInfo.WearInfo wearInfo = new CustomizeDataInfo.WearInfo();

        SkinnedMeshRenderer skin = transform.GetComponent<SkinnedMeshRenderer>();

        wearInfo.bounds_center = skin.localBounds.center;
        wearInfo.bounds_extent = skin.localBounds.extents;

        wearInfo.rootBone = BoneNameToIndex(skin.rootBone.name);

        wearInfo.bones = new int[skin.bones.Length];

        for (int j = 0; j < skin.bones.Length; j++)
        {
            wearInfo.bones[j] = BoneNameToIndex(skin.bones[j].name);
        }

        string jsonString = JsonUtility.ToJson(wearInfo);

        string stateNum = "";

        switch (state)
        {
            case WearState.Upper:
                {
                    stateNum = "30";
                }
                break;
            case WearState.Upper_Cut:
                {
                    stateNum = "31";
                }
                break;
            case WearState.Lower:
                {
                    stateNum = "32";
                }
                break;
            case WearState.Lower_Sub:
                {
                    stateNum = "33";
                }
                break;
            case WearState.Foot:
                {
                    stateNum = "34";
                }
                break;
            case WearState.Hand:
                {
                    stateNum = "35";
                }
                break;
            case WearState.Body:
                {
                    File.WriteAllText(Application.dataPath + "/03. Individual Resources/Lee Jaeheung/Resources/ItemInfos/BodyInfo_" + meshName + ".json", jsonString);
                    Debug.Log("Save JsonFile - BodyInfo_" + stateNum + meshName);
                    return;
                }
        }


        File.WriteAllText(Application.dataPath + "/03. Individual Resources/Lee Jaeheung/Resources/ItemInfos/WearInfo_" + stateNum + meshName + ".json", jsonString);

        Debug.Log("Save JsonFile - WearInfo_" + stateNum + meshName);
    }

    public static int BoneNameToIndex(string _boneName)
    {
        switch (_boneName)
        {
            case "Bip001":
                return 0;
            case "Bip001 Pelvis":
                return 1;
            case "Bip001 L Thigh":
                return 2;
            case "Bip001 LThighTwist":
                return 3;
            case "Bip001 L Calf":
                return 4;
            case "Bip001 L Foot":
                return 5;
            case "Bip001 L Toe0":
                return 6;
            case "Bip001 R Thigh":
                return 7;
            case "Bip001 RThighTwist":
                return 8;
            case "Bip001 R Calf":
                return 9;
            case "Bip001 R Foot":
                return 10;
            case "Bip001 R Toe0":
                return 11;
            case "Bip001 Spine":
                return 12;
            case "Bip001 Spine1":
                return 13;
            case "Bip001 Spine2":
                return 14;
            case "Bip001 L Clavicle":
                return 15;
            case "Bip001 L UpperArm":
                return 16;
            case "Bip001 LUpArmTwist":
                return 17;
            case "Bip001 L Forearm":
                return 18;
            case "Bip001 L ForeTwist":
                return 19;
            case "Bip001 L Hand":
                return 20;
            case "Bip001 L Finger0":
                return 21;
            case "Bip001 L Finger01":
                return 22;
            case "Bip001 L Finger02":
                return 23;
            case "Bip001 L Finger1":
                return 24;
            case "Bip001 L Finger11":
                return 25;
            case "Bip001 L Finger12":
                return 26;
            case "Bip001 L Finger2":
                return 27;
            case "Bip001 L Finger21":
                return 28;
            case "Bip001 L Finger22":
                return 29;
            case "Bip001 L Finger3":
                return 30;
            case "Bip001 L Finger31":
                return 31;
            case "Bip001 L Finger32":
                return 32;
            case "Bip001 L Finger4":
                return 33;
            case "Bip001 L Finger41":
                return 34;
            case "Bip001 L Finger42":
                return 35;
            case "Bip001 Neck":
                return 36;
            case "Bip001 Head":
                return 37;
            case "Bip001 R Clavicle":
                return 38;
            case "Bip001 R UpperArm":
                return 39;
            case "Bip001 RUpArmTwist":
                return 40;
            case "Bip001 R Forearm":
                return 41;
            case "Bip001 R ForeTwist":
                return 42;
            case "Bip001 R Hand":
                return 43;
            case "Bip001 R Finger0":
                return 44;
            case "Bip001 R Finger01":
                return 45;
            case "Bip001 R Finger02":
                return 46;
            case "Bip001 R Finger1":
                return 47;
            case "Bip001 R Finger11":
                return 48;
            case "Bip001 R Finger12":
                return 49;
            case "Bip001 R Finger2":
                return 50;
            case "Bip001 R Finger21":
                return 51;
            case "Bip001 R Finger22":
                return 52;
            case "Bip001 R Finger3":
                return 53;
            case "Bip001 R Finger31":
                return 54;
            case "Bip001 R Finger32":
                return 55;
            case "Bip001 R Finger4":
                return 56;
            case "Bip001 R Finger41":
                return 57;
            case "Bip001 R Finger42":
                return 58;
            default:
                {
                    //Debug.LogWarning("Not Find - " + _boneName);
                }
                return -1;
        }
    }
}
