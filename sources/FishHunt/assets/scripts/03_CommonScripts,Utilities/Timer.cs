using UnityEngine;
using System.Collections;
using System;

enum ETimerState 
{
	kTimerState_Paused,
	kTimerState_Stopped,
	kTimerState_Ended,
	kTimerState_Runned,
	kTimerState_Created,
}
public class Timer
{
	public enum EInitState
	{
		Start,
		Sleep,
	}
	private	string	messageOnTick;
	
	private	int		periodOnTick;
	
	private float 	currPeriodOnTick;
	
	private float	seconds;
	
	private bool isPaused = false;
	
	private bool isStarted = false;
	
	private	bool	isInfinityTimer = false;
	
	float prevRealtimeSinceStartup;
	
	private GameObject parent;
	
	public int	LeftSecond()
	{
		return Mathf.FloorToInt(seconds + 0.5f);
	}
	
	public float LeftTime()
	{
		return seconds;
	}
	
	public void Setup(int period, string message, GameObject _parent, float amountSeconds = -1)
	{
		isInfinityTimer = amountSeconds < 0f;
		seconds = amountSeconds;
		periodOnTick = period;
		currPeriodOnTick = period;
		messageOnTick = message;
		parent = _parent;
		
		prevRealtimeSinceStartup = Time.realtimeSinceStartup;
	}
	
	public Timer(float amountSeconds, GameObject _parent, EInitState initState)
	{
		Setup(1, "OnLeftTimerChange", _parent, amountSeconds);

		if (initState == EInitState.Sleep)
			Sleep();
	}
	
	public Timer(float amountSeconds, int period, string message, GameObject _parent, EInitState initState)
	{
		Setup(period, message, _parent, amountSeconds);

		if (initState == EInitState.Sleep)
			Sleep();
	}
	
	public Timer(int period, string message, GameObject _parent, EInitState initState)
	{
		Setup(period, message, _parent);

		if (initState == EInitState.Sleep)
			Sleep();
	}
	
	public void UpdateTime()
	{
		if (!isStarted)
		{
			isStarted = true;
			parent.BroadcastMessage("OnStartTimer", this, SendMessageOptions.DontRequireReceiver);
		}
		
		if ( !isPaused)
		{
			float deltaTime = Time.realtimeSinceStartup - prevRealtimeSinceStartup;
			prevRealtimeSinceStartup = Time.realtimeSinceStartup;
			
			if (!isInfinityTimer)
				seconds -= deltaTime;
			
			currPeriodOnTick -= deltaTime;
			
			if (seconds > 0 || isInfinityTimer)
			{
				if (currPeriodOnTick <= 0)
				{					
					currPeriodOnTick = periodOnTick - Mathf.Clamp(Mathf.Abs(currPeriodOnTick), 0, periodOnTick);
					parent.BroadcastMessage(messageOnTick, this, SendMessageOptions.DontRequireReceiver);
				}
			}
			else
			{
				Sleep();
				parent.BroadcastMessage("OnTimeEnd", this, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
		
	public void Sleep()
	{
		isPaused = true;
		parent.BroadcastMessage("OnTimerPause", this, SendMessageOptions.DontRequireReceiver);
	}
	
	public void WakeUp()
	{
		prevRealtimeSinceStartup = Time.realtimeSinceStartup;
		currPeriodOnTick = periodOnTick;
		isPaused = false;
		parent.BroadcastMessage("OnTimerWakeUp", this, SendMessageOptions.DontRequireReceiver);
	}
	
	public void Reset(float amountSecond)
	{
		seconds = amountSecond;
		if (isInfinityTimer)
		{
			currPeriodOnTick = amountSecond;
			periodOnTick = Mathf.FloorToInt(amountSecond);
		}
		currPeriodOnTick = periodOnTick;
		parent.BroadcastMessage("OnTimerReset", this, SendMessageOptions.DontRequireReceiver);
	}
	
	public bool IsTimeEnd()
	{
		return seconds <= 0f;
	}
	
	public override string ToString()
	{
		int minute = (int)seconds/60;
		return String.Format("{0:00}",minute) + ":" + String.Format("{0:00}",Mathf.FloorToInt(seconds - minute * 60 + 0.5f));
	}
}
