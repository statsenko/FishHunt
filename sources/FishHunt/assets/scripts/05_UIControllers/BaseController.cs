using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseController : MonoBehaviour 
{
	protected float camWidth = 0f;
	protected float camHeight = 0f;
	public Camera mCamera;
	protected UIRoot mRoot;

	protected BaseController parent_;

	protected bool mStarted = false;

	public BaseController Parent
	{
		get 
		{
			if (parent_ == null)
				parent_ = NGUITools.FindInParents<BaseController>(transform.parent.gameObject);
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

	public virtual void LoadInAppPurchaseScene()
	{
	}

	public virtual void LoadShopScene()
	{
	}

	public virtual void BackToLoginScene()
	{
	}

	public virtual void LoadLevelUpScene()
	{
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
