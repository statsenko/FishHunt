using UnityEngine;
using System.Collections;

public class UIProgressCountBar : MonoBehaviour 
{
	public UISprite background = null;
	public UISprite foreground = null;
	
	protected int count = 0;
	protected int maxCount = 0;
	protected bool mNeedUpdate = false;
	// Use this for initialization
	void Awake ()
	{
		Count = 0;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (mNeedUpdate)
		{
			if (foreground!=null && background!=null)
			{
				float bgOriginalWidth = new Rect(background.GetAtlasSprite().outer).width;
				float fgOriginalWidth = new Rect(foreground.GetAtlasSprite().outer).width;
				
				background.transform.localScale = new Vector3(bgOriginalWidth*MaxCount, background.transform.localScale.y, 1f);
				foreground.transform.localScale = new Vector3(fgOriginalWidth*Count, foreground.transform.localScale.y, 1f);
			}
			mNeedUpdate = false;
		}
	}
	
	public int Count
	{
		set 
		{
			count = Mathf.Clamp( value, 0, MaxCount ) ;
			mNeedUpdate = true;
		}
		get 
		{
			return count;
		}
	}
	
	public int MaxCount
	{
		set 
		{
			maxCount = value;
			mNeedUpdate = true;
		}
		get 
		{
			return maxCount;
		}
	}
}
