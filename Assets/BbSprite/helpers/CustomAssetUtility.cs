
using System.IO;
using UnityEditor;
using UnityEngine;

public static class CustomAssetUtility
{
	public static void CreateAsset<T>(string assetName) where T : ScriptableObject
	{
		var asset = ScriptableObject.CreateInstance<T>();

		var path = AssetDatabase.GetAssetPath (Selection.activeObject);
		if (path == "")
		{
			path = "Assets";
		}
		else if (Path.GetExtension (path) != "")
		{
			// ReSharper disable once AssignNullToNotNullAttribute
			path = path.Replace (Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");
		}

		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "/" + assetName + ".asset");

		//ProjectWindowUtil.CreateAsset(asset, assetPathAndName);
		AssetDatabase.CreateAsset (asset, assetPathAndName);

		AssetDatabase.SaveAssets();
		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = asset;
	}
}
