using UnityEngine;
using System.Collections;
using System;


public class LifesManager 
{	
	// constant keys
	public static string LifeTimerTag = "LifeTimerTag";
	
	// number of lifes, the player has
	private int _lifesCount = 0;
	public int LifesCount
	{	
		get
		{ 
			return _lifesCount;
		}
		set
		{ 
			_lifesCount= value;
		}
	}

	private int baseLifeTimerPeriod = 0;
	protected int BaseLifeTimerPeriod
	{
		get { return baseLifeTimerPeriod; }
		private set { baseLifeTimerPeriod = value; }
	}

	// the time (in seconds) to request fot new  lifes 
	private int _lifeTimerPeriod = 0;
	protected int LifeTimerPeriod
	{	
		get
		{ 
			return _lifeTimerPeriod;
		}
		set
		{
			_lifeTimerPeriod = value;
		}
	}
	
	private int _currentTimeValue = 0;
	public int CurrentTimeValue
	{	
		get
		{ 
			return _currentTimeValue;
		}
		set
		{
			_currentTimeValue = value;
		}
	}

	// the time (in seconds) to request fot new  lifes 
	private int _maximalNumberOfLifes = 0;
	public int MaximalNumberOfLifes
	{	
		get
		{ 
			return _maximalNumberOfLifes;
		}
	}
	

	/// delegates ///
	public delegate void  OnLifeTimerUpdated();
	public delegate void  OnLifesCountUpdated();

	public OnLifeTimerUpdated onLifeTimerUpdated = null;
	public OnLifesCountUpdated onLifesCountUpdated = null;
	
	void DelegateLifeTimerUpdated()	 { if (onLifeTimerUpdated != null)	onLifeTimerUpdated(); 	}
	void DelegateLifesCountUpdated() { if (onLifesCountUpdated != null)	onLifesCountUpdated(); 	}
	/////////////////////

	public void Clear()
	{
		_lifesCount = 0;
		_lifeTimerPeriod = 0;
		_currentTimeValue = 0;
		_maximalNumberOfLifes = 0;
	}

	void OnTimerTick(TimeEvent timeEvent)
	{
		CurrentTimeValue =  (int)User.EventScheduler.RemainingTimeOfEventWithTag(LifeTimerTag);
		DelegateLifeTimerUpdated();
	}

	void OnTimerFinish(TimeEvent timeEvent)
	{
		if (LifesCount>=MaximalNumberOfLifes)
			return;
		IncrementLifesCount();
	}

	public void IncrementLifesCount ()
	{
		LifesCount++;
		DelegateLifesCountUpdated();

		if (LifesCount < MaximalNumberOfLifes)
		{
			if (!User.EventScheduler.IsScheduledEventWithTag(LifeTimerTag))
				User.EventScheduler.ScheduleTimeEvent(LifeTimerPeriod,LifeTimerTag, null, OnTimerTick, OnTimerFinish, null);
		}
		else
		{
			if (User.EventScheduler.IsScheduledEventWithTag(LifeTimerTag))
				User.EventScheduler.UnscheduleEventWithTag(LifeTimerTag);
		}
	}

	public void	DecreaseLifesTimerPeriod(int seconds)
	{
		if (BaseLifeTimerPeriod > seconds)
			LifeTimerPeriod = BaseLifeTimerPeriod - seconds;
		else
			LifeTimerPeriod = 0;
	}

	public int GetTimeWhenAllLifesWillRestored()
	{
		return (MaximalNumberOfLifes - LifesCount - 1 ) * LifeTimerPeriod + CurrentTimeValue;
	}
}
