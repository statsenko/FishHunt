using UnityEngine;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System;

public class UICounter :IgnoreTimeScale
{
	public UILabel mLabel = null;
	public	string	stringFormat = "{0}";
	public NumberFormatInfo numberFormat = null;
	public delegate void OnAnimationFinished();
	public OnAnimationFinished onAnimationFinished;

	protected double valuePerSec = 25;
	protected double currentValue = 0;
	public int targetValue = 0;
	protected bool mAnimated = false;

	public void SetCount(int newValue, float duration = 0.75f, float delay = 0f)
	{
		if (!mLabel)
			return;
		if (duration <= 0f)
		{
			StopCoroutine("valueAnimation");
			mAnimated = false;
			currentValue = newValue;
			targetValue = newValue;
			string valueString = (numberFormat != null) ?newValue.ToString("n", numberFormat) :newValue.ToString();
			mLabel.text = string.Format(stringFormat, valueString);
		}
		else
		{
			StopCoroutine("valueAnimation");
			targetValue = newValue;
			valuePerSec = (targetValue-currentValue)/duration;
			mAnimated = true;
			StartCoroutine("valueAnimation", delay);
		}
	}
	
	IEnumerator valueAnimation(float delay)
	{
		yield return new WaitForSeconds(delay);
		bool completed = false;
		UpdateRealTimeDelta();
		do
		{
			currentValue = currentValue + valuePerSec*UpdateRealTimeDelta();

			int currentValueInt = Convert.ToInt32(currentValue);
		
			if ( (valuePerSec < 0 && currentValueInt < targetValue) || (valuePerSec >= 0 && currentValueInt >= targetValue))
			{
				currentValue = targetValue;
				currentValueInt = targetValue;
				completed = true;
			}

			string valueString = (numberFormat != null) ?currentValueInt.ToString("n", numberFormat) :currentValueInt.ToString();
			mLabel.text = string.Format(stringFormat, valueString);

			if (!completed)
				yield return null;

		}while(!completed);

		mAnimated = false;
		if (onAnimationFinished != null)
			onAnimationFinished();
	}

	public bool IsAnimated()
	{
		return mAnimated;
	}
}