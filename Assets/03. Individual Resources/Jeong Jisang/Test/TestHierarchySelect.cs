using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using System.Linq;
using System;
using System.Reflection;

#if UNITY_EDITOR && DYNAMIC_LINQ
using UnityEditor;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

public class TestHierarchySelect : MonoBehaviour
{
    [SerializeField]
    private bool FindInSelection;
    [SerializeField]
    private bool SelectOriginal;

    [SerializeField]
    private string TypeName;
    [SerializeField]
    private string AssemblyName;
    [SerializeField]
    private string argName;
    //[SerializeField]
    //private string lastTypeName;

    [Space(10)]
    [TextArea]
    [Header("new Func<-TypeName-,bool>((-argName-)=> value)")]
    [SerializeField]
    public string lambdaExpression;

    //그냥 버튼용... 에디터 스크립트 버튼만 만드는법을 모름
    [Button("SearchTarget", true)]
    public bool value;


    public void SearchTarget(bool value)
    {
        var testType = typeof(UnityEngine.Rendering.VolumeProfile);
        var type = GetType(TypeName, AssemblyName);
        //var lastType = GetType(lastTypeName);
        var p = Expression.Parameter(type, argName);
        var e = DynamicExpressionParser.ParseLambda(new[] { p }, null, lambdaExpression);

        var SelectToGameObject = new Func<UnityEngine.Object, GameObject>((obj) =>
        {
            if (obj is GameObject) return (obj as GameObject);
            else if (obj is Component) return (obj as Component).gameObject;
            else if (obj is Transform) return (obj as Transform).gameObject;
            else if (obj is ScriptableObject) return obj as GameObject;

            return null;

        });

        if (FindInSelection)
        {
            var targets = Selection.objects.Where(obj => (obj as GameObject).TryGetComponent(type, out var component)).Where(obj => obj != null).Where(obj => (bool)e.Compile().DynamicInvoke((obj as GameObject).GetComponent(type)));
            Selection.objects = targets.Select(SelectToGameObject).Where(obj => obj != null).ToArray();
        }
        else
        {
            var expression = e.Compile();
            var targets = Resources.FindObjectsOfTypeAll(type).Where(obj => (bool)e.Compile().DynamicInvoke(obj));
            //var targets = Resources.FindObjectsOfTypeAll(type).Select(obj =>
            //typeof(TestHierarchySelect).GetMethod("ConvertTo").MakeGenericMethod(lastType).Invoke(this, new object[] { lastType, e.Compile().DynamicInvoke(obj) }));
            //Selection.objects = targets.ToArray();
            if (SelectOriginal)
            {
                Selection.objects = targets.ToArray();
            }
            else
            {
                Selection.objects = targets.Select(SelectToGameObject).ToArray();
            }
        }

        //private GameObject SelectToGameObject(in UnityEngine.Object obj)
        //{
        //    if (obj is GameObject) return (obj as GameObject);
        //    else if (obj is Component) return (obj as Component).gameObject;
        //    else if (obj is Transform) return (obj as Transform).gameObject;

        //    return null;
        //}
    }


    public T ConvertTo<T>(Type type, object obj)
    {
        return (T)Convert.ChangeType(obj , type);
    }

    public bool test(GameObject obj)
    {
        //UnityEngine.Rendering.VolumeProfile vp;
        //vp.components.Find((compo)=>compo.name.ToLower().Contains("bloom")

        BoxCollider collider;
        if(obj.TryGetComponent<Box.BeatListener>(out var listener))
        {
            //var boo = typeof(Box.BeatListener).GetField("Axis", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(listener)) == -1;

            var field = typeof(Box.BeatListener).GetField("Axis", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var value = (Vector3)field.GetValue(listener);

            return value.z == -1;
        }

        return false;
    }
    public static Type GetType(string TypeName)
    {

        // Try Type.GetType() first. This will work with types defined
        // by the Mono runtime, etc.
        var type = Type.GetType(TypeName);

        // If it worked, then we're done here
        if (type != null)
            return type;

        // Get the name of the assembly (Assumption is that we are using
        // fully-qualified type names)
        var assemblyName = TypeName.Substring(0, TypeName.IndexOf('.'));

        // Attempt to load the indicated Assembly
        var assembly = Assembly.Load(assemblyName);
        if (assembly == null)
            return null;

        // Ask that assembly to return the proper Type
        return assembly.GetType(TypeName);

    }

    public static Type GetType(in string TypeName, string AssemblyName)
    {

        //Unity.RenderPipelines.Core.Runtime
        var type = Type.GetType(TypeName);

        // If it worked, then we're done here
        if (type != null)
            return type;

        // Get the name of the assembly (Assumption is that we are using
        // fully-qualified type names)
        if (string.IsNullOrEmpty(AssemblyName))
        {
            AssemblyName = TypeName.Substring(0, TypeName.IndexOf('.'));
        }

        // Attempt to load the indicated Assembly
        var assembly = Assembly.Load(AssemblyName);
        if (assembly == null)
            return null;

        // Ask that assembly to return the proper Type
        return assembly.GetType(TypeName);
    }
}

#else
public class TestHierarchySelect : MonoBehaviour
{
}
#endif