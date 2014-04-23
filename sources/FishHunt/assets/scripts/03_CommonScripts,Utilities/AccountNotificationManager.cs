using System;
using UnityEngine;

public class AccountNotificationManager :LaunchNotificationManager<AccountNotificationManager>, ILaunchNotificationManager
{
	public string nextNotificationLaunchKey__Impl()
	{
		return "AccountNotificationManager_NextNotificationLaunch";
	}
	public int repeatAfterLaunchNumber__Impl()
	{
		return GameConstants.AccountNotificateLaunchNumber;
	}
	
	public override bool CanNotificate__Impl()
	{
		return base.CanNotificate__Impl() && (string.IsNullOrEmpty(User.AccountLoginName) && string.IsNullOrEmpty(User.FacebookId));
	}
}