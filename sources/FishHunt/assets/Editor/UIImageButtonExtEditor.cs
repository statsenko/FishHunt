using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


// [CustomEditor(typeof(UIImageButtonExt))]
public class UIImageButtonExtEditor : Editor
{
	/*
	UIImageButton mButton;
	UISprite mSprite;

	/// <summary>
	/// Atlas selection callback.
	/// </summary>

	void OnSelectAtlas (MonoBehaviour obj)
	{
		if (mButton.target != null)
		{
			NGUIEditorTools.RegisterUndo("Atlas Selection", mButton.target);
			mButton.target.atlas = obj as MYAtlas;
			mButton.target.MakePixelPerfect();
		}
	}

	public override void OnInspectorGUI ()
	{
		EditorGUIUtility.LookLikeControls();
		mButton = target as UIImageButton;
		mSprite = EditorGUILayout.ObjectField("Sprite", mButton.target, typeof(UISprite), true) as UISprite;

		if (mButton.target != mSprite)
		{
			NGUIEditorTools.RegisterUndo("Image Button Change", mButton);
			mButton.target = mSprite;
			if (mSprite != null) mSprite.spriteName = mButton.normalSprite;
		}

		if (mSprite != null)
		{
			ComponentSelector.Draw<MYAtlas>(mSprite.atlas, OnSelectAtlas);

			if (mSprite.atlas != null)
			{
				NGUIEditorTools.SpriteField("Normal", mSprite.atlas, mButton.normalSprite, OnNormal);
				NGUIEditorTools.SpriteField("Hover", mSprite.atlas, mButton.hoverSprite, OnHover);
				NGUIEditorTools.SpriteField("Pressed", mSprite.atlas, mButton.pressedSprite, OnPressed);
				NGUIEditorTools.SpriteField("Disabled", mSprite.atlas, mButton.disabledSprite, OnDisabled);
			}
		}
	}

	void OnNormal (string spriteName)
	{
		NGUIEditorTools.RegisterUndo("Image Button Change", mButton, mButton.gameObject, mSprite);
		mButton.normalSprite = spriteName;
		mSprite.spriteName = spriteName;
		mSprite.MakePixelPerfect();
		if (mButton.collider == null || (mButton.collider is BoxCollider)) NGUITools.AddWidgetCollider(mButton.gameObject);
		Repaint();
	}

	void OnHover (string spriteName)
	{
		NGUIEditorTools.RegisterUndo("Image Button Change", mButton, mButton.gameObject, mSprite);
		mButton.hoverSprite = spriteName;
		Repaint();
	}

	void OnPressed (string spriteName)
	{
		NGUIEditorTools.RegisterUndo("Image Button Change", mButton, mButton.gameObject, mSprite);
		mButton.pressedSprite = spriteName;
		Repaint();
	}
	
	void OnDisabled(string spriteName)
	{
		NGUIEditorTools.RegisterUndo("Image Button Change", mButton, mButton.gameObject, mSprite);
		mButton.disabledSprite = spriteName;
		Repaint();
	}
	*/
}