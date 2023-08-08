#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ColorPaletteCtrl))]
public class Edit_ColorPaletteCtrl : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ColorPaletteCtrl colorCtrl = (ColorPaletteCtrl)target;
        if (GUILayout.Button("Create"))
        {
            colorCtrl.CreateColorPalette();
        }
    }
}
#endif