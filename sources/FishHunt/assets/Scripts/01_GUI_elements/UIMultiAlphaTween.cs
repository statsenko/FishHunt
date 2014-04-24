using UnityEngine;
using System.Collections;

public class UIMultiAlphaTween : UITweener
{
	public float from = 1f;
	public float to = 1f;

	Transform mTrans;
	UIWidget[] mWidgets;

	/// <summary>
	/// Current alpha.
	/// </summary>

	public float alpha 
		{ 
			get { return mWidgets[0].alpha; } 
			set 
			{
				foreach (UIWidget mWidget in mWidgets)
					mWidget.alpha = value;
			} 
		}

	/// <summary>
	/// Find all needed components.
	/// </summary>
	
	protected override void OnEnable () 
	{ 
		base.OnEnable ();
		mWidgets = GetComponentsInChildren<UIWidget>(); 
	}

	/// <summary>
	/// Interpolate and update the alpha.
	/// </summary>

	override protected void OnUpdate (float factor, bool isFinished) { alpha = Mathf.Lerp(from, to, factor); }

	/// <summary>
	/// Start the tweening operation.
	/// </summary>

	static public UIMultiAlphaTween Begin (GameObject go, float duration, float alpha)
	{
		UIMultiAlphaTween comp = UITweener.Begin<UIMultiAlphaTween>(go, duration);
		comp.from = comp.alpha;
		comp.to = alpha;

		if (duration <= 0f)
		{
			comp.Sample(1f, true);
			comp.enabled = false;
		}
		return comp;
	}
}