using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(UISprite))]

public class UISpriteAnimationExt : MonoBehaviour
{
	public string[] frameNames = null;
	public int FPS = 30;
	public bool loop = true;
	public float minDelay = 0f;
	public float maxDelay = 1f;
	
	protected UISprite mSprite;
	protected float mDelta = 0f;
	protected float mDelay = 0f;
	protected int mIndex = 0;
	protected bool mActive = true;
	protected List<string> mSpriteNames = new List<string>();

	public int frames { get { return mSpriteNames.Count; } }

	public bool isPlaying { get { return mActive; } }

	void Start () { RebuildSpriteList(); }

	void Update ()
	{
		if (mActive && mSpriteNames.Count > 1 && Application.isPlaying && FPS > 0f)
		{
			if (loop && mDelay > 0f)
			{
				mDelay -= Time.deltaTime;
			}
			else
			{
				float rate = 1f / FPS;
				mDelta += Time.deltaTime;
				if (rate < mDelta)
				{
					mDelta = (rate > 0f) ? mDelta - rate : 0f;
					if (++mIndex >= mSpriteNames.Count)
					{
						mIndex = 0;
						mActive = loop;
						mDelay = Random.Range(minDelay, maxDelay);
					}
					
					if (mActive && mDelay <= 0f)
					{
						string spriteName = mSpriteNames[mIndex];
						if (!string.IsNullOrEmpty(spriteName))
						{
							mSprite.alpha = 1.0f;
							mSprite.spriteName = spriteName;
							mSprite.MakePixelPerfect();
						}
						else
							mSprite.alpha = 0f;
						
					}
				}
			}
		}
	}

	protected virtual void RebuildSpriteList ()
	{
		if (mSprite == null) mSprite = GetComponent<UISprite>();
		mSpriteNames.Clear();
		
		if (frameNames != null && frameNames.Length > 0)
		{
			if (mSprite != null)
				mSpriteNames.AddRange(frameNames);
		}
	}

	public virtual void Reset()
	{
		RebuildSpriteList();
		mActive = true;
		mIndex = 0;
		mDelay = Random.Range(minDelay, maxDelay);
		
		if (mSprite != null && mSpriteNames.Count > 0)
		{
			if (string.IsNullOrEmpty(mSpriteNames[mIndex]))
				mSprite.alpha = 0f;
			else
			{
				mSprite.alpha = 1f;
				mSprite.spriteName = mSpriteNames[mIndex];
				mSprite.MakePixelPerfect();
			}
		}
	}
}
