//
//using UnityEngine;
//using UnityEditor;
//
//public class Editor_Shader_Alpha : MaterialEditor
//{
//	public override void OnInspectorGUI()
//	{
//		Material material = target as Material;
//		if (BbSprite.RenderSettings.IsShaderColorKey(material.shader))
//		{
//			Editor_Shader_ColorKey.drawInspector(material);
//			return;
//		}
//		drawInspector(material);
//	}
//
//	internal static void drawInspector(Material material)
//	{
//		// Float - Cutoff
//		var floatBuf = EditorGUILayout.Slider(new GUIContent(
//			"Cutoff", "The lowest alpha value before transparancy"),
//			material.GetFloat("_Cutoff"), 0, 1);
//		if (floatBuf != material.GetFloat("_Cutoff"))
//		{
//			material.SetFloat("_Cutoff", floatBuf);
//		}
//	}
//}
