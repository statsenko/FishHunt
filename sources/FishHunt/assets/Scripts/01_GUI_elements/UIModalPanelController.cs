using UnityEngine;
using System.Collections;

public enum UIModalPanelControllerState
{
	Appearing,
	Disappearing,
	Hidden,
	Visible,
}

public class UIModalPanelController : UIAnchoredPanel {

	protected static float kAppearDurationDefault = 0.75f;
	protected static float kDisappearDurationDefault = 0.15f;
	
	public float appearDuration = kAppearDurationDefault;
	public float disappearDuration = kDisappearDurationDefault;
	
	protected string callWhenAppear = null;
	protected string callWhenDisappear = null;
	protected GameObject eventListner = null;
	protected bool deactivateWhenFinished;
	
	protected UIModalPanelControllerState state = UIModalPanelControllerState.Visible;
	
	public bool Hidden
	{
		get {return state==UIModalPanelControllerState.Hidden;}
	}
	public bool Visible
	{
		get {return state==UIModalPanelControllerState.Visible;}
	}
	public bool Appearing
	{
		get {return state==UIModalPanelControllerState.Appearing;}
	}
	public bool Disappearing
	{
		get {return state==UIModalPanelControllerState.Disappearing;}
	}
	
	public virtual void Hide(bool animated_, GameObject eventListner_ = null, string callWhenAppear_= null, bool deactivateWhenFinished_= false)
	{
		throw new UnityException("override this function");
	}
	
	public virtual void Show(bool animated_, GameObject eventListner_ = null, string callWhenAppear_ = null)
	{
		throw new UnityException("override this function");
	}
	
	protected void SetupShowMode(bool animated_ = false, GameObject eventListner_ = null, string callWhenAppear_ = null)
	{
		state = UIModalPanelControllerState.Appearing;
		
		NGUITools.SetActive(gameObject, true);
		callWhenAppear = callWhenAppear_;
		eventListner = eventListner_;
	}
	
	protected void SetupHideMode(bool animated_ = false, GameObject eventListner_ = null, string callWhenDisappear_ = null, bool deactivateWhenFinished_ = true)
	{
		state = UIModalPanelControllerState.Disappearing;
		
		deactivateWhenFinished = deactivateWhenFinished_;
		callWhenDisappear = callWhenDisappear_;
		eventListner = eventListner_;
	}
	
	public virtual void OnAppear()
	{
		state = UIModalPanelControllerState.Visible;
		
		if (eventListner && !string.IsNullOrEmpty(callWhenAppear) )
			eventListner.SendMessage(callWhenAppear, SendMessageOptions.DontRequireReceiver);
	}
	
	public virtual void OnDisappear()
	{
		state = UIModalPanelControllerState.Hidden;
		if (deactivateWhenFinished)
			NGUITools.SetActive(gameObject, false);
		
		if (eventListner && !string.IsNullOrEmpty(callWhenDisappear) )
			eventListner.SendMessage(callWhenDisappear, SendMessageOptions.DontRequireReceiver);
	}

	protected override void DoAwake ()
	{
		base.DoAwake();
	}
}
