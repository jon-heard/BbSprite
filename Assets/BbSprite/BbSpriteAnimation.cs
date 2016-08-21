
using System;
using System.Collections.Generic;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

// ReSharper disable once CheckNamespace
[AddComponentMenu("BbSprite/Animation")]
public class BbSpriteAnimation : MonoBehaviour
{
	/// Inspector visible fields
	// ReSharper disable InconsistentNaming
	public BbSpriteAnimationClip clip;
	// ReSharper disable once FieldCanBeMadeReadOnly.Local
	[SerializeField]
	private List<BbSpriteAnimationClip> animations = new List<BbSpriteAnimationClip>();
	public bool playAutomatically = true;
	public WrapMode wrapMode = WrapMode.Default;
	// ReSharper restore InconsistentNaming

	public int CurrentCell
	{
		get
		{
			return _runner.CurrentCell;
		}
	}

	public int GetClipCount()
	{
		if (animations == null) return 0;
		return animations.Count;
	}

	public bool Play()
	{
		if (clip == null) return false;
		if (_runner.Clip == clip) return true;
		_clipQueue.Clear();
		_runner.Clip = clip;
		return true;
	}
	public bool Play(string pName)
	{
		if (!_animationsByName.ContainsKey(pName)) return false;
		if (_runner.Clip == _animationsByName[pName]) return true;
		_clipQueue.Clear();
		_runner.Clip = _animationsByName[pName];
		return true;
	}

	public bool PlayQueued(string pName)
	{
		if (!_animationsByName.ContainsKey(pName)) return false;
		var curAnim = _animationsByName[pName];
		_clipQueue.Enqueue(curAnim);
		return true;
	}

	public bool Stop()
	{
		if (_runner.Clip == null) return false;
		_runner.Clip = _clipQueue.Count > 0 ? _clipQueue.Dequeue() : null;
		return true;
	}

	public void Rewind()
	{
		_runner.Rewind();
	}

	public bool IsPlaying(string pName)
	{
		if (!_animationsByName.ContainsKey(pName)) return false;
		if (_runner.Clip != _animationsByName[pName]) return false;
		return true;
	}

	public void AddClip(BbSpriteAnimationClip pClip, string pName)
	{
		animations.Add(pClip);
		_animationsCheck.Add(pClip);
		_animationsByName[pName] = pClip;
	}

	public void RemoveClip(BbSpriteAnimationClip pClip)
	{
		animations.Remove(pClip);
		_animationsCheck.Remove(pClip);
		foreach (var entry in _animationsByName)
		{
			if (entry.Value == pClip)
			{
				_animationsByName.Remove(entry.Key);
				break;
			}
		}
	}

	public void Update()
	{
		if (Application.isEditor)
		{
			_runner.Clip = playAutomatically ? clip : null;
		}
		_runner.Update();
		if (_runner.IsDone)
		{
			Stop();
		}
	}

	public void SyncDictionary()
	{
		var synced = true;
		if (_animationsCheck.Count != animations.Count) synced = false;
		else
		{
			for (int i = 0; i < animations.Count; i++)
			{
				if (_animationsCheck[i] != animations[i])
				{
					synced = false;
					break;
				}
			}
		}
		if (!synced)
		{
			Debug.Assert(_animationsCheck != null, "_animationsCheck != null");
			_animationsCheck.Clear();
			_animationsByName.Clear();
			foreach (var anim in animations)
			{
				_animationsCheck.Add(anim);
				if(anim != null)
					_animationsByName.Add(anim.name, anim);
			}
		}
	}

	public void Setup3X4()
	{
		animations.Clear();
		_animationsCheck.Clear();
		var clip_idle = BbSpriteAnimationClip.CreateAnimationClip(
			"idle",
			new int[] { 4 }
		);
		animations.Add(clip_idle);
		var clip_walk = BbSpriteAnimationClip.CreateAnimationClip(
			"walk",
			new int[] { 0, 4, 8, 4 },
			.2f,
			WrapMode.Loop
		);
		animations.Add(clip_walk);
		clip = clip_walk;
		SyncDictionary();
	}

	public void Setup4X4()
	{
		animations.Clear();
		_animationsCheck.Clear();
		var clip_idle = BbSpriteAnimationClip.CreateAnimationClip(
			"idle",
			new int[] { 0 }
		);
		animations.Add(clip_idle);
		var clip_walk = BbSpriteAnimationClip.CreateAnimationClip(
			"walk",
			new int[] { 4, 8, 12, 8 },
			.2f,
			WrapMode.Loop
		);
		animations.Add(clip_walk);
		clip = clip_walk;
		SyncDictionary();
	}

	// We need to be able to refer to animations by name for the "Play" method.
	private readonly Dictionary<string, BbSpriteAnimationClip> _animationsByName = new Dictionary<string, BbSpriteAnimationClip>();
	// This allows us to check the "animations" list for changes
	private readonly List<BbSpriteAnimationClip> _animationsCheck = new List<BbSpriteAnimationClip>();
	// This is the animation queue that is run through while playing
	private readonly Queue<BbSpriteAnimationClip> _clipQueue = new Queue<BbSpriteAnimationClip>();
	// Keeps track of the current animation running
	private readonly BbSpriteAnimationRunner _runner = new BbSpriteAnimationRunner();

	private void Start()
	{
		SyncDictionary();
	}
}




public enum WrapMode { Default, Loop }




internal class BbSpriteAnimationRunner
{
	// The clip being animated
	public BbSpriteAnimationClip Clip
	{
		get { return _clip; }
		set
		{
			if (_clip != value)
			{
				_clip = value;
				Rewind();
			}
		}
	}
	private BbSpriteAnimationClip _clip;

	// The cell the current clip is pointing to
	public int CurrentCell 
	{
		get
		{
			if (Clip == null) return 0;
			return Clip.frameList[_curFrame];
		}
	}

	// Returns true if the current clip is done animating, false otherwise.
	public bool IsDone
	{
		get
		{
			if (Clip == null) return true;
			if(Clip.wrapMode == WrapMode.Loop) return false;
			if (_curFrame < Clip.frameList.Length) return false;
			return true;
		}
	}

	public void Update()
	{
		if (Clip == null) return;
		// Frame step
		var currentTick = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
		_counter += (currentTick - _previousTick);
		_previousTick = currentTick;
		if (_counter > (Clip.framerate * 1000))
		{
			_counter = 0;
			_curFrame++;
			if (_curFrame >= Clip.frameList.Length)
			{
				switch (Clip.wrapMode)
				{
					case WrapMode.Default:
						_curFrame = Clip.frameList.Length - 1;
						break;
					case WrapMode.Loop:
						_curFrame = 0;
						break;
				}
			}
		}
	}

	public void Rewind()
	{
		_curFrame = 0;
		_counter = 0;
		_previousTick = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
	}

	private int _curFrame;
	private long _counter;
	private long _previousTick;
}
