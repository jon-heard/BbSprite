
using UnityEngine;

#if !UNITY_EDITOR

// Adjust the settings of an image to optimize it for pixel perfect rendering
public static class TextureSettings
{
    public static bool AddTemporaryReadableToTexture(Texture val)
    {
        return true;
    }

    public static bool RemoveTemporaryReadableFromTexture(Texture val)
    {
        return true;
    }

    public static string BbSpriteOptimalSettingsList
    {
        get
        {
            return
                "- Non Power of 2 = None\n" +
                "- Generate Mip Maps = off\n" +
                "- Filter Mode = Point\n" +
                "- Format = ARGB 32 bit\n";
        }
    }

    public static bool IsAtlasOptimalForBbSprite(Texture val)
    {
        return true;
    }

    public static void SetAtlasAsOptimalForBbSprite(Texture val)
    {
    }

    public static bool CheckTextureColumnForOpacity(Texture2D toCheck, int colId, bool hasKeyColor)
    {
        var height = toCheck.height;
        var pixelList = toCheck.GetPixels(colId, 0, 1, height);
        var keyColor = pixelList[0];
        foreach (var pixel in pixelList)
        {
            if (hasKeyColor)
            {
                if (pixel != keyColor)
                {
                    return true;
                }
            }
            else
            {
                if (pixel.a != 0)
                {
                    return true;
                }
            }
        }
        return false;
    }
}

#else
using System.Collections.Generic;
using UnityEditor;

// Adjust the settings of an image to optimize it for pixel perfect rendering
public static class TextureSettings
{
	private static readonly Dictionary<Texture, TexInfo> TemporaryImporters =
		new Dictionary<Texture, TexInfo>();

	public static bool AddTemporaryReadableToTexture(Texture val)
	{
		TexInfo info;
		if (TemporaryImporters.TryGetValue(val, out info))
		{
			info.ReadableCount++;
			return true;
		}
		var path = AssetDatabase.GetAssetPath(val);
		if (path == null) return false;
		var importer = AssetImporter.GetAtPath(path) as TextureImporter;
		if (importer == null) return false;
		if (!importer.isReadable)
		{
			importer.isReadable = true;
			AssetDatabase.ImportAsset(path);
			TemporaryImporters[val] = new TexInfo(importer);
		}
		return true;
	}

	public static bool RemoveTemporaryReadableFromTexture(Texture val)
	{
		TexInfo info;
		if (TemporaryImporters.TryGetValue(val, out info))
		{
			info.ReadableCount--;
			if (info.ReadableCount == 0)
			{
				TemporaryImporters.Remove(val);
				var path = AssetDatabase.GetAssetPath(val);
				info.Importer.isReadable = false;
				AssetDatabase.ImportAsset(path);
			}
			return true;
		}
		return false;
	}

	public static string BbSpriteOptimalSettingsList
	{
		get
		{
			return
				"- Non Power of 2 = None\n" +
				"- Generate Mip Maps = off\n" +
				"- Filter Mode = Point\n" +
				"- Format = ARGB 32 bit\n";
		}
	}

	public static bool IsAtlasOptimalForBbSprite(Texture val)
	{
		if (val == null) return true;
		var path = AssetDatabase.GetAssetPath(val);
		var importer =
			AssetImporter.GetAtPath(path) as
				TextureImporter;
		Debug.Assert(importer != null, "importer != null");
		if (importer.npotScale != TextureImporterNPOTScale.None)
		{
			return false;
		}
		if (importer.mipmapEnabled)
		{
			return false;
		}
		if (importer.filterMode != FilterMode.Point)
		{
			return false;
		}
		if (importer.textureFormat != TextureImporterFormat.ARGB32)
		{
			return false;
		}
		return true;
	}

	public static void SetAtlasAsOptimalForBbSprite(Texture val)
	{
		if (val == null) return;
		var path = AssetDatabase.GetAssetPath(val);
		var importer =
			AssetImporter.GetAtPath(path) as
				TextureImporter;
		Debug.Assert(importer != null, "importer != null");
		importer.npotScale = TextureImporterNPOTScale.None;
		importer.mipmapEnabled = false;
		importer.filterMode = FilterMode.Point;
		importer.textureFormat = TextureImporterFormat.ARGB32;
		AssetDatabase.ImportAsset(path);
	}

	public static bool CheckTextureColumnForOpacity(Texture2D toCheck, int colId, bool hasKeyColor)
	{
		var height = toCheck.height;
		var pixelList = toCheck.GetPixels(colId, 0, 1, height);
		var keyColor = pixelList[0];
		foreach (var pixel in pixelList)
		{
			if (hasKeyColor)
			{
				if (pixel != keyColor)
				{
					return true;
				}
			}
			else
			{
				if (pixel.a != 0)
				{
					return true;
				}
			}
		}
		return false;
	}
}

class TexInfo
{
	public TextureImporter Importer;
	public int ReadableCount = 1;
	public TexInfo(TextureImporter importer)
	{
		Importer = importer;
	}
}
#endif
