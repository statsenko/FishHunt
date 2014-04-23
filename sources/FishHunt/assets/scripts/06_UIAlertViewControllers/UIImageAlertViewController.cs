using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Extensions;

[ExecuteInEditMode]
public class UIImageAlertViewController : UIMessageAlertViewController 
{
	/*
	public UITexture imageTexture = null;
	public UITexture confirmBtnIconTexture = null;

	protected override void DoAwake()
	{
		base.DoAwake();
	}
	
	protected override void DoUpdate () 
	{
		base.DoUpdate();
	}
	
	public void Show(Texture2DAtlas atlas_, Rect imageUv_, string title_, string message_, string confirmButtonTitle_, Rect confirmBtnUv_, string[] otherButtonTitles_, bool showTitleIcon_)
	{
		if (Application.isEditor && !Application.isPlaying)
		{
			Debug.LogWarning("Show can only be called in play mode");
			return;
		}

		InitializeLabelWithTextSafely(messageLabel, message_);

		if (atlas_ != null)
		{
			if (imageTexture != null)
			{
				NGUITools.SetActive(imageTexture.gameObject, true);
				
				imageTexture.mainTexture = null;
				imageTexture.material = atlas_.AtlasMaterial;
				imageTexture.uvRect = imageUv_;
				imageTexture.MakePixelPerfect ();
			}

			if(confirmBtnIconTexture!=null)
			{	
				if (confirmBtnUv_.width != 0 && confirmBtnUv_.height != 0)
				{
					NGUITools.SetActive(confirmBtnIconTexture.cachedGameObject, true);

					confirmBtnIconTexture.mainTexture = null;
					confirmBtnIconTexture.material = atlas_.AtlasMaterial;
					confirmBtnIconTexture.uvRect = confirmBtnUv_;
					confirmBtnIconTexture.MakePixelPerfect ();

					if(confirmationButton!=null)
					{
						confirmationButton.leftInsetPx = 70;
						confirmationButton.Commit();
					}
				} 
				else
				{
					NGUITools.SetActive(confirmBtnIconTexture.cachedGameObject, false);
					if(confirmationButton!=null)
					{
						confirmationButton.leftInsetPx = 40;
						confirmationButton.Commit();
					}
				}
			}
		}
		else
		{
			if	(imageTexture != null)	NGUITools.SetActive(imageTexture.cachedGameObject, false);
			if	(confirmBtnIconTexture != null)	NGUITools.SetActive(confirmBtnIconTexture.cachedGameObject, false);
		}

		base.Show (title_, message_, confirmButtonTitle_, otherButtonTitles_, showTitleIcon_);
	}
	
	protected override void Reposition()
	{
		base.Reposition();
	}
	*/
}