using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTagLayer : MonoBehaviour
{

    public string TagName;
    public string LayerName;
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.tag = TagName;
        this.gameObject.layer = LayerMask.NameToLayer(LayerName);
    }
}
