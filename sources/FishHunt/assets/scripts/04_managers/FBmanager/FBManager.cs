using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Facebook;
using Facebook.MiniJSON;

public class FBManager : MonoBehaviour 
{
	public delegate void FBManagerDelegate (FBResult result);

	private static FBManager mInstance = null;

	private string permissions = "email,publish_actions,user_games_activity, friends_relationship_details";
	public static FBManager Instance 
	{
		get 
		{
			if (mInstance == null)
			{
				GameObject go = new GameObject("_FBManager");
				mInstance = go.AddComponent(typeof(FBManager)) as FBManager;
				DontDestroyOnLoad(mInstance);
				Reset();
			}
			return mInstance;
		}
	}

	private Dictionary<FacebookDelegate, FBManagerDelegate> callbacks = null;

	private bool isInit = false;
	private WWWForm formWithScreenshot = null;
	private WWWForm formWithLevelUp = null;

	public static bool IsInit
	{
		get { return Instance.isInit; }
	}

	public static void Reset()
	{
		Instance.callbacks = new Dictionary<FacebookDelegate, FBManagerDelegate>();
	}

	public static void Init()
	{
		if ( !IsInit)
			FB.Init(Instance.OnInitComplete);
	}

	public static void Logout ()
	{
		FB.Logout();
		Instance.isInit = false;
	}

	public static bool IsLoggedIn
	{
		get { return FB.IsLoggedIn; }
	}

	private void OnInitComplete()
	{
		Debug.Log("FB.Init completed: Is user logged in? " + FB.IsLoggedIn);
		isInit = true;

		//OnFBRegister(null);
	}

	string[] urls = {"http://ntgamesltd.com/luckychef/achievements/start.html",
		"http://ntgamesltd.com/luckychef/achievements/levelup05.html", 
		"http://ntgamesltd.com/luckychef/achievements/levelup10.html", 
		"http://ntgamesltd.com/luckychef/achievements/levelup15.html", 
		"http://ntgamesltd.com/luckychef/achievements/levelup20.html", 
		"http://ntgamesltd.com/luckychef/achievements/levelup25.html", 
		"http://ntgamesltd.com/luckychef/achievements/levelup30.html", 
		"http://ntgamesltd.com/luckychef/achievements/levelup35.html", 
		"http://ntgamesltd.com/luckychef/achievements/levelup40.html", 
		"http://ntgamesltd.com/luckychef/achievements/levelup45.html", 
		"http://ntgamesltd.com/luckychef/achievements/levelup50.html", 
		"http://ntgamesltd.com/luckychef/achievements/levelup55.html", 
		"http://ntgamesltd.com/luckychef/achievements/levelup60.html", 
		"http://ntgamesltd.com/luckychef/achievements/levelup65.html", 
		"http://ntgamesltd.com/luckychef/achievements/levelup70.html", 
		"http://ntgamesltd.com/luckychef/achievements/levelup75.html", 
		"http://ntgamesltd.com/luckychef/achievements/levelup80.html", 
		"http://ntgamesltd.com/luckychef/achievements/levelup85.html", 
		"http://ntgamesltd.com/luckychef/achievements/levelup90.html", 
		"http://ntgamesltd.com/luckychef/achievements/levelup95.html", 
		"http://ntgamesltd.com/luckychef/achievements/levelup100.html", 
		"http://ntgamesltd.com/luckychef/achievements/levelup110.html", 
		"http://ntgamesltd.com/luckychef/achievements/levelup120.html", 
		"http://ntgamesltd.com/luckychef/achievements/boss01.html", 
		"http://ntgamesltd.com/luckychef/achievements/boss02.html", 
		"http://ntgamesltd.com/luckychef/achievements/boss03.html", 
		"http://ntgamesltd.com/luckychef/achievements/boss04.html", 
		"http://ntgamesltd.com/luckychef/achievements/boss05.html", 
		"http://ntgamesltd.com/luckychef/achievements/boss06.html", 
		"http://ntgamesltd.com/luckychef/achievements/boss07.html", 
		"http://ntgamesltd.com/luckychef/achievements/boss08.html", 
		"http://ntgamesltd.com/luckychef/achievements/cafe02.html",
		"http://ntgamesltd.com/luckychef/achievements/cafe03.html",
		"http://ntgamesltd.com/luckychef/achievements/cafe04.html",
		"http://ntgamesltd.com/luckychef/achievements/cafe05.html",
		"http://ntgamesltd.com/luckychef/achievements/cafe06.html",
		"http://ntgamesltd.com/luckychef/achievements/cafe07.html",
		"http://ntgamesltd.com/luckychef/achievements/cafe08.html"};

	int currIndex = 0;

	void OnFBRegister(FBResult result)
	{
		if (result != null)
		{
			if (result.Error != null)
				Debug.LogError("ERROR: FBManager: " + result.Error);
		}

		if (currIndex < urls.Length)
		{
			var dict = new Dictionary<string,string>();
			dict["achievement"] = urls[currIndex];
			dict["access_token"] = "232729623540531|f4a3a8e75cb5668945a33b402f5aee68";
			
			string url2 = "/" + FB.AppId + "/achievements";

			FB.API(url2, Facebook.HttpMethod.POST, Instance.OnFBRegister, dict );
			++currIndex;
		}
	}


	public static void RemoveDelegate(FBManagerDelegate callback)
	{
		foreach (FacebookDelegate del in Instance.callbacks.Keys)
		{
			if (Instance.callbacks[del] == callback)
			{
				Instance.callbacks.Remove(del);
				break;
			}
		}
	}

	public static void PostScreenshot(Texture2D tex, FBManagerDelegate callback = null)
	{
		byte[] screenshot = tex.EncodeToPNG();
		
		WWWForm form = new WWWForm();
		form.AddBinaryData("image", screenshot);
		form.AddField("message", GameStrings.GetLocalizedString(GameStrings.FB_ShareScreenshot_message));

		if (callback != null)
			Instance.callbacks[Instance.OnFBSendScreenshot] = callback;

		if (FB.IsLoggedIn)
		{
			FB.API("me/photos", Facebook.HttpMethod.POST, Instance.OnFBSendScreenshot, form);

			SendToFlurryAboutScreenshot();
		}
		else
		{
			Instance.formWithScreenshot = form;
			FB.Login(mInstance.permissions, Instance.OnFBLoginBeforePostScreenshotEvent);
		}
	}

	public static void PostAchievment(string achievmentId)
	{
		if (FB.IsLoggedIn)
		{
			var dict = new Dictionary<string,string>();
			dict["achievement"] = achievmentId;
			dict["access_token"] = FB.AccessToken;

			string url2 = "/" + FB.UserId + "/achievements";

			FB.API(url2, Facebook.HttpMethod.POST, Instance.OnPostAchievement, dict );
		}
	}

	public static void GetAllAchievements()
	{
		if (FB.IsLoggedIn)
		{
			var dict = new Dictionary<string,string>();
			dict["access_token"] = FB.AccessToken;

			string url2 = "/" + FB.UserId + "/achievements";
			FB.API(url2, Facebook.HttpMethod.GET, Instance.OnGetAllAchievements);	//версия без передачи словаря в парамере возвращает список всех достижений!!!!!!!!!!!!!!!!!!!!!!!
			
		}
	}

	public static void PostToFeed(string pictureURL, string message)
	{
		WWWForm form = new WWWForm();
		form.AddField("message", message);
		form.AddField("picture", pictureURL);
		form.AddField("link", GameConstants.UpgradeBuildURL);

		if (FB.IsLoggedIn)
		{	
			FB.API("me/feed", Facebook.HttpMethod.POST, Instance.OnFBSendToFeed, form);
		}
		else
		{
			Instance.formWithLevelUp = form;
			FB.Login(mInstance.permissions, Instance.OnFBLoginBeforePostToFeedEvent);
		}
	}
	
	public static void Login(FBManagerDelegate callback = null)
	{
		if (callback != null)
			Instance.callbacks[Instance.OnFBLoginEvent] = callback;

		FB.Login(mInstance.permissions, Instance.OnFBLoginEvent);
	}

	public static void API(string query, HttpMethod method, FBManagerDelegate callback = null, Dictionary<string, string> formData = null)
	{
		if (callback != null)
			Instance.callbacks[Instance.OnFBApiEvent] = callback;

		FB.API(query, method, Instance.OnFBApiEvent, formData);
	}

	void OnFBLoginEvent(FBResult result)
	{
		CallAndRemove(OnFBLoginEvent, result);
	}

	void OnFBApiEvent(FBResult result)
	{
		CallAndRemove(OnFBApiEvent, result);
	}

	void OnFBSendScreenshot(FBResult result)
	{
		CallAndRemove(OnFBSendScreenshot, result);
	}

	void OnFBSendToFeed(FBResult result)
	{
		if (result.Error != null)
			if ( !result.Error.Contains("400"))
				Debug.LogError("ERROR: FBManager: " + result.Error);
	}
	
	void OnFBLoginBeforePostScreenshotEvent( FBResult result )
	{
		if (result.Error != null)
			CallAndRemove(OnFBSendScreenshot, result);
		else
		{
			FB.API("me/photos", Facebook.HttpMethod.POST,  Instance.OnFBSendScreenshot, formWithScreenshot);

			SendToFlurryAboutScreenshot();
		}
	}

	static void SendToFlurryAboutScreenshot()
	{
//		var eventParams = new Dictionary<string, string>();
//		eventParams.Add("level" , User.GetExpManager.LevelCount.ToString());
//		FlurryBinding.logEventWithParameters(GameConstants.FlurryEvent_ScreenshotSend, eventParams, false);
	}

	void OnFBLoginBeforePostToFeedEvent( FBResult result )
	{
		if (result.Error == null)
		{
			FB.API("me/feed", Facebook.HttpMethod.POST, Instance.OnFBSendToFeed, formWithLevelUp);
		}
		else
			if ( !result.Error.Contains("400"))
				Debug.LogError("ERROR: FBManager: " + result.Error);
	}

	void CallAndRemove(FacebookDelegate fbDelegate, FBResult result)
	{
		if (callbacks.ContainsKey(fbDelegate))
		{
			FBManagerDelegate callback = callbacks[fbDelegate];
			if (callback != null)
			{
				callback(result);
				RemoveDelegate(callback);
			}
		}
	}

	void OnPostAchievement(FBResult result)
	{
		if (result.Error != null)
			if ( !result.Error.Contains("400"))
				Debug.LogError("ERROR: FBManager: " + result.Error);
	}

	void OnGetAllAchievements(FBResult result)
	{
		if (result.Error != null)
			Debug.LogError("ERROR: FBManager: " + result.Error);
		else
		{
			if (result.Text.Length != 0)
			{		
				var ht = Json.Deserialize(result.Text) as Dictionary<string,object>;
			
				if (ht != null)
				{
					foreach (string key in ht.Keys)
					{
						Debug.Log("key= " + key);
						Debug.Log("value= " + ht[key].ToString());
					}
				}
			}
		}
	}
}
