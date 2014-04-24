using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class UIAnchoredPanel : UIController 
{
	protected	UIAnchor	rootAnchor = null;
	protected override void DoStart()
	{
		base.DoStart();
	}
	
	protected override void DoAwake()
	{	
		Transform rootAnchorTransform = transform.FindChild("Anchor");
		if (rootAnchorTransform)
			rootAnchor = rootAnchorTransform.GetComponent<UIAnchor>();
	}
	protected override void DoUpdate()
	{
		base.DoUpdate();
		
		//if (Application.isPlaying)
		{
			if (mCamera != null && rootAnchor != null)
			{
				rootAnchor.relativeOffset = new Vector2(transform.localPosition.x/camWidth, transform.localPosition.y/camHeight);
			}
		}
	}
}
