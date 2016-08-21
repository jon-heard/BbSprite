
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
class Window_BbSpriteOptions : EditorWindow
{
	private static readonly Vector2 Size = new Vector2(265, 315);
	private static readonly Texture _logo;

	public static void ShowWindow()
	{
		var sceneViewPos = GetWindow(typeof(SceneView)).position;
		var window = GetWindow<Window_BbSpriteOptions>("BbSprite options");
		var windowSize = Size;
		window.maximized = false;
		window.maxSize = window.minSize = windowSize;
		window.position = new Rect(
			sceneViewPos.x + (sceneViewPos.width - windowSize.x) / 2.0f,
			sceneViewPos.y + (sceneViewPos.height - windowSize.y) / 2.0f,
			windowSize.x,
			windowSize.y);
		window.Show();
	}

	static Window_BbSpriteOptions()
	{
		if (_logo == null)
		{
			_logo = Resources.Load<Texture>("logo_64");
			_logo.hideFlags = HideFlags.HideAndDontSave;
		}
	}

	private void OnDisable()
	{
		EditorPrefs.SetBool("BbSprite_AutoOptimizeImages", BbSprite.AutoOptimizeImages);
		EditorPrefs.SetBool("BbSprite_AutoGuessTransparency", BbSprite.AutoGuessTransparency);
		EditorPrefs.SetBool("BbSprite_AutoGuessLayout", BbSprite.AutoGuessLayout);
		EditorPrefs.SetBool("BbSprite_AutoGuessAnimation", BbSprite.AutoGuessAnimation);
		EditorPrefs.SetFloat("BbSprite_OrientPtrRed", BbSprite.OrientPtrColor.r);
		EditorPrefs.SetFloat("BbSprite_OrientPtrGreen", BbSprite.OrientPtrColor.g);
		EditorPrefs.SetFloat("BbSprite_OrientPtrBlue", BbSprite.OrientPtrColor.b);
		EditorPrefs.SetBool("BbSprite_OrientPtrAtRun", BbSprite.OrientPtrAtRun);
	}

	private void OnGUI()
	{
		var style = new GUIStyle {alignment = TextAnchor.MiddleCenter};

		// About section
		GUILayout.Label("");
		GUILayout.Box(_logo, style);
		GUILayout.Label("");
		style.fontStyle = FontStyle.Bold;
		GUILayout.Label("BbSprite", style);
		style.fontStyle = FontStyle.Italic;
		GUILayout.Label("v.0.07", style);
		GUILayout.Label("Copyright 2014 Pixelgig", style);
		GUILayout.Label("");

		// Options section
		BbSprite.AutoOptimizeImages = GUILayout.Toggle(BbSprite.AutoOptimizeImages, "Automatically optimize spritesheets");
		BbSprite.AutoGuessTransparency = GUILayout.Toggle(BbSprite.AutoGuessTransparency, "Automatically guess transparency shader");
		BbSprite.AutoGuessLayout = GUILayout.Toggle(BbSprite.AutoGuessLayout, "Automatically guess layout type");
		BbSprite.AutoGuessAnimation = GUILayout.Toggle(BbSprite.AutoGuessAnimation, "Automatically guess AnimationClipOld set");
		BbSprite.OrientPtrColor = EditorGUILayout.ColorField("Orientation pointer color", BbSprite.OrientPtrColor);
		BbSprite.OrientPtrAtRun = GUILayout.Toggle(BbSprite.OrientPtrAtRun, "Show orientation pointers at runtime");

		// Ok button
		GUILayout.Label("");
		GUILayout.BeginHorizontal();
		//GUILayout.FlexibleSpace();
		if (GUILayout.Button("Ok")) Close();
		GUILayout.EndHorizontal();
	}
}
