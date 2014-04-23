using UnityEngine;
using System.Collections;

public class Counter :MonoBehaviour
{
	public OnUpdateCounterValue onUpdateCounterValue = null;
	public OnCompleteCounter onCompleteCounter = null;
	public OnStartCounter onStartCounter = null;

	protected double startCount = 0f;
	protected double targetCount = 0f;
	protected double currentCount = 0f;
	protected double oldCount = 0f;
	protected double countPerSecond = 0f;
	protected bool autoDestruct = false;
	
	public float Delta
	{
		get { return (float)(currentCount-oldCount);}
	}
	public float CurrentCount
	{
		get { return (float)currentCount;}
	}
	public float NormalizedDelta
	{
		get { return Delta/(float)(targetCount-startCount);}
	}

	public delegate void OnUpdateCounterValue(Counter sender);
	public delegate void OnCompleteCounter(Counter sender);
	public delegate void OnStartCounter(Counter sender);

	public static Counter Create()
	{
		GameObject newCounter = new GameObject("_counter");
		Counter counter = newCounter.AddComponent<Counter>() as Counter;
		return counter;
	}
	
	
	public void PrepareCount(double startCount_, double endCount_, float duration_, bool autoDestruct_)
	{
		StopCoroutine("Count");
		autoDestruct = autoDestruct_;
		targetCount = endCount_;
		startCount = startCount_;
		currentCount = startCount;
		oldCount = currentCount;
		countPerSecond = (targetCount - currentCount)/duration_;
		
		if (onUpdateCounterValue != null)
			onUpdateCounterValue(this);
	}
	
	public void Stop(float delay = 0f)
	{
		StartCoroutine("StopCount", delay);
	}
	public void Play(float delay)
	{
		StopCoroutine("StopCount");
		StartCoroutine("Count", delay);
	}
	
	IEnumerator StopCount(float delay = 0f)
	{
		if (delay > 0f)
			yield return new WaitForSeconds(delay);
		StopCoroutine("Count");
	}
	
	IEnumerator Count(float delay)
	{
		if (delay > 0f)
			yield return new WaitForSeconds(delay);
		
		if (onStartCounter != null)
			onStartCounter(this);
		
		bool completed = false;
		while ( !completed)
		{
			oldCount = currentCount;
			currentCount += countPerSecond*Time.deltaTime;
			if ((countPerSecond >= 0f && currentCount >= targetCount) || (countPerSecond < 0f && currentCount<=targetCount))
			{
				currentCount = targetCount;
				completed = true;
			}
			
			if (onUpdateCounterValue != null)
			{
				onUpdateCounterValue(this);
			}
			yield return null;
		}
		
		if (onCompleteCounter != null)
			onCompleteCounter(this);
		if (autoDestruct)
			Destroy(gameObject);
	}
}
