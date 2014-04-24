using UnityEngine;
using System.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UIAlertViewFactory : MonoBehaviour 
{
	protected class ShowAlertGenericParams
	{
		public object[] alertPresentParams
		{
			get {return (object[])invokeParams[0];}
		}
		
		public GameObject alertCallbackTarget
		{
			get {return (GameObject)invokeParams[1];}
		}
		
		public string alertCallbackMethod
		{
			get {return (string)invokeParams[2];}
		}
		
		public string alertPrefabPath
		{
			get {return (string)invokeParams[3];}
		}
		
		public System.Type genericType;
		public object[] invokeParams;
	
		public ShowAlertGenericParams(object[] alertPresentParams_, GameObject alertCallbackTarget_, string alertCallbackMethod_, string alertPrefabPath_, System.Type alertType_)
		{
			invokeParams = new object[]{alertPresentParams_, alertCallbackTarget_, alertCallbackMethod_, alertPrefabPath_};
			genericType = alertType_;
		}
	}
	
	private static float appearFadeDuration = 0.5f;
	private static Color fadeColor = new Color(0f,0f,0f,0.6f);

	private static UIAlertViewFactory mInstance; 
	
	private UIAlertViewControllerBase avController = null;
	private UICamera _camera = null;
	private UIPanel _uiPanel = null;
	private UITexture fadeTexture = null;
	
	ShowAlertGenericParams currentShowAlertParams = null;
	
	System.Collections.Generic.Stack<ShowAlertGenericParams> showStack;
	System.Collections.Generic.Queue<ShowAlertGenericParams> showQueue;
	private static Dictionary<string, Texture2D> _texturesDict = null;

	protected static UIAlertViewFactory Instance 
	{
		get 
		{
            if (mInstance == null)
			{
				GameObject go = new GameObject("_UIAlertViewFactory");
				go.layer = LayerMask.NameToLayer("UIAlertView");
				mInstance = go.AddComponent(typeof(UIAlertViewFactory)) as UIAlertViewFactory;
				
				//attach and setup UIRoot
				UIRoot uiRoot = go.AddComponent(typeof( UIRoot )) as UIRoot;
				uiRoot.maximumHeight = 1136;
				uiRoot.minimumHeight = 960;
				uiRoot.manualHeight = 960;
				uiRoot.automatic = true;
				uiRoot.scalingStyle = UIRoot.Scaling.PixelPerfect;
				
				//attach and setup UICamera
				mInstance._camera = NGUITools.AddChild<UICamera>(go);
				mInstance._camera.name = "AlertViewCamera";
				mInstance._camera.allowMultiTouch = false;
				mInstance._camera.eventReceiverMask = 1<<mInstance.gameObject.layer;
				
				Camera cam = mInstance._camera.camera;
				cam.orthographic = true;
				cam.orthographicSize = 1f;
				cam.farClipPlane = 1000f;
				cam.nearClipPlane = -1000f;
				cam.cullingMask = mInstance._camera.eventReceiverMask;
				cam.clearFlags = CameraClearFlags.Depth;
				cam.depth = 400;
				
				//attach and setup UIAnchor
				UIAnchor uiAnchor = NGUITools.AddChild<UIAnchor>(mInstance._camera.gameObject);
				uiAnchor.uiCamera = cam;
				
				//attach and Setup UIPanel
				UIPanel panel = NGUITools.AddChild<UIPanel>(uiAnchor.gameObject);
				panel.depthPass = false; 
				mInstance._uiPanel = panel;
				
				//attach FadeTexture and collider
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
				
				mInstance.IsHidden_ = true;
            }
            return mInstance;
        }
	}
	
	public void HideCurrentAlert_()
	{
		if (avController != null)
		{
			avController.Dismiss();
		}
	}
	
	public void ShowAlertGeneric<T> (object[] alertPresentParams_, GameObject alertCallbackTarget_, string alertCallbackMethod_, string alertPrefabPath_) where T : UIAlertViewControllerBase
	{
		System.Type alertType_ = typeof(T);
		ShowAlertGenericParams showAlertParams_ = new ShowAlertGenericParams(alertPresentParams_, alertCallbackTarget_, alertCallbackMethod_, alertPrefabPath_, alertType_);
		if ((avController != null) ?avController.IsAppearAnimationPlaying :false)
		{
			showQueue.Enqueue(showAlertParams_); 
			return;
		}
		
		bool specificAlertViewExists_ = (avController != null) ?avController.GetType().Equals(alertType_) :false;
		
		if (!specificAlertViewExists_)
		{
			if (avController == null)
			{
				FadeIn_();
				//create stack
				showStack = new System.Collections.Generic.Stack<ShowAlertGenericParams>();
				showQueue = new System.Collections.Generic.Queue<ShowAlertGenericParams>();
			}
			else
			{
				Destroy( avController.gameObject );
				avController = null;
			}
			
			GameObject prefab = Resources.Load(alertPrefabPath_) as GameObject;
			
			GameObject avClone = NGUITools.AddChild(Instance._uiPanel.gameObject, prefab);
			avController = avClone.GetComponent<UIAlertViewControllerBase>();
			
			avController.onAlertViewButton = OnAlertViewButton;
			avController.onAlertViewDidAppear = OnAlertViewDidAppear;
			avController.onAlertViewDidDisappear = OnAlertViewDidDisappear;
			avController.onAlertViewWillDisappear = OnAlertViewWillDisappear;
		}
			
		currentShowAlertParams = showAlertParams_;
					
		avController.transform.localPosition = Vector3.zero + Vector3.back;
		MethodInfo specificAlertMethod_  = alertType_.GetMethod("Show", BindingFlags.Public|BindingFlags.Instance|BindingFlags.DeclaredOnly );
		specificAlertMethod_.Invoke(avController, alertPresentParams_);
	}
		
	protected bool isHidden = true;
	protected bool IsHidden_
	{ get{return isHidden;} set{isHidden = value;}}
	
	
	protected static void PrepareToShow_()
	{
		Instance.SetEnableInputController_(false);
		IsHidden = false;
		
		//dismiss current input
		UIInput currentInput = UIInput.current;
		if (currentInput != null)
		{
			Debug.Log("Current input set selected to false");
			currentInput.selected = false;
		}
	}
	protected static void Hide_()
	{
		IsHidden = true;
		Instance.SetEnableInputController_(true);
		Instance.FadeOut_();
	}
	
	protected void SetEnableInputController_(bool isEnableInputController_)
	{
		if ( !UILoadingIndicatorFactory.IsHidden && isEnableInputController_ )
			return;

		InputController input = InputController.Instance();
		if (input != null)
		{
			input.enabled = isEnableInputController_;
		}
	}
	
	protected void FadeIn_()
	{
		//lock UI
		TweenColor colorTweener = fadeTexture.GetComponentInChildren<TweenColor>();
		if (colorTweener == null)
			colorTweener = fadeTexture.gameObject.AddComponent<TweenColor>();

		colorTweener.from = new Color(0f,0f,0f,0f);
		colorTweener.to = fadeColor;
		
		colorTweener.duration = appearFadeDuration;
		colorTweener.Reset();
		colorTweener.Play(true);
		
	}
	
	protected void FadeOut_()
	{
		if (avController != null)
		{
			Destroy( avController.gameObject );
			avController = null;
			Resources.UnloadUnusedAssets();
		}
				
		TweenColor colorTweener = fadeTexture.GetComponentInChildren<TweenColor>();
		if (colorTweener == null)
			colorTweener = fadeTexture.gameObject.AddComponent<TweenColor>();

		colorTweener.from = new Color(0f,0f,0f,0f);
		colorTweener.to = fadeColor;
		
		colorTweener.duration = appearFadeDuration;
		colorTweener.Play(false);
		
		currentShowAlertParams = null;
	}
	
	void OnAlertViewButton(int buttonIndex)
	{
		if (currentShowAlertParams != null)
		{
			if (currentShowAlertParams.alertCallbackTarget != null && currentShowAlertParams.alertCallbackMethod != null)
				currentShowAlertParams.alertCallbackTarget.SendMessage(currentShowAlertParams.alertCallbackMethod, buttonIndex, SendMessageOptions.DontRequireReceiver);
		}
	}
	void OnAlertViewWillDisappear(UIAlertViewControllerBase sender)
	{
		if (showStack.Count > 0)
			showStack.Pop();
	}
	
	void OnAlertViewDidDisappear(UIAlertViewControllerBase sender)
	{
		if (showStack.Count > 0)
		{
			ShowAlertGenericParams genericParams_ = showStack.Pop();			
			
			System.Type genericType_ = genericParams_.genericType;
			MethodInfo genericMethod_ = GetType().GetMethod("ShowAlertGeneric").MakeGenericMethod(genericType_);
			genericMethod_.Invoke(this, genericParams_.invokeParams);
		}
		else
		{
			Hide_();
		}
	}
	
	void OnAlertViewDidAppear(UIAlertViewControllerBase sender)
	{
		//push current params 
		showStack.Push( currentShowAlertParams);
		
		if (showQueue.Count > 0)
		{
			ShowAlertGenericParams genericParams_ = showQueue.Dequeue();

			System.Type genericType_ = genericParams_.genericType;
			MethodInfo genericMethod_ = GetType().GetMethod("ShowAlertGeneric").MakeGenericMethod(genericType_);
			genericMethod_.Invoke(this, genericParams_.invokeParams);
		}
	}
	
	public static UICamera GetAlertViewCamera()
	{
		return Instance._camera;
	}
	
	public static void Destroy()
	{
		Hide_();
	}

	/*
	public static void ShowShopMultiIconItemsPurchaseAlert(List<IShopItem> shopItems_, string alertTitleString_, KeyValuePair<ECurrencyType, int> price_, string otherButtonString_, string cancelBtnString_, GameObject alertCallbackTarget_, string alertCallbackMethod_)
	{
		PrepareToShow_();

		//map shopItems list to Dictionary: (key:itemType -> value:dictionary(key:itemId -> value:itemCount))
		Dictionary<EShopItemType, Dictionary<int, int>> sortedShopItems = shopItems_.GroupBy(shopItem => shopItem.Type).ToDictionary(groupByType => groupByType.Key, groupByType => groupByType.GroupBy(shopItem => shopItem.Id).ToDictionary(groupById => groupById.Key, groupById => groupById.Count()));

		//CREATE TEXTURE ATLAS
		Texture2DAtlas atlas_ = null;
		Dictionary<string, Texture2D> texturesDictionary_ = new Dictionary<string, Texture2D> ();
		List<Rect> itemsIcnUvs_ = new List<Rect>();
		List<string> itemsCountStrings_ = new List<string> ();

		foreach (EShopItemType itemType_ in sortedShopItems.Keys)
		{
			Dictionary<int, int> itemsCountsForType_ = sortedShopItems[itemType_];
			foreach (int itemId_ in itemsCountsForType_.Keys)
			{
				string itemIconPath_ = User.GetShopManager.GetShopItem(itemId_, itemType_).IconPath;
				if (string.IsNullOrEmpty(itemIconPath_))
					Debug.LogWarning(string.Format("WARNING: UIAlertViewFactory: ShowShopMultiIconItemsPurchaseAlert: Can not load item icon for shop item: (type: {0}, id: {1})", itemType_, itemId_));
				else
					texturesDictionary_[itemIconPath_] = Resources.Load(itemIconPath_) as Texture2D;
			}
		}

		//Load Currency icon to texture atlas
		string currencyIconPath_ = GameConstants.CommonImagesPath + (price_.Key == ECurrencyType.defaultCurrency ? GameConstants.defMoney_icon_2_imgName : GameConstants.premMoney_icon_2_imgName); 
		texturesDictionary_ [currencyIconPath_] = Resources.Load (currencyIconPath_) as Texture2D;
		
		atlas_ = new Texture2DAtlas (texturesDictionary_, Texture2DAtlas.defaultScale);

		foreach (EShopItemType itemType_ in sortedShopItems.Keys)
		{
			Dictionary<int, int> itemsCountsForType_ = sortedShopItems[itemType_];
			List<int> itemsIds_ = itemsCountsForType_.Keys.ToList();
			itemsIds_.Sort();
		
			foreach (int itemId_ in itemsIds_)
			{
				Rect itemIcnUv_ = new Rect();
				string itemIconPath_ = User.GetShopManager.GetShopItem(itemId_, itemType_).IconPath;
				if (!string.IsNullOrEmpty(itemIconPath_))
					itemIcnUv_ = atlas_.GetRect(itemIconPath_);
				itemsIcnUvs_.Add(itemIcnUv_);
				
				string itemCountString_ = string.Format("x{0}", itemsCountsForType_[itemId_]);
				itemsCountStrings_.Add(itemCountString_);
			}
		}

		Rect currencyIconUv_ = atlas_.GetRect(currencyIconPath_);

		//INIT PRICE STRING AND COLOR
		string priceString_ = price_.Value.ToString ();
		Color priceColor_ =  User.GetMoneyManager.HasMoney(price_) ?ShopUIMultiIconAlertViewController.enoughMoneyColor : ShopUIMultiIconAlertViewController.notEnoughMoneyColor;

		object[] alertPresentParams_ = new object[]{atlas_, itemsIcnUvs_, itemsCountStrings_, alertTitleString_, currencyIconUv_, priceString_, priceColor_, otherButtonString_, cancelBtnString_}; 
		Instance.ShowAlertGeneric<ShopUIMultiIconAlertViewController>(alertPresentParams_, alertCallbackTarget_, alertCallbackMethod_, "Prefabs/UI/AlertViews/ShopUIMultiIconAlertView");
	}

	public static void ShowShopUIMessageAlert(Dictionary<ECurrencyType, int> priceDictionary_, string alertTitleString_, string alertMessageString_, 
	                                          string confirmationBtnString_, string cancelBtnString_, GameObject alertCallbackTarget_, string alertCallbackMethod_)
	{
		PrepareToShow_();

		//INIT PRICE STRINGS
		string defaultPriceString_ = string.Empty;
		string premiumPriceString_ = string.Empty;
		//INIT PRICE STRINGS COLOR and WARNING STRING IF NECCESSARY
		string warningString_ = string.Empty;
		Color defaultPriceColor_ = ShopUISingleIconAlertViewController.enoughMoneyColor;
		Color premiumPriceColor_ = ShopUISingleIconAlertViewController.enoughMoneyColor;

		foreach(ECurrencyType currency in priceDictionary_.Keys)
		{
			KeyValuePair<ECurrencyType, int> price = new KeyValuePair<ECurrencyType, int>(currency, priceDictionary_[currency]);
			if(currency == ECurrencyType.defaultCurrency)
			{
				defaultPriceString_= priceDictionary_[currency].ToString();
				if (!User.GetMoneyManager.HasMoney(price))
				{
					warningString_ = GameStrings.GetLocalizedString(GameStrings.Common_NA_Money);
					defaultPriceColor_ = ShopUISingleIconAlertViewController.notEnoughMoneyColor;
				}
			} 
			else if(currency == ECurrencyType.premiumCurrency)
			{
				premiumPriceString_= priceDictionary_[currency].ToString();
				if (!User.GetMoneyManager.HasMoney(price))
				{
					warningString_ = GameStrings.GetLocalizedString(GameStrings.Common_NA_Money);
					premiumPriceColor_ = ShopUISingleIconAlertViewController.notEnoughMoneyColor;
				}
			} 

		}

		object[] alertPresentParams_ = new object[]{ alertTitleString_,  alertMessageString_, defaultPriceString_, defaultPriceColor_, 
													premiumPriceString_, premiumPriceColor_, warningString_, confirmationBtnString_, cancelBtnString_}; 
		Instance.ShowAlertGeneric<ShopUIMessageAlertViewController>(alertPresentParams_, alertCallbackTarget_, alertCallbackMethod_, "Prefabs/UI/AlertViews/ShopUIMessageAlertView");
	}


	public static void ShowRecipesCraftUIPayWallAlert(Recipe recipe, string alertTitleString_, string messageString_, 
	                                                  string confirmationBtnString_, bool hideConfirmBtnIcon_, string otherBtnString_, string cancelBtnString_, KeyValuePair<ECurrencyType,int> confirmationPrice_,
	                                                    GameObject alertCallbackTarget_, string alertCallbackMethod_)
	{
		// RecipesCraftUIMultiIconAlertViewController

		PrepareToShow_();

		//INIT PRICE STRINGS
		string defaultPriceString_ = string.Empty;
		string premiumPriceString_ = string.Empty;
		//INIT PRICE STRINGS COLOR and WARNING STRING IF NECCESSARY
		string warningString_ = string.Empty;
		Color defaultPriceColor_ = ShopUISingleIconAlertViewController.enoughMoneyColor;
		Color premiumPriceColor_ = ShopUISingleIconAlertViewController.enoughMoneyColor;

		// first check, if user has enough ingredients
		if( !User.GetIngredientsManager.HasIngredients( recipe.Ingredients ))
		{
			warningString_ = GameStrings.GetLocalizedString(GameStrings.Common_NA_Ingredients);
		}


		// then check if user has enough money
		KeyValuePair<ECurrencyType, int> price =  recipe.CraftPrice;
		ECurrencyType currency = price.Key;
		if(currency == ECurrencyType.defaultCurrency)
		{
			defaultPriceString_= price.Value.ToString();
			if (!User.GetMoneyManager.HasMoney(price))
			{
				if (string.IsNullOrEmpty(warningString_)) warningString_ = GameStrings.GetLocalizedString(GameStrings.Common_NA_Money);
				defaultPriceColor_ = ShopUISingleIconAlertViewController.notEnoughMoneyColor;
			}
		} 
		else if(currency == ECurrencyType.premiumCurrency)
		{
			premiumPriceString_= price.Value.ToString();
			if (!User.GetMoneyManager.HasMoney(price))
			{
				if (string.IsNullOrEmpty(warningString_)) warningString_ = GameStrings.GetLocalizedString(GameStrings.Common_NA_Money);
				premiumPriceColor_ = ShopUISingleIconAlertViewController.notEnoughMoneyColor;
			}
		} 

		string currentRecipeIconPath = recipe.IconPath;
		string nextRecipeIconPath = recipe.NextRecipeIconPath;
		string activeIngredientBack = GameConstants.cell_single_5;
		string inActiveIngredientBack = GameConstants.ingredients_back_texture_light;

		_texturesDict = new Dictionary<string, Texture2D>();
		string[] images = new string[]
		{
			activeIngredientBack,
			inActiveIngredientBack,
			currentRecipeIconPath, 
			nextRecipeIconPath,
			GameConstants.CommonImagesPath + GameConstants.defMoney_icon_2_imgName,
			GameConstants.CommonImagesPath + GameConstants.premMoney_icon_2_imgName

		};
	
		foreach (string imageName  in images)
		{
			AddImageWithNameAndPathToDictionaryIfNeeded( imageName  );
		}

		foreach (int ingredientID  in recipe.Ingredients.Keys)
		{
			Ingredient ingredient = User.GetIngredientsManager.GetIngredient(ingredientID);
			int ingrefientCount = recipe.Ingredients[ingredientID];
			Dictionary<int, int> ingredientsRequiredDict = new Dictionary<int, int>();
			ingredientsRequiredDict.Add(ingredientID, ingrefientCount);
			if( User.GetIngredientsManager.HasIngredients( ingredientsRequiredDict ))
				AddImageWithNameAndPathToDictionaryIfNeeded(ingredient.IconPath);
			else
				AddImageWithNameAndPathToDictionaryIfNeeded(ingredient.DisabledIconPath);
		}

		Texture2DAtlas atlas_ = new Texture2DAtlas(_texturesDict, Texture2DAtlas.defaultScale);
		_texturesDict.Clear();
		_texturesDict = null;
		Rect uvCurrentRecipeIconRect_ = atlas_.GetRect(currentRecipeIconPath);  
		Rect uvNextRecipeIconRect_ = atlas_.GetRect(nextRecipeIconPath);  

		List<Rect> itemsIconUvs_ = new List<Rect>(); 
		List<Rect> itemsBackgroundUvs_ = new List<Rect>(); 
		foreach (int ingredientID  in recipe.Ingredients.Keys)
		{
			Ingredient ingredient = User.GetIngredientsManager.GetIngredient(ingredientID);
			int ingrefientCount = recipe.Ingredients[ingredientID];
			Dictionary<int, int> ingredientsRequiredDict = new Dictionary<int, int>();
			ingredientsRequiredDict.Add(ingredientID, ingrefientCount);
			if( User.GetIngredientsManager.HasIngredients( ingredientsRequiredDict ))
			{
				itemsIconUvs_.Add(atlas_.GetRect(ingredient.IconPath));
				itemsBackgroundUvs_.Add(atlas_.GetRect(activeIngredientBack));
			}
			else
			{
				itemsIconUvs_.Add(atlas_.GetRect(ingredient.DisabledIconPath));
				itemsBackgroundUvs_.Add(atlas_.GetRect(inActiveIngredientBack));
			}
		}

		Rect confirmationBtnIconUv_ = new Rect(0f,0f,0f,0f);
		switch(confirmationPrice_.Key)
		{
			case ECurrencyType.defaultCurrency:
				confirmationBtnString_ = confirmationBtnString_ + confirmationPrice_.Value.ToString();
				confirmationBtnIconUv_ = atlas_.GetRect(GameConstants.CommonImagesPath + GameConstants.defMoney_icon_2_imgName);
				break;
			case ECurrencyType.premiumCurrency:
				confirmationBtnString_ = confirmationBtnString_ + confirmationPrice_.Value.ToString();
				confirmationBtnIconUv_ = atlas_.GetRect(GameConstants.CommonImagesPath + GameConstants.premMoney_icon_2_imgName);
				break;
			case ECurrencyType.NA:
				// confirmationBtnString_ should not be changed
				// confirmationBtnIconUv_ should be eqal to = new Rect(0f,0f,0f,0f);
				break;
			default:
				break;
		}

		Color confirmBtnTitleColor_ = confirmationPrice_.Key == ECurrencyType.NA ?RecipesCraftUIPayWallAlertViewController.enoughMoneyColor :User.GetMoneyManager.HasMoney(confirmationPrice_) ?RecipesCraftUIPayWallAlertViewController.enoughMoneyColor :RecipesCraftUIPayWallAlertViewController.notEnoughMoneyColor;

		object[] alertPresentParams_ = new object[]{ atlas_, itemsIconUvs_, itemsBackgroundUvs_, uvCurrentRecipeIconRect_, uvNextRecipeIconRect_, confirmationBtnIconUv_, alertTitleString_, messageString_, defaultPriceString_, defaultPriceColor_, premiumPriceString_, premiumPriceColor_, warningString_, confirmationBtnString_, confirmBtnTitleColor_, hideConfirmBtnIcon_, otherBtnString_, cancelBtnString_};
		Instance.ShowAlertGeneric<RecipesCraftUIPayWallAlertViewController>(alertPresentParams_, alertCallbackTarget_, alertCallbackMethod_, "Prefabs/UI/AlertViews/RecipesCraftUIPayWallAlertView");
	}

	private static void AddImageWithNameAndPathToDictionaryIfNeeded(string resourceName, string resourcePath = null)
	{
		if(!_texturesDict.ContainsKey(resourceName))
		{
			Texture2D texture = Resources.Load( (resourcePath == null) ? resourceName : resourcePath, typeof(Texture2D)) as Texture2D;
			_texturesDict[resourceName] = texture;
		}
	}
	
	public static void ShowShopSingleIconPackPurchaseAlert(ShopItemsPack shopPack_, string _customAlertTitleString,  string _alertDescriptionString, string confirmationBtnString_, string cancelBtnString_, GameObject alertCallbackTarget_, string alertCallbackMethod_)
	{
		PrepareToShow_();

		//CREATE TEXTURE ATLAS
		Texture2DAtlas atlas_ = null;
		Dictionary<string, Texture2D> texturesDictionary_ = new Dictionary<string, Texture2D> ();

		Rect itemIcnUv_ = new Rect();
		string itemIconPath_ = shopPack_.IconPath;
		if (string.IsNullOrEmpty(itemIconPath_))
			Debug.LogWarning(string.Format("WARNING: UIAlertViewFactory: ShowShopSingleIconPackPurchaseAlert: Can not load item icon for shop pack: (type: {0}, id: {1})", shopPack_.Type, shopPack_.Id));
		else
		{
			texturesDictionary_[itemIconPath_] = Resources.Load(itemIconPath_) as Texture2D;
			atlas_ = new Texture2DAtlas (texturesDictionary_, Texture2DAtlas.defaultScale);
			itemIcnUv_ = atlas_.GetRect(itemIconPath_);
		}

		//INIT PRICE STRINGS
		string defaultPriceString_ = string.Empty;
		string premiumPriceString_ = string.Empty;
		switch (shopPack_.Price.Key)
		{
		case ECurrencyType.defaultCurrency:
			defaultPriceString_ = shopPack_.Price.Value.ToString();
			break;
		case ECurrencyType.premiumCurrency:
			premiumPriceString_ = shopPack_.Price.Value.ToString();
			break;
		default:
			break;
		}

		//INIT PRICE STRINGS COLOR and WARNING STRING IF NECCESSARY
		string warningString_ = string.Empty;
		Color priceColor_ = ShopUISingleIconAlertViewController.enoughMoneyColor;
		if (!User.GetShopManager.IsEnoughMoneyForPack(shopPack_))
		{
			warningString_ = GameStrings.GetLocalizedString(GameStrings.Common_NA_Money);
			priceColor_ = ShopUISingleIconAlertViewController.notEnoughMoneyColor;
		}
		//INIT ALERT TITLE STRING
		string alertTitleString_ = string.IsNullOrEmpty(_customAlertTitleString) ? GameStrings.GetLocalizedString(GameStrings.ShopPurchaseConfirmAV_Title) : _customAlertTitleString;
		string alertDescriptionString_ = _alertDescriptionString;

		object[] alertPresentParams_ = new object[]{atlas_, itemIcnUv_, alertTitleString_, alertDescriptionString_, shopPack_.Title, shopPack_.Description, defaultPriceString_, priceColor_, premiumPriceString_, priceColor_, warningString_, confirmationBtnString_, cancelBtnString_}; 
		Instance.ShowAlertGeneric<ShopUISingleIconAlertViewController>(alertPresentParams_, alertCallbackTarget_, alertCallbackMethod_, "Prefabs/UI/AlertViews/ShopUISingleIconAlertView");
	}
	
	public static void ShowCafeMapUIPurchaseAlert(Cafe cafe_, string confirmationBtnString_, string cancelBtnString_, GameObject alertCallbackTarget_, string alertCallbackMethod_)
	{
		PrepareToShow_();

		//CREATE TEXTURE ATLAS
		Texture2DAtlas atlas_ = null;
		Dictionary<string, Texture2D> texturesDictionary_ = new Dictionary<string, Texture2D> ();
		List<Rect> itemsIcnUvs_ = new List<Rect>();
		List<Rect> prizesIcnUvs_ = new List<Rect>();
		Rect cafeIcnUv_ = new Rect ();
		Rect itemsBgUv_ = new Rect ();

		//LOAD RECIPE ICONS TEXTURES
		foreach (int recipeId_ in cafe_.RecipesList)
		{
			string recipeIcnPath_ = User.GetRecipesManager.GetRecipe(recipeId_).IconPath;
			if (string.IsNullOrEmpty(recipeIcnPath_))
				Debug.LogWarning(string.Format("WARNING: UIAlertViewFactory: ShowCafeMapUIPurchaseAlert: Can not load item icon for cafe recipe: (cafe id: {0}, recipe id: {1})", cafe_.Id, recipeId_));
			else
				texturesDictionary_[recipeIcnPath_] = Resources.Load(recipeIcnPath_) as Texture2D;
		}

		//LOAD PRIZE ICONS TEXTURES
		foreach (Prize prize_ in User.GetPrizeGeneratorManager.GetPrizeGenerator(cafe_.PrizeGeneratorId).GetPremiumIngredientsPrizesList)
		{
			string prizeIcnPath_ = prize_.IconPath;
			if (string.IsNullOrEmpty(prizeIcnPath_))
				Debug.LogWarning(string.Format("WARNING: UIAlertViewFactory: ShowCafeMapUIPurchaseAlert: Can not load item icon for cafe prize: (cafe id: {0}, prize id: {1})", cafe_.Id, prize_.Id));
			else
				texturesDictionary_[prizeIcnPath_] = Resources.Load(prizeIcnPath_) as Texture2D;
		}

		//LOAD CAFE ICON TEXTURE
		#warning replace with big image
		string cafeIcnPath_ = cafe_.MapImagePath;
		if (string.IsNullOrEmpty(cafeIcnPath_))
			Debug.LogWarning(string.Format("WARNING: UIAlertViewFactory: ShowCafeMapUIPurchaseAlert: Can not load big image for cafe: (cafe id: {0})", cafe_.Id));
		else
			texturesDictionary_[cafeIcnPath_] = Resources.Load(cafeIcnPath_) as Texture2D;

		//LOAD ITEMS BG UV
		string itemsBgImagePath_ = GameConstants.cell_single_6;
		if (string.IsNullOrEmpty(itemsBgImagePath_))
			Debug.LogWarning(string.Format("WARNING: UIAlertViewFactory: ShowCafeMapUIPurchaseAlert: Can not load bg image for items in cafe: (cafe id: {0})", cafe_.Id));
		else
			texturesDictionary_[itemsBgImagePath_] = Resources.Load(itemsBgImagePath_) as Texture2D;
		
		atlas_ = new Texture2DAtlas (texturesDictionary_, Texture2DAtlas.defaultScale);

		//GET RECIPES ICON RECTS FROM ATLAS
		foreach (int recipeId_ in cafe_.RecipesList)
		{
			Rect itemIcnUv_ = new Rect();
			string recipeIconPath_ = User.GetRecipesManager.GetRecipe(recipeId_).IconPath;
			if (!string.IsNullOrEmpty(recipeIconPath_))
				itemIcnUv_ = atlas_.GetRect(recipeIconPath_);
			itemsIcnUvs_.Add(itemIcnUv_);
		}

		//GET PRIZES ICON RECTS FROM ATLAS
		foreach (Prize prize_ in User.GetPrizeGeneratorManager.GetPrizeGenerator(cafe_.PrizeGeneratorId).GetPremiumIngredientsPrizesList)
		{
			Rect prizeIcnUv_ = new Rect();
			string prizeIconPath_ = prize_.IconPath;
			if (!string.IsNullOrEmpty(prizeIconPath_))
				prizeIcnUv_ = atlas_.GetRect(prizeIconPath_);
			prizesIcnUvs_.Add(prizeIcnUv_);
		}
		
		//GET CAFE ICON RECT FROM ATLAS
		if (!string.IsNullOrEmpty(cafeIcnPath_))
			cafeIcnUv_ = atlas_.GetRect(cafeIcnPath_);
		
		//GET ITEMS BG RECT FROM ATLAS
		if (!string.IsNullOrEmpty(itemsBgImagePath_))
			itemsBgUv_ = atlas_.GetRect(itemsBgImagePath_);

		//INIT PRICE AND LEVEL STRINGS
		string defaultPriceString_ = string.Empty;
		string premiumPriceString_ = string.Empty;
		string levelString_ = cafe_.LevelRequired.ToString();

		switch (cafe_.Price.Key)
		{
		case ECurrencyType.defaultCurrency:
			defaultPriceString_ = cafe_.Price.Value.ToString();
			break;
		case ECurrencyType.premiumCurrency:
			premiumPriceString_ = cafe_.Price.Value.ToString();
			break;
		default:
			break;
		}

		//INIT CAFE TITLE STRING
		string cafeTitleString_ = cafe_.Title;

		//INIT ITEMS GRID TITLE STRING
		string itemsGridTitleString_ = GameStrings.GetLocalizedString(GameStrings.CafeMap_CafePurchaseAV_Recipes_Title);
		//INIT PRIZES GRID TITLE STRING
		string prizesGridTitleString_ = GameStrings.GetLocalizedString(GameStrings.CafeMap_CafePurchaseAV_Prizes_Title);
		//INIT EXP PROFIT STRING
		#warning replace with appropriate data
		string expProfitString_ = "+300%";
		//INIT MONEY PROFIT STRING
		#warning replace with appropriate data
		string moneyProfitString_ = "+100%";

		
		//INIT PRICE AND LEVEL STRINGS COLOR and WARNING STRING IF NECCESSARY
		string warningString_ = string.Empty;
		Color priceColor_ = CafeMapUIPurchaseAlertViewController.enoughColor;
		Color levelColor_ = CafeMapUIPurchaseAlertViewController.enoughColor;

		bool levelEnough_ = cafe_.LevelRequired <= User.GetExpManager.LevelCount;
		bool moneyEnough_ = User.GetMoneyManager.HasMoney(cafe_.Price);

		if (!moneyEnough_)
		{
			warningString_ = GameStrings.GetLocalizedString(GameStrings.Common_NA_Money);
			priceColor_ = CafeMapUIPurchaseAlertViewController.notEnoughColor;
		}
		if (!levelEnough_)
		{
			warningString_ = GameStrings.GetLocalizedString(GameStrings.Common_NA_Level);
			levelColor_ = CafeMapUIPurchaseAlertViewController.notEnoughColor;
		}

		if (!moneyEnough_ && !levelEnough_)
		{
			warningString_ = GameStrings.GetLocalizedString(GameStrings.Common_NA_MoneyAndLevel);
		}

		object[] alertPresentParams_ = new object[]{atlas_, cafeIcnUv_, itemsBgUv_, itemsIcnUvs_, prizesIcnUvs_, cafeTitleString_, itemsGridTitleString_, prizesGridTitleString_, moneyProfitString_, expProfitString_, defaultPriceString_, priceColor_, premiumPriceString_, priceColor_, levelString_, levelColor_, warningString_, confirmationBtnString_, cancelBtnString_};
		Instance.ShowAlertGeneric<CafeMapUIPurchaseAlertViewController>(alertPresentParams_, alertCallbackTarget_, alertCallbackMethod_, "Prefabs/UI/AlertViews/CafeMapUIPurchaseAlertView");
	}

	public static void ShowCafeMapUIInfoAlert(Cafe cafe_, string confirmationBtnString_, string cancelBtnString_, GameObject alertCallbackTarget_, string alertCallbackMethod_)
	{
		PrepareToShow_();
		
		//CREATE TEXTURE ATLAS
		Texture2DAtlas atlas_ = null;
		Dictionary<string, Texture2D> texturesDictionary_ = new Dictionary<string, Texture2D> ();
		List<Rect> itemsIcnUvs_ = new List<Rect>();
		List<Rect> prizesIcnUvs_ = new List<Rect>();
		Rect cafeIcnUv_ = new Rect ();
		Rect itemsBgUv_ = new Rect ();
		
		//LOAD RECIPE ICONS TEXTURES
		foreach (int recipeId_ in cafe_.RecipesList)
		{
			string recipeIcnPath_ = User.GetRecipesManager.GetRecipe(recipeId_).IconPath;
			if (string.IsNullOrEmpty(recipeIcnPath_))
				Debug.LogWarning(string.Format("WARNING: UIAlertViewFactory: ShowCafeMapUIInfoAlert: Can not load item icon for cafe recipe: (cafe id: {0}, recipe id: {1})", cafe_.Id, recipeId_));
			else
				texturesDictionary_[recipeIcnPath_] = Resources.Load(recipeIcnPath_) as Texture2D;
		}

		//LOAD PRIZE ICONS TEXTURES
		foreach (Prize prize_ in User.GetPrizeGeneratorManager.GetPrizeGenerator(cafe_.PrizeGeneratorId).GetPremiumIngredientsPrizesList)
		{
			string prizeIcnPath_ = prize_.IconPath;
			if (string.IsNullOrEmpty(prizeIcnPath_))
				Debug.LogWarning(string.Format("WARNING: UIAlertViewFactory: ShowCafeMapUIInfoAlert: Can not load item icon for cafe prize: (cafe id: {0}, prize id: {1})", cafe_.Id, prize_.Id));
			else
				texturesDictionary_[prizeIcnPath_] = Resources.Load(prizeIcnPath_) as Texture2D;
		}

		//LOAD CAFE ICON TEXTURE
		#warning replace with big image
		string cafeIcnPath_ = cafe_.MapImagePath;
		if (string.IsNullOrEmpty(cafeIcnPath_))
			Debug.LogWarning(string.Format("WARNING: UIAlertViewFactory: ShowCafeMapUIInfoAlert: Can not load big image for cafe: (cafe id: {0})", cafe_.Id));
		else
			texturesDictionary_[cafeIcnPath_] = Resources.Load(cafeIcnPath_) as Texture2D;

		//LOAD ITEMS BG UV
		string itemsBgImagePath_ = GameConstants.cell_single_6;
		if (string.IsNullOrEmpty(itemsBgImagePath_))
			Debug.LogWarning(string.Format("WARNING: UIAlertViewFactory: ShowCafeMapUIInfoAlert: Can not load bg image for items in cafe: (cafe id: {0})", cafe_.Id));
		else
			texturesDictionary_[itemsBgImagePath_] = Resources.Load(itemsBgImagePath_) as Texture2D;

		atlas_ = new Texture2DAtlas (texturesDictionary_, Texture2DAtlas.defaultScale);
		
		//GET RECIPES ICON RECTS FROM ATLAS
		foreach (int recipeId_ in cafe_.RecipesList)
		{
			Rect itemIcnUv_ = new Rect();
			string recipeIconPath_ = User.GetRecipesManager.GetRecipe(recipeId_).IconPath;
			if (!string.IsNullOrEmpty(recipeIconPath_))
				itemIcnUv_ = atlas_.GetRect(recipeIconPath_);
			itemsIcnUvs_.Add(itemIcnUv_);
		}

		//GET PRIZES ICON RECTS FROM ATLAS
		foreach (Prize prize_ in User.GetPrizeGeneratorManager.GetPrizeGenerator(cafe_.PrizeGeneratorId).GetPremiumIngredientsPrizesList)
		{
			Rect prizeIcnUv_ = new Rect();
			string prizeIconPath_ = prize_.IconPath;
			if (!string.IsNullOrEmpty(prizeIconPath_))
				prizeIcnUv_ = atlas_.GetRect(prizeIconPath_);
			prizesIcnUvs_.Add(prizeIcnUv_);
		}
		
		//GET CAFE ICON RECT FROM ATLAS
		if (!string.IsNullOrEmpty(cafeIcnPath_))
			cafeIcnUv_ = atlas_.GetRect(cafeIcnPath_);

		//GET ITEMS BG RECT FROM ATLAS
		if (!string.IsNullOrEmpty(itemsBgImagePath_))
			itemsBgUv_ = atlas_.GetRect(itemsBgImagePath_);
		
		//INIT CAFE TITLE STRING
		string cafeTitleString_ = cafe_.Title;
		
		//INIT ITEMS GRID TITLE STRING
		string itemsGridTitleString_ = GameStrings.GetLocalizedString(GameStrings.CafeMap_CafeInfoAV_Recipes_Title);
		//INIT PRIZES GRID TITLE STRING
		string prizesGridTitleString_ = GameStrings.GetLocalizedString(GameStrings.CafeMap_CafeInfoAV_Prizes_Title);
		//INIT EXP PROFIT STRING
		#warning replace with appropriate data
		string expProfitString_ = "+300%";
		//INIT MONEY PROFIT STRING
		#warning replace with appropriate data
		string moneyProfitString_ = "+100%";

		object[] alertPresentParams_ = new object[]{atlas_, cafeIcnUv_, itemsBgUv_, itemsIcnUvs_, prizesIcnUvs_, cafeTitleString_, itemsGridTitleString_, prizesGridTitleString_, moneyProfitString_, expProfitString_, cafe_.MasterLevel, cafe_.CurrentWinCount, cafe_.MasterWinCount, confirmationBtnString_, cancelBtnString_};
		Instance.ShowAlertGeneric<CafeMapUIInfoAlertViewController>(alertPresentParams_, alertCallbackTarget_, alertCallbackMethod_, "Prefabs/UI/AlertViews/CafeMapUIInfoAlertView");
	}
	*/
	public static void ShowAlert(string title_, string message_, string confirmButtonTitle_, string[] otherButtonTitles_, GameObject alertCallbackTarget_, string alertCallbackMethod_, bool showTitleIcon_ = true)
	{
		PrepareToShow_();
		object[] alertPresentParams_ = new object[]{title_, message_, confirmButtonTitle_, otherButtonTitles_, showTitleIcon_};
		Instance.ShowAlertGeneric<UIMessageAlertViewController>(alertPresentParams_, alertCallbackTarget_, alertCallbackMethod_, "Prefabs/UI/AlertViews/UIDefaultAlertView");
	}

	public static void ShowImageAlert(string texturePath_, string title_, string message_, string confirmButtonTitle_, string confirmButtonIconPath_, string[] otherButtonTitles_, GameObject alertCallbackTarget_, string alertCallbackMethod_, bool showTitleIcon_ = false)
	{
		PrepareToShow_();

		//CREATE TEXTURE ATLAS
		Texture2DAtlas atlas_ = null;
		Dictionary<string, Texture2D> texturesDictionary_ = new Dictionary<string, Texture2D> ();

		if (!string.IsNullOrEmpty(texturePath_)) 			texturesDictionary_[texturePath_] = Resources.Load(texturePath_) as Texture2D;
		if (!string.IsNullOrEmpty(confirmButtonIconPath_)) 	texturesDictionary_[confirmButtonIconPath_] = Resources.Load(confirmButtonIconPath_) as Texture2D;

		atlas_ = new Texture2DAtlas (texturesDictionary_, Texture2DAtlas.defaultScale);

		Rect imageUv_ = string.IsNullOrEmpty(texturePath_) ? new Rect() : atlas_.GetRect(texturePath_);
		Rect confirmBtnUv_ = string.IsNullOrEmpty(confirmButtonIconPath_) ? new Rect(0f,0f,0f,0f) : atlas_.GetRect(confirmButtonIconPath_);

		object[] alertPresentParams_ = new object[]{atlas_, imageUv_, title_, message_, confirmButtonTitle_, confirmBtnUv_, otherButtonTitles_, showTitleIcon_};
		Instance.ShowAlertGeneric<UIImageAlertViewController>(alertPresentParams_, alertCallbackTarget_, alertCallbackMethod_, "Prefabs/UI/AlertViews/UIImageAlertView");
	}

	public static void HideCurrentAlert()
	{
		Instance.HideCurrentAlert_();
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
				mInstance.IsHidden_ = value;
		}
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
