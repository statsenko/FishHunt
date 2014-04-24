using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIButtonExt : UIButton
{
	protected Dictionary<UIWidget, Color>	_widgetColors;
	
	public Dictionary<UIWidget, Color>	WidgetColors
	{
		get {return _widgetColors; }
	}

	List<UIWidget> GetWidgetsInChildren(Transform t)
	{
		List<UIWidget> widgets_ = new List<UIWidget>();
		
		UIWidget widget_ = t.GetComponent<UIWidget> ();
		
		if (widget_ != null )
			widgets_.Add(widget_);
		
		foreach( Transform child in t )
			widgets_.AddRange( GetWidgetsInChildren( child ) );
		
		return widgets_;
	}

	protected override void OnEnable ()
	{
		UpdateColor(isEnabled, true);
	}
	
	protected override void Init ()
	{
		_widgetColors = new Dictionary<UIWidget, Color>();
		
		Transform tweenTarget_ = (tweenTarget == null) ?transform :tweenTarget.transform;
		List<UIWidget> widgets_ = GetWidgetsInChildren (tweenTarget_);
		foreach (UIWidget widget_ in widgets_)
			_widgetColors.Add(widget_, widget_.color);
		
		if ( (tweenTarget_ == null || tweenTarget_.GetComponent<UIWidget>() == null) && widgets_.Count > 0 )
		{
			tweenTarget = widgets_[0].gameObject;
		}
	}
	
	public override bool isEnabled
	{
		get
		{
			return base.isEnabled;
		}
		set
		{
			Collider col = collider;
			if (!col) return;

			if (!mStarted)
			{
				mStarted = true;
				Init();
			}

			if (col.enabled != value)
			{
				col.enabled = value;
				UpdateColor(value, false);
			}
		}
	}

	private bool isShaded = false;
	public bool IsShaded
	{
		get { return isShaded; }
		set
		{
			if (!mStarted)
			{
				mStarted = true;
				Init();
			}

			isShaded = value;

			UpdateColor( !isShaded, false);
		}
	}
	
	public override void OnPress (bool isPressed)
	{
		if (isEnabled) 
		{
			UpdateColor(!isPressed, false);
		}
	}

	public override void OnHover (bool isOver)
	{
		if (!IsShaded && isEnabled) 
		{
			UpdateColor(!isOver, false);
		}
	}

	public override void UpdateColor(bool shouldBeEnabled, bool immediate)
	{	
		if (!mStarted)
		{
			mStarted = true;
			Init();
		}

		float duration = 0.2f;
		foreach(UIWidget tweenTarget_ in _widgetColors.Keys)
		{
			if (tweenTarget_ != null && NGUITools.GetActive(tweenTarget_.gameObject))
			{
				Color c = shouldBeEnabled ? _widgetColors[tweenTarget_]: _widgetColors[tweenTarget_]*disabledColor;
				TweenColor tc = TweenColor.Begin(tweenTarget_.gameObject, duration, c);
				if (immediate)
				{
					tc.Sample(1.0f, true);
					tc.enabled = false;
				}
			}		
		}
	}
}
