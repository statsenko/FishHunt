using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIImageButtonExt : UIImageButton
{
	/// <summary>
	/// Color that will be applied when the button is disabled.
	/// </summary>
	public Color disabledColor = Color.gray;
	
	private Dictionary<UIWidget, Color>	_widgetColors;
	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="UISlicedImageButton"/> is enabled.
	/// </summary>
	new public bool isEnabled
	{
		get
		{
			return base.isEnabled;
		}
		set
		{
			Collider col = collider;
			if (!col) return;
		
			if (col.enabled != value)
			{
				col.enabled = value;
				UpdateColor(value);
			}
		}
	}
	
	/// <summary>
	/// Update the button's color to either enabled or disabled state.
	/// </summary>
	private void UpdateColor (bool shouldBeEnabled)
	{	
		float duration = 0.2f;	
						
		if (string.IsNullOrEmpty(disabledSprite))
		{			
			foreach(UIWidget tweenTarget_ in _widgetColors.Keys)
			{
				if (tweenTarget_ != null && NGUITools.GetActive(tweenTarget_.gameObject))
				{
					TweenColor.Begin(tweenTarget_.gameObject, duration, shouldBeEnabled ? _widgetColors[tweenTarget_] : disabledColor);
				}		
			}
		}
		else
		{
			if (isEnabled)
			{
				target.spriteName = UICamera.IsHighlighted(gameObject) ? hoverSprite : normalSprite;
			}
			else
			{
				target.spriteName = disabledSprite;
			}	
			target.MakePixelPerfect();
		}
	}
	
	void Awake()
	{	
		_widgetColors = new Dictionary<UIWidget, Color>();
		UIWidget[] widgets_ = transform.GetComponentsInChildren<UIWidget>(true);
		bool targetIsChild = false;
		foreach (UIWidget widget_ in widgets_)
		{
			if (widget_ == target)
				targetIsChild = true;
			_widgetColors.Add(widget_, widget_.color);
		}
	
		if (target && !targetIsChild)
			_widgetColors.Add(target, target.color);
	}
}
