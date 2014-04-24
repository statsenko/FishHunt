using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class UIStretchWindow : MonoBehaviour {
	
	public float minWidthPx = 0f;
	public float minHeightPx = 0f;
	
	public float sizeStepX_Px = 0f;
	public float sizeStepY_Px = 0f;
	
	public UISprite target = null;
	
	public UISprite topLeftCorner = null;
	public UISprite bottomLeftCorner = null;
	public UISprite topRightCorner = null;
	public UISprite bottomRightCorner = null;
	
	public UISprite topShadow = null;
	public UISprite bottomShadow = null;
	public UISprite leftShadow = null;
	public UISprite rightShadow = null;
	
	public UISprite topFrame = null;
	public UISprite bottomFrame = null;
	public UISprite leftFrame = null;
	public UISprite rightFrame = null;
	
	
	public UIWidget.Pivot pivot = UIWidget.Pivot.Center;
	
	void Awake()
	{	
		Setup();
	}
	
	void Setup()
	{
		//setup sprites pivots
		if (target)
			target.pivot = UIWidget.Pivot.Center;
		
		//all sprites are anchored to target sprite
		//corners should be anchored by center pivot:
		if (topLeftCorner)
			topLeftCorner.pivot = UIWidget.Pivot.BottomRight;
		if (topRightCorner)
			topRightCorner.pivot = UIWidget.Pivot.BottomLeft;
		if (bottomLeftCorner)
			bottomLeftCorner.pivot = UIWidget.Pivot.TopRight;
		if (bottomRightCorner)
			bottomRightCorner.pivot = UIWidget.Pivot.TopLeft;
		
		/*
		//frames and shadows should be anchored by side pivot, depending on the frame side. Note that shadow will be drawed on target, and frame will be drawed near closed target
		if (topFrame)
			topFrame.pivot = UIWidget.Pivot.Bottom;
		if (topShadow)
			topShadow.pivot = UIWidget.Pivot.Top;
		
		if (bottomFrame)
			bottomFrame.pivot = UIWidget.Pivot.Top;
		if (bottomShadow)
			bottomShadow.pivot = UIWidget.Pivot.Bottom;
		
		if (leftFrame)
			leftFrame.pivot = UIWidget.Pivot.Right;
		if (leftShadow)
			leftShadow.pivot = UIWidget.Pivot.Left;
		
		if (rightFrame)
			rightFrame.pivot = UIWidget.Pivot.Left;
		if (rightShadow)
			rightShadow.pivot = UIWidget.Pivot.Right;
		*/
	}
	
	public void SetSize(Vector2 newSize, bool includeFrames = true)
	{
		newSize.x -= (rightFrame != null && includeFrames)?rightFrame.cachedTransform.localScale.x :0f;
		newSize.x -= (leftFrame != null && includeFrames)?leftFrame.cachedTransform.localScale.x :0f;
			
		newSize.y -= (bottomFrame != null && includeFrames)?bottomFrame.cachedTransform.localScale.y :0f;
		newSize.y -= (topFrame != null && includeFrames)?topFrame.cachedTransform.localScale.y :0f;
		
		if (target)
			target.transform.localScale = new Vector3(newSize.x, newSize.y, 1f);
		
		Commit();
	}
	
	public Vector2 GetSize(bool includeFrames = true)
	{
		Vector2 size = Vector2.zero;
		if (target)
			size = target.transform.localScale;
		
		size.x += (rightFrame != null && includeFrames)?rightFrame.cachedTransform.localScale.x :0f;
		size.x += (leftFrame != null && includeFrames)?leftFrame.cachedTransform.localScale.x :0f;
			
		size.y += (bottomFrame != null && includeFrames)?bottomFrame.cachedTransform.localScale.y :0f;
		size.y += (topFrame != null && includeFrames)?topFrame.cachedTransform.localScale.y :0f;
			
		return size;
	}
	
	public void Commit()
	{
		//Reset local window local scale. NOTE! window size should be changed via SetSize()
		transform.localScale = Vector3.one;
	
		Vector3 oldSize = Vector3.zero;
		if (target)
			oldSize = target.cachedTransform.localScale;
		
		oldSize.x = Mathf.Abs(oldSize.x);
		oldSize.y = Mathf.Abs(oldSize.y);
		oldSize.z = 1f;
		
		sizeStepX_Px = Mathf.Abs(sizeStepX_Px);
		if (sizeStepX_Px == 0)
			sizeStepX_Px = 1f;
		
		sizeStepY_Px = Mathf.Abs(sizeStepY_Px);
		if (sizeStepY_Px == 0)
			sizeStepY_Px = 1f;
		
		minWidthPx = Mathf.Abs(minWidthPx);
		if (minWidthPx == 0)
			minWidthPx = 1f;
		
		minHeightPx = Mathf.Abs(minHeightPx);
		if (minHeightPx == 0)
			minHeightPx = 1f;
		
		Vector3 newSize = new Vector3(Mathf.Ceil(oldSize.x/sizeStepX_Px)*sizeStepX_Px, Mathf.Ceil(oldSize.y/sizeStepY_Px)*sizeStepY_Px, oldSize.z);
		if (newSize.x <= minWidthPx)
			newSize.x = Mathf.Ceil(minWidthPx/sizeStepX_Px)*sizeStepX_Px;
		if (newSize.y <= minHeightPx)
			newSize.y = Mathf.Ceil(minHeightPx/sizeStepY_Px)*sizeStepY_Px;
		
		if (target)
		{
			target.cachedTransform.localScale = newSize;
		
			//all sprites are anchored to target sprite
			target.MakePixelPerfect();
		}
		
		
		//target positioning
	
		float halfTWidth = (target != null) ?target.cachedTransform.localScale.x*0.5f :0f;
		float halfTHeight = (target != null) ?target.cachedTransform.localScale.y*0.5f :0f;
		
		float fullWidth = halfTWidth*2f;
		fullWidth += (leftFrame != null) ?leftFrame.cachedTransform.localScale.x : 0f;
		fullWidth += (rightFrame != null) ?rightFrame.cachedTransform.localScale.x : 0f;
		
		float fullHeight = halfTHeight*2f;
		fullHeight += (topFrame != null) ?topFrame.cachedTransform.localScale.y : 0f;
		fullHeight += (bottomFrame != null) ?bottomFrame.cachedTransform.localScale.y : 0f;
		
		float centerPosY = 0f;
		centerPosY += (bottomFrame != null) ?bottomFrame.cachedTransform.localScale.y/2f :0f;
		centerPosY -= (topFrame != null) ?topFrame.cachedTransform.localScale.y/2f :0f;
		
		float centerPosX = 0f;
		centerPosX -= (rightFrame != null) ?rightFrame.cachedTransform.localScale.x/2f :0f;
		centerPosX += (leftFrame != null) ?leftFrame.cachedTransform.localScale.x/2f : 0f;
		
		switch (pivot)
		{
		case UIWidget.Pivot.Bottom:
			centerPosY += fullHeight/2f;
			break;
		case UIWidget.Pivot.BottomLeft:
			centerPosY += fullHeight/2f;
			centerPosX += fullWidth/2f;
			break;
		case UIWidget.Pivot.BottomRight:
			centerPosY += fullHeight/2f;
			centerPosX -= fullWidth/2f;
			break;
		case UIWidget.Pivot.Center:
			break;
		case UIWidget.Pivot.Left:
			centerPosX += fullWidth/2f;
			break;
		case UIWidget.Pivot.Right:
			centerPosX -= fullWidth/2f;
			break;
		case UIWidget.Pivot.Top:
			centerPosY -= fullHeight/2f;
			break;
		case UIWidget.Pivot.TopLeft:
			centerPosY -= fullHeight/2f;
			centerPosX += fullWidth/2f;
			break;
		case UIWidget.Pivot.TopRight:
			centerPosY -= fullHeight/2f;
			centerPosX -= fullWidth/2f;
			break;
		default:
			break;
		}
		
		Vector3 center = new Vector3(Mathf.RoundToInt(centerPosX), Mathf.RoundToInt(centerPosY), (target != null) ?Mathf.RoundToInt(target.cachedTransform.localPosition.z) :0f);
		
		if (target)
			target.cachedTransform.localPosition = center;
	
		float tPosX = center.x;
		float tPosY = center.y;
		
		fullWidth = halfTWidth*2f;
		fullHeight = halfTHeight*2f;
		float framesInset = 0f;
		
		//corners positioning
		if (topLeftCorner)
			topLeftCorner.cachedTransform.localPosition = new Vector3(tPosX-halfTWidth+framesInset, tPosY+halfTHeight-framesInset, topLeftCorner.cachedTransform.localPosition.z);
		if (topRightCorner)
			topRightCorner.cachedTransform.localPosition = new Vector3(tPosX+halfTWidth-framesInset, tPosY+halfTHeight-framesInset, topRightCorner.cachedTransform.localPosition.z);
		if (bottomLeftCorner)
			bottomLeftCorner.cachedTransform.localPosition = new Vector3(tPosX-halfTWidth+framesInset, tPosY-halfTHeight+framesInset, bottomLeftCorner.cachedTransform.localPosition.z);
		if (bottomRightCorner)
			bottomRightCorner.cachedTransform.localPosition = new Vector3(tPosX+halfTWidth-framesInset, tPosY-halfTHeight+framesInset, bottomRightCorner.cachedTransform.localPosition.z);
		
		//frames and shadows sizing
		if (topFrame)
		{
			UIWidget.Pivot pivot_ = topFrame.pivot;
			if (pivot_==UIWidget.Pivot.BottomRight || pivot_==UIWidget.Pivot.Right || pivot_==UIWidget.Pivot.TopRight)
				topFrame.cachedTransform.localScale = new Vector3(-fullWidth, topFrame.cachedTransform.localScale.y, 1f);
			else
				topFrame.cachedTransform.localScale = new Vector3(fullWidth, topFrame.cachedTransform.localScale.y, 1f);
		}
		if (topShadow)
		{
			UIWidget.Pivot pivot_ = topShadow.pivot;
			if (pivot_==UIWidget.Pivot.BottomRight || pivot_==UIWidget.Pivot.Right || pivot_==UIWidget.Pivot.TopRight)
				topShadow.cachedTransform.localScale = new Vector3(-fullWidth, topShadow.cachedTransform.localScale.y, 1f);
			else
				topShadow.cachedTransform.localScale = new Vector3(fullWidth, topShadow.cachedTransform.localScale.y, 1f);
		}
		if (bottomFrame)
		{
			UIWidget.Pivot pivot_ = bottomFrame.pivot;
			if (pivot_==UIWidget.Pivot.BottomRight || pivot_==UIWidget.Pivot.Right || pivot_==UIWidget.Pivot.TopRight)
				bottomFrame.cachedTransform.localScale = new Vector3(-fullWidth, bottomFrame.cachedTransform.localScale.y, 1f);
			else
				bottomFrame.cachedTransform.localScale = new Vector3(fullWidth, bottomFrame.cachedTransform.localScale.y, 1f);
		}
		if (bottomShadow)
		{
			UIWidget.Pivot pivot_ = bottomShadow.pivot;
			if (pivot_==UIWidget.Pivot.BottomRight || pivot_==UIWidget.Pivot.Right || pivot_==UIWidget.Pivot.TopRight)
				bottomShadow.cachedTransform.localScale = new Vector3(-fullWidth, bottomShadow.cachedTransform.localScale.y, 1f);
			else
				bottomShadow.cachedTransform.localScale = new Vector3(fullWidth, bottomShadow.cachedTransform.localScale.y, 1f);
		}
		
		if (leftFrame)
		{
			UIWidget.Pivot pivot_ = leftFrame.pivot;
			if (pivot_==UIWidget.Pivot.BottomLeft || pivot_==UIWidget.Pivot.BottomRight || pivot_==UIWidget.Pivot.Bottom)
				leftFrame.cachedTransform.localScale = new Vector3(leftFrame.cachedTransform.localScale.x, -fullHeight, 1f);
			else
				leftFrame.cachedTransform.localScale = new Vector3(leftFrame.cachedTransform.localScale.x, fullHeight, 1f);
		}
		if (leftShadow)
		{
			UIWidget.Pivot pivot_ = leftShadow.pivot;
			if (pivot_==UIWidget.Pivot.BottomLeft || pivot_==UIWidget.Pivot.BottomRight || pivot_==UIWidget.Pivot.Bottom)
				leftShadow.cachedTransform.localScale = new Vector3(leftShadow.cachedTransform.localScale.x, -fullHeight, 1f);
			else
				leftShadow.cachedTransform.localScale = new Vector3(leftShadow.cachedTransform.localScale.x, fullHeight, 1f);
		}
		
		if (rightFrame)
		{
			UIWidget.Pivot pivot_ = rightFrame.pivot;
			if (pivot_==UIWidget.Pivot.BottomLeft || pivot_==UIWidget.Pivot.BottomRight || pivot_==UIWidget.Pivot.Bottom)
				rightFrame.cachedTransform.localScale = new Vector3(rightFrame.cachedTransform.localScale.x, -fullHeight, 1f);
			else
				rightFrame.cachedTransform.localScale = new Vector3(rightFrame.cachedTransform.localScale.x, fullHeight, 1f);
		
		}
		if (rightShadow)
		{
			UIWidget.Pivot pivot_ = rightShadow.pivot;
			if (pivot_==UIWidget.Pivot.BottomLeft || pivot_==UIWidget.Pivot.BottomRight || pivot_==UIWidget.Pivot.Bottom)
				rightShadow.cachedTransform.localScale = new Vector3(rightShadow.cachedTransform.localScale.x, -fullHeight, 1f);
			else
				rightShadow.cachedTransform.localScale = new Vector3(rightShadow.cachedTransform.localScale.x, fullHeight, 1f);
		}
		
		//frames and shadows positioning
		if (topFrame)
		{
			UIWidget.Pivot pivot_ = topFrame.pivot;
			if (pivot_==UIWidget.Pivot.Center || pivot_==UIWidget.Pivot.Bottom || pivot_==UIWidget.Pivot.Top)
				topFrame.cachedTransform.localPosition = new Vector3(tPosX, tPosY+halfTHeight-framesInset, topFrame.cachedTransform.localPosition.z);
			else
				topFrame.cachedTransform.localPosition = new Vector3(tPosX-halfTWidth, tPosY+halfTHeight-framesInset, topFrame.cachedTransform.localPosition.z);	
		}
		if (topShadow)
		{
			UIWidget.Pivot pivot_ = topShadow.pivot;
			if (pivot_==UIWidget.Pivot.Center || pivot_==UIWidget.Pivot.Bottom || pivot_==UIWidget.Pivot.Top)
				topShadow.cachedTransform.localPosition = new Vector3(tPosX, tPosY+halfTHeight, topFrame.cachedTransform.localPosition.z);
			else
				topShadow.cachedTransform.localPosition = new Vector3(tPosX-halfTWidth, tPosY+halfTHeight, topShadow.cachedTransform.localPosition.z);
		}
		
		if (bottomFrame)
		{
			UIWidget.Pivot pivot_ = bottomFrame.pivot;
			if (pivot_==UIWidget.Pivot.Center || pivot_==UIWidget.Pivot.Bottom || pivot_==UIWidget.Pivot.Top)
				bottomFrame.cachedTransform.localPosition = new Vector3(tPosX, tPosY-halfTHeight+framesInset, bottomFrame.cachedTransform.localPosition.z);
			else
				bottomFrame.cachedTransform.localPosition = new Vector3(tPosX-halfTWidth, tPosY-halfTHeight+framesInset, bottomFrame.cachedTransform.localPosition.z);
			
		}
		if (bottomShadow)
		{
			UIWidget.Pivot pivot_ = bottomShadow.pivot;
			if (pivot_==UIWidget.Pivot.Center || pivot_==UIWidget.Pivot.Bottom || pivot_==UIWidget.Pivot.Top)
				bottomShadow.cachedTransform.localPosition = new Vector3(tPosX, tPosY-halfTHeight, bottomShadow.cachedTransform.localPosition.z);
			else
				bottomShadow.cachedTransform.localPosition = new Vector3(tPosX-halfTWidth, tPosY-halfTHeight, bottomShadow.cachedTransform.localPosition.z);
			
		}
		
		if (leftFrame)
		{
			UIWidget.Pivot pivot_ = leftFrame.pivot;
			if (pivot_==UIWidget.Pivot.Center || pivot_==UIWidget.Pivot.Left || pivot_==UIWidget.Pivot.Right)
				leftFrame.cachedTransform.localPosition = new Vector3(tPosX-halfTWidth+framesInset, tPosY, leftFrame.cachedTransform.localPosition.z);
			else
				leftFrame.cachedTransform.localPosition = new Vector3(tPosX-halfTWidth+framesInset, tPosY+halfTHeight, leftFrame.cachedTransform.localPosition.z);
			
		}
		if (leftShadow)
		{
			UIWidget.Pivot pivot_ = leftShadow.pivot;
			if (pivot_==UIWidget.Pivot.Center || pivot_==UIWidget.Pivot.Left || pivot_==UIWidget.Pivot.Right)
				leftShadow.cachedTransform.localPosition = new Vector3(tPosX-halfTWidth, tPosY, leftShadow.cachedTransform.localPosition.z);
			else
				leftShadow.cachedTransform.localPosition = new Vector3(tPosX-halfTWidth, tPosY+halfTHeight, leftShadow.cachedTransform.localPosition.z);

		}
		
		if (rightFrame)
		{
			UIWidget.Pivot pivot_ = rightFrame.pivot;
			if (pivot_==UIWidget.Pivot.Center || pivot_==UIWidget.Pivot.Left || pivot_==UIWidget.Pivot.Right)
				rightFrame.cachedTransform.localPosition = new Vector3(tPosX+halfTWidth-framesInset, tPosY, rightFrame.cachedTransform.localPosition.z);
			else
				rightFrame.cachedTransform.localPosition = new Vector3(tPosX+halfTWidth-framesInset, tPosY+halfTHeight, rightFrame.cachedTransform.localPosition.z);
			

		}
		if (rightShadow)
		{
			UIWidget.Pivot pivot_ = rightShadow.pivot;
			if (pivot_==UIWidget.Pivot.Center || pivot_==UIWidget.Pivot.Left || pivot_==UIWidget.Pivot.Right)
				rightShadow.cachedTransform.localPosition = new Vector3(tPosX+halfTWidth, tPosY, rightShadow.cachedTransform.localPosition.z);
			else
				rightShadow.cachedTransform.localPosition = new Vector3(tPosX+halfTWidth, tPosY+halfTHeight, rightShadow.cachedTransform.localPosition.z);

		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Application.isEditor && !Application.isPlaying)
		{
			Setup();
			Commit();
		}
	}
}
