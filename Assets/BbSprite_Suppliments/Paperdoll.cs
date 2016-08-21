
using UnityEngine;

[ExecuteInEditMode]
public class Paperdoll : MonoBehaviour
{
	public Texture2D[] layers;
	public bool refresh = false;

	private Texture2D final;

	public void Update()
	{
		if(refresh)
		{
			refresh = false;
			refreshOutput();
		}
	}

	public void refreshOutput()
	{
		if(layers.Length < 0 || layers[0] == null)
		{
			final = new Texture2D(0,0);
		}
		else
		{
			final = CombineTextures(layers[0], layers[0]);
			for(int i = 1; i < layers.Length; i++)
			{
				if(layers[i] != null)
				{
					final = CombineTextures(final, layers[i]);
				}
			}
		}
		this.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", final);
	}

	private Texture2D CombineTextures(Texture2D aBaseTexture, Texture2D aToCopyTexture) {
		int aWidth = aBaseTexture.width;
		int aHeight = aBaseTexture.height;

		Texture2D aReturnTexture = new Texture2D(aWidth, aHeight, TextureFormat.RGBA32, false);
		aReturnTexture.filterMode = FilterMode.Point;

		Color[] aBaseTexturePixels = aBaseTexture.GetPixels();
		Color[] aCopyTexturePixels = aToCopyTexture.GetPixels();
		Color[] aColorList = new Color[aBaseTexturePixels.Length];

		int aPixelLength = aBaseTexturePixels.Length;

		for (int p = 0; p < aPixelLength; p++) {
			aColorList[p] = Color.Lerp(aBaseTexturePixels[p], aCopyTexturePixels[p], aCopyTexturePixels[p].a);
		}

		aReturnTexture.SetPixels(aColorList);
		aReturnTexture.Apply(false);

		return aReturnTexture;
	}
}
