
using bbSprite;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Debug = System.Diagnostics.Debug;


// Inspector menu for BbSprites
// ReSharper disable once CheckNamespace
[CanEditMultipleObjects]
[CustomEditor(typeof (BbSprite))]
public class Editor_BbSprite : Editor
{
	// Both of these arrays are used to display the MyFacingType field
	private static readonly GUIContent[] FacingTypeList =
	{
		new GUIContent("About Y (characters,trees)"),
		new GUIContent("About All (orbs,explosions)"),
		new GUIContent("About None (fences,doors)")
	};

	// See comment above
	private static readonly int[] FacingTypeIndexList = {0, 1, 2};

    private static bool _staticInited = false;
    public void OnEnable()
    {
        if (!_staticInited)
        {
            _staticInited = true;
            BBSpriteSettings.AutoOptimizeImages =
                EditorPrefs.GetBool("BbSprite_AutoOptimizeImages", true);
            BBSpriteSettings.AutoGuessTransparency =
                EditorPrefs.GetBool("BbSprite_AutoGuessTransparency", true);
            BBSpriteSettings.AutoGuessLayout =
                EditorPrefs.GetBool("BbSprite_AutoGuessLayout", true);
            BBSpriteSettings.AutoGuessAnimation =
                EditorPrefs.GetBool("BbSprite_AutoGuessAnimation", true);
            BBSpriteSettings.OrientPtrColor = new Color(
                EditorPrefs.GetFloat("BbSprite_OrientPtrRed", 0),
                EditorPrefs.GetFloat("BbSprite_OrientPtrGreen", 0),
                EditorPrefs.GetFloat("BbSprite_OrientPtrBlue", 1));
            BBSpriteSettings.OrientPtrAtRun =
                EditorPrefs.GetBool("BbSprite_OrientPtrAtRun", false);
        }
    }

	// Called to draw the inspector UI for the current sprite
	public override void OnInspectorGUI()
	{
		var verticalOffset = serializedObject.FindProperty("VerticalOffset");
		var facingType = serializedObject.FindProperty("MyFacingType");

		// Field controls
		var content = new GUIContent
			("Vertical offset", "Useful if a sprite's feet aren't touching the ground.");
		EditorGUILayout.PropertyField(verticalOffset, content);
		content = new GUIContent
			("Facing type", "Determines the way the sprite rotates to face the camera.");
		EditorGUILayout.IntPopup(facingType, FacingTypeList, FacingTypeIndexList, content);
		// These controls change the fields, not the properties.  BbSprites only
		// know to update Mesh when properties are changed.  Therefore we must 
		// do this manually by calling Update if fields have changed.
		// This also needs to happen during an "undo" operation, so we have to
		// check for that too.
		if (serializedObject.ApplyModifiedProperties() ||
			  Event.current.commandName == "UndoRedoPerformed")
		{
			foreach (var oTarget in serializedObject.targetObjects)
			{
				var myTarget = oTarget as BbSprite;
				Debug.Assert(myTarget != null, "myTarget != null");
				myTarget.Update();
			}
		}

		// Buttons
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUI.enabled = !IsOptimized();
		if (GUILayout.Button(new GUIContent("Optimize image",
			"If image is blurry, this will make it crisp."))) Optimize();
		GUI.enabled = true;
		if (GUILayout.Button(new GUIContent("Options...",
			"Options for BbSprite"))) Window_BbSpriteOptions.ShowWindow();
		EditorGUILayout.EndHorizontal();
	}

	private bool IsOptimized()
	{
		foreach (var oTarget in serializedObject.targetObjects)
		{
			var myTarget = oTarget as BbSprite;
			Debug.Assert(myTarget != null, "myTarget != null");
			if (!TextureSettings.IsAtlasOptimalForBbSprite(myTarget.Atlas))
			{
				return false;
			}
		}
		return true;
	}

	private void Optimize()
	{
		if (EditorUtility.DisplayDialog(
			"Turn off blurring",
			"This will alter the current texture's settings to remove " +
			"blur/antialiasing/etc.\nThe texture's pixels will be sharp and " +
			"unobscured.\n\nWarning: These changes must be undone manually.\n" +
			"They include:\n" + TextureSettings.BbSpriteOptimalSettingsList,
			"OK", "Cancel"))
		{
			foreach (var oTarget in serializedObject.targetObjects)
			{
				var myTarget = oTarget as BbSprite;
				Debug.Assert(myTarget != null, "myTarget != null");
				TextureSettings.SetAtlasAsOptimalForBbSprite(myTarget.Atlas);
				myTarget.Update();
			}
		}
	}


	private static readonly Material BbSpriteDefaultMaterial;

	static Editor_BbSprite()
	{
		if (BbSpriteDefaultMaterial == null)
		{
			BbSpriteDefaultMaterial = Resources.Load<Material>("Default-BbSprite");
			BbSpriteDefaultMaterial.hideFlags = HideFlags.NotEditable;
		}
	}

	// Menu item
	[MenuItem("GameObject/Create Other/BbSprite", false, 1100)]
	private static void createBbSprite_MultiCellAnimatedShadow()
	{
		// Basic Mesh aspect
		var result = new GameObject();
		Undo.RegisterCreatedObjectUndo(result, "Create BbSprite");
		result.AddComponent<MeshFilter>();
		var renderer = result.AddComponent<MeshRenderer>();
	    renderer.shadowCastingMode = ShadowCastingMode.Off;
		var collider = result.AddComponent<CapsuleCollider>();
		collider.center = new Vector3(0, .5f, 0);
		collider.radius = .33333f;
		collider.height = 1;
		// Default position
	    var sceneCam = SceneView.lastActiveSceneView.camera;
	    result.transform.parent = sceneCam.transform;
        result.transform.localPosition = new Vector3(0, 0, 2);
	    result.transform.parent = null;
		// BbSprite aspect
		result.name = "BbSprite";
		renderer.sharedMaterial = BbSpriteDefaultMaterial;
		var sprite = result.AddComponent<BbSprite>();
		// BbSprite layout aspect
		var layout = result.AddComponent<BbSpriteLayout>();
		sprite.Update();
		layout.MyType = BbSpriteLayout.Type.RpgMaker2K;
		// BbSprite Animation aspect
		var animation = result.AddComponent<BbSpriteAnimation>();
		animation.Setup3X4();
		result.AddComponent<BlobShadow>();
	}
}
