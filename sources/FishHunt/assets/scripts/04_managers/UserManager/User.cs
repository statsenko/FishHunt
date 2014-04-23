using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class User : MonoBehaviour
{
	protected static User mInstance = null;
	public static User Instance 
	{
		get 
		{
			if (mInstance == null)
			{
				GameObject go = new GameObject("_User");
				mInstance = go.AddComponent(typeof(User)) as User;	
				mInstance._eventScheduler = go.AddComponent(typeof(TimeEventScheduler)) as TimeEventScheduler;
				DontDestroyOnLoad(go);
			}
			return mInstance;
		}
	}

	void OnDestroy()
	{
		mInstance = null;
	}

	protected string _accountLogin = "";
	public static string AccountLoginName
	{
		get { return Instance._accountLogin; }
		set { Instance._accountLogin = value; }
	}
	
	protected string _facebookId = "";
	public static string FacebookId
	{
		get { return Instance._facebookId; }
		set { Instance._facebookId = value; }
	}

	protected string _facebookTitle = "";
	public static string FacebookTitle
	{
		get { return Instance._facebookTitle; }
		set { Instance._facebookTitle = value; }
	}

	/* TUTORIAL - to be implemented
	protected TutorialScriptManager tutorialManager = new TutorialScriptManager();
	public static TutorialScriptManager TutorialManager
	{
		get { return Instance.tutorialManager; }
	}
	*/
	
	protected TimeEventScheduler _eventScheduler = null;
	public static TimeEventScheduler EventScheduler
	{
		get { return Instance._eventScheduler; }
	}
	
	protected LifesManager _lifesManager = new LifesManager();
	public static LifesManager GetLifesManager
	{
		get { return Instance._lifesManager; }
	}

	protected StarsManager _starsManager = new StarsManager();
	public static StarsManager GetStarsManager
	{
		get { return Instance._starsManager; }
	}
	
	protected MoneyManager _moneyManager = new MoneyManager();
	public static MoneyManager GetMoneyManager
	{
		get { return Instance._moneyManager; }
	}
	
	public static void Clear()
	{
		User.EventScheduler.Clear();

		Instance._lifesManager.Clear ();
		Instance._starsManager.Clear ();
		Instance._moneyManager.Clear ();
	}
	
	//COMPARATORS
	public static int StringsAsIntCompare_(int one, int two)
	{
		if (one < two)
			return -1;
		else 
		{
			if (one > two)
				return 1;
			else
				return 0;
		}
	}
}
