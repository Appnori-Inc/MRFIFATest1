using UnityEngine;
using UnityEditor;
using System.Reflection;

[CustomPropertyDrawer(typeof(ButtonAttribute))]
public class ButtonDrawer : PropertyDrawer
{
	ButtonAttribute battribute;
	Object obj;
	Rect buttonRect;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		battribute = attribute as ButtonAttribute;
		obj = property.serializedObject.targetObject;
		MethodInfo method = obj.GetType().GetMethod(battribute.methodName, battribute.flags);

		if (method == null)
		{
			EditorGUI.HelpBox(position, "Method Not Found", MessageType.Error);

		}
		else
		{
			if (battribute.useValue)
			{
				buttonRect = new Rect(position.x , position.y, position.width, position.height);

				EditorGUI.PropertyField(Rect.zero, property, GUIContent.none);
				if (GUI.Button(buttonRect, battribute.buttonName))
				{
					method.Invoke(obj, new object[] { fieldInfo.GetValue(obj) });
				}

			}
			else
			{
				if (GUI.Button(position, battribute.buttonName))
				{
					method.Invoke(obj, null);
				}
			}
		}
	}
}