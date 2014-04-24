using UnityEngine;
using System.Collections;

[System.Serializable]
	public class UIStretchImageDescription
	{
		public string leftBorderImage;
		public string rightBorderImage;
		public string centerImage;
		
		public bool IsEmpty()
		{
			return leftBorderImage==null || centerImage==null || rightBorderImage==null;
		}
	}
	
	[System.Serializable]
	public enum UIStretchImageButtonPivot
	{
		left = 0,
		right = 1,
		center = 2,
	}

[ExecuteInEditMode]
public class UIStretchImageButton : MonoBehaviour 
{
	public UIStretchImageDescription normalImageDescription;
	public UIStretchImageDescription hoverImageDescription;
	public UIStretchImageDescription pressedImageDescription;
	
	public UISprite leftBorderSprite = null;
	public UISprite rightBorderSprite = null;
	public UITiledSprite centerSprite = null;
	public UILabel titleLabel = null;
	public BoxCollider boxCollider = null;

	public float minWidthPx = 40f;
	public float leftInsetPx = 40f;
	public float rightInsetPx = 40f;
	public UIStretchImageButtonPivot pivot = UIStretchImageButtonPivot.center;
	
	void Awake()
	{
		Setup();
	}
	
	void Setup()
	{
		if (!Application.isEditor || Application.isPlaying)
		{
			if (!leftBorderSprite)
			{
				Debug.LogWarning("ERROR: UISprite component not found on leftBorderSprite child of UIStretchImageButton");
				throw new UnityException("UISprite component not found on leftBorderSprite child of UIStretchImageButton");
			}
		
			if (!rightBorderSprite)
			{
				Debug.LogWarning("ERROR: UISprite component not found on rightBorderSprite child of UIStretchImageButton");
				throw new UnityException("UISprite component not found on rightBorderSprite child of UIStretchImageButton");
			}
		
			if (!centerSprite)
			{
				Debug.LogWarning("ERROR: UISprite component not found on centerSprite child of UIStretchImageButton");
				throw new UnityException("UITiledSprite component not found on centerSprite child of UIStretchImageButton");
			}
			
			if (normalImageDescription.IsEmpty())
			{
				Debug.LogWarning("ERROR: UIStretchImageButton: normalImageDescription is missed");
				throw new UnityException("normalImageDescription is missed");
			}
			if (hoverImageDescription.IsEmpty())
			{
				Debug.LogWarning("ERROR: UIStretchImageButton: hoverImageDescription is missed");
				throw new UnityException("hoverImageDescription is missed");
			}
			if (pressedImageDescription.IsEmpty())
			{
				Debug.LogWarning("ERROR: UIStretchImageButton: pressedImageDescription is missed");
				throw new UnityException("pressedImageDescription is missed");
			}
		}
		
		boxCollider = transform.GetComponent<BoxCollider>();
	}
	
	void Start()
	{
		if (!centerSprite || !leftBorderSprite || !rightBorderSprite || normalImageDescription.IsEmpty() || hoverImageDescription.IsEmpty() || pressedImageDescription.IsEmpty())
			return;
		
		bool isHighlighted = UICamera.IsHighlighted(gameObject);
	
		centerSprite.spriteName = isHighlighted? hoverImageDescription.centerImage : normalImageDescription.centerImage;
		leftBorderSprite.spriteName = isHighlighted? hoverImageDescription.leftBorderImage : normalImageDescription.leftBorderImage;
		rightBorderSprite.spriteName = isHighlighted? hoverImageDescription.rightBorderImage : normalImageDescription.rightBorderImage;
		
		centerSprite.MakePixelPerfect();
		leftBorderSprite.MakePixelPerfect();
		rightBorderSprite.MakePixelPerfect();
		
		Commit();
	}

	void OnHover (bool isOver)
	{
		if (enabled)
		{
			if (!centerSprite || !leftBorderSprite || !rightBorderSprite || normalImageDescription.IsEmpty() || hoverImageDescription.IsEmpty() || pressedImageDescription.IsEmpty())
				return;
			
			centerSprite.spriteName = isOver? hoverImageDescription.centerImage : normalImageDescription.centerImage;
			leftBorderSprite.spriteName = isOver? hoverImageDescription.leftBorderImage : normalImageDescription.leftBorderImage;
			rightBorderSprite.spriteName = isOver? hoverImageDescription.rightBorderImage : normalImageDescription.rightBorderImage;
			
			centerSprite.MakePixelPerfect();
			leftBorderSprite.MakePixelPerfect();
			rightBorderSprite.MakePixelPerfect();

			Commit();
		}
	}

	void OnPress (bool pressed)
	{
		if (enabled)
		{
			if (!centerSprite || !leftBorderSprite || !rightBorderSprite || normalImageDescription.IsEmpty() || hoverImageDescription.IsEmpty() || pressedImageDescription.IsEmpty())
				return;
			
			centerSprite.spriteName = pressed? pressedImageDescription.centerImage : normalImageDescription.centerImage;
			leftBorderSprite.spriteName = pressed? pressedImageDescription.leftBorderImage : normalImageDescription.leftBorderImage;
			rightBorderSprite.spriteName = pressed? pressedImageDescription.rightBorderImage : normalImageDescription.rightBorderImage;
			
			centerSprite.MakePixelPerfect();
			leftBorderSprite.MakePixelPerfect();
			rightBorderSprite.MakePixelPerfect();
			
			Commit();
		}
	}
	
	public bool isEnabled
	{
		get
		{
			Collider col = collider;
			return col && col.enabled;
		}
		set
		{
			Collider col = boxCollider;
			if (!col) return;

			if (col.enabled != value)
			{
				col.enabled = value;
			}
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
			if (!leftBorderSprite || !rightBorderSprite || !centerSprite)
			{
				Debug.LogWarning("Not all StretchImageButton components found");
				return;
			}
		}

		//transform.localScale = Vector3.one;
		//calculate new width based on label width
		if (titleLabel)
		{
			titleLabel.MakePixelPerfect();
			
			float rightBorderWidthPx = rightBorderSprite.cachedTransform.localScale.x;
			float leftBorderWidthPx = leftBorderSprite.cachedTransform.localScale.x;
			float centerWidthPx = centerSprite.cachedTransform.localScale.x;
			
			float labelWidthPx = titleLabel.relativeSize.x*titleLabel.cachedTransform.localScale.x + leftInsetPx + rightInsetPx;
			float buttonWidthPx = centerWidthPx + leftBorderWidthPx + rightBorderWidthPx;
			
			float widthChangePx = labelWidthPx-buttonWidthPx;
			float newButtonWidthPx = buttonWidthPx + widthChangePx;
		
			if (newButtonWidthPx < minWidthPx) 
				newButtonWidthPx = minWidthPx;
			
			float newCenterWidthPx = newButtonWidthPx-rightBorderWidthPx-leftBorderWidthPx;
			if (newCenterWidthPx < 0f)
				newCenterWidthPx = 0f;
			
			float centerPosx = (leftBorderWidthPx-rightBorderWidthPx)/2f;
	
			if (pivot == UIStretchImageButtonPivot.left)
				centerPosx = centerPosx + (leftBorderWidthPx+newCenterWidthPx/2f);
			else if (pivot == UIStretchImageButtonPivot.right)
				centerPosx = centerPosx - (rightBorderWidthPx+newCenterWidthPx/2f);
			
			centerSprite.cachedTransform.localPosition = new Vector3(centerPosx, centerSprite.cachedTransform.localPosition.y, centerSprite.cachedTransform.localPosition.z);
			
			centerSprite.cachedTransform.localScale = new Vector3(newCenterWidthPx, centerSprite.cachedTransform.localScale.y, centerSprite.cachedTransform.localScale.z);
			centerSprite.MakePixelPerfect();
		}
		//reposition border sprites
		leftBorderSprite.pivot = UIWidget.Pivot.Right;
		leftBorderSprite.cachedTransform.localPosition = centerSprite.cachedTransform.localPosition + Vector3.left * (centerSprite.cachedTransform.localScale.x*0.5f - 0.5f);
		//leftBorderSprite.MakePixelPerfect();
		
		rightBorderSprite.pivot = UIWidget.Pivot.Left;
		rightBorderSprite.cachedTransform.localPosition = centerSprite.cachedTransform.localPosition + Vector3.right * (centerSprite.cachedTransform.localScale.x*0.5f - 0.5f);
		//rightBorderSprite.MakePixelPerfect();
		
		
		//reposition title label if needed
		if (titleLabel)
		{
			if (titleLabel.pivot == UIWidget.Pivot.BottomLeft || titleLabel.pivot == UIWidget.Pivot.TopLeft || titleLabel.pivot == UIWidget.Pivot.Left)
			{
				float posx = leftBorderSprite.cachedTransform.localPosition.x - leftBorderSprite.cachedTransform.localScale.x + leftInsetPx;
				titleLabel.cachedTransform.localPosition = new Vector3( posx, titleLabel.cachedTransform.localPosition.y, titleLabel.cachedTransform.localPosition.z);
			}
			else if (titleLabel.pivot == UIWidget.Pivot.BottomRight || titleLabel.pivot == UIWidget.Pivot.TopRight || titleLabel.pivot == UIWidget.Pivot.Right)
			{
				float posx = rightBorderSprite.cachedTransform.localPosition.x + rightBorderSprite.cachedTransform.localScale.x - rightInsetPx;
				titleLabel.cachedTransform.localPosition = new Vector3( posx, titleLabel.cachedTransform.localPosition.y, titleLabel.cachedTransform.localPosition.z);
			}
			else
			{
				float rightBorderWidthPx = rightBorderSprite.cachedTransform.localScale.x;
				float leftBorderWidthPx = leftBorderSprite.cachedTransform.localScale.x;
				float centerWidthPx = centerSprite.cachedTransform.localScale.x;
				float labelWidthPx = titleLabel.relativeSize.x*titleLabel.cachedTransform.localScale.x;
		
				float buttonWidthPx = centerWidthPx + leftBorderWidthPx + rightBorderWidthPx;
				//center of button:
				float buttonCenter = leftBorderSprite.cachedTransform.localPosition.x-leftBorderSprite.cachedTransform.localScale.x + buttonWidthPx/2f;
			
				float lBorderPos = buttonCenter - (buttonWidthPx/2f-leftInsetPx);
				float rBorderPos = buttonCenter + (buttonWidthPx/2f-rightInsetPx);
				
				float posx = buttonCenter;
				
				if (((buttonCenter - labelWidthPx/2f) < lBorderPos) || ((buttonCenter + labelWidthPx/2f) > rBorderPos))
				{
					posx = lBorderPos + (rBorderPos-lBorderPos)/2f;
				}
				
				titleLabel.cachedTransform.localPosition = new Vector3( posx, titleLabel.cachedTransform.localPosition.y, titleLabel.cachedTransform.localPosition.z);
			}
		}
		//resize boxcollider
		if (boxCollider)
		{
			NGUITools.AddWidgetCollider(gameObject);
		}
		
	}
	
	public Vector2 GetSize()
	{
		if (!leftBorderSprite || !rightBorderSprite || !centerSprite)
			return Vector2.zero;
		
		float width = rightBorderSprite.cachedTransform.localScale.x + leftBorderSprite.cachedTransform.localScale.x + centerSprite.cachedTransform.localScale.x;
		float height = Mathf.Max(Mathf.Max(rightBorderSprite.cachedTransform.localScale.y, leftBorderSprite.cachedTransform.localScale.y), centerSprite.cachedTransform.localScale.y);
		return new Vector2(width, height);
	}
	
	public void SetTitle(string title)
	{
		if (titleLabel)
			titleLabel.text = title;
	} 
}
