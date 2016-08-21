
using System;
using UnityEngine;

/// <summary>
/// This class always calculates the cells based on a grid of <b>3 columns
/// and 4 rows</b> that spans the entire atlas.  The cells determined by
/// the sprite's rotation are aligned to fit a standard LayoutData_RpgMaker sprite
/// (except for LayoutData_RpgMaker 2000, for which you should use the LayoutData_RpgMaker2K layout).
/// <br/> @image html RpgMaker.png
/// </summary>
[Serializable]
public class LayoutData_RpgMaker : LayoutData
{

	// The number of cells recognized by this layout.
	public override int TotalCellCount { get { return 12; } }

	// See MyLayoutData class for proper documentation here
	public override Rect GetCell(int index)
	{
		return new Rect(
			// ReSharper disable once RedundantCast
			(int)(index / 4) * .33333f,
			1 - (index % 4) * .25f - .25f,
			.33333f, .25f);
	}


	public override Vector2 GetCellScale(int index, Texture atlas)
	{
		var aspectRatio = 1.33333f * atlas.width / atlas.height;
		if (aspectRatio > 1.0f)
		{
			return new Vector2(1.0f, 1.0f / aspectRatio);
		}
		return new Vector2(aspectRatio, 1.0f);
	}

	public override string ToString() { return "LayoutData_RpgMaker"; }

	internal override int AdjustIndexByRotation(int index, float rotation)
	{
		rotation = (rotation + 360) % 360;
		if (rotation > 45 && rotation <= 135)	// on right side
		{
			return index + 2;
		}
		if (rotation > 135 && rotation <= 225)	// on front side
		{
			return index + 3;
		}
		if (rotation > 225 && rotation <= 315)	// on left side
		{
			return index + 1;
		}
		return index;
	}
}
