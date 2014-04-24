using UnityEngine;
using System.Collections;

public class UILoadingIndicatorFactory : MonoBehaviour 
{
	private static float appearFadeDuration = 0.5f;
	private static Color fadeColor = new Color(0f,0f,0f,0.5f);
	
	private static string loadingIndicatorPrefabPath = "Prefabs/UI/UILoadingIndicatorView";
	private static UILoadingIndicatorFactory mInstance; 
	private UICamera _camera = null;
	//private UIPanel uiPanel = null;
	private UITexture fadeTexture = null;
	private UISprite rotatedSprite = null;
	
	protected static UILoadingIndicatorFactory Instance 
	{
		get 
		{
            if (mInstance == null)
			{
				GameObject go = new GameObject("_UILoadingIndicatorFactory");
				go.layer = LayerMask.NameToLayer("UILoadingIndicator");
				mInstance = go.AddComponent(typeof(UILoadingIndicatorFactory)) as UILoadingIndicatorFactory;
				DontDestroyOnLoad((Object)mInstance);
				
				//attach and setup UIRoot
				UIRoot uiRoot = go.AddComponent(typeof( UIRoot )) as UIRoot;
				uiRoot.maximumHeight = 1136;
				uiRoot.minimumHeight = 960;
				uiRoot.manualHeight = 960;
				uiRoot.automatic = true;
				uiRoot.scalingStyle = UIRoot.Scaling.PixelPerfect;
				
				//attach and setup UICamera
				mInstance._camera = NGUITools.AddChild<UICamera>(go);
				mInstance._camera.name = "LoadingViewCamera";
				mInstance._camera.allowMultiTouch = false;
				mInstance._camera.eventReceiverMask = 1<<mInstance.gameObject.layer;
				
				Camera cam = mInstance._camera.camera;
				cam.orthographic = true;
				cam.orthographicSize = 1f;
				cam.farClipPlane = 1000f;
				cam.nearClipPlane = -1000f;
				cam.cullingMask = mInstance._camera.eventReceiverMask;
				cam.clearFlags = CameraClearFlags.Depth;
				cam.depth = 100;
				
				//attach and setup UIAnchor
				UIAnchor uiAnchor = NGUITools.AddChild<UIAnchor>(mInstance._camera.gameObject);
				uiAnchor.uiCamera = cam;
				
				//attach and Setup UIPanel
				UIPanel panel = NGUITools.AddChild<UIPanel>(uiAnchor.gameObject);
				panel.depthPass = false;
				//mInstance.uiPanel = panel;   
				
				//attach FadeTexture
				Texture2D fadeTexture2D = new Texture2D (1, 1);
    			fadeTexture2D.SetPixel(0, 0, Color.white);
    			fadeTexture2D.Apply();
				UITexture fadeTextureUI = NGUITools.AddWidget<UITexture>(panel.gameObject);
				UIStretch stretch = fadeTextureUI.gameObject.AddComponent<UIStretch>();
				stretch.style = UIStretch.Style.Both;
				stretch.uiCamera = cam;
				
        		fadeTextureUI.material = new Material(Shader.Find("Unlit/Transparent Colored"));
       			fadeTextureUI.material.mainTexture = fadeTexture2D;
				fadeTextureUI.color = new Color(0f,0f,0f,0f);
				mInstance.fadeTexture = fadeTextureUI;
				
				//attach rotated indicator view
				GameObject prefab = Resources.Load(loadingIndicatorPrefabPath) as GameObject;
				if (prefab != null)
				{
					mInstance.rotatedSprite = NGUITools.AddChild( panel.gameObject, prefab ).GetComponent<UISprite>() ;
					NGUITools.SetActive(mInstance.rotatedSprite.gameObject, false);
				}
				
				mInstance.isHidden = true;
            }
            return mInstance;
        }
	}
	
	public static void Show(bool animated = true)
	{
		Instance.Show_(animated);
	}
	public static void Hide()
	{
		if (mInstance != null)
		{
			Instance.Hide_();
		}
	}
	
	public static bool IsHidden
	{
		get
		{
			if (mInstance != null)
				return mInstance.IsHidden_;
			else
				return true;
		}
		set
		{
			if (mInstance != null)
			{
				mInstance.IsHidden_ = value;
			}
		}
	}
	
	protected void Show_(bool animated = true)
	{
		if (IsHidden_)
		{
			FadeIn_(animated);
		}
	}
	
	protected void Hide_()
	{
		FadeOut_();
	}
	
	protected bool isHidden = true;
	protected bool IsHidden_
	{ get{return isHidden;} set{isHidden = value;}}

	public static UICamera GetLoadingIndicatorCamera()
	{
		if (mInstance == null)
			return null;
		else
			return Instance._camera;
	}
	
	protected void FadeIn_(bool animated = true)
	{
		//lock UI
		IsHidden_ = false;
		
		NGUITools.SetActive(rotatedSprite.gameObject, true);
		rotatedSprite.MakePixelPerfect();
		
		TweenColor colorTweener = TweenColor.Begin<TweenColor>(fadeTexture.gameObject, appearFadeDuration);	
		colorTweener.from = fadeTexture.color;
		colorTweener.to = fadeColor;
		
		TweenAlpha alphaTween = TweenAlpha.Begin<TweenAlpha>(rotatedSprite.gameObject, appearFadeDuration);
		alphaTween.from = rotatedSprite.alpha;
		alphaTween.to = 1.0f;
		
		if (!animated)
		{
			colorTweener.enabled = false;
			fadeTexture.color = fadeColor;
			
			alphaTween.enabled = false;
			rotatedSprite.alpha = 1.0f;
			return;
		}
	}
	
	protected void FadeOut_()
	{
		IsHidden_ = true;
		
		TweenColor colorTweener = TweenColor.Begin<TweenColor>(fadeTexture.gameObject, appearFadeDuration);
		colorTweener.from = fadeTexture.color;
		colorTweener.to = new Color(0f,0f,0f,0f);
		colorTweener.onFinished = (tween) => 
		{
			if (mInstance)
			{
				Destroy(mInstance.gameObject);
				mInstance = null;
			}
		};
	
		TweenAlpha alphaTween = TweenAlpha.Begin<TweenAlpha>(rotatedSprite.gameObject, appearFadeDuration);
		alphaTween.from = rotatedSprite.alpha;
		alphaTween.to = 0.0f;
		alphaTween.onFinished = (tween) => 
		{
			NGUITools.SetActive(rotatedSprite.gameObject, false);
		};
	}

	void OnApplicationQuit()
	{
		if (mInstance != null)
		{
			if (Application.isEditor)
				DestroyImmediate(mInstance.gameObject);
			else
				Destroy(mInstance.gameObject);
			mInstance = null;
		}
	}
}
