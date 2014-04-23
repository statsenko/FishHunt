using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public delegate void TimeEventSchedulerDelegate(TimeEvent sender);

public class TimeEvent
{
	public string tag = null;
	public float timeToRise = 0f;
	public float startTime = 0f;

	public Dictionary<string, object> userInfo;
	
	public TimeEventSchedulerDelegate onStart = null;
	public TimeEventSchedulerDelegate onUpdate = null;
	public TimeEventSchedulerDelegate onFinish = null;

	//COMPARATORS
	public bool IsEqualTo(TimeEvent timeEvent)
	{
		// compare time event by their tags
		if(timeEvent== null)
			return false;
		if(string.IsNullOrEmpty(timeEvent.tag))
			return false;
		if(string.Compare(tag,timeEvent.tag)==0)
			return true;
		else
			return false;
	}


	// CONSTRUCTORS
	public TimeEvent(float riseTime, string tagName)
	{

		if(string.IsNullOrEmpty(tagName))
		{
			Debug.LogWarning("The tag name should not be null!");
			return;
		}
		tag = tagName;
		timeToRise =  riseTime;
	}

	public TimeEvent(float riseTime, string tagName, TimeEventSchedulerDelegate onStartAction, TimeEventSchedulerDelegate onUpdateAction, TimeEventSchedulerDelegate onFinishAction, Dictionary<string, object> info)
	{
		if(string.IsNullOrEmpty(tagName))
		{
			Debug.LogWarning("The tag name should not be null!");
			return;
		}
		//TimeEvent(tagName);
		tag = tagName;
		timeToRise =  riseTime;
		onStart = onStartAction;
		onUpdate = onUpdateAction;
		onFinish = onFinishAction;
		userInfo = info;
	}

}

public class TimeEventScheduler : IgnoreTimeScale {
	// IgnoreTimeScale унаследован от MonoBehaviour потому его добавлять к User как AddComponent

	Dictionary<float,TimeEvent> _eventsDict = null;
	Dictionary<float,TimeEvent> EventsDict
	{
		get
		{
			if (_eventsDict==null)
				_eventsDict = new Dictionary<float,TimeEvent>();
			return _eventsDict; 
		}
		set 
		{	_eventsDict = value; }
	}

	public void Clear()
	{
		_eventsDict = new Dictionary<float, TimeEvent>();
		_keysToRemove = new ArrayList();
		_actionsToPerform = new ArrayList ();
	}

	ArrayList _keysToRemove;
	public ArrayList KeysToRemove
	{
		get
		{
			if (_keysToRemove==null)
				_keysToRemove = new ArrayList();
			return _keysToRemove; 
		}
		set 
		{	_keysToRemove = value; }
	}

	ArrayList _actionsToPerform;
	public ArrayList ActionsToPerform
	{
		get
		{
			if (_actionsToPerform==null)
				_actionsToPerform = new ArrayList();
			return _actionsToPerform; 
		}
		set 
		{	_keysToRemove = value; }
	}

	private void Schedule (TimeEvent timeEvent)
	{
		if(timeEvent == null)
		{
			Debug.LogWarning("The time event is not set correctly!");
			return;
		}
		EventsDict[Time.realtimeSinceStartup] =timeEvent; // Time.realtimeSinceStartup is used as a unique key for adding the TimeEvent into the arrayList
		if(timeEvent.onStart != null)
			timeEvent.onStart(timeEvent);
	}

	public void ScheduleTimeEvent(float time, string tag, TimeEventSchedulerDelegate onStartAction, TimeEventSchedulerDelegate onUpdateAction, TimeEventSchedulerDelegate onFinishAction, Dictionary<string, object> userInfo)
	{
		TimeEvent tE = new TimeEvent(time + realTime,  tag, onStartAction, onUpdateAction, onFinishAction, userInfo);
		tE.startTime = realTime;
		if(tE!=null)
			Schedule(tE);
	}

	public void UnscheduleEventWithTag (string timeEventTag)
	{
		foreach(float timeKey in EventsDict.Keys)
		{
			TimeEvent currentEvent = EventsDict[timeKey];
			//TimeEvent currentEvent in eventsDict
			if(string.IsNullOrEmpty(currentEvent.tag) || !timeEventTag.Equals(currentEvent.tag))
				continue;
			else
			{
				//  delete the timeEvent from the dictionary
				KeysToRemove.Add(timeKey);
			}
			return;	
		}
		removeTimeEventsWithSelectedKeys();
	}
	
	public void removeTimeEventsWithSelectedKeys()
	{
		foreach (float key in KeysToRemove)
		{
			if(EventsDict.ContainsKey(key))
			{
				EventsDict.Remove(key);
			}
		}
		KeysToRemove.Clear();
	}

	// Update is called once per frame
	void Update () 
	{
		UpdateRealTimeDelta();
		CheckTheCollectionForEventsToRise();
	}

	public void CheckTheCollectionForEventsToRise()
	{
		foreach(float timeKey in EventsDict.Keys)
		{
			TimeEvent currentEvent;
			EventsDict.TryGetValue(timeKey, out currentEvent);
			if(currentEvent.onUpdate != null)
				currentEvent.onUpdate(currentEvent);
			if(currentEvent.timeToRise>realTime)
				continue;
			else
			{
				// delete the timeEvent from the connection
				KeysToRemove.Add(timeKey);

				if(currentEvent.onFinish != null)
					ActionsToPerform.Add (currentEvent);
			}
		}
		
		if(ActionsToPerform.Count >0)
		{
			foreach (TimeEvent action in ActionsToPerform)
				action.onFinish(action);
			ActionsToPerform.Clear();
		}
		removeTimeEventsWithSelectedKeys();
	}

	// CHECK FOR SCHEDULING METHODS
	public bool IsScheduled(TimeEvent timeEvent)
	{
		if (string.IsNullOrEmpty(timeEvent.tag))
		{
			return false;
		}
		else
			return IsScheduledEventWithTag(timeEvent.tag);
	}

	// CHECK FOR SCHEDULING METHODS
	public bool IsScheduledEventWithTag(string tag)
	{
		if (string.IsNullOrEmpty(tag))
		{
			return false;
		}
		foreach(float timeKey in EventsDict.Keys)
		{
			TimeEvent currentEvent = EventsDict[timeKey];
			if (tag.Equals( currentEvent.tag, StringComparison.Ordinal) && !KeysToRemove.Contains(timeKey))
				return true;
		}
		return false;
	}

	public List<TimeEvent> TimeEventsWithTag(string tag)
	{
		List<TimeEvent> result = new List<TimeEvent>();
		foreach(float timeKey in EventsDict.Keys)
		{
			TimeEvent currentEvent = EventsDict[timeKey];
			if(currentEvent!= null)
			{
				if (tag.Equals( currentEvent.tag, StringComparison.Ordinal) && !KeysToRemove.Contains(timeKey))
					result.Add(currentEvent);
			}
		}
		return result;
	}

	public float RemainingTimeOfEventWithTag(string tag)
	{
		TimeEvent te = null;
		if (string.IsNullOrEmpty(tag))
		{
			return -1;
		}
		foreach(float timeKey in EventsDict.Keys)
		{
			TimeEvent currentEvent = EventsDict[timeKey];
			if (tag.Equals( currentEvent.tag, StringComparison.Ordinal))
				te = currentEvent;
		}
		if(te==null)
			return -1;
		return te.timeToRise-realTime;
	}
}

