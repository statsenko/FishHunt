using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class UIScrollView : MonoBehaviour 
{			
	public UIWidget.Pivot pivot = UIWidget.Pivot.Center;
	public Vector3 verticalScrollBarInset = Vector3.zero;
	
	public UISprite background = null;
	public Vector4 backgroundInset = Vector4.zero;

	public UIPanel viewPanel = null;
	public UIDraggablePanelExt viewDragPanel = null;
	public Transform content = null;
	
	public Vector2 size = Vector2.zero; 
	
	Vector2 dragAmount = Vector2.zero; 
	
	
	void Awake()
	{
		if (content == null && viewDragPanel != null)
		{
			content = (viewDragPanel.transform.childCount > 0) ?viewDragPanel.transform.GetChild(0) :null;
		}
	}
	
	public void Reposition()
	{	
		//reset drag position
		viewPanel.transform.localScale = Vector3.one;
		viewPanel.clipRange = new Vector4(0f, 0f, size.x, size.y);
		
		float centerPosY = 0f;
		float centerPosX = 0f;
		Vector2 size_ = Size;
		switch (pivot)
		{
		case UIWidget.Pivot.Bottom:
			centerPosY += size_.y/2f;
			break;
		case UIWidget.Pivot.BottomLeft:
			centerPosY += size_.y/2f;
			centerPosX += size_.x/2f;
			break;
		case UIWidget.Pivot.BottomRight:
			centerPosY += size_.y/2f;
			centerPosX -= size_.x/2f;
			break;
		case UIWidget.Pivot.Center:
			break;
		case UIWidget.Pivot.Left:
			centerPosX += size_.x/2f;
			break;
		case UIWidget.Pivot.Right:
			centerPosX -= size_.x/2f;
			break;
		case UIWidget.Pivot.Top:
			centerPosY -= size_.y/2f;
			break;
		case UIWidget.Pivot.TopLeft:
			centerPosY -= size_.y/2f;
			centerPosX += size_.x/2f;
			break;
		case UIWidget.Pivot.TopRight:
			centerPosY -= size_.y/2f;
			centerPosX -= size_.x/2f;
			break;
		default:
			break;
		}

		viewPanel.transform.localPosition = new Vector3(centerPosX, centerPosY, viewPanel.transform.localPosition.z);
		
		UIScrollBar verticalBar = viewDragPanel.verticalScrollBar;
		if (verticalBar != null)
		{
			UISprite barBgSprite = verticalBar.background;
			if (barBgSprite != null)
			{	
				Vector3 tmp = barBgSprite.transform.localScale;
				barBgSprite.transform.localScale = new Vector3(tmp.x, size.y-verticalScrollBarInset.y-verticalScrollBarInset.z, tmp.z);
			}
			verticalBar.transform.localPosition = new Vector3(centerPosX + size.x/2f + verticalScrollBarInset.x, centerPosY + size.y/2f - verticalScrollBarInset.y, 0f);
		}
		
		if (content != null)
			 content.SendMessage("Reposition", SendMessageOptions.DontRequireReceiver);
		
		viewDragPanel.SetDragAmount(dragAmount.x, dragAmount.y, false);
		
		if (background != null)
		{
			background.pivot = UIWidget.Pivot.TopLeft;
			background.transform.localScale = new Vector3(size.x+backgroundInset.x+backgroundInset.z, size.y+backgroundInset.y+backgroundInset.w, 1f);
			background.transform.localPosition = new Vector3(centerPosX-size.x/2f-backgroundInset.x, centerPosY+size.y/2f+backgroundInset.y, viewPanel.transform.localPosition.z);
		}
	}
	
	void Update () 
	{
		if ( Application.isEditor && !Application.isPlaying)
		{
			if (viewPanel && viewDragPanel)
				Size = size;
		}
	}
	
	public Vector2 Size
	{
		get 
		{	
			return (viewPanel != null)?new Vector2(viewPanel.clipRange.z, viewPanel.clipRange.w) :Vector2.zero;
		}
		set 
		{
			dragAmount = (viewDragPanel != null) ?viewDragPanel.relativePositionOnReset :Vector2.zero;
			size = value;
			Reposition();
		}
	}
	
	public Vector2 DragAmount
	{
		get 
		{
			return (viewDragPanel != null)?viewDragPanel.CurrentDragAmount() :Vector2.zero;
		}
		set 
		{
			dragAmount = value;
			Reposition();
		}
	}
	
	public Vector2 ContentSize
	{
		get
		{
			if (content != null)
				return NGUIMath.CalculateRelativeWidgetBounds(content, content).size;
			else
				return Vector3.zero;
		}
	}
}
