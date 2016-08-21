
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

public static class PseudoDefaultInspector
{
	public static bool DrawAsFoldout(SerializedObject serializedObject, string propName, string title)
	{
		var prop = serializedObject.FindProperty(propName);
		if (prop.objectReferenceValue == null || prop.hasMultipleDifferentValues)
		{
			return false;
		}
		else
		{
			if (!FoldoutStates.ContainsKey(title)) FoldoutStates.Add(title, false);
			FoldoutStates[title] = EditorGUILayout.Foldout(FoldoutStates[title], title);
			if (FoldoutStates[title])
			{
				var propList = new List<UnityEngine.Object>();
				foreach (var oTarget in serializedObject.targetObjects)
				{
					var myTarget = oTarget as UnityEngine.Object;
					Debug.Assert(myTarget != null, "MyTarget != null");
					var value = myTarget.GetType().GetField(propName).GetValue(myTarget) as UnityEngine.Object;
					if (value != null)
					{
						propList.Add(value);
					}
				}
				var obj_clip = new SerializedObject(propList.ToArray());
				return PseudoDefaultInspector.Draw(obj_clip);
			}
		}
		return false;
	}
	public static bool Draw(SerializedObject serializedObject, string propName)
	{
		var propList = new List<UnityEngine.Object>();
		var field = serializedObject.targetObject.GetType().GetField(propName);
		if (field == null) return false;
		foreach (var oTarget in serializedObject.targetObjects)
		{
			var myTarget = oTarget as UnityEngine.Object;
			Debug.Assert(myTarget != null, "MyTarget != null");
			var value = field.GetValue(myTarget) as UnityEngine.Object;
			if (value == null) return false;
			propList.Add(value);
		}
		var obj_clip = new SerializedObject(propList.ToArray());
		return PseudoDefaultInspector.Draw(obj_clip);
	}
	public static bool Draw(SerializedObject obj)
	{
		EditorGUI.BeginChangeCheck();
		obj.Update();
		SerializedProperty iterator = obj.GetIterator();
		bool enterChildren = true;
		while (iterator.NextVisible(enterChildren))
		{
			if (iterator.name != "m_Script")
			{
				EditorGUILayout.PropertyField(iterator, true, new GUILayoutOption[0]);
				enterChildren = false;
			}
		}
		obj.ApplyModifiedProperties();
		return EditorGUI.EndChangeCheck();
	}

	private static readonly Dictionary<string, bool> FoldoutStates = new Dictionary<string, bool>();
}
