
using UnityEditor;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

[CanEditMultipleObjects]
[CustomEditor(typeof(BbSpriteLayout))]
// ReSharper disable once CheckNamespace
class Editor_BbSpriteLayout : Editor
{
	public static BbSpriteLayout MyTarget;
	public static Texture MyAtlas
	{
		get
		{
			if(MyTarget == null) return null;
			var sprite = MyTarget.GetComponent<BbSprite>();
			if (sprite == null) return null;
			return sprite.Atlas;
		}
	}
	public static Editor_BbSpriteLayout Thiss;

	private void OnEnable()
	{
		Thiss = this;
	}
	private void OnDisable()
	{
		MyTarget = null;
	}

	public override void OnInspectorGUI()
	{
		var type = serializedObject.FindProperty("MyType");
		MyTarget = target as BbSpriteLayout;

		// Preview button
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		var content = new GUIContent(
			Window_AtlasPreview.IsOpened ? "Close preview" : "Open preview...",
			"Toggle a window that lets you view the layout of the selected BbSprite"
			);
		if (GUILayout.Button(content)) TogglePreview();
		EditorGUILayout.EndHorizontal();

		// Type
		content = new GUIContent
			("Type", "The layout configuration type for this sprite's sheet.");
		EditorGUILayout.PropertyField(type, content);

		//PseudoDefaultInspector.Draw(serializedObject, "_myLayoutAsCustomGrid");

		//var prop_grid = serializedObject.FindProperty("_myLayoutAsCustomGrid");
		//if (prop_grid.objectReferenceValue != null && !prop_grid.hasMultipleDifferentValues)
		//{
		//	_isGridEditorOpened = EditorGUILayout.Foldout(_isGridEditorOpened, "Clip properties");
		//	if (_isGridEditorOpened)
		//	{
		//		var grids = new List<LayoutData_CustomGrid>();
		//		foreach (var oTarget in serializedObject.targetObjects)
		//		{
		//			var myTarget = oTarget as BbSpriteLayout;
		//			Debug.Assert(myTarget != null, "MyTarget != null");
		//			grids.Add(myTarget._myLayoutAsCustomGrid);
		//		}
		//		var obj_clip = new SerializedObject(grids.ToArray());
		//		PseudoDefaultInspector.Draw(obj_clip);
		//	}
		//}

		// Show extra ui for certain layout types
		//if (
		//	type.enumValueIndex == (int)BbSpriteLayout.Type.CustomGrid &&
		//	!type.hasMultipleDifferentValues)
		//{
		//	DrawUi_CustomGrid();
		//}

		// Update properties and, if they changed, call sprite's Update
		if (serializedObject.ApplyModifiedProperties() ||
			Event.current.commandName == "UndoRedoPerformed") {
			foreach (var oTarget in serializedObject.targetObjects) {
				var myTarget = oTarget as BbSpriteLayout;
				Debug.Assert(myTarget != null, "myTarget != null");
				myTarget.Update();
				var sprite = myTarget.GetComponent<BbSprite>();
				if (sprite != null) {
					sprite.Update();
				}
			}
		}
	}

	private void TogglePreview()
	{
		if (Window_AtlasPreview.IsOpened)
		{
			Window_AtlasPreview.HideWindow();
		}
		else
		{
			Window_AtlasPreview.ShowWindow();
		}
	}

	private void DrawUi_CustomGrid()
	{
		var customGridProperty = serializedObject.FindProperty("_myLayoutAsCustomGrid");
		if (customGridProperty == null) return;
		var cellCount = customGridProperty.FindPropertyRelative("CellCount");
		var area = customGridProperty.FindPropertyRelative("Area");
		var verticalIndexing = customGridProperty.FindPropertyRelative("VerticalIndexing");
		//var directionOffsetList = customGridProperty.FindPropertyRelative("DirectionOffsetList");

		var content = new GUIContent
			("Cell count", "The number of rows and columns in this grid.");
		//		EditorGUILayout.PropertyField(serializedObject.FindProperty("_gridLayout.CellCountOld"), content);
		EditorGUILayout.PropertyField(cellCount, content);
		content = new GUIContent
			("LayoutData_CustomGrid area", "The area of the texture that this grid is laid over.");
		EditorGUILayout.PropertyField(area, content);
		content = new GUIContent("Vertical indexing",
			"If checked, cells are ordered top-to-bottom, else they are ordered left-to-right.");
		EditorGUILayout.PropertyField(verticalIndexing, content);

		// Directional offsets
		//content = new GUIContent("Direction offsets",
		//	"Cell offsets to shift by for different view angles.");
		//var valRef = (int[])directionOffsetList.objectReferenceValue;
		//var directionOffsetString = IntArrayStringHelper.intArrayToString(valList);
		//var newTextVal = EditorGUILayout.TextField(content, directionOffsetString);
		//if (newTextVal != directionOffsetString)
		//{
		//	try
		//	{
		//		var val = IntArrayStringHelper.StringToIntArray(newTextVal, true);
		//		UnityEngine.Debug(val.Length);
		//		directionOffsetList.ClearArray();
		//		directionOffsetList.arraySize = val.Length;
		//		directionOffsetList.
		//	}
		//	catch {}
		//		Undo.RecordObject(layout, "MyLayoutData change");
		//	layout.DirectionOffsetString = newTextVal;
		//}
	}

	private bool _isGridEditorOpened;
}

//[CustomPropertyDrawer(typeof(UIntVector2))]
//class Drawer_UIntVector2 : PropertyDrawer
//{
//	public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
//	{
//		var x = prop.FindPropertyRelative("x");
//		var y = prop.FindPropertyRelative("y");
//		EditorGUILayout.LabelField(label);
//		EditorGUILayout.BeginHorizontal();
//			GUILayout.FlexibleSpace();
//			EditorGUILayout.PropertyField(x);
//			EditorGUILayout.PropertyField(y);
//		EditorGUILayout.EndHorizontal();
//	}
//}
