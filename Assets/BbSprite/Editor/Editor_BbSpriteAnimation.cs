
using UnityEditor;
using Debug = System.Diagnostics.Debug;

[CanEditMultipleObjects]
[CustomEditor(typeof(BbSpriteAnimation))]
// ReSharper disable once CheckNamespace
class Editor_BbSpriteAnimation : Editor
{
	private void OnEnable()
	{
		EditorApplication.update += UpdateCallback;
	}

	private void OnDisable()
	{
		Debug.Assert(EditorApplication.update != null, "EditorApplication.update != null");
		EditorApplication.update -= UpdateCallback;
	}

	private void UpdateCallback()
	{
		foreach (var oTarget in serializedObject.targetObjects)
		{
			var myTarget = oTarget as BbSpriteAnimation;
			if (myTarget != null)
			{
				myTarget.Update();
				var sprite = myTarget.GetComponent<BbSprite>();
				if (sprite != null)
				{
					sprite.Update();
				}
			}
		}
	}

	public override void OnInspectorGUI()
	{
		if (DrawDefaultInspector())
		{
			foreach (var oTarget in serializedObject.targetObjects)
			{
				var myTarget = oTarget as BbSpriteAnimation;
				Debug.Assert(myTarget != null, "myTarget != null");
				myTarget.SyncDictionary();
			}
		}

		PseudoDefaultInspector.DrawAsFoldout(serializedObject, "clip", "Clip properties");
	}

    [MenuItem("Assets/Create/BbSprite Animation Clip")]
    public static void CreateAnimationClipFile()
    {
        CustomAssetUtility.CreateAsset<BbSpriteAnimationClip>("New BbSpriteAnimationClip");
    }
}
