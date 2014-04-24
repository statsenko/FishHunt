using UnityEngine;
using System.Collections;

public class UICheckBoxExt : UICheckbox {

	public UILabel label = null;
	public Color checkedTextColor = Color.white;
	public Color checkedTextEffectColor = Color.gray;

	public Color uncheckedTextColor = Color.gray;
	public Color uncheckedTextEffectColor = Color.black;

	protected override void Set(bool state)
	{
		if (!mStarted || mChecked != state)
		{
			if (label != null) 
			{
				label.color = state ? checkedTextColor : uncheckedTextColor;
				label.effectColor = state ? checkedTextEffectColor : uncheckedTextEffectColor;
			}
		}
		base.Set(state);
	}
}
