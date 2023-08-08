using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CustomizeProp : MonoBehaviour
{
    public abstract Vector3 GetCenterPos();
    public abstract void SetSize(bool isSelect);
}