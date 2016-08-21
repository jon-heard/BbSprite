
using System;
using UnityEngine;

/// <summary>
/// The Base class of layout types for BbSpriteLayout. These Layouts are used
/// to define how a texture image of sprites (an atlas) is broken up into
/// individual areas (cells) to be drawn by a BbSprite.
/// </summary>
[Serializable]
public abstract class LayoutData
{
	// The number of cells recognized by this layout.
	public abstract int TotalCellCount { get; }

	// Returns the bounding rectangle of the given cell altered by the given
	// rotation.  See GetCell(int index) for more info.
	// <param name="index">The index of the cell to get the bounds for</param>
	// <param name="rotation">The rotational angle of the BbSprite in degrees.
	// This is used to adjust the index.</param>
	// <returns>A rect storing the bounds of the given cell</returns>
	public Rect GetCell(int index, float rotation)
	{
		index = AdjustIndexByRotation(index, rotation);
		if (index < 0 || index >= TotalCellCount)
		{
			return UnitRect;
		}
		return GetCell(index);
	}

	// Returns the bounding rectangle of the given cell.  The returned values
	// are relative to the atlas, so should all be between 0 and 1.
	// <param name="index">The index of the cell to get the bounds for</param>
	// <returns>A rect storing:<br/>
	// <br/><B>x,y</B> Offset from the upper left corner of the atlas<br/>
	// <b>width,height</b> Width and height of the cell relative to the atlas
	// </returns>
	public abstract Rect GetCell(int index);


	// Called to get any adjustments to scale due to aspect ratio
	public abstract Vector2 GetCellScale(int index, Texture atlas);

	public override string ToString() { return "LayoutData"; }

	// Called by Layouts expecting 4 directions to get rotational frame offset
	internal abstract int AdjustIndexByRotation(int index, float rotation);

	protected static readonly Rect UnitRect = new Rect(0, 0, 1, 1);
}
