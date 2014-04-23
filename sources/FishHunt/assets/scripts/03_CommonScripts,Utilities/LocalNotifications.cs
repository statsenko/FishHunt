using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using System.Globalization;

public class LocalNotifications : MonoBehaviour
{
	class LocalNotificationDesc
	{
		public string		idEvent;
		public string		message;
		public DateTime 	fireDate;
		
		public LocalNotificationDesc(string id, string mess, DateTime date)
		{
			idEvent = id;
			message = mess;
			fireDate = date;
		}
		
		public LocalNotificationDesc()
		{
			idEvent = "";
			message = "";
		}
	}
	
	public static string IdEvent_RememberAfter24hours = "remember24hours";
	public static string IdEvent_RememberAfter48hours = "remember48hours";
	public static string IdEvent_RememberAfter7Days = "remember7days";
	public static string IdEvent_AllLifesRestored = "lifes";
	
	private List<LocalNotificationDesc>	_allEvents = new List<LocalNotificationDesc>();
	
	private static LocalNotifications mInstance; 	
	public static void Init()
	{
		if (mInstance == null)
		{
            mInstance = new GameObject("_LocalNotifications").AddComponent(typeof(LocalNotifications)) as LocalNotifications;
			
			DontDestroyOnLoad(mInstance.gameObject);
        }
	}
	
	public static void Schedule(DateTime fireDate, string message, string idEvent) 
	{
		if (fireDate < DateTime.Now)
			return;
		
		foreach (LocalNotificationDesc desc in mInstance._allEvents)
		{
			if (desc.idEvent == idEvent)
			{
				mInstance._allEvents.Remove(desc);
				break;
			}
		}
		
		
		mInstance._allEvents.Add(new LocalNotificationDesc(idEvent, message, fireDate));
		
		CancelAllEvents();
		ResetAllEvents();
	}
	
    public static void Schedule(int secondsBefore, string message, string idEvent) 
	{
		if (secondsBefore > 0)
		{
			DateTime fireDate = DateTime.Now.AddSeconds(secondsBefore);
			Schedule(fireDate, message, idEvent);
		}
    }
	
	private static void ResetAllEvents()
	{
		List<LocalNotificationDesc> sortedAllEvents = GetSortedEvents();
				
		for (int i = 0; i < sortedAllEvents.Count; ++i)
		{
			LocalNotification ln = new LocalNotification();
			ln.fireDate = sortedAllEvents[i].fireDate;
			
			ln.alertBody = sortedAllEvents[i].message;
			ln.applicationIconBadgeNumber = i + 1;
			ln.soundName = LocalNotification.defaultSoundName;
			
			Debug.Log("Schedule notification: " + i.ToString() + " event messsage " + sortedAllEvents[i].message + " , date " + sortedAllEvents[i].fireDate.ToString());
			
			NotificationServices.ScheduleLocalNotification(ln);	
		}
	}
	
	public static void CancelAllEvents() 
	{
		NotificationServices.CancelAllLocalNotifications();
		NotificationServices.ClearLocalNotifications();
			
		//удаляем те ивенты время которых истекло
		bool isFound = false;
		DateTime nowDate = DateTime.Now;
		do
		{
			isFound = false;
			foreach (LocalNotificationDesc desc in mInstance._allEvents)
			{
				if (desc.fireDate < nowDate)
				{
					mInstance._allEvents.Remove(desc);
					isFound = true;
					break;
				}
			}
		} while (isFound);
    }
		
	static bool IsNight(DateTime date)
	{
		if (date.Kind == DateTimeKind.Local)
		{
			if (date.Hour >= 21 || date.Hour < 11)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		else
		{
			return false;
		}
	}
	
	static DateTime CreateDateByMorning(DateTime date)
	{
		if (IsNight(date))
		{
			date = date.AddSeconds(60 - date.Second);
			date = date.AddMinutes(60 - date.Minute);
		}
		
		if (date.Hour >= 21)		//в 9 вечера мы уже не хотим получать нотификейшны
			date = date.AddHours(24 - date.Hour + 11);	//до 11 утра след дня
		else
			date = date.AddHours(11 - date.Hour);
		
		return date;
	}
	
	private static List<LocalNotificationDesc> GetSortedEvents()
	{
		List<LocalNotificationDesc> arrEvents = new List<LocalNotificationDesc>(mInstance._allEvents);
		
		int countNightEvents = 0;
		foreach (LocalNotificationDesc desc in arrEvents)
		{
			if (IsNight(desc.fireDate))
				++countNightEvents;
		}
		
		if (countNightEvents > 0)
		{
			LocalNotificationDesc newDesc = new LocalNotificationDesc();
			if (countNightEvents > 1)
				newDesc.message = GameStrings.GetLocalizedString(GameStrings.LocalNotifications_CommonText);
			
			bool isFound = false;
			do
			{
				isFound = false;
				foreach (LocalNotificationDesc desc in arrEvents)
				{
					if (IsNight(desc.fireDate))
					{
						newDesc.fireDate = CreateDateByMorning(desc.fireDate);
						if (countNightEvents == 1)
							newDesc.message = desc.message;
						
						isFound = true;
						arrEvents.Remove(desc);
						break;
					}
				}
			} while (isFound);
			
			arrEvents.Add(newDesc);
		}
		
		arrEvents.Sort(ComparisonLocalNotifications);
		return arrEvents;
	}
	
	static int ComparisonLocalNotifications(LocalNotificationDesc one, LocalNotificationDesc two)
	{
		if (one.fireDate < two.fireDate)
			return -1;
		else
			return 1;
	}
	
	void OnApplicationPause(bool paused)
	{

		if (paused)
		{
			Schedule(User.GetLifesManager.GetTimeWhenAllLifesWillRestored(), GameStrings.GetLocalizedString(GameStrings.LocalNotifications_AllLifesRestored), IdEvent_AllLifesRestored);

			DateTime date = DateTime.Now;
			date = date.AddDays(1);
			Schedule(date, GameStrings.GetLocalizedString(GameStrings.LocalNotifications_LaunchMe24Hours), IdEvent_RememberAfter24hours);

			date = date.AddDays(1);
			Schedule(date, GameStrings.GetLocalizedString(GameStrings.LocalNotifications_LaunchMe48Hours), IdEvent_RememberAfter48hours);

			date = date.AddDays(5);
			Schedule(date, GameStrings.GetLocalizedString(GameStrings.LocalNotifications_LaunchMe7Days), IdEvent_RememberAfter7Days);

		}
		else
		{
			LocalNotification setToZeroBadge = new LocalNotification();
			setToZeroBadge.fireDate = System.DateTime.Now;
			setToZeroBadge.applicationIconBadgeNumber = -1000;
			setToZeroBadge.hasAction = true;
			NotificationServices.PresentLocalNotificationNow(setToZeroBadge);
			
			CancelAllEvents();
		}
	}
}
