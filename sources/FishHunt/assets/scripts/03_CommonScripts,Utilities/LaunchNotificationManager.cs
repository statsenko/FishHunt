using System;
using System.Collections;
using UnityEngine;

public interface ILaunchNotificationManager
{
	//has not base implementation. Should be implemented in derived class:
	string nextNotificationLaunchKey__Impl();
	int repeatAfterLaunchNumber__Impl();
	
	//has base implementation:
	void Disable__Impl();
	void Reset__Impl();
	bool CanNotificate__Impl();
}

public abstract class LaunchNotificationManager<T>  where T : ILaunchNotificationManager, new()
{
	protected static T instance_ = default (T);
	protected static T instance
	{
		get 
		{
			if (instance_ == null)
			{
				instance_ = new T();
			}
			return instance_;
		}
	}
	
	public static void destroy()
	{
		instance_ = default (T);
	}

	/*
	protected static int nextNotificationLaunchNumber
	{
		get 
		{
			string key = User.CharacterName + nextNotificationLaunchKey__();
			if (PlayerPrefs.HasKey(key))
			{
				return PlayerPrefs.GetInt(key);
			}
			else
			{
				int number = LaunchCounter.LaunchCount + repeatAfterLaunchNumber__();
				//call setter
				nextNotificationLaunchNumber = number;
				return number;
			}
		}
		set
		{
			string key = User.CharacterName + nextNotificationLaunchKey__();
			PlayerPrefs.SetInt(key, value);
			PlayerPrefs.Save();
		}
	}
	*/
	
	protected static string nextNotificationLaunchKey__()
	{
		return instance.nextNotificationLaunchKey__Impl();
	}
	
	protected static int repeatAfterLaunchNumber__()
	{
		return instance.repeatAfterLaunchNumber__Impl();
	}
	
	//will disable notification counter 
	public static void Disable__()
	{
		instance.Disable__Impl();
	}
	
	//will reset notification counter 
	public static void Reset__()
	{
		instance.Reset__Impl();
	}
	
	//returns true if notification can be presented
	public static bool CanNotificate__()
	{
		return instance.CanNotificate__Impl();
	}
	
	//base Implementation of interface ILaunchNotificationManager
	public virtual void Disable__Impl()
	{
	//	nextNotificationLaunchNumber = -1;
	}
	
	public virtual void Reset__Impl()
	{
//		nextNotificationLaunchNumber = LaunchCounter.LaunchCount + repeatAfterLaunchNumber__();
	}
	
	public virtual bool CanNotificate__Impl()
	{
		return true; //(nextNotificationLaunchNumber > 0 && nextNotificationLaunchNumber <= LaunchCounter.LaunchCount);
	}
}
