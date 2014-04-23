using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


[CustomEditor(typeof(UISlicedImageButton))]
public class UISlicedImageButtonEditor : Editor
{
	/*
	UISlicedImageButton mButton;	
	UISprite mSprite;
	UISprite mIconSprite;
	MYLabel mTitleLabel;
	UIImageButtonPivot mPivot;
	
	float mLeftInset;
	float mRightInset;
	float mMinWidth;
	float mMinHeight;
	
	int mIconCenter;
	
	bool mIsEnabled;
	Color mDisabledColor;
	
	string mTitleLabelText;
	
	void UpdateSizes()
	{
		bool needUpdate = false;
		mMinWidth = mLeftInset + mRightInset;
		
		if (mButton.leftInsetPx != mLeftInset)
		{
			mButton.leftInsetPx = mLeftInset;
			needUpdate = true;
		}
		
		if (mButton.rightInsetPx != mRightInset)
		{
			mButton.rightInsetPx = mRightInset;
			needUpdate = true;
		}		
		
		if (mButton.sprite)
		{
			mButton.sprite.MakePixelPerfect();
			if(mMinWidth < mButton.sprite.transform.localScale.x)
				mMinWidth = mButton.sprite.transform.localScale.x;
			
			if(mMinHeight < mButton.sprite.transform.localScale.y)
				mMinHeight = mButton.sprite.transform.localScale.y;
			
		}
		
		if (mButton.iconSprite)
		{
			mButton.iconSprite.MakePixelPerfect();
			float sizeOficon = mButton.iconSprite.transform.localScale.x;
			if(mButton.leftInsetPx< sizeOficon)
				mButton.leftInsetPx = sizeOficon;
		}
		
		if (mButton.titleLabel)
		{
			mButton.titleLabel.MakePixelPerfect();
			float titleScale = mTitleLabel.transform.localScale.x;
			if(mMinWidth < titleScale + mLeftInset + mRightInset)
				mMinWidth = titleScale + mLeftInset + mRightInset;
		}
		else if(mMinWidth< mLeftInset + mRightInset)
		{
			mMinWidth = mLeftInset + mRightInset;
		}
		
		if (mButton.minWidthPx != mMinWidth)
			mButton.minWidthPx = mMinWidth;
		
		if (mButton.minHeightPx != mMinHeight)
			mButton.minHeightPx = mMinHeight;
		
		if (needUpdate)
			EditorUtility.SetDirty(target);
	}
	
	
	void OnSelectSpriteAtlas (MonoBehaviour obj)
	{
		//normal image
		if (mButton.sprite != null)
		{
			NGUIEditorTools.RegisterUndo("Atlas Selection", mButton.sprite);
			mButton.sprite.atlas = obj as MYAtlas;
			mButton.sprite.MakePixelPerfect();
		}
		UpdateSizes();
	}
	
	void OnSelectIconSpriteAtlas (MonoBehaviour obj)
	{
		//normal image
		if (mButton.iconSprite != null)
		{
			NGUIEditorTools.RegisterUndo("Icon", mButton.iconSprite);
			mButton.iconSprite.atlas = obj as MYAtlas;
			mButton.iconSprite.MakePixelPerfect();
		}
		UpdateSizes();
	}
	
	public override void OnInspectorGUI ()
	{
		EditorGUIUtility.LookLikeControls();
		mButton = target as UISlicedImageButton;
		
		mSprite = EditorGUILayout.ObjectField("Sprite", mButton.sprite, typeof(UISprite), true) as UISprite;
		mIconSprite = EditorGUILayout.ObjectField("Icon Sprite", mButton.iconSprite, typeof(UISprite), true) as UISprite;
		
		//////////////// SPRITE ///////////////////////
		
		if (mButton.sprite != mSprite)
		{
			NGUIEditorTools.RegisterUndo("Image Button Change", mButton);
			mButton.sprite = mSprite;
			if (mSprite != null) 
				mSprite.spriteName = mButton.normalImageName;
			UpdateSizes();
		}
		
		if (mSprite != null)
		{
			ComponentSelector.Draw<MYAtlas>(mSprite.atlas, OnSelectSpriteAtlas);
			if (mSprite.atlas != null)
			{
				NGUIEditorTools.SpriteField("Normal", mSprite.atlas, mButton.normalImageName, OnNormal);
				NGUIEditorTools.SpriteField("Pressed", mSprite.atlas, mButton.pressedImageName, OnPressed);
				NGUIEditorTools.SpriteField("Hover", mSprite.atlas, mButton.hoverImageName, OnHover);
			}
		}
		
		//////////////// ICON SPRITE ///////////////////
		
		if (mButton.iconSprite != mIconSprite)
		{
			NGUIEditorTools.RegisterUndo("Image Button Change", mButton);
			mButton.iconSprite = mIconSprite;
			if (mIconSprite != null) 
				mIconSprite.spriteName = mButton.iconImageName;
			UpdateSizes();
		}
		
		if(mIconSprite != null)
		{
			ComponentSelector.Draw<MYAtlas>(mIconSprite.atlas, OnSelectIconSpriteAtlas);
			if (mIconSprite != null)
			{
				NGUIEditorTools.SpriteField("Icon", mIconSprite.atlas, mButton.iconImageName, OnIconChanged);
			}
		}
		
		if (mButton.iconSprite != mIconSprite)
		{
			NGUIEditorTools.RegisterUndo("Image Button Change", mButton);
			mButton.iconSprite = mIconSprite;
			if (mIconSprite != null) 
				mIconSprite.spriteName = mButton.iconImageName;
			UpdateSizes();
		}
		
		/////////////////////////////////////////////////
		
		bool needUpdate = false;
		//isHighlighted
		mIsEnabled = EditorGUILayout.Toggle("Enabled", mButton.isEnabled, GUILayout.Width(500));
		if (mButton.isEnabled != mIsEnabled)
		{
			mButton.isEnabled = mIsEnabled;
			needUpdate = true;
		}
		
		mDisabledColor = EditorGUILayout.ColorField("Disabled Color",mButton.disabledColor, GUILayout.Width(150));
		if (mButton.disabledColor != mDisabledColor)
		{
			mButton.disabledColor = mDisabledColor;
			needUpdate = true;
		}
		
		//public MYLabel titleLabe
		mTitleLabel = EditorGUILayout.ObjectField("Title Label", mButton.titleLabel, typeof(MYLabel), true) as MYLabel;
		if (mButton.titleLabel != mTitleLabel)
		{
			mButton.titleLabel = mTitleLabel;
			if ( mTitleLabel != null )
				mTitleLabel.text = "Title";
		}
		
		if ( mTitleLabel != null )
		{
			mTitleLabelText = EditorGUILayout.TextField("Label Text", mButton.titleLabel.text, GUILayout.Width(500));
			if (string.Compare(mButton.titleLabel.text, mTitleLabelText, false) != 0)
			{
				mButton.titleLabel.text = mTitleLabelText;	
				needUpdate = true;
			}
		}
		
		//public UISlicedImageButtonPivot pivot 
		mPivot = (UIImageButtonPivot)EditorGUILayout.EnumPopup("Pivot", mButton.pivot);
		if (mButton.pivot != mPivot)
		{
			mButton.pivot = mPivot;
			needUpdate = true;
		}
		
		// leftInsetPx
		mLeftInset = EditorGUILayout.FloatField("Left Inset", mButton.leftInsetPx); 
		if (mButton.leftInsetPx != mLeftInset)
		{
			mButton.leftInsetPx = mLeftInset;
			needUpdate = true;
		}
		// rightInsetPx
		mRightInset = EditorGUILayout.FloatField("Right Inset", mButton.rightInsetPx);
		if (mButton.rightInsetPx != mRightInset)
		{
			mButton.rightInsetPx = mRightInset;
			needUpdate = true;
		}
		
		// minWidthPx
		if(mIconSprite != null && mLeftInset > 0)
		{
			mIconCenter = EditorGUILayout.IntSlider("Icon Center", mButton.iconCenterPx, 0 , (int)mLeftInset, GUILayout.Width(500));
			if (mButton.iconCenterPx != mIconCenter)
			{
				mButton.iconCenterPx = mIconCenter;
				needUpdate = true;
			}
		}
		
		// minWidthPx
		mMinWidth = EditorGUILayout.FloatField("Min Width", mButton.minWidthPx);
		if (mButton.minWidthPx != mMinWidth)
		{
			mButton.minWidthPx = mMinWidth;
			needUpdate = true;
		}
		
		mMinHeight= EditorGUILayout.FloatField("Min Height", mButton.minHeightPx);
		if (mButton.minHeightPx != mMinHeight)
		{
			mButton.minHeightPx = mMinHeight;
			needUpdate = true;
		}
		
		if (needUpdate)
		{
			EditorUtility.SetDirty(target);
		}
	}
	
	void OnNormal (string imageName)
	{
		NGUIEditorTools.RegisterUndo("Image Button Change", mButton, mButton.gameObject, mSprite);
		mButton.normalImageName = imageName;
		mSprite.spriteName = imageName;
		mSprite.MakePixelPerfect();
		if (mButton.collider == null || (mButton.collider is BoxCollider)) NGUITools.AddWidgetCollider(mButton.gameObject);
		UpdateSizes();
		Repaint();
	}
	
	void OnPressed (string imageName)
	{
		NGUIEditorTools.RegisterUndo("Image Button Change", mButton, mButton.gameObject, mSprite);
		mButton.pressedImageName = imageName;
		//UpdateSizes();
		Repaint();
	}
	
	void OnHover (string imageName)
	{
		NGUIEditorTools.RegisterUndo("Image Button Change", mButton, mButton.gameObject, mSprite);
		mButton.hoverImageName = imageName;
		//UpdateSizes();
		Repaint();
	}
	
	
	void OnIconChanged (string imageName)
	{
		NGUIEditorTools.RegisterUndo("Icon Change", mButton, mButton.gameObject, mIconSprite);
		mButton.iconImageName = imageName;
		mIconSprite.spriteName = imageName;
		mIconSprite.MakePixelPerfect();
		UpdateSizes();
		Repaint();
	}
*/	
}