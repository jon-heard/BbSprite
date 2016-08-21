
using System;
using UnityEngine;

/// <summary>
/// This layout allows for a customized layout based on breaking an atlas
/// into a contiguous grid of cells.
/// <br/> @image html CustomGrid.png
/// </summary>
[Serializable]
public class LayoutData_CustomGrid : LayoutData
{
	//public Vector2 CellCountOld = new Vector2(3,4);
	/// <summary>
	///   The number of columns and rows of cells in the grid recognized by this
	///   layout.
	/// </summary>
	public UIntVector2 CellCount = new UIntVector2(3, 4);
	/// <summary>
	///   The area of the grid relative to the entire atlas.
	/// </summary>
	public Rect Area = new Rect(0, 0, 1, 1);
	/// <summary>
	///   Determines how the cells are counted: column first or row first.
	///   True - Cell indices are ordered by column first, then row.<br />
	///   False - Cell indices are ordered by row first, then column. (like
	///   reading).<br />
	///   True is detault as, while it is less intuitive, it makes it <i>much</i>
	///   easier to add new cells without needing to change frames of animations.
	/// </summary>
	public bool VerticalIndexing = true;
	/// <summary>
	///   The list of frame offsets for viewing angles in clockwise order.
	///   The full rotation is broken up into segments assigned to each offset
	///   starting at 0 degrees being at the center of the first offset.
	///   <i>
	///     Example: "0,2,3,1" maps these angle ranges to frame offsets:<br />
	///     - 0-45 degrees = 0 frame offset<br />
	///     - 45-135 degrees = 2 frame offset<br />
	///     - 135-225 degrees = 3 frame offset<br />
	///     - 225-315 degrees = 1 frame offset<br />
	///     - 315-360 degrees = 0 frame offset<br />
	///   </i>
	/// </summary>
	public int[] DirectionOffsetList = { 0, 2, 3, 1 };
	/// <summary>
	/// Allows editing the direction offset list as a comma delimited string.
	/// </summary>
	public string DirectionOffsetString
	{
		get { return IntArrayStringHelper.IntArrayToString(DirectionOffsetList); }
		set
		{
			if (value != DirectionOffsetString)
			{
				try
				{
					DirectionOffsetList = IntArrayStringHelper.StringToIntArray(value, false);
				}
				catch (ArgumentException) { }
			}
		}
	}


	// The number of cells recognized by this layout.
	public override int TotalCellCount
	{
		get { return CellCount.x * CellCount.y; }
	}

	// See MyLayoutData class for proper documentation here
	public override Rect GetCell(int index)
	{
		// Make sure index is in bounds
		if (Math.Abs(index) >= TotalCellCount)
		{
			throw new IndexOutOfRangeException();
		}
		// Adjust for negative index
		bool flipped = index < 0;
		index = Mathf.Abs(index);
		// Calculate cell bounds
		var cellSize = new Vector2(
			Area.width / CellCount.x,
			Area.height / CellCount.y);
		var cellOffset = Vector2.zero;
		if (VerticalIndexing)
		{
			// ReSharper disable once RedundantCast
			cellOffset.x = (int)(index / CellCount.y);
			cellOffset.y = index % CellCount.y;
		}
		else
		{
			cellOffset.x = index % CellCount.x;
			// ReSharper disable once RedundantCast
			cellOffset.y = (int)(index / CellCount.x);
		}
		cellOffset.x = cellOffset.x * cellSize.x + Area.x;
		cellOffset.y = 1 - cellOffset.y * cellSize.y - Area.y - cellSize.y;

		if (flipped)
		{
			return new Rect
				(cellOffset.x + cellSize.x, cellOffset.y, -cellSize.x, cellSize.y);
		}
		return new Rect(cellOffset.x, cellOffset.y, cellSize.x, cellSize.y);
	}


	// See MyLayoutData class for proper documentation here
	public override Vector2 GetCellScale(int index, Texture atlas)
	{
		var cellSize = new Vector2(
			Area.width / CellCount.x,
			Area.height / CellCount.y);
		float aspectRatio = cellSize.x / cellSize.y * atlas.width / atlas.height;
		if (aspectRatio > 1)
		{
			return new Vector2(1, 1 / aspectRatio);
		}
		return new Vector2(aspectRatio, 1);
	}

	public override string ToString() { return "LayoutData_CustomGrid"; }

	// Used by "GetCell" and "getCellScale" to add index offset based
	// on rotation
	internal override int AdjustIndexByRotation(int index, float rotation)
	{
		// Find direction offset
		rotation = (rotation + 360) % 360;
		int count = DirectionOffsetList.Length;
		int segHalf = 180 / count;
		var segment = (int)Mathf.Ceil((int)(rotation / segHalf) / 2.0f);
		if (segment == count)
		{
			segment = 0;
		}
		int directionOffset = DirectionOffsetList[segment];
		// Add directionOffset to index (accounting for negatives)
		bool flipped = (index < 0);
		if (directionOffset < 0)
		{
			flipped = !flipped;
		}
		index = Mathf.Abs(index) + Mathf.Abs(directionOffset);
		return flipped ? -index : index;
	}
}

[Serializable]
public class UIntVector2
{
	[SerializeField]
	// ReSharper disable once InconsistentNaming
	public int x;
	[SerializeField]
	// ReSharper disable once InconsistentNaming
	public int y;

	public UIntVector2(int x, int y)
	{
		this.x = x;
		this.y = y;
	}
	// ReSharper disable once InconsistentNaming
	public float magnitude { get { return new Vector2(x, y).magnitude; } }
}
