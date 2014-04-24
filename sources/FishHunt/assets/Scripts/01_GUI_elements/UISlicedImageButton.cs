using UnityEngine;
using System.Collections;
using System.Collections.Generic;

	[System.Serializable]
	public enum UIImageButtonPivot
	{
		left = 0,
		right = 1,
		center = 2,
	}

[ExecuteInEditMode]
public class UISlicedImageButton : MonoBehaviour 
{
	public string normalImageName = null;
	public string pressedImageName = null;
	public string hoverImageName = null;
	
	public string iconImageName = null;
	
	public UISprite sprite = null;
	public UILabel titleLabel = null;
	public BoxCollider boxCollider = null;

	public UISprite iconSprite = null;

	public float minWidthPx = 40f;
	public float minHeightPx = 100f;
	
	public float leftInsetPx = 40f;
	public int iconCenterPx = 20;
	public float rightInsetPx = 40f;
	public UIImageButtonPivot pivot = UIImageButtonPivot.center;
	
	
	/// <summary>
	/// Color that will be applied when the button is disabled.
	/// </summary>
	public Color disabledColor = Color.gray;

	private Dictionary <UIWidget, Color> _widgetColors = new Dictionary <UIWidget, Color>();

	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="UISlicedImageButton"/> is enabled.
	/// </summary>
	public bool isEnabled
	{
		get
		{
			Collider col = collider;
			return col && col.enabled;
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
	public void UpdateColor (bool shouldBeEnabled)
	{	
		float duration = 0.2f;

		foreach(UIWidget tweenTarget_ in _widgetColors.Keys)
		{
			if (tweenTarget_ != null && NGUITools.GetActive(tweenTarget_.gameObject))
			{
				TweenColor.Begin(tweenTarget_.gameObject, duration, shouldBeEnabled ? _widgetColors[tweenTarget_]: disabledColor);
			}		
		}
	}

	void Awake()
	{
		Setup();
	}
	
	void Setup()
	{
		boxCollider = transform.GetComponent<BoxCollider>();
	}
	
	void Start()
	{
		if (!sprite || string.IsNullOrEmpty(normalImageName) ||  string.IsNullOrEmpty(pressedImageName))
			return;
		
		bool isHighlighted = UICamera.IsHighlighted(gameObject);
		
		sprite.spriteName = isHighlighted? hoverImageName : normalImageName;
		sprite.MakePixelPerfect();
		
		Commit();
		
		// add elements to array
		UIWidget[] widgets_ = gameObject.GetComponentsInChildren<UIWidget>(true);
		foreach (UIWidget widget_ in widgets_)
			_widgetColors.Add(widget_, widget_.color);

		if (Application.isPlaying)
			this.UpdateColor(isEnabled);
	}

	void OnHover (bool isOver)
	{
		if (enabled)
		{
			if (!sprite || string.IsNullOrEmpty(normalImageName) ||  string.IsNullOrEmpty(hoverImageName))
				return;
			
			sprite.spriteName = isOver? hoverImageName : normalImageName;
			sprite.MakePixelPerfect();
			
			Commit();
		}
	}

	void OnPress (bool pressed)
	{
		if (enabled)
		{
			if (!sprite || string.IsNullOrEmpty(normalImageName) ||  string.IsNullOrEmpty(pressedImageName))
				return;
			
			sprite.spriteName = pressed? pressedImageName : normalImageName;
			sprite.MakePixelPerfect();
			
			Commit();
		}
	}
	
	void Update () 
	{
		if (Application.isEditor && !Application.isPlaying)
		{
			Setup();
			Commit();
		}
	}
	
	public void Commit()
	{
		if (Application.isEditor && !Application.isPlaying)
		{
			if (!sprite)
			{
				//Debug.LogWarning("Not all SlicedImageButton components found–∑—à–º—â–µ");
				return;
			}
		}

		transform.localScale = Vector3.one;
		
		float iconWidthPx = 0.0f;
		if  (!string.IsNullOrEmpty(iconImageName) && iconSprite != null && NGUITools.GetActive(iconSprite.gameObject))
			 iconWidthPx = iconSprite.relativeSize.x*iconSprite.transform.localScale.x;
		if(leftInsetPx < iconWidthPx)
			leftInsetPx = iconWidthPx;
		
		
				//reposition icon sprites

		
		float centerPosx = 0.0f;
		float labelWidthPx = 0f;
		float newButtonWidthPx = 0f;
		//calculate new width based on label width
		if (titleLabel)
		{
			titleLabel.MakePixelPerfect();
			
			// float spriteWidthPx = sprite.transform.localScale.x;		
			 labelWidthPx = titleLabel.relativeSize.x*titleLabel.transform.localScale.x;
			
			 newButtonWidthPx = labelWidthPx + leftInsetPx + rightInsetPx;
		
			if (newButtonWidthPx < minWidthPx) 
				newButtonWidthPx = minWidthPx;
			
			
	
			if (pivot == UIImageButtonPivot.left)
				centerPosx = newButtonWidthPx/2.0f;
			else if (pivot == UIImageButtonPivot.center)
				centerPosx = 0;
			else if (pivot == UIImageButtonPivot.right)
				centerPosx = newButtonWidthPx/(-2.0f);
			
			sprite.transform.localPosition = new Vector3(centerPosx, sprite.transform.localPosition.y, sprite.transform.localPosition.z);
			if(sprite.transform.localPosition.y<minHeightPx)
				sprite.transform.localScale = new Vector3(newButtonWidthPx, minHeightPx, sprite.transform.localScale.z);
			else
				sprite.transform.localScale = new Vector3(newButtonWidthPx, sprite.transform.localScale.y, sprite.transform.localScale.z);
			sprite.MakePixelPerfect();
		}
		
		if(iconSprite!= null && NGUITools.GetActive(iconSprite.gameObject))
		{
			iconSprite.pivot = UIWidget.Pivot.Center;
			iconSprite.transform.localPosition = sprite.transform.localPosition + Vector3.left * ((sprite.transform.localScale.x*0.5f) - iconCenterPx); // - (leftInsetPx*0.5f));
			iconSprite.MakePixelPerfect();
		}
		
		//reposition title label if needed
		if (titleLabel)
		{
			//titleLabel.pivot = UIWidget.Pivot.Center;
			
			if (titleLabel.pivot == UIWidget.Pivot.BottomLeft || titleLabel.pivot == UIWidget.Pivot.TopLeft || titleLabel.pivot == UIWidget.Pivot.Left)
			{
				//Debug.Log("Left PIVOT");
				float posx = sprite.transform.localPosition.x - (sprite.transform.localScale.x/2f) + leftInsetPx; 
				titleLabel.transform.localPosition = new Vector3( posx, titleLabel.transform.localPosition.y, titleLabel.transform.localPosition.z);
			}
			else if (titleLabel.pivot == UIWidget.Pivot.BottomRight || titleLabel.pivot == UIWidget.Pivot.TopRight || titleLabel.pivot == UIWidget.Pivot.Right)
			{
				//Debug.Log("Right PIVOT");
				float posx = sprite.transform.localPosition.x + (sprite.transform.localScale.x/2f) - rightInsetPx; 
				titleLabel.transform.localPosition = new Vector3( posx, titleLabel.transform.localPosition.y, titleLabel.transform.localPosition.z);
			}
			else
			{
				float spriteWidthPx = sprite.transform.localScale.x;
				
				float buttonWidthPx = spriteWidthPx;
				//center of button:
				//float buttonCenter = 0;
				
				//center of label:
				float labelCenter = 0;
				if((leftInsetPx +labelWidthPx+rightInsetPx) > minWidthPx)
					labelCenter = leftInsetPx + ((buttonWidthPx - leftInsetPx - rightInsetPx)/2f) - (buttonWidthPx/2f);
				else
					labelCenter = leftInsetPx + ((buttonWidthPx - leftInsetPx - rightInsetPx)/2f) - (buttonWidthPx/2f);
				
				if (pivot == UIImageButtonPivot.left)
				{
					
					labelCenter += newButtonWidthPx/2.0f;
				}
				else if (pivot == UIImageButtonPivot.center)
				{
					//do nothing
				}
				else if (pivot == UIImageButtonPivot.right)
				{
					labelCenter -= newButtonWidthPx/(2.0f);
				}
				titleLabel.transform.localPosition = new Vector3( labelCenter, titleLabel.transform.localPosition.y, titleLabel.transform.localPosition.z);
			}
		}
		//resize boxcollider
		if (boxCollider)
		{
			float tmp = boxCollider.center.z;
			boxCollider = NGUITools.AddWidgetCollider(gameObject, true);
			if (boxCollider.center.z > tmp)
				boxCollider.center = new Vector3(boxCollider.center.x, boxCollider.center.y, tmp);
		}
		
	}
	
	public Vector2 GetSize()
	{
		if (!sprite)
			return Vector2.zero;
		
		float width = sprite.cachedTransform.localScale.x;
		float height = sprite.cachedTransform.localScale.y;
		return new Vector2(width, height);
	}
	
	public void SetTitle(string title)
	{
		if (titleLabel)
			titleLabel.text = title;
	} 

	public void Hide()
	{
		CancelInvoke("Show");

		Vector3 visibleScale = Vector3.one;
		Vector3 hiddenScale = Vector3.zero;
		visibleScale.z = hiddenScale.z = 1f;

		if (NGUITools.GetActive(gameObject) == false)
			return;

		boxCollider.enabled = false;
		TweenScale scaleIn = TweenScale.Begin<TweenScale>(gameObject, 0.2f);
		scaleIn.from = transform.localScale;
		scaleIn.to = transform.localScale*1.15f;
		scaleIn.method = UITweener.Method.EaseIn;
		scaleIn.onFinished = (scaleIn_) =>
		{
			TweenScale scaleOut = TweenScale.Begin<TweenScale>(gameObject, 0.3f);
			scaleOut.from = transform.localScale;
			scaleOut.to = hiddenScale;
			scaleOut.method = UITweener.Method.EaseInOut;
			scaleOut.onFinished = (tween) =>
			{
				NGUITools.SetActive(gameObject,false);
			};
		};
	}

	public void Show()
	{
		CancelInvoke("Hide");

		Vector3 visibleScale = Vector3.one;
		Vector3 hiddenScale = Vector3.zero;
		visibleScale.z = hiddenScale.z = 1f;

		boxCollider.enabled = true;

		if (NGUITools.GetActive(gameObject) != true)
			transform.localScale = hiddenScale;

		NGUITools.SetActive(gameObject,true);
		TweenScale scaleIn = TweenScale.Begin<TweenScale>(gameObject, 0.5f);
		scaleIn.from = transform.localScale;
		scaleIn.to = visibleScale;
		scaleIn.method = UITweener.Method.BounceIn;
		scaleIn.onFinished = null;
	}
}