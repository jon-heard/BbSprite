
using System;
using UnityEngine;

/// <summary>
/// This class always calculates the cells based on a grid of <b>1 column
/// and 1 row</b> that spans the entire atlas.
/// <br/> @image html SingleCell.png
/// </summary>
[Serializable]
public class LayoutData_SingleCell : LayoutData
{

	// The number of cells recognized by this layout.
	public override int TotalCellCount { get { return 1; } }

	// See MyLayoutData class for proper documentation here
	public override Rect GetCell(int index)
	{
		return UnitRect;
	}


	public override Vector2 GetCellScale(int index, Texture atlas)
	{
		var aspectRatio = (float)atlas.width / atlas.height;
		if (aspectRatio > 1.0f)
		{
			return new Vector2(1.0f, 1.0f / aspectRatio);
		}
		return new Vector2(aspectRatio, 1.0f);
	}

	public override string ToString() { return "LayoutData_SingleCell"; }

	internal override int AdjustIndexByRotation(int index, float rotation)
	{
		return 0;
	}
}
