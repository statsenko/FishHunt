using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;


public class iSmartNewsBridge : MonoBehaviour {

#if UNITY_IPHONE
	[DllImport("__Internal")] 
	private static extern void _initWithUrlString( string urlString );


	[DllImport("__Internal")]
	private static extern void _iSmartNews_ApplicationWillEnterForegroundAction();
#endif //UNITY_IPHONE

	private static iSmartNewsBridge mInstance;

	/*******************************
	possible values of userChoice from iSmartNews.m file:
		"NothingWasPressed" 
		"CancelWasPressed"
		"ActionWasPressed" 
	*******************************/
	public static string userChoice;

	public static void Init()
	{
		if (mInstance == null)
		{
			mInstance = new GameObject("_iSmartNewsBridge").AddComponent(typeof(iSmartNewsBridge)) as iSmartNewsBridge;
			userChoice = string.Empty;
			DontDestroyOnLoad(mInstance.gameObject);
			#if UNITY_IPHONE && !UNITY_EDITOR
			string urlString = GameConstants.iSmartNewsUrlString;
			_initWithUrlString(urlString);
			#endif //UNITY_IPHONE
		}
	}

	void OnApplicationPause(bool paused)
	{
		if (!paused)
		{
			// Application came back to the fore; double-check authentication
			#if UNITY_IPHONE && !UNITY_EDITOR
			_iSmartNews_ApplicationWillEnterForegroundAction();
			#endif //UNITY_IPHONE
		}
	}

	void OnUserChoice(string _userChoice)
	{
		#if UNITY_IPHONE && !UNITY_EDITOR
		Debug.Log("OnUserChoice " + userChoice);
		userChoice = _userChoice;

		// IMPLEMENT SOME NAVIGATION LOGIC THERE

		#endif //UNITY_IPHONE
	}
}
