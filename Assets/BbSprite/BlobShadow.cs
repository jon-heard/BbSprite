
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A monobehavior that adds a circular darkness beneath a gameobject
/// strongly reminiscent of a shadow.
/// </summary>
[SerializeField]
[ExecuteInEditMode]
[AddComponentMenu("BbSprite/BlobShadow")]
// ReSharper disable once CheckNamespace
public class BlobShadow : MonoBehaviour
{
	private const string GameObjectName = "_?_Shadow_?_";

	/// <summary>
	/// The size of the blobshadow.
	/// </summary>
	[Range(0,1)]
	public float BlobSize = .3f;
	/// <summary>
	/// Determines the darkness/density of the blobshadow
	/// </summary>
	[Range(0, 1)]
	public float BlobTransparency = .5f;

	[HideInInspector]
	[SerializeField]
	private Projector _projector;

	private void OnEnable()
	{
    if (_shadowMaterial == null)
    {
      _shadowMaterial = Resources.Load<Material>("Default-Shadow");
      _shadowMaterial.hideFlags = HideFlags.NotEditable;
    }
		Update();
	}

	private void OnDisable()
	{
		if(!enabled)
			RemoveShadow();
	}

	private void Update()
	{
		if (enabled)
		{
			if (_projector == null)
			{
				GetShadow();
			}
			if (_projector != null)
			{
				var scale = transform.lossyScale.x;
				if (_projector.transform.localPosition.y != BlobTransparency)
				{
					_projector.transform.localPosition = new Vector3(0, BlobTransparency, 0);
				}
				// ReSharper disable once RedundantCheckBeforeAssignment
				if (_projector.orthographicSize != BlobSize * scale)
				{
					_projector.orthographicSize = BlobSize * scale;
				}
				// ReSharper disable once RedundantCheckBeforeAssignment
				if (_projector.farClipPlane != 1.5f * scale)
				{
					_projector.farClipPlane = 1.5f*scale;
				}
			}
		}
	}

	private void GetShadow()
	{
		var shadowTransform = transform.FindChild(GameObjectName);
		if (shadowTransform == null)
		{
			RemoveShadow();
			var shadow = new GameObject
			{
				name = GameObjectName,
				hideFlags = HideFlags.HideInHierarchy
			};
			shadowTransform = shadow.transform;
			shadowTransform.eulerAngles = new Vector3(90, 0, 0);
			_projector = shadow.AddComponent<Projector>();
			_projector.nearClipPlane = 0;
			_projector.farClipPlane = 1.5f;
			_projector.orthographic = true;
			_projector.material = _shadowMaterial;
			shadow.transform.parent = transform;
		}
		else
		{
			_projector = shadowTransform.GetComponent<Projector>();
		}
	}

	private void RemoveShadow()
	{
		var toDestroyList = new List<GameObject>();
		var count = transform.childCount;
		for (int i = 0; i < count; i++) {
			var curChild = transform.GetChild(i);
			if (curChild.name == GameObjectName) {
				toDestroyList.Add(curChild.gameObject);
			}
		}
		foreach (var toDestroy in toDestroyList) {
			DestroyImmediate(toDestroy);
		}
		_projector = null;
	}

	private static Material _shadowMaterial;
}
