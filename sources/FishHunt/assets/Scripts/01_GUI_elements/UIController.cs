using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIController : MonoBehaviour 
{
	protected float camWidth = 0f;
	protected float camHeight = 0f;
	public Camera mCamera;
	protected UIRoot mRoot;

	protected UIController parent_;

	protected bool mStarted = false;

	public UIController Parent
	{
		get 
		{
			if (parent_ == null)
			{
				parent_ = transform.parent != null ?NGUITools.FindInParents<UIController>(transform.parent.gameObject) :null;
			}
			return parent_;
		}
	}
	
	void Awake()
	{
		DoAwake();
	}
	protected virtual void DoAwake()
	{
		foreach (UILabel label in GetComponentsInChildren<UILabel>())
			label.text = string.Empty;
	}

	void Start () 
	{
		DoStart();
	}
	protected virtual void DoStart()
	{
		UpdateScreenSize();
	}
	
	void Update () 
	{
		DoUpdate();
	}
	protected virtual void DoUpdate()
	{
		if (Application.isEditor && !Application.isPlaying)
		{
			if (mCamera != null)
			{
				UpdateScreenSize();
			}
		}
		else
		{
			if (!mStarted)
				mStarted = true;
		}
	}

	protected bool isNeedReloadAfterBackground = true;
	IEnumerator OnApplicationPause(bool paused)
	{
		if (Parent == null )
		{
			if ( !paused && isNeedReloadAfterBackground //&& SmartFoxConnection.IsInitialized
			    )
			{
				/*
				//первый вызов: запускаем очередь smartfox ивентов (все слушатели будут удалены, события smartfox будут помещаться в очередь)
				//и вызываем SmartFoxConnection.Connection.ProcessEvents() чтобы обновить состояние smartfox
				SmartFoxConnection.StartQueuingEvents();
				SmartFoxConnection.Connection.ProcessEvents();
				*/
				yield return null;

				/*
				//второй вызов: проверяем текущее состояние smartfox
				//если соединение потеряно - загружаем сцену Login для запукска процесса релогина
				//иначе - останавливаем очередь smartfox ивентов (все слушатели будут восстановлены и получат сообщения, произошедшие в то время пока была запущена очередь)
				if (!SmartFoxConnection.IsConnected)
				{
					SmartFoxConnection.Disconnect();
					PrepareSceneLoad();
					FadeTransition.PopToLevel("Login", 0f, 0f);
				}
				else
				{
					SmartFoxConnection.StopQueuingEvents(true);
				}
				*/
			}
			else if (!paused)
				isNeedReloadAfterBackground = true;
		}
	}
	
	protected void UpdateScreenSize()
	{
		if (mCamera != null)
		{
			camWidth = mCamera.pixelWidth;
			camHeight = mCamera.pixelHeight;
	
			mRoot = NGUITools.FindInParents<UIRoot>(mCamera.gameObject);

			if (mRoot != null)
			{
				camHeight = mRoot.activeHeight;
				camWidth = mRoot.activeHeight * mCamera.aspect;
			}
		}
	}
	
	public IEnumerator DestroyFx(ParticleSystem fx, float dellay)
	{
		yield return new WaitForSeconds(dellay);
		fx.enableEmission = false;
		
		yield return new WaitForSeconds(fx.startLifetime);
		Destroy(fx.gameObject);
	}

	public virtual void SetupAllEventListners()
	{
	}

	public virtual void RemoveAllEventListners()
	{
	}

	public virtual void PreSetupGUI()
	{
	}
	
	public virtual void SetupGUI()
	{
	}

	public virtual void RefreshGUI()
	{
	}
	
	public virtual void PrepareSceneLoad()
	{
		RemoveAllEventListners();
		UIAlertViewFactory.Destroy();
	}

	public virtual void LoadPreviousScene()
	{
		PrepareSceneLoad();
		FadeTransition.PopLevel();
	}

	public virtual void LoadCharacterCreationScene()
	{
		PrepareSceneLoad ();
		FadeTransition.PushLevel("CharacterCreation");
	}

	public virtual void LoadHistoryComicsScene()
	{
		PrepareSceneLoad ();
		FadeTransition.PushLevel("HistoryComics");
	}

	public virtual void LoadMainLobbyScene()
	{
		PrepareSceneLoad ();
		FadeTransition.PushLevel("MainLobby");
	}

	public virtual void LoadCafeMapScene()
	{
		PrepareSceneLoad();
		FadeTransition.PushLevel("CafeMap");
	}

	public virtual void LoadShopScene()
	{
		PrepareSceneLoad ();
		FadeTransition.PushLevel("Shop");
	}

	public virtual void LoadRespecScene()
	{
		PrepareSceneLoad();
		FadeTransition.PushLevel("LevelRespec");
	}

	public virtual void LoadCraftScene()
	{
		PrepareSceneLoad();
		FadeTransition.PushLevel("RecipesCraft");
	}

	public virtual void LoadSettingsScene()
	{
		PrepareSceneLoad();
		FadeTransition.PushLevel("Settings");
	}

	public virtual void LoadGameLoadingScene()
	{
		PrepareSceneLoad();
		FadeTransition.PushLevel("GameLoading");
	}

	public virtual void LoadGameScene()
	{
		PrepareSceneLoad ();
		FadeTransition.PushLevel("Game");
	}

	public virtual void LoadInAppPurchaseScene()
	{
		PrepareSceneLoad ();
		FadeTransition.PushLevel("InAppPurchase");
	}
	
	public virtual void LoadLevelUpScene()
	{
		PrepareSceneLoad ();
		FadeTransition.PushLevel("LevelUp");
	}
	
	public virtual void LoadPrizeGeneratorScene()
	{
		PrepareSceneLoad();
		FadeTransition.PushLevel("PrizeGeneration");
	}

	public virtual void LoadExtraBonusScene()
	{
		PrepareSceneLoad();
		FadeTransition.PushLevel("ExtraBonus");
	}

	public virtual void LoadCafeMasteringScene(int cafeId)
	{
		PrepareSceneLoad ();
		Dictionary<string,object> passedParamsDict = new Dictionary<string,object>();
		passedParamsDict["cafeId"] = cafeId;
		FadeTransition.PushLevel("CafeMastering", passedParamsDict);
	}

	public virtual void BackToLoginScene()
	{
		PrepareSceneLoad();
		FadeTransition.PopToLevel("Login");
	}
	
	public virtual void BackToMainLobbyScene()
	{
		PrepareSceneLoad ();
		FadeTransition.PopToLevel("MainLobby");
	}

	protected void AddImageWithPathNameAndPathToDictionaryIfNeeded(string resourceName, string resourcePath, Dictionary<string, Texture2D> texturesDict)
	{
		if(!texturesDict.ContainsKey(resourceName))
		{
			Texture2D texture = Resources.Load(resourcePath, typeof(Texture2D)) as Texture2D;
			texturesDict[resourceName] = texture;
		}
	}
	protected void AddImageAtPathToDictionaryIfNeeded(string resourcePath, Dictionary<string, Texture2D> texturesDict)
	{
		if(!texturesDict.ContainsKey(resourcePath))
		{
			Texture2D texture = Resources.Load(resourcePath, typeof(Texture2D)) as Texture2D;
			texturesDict[resourcePath] = texture;
		}
	}
}
