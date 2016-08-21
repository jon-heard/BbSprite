
using UnityEngine;

/// <summary>
/// The MonoBehavior that determines how a BbSprite's atlas (texture image)
/// should be broken up into cells. The BbSprite behavior looks to <i>this</i>
/// behavior to help determine what part of the atlas to draw at any given time.
/// </summary>
[AddComponentMenu("BbSprite/Layout")]
[ExecuteInEditMode]
// ReSharper disable once CheckNamespace
public class BbSpriteLayout : MonoBehaviour
{
	/// <summary>
	/// This BbSpriteLayout's type of layout.<br/>
	/// </summary>
	public Type MyType = Type.SingleCell;
	private Type _lastMyType = Type.SingleCell;

	/// <summary>
	/// Holds data for the layout type specified by the MyType field.
	/// For a CustomGrid layout, this has parameters that can used to specify
	/// how the cells are laid out.
	/// </summary>
	public LayoutData MyLayoutData
	{ 
		get { return _myLayoutData; }
		private set { _myLayoutData = value; }
	}
	private LayoutData _myLayoutData = new LayoutData_SingleCell();
	[SerializeField]
	// ReSharper disable once NotAccessedField.Local
	public LayoutData_CustomGrid _myLayoutAsCustomGrid;

	public void Update()
	{
		if (_lastMyType != MyType)
		{
		_lastMyType = MyType;
			RefreshCurrentLayout();

			if (BbSprite.AutoGuessAnimation)
			{
				var layout = GetComponent<BbSpriteLayout>();
				if (layout == null) return;
				var animation = GetComponent<BbSpriteAnimation>();
				if (animation == null) return;

				if (
					layout.MyType == Type.RpgMaker ||
					layout.MyType == Type.RpgMaker2K)
				{
					animation.Setup3X4();
				}
				else if (layout.MyType == Type.FourByFour)
				{
					animation.Setup4X4();
				}
			}
		}

		if (MyLayoutData == null ||
			(MyLayoutData is LayoutData_SingleCell && MyType != Type.SingleCell))
		{
			RefreshCurrentLayout();
		}
	}

	private void Awake()
	{
		_lastMyType = MyType;
	}

	private void RefreshCurrentLayout()
	{
		switch (MyType)
		{
			case Type.SingleCell:
				MyLayoutData = new LayoutData_SingleCell();
				break;
			case Type.RpgMaker:
				MyLayoutData = new LayoutData_RpgMaker();
				break;
			case Type.RpgMaker2K:
				MyLayoutData = new LayoutData_RpgMaker2K();
				break;
			case Type.FourByFour:
				MyLayoutData = new LayoutData_FourByFour();
				break;
			case Type.CustomGrid:
				MyLayoutData = _myLayoutAsCustomGrid = new LayoutData_CustomGrid();
				break;
		}
	}

	/// <summary>
	/// Represents a type of layout for a BbSpriteLayout to be configered to.
	/// <ul>
	/// <li>SingleCell - The atlas is divided into one single cell that encompasses the entire atlas.<br/> @image html SingleCell.png</li>
	/// <li>RpgMaker - Breaks an atlas into 12 cells: 3 cells across by 4 cells down.<br/> @image html RpgMaker.png</li>
	/// <li>RpgMaker2K - Like Rpgmaker, except that the rotational ordering is different.<br/> @image html RpgMaker2K.png</li>
	/// <li>FourByFour - Breaks an atlas into 16 cells: 4 cells across by 4 cells down.<br/> @image html FourByFour.png</li>
	/// <li>CustomGrid - Breaks an atlas into a user defined grid of cells.<br/> @image html CustomGrid.png</li>
	/// </ul>
	/// </summary>
	public enum Type {
		SingleCell,
		RpgMaker,
		RpgMaker2K,
		FourByFour,
		CustomGrid,
	}
}
