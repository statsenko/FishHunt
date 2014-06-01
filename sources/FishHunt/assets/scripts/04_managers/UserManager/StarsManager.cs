using UnityEngine;
using System.Collections;
using System;

public class StarsManager 
{	
	private int _starsCount = 0;
	public int StarsCount
	{	
		get	{ return _starsCount; }
		set
		{
			_starsCount = value; 
			DelegateStarsCountUpdated();
		}
	}

	public delegate void  OnStarsCountUpdated();
	public OnStarsCountUpdated onStarsCountUpdated = null;

	void DelegateStarsCountUpdated() 
	{
		if (onStarsCountUpdated != null)	
			onStarsCountUpdated();
	}

	public void Clear()
	{
		_starsCount = 0;
	}
	
}
