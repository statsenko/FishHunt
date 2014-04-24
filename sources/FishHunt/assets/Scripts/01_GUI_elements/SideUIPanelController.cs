using UnityEngine;
using System.Collections;

public class SideUIPanelController : UIModalPanelController
{
	protected static float kLockUIAnimationDuration = 0.35f;
	protected static float kUnlockUIAnimationDuration = 0.35f;
	
	public bool hideOnStart = false;
	public Vector2 relativePositionOnShow = new Vector2(0,0f);
	public Vector2 relativePositionOnHide = new Vector2(-1f,0f);
	
	public UIStretchWindow window = null;
	public Vector4 windowInset = Vector4.zero;
	
	public UIScrollView scrollView = null;
	public Vector4 scrollViewInset = Vector4.zero;

	public UISprite uiLockSprite = null;
	
	public UILabel sideButtonLabel = null;
	public UILabel titleLabel = null;
	
	protected TweenPosition positionTweener = null;
	protected float	showDepthOffset = -6;
	public	float	hideDepthOffset = -1;
	
	protected override void DoAwake()
	{
		base.DoAwake();
		
		if (rootAnchor == null)
			Debug.LogWarning("can't find Anchor in SideUIPanelContrller");
	}
	
	protected override void DoStart ()
	{
		base.DoStart();
	
		if (Application.isPlaying)
		{
			UnlockUI();
			if (hideOnStart)
			{
				Hide(false);
			}
			else
			{
				Show(false);
			}
			ResizeWindow();
		}
	}
	
	protected override void DoUpdate()
	{
		if (Application.isEditor && !Application.isPlaying)
		{
			SetHideOnStart(hideOnStart);
			ResizeWindow();
		}

		base.DoUpdate();
	}
	
	protected void SetHideOnStart(bool hide_)
	{
		Vector3 relativePos_ = hide_ ?relativePositionOnHide :relativePositionOnShow;
		transform.localPosition = new Vector3(relativePos_.x*camWidth, relativePos_.y*camHeight, transform.localPosition.z);
			
		if (rootAnchor != null)
		{
			float zPosition_ = (hide_ ?hideDepthOffset :showDepthOffset) *100f;
			rootAnchor.transform.localPosition = new Vector3(rootAnchor.transform.localPosition.x, rootAnchor.transform.localPosition.y, zPosition_);
		}
	}
	
	protected virtual bool ResizeWindow()
	{
		if (mCamera != null && window != null)
		{
			// resize window
			Vector2 newSize = new Vector2(camWidth-Mathf.Abs(windowInset.x)-Mathf.Abs(windowInset.z), camHeight-Mathf.Abs(windowInset.y)-Mathf.Abs(windowInset.w));
			
			window.SetSize(newSize);
			window.transform.localPosition = new Vector3(-windowInset.x, -windowInset.y, window.transform.localPosition.z);
			window.Commit();
				
			// resize root collider:
			BoxCollider boxCollider = NGUITools.AddWidgetCollider(window.gameObject);
			boxCollider.size = new Vector3(newSize.x, newSize.y, boxCollider.size.z);
			Vector3 center = Vector3.zero;
			center.z = 0f;
			switch (window.pivot)
			{
			case UIWidget.Pivot.Left:
				center.x = newSize.x*0.5f;
				break;
			case UIWidget.Pivot.TopLeft:
				center.x = newSize.x*0.5f;
				center.y = -newSize.y*0.5f;
				break;
			case UIWidget.Pivot.BottomLeft:
				center.x = newSize.x*0.5f;
				center.y = newSize.y*0.5f;
				break;
			case UIWidget.Pivot.Right:
				center.x = -newSize.x*0.5f;
				break;
			case UIWidget.Pivot.TopRight:
				center.x = -newSize.x*0.5f;
				center.y = -newSize.y*0.5f;
				break;
			case UIWidget.Pivot.BottomRight:
				center.x = -newSize.x*0.5f;
				center.y = newSize.y*0.5f;
				break;
			case UIWidget.Pivot.Center:
				break;
			case UIWidget.Pivot.Top:
				center.y = -newSize.y*0.5f;
				break;
			case UIWidget.Pivot.Bottom:
				center.y = newSize.y*0.5f;
				break;
			default:
				break;
			}
			
			boxCollider.center = center;
		
			// resize scroll view:
			if (scrollView != null)
			{
				Vector2 oldDragAmount = scrollView.DragAmount;
				
				Vector2 scrollViewSize = new Vector2(newSize.x - Mathf.Abs(scrollViewInset.x) - Mathf.Abs(scrollViewInset.z), newSize.y - Mathf.Abs(scrollViewInset.y)-Mathf.Abs(scrollViewInset.w));
				scrollView.transform.localPosition = new Vector3(-scrollViewInset.x, -scrollViewInset.y, scrollView.transform.localPosition.z);
				scrollView.Size = scrollViewSize;
				scrollView.DragAmount = oldDragAmount;
				
				if (uiLockSprite != null)
					uiLockSprite.transform.localScale = new Vector3(scrollViewSize.x, scrollViewSize.y, 1f);
			}
			
			return true;
		}
		return false;
	}
		
	public virtual void LockUI()
	{
		if (uiLockSprite != null)
		{
			NGUITools.SetActive(uiLockSprite.gameObject, true);
			if (uiLockSprite.collider != null)
				uiLockSprite.collider.enabled = true;
			else
				NGUITools.AddWidgetCollider(uiLockSprite.gameObject);
			
			TweenAlpha.Begin(uiLockSprite.gameObject, kLockUIAnimationDuration, 1.0f);
		}
	}
	
	public virtual void UnlockUI()
	{
		if (uiLockSprite != null)
		{
			if (uiLockSprite.collider != null)
				uiLockSprite.collider.enabled = false;
			
			NGUITools.SetActive(uiLockSprite.gameObject, true);
			TweenAlpha.Begin(uiLockSprite.gameObject, kUnlockUIAnimationDuration, 0.0f);
		}
	}
	
	public override void Show(bool animated_, GameObject eventListner_ = null, string callWhenAppear_ = null)
	{
		if (rootAnchor != null)
			rootAnchor.transform.localPosition = new Vector3(rootAnchor.transform.localPosition.x, rootAnchor.transform.localPosition.y, showDepthOffset*100f);
		SetupShowMode(animated_, eventListner_, callWhenAppear_);
		UpdateScreenSize();
		if (animated_ == true)
		{
			base.state = UIModalPanelControllerState.Appearing;
			//setup appear position tweener:
			positionTweener = TweenPosition.Begin<TweenPosition>(gameObject, appearDuration);

			positionTweener.from = transform.localPosition;
			positionTweener.to = new Vector3(relativePositionOnShow.x*camWidth, relativePositionOnShow.y*camHeight, transform.localPosition.z);
			
			positionTweener.method = UITweener.Method.EaseInOut;
			positionTweener.callWhenFinished = "OnAppear";
			positionTweener.eventReceiver = gameObject;
	
//			AudioManager.PlaySfxClip(AudioManager.lobby_panel_open_sfx);
		}
		else
		{
			transform.localPosition = new Vector3(relativePositionOnShow.x*camWidth, relativePositionOnShow.y*camHeight, transform.localPosition.z);
			OnAppear();
		}
	}
			
	public override void Hide(bool animated_, GameObject eventListner_ = null, string callWhenDisappear_ = null, bool deactivateWhenFinished_ = false)
	{
		if (Hidden)
		{
			SetupHideMode(animated_, eventListner_, callWhenDisappear_, deactivateWhenFinished_);
			OnDisappear();
		}
		else
		{
			SetupHideMode(animated_, eventListner_, callWhenDisappear_, deactivateWhenFinished_);
			UpdateScreenSize();
			if (animated_ == true && !Hidden)
			{			
				//setup appear position tweener:

				positionTweener = TweenPosition.Begin<TweenPosition>(gameObject, disappearDuration);
			
				positionTweener.from = transform.localPosition;
				positionTweener.to =  new Vector3(relativePositionOnHide.x*camWidth, relativePositionOnHide.y*camHeight, transform.localPosition.z);
		
				positionTweener.method = UITweener.Method.EaseInOut;
				positionTweener.callWhenFinished = "OnDisappear";
				positionTweener.eventReceiver = gameObject;

//				AudioManager.PlaySfxClip(AudioManager.button_click_sfx);
			}
			else
			{
				transform.localPosition =  new Vector3(relativePositionOnHide.x*camWidth, relativePositionOnHide.y*camHeight, transform.localPosition.z);
				OnDisappear();
			}
		}
	}
	
	public override void OnAppear()
	{
		base.OnAppear();
	}
	
	public override void OnDisappear()
	{
		if (rootAnchor != null)
			rootAnchor.transform.localPosition = new Vector3(rootAnchor.transform.localPosition.x, rootAnchor.transform.localPosition.y, hideDepthOffset*100f);
		base.OnDisappear();
	}
}
