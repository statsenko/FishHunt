using UnityEngine;
using System.Collections;

/*
using Prime31;

public class InAppPurchaseUITableCellController : MonoBehaviour 
{
	public MYLabel titleLabel = null;
	public MYLabel descriptionLabel = null;
	public MYLabel priceLabel = null;
	public UISprite iconSprite = null;
	
	public InAppPurchaseProduct product = null;
	public delegate void OnBuyButtonPressed(InAppPurchaseUITableCellController sender);
	public OnBuyButtonPressed onBuyButtonPressed;

	void OnBuyButton()
	{
		AudioManager.PlaySfxClip(AudioManager.button_click_sfx, false);
		if (onBuyButtonPressed != null)
			onBuyButtonPressed(this);
	}
	
	public void SetupCell(InAppPurchaseProduct product_)
	{
		product = product_;
		
		titleLabel.text = product.title;
		descriptionLabel.text = product.description;
		
		string priceStringFormat = "{0} {1}";
		priceLabel.text = string.Format(priceStringFormat, product.price, product.currencySymbol);
		
		iconSprite.spriteName = product_.icon;
	}
}
*/
