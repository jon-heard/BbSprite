
using System;
using UnityEditor;
using UnityEngine;

[Serializable]
// ReSharper disable once CheckNamespace
public class BbSpriteAnimationClip : ScriptableObject
{
	// ReSharper disable InconsistentNaming
	[Range(0,2)]
	public float framerate = 1f;
	public WrapMode wrapMode = WrapMode.Default;
	public int[] frameList = {0};
	// ReSharper restore InconsistentNaming

	public static BbSpriteAnimationClip CreateAnimationClip(
		string name = "clip",
		int[] frameList = null,
		float framerate = 1,
		WrapMode wrapMode = WrapMode.Default)
	{
		if (frameList == null) frameList = new int[] { 0 };
		var result = ScriptableObject.CreateInstance<BbSpriteAnimationClip>();
		result.name = name;
		result.frameList = frameList;
		result.framerate = framerate;
		result.wrapMode = wrapMode;
		return result;
	}

	internal readonly static BbSpriteAnimationClip Idle_3X4, Idle_4X4, Walk_3X4, Walk_4X4;
	static BbSpriteAnimationClip()
	{
		if (Idle_3X4 == null)
		{
			Idle_3X4 = Resources.Load<BbSpriteAnimationClip>("3x4_idle");
			if (Idle_3X4 != null)
			{
				Idle_3X4.hideFlags = HideFlags.NotEditable;
			}
		}
		if (Idle_4X4 == null)
		{
			Idle_4X4 = Resources.Load<BbSpriteAnimationClip>("4x4_idle");
			if (Idle_4X4 != null)
			{
				Idle_4X4.hideFlags = HideFlags.NotEditable;
			}
		}
		if (Walk_3X4 == null)
		{
			Walk_3X4 = Resources.Load<BbSpriteAnimationClip>("3x4_walk");
			if (Walk_3X4 != null)
			{
				Walk_3X4.hideFlags = HideFlags.NotEditable;
			}
		}
		if (Walk_4X4 == null)
		{
			Walk_4X4 = Resources.Load<BbSpriteAnimationClip>("4x4_walk");
			if (Walk_4X4 != null)
			{
				Walk_4X4.hideFlags = HideFlags.NotEditable;
			}
		}

		//if (Idle_3X4 == null) UnityEngine.Debug.Log("I.3.4 invalid");
		//if (Idle_4X4 == null) UnityEngine.Debug.Log("I.4.4 invalid");
		//if (Walk_3X4 == null) UnityEngine.Debug.Log("W.3.4 invalid");
		//if (Walk_4X4 == null) UnityEngine.Debug.Log("W.4.4 invalid");
	}
}
