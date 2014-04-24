using UnityEngine;
using System.Collections;

public class UITextureProgressBar : IgnoreTimeScale 
{	
	public UITexture foreground = null;
	public UITexture background = null;
	public delegate void OnAnimationFinished();
	public OnAnimationFinished onAnimationFinished;

	protected float valuePerSec = 10;
	protected int minValue = 0;
	protected int maxValue = 0;
	protected float currentValue = 0;
	protected int targetValue = 0;
	protected bool mAnimated = false;

	public int Value
	{
		get { return targetValue; }
	}

	public void SetValue(int newValue, float duration = 0.5f, float delay = 0f)
	{
		if (!foreground)
			return;

		newValue = Mathf.Clamp(newValue, MinValue, MaxValue);

		if (duration <= 0f)
		{
			StopCoroutine("valueAnimation");
			mAnimated = false;

			currentValue = newValue;
			targetValue = newValue;

			foreground.fillAmount = Mathf.Abs(currentValue-minValue) / Mathf.Abs(maxValue-minValue);
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


	public int MaxValue 
	{
		get
		{
			return maxValue;
		}
		set
		{
			maxValue = value;
			targetValue = Mathf.Clamp(targetValue,MinValue, MaxValue);
			currentValue = Mathf.Clamp(currentValue,MinValue, MaxValue);
			foreground.fillAmount = Mathf.Abs(maxValue-minValue) != 0f ?Mathf.Abs(currentValue-minValue) / Mathf.Abs(maxValue-minValue) :0f;
		}
	}

	public int MinValue 
	{
		get
		{
			return minValue;
		}
		set
		{
			minValue = value;
			targetValue = Mathf.Clamp(targetValue,MinValue, MaxValue);
			currentValue = Mathf.Clamp(currentValue,MinValue, MaxValue);
			foreground.fillAmount = Mathf.Abs(maxValue-minValue) != 0f ?Mathf.Abs(currentValue-minValue) / Mathf.Abs(maxValue-minValue) :0f;
		}
	}

	/*
	public Vector3 FillPosition
	{
		get
		{
		if (mTexture != null)
			return mTexture.transform.parent.TransformPoint(mTexture.transform.localPosition +(mTexture.invert ?Vector3.left :Vector3.right)*mTexture.transform.localScale.x*Mathf.Clamp01(mTexture.fillAmount) );
		else
			return transform.position;
		}
	}
	*/

	protected IEnumerator valueAnimation(float delay)
	{
		yield return new WaitForSeconds(delay);

		UpdateRealTimeDelta();
		bool completed = false;

		do
		{
			currentValue = currentValue + valuePerSec*UpdateRealTimeDelta();
		
			if ( (valuePerSec < 0 && currentValue < targetValue) || (valuePerSec >= 0 && currentValue >= targetValue))
			{
				currentValue = targetValue;
				completed = true;
			}
			
			foreground.fillAmount = Mathf.Abs(maxValue-minValue) != 0f ?Mathf.Abs(currentValue-minValue) / Mathf.Abs(maxValue-minValue) :0f;

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
