
using UnityEditor;
using UnityEngine;

/// <summary>
/// The Primary MonoBehavior for a BbSprite.  It's GameObject is rendered as a
/// billboarded sprite based on the current material texture.
/// If the GameObject contains BbSpriteLayout and/or BbSpriteAnimation then
/// these are referenced to determine what part of the Atlas (material texture)
/// is rendered to the billboard at any given time.
/// </summary>
[AddComponentMenu("BbSprite/BbSprite")]
[ExecuteInEditMode]
// ReSharper disable once CheckNamespace
public class BbSprite : MonoBehaviour
{
	private const float OrientSize = .25f;

	/// <summary>
	/// Adjust where the BbSprite is drawn vertically.  Useful for sprites
	/// who's feet don't touch the ground as they are drawn.<br/>
	/// @image html VerticalOffset.png
	/// </summary>
	[Range(-1, 1)] public float VerticalOffset;
	private float _previousVerticalOffset;

	/// <summary>
	/// Determines how the billboard rotates to face the camera.
	/// </summary>
	public FacingType MyFacingType = FacingType.AboutY;
	private FacingType _previousMyFacingType = FacingType.AboutY;

	// The image currently being rendered by this BbSprite. 
	// This is lets us recognize when the texture has changed so that the
	// sprite can adjust to the new texture. It also makes it easier to access
	// the Atlas directly, which is why it is public
	public Texture Atlas { get; private set; }
	private Texture _previousAtlas;

	public void Update()
	{
		// Get latest texture
		if (
            GetComponent<Renderer>() != null &&
            GetComponent<Renderer>().sharedMaterial != null)
		{
			Atlas =
                GetComponent<Renderer>().sharedMaterial.GetTexture("_MainTex");
			// If received new image, make sure it's uptimized and stuff
			if (_previousAtlas != Atlas)
			{
				PrepNewTexture();
			}
		}

		// Get new cell bounds and scaling from layout and animationSet
		LayoutData curLayoutData = DefaultLayoutData;
		var layoutComponent = GetComponent<BbSpriteLayout>();
		if (layoutComponent != null && layoutComponent.enabled)
		{
			curLayoutData = layoutComponent.MyLayoutData;
		}
		int currentCell = 0;
		var animation = GetComponent<BbSpriteAnimation>();
		if (animation != null && animation.enabled)
		{
			currentCell = animation.CurrentCell;
		}
		_cellBounds = curLayoutData.GetCell(
            currentCell,
            (_spriteToCamRotation.y - transform.eulerAngles.y + 360) % 360);
		_scale = curLayoutData.GetCellScale(currentCell, Atlas);

		// Check for reason to update the mesh
		var dirtyMesh = false;
		if (_previousAtlas != Atlas)
		{
			_previousAtlas = Atlas;
			dirtyMesh = true;
		}
		if (_previousYRotation != transform.eulerAngles.y)
		{
			_previousYRotation = transform.eulerAngles.y;
			dirtyMesh = true;
		}
		if (_previousCellBounds != _cellBounds)
		{
			_previousCellBounds = _cellBounds;
			dirtyMesh = true;
		}
		if (_previousMyFacingType != MyFacingType)
		{
			_previousMyFacingType = MyFacingType;
			dirtyMesh = true;
		}
		if (_previousScale != _scale)
		{
			_previousScale = _scale;
			var cCollider = GetComponent<CapsuleCollider>();
			if (cCollider != null)
			{
				cCollider.center = new Vector3(0, _scale.y/2, 0);
				cCollider.radius = _scale.x/2;
				cCollider.height = _scale.y;
			}
			dirtyMesh = true;
		}
		if (_previousSpriteToCamRotation != _spriteToCamRotation)
		{
			_previousSpriteToCamRotation = _spriteToCamRotation;
			dirtyMesh = true;
		}
		if (_previousVerticalOffset != VerticalOffset)
		{
			_previousVerticalOffset = VerticalOffset;
			dirtyMesh = true;
		}
		if (dirtyMesh)
		{
			UpdateMesh();
		}
	}


	// The name given to new BbSprite meshes.
	private const string MeshName = "BbSprite mesh";
	// Most sprites are NOT designed to be perfect squares.  This stores the
	// sprites width to height ratio for scaling purposes.
	private Vector2 _scale = new Vector2(1, 1);
	private Vector2 _previousScale = new Vector2(1, 1);
	// Stores the rotation the sprite needs to face the cam.
	private Vector3 _spriteToCamRotation;
	private Vector3 _previousSpriteToCamRotation;
	// The area of the spritesheet currently being displayed on the BbSprite
	private Rect _cellBounds;
	private Rect _previousCellBounds;
	// We need to check for changes in Y rotation to update mesh
	private float _previousYRotation;
	// Set to refer to THIS BbSprite object.  See Awake() method for details.
	private BbSprite _selfReference;

	// If no layout is available, revert to this layout
	private static readonly LayoutData DefaultLayoutData =
        new LayoutData_SingleCell();

	// True = an images is automatically optimized when added to a BbSprite
	public static bool AutoOptimizeImages = true;
	// True = BbSprite's transparency system is guessed when an image is added
	public static bool AutoGuessTransparency = true;
	// True = BbSprite layout type is guessed when an image is added
	public static bool AutoGuessLayout = true;
	// True = BbSprite AnimationClip type is guessed when layout type is picked
	public static bool AutoGuessAnimation = true;
	// What color the orientation pointer is drawn with
	public static Color OrientPtrColor = Color.blue;
	// If true, all orientation pointers are drawn at runtime
	public static bool OrientPtrAtRun = false;

	// Lets us get the mesh directly, while handling inaccessability issues.
	private Mesh MyMesh
	{
		get
		{
			var filter = GetComponent<MeshFilter>();
			if (filter == null) return null;
			Mesh result = filter.sharedMesh;
			if (result == null || result.name != MeshName)
			{
				filter.hideFlags = HideFlags.HideAndDontSave;
				result = filter.sharedMesh = CreateBbSpriteMesh();
				filter.hideFlags = HideFlags.None;
			}
			return result;
		}
	}

	// _selfReference checked to detect duplication (will not equal 'this').
	// Allows us to avoid same Mesh instance in duplicated BbSprites.
	private void Awake()
	{
		if (_selfReference != this)
		{
			var filter = GetComponent<MeshFilter>();
			if (filter != null)
			{
				filter.mesh = CreateBbSpriteMesh();
			}
		}
		_selfReference = this;
		if (
            GetComponent<Renderer>() != null &&
            GetComponent<Renderer>().sharedMaterial != null)
		{
			_previousAtlas = Atlas =
                GetComponent<Renderer>().sharedMaterial.GetTexture("_MainTex");
		}
	}

	// Called right before each camera render.  Allows adjusting billboard
	// rotation to face whatever camera is being rendered.
	private void OnWillRenderObject()
	{
		// Make sure we have a camera to rotate towards
		Camera cam = Camera.current;
		if (cam != null)
		{
			// Rotate the sprite (by setting _spriteToCamRotation var)
			_spriteToCamRotation = Quaternion.LookRotation
				(transform.position - cam.transform.position).eulerAngles;
			// If we're in edit mode, we need to manually call Update()
			if (!Application.isPlaying)
			{
				Update();
			}
		}
	}

	// Called with new atlas.  Guesses the appropriate transparency type for
    // the atlas and select the shader based on this.
	private void PrepNewTexture()
	{
		if (!AutoOptimizeImages && !AutoGuessTransparency && !AutoGuessLayout)
            return;

		// Grab values to check against
		var atlas = Atlas as Texture2D;
		if (atlas == null)
		{
			return;
		}
		if (GetComponent<Renderer>() == null)
		{
			return;
		}
		Material material = GetComponent<Renderer>().sharedMaterial;
		if (material == null)
		{
			return;
		}

		TextureSettings.AddTemporaryReadableToTexture(atlas);

		// Optimize texture properties
		if (AutoOptimizeImages)
		{
			TextureSettings.SetAtlasAsOptimalForBbSprite(Atlas);
		}
		// Determine which shader to use
		if (AutoGuessTransparency)
		{
			if (atlas.GetPixel(0, 0).a != 1)
			{
				material.shader = Shader.Find("Sprites/BbSprites/Alpha");
			}
			else
			{
				Shader defaultColorShader =
                    Shader.Find("Sprites/BbSprites/ColorKey_UL");
				if (
					material.shader != defaultColorShader &&
					material.shader !=
                        Shader.Find("Sprites/BbSprites/ColorKey_UR") &&
					material.shader !=
                        Shader.Find("Sprites/BbSprites/ColorKey_LL") &&
					material.shader !=
                        Shader.Find("Sprites/BbSprites/ColorKey_LR") &&
					material.shader !=
                        Shader.Find("Sprites/BbSprites/ColorKey_Custom"))
				{
					material.shader = defaultColorShader;
				}
			}
		}
		// Determine which layout type to use
		if (AutoGuessLayout)
		{
			var layout = GetComponent<BbSpriteLayout>();
			if (layout == null) return;
			var width = atlas.width;
			var hasKeyColor =
                (material.shader != Shader.Find("Sprites/BbSprites/Alpha"));
			// check for rpgmaker
			if (
				!TextureSettings.CheckTextureColumnForOpacity(
                    atlas, (int) (width*.33333f), hasKeyColor) &&
				!TextureSettings.CheckTextureColumnForOpacity(
                    atlas, (int) (width*.66666f), hasKeyColor)
				)
			{
				layout.MyType = BbSpriteLayout.Type.RpgMaker;
				layout.Update();
			}
				// check for 4 by 4
			else if (
				!TextureSettings.CheckTextureColumnForOpacity(
                    atlas, (int) (width*.5f), hasKeyColor) &&
				!TextureSettings.CheckTextureColumnForOpacity(
                    atlas, (int) (width*.25f), hasKeyColor) &&
				!TextureSettings.CheckTextureColumnForOpacity(
                    atlas, (int) (width*.75f), hasKeyColor)
				)
			{
				layout.MyType = BbSpriteLayout.Type.FourByFour;
				layout.Update();
			}
				// If failed other checks, is single cell
			else
			{
				layout.MyType = BbSpriteLayout.Type.SingleCell;
				layout.Update();
			}
		}

		TextureSettings.RemoveTemporaryReadableFromTexture(atlas);
	}


	// Called to create a Mesh object for a BbSprite.
	private static Mesh CreateBbSpriteMesh()
	{
		var result = new Mesh();
		result.MarkDynamic();
		result.hideFlags = HideFlags.HideAndDontSave;
		result.name = MeshName;
		result.vertices = new[]
		{
			new Vector3(.5f, 0, 0), new Vector3(-.5f, 1, 0),
			new Vector3(-.5f, 0, 0), new Vector3(.5f, 1, 0)
		};
		result.triangles = new[]
		{
			1, 3, 2, 3, 0, 2
		};
		result.uv = new[]
		{
			new Vector2(1, 0), new Vector2(0, 1),
			new Vector2(0, 0), new Vector2(1, 1)
		};
		result.RecalculateNormals();
		result.RecalculateBounds();
		return result;
	}

	// The mesh needs to be dynamic to handle billboard orientation, cell
	// bounds and transform rotation.  This is called to recalculate the mesh.
	// Reasons this is called include:
	// - Cellbounds - Current cell has changed
	// - spritetocamrotation - camera or sprite has moved
	// - Vertical offset - Vertical offset changed
	// - Update - Source image has changed
	// - Optimize - Image settings being changed, including size
	// - OnInspectorGUI - vertoffset or facingtype changed or undo
	// - MyFacingType - facing type has changed
	// - layout type or grid params
	//  Not yet implemented
	// - transform rotation
	private void UpdateMesh()
	{
		// Get the mesh to update.
		var mesh = MyMesh;
		if (mesh == null)
		{
			return;
		}

		// Vars
		Quaternion qRot = Quaternion.identity;
		float yOffset = 0.0f;
		// Orient based on facingType
		switch (MyFacingType)
		{
			case FacingType.AboutY:
				qRot = Quaternion.Euler(0, _spriteToCamRotation.y -
                    transform.eulerAngles.y, 0);
				break;
			case FacingType.AboutAll:
				qRot = Quaternion.Euler(_spriteToCamRotation);
				yOffset -= .5f;
				break;
		}
		// Adjustment matrix
		Matrix4x4 m = Matrix4x4.TRS(
			new Vector3(0, VerticalOffset, 0),
			qRot,
			new Vector3(_scale.x, _scale.y, 1));
		// Redo mesh vertices
		Vector3[] vertices = mesh.vertices;
		vertices[0] = m*new Vector4(+.5f, 0 + yOffset, 0, 1);
		vertices[1] = m*new Vector4(-.5f, 1 + yOffset, 0, 1);
		vertices[2] = m*new Vector4(-.5f, 0 + yOffset, 0, 1);
		vertices[3] = m*new Vector4(+.5f, 1 + yOffset, 0, 1);
		mesh.vertices = vertices;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		// Redo mesh uv's based on cell bounds
		mesh.uv = new[]
		{
			new Vector2(_cellBounds.x + _cellBounds.width, _cellBounds.y),
			new Vector2(_cellBounds.x, _cellBounds.y + _cellBounds.height),
			new Vector2(_cellBounds.x, _cellBounds.y),
			new Vector2(
				_cellBounds.x + _cellBounds.width,
				_cellBounds.y + _cellBounds.height)
		};
		SceneView.RepaintAll();
	}

    private void OnRenderObject()
    {
        if (OrientPtrAtRun) DrawOrientationPointer();
    }
	private void OnDrawGizmosSelected() { DrawOrientationPointer();}
	private void DrawOrientationPointer()
	{
		float goScale = transform.lossyScale.x;
		float size = OrientSize * goScale;
		Vector2 bbScale = _scale / 2 * goScale;
		float dsx = bbScale.x + size * 1.5f;
		float gs = size / 2;
		GL.InvalidateState();
		GL.PushMatrix();
		GL.modelview = GL.modelview * Matrix4x4.TRS(
            transform.position + new Vector3(0, bbScale.y, 0),
            transform.rotation,
            new Vector3(1, 1, 1));
		GL.Begin(GL.LINES);
		GL.Color(OrientPtrColor);
		// Point
//      GL.Vertex3(0, 0, 0);
//      GL.Vertex3(1, 1, 1);
//		GL.Vertex3(0, 0, dsx);
//		GL.Vertex3(gs, gs, bbScale.x);
//		GL.Vertex3(0, 0, dsx);
//		GL.Vertex3(-gs, gs, bbScale.x);
//		GL.Vertex3(0, 0, dsx);
//		GL.Vertex3(-gs, -gs, bbScale.x);
//		GL.Vertex3(0, 0, dsx);
//		GL.Vertex3(gs, -gs, bbScale.x);
//		// Base
//		GL.Vertex3(gs, gs, bbScale.x);
//		GL.Vertex3(-gs, gs, bbScale.x);
//		GL.Vertex3(gs, -gs, bbScale.x);
//		GL.Vertex3(-gs, -gs, bbScale.x);
//		GL.Vertex3(-gs, gs, bbScale.x);
//		GL.Vertex3(-gs, -gs, bbScale.x);
//		GL.Vertex3(gs, gs, bbScale.x);
//		GL.Vertex3(gs, -gs, bbScale.x);
		GL.End();
		GL.PopMatrix();
	}

	/// <summary>
	/// Determines the kind of billboard rotation on a given BbSprite.
	/// <ul>
	/// <li>AboutY - The BbSprite rotates to face the camera only about the
	/// Y axis.  This is useful for characters, trees and other things that
	/// sit upon the ground.<br/> @image html AboutY.png</li>
	/// <li>AboutAll - The BbSprite rotates to face the camera on all axes.
	/// This is useful for orbs, explosions and other things that look the
	/// same regardless of the view angle.<br/> @image html AboutAll.png</li>
	/// <li>AboutNone - The BbSprite does not rotate to face the camera at
	/// all.  This is useful for doors, fences and other things that have very
	/// little thickness compared to width and height.
	/// <br/> @image html AboutNone.png</li>
	/// </ul>
	/// </summary>
	public enum FacingType
	{
		AboutY, // characters, trees
		AboutAll, // orbs, explosions
		AboutNone // doors, fences
	}
}
