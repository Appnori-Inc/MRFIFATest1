using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "MapData", menuName = "ScriptableObjects/MapData", order = 1)]
public class MapData : ScriptableObject
{
    public string Name;
    public string themaId;
    public int  priceDia;
    public Material skyMat;
    public Color skyColor;
    public bool isFog;
    public Color fogColor;
    public bool buyYn;
    public Sprite UIImage;
    public Sprite UISquImage;

}