using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Extensions;

[ExecuteInEditMode]
public class UIAlertViewControllerBase : MonoBehaviour 
{
	/*
	public delegate void OnAlertViewWillAppear(UIAlertViewControllerBase sender);
	public OnAlertViewWillAppear onAlertViewWillAppear;
	
	public delegate void OnAlertViewWillDisappear(UIAlertViewControllerBase sender);
	public OnAlertViewWillDisappear onAlertViewWillDisappear;
	
	public delegate void OnAlertViewDidDisappear(UIAlertViewControllerBase sender);
	public OnAlertViewDidDisappear onAlertViewDidDisappear;
	
	public delegate void OnAlertViewDidAppear(UIAlertViewControllerBase sender);
	public OnAlertViewDidAppear onAlertViewDidAppear;
	
	public delegate void OnAlertViewButton(int buttonIndex);
	public OnAlertViewButton onAlertViewButton;
	
	const float kAppearDuration = 0.65f;
	const float kDisappearDuration = 0.15f;
	
	protected bool isAppearAnimationPlaying = false;
	public bool IsAppearAnimationPlaying {get{return isAppearAnimationPlaying;}}
	
	protected bool isDisappearAnimationPlaying = false;
	public bool IsDisappearAnimationPlaying {get{return isDisappearAnimationPlaying;}}
	
	protected List<UISlicedImageButton> activeButtons = new List<UISlicedImageButton>();
	
	public UISprite backgroundSprite = null;
	public MYLabel titleLabel = null;
	public UISprite titleIconSprite = null;
	public Transform contentGroupTransform = null;
	
	public UISlicedImageButton cancelButton = null;
	public UISlicedImageButton confirmationButton = null;
	
	public Transform buttonsGroupTransform = null;

	// Use this for initialization
	void Awake()
	{
		DoAwake();
	}
	
	protected virtual void DoAwake()
	{
		activeButtons = new List<UISlicedImageButton>();
		if (Application.isPlaying)
		{		
			UIButtonMessage btnMsg = null;
			if (confirmationButton != null)
			{
				btnMsg = confirmationButton.gameObject.AddComponent<UIButtonMessage>();
				btnMsg.functionName = "OnButton";
				btnMsg.target = gameObject;
			}
			
			if (cancelButton != null)
			{
				btnMsg = cancelButton.gameObject.AddComponent<UIButtonMessage>();
				btnMsg.functionName = "OnButton";
				btnMsg.target = gameObject;
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		DoUpdate();
	}
	
	protected virtual void DoUpdate()
	{
		if (Application.isEditor && !Application.isPlaying)
		{
			this.Reposition();
		}
	}

	protected virtual void Present()
	{
		if ( onAlertViewWillAppear != null)
			onAlertViewWillAppear(this);
		
		//remove disappear animation tweeners:
		UIMultiAlphaTween multiAlphaTweener = gameObject.GetComponentInChildren<UIMultiAlphaTween>();
		if (multiAlphaTweener != null)
		{
			multiAlphaTweener.Reset();
			multiAlphaTweener.enabled = false;
		}
		
		isDisappearAnimationPlaying = false;
		isAppearAnimationPlaying = true;
		
		//setup appear animation tweeners:
		TweenPosition positionTweener = gameObject.GetComponentInChildren<TweenPosition>();
		if (positionTweener == null)
			positionTweener = gameObject.AddComponent<TweenPosition>();
	
		
		positionTweener.from = transform.localPosition + Vector3.up*200f;
		positionTweener.to = transform.localPosition;
		positionTweener.duration = kAppearDuration*0.5f;
		positionTweener.method = UITweener.Method.EaseOut;
		
		TweenScale scaleTweener = gameObject.GetComponentInChildren<TweenScale>(); 
		if (scaleTweener == null)
			scaleTweener = gameObject.AddComponent<TweenScale>();
		
		scaleTweener.from = new Vector3(0.1f, 0.1f, 1f);
		scaleTweener.to = Vector3.one;
		scaleTweener.duration = kAppearDuration;
		scaleTweener.method = UITweener.Method.BounceIn;
		scaleTweener.callWhenFinished = "OnAppear";
		scaleTweener.eventReceiver = gameObject;
		
		scaleTweener.Reset();
		scaleTweener.Play(true);
		positionTweener.Reset();
		positionTweener.Play(true);
		
		//play sfx
		AudioManager.PlaySfxClip(AudioManager.alert_appear_sfx);
	}
	
	public virtual void Dismiss()
	{
		if ( onAlertViewWillDisappear != null)
			onAlertViewWillDisappear(this);
		//remove appear animation tweeners:
		TweenPosition positionTweener = gameObject.GetComponentInChildren<TweenPosition>();
		if (positionTweener != null)
			positionTweener.enabled = false;
		TweenScale scaleTweener = gameObject.GetComponentInChildren<TweenScale>(); 
		if (scaleTweener != null)
			scaleTweener.enabled = false;
		
		isAppearAnimationPlaying = false;
		isDisappearAnimationPlaying = true;
		
		//setup disappear animation tweeners:
		UIMultiAlphaTween multiAlphaTweener = gameObject.GetComponentInChildren<UIMultiAlphaTween>();
		if (multiAlphaTweener == null)
			multiAlphaTweener = gameObject.AddComponent<UIMultiAlphaTween>();
		
		multiAlphaTweener.callWhenFinished = "OnDisappear";
		multiAlphaTweener.eventReceiver = gameObject;
		multiAlphaTweener.from = 1f;
		multiAlphaTweener.to = 0.0f;
		multiAlphaTweener.method = UITweener.Method.EaseIn;
		multiAlphaTweener.duration = kDisappearDuration;
		multiAlphaTweener.Reset();
		multiAlphaTweener.Play(true);
	}
	
	void OnButton(GameObject sender)
	{
		if (isDisappearAnimationPlaying || isAppearAnimationPlaying)
			return;
		
		Dismiss();
		
		if (onAlertViewButton != null)
		{
			int index = activeButtons.IndexOf(sender.GetComponent<UISlicedImageButton>());
			onAlertViewButton(index);
		}
		
		AudioManager.PlaySfxClip(AudioManager.button_click_sfx);
	}
	
	void OnDisappear()
	{
		isDisappearAnimationPlaying = false;
		if (onAlertViewDidDisappear != null)
		{
			onAlertViewDidDisappear(this);
		}
	}
	
	protected virtual void OnAppear()
	{	
		isAppearAnimationPlaying = false;
		if (onAlertViewDidAppear != null)
		{
			onAlertViewDidAppear(this);
		}
	}
	
	public void InitializeLabelWithTextSafely (MYLabel label, string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			if (label != null) 
			{
				label.text = string.Empty;
				NGUITools.SetActive(label.gameObject, false);
			}
		}
		else
		{
			if (label != null) 
			{
				NGUITools.SetActive(label.gameObject, true);
				label.text = text;
			}
		}
	}

	public void InitializeTextureWithRectAndAtlasSafely (UITexture texture, Texture2DAtlas atlas, Rect uv)
	{
		//INITIALIZE CAFE TEXTURE
		if (atlas == null && texture != null)
			NGUITools.SetActive(texture.gameObject, false);
		else if (texture != null)
		{
			NGUITools.SetActive(texture.gameObject, true);
			
			texture.mainTexture = null;
			texture.material = atlas.AtlasMaterial;
			texture.uvRect = uv;
			texture.ScaleToFit();
		}
	}

	public void InitializeProgressBarSafely (UIProgressBar progressBar_, int minValue_, int maxValue_, int currentValue_)
	{
		if (progressBar_ != null)
		{
			NGUITools.SetActive(progressBar_.gameObject, true);
			progressBar_.MinValue = minValue_;
			progressBar_.MaxValue = maxValue_;
			progressBar_.SetValue(currentValue_,0f,0f);
		}
	}

	public void InitializeGridWithAtlasAndRectsSafely(UIGridExt grid_, Texture2DAtlas atlas_, List<Rect> uvs_, Rect bgUv_)
	{
		if (uvs_ == null)
			uvs_ = new List<Rect>();
		
		int countOfItemsForDisplay_ = uvs_.Count;
		
		if (countOfItemsForDisplay_ == 0 && grid_ != null)
			NGUITools.SetActive(grid_.gameObject, false);
		else if (grid_ != null)
		{
			NGUITools.SetActive(grid_.gameObject, true);
			grid_.hideInactive = false;
			List<GameObject> existedItemCells_ = grid_.GetSortedCells();
			
			if (existedItemCells_.Count > 0)
			{
				//instantiate required cells 
				for (int i = existedItemCells_.Count; i < countOfItemsForDisplay_; i++)
					grid_.AddCell(existedItemCells_[0]);
				
				//destory unwanted cells
				for (int i = countOfItemsForDisplay_; i < existedItemCells_.Count; i++)
					grid_.RemoveCell(existedItemCells_[i]);
				
				//setup all cells
				existedItemCells_ = grid_.GetSortedCells();
				for (int i = 0; i < countOfItemsForDisplay_; i++)
				{
					GameObject cellGo_ = existedItemCells_[i];
					NGUITools.SetActive(cellGo_, true);
					UICountableItemCell cell_ = cellGo_.GetComponent<UICountableItemCell>();
					if (cell_ != null)
					{
						if (atlas_ != null && i < uvs_.Count && cell_.iconTexture != null)
						{
							cell_.iconTexture.mainTexture = null;
							cell_.iconTexture.material = atlas_.AtlasMaterial;
							cell_.iconTexture.uvRect = uvs_[i];
							cell_.iconTexture.ScaleToFit ();
						}
						else if (cell_.iconTexture != null)
							NGUITools.SetActive(cell_.iconTexture.gameObject, false);

						if (atlas_ != null && cell_.backgroundTexture != null & bgUv_.width != 0f && bgUv_.height != 0f)
						{
							cell_.backgroundTexture.mainTexture = null;
							cell_.backgroundTexture.material = atlas_.AtlasMaterial;
							cell_.backgroundTexture.uvRect = bgUv_;
							cell_.backgroundTexture.ScaleToFit ();
						}
						else if (cell_.backgroundTexture != null)
							NGUITools.SetActive(cell_.backgroundTexture.gameObject, false);

						//deactive cell count label, because it is not required
						if (cell_.countLabel != null)
							NGUITools.SetActive(cell_.countLabel.gameObject, false);
					}
				}
			}
		}
	}

	public void InitializeSlicedButtonWithTextSafely (UISlicedImageButton button, string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			if (button != null) 
			{
				NGUITools.SetActive(button.gameObject, false);
			}
		}
		else
		{
			if (button != null) 
			{
				NGUITools.SetActive(button.gameObject, true);
				button.SetTitle(text);
				button.Commit();
			}
		}
	}

	public void RepositionScrollableGrid(UIGridExt grid_)
	{
		//REPOSITION ITEMS GRID
		if (grid_ != null)
		{
			grid_.hideInactive = true;
			grid_.Reposition();
			
			UIDraggablePanelExt draggablePanel = NGUITools.FindInParents<UIDraggablePanelExt>(grid_.gameObject) as UIDraggablePanelExt;
			UIPanel scrollViewPanel = NGUITools.FindInParents<UIPanel>(grid_.gameObject) as UIPanel;
			
			if (scrollViewPanel != null && draggablePanel != null)
			{
				float panelWidth = scrollViewPanel.clipRange.z;
				float contentSize = grid_.cellWidth*grid_.GetCells().Count;
				
				if(contentSize<=panelWidth)
					draggablePanel.relativePositionOnReset = new Vector2(0.5f ,0.5f);
				else
					draggablePanel.relativePositionOnReset = new Vector2(0.0f,0.5f);
				
				draggablePanel.ResetPosition();
			}
		}
	}
	
	public virtual void Show(string alertTitleString_, string confirmationBtnString_, string[] otherBtnStrings_, bool showTitleIcon_)
	{	
		if (Application.isEditor && !Application.isPlaying)
		{
			Debug.LogWarning("Show can only be called in play mode");
			return;
		}

		InitializeLabelWithTextSafely(titleLabel, alertTitleString_);
		InitializeSlicedButtonWithTextSafely(confirmationButton, confirmationBtnString_);

		if ( otherBtnStrings_ != null && cancelButton != null)
		{
			int buttonIndex_ = 0;
			foreach (string btnTitle_ in  otherBtnStrings_)
			{
				UISlicedImageButton button_ = null;

				if(buttonIndex_==0)
					button_ = cancelButton;

				else //make new instance of button
				{	
					GameObject go = NGUITools.AddChild(buttonsGroupTransform.gameObject, cancelButton.gameObject);
					go.transform.localPosition = cancelButton.transform.localPosition;

					UIButtonMessage btnMsg = go.AddComponent<UIButtonMessage>();
					btnMsg.functionName = "OnButton";
					btnMsg.target = gameObject;
				
					button_ = go.GetComponent<UISlicedImageButton>();
				}

				NGUITools.SetActive(button_.gameObject, true);
				button_.SetTitle(btnTitle_);
				button_.Commit();

				button_.name = buttonIndex_ + ".Other button";
				buttonIndex_++;
			}
		} 
		else if (cancelButton != null)
		{	
			// there is no cancel button
			NGUITools.SetActive(cancelButton.gameObject, false);
		}

		if(titleIconSprite!= null)
		{	
			NGUITools.SetActive(titleIconSprite.gameObject, showTitleIcon_); 
		}
		Reposition();
		Present();
	}

	protected virtual void RepositionOfButtons()
	{
		if (activeButtons.Count <= 2)  //buttonsAlignmentStyle = HORIZONTAL
		{
			float allButtonsWidth_ = 0;
			
			foreach (UISlicedImageButton button_ in activeButtons)
				allButtonsWidth_ += button_.GetSize().x;
			allButtonsWidth_ += Mathf.Clamp((activeButtons.Count - 1), 0, activeButtons.Count) * _buttonsIndentPx;
			
			float btnPosX_ = -allButtonsWidth_ *0.5f;
			float btnPosY_ = 0f;
			float btnPosZ_ = 0f;
			foreach (UISlicedImageButton button_ in activeButtons.Reverse<UISlicedImageButton>() )
			{
				button_.pivot = UIImageButtonPivot.center; 
				btnPosX_ += button_.GetSize().x *0.5f;
				btnPosY_ = button_.transform.localPosition.y;
				btnPosZ_ = button_.transform.localPosition.z;
				button_.transform.localPosition = new Vector3(btnPosX_, btnPosY_, btnPosZ_);
				btnPosX_ += button_.GetSize().x *0.5f + _buttonsIndentPx;
			}
		}
		else //buttonsAlignmentStyle = VERTICAL
		{
			float btnPosX_ = 0f;
			float btnPosY_ = 0f;
			float btnPosZ_ = 0f;
			foreach (UISlicedImageButton button_ in activeButtons.Reverse<UISlicedImageButton>() )
			{
				button_.pivot = UIImageButtonPivot.center;
				btnPosY_ += button_.GetSize().y *0.5f;
				btnPosZ_ = button_.transform.localPosition.z;
				button_.transform.localPosition = new Vector3(btnPosX_, btnPosY_, btnPosZ_);
				btnPosY_ += button_.GetSize().y *0.5f + _buttonsIndentPy;
			}
		}
	}

	protected float _buttonsIndentPx = 20f;
	protected float _buttonsIndentPy = 20f;
	protected float _addToHeightPx = 80f;
	protected virtual void Reposition()
	{
		//COLLECTING activeButtons
		activeButtons.Clear();
		// confirmation btn is first one
		if (confirmationButton != null && NGUITools.GetActive(confirmationButton.gameObject))
			activeButtons.Add(confirmationButton);

		List<UISlicedImageButton> buttons_ = new List<UISlicedImageButton> ();
		foreach (UISlicedImageButton button_ in buttonsGroupTransform.GetComponentsInChildren<UISlicedImageButton>())
			if (cancelButton != button_ &&  NGUITools.GetActive(button_.gameObject) ) buttons_.Add(button_);

		foreach (UISlicedImageButton button_ in buttons_.OrderBy(b => b.name).ToList())
		{
			if (button_ != confirmationButton && NGUITools.GetActive(button_.gameObject))
				activeButtons.Add(button_);
		}

		// cancel btn is last one		
		if (cancelButton != null && NGUITools.GetActive(cancelButton.gameObject))
			activeButtons.Add(cancelButton);

		//REPOSITION OF TITLE
		if(titleLabel!= null)
		{
			Transform tr =  titleLabel.gameObject.transform;
			Vector3 titleLabelPosition = tr.localPosition;
			if(titleIconSprite == null || !NGUITools.GetActive(titleIconSprite.gameObject))
				tr.localPosition = new Vector3(0f, titleLabelPosition.y, titleLabelPosition.z); 
			else
				tr.localPosition = new Vector3(titleIconSprite.gameObject.transform.localScale.x/2f, titleLabelPosition.y, titleLabelPosition.z);
		}

		//REPOSITION BUTTONS
		RepositionOfButtons();

		//REPOSITION OF ALL ANCHORS
		List<UIAnchor> contentAnchors_ = new List<UIAnchor> ();
		foreach (UIAnchor anchor_ in contentGroupTransform.gameObject.GetComponentsInChildren<UIAnchor>())
			contentAnchors_.Add(anchor_);
		//anchors will be repositioned in order by name
		foreach (UIAnchor anchor_ in contentAnchors_.OrderBy(anchor => anchor.name).ToList())
			anchor_.Update();
		
		//RESIZE ALERT
		if (contentGroupTransform != null && backgroundSprite != null && buttonsGroupTransform != null)
		{ 
			Vector2 contentSize_ = NGUIMath.CalculateRelativeWidgetBounds(contentGroupTransform).size;
			Vector2 buttonSize_ = NGUIMath.CalculateRelativeWidgetBounds(buttonsGroupTransform).size;

			float summaryHeight_ = contentSize_.y  + buttonSize_.y + _addToHeightPx;
			
			Vector2 bgSize_ = backgroundSprite.transform.localScale;
			
			Vector2 tileSize_ = backgroundSprite.inner;
			
			bgSize_.y = (Mathf.Round(summaryHeight_/tileSize_.y)-1f) * tileSize_.y + backgroundSprite.border.y + backgroundSprite.border.w;
			backgroundSprite.transform.localScale = bgSize_;
		}

		//REPOSITION OF ALL ANCHORS IN BUTTONS GROUP
		List<UIAnchor> buttonsAnchors_ = new List<UIAnchor> ();
		foreach (UIAnchor anchor_ in buttonsGroupTransform.gameObject.GetComponentsInChildren<UIAnchor>())
			buttonsAnchors_.Add(anchor_);
		//anchors will be repositioned in order by name
		foreach (UIAnchor anchor_ in buttonsAnchors_.OrderBy(anchor => anchor.name).ToList())
			anchor_.Update();
	}
	*/
}
