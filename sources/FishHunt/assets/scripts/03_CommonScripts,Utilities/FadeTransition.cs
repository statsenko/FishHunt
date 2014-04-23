using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LevelDescription
{
	public LevelDescription(string levelName_, Dictionary<string, object> _passedParams, Dictionary<string, object> _popParams, string _popTarget)
	{
		LevelName = levelName_;
		PassedParams = _passedParams;
		PopParams = _popParams;
		PopTarget = _popTarget;
	}

	// Level name
	string _levelName = null;
	public string LevelName
	{
		get
		{
			return _levelName;
		}
		set
		{
			_levelName = value;
		}
	}

	// Parameters
	Dictionary<string, object> _passedParams = null;
	public Dictionary<string, object> PassedParams
	{
		get 
		{
			if (_passedParams == null)
				_passedParams = new Dictionary<string, object>();
			return _passedParams;
		}
		set
		{
			if (value != null)
				_passedParams = new Dictionary<string, object>(value);
		}
	}

	// Parameters for previous scene
	Dictionary<string, object> _popParams = null;
	public Dictionary<string, object> PopParams
	{
		get 
		{
			if (_popParams == null)
				_popParams = new Dictionary<string, object>();
			return _popParams;
		}
		set
		{
			if (value != null)
				_popParams = new Dictionary<string, object>(value);
		}
	}

	// Scene which will load after current scene
	private string _popTarget;
	public string PopTarget
	{
		get
		{
			return _popTarget;
		}
		set
		{
			_popTarget = value;
		}
	}
}

public class FadeTransition : MonoBehaviour
{
	enum EFadeState 
	{
		kFadeState_FadeIn,
		kFadeState_LoadingLevel,
		kFadeState_LoadAnotherLevel,
		kFadeState_FadeOut,
		kFadeState_Waiting
	}
	
	static private FadeTransition instance = null;
	static public FadeTransition Instance
	{
		get 
		{
			if (instance == null)
			{
				GameObject go = new GameObject("_FadeTransition");
				go.layer = LayerMask.NameToLayer("FadeTransition");
				instance = go.AddComponent(typeof(FadeTransition)) as FadeTransition;
				DontDestroyOnLoad(instance.gameObject);
			}
			
			return instance;
		}
	}
	
	protected static List<LevelDescription> _levelsStack = new List<LevelDescription> ();

	public static LevelDescription LastLoadedLevelDescription
	{
		get
		{
			if (_levelsStack.Count <= 0)
				return new LevelDescription(Application.loadedLevelName, null, null, null);
			else
				return _levelsStack[_levelsStack.Count-1];
		}
	}

	public static string LastLoadedLevelName
	{
		get
		{
			if (_levelsStack.Count <= 0)
				return Application.loadedLevelName;
			else
				return _levelsStack[_levelsStack.Count-1].LevelName;
		}
	}

	public static void PushLevel(string levelName_, Dictionary<string, object> passedParams_ = null, Dictionary<string, object> popParams_ = null, string popTarget_ = null, float fadeLength_ = 0.5f, float fadeOutLength_ = 0.5f) 
	{
		LevelDescription newLevel_ = new LevelDescription(levelName_, passedParams_, popParams_, popTarget_);
		_levelsStack.Add(newLevel_);
		FadeAndLoadLevel(levelName_, fadeLength_, fadeOutLength_);
	}

	//if passedParams is null, level wil be loaded with previously passed params for this level
	public static void PopLevel(float fadeLength_ = 0.5f, float fadeOutLength_ = 0.5f)
	{
		if (_levelsStack == null || _levelsStack.Count <= 1)
		{
			Debug.LogError(string.Format("ERROR: FadeTransition: You can't pop level, because levels stack contains {0} elements including current level. Maybe you didn't push any level yet?", _levelsStack!=null?_levelsStack.Count :0));
		}
		else
		{
			// If popTarget is set
			if (LastLoadedLevelDescription.PopTarget != null) 
			{
				// Load popTarget scene
				PopToLevel(LastLoadedLevelDescription.PopTarget);
			}
			else 
			{	
				Dictionary<string, object> popParams_ = LastLoadedLevelDescription.PopParams; 
				_levelsStack.RemoveAt(_levelsStack.Count-1);
				LevelDescription newLevel_ = _levelsStack[_levelsStack.Count-1];
				if (popParams_ != null)
					newLevel_.PassedParams = popParams_;
				FadeAndLoadLevel(newLevel_.LevelName, fadeLength_, fadeOutLength_);
			}
		}
	}

	//if passedParams is null, level wil be loaded with previously passed params for this level
	public static void PopToLevel(string levelName_, float fadeLength_ = 0.5f, float fadeOutLength_ = 0.5f) 
	{
		LevelDescription targetLevel_ = _levelsStack == null ? null : _levelsStack.LastOrDefault (levelDescriptor_ => levelDescriptor_.LevelName.Equals (levelName_));
		
		if (targetLevel_ == null)
		{
			//level was not found in levels stack, push new one
			PushLevel(levelName_, null, null, null, fadeLength_, fadeOutLength_);
		}
		else
		{
			Dictionary<string, object> popParams_ = LastLoadedLevelDescription.PopParams; 
			int indexOfTargetLevel_ = _levelsStack.LastIndexOf(targetLevel_);
		
			int countOfItemsToRemove_ = (_levelsStack.Count)-indexOfTargetLevel_-1;

			if (countOfItemsToRemove_ > 0)
				_levelsStack.RemoveRange(indexOfTargetLevel_+1, countOfItemsToRemove_);

			LevelDescription newLevel_ = _levelsStack[_levelsStack.Count-1];
			if (popParams_ != null)
				newLevel_.PassedParams = popParams_;

			FadeAndLoadLevel(levelName_, fadeLength_, fadeOutLength_);
		}
	}

	static void FadeAndLoadLevel(string levelName_, float fadeLength_ = 0.5f, float fadeOutLength_ = 0.5f) 
	{
/*		FadeTransition.Instance.enabled = true;
		FadeTransition.Instance._fadeInLength = fadeLength_;
		FadeTransition.Instance._fadeOutLength = fadeOutLength_;

		UILoadingIndicatorFactory.Show(true);
		FadeTransition.Instance.PrepareFadeIn(levelName_);
*/	}
	
	void PrepareFadeIn(string levelName_)
	{
		_levelName = levelName_;
		
		switch (FadeTransition.Instance._state)
		{
		case EFadeState.kFadeState_LoadAnotherLevel:
			//ничего не делать, просто перепишем загружаемый уровень
			break;
		case EFadeState.kFadeState_FadeIn:
			//по окончанию фэйда просто загрузит другой уровень, ничего не делаем
			break;
		case EFadeState.kFadeState_FadeOut:
			//обратить фейд обратно а после загрузится другой уровень
			_state = EFadeState.kFadeState_FadeIn;
			FadeIn();
			break;
		case EFadeState.kFadeState_LoadingLevel:
			_state = EFadeState.kFadeState_LoadAnotherLevel;
			//по окончанию загрузки загрузиться другой уровень и дальше как оыбчно
			break;
		case EFadeState.kFadeState_Waiting:
			_state = EFadeState.kFadeState_FadeIn;		
			FadeIn();
			break;
		default:
			throw new Exception("Unknown state in PrepareFadeIn");
		}
	}
	
	void FadeIn()
	{
		/*
		if (_fadeInLength > 0)
		{
			TweenColor colorTweener = TweenColor.Begin<TweenColor>(instance._fadeTexture.gameObject, _fadeInLength);
			colorTweener.from = instance._fadeTexture.color;
			colorTweener.to = new Color(0f, 0f, 0f, 1f);
			colorTweener.eventReceiver = instance.gameObject;
			colorTweener.duration = _fadeInLength;
			colorTweener.callWhenFinished = "FadeInCompleted";
			colorTweener.Play(true);
		}
		else
			FadeInCompleted();
		*/
	}

	void FadeInCompleted()
	{
		LoadLevel();
	}

	void FadeOut()
	{
		/*
		if (_fadeOutLength > 0)
		{
			TweenColor colorTweener = TweenColor.Begin<TweenColor>(instance._fadeTexture.gameObject, _fadeOutLength);
			colorTweener.from = instance._fadeTexture.color;
			colorTweener.to = new Color(0f, 0f, 0f, 0f);
			colorTweener.eventReceiver = instance.gameObject;
			colorTweener.duration = _fadeOutLength;
			colorTweener.callWhenFinished = "FadeOutCompleted";
			colorTweener.Play(true);
		}
		else
			FadeOutCompleted();
			*/
	}

	void FadeOutCompleted()
	{
	
	}

	void LoadLevel()
	{
		_state = EFadeState.kFadeState_LoadingLevel;
		Application.LoadLevelAsync(_levelName);
	}

	string _levelName = string.Empty;
	float _fadeInLength = 0;
	float _fadeOutLength = 0;

	Texture _fadeTexture;
	EFadeState _state;
	
	bool _updateWasCalledAfterLevelLoad = true;
	void OnLevelWasLoaded(int index)
	{
		_updateWasCalledAfterLevelLoad = false;
	}
	
	void Awake() 
	{
/*
		_state = EFadeState.kFadeState_Waiting;
		
		UIRoot uiRoot = gameObject.AddComponent(typeof( UIRoot )) as UIRoot;
		uiRoot.maximumHeight = 1136;
		uiRoot.minimumHeight = 960;
		uiRoot.manualHeight = 960;
		uiRoot.automatic = false;
		uiRoot.scalingStyle = UIRoot.Scaling.PixelPerfect;

		UICamera _camera = NGUITools.AddChild<UICamera>(gameObject);
		_camera.allowMultiTouch = false;
		_camera.eventReceiverMask = 1<<gameObject.layer;
		
		Camera cam = _camera.camera;
		cam.orthographic = true;
		cam.orthographicSize = 1f;
		cam.farClipPlane = 1000f;
		cam.nearClipPlane = -1000f;
		cam.cullingMask = _camera.eventReceiverMask;
		cam.clearFlags = CameraClearFlags.Depth;
		cam.depth = 300;

		Texture2D fadeTexture2D = new Texture2D (1, 1);
		fadeTexture2D.SetPixel(0, 0, Color.black);
		fadeTexture2D.Apply();
		
		UITexture fadeTextureUI = NGUITools.AddWidget<UITexture>(_camera.gameObject);
		UIStretch stretch = fadeTextureUI.gameObject.AddComponent<UIStretch>();
		stretch.style = UIStretch.Style.Both;
		stretch.uiCamera = cam;
		
		fadeTextureUI.material = new Material(Shader.Find("Unlit/Transparent Colored"));
		fadeTextureUI.material.mainTexture = fadeTexture2D;
		fadeTextureUI.color = new Color(0f,0f,0f,0f);
		_fadeTexture = fadeTextureUI;
*/
	}
	
	void Update()
	{
		if (!_updateWasCalledAfterLevelLoad)
		{
			if (_state == EFadeState.kFadeState_LoadAnotherLevel)
			{
				LoadLevel();
			}
			else
			{
				_state = EFadeState.kFadeState_FadeOut;
				FadeOut();
			}			
			_updateWasCalledAfterLevelLoad = true;
		}
	}
}
