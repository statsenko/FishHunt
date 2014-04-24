using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class UIDraggablePanelExt : UIDraggablePanel {
	
	public Transform content = null;

	protected bool mStarted = false;

	public override Bounds bounds
	{
		get
		{
			if (!mCalculatedBounds)
			{
				mCalculatedBounds = true;
				mBounds = NGUIMath.CalculateRelativeWidgetBounds(mTrans,content!=null?content :mTrans);
			}
			return mBounds;
		}
	}
	
	public Vector2 CurrentDragAmount()
	{
		Bounds b = bounds;
		if (b.min.x == b.max.x || b.min.y == b.max.x) return relativePositionOnReset;
		
		Vector4 cr = panel.clipRange;
		 
		float hx = cr.z * 0.5f;
		float hy = cr.w * 0.5f;
		float left = b.min.x + hx;
		float right = b.max.x - hx;
		float bottom = b.min.y + hy;
		float top = b.max.y - hy;

		if (panel.clipping == UIDrawCall.Clipping.SoftClip)
		{
			left -= panel.clipSoftness.x;
			right += panel.clipSoftness.x;
			bottom -= panel.clipSoftness.y;
			top += panel.clipSoftness.y;
		}
		
		
		// Calculate the offset based on the scroll value
		float ox = (cr.x-left)/(right-left);
		float oy = (cr.y-top)/(bottom-top);

		return new Vector2(ox, oy);
	}

	public void Update()
	{
		if (!Application.isPlaying || !mStarted)
		{
			mStarted = true;
			ResetPosition();
		}
	}
}
