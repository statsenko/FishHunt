using UnityEngine;
using System;
using System.Collections;

public class LaunchCounter : MonoBehaviour
{
	private static LaunchCounter mInstance = null;
	
	public static LaunchCounter Instance
	{
		get
		{
			if (mInstance == null)
				LaunchCounter.Init();
			
			return mInstance;
		}
	}
	
	public static void Init()
	{
		if (mInstance == null)
		{
            mInstance = new GameObject("_LaunchCounter").AddComponent(typeof(LaunchCounter)) as LaunchCounter;
			
			DontDestroyOnLoad(mInstance.gameObject);
			
//!!!			mInstance.launchCount = PlayerPrefs.GetInt(User.CharacterName + "_countExitFromBg", 0);
			mInstance.launchCount++;
        }
	}
	
	public static void Destroy()
	{
		if (mInstance != null)
		{
			Destroy(mInstance.gameObject);
			mInstance = null;
		}
	}
	
	public static int LaunchCount
	{
		get {return Instance.launchCount;}
	}
	
	public int launchCount__ = 0;
	protected int launchCount
	{
		get {return launchCount__;}
		set 
		{
			launchCount__ = value;
//!!!		PlayerPrefs.SetInt(User.CharacterName + "_countExitFromBg", launchCount__);
			PlayerPrefs.Save();
		}
	}
	
	void OnApplicationPause(bool paused)
	{
		if (!paused)
		{
			launchCount++;
		}
	}
}