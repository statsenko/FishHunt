using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;



public class UnityExceptionHandler : MonoBehaviour
{
	
	protected static UnityExceptionHandler mInstance = null;
	
	public static void Init()
	{
		if (mInstance == null)
		{
	        mInstance = new GameObject("_UnityExceptionHandler").AddComponent(typeof(UnityExceptionHandler)) as UnityExceptionHandler;
			DontDestroyOnLoad(mInstance.gameObject);
			if (Application.isPlaying)
			{
				Application.RegisterLogCallback(HandleLog);
			}
        }
	}
		
    private static void HandleLog(string logString, string stackTrace, LogType type) 
	{
		Dictionary<string,string> parametersDictionary = new Dictionary<string, string>();
		if (stackTrace!= null)
			parametersDictionary.Add("stacktrace", stackTrace);
		switch(type)
		{
			case LogType.Error:
				parametersDictionary.Add("type", "Error");
//				FlurryBinding.logEventWithParameters(logString,parametersDictionary,false);
				break;
			case LogType.Assert:
				parametersDictionary.Add("type", "Assert");
//				FlurryBinding.logEventWithParameters(logString,parametersDictionary,false);
				break;
			case LogType.Warning:
				// do nothing
				break;
			case LogType.Log:
				// do nothing				
				break;
			case LogType.Exception:
				parametersDictionary.Add("type", "Exception");
//				FlurryBinding.logEventWithParameters(logString,parametersDictionary,false);
				break;
		}
    }
	
}