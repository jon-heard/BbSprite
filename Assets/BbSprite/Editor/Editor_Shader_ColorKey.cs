//
//using UnityEngine;
//using UnityEditor;
//
//public class Editor_Shader_ColorKey : MaterialEditor
//{
//
//	Editor_Shader_Alpha otherEditor;
//
//	public override void OnInspectorGUI()
//	{
//		Material material = target as Material;
//		if (material.shader == BbSprite.RenderSettings.shader_alpha)
//		{
//			Editor_Shader_Alpha.drawInspector(material);
//			return;
//		}
//		drawInspector(material);
//	}
//
//	internal static void drawInspector(Material material)
//	{
//		// Enum - Color key source
//		var curSource = 0;
//		if(material.shader == BbSprite.RenderSettings.shader_colorKey_ul)
//		{
//			curSource = 0;
//		}
//		else if(material.shader == BbSprite.RenderSettings.shader_colorKey_lr)
//		{
//			curSource = 1;
//		}
//		else if (material.shader == BbSprite.RenderSettings.shader_colorKey_ur)
//		{
//			curSource = 2;
//		}
//		else if (material.shader == BbSprite.RenderSettings.shader_colorKey_ll)
//		{
//			curSource = 3;
//		}
//		else if (material.shader == BbSprite.RenderSettings.shader_colorKey_custom)
//		{
//			curSource = 4;
//		}
//		var intBuf = EditorGUILayout.Popup(new GUIContent(
//			"Color key source",
//			"What pixel of the atlas determines the color key (the clear color)"),
//			curSource, new GUIContent[] {
//				new GUIContent("Upper left"),
//				new GUIContent("Lower right"),
//				new GUIContent("Upper right"),
//				new GUIContent("Lower left"),
//				new GUIContent("Custom color") });
//		if (intBuf != curSource)
//		{
//			switch (intBuf)
//			{
//				case 0:
//					material.shader = BbSprite.RenderSettings.shader_colorKey_ul;
//					break;
//				case 1:
//					material.shader = BbSprite.RenderSettings.shader_colorKey_lr;
//					break;
//				case 2:
//					material.shader = BbSprite.RenderSettings.shader_colorKey_ur;
//					break;
//				case 3:
//					material.shader = BbSprite.RenderSettings.shader_colorKey_ll;
//					break;
//				case 4:
//					material.shader = BbSprite.RenderSettings.shader_colorKey_custom;
//					break;
//			}
//		}
//
//		// Color - ColorKey
//		if (intBuf == 4)
//		{
//			Color colorBuf = EditorGUILayout.ColorField(new GUIContent(
//				"Color key", "This color will be clear when drawing the sprite"),
//				material.GetColor("_ColorKey"));
//			if (colorBuf != material.GetColor("_ColorKey"))
//			{
//				material.SetColor("_ColorKey", colorBuf);
//			}
//		}
//
//		// Float - Colorkey range
//		var floatBuf = EditorGUILayout.Slider(new GUIContent(
//			"Color key range", "Colors closer than this to color key are drawn clear"
//			), material.GetFloat("_ColorKeyRange"), 0, 2);
//		if (floatBuf != material.GetFloat("_ColorKeyRange"))
//		{
//			material.SetFloat("_ColorKeyRange", floatBuf);
//		}
//	}
//}
