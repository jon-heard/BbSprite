
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class Window_AtlasPreview : EditorWindow
{
	private const int MaxInitialSize = 400;
	private readonly static Color RulerColor = new Color(0.87f, .67f, .39f);
	private const int RulerWidth = 25;
	private const int RulerTickHeight = 5;
	private const int Extra = 10;
	private static GUIStyle _style_rulerTop;
	private static GUIStyle _style_rulerLeft;
	private static GUIStyle _style_cell;
	private static GUIStyle _style_ruler;

	static Window_AtlasPreview()
	{
		CreateStyles();
	}

	public static bool IsOpened { get; private set; }

	public static void ShowWindow()
	{
		var window = GetWindow<Window_AtlasPreview>("Atlas preview");
		window.Show();
	}

	public static void HideWindow()
	{
		var window = GetWindow<Window_AtlasPreview>("Atlas preview");
		window.Close();
	}


	private void OnEnable() {
		CreateStyles();
		var sceneViewPos = GetWindow(typeof(SceneView)).position;
		var window = GetWindow<Window_AtlasPreview>("Atlas preview");
		var atlas = Editor_BbSpriteLayout.MyAtlas;
		var windowSize = new Vector2(100, 100);
		if (atlas != null)
		{
			var size = new Vector2(atlas.width, atlas.height);
			while (size.x < MaxInitialSize && size.y < MaxInitialSize)
			{
				size.x *= 2;
				size.y *= 2;
			}
			windowSize = new Vector2(size.x + RulerWidth, size.y + RulerWidth);
		}
		window.position = new Rect(
			sceneViewPos.x + (sceneViewPos.width - windowSize.x) / 2.0f,
			sceneViewPos.y + (sceneViewPos.height - windowSize.y) / 2.0f,
			windowSize.x,
			windowSize.y);
		IsOpened = true;
	}

	private void OnDisable()
	{
		IsOpened = false;
		if (Editor_BbSpriteLayout.Thiss != null)
		{
			Editor_BbSpriteLayout.Thiss.Repaint();
		}
	}

	private void Update()
	{
		Repaint();
	}

	public void OnGUI()
	{
		var target = Editor_BbSpriteLayout.MyTarget;
		var atlas = Editor_BbSpriteLayout.MyAtlas;
		if (target != null && atlas != null && target.MyLayoutData != null)
		{
			var layout = target.MyLayoutData;

			// Keep aspect ratio
			var winPos = position;
			float scale = Mathf.Min(
				winPos.width / atlas.width,
				winPos.height / atlas.height
			);
			winPos.width = atlas.width * scale - RulerWidth - Extra;
			winPos.height = atlas.height * scale - RulerWidth - Extra;

			// Draw image
			GUI.DrawTexture(new Rect(RulerWidth, RulerWidth, winPos.width, winPos.height), atlas);

			// Draw grid & cell numbers
			var randomColor = new Color(Random.value,Random.value,Random.value,1);
			GUI.contentColor = randomColor;
			GUI.backgroundColor = randomColor;
			var cellCount = layout.TotalCellCount;
			for (var i = 0; i < cellCount; i++)
			{
				var cellPosition = layout.GetCell(i);
				cellPosition.y = 1 - cellPosition.y - cellPosition.height;
				cellPosition.x *= winPos.width;
				cellPosition.y *= winPos.height;
				cellPosition.x += RulerWidth;
				cellPosition.y += RulerWidth;
				cellPosition.width *= winPos.width;
				cellPosition.height *= winPos.height;
				GUI.Box(cellPosition, " " + i, _style_cell);
			}

			// Draw rulers
			GUI.contentColor = Color.black;
			GUI.backgroundColor = Color.white;
			GUI.Box(new Rect(0, 0, RulerWidth, position.height), "", _style_ruler);
			GUI.Box(new Rect(0, 0, position.width, RulerWidth), "", _style_ruler);

			var pos = new Vector2(RulerWidth + winPos.width*0, RulerWidth - RulerTickHeight);
				GUI.Label(new Rect(pos.x-25, pos.y-50, 50, 50), "0", _style_rulerTop);
				GUI.Box(new Rect(pos.x, pos.y, 1, RulerTickHeight), "");
			pos.x = RulerWidth + winPos.width*1;
				GUI.Label(new Rect(pos.x-25, pos.y-50, 50, 50), "1", _style_rulerTop);
				GUI.Box(new Rect(pos.x, pos.y, 1, RulerTickHeight), "");
			pos.x = RulerWidth + winPos.width * .5f;
				GUI.Label(new Rect(pos.x-25, pos.y-50, 50, 50), ".5", _style_rulerTop);
				GUI.Box(new Rect(pos.x, pos.y, 1, RulerTickHeight), "");
			pos.x = RulerWidth + winPos.width * .25f;
				GUI.Label(new Rect(pos.x-25, pos.y-50, 50, 50), ".25", _style_rulerTop);
				GUI.Box(new Rect(pos.x, pos.y, 1, RulerTickHeight), "");
			pos.x = RulerWidth + winPos.width * .75f;
				GUI.Label(new Rect(pos.x-25, pos.y-50, 50, 50), ".75", _style_rulerTop);
				GUI.Box(new Rect(pos.x, pos.y, 1, RulerTickHeight), "");
			pos.x = RulerWidth + winPos.width * .33333f;
				GUI.Label(new Rect(pos.x-25, pos.y-50, 50, 50), ".33", _style_rulerTop);
				GUI.Box(new Rect(pos.x, pos.y, 1, RulerTickHeight), "");
			pos.x = RulerWidth + winPos.width * .66666f;
				GUI.Label(new Rect(pos.x-25, pos.y-50, 50, 50), ".66", _style_rulerTop);
				GUI.Box(new Rect(pos.x, pos.y, 1, RulerTickHeight), "");

			pos = new Vector2(RulerWidth - RulerTickHeight, RulerWidth + winPos.height * 0);
				GUI.Label(new Rect(pos.x-50, pos.y-25, 50, 50), "0", _style_rulerLeft);
				GUI.Box(new Rect(pos.x, pos.y, RulerTickHeight, 1), "");
			pos = new Vector2(RulerWidth - RulerTickHeight, RulerWidth + winPos.height * 1);
				GUI.Label(new Rect(pos.x-50, pos.y-25, 50, 50), "1", _style_rulerLeft);
				GUI.Box(new Rect(pos.x, pos.y, RulerTickHeight, 1), "");
			pos = new Vector2(RulerWidth - RulerTickHeight, RulerWidth + winPos.height * .5f);
				GUI.Label(new Rect(pos.x-50, pos.y-25, 50, 50), ".5", _style_rulerLeft);
				GUI.Box(new Rect(pos.x, pos.y, RulerTickHeight, 1), "");
			pos = new Vector2(RulerWidth - RulerTickHeight, RulerWidth + winPos.height * .25f);
				GUI.Label(new Rect(pos.x-50, pos.y-25, 50, 50), ".25", _style_rulerLeft);
				GUI.Box(new Rect(pos.x, pos.y, RulerTickHeight, 1), "");
			pos = new Vector2(RulerWidth - RulerTickHeight, RulerWidth + winPos.height * .75f);
				GUI.Label(new Rect(pos.x-50, pos.y-25, 50, 50), ".75", _style_rulerLeft);
				GUI.Box(new Rect(pos.x, pos.y, RulerTickHeight, 1), "");
			pos = new Vector2(RulerWidth - RulerTickHeight, RulerWidth + winPos.height * .33333f);
				GUI.Label(new Rect(pos.x-50, pos.y-25, 50, 50), ".33", _style_rulerLeft);
				GUI.Box(new Rect(pos.x, pos.y, RulerTickHeight, 1), "");
			pos = new Vector2(RulerWidth - RulerTickHeight, RulerWidth + winPos.height * .66666f);
				GUI.Label(new Rect(pos.x-50, pos.y-25, 50, 50), ".66", _style_rulerLeft);
				GUI.Box(new Rect(pos.x, pos.y, RulerTickHeight, 1), "");
		}
	}

	private static void CreateStyles()
	{
		if (_style_rulerTop == null)
		{
			_style_rulerTop = new GUIStyle { alignment = TextAnchor.LowerCenter };
		}
		if (_style_rulerLeft == null)
		{
			_style_rulerLeft = new GUIStyle { alignment = TextAnchor.MiddleRight };
		}
		if (_style_cell == null)
		{
			var background = new Texture2D(3, 3, TextureFormat.RGBA32, false)
			{
				hideFlags = HideFlags.DontSave,
				filterMode = FilterMode.Point
			};
			var border = Color.white;
			var back = new Color(0, 0, 0, 0);
			background.SetPixels(new Color[] {
				border, border, border,
				border, border, border,
				border, border, border });
			background.SetPixel(1, 1, back);
			background.Apply();

			_style_cell = new GUIStyle
			{
				normal = {background = background},
				border = new RectOffset(1, 1, 1, 1)
			};
			_style_cell.normal.textColor = Color.white;
		}
		if (_style_ruler == null)
		{
			var background = new Texture2D(3, 3, TextureFormat.RGBA32, false)
			{
				hideFlags = HideFlags.DontSave,
				filterMode = FilterMode.Point
			};
			background.SetPixels(new Color[] {
				RulerColor, RulerColor, RulerColor,
				RulerColor, RulerColor, RulerColor,
				RulerColor, RulerColor, RulerColor });
			background.Apply();

			_style_ruler = new GUIStyle
			{
				normal = {background = background},
				border = new RectOffset(1, 1, 1, 1)
			};
			_style_ruler.normal.textColor = Color.black;
		}
	}
}
