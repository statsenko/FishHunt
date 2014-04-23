using System;
using UnityEngine;
public class RRNotificationManager : LaunchNotificationManager<RRNotificationManager>, ILaunchNotificationManager
{
	protected bool userCameFromGameWin = false;
	public static bool IsUserCameFromGameWin
	{
		get
		{
			return instance.userCameFromGameWin;
		}
		set
		{
			instance.userCameFromGameWin = value;
		}
	}
	
	public string nextNotificationLaunchKey__Impl()
	{
		return "RRNotificationManager_NextNotificationLaunch";
	}
	public int repeatAfterLaunchNumber__Impl()
	{
		return GameConstants.RateNotificateLaunchNumber;
	}
	
	public override bool CanNotificate__Impl()
	{
		return base.CanNotificate__Impl() && IsUserCameFromGameWin;
	}
}
