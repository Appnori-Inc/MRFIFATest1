
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class HierarchySelectData : ScriptableObject
{
    public Predicate<GameObject> predicate = new Predicate<GameObject>((obj) => {  return obj.name.ToLower().Contains("box");});
}
