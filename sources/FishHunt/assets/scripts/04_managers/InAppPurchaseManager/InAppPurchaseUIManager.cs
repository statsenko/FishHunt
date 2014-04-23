using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


/*
using Prime31;

public class InAppPurchaseProduct
{
	public string productId;
	public string currencySymbol;
	public string description;
	public string icon;
	public string title;
	public Dictionary<ECurrencyType, int> moneyProfit;
	public string price;
}

[ExecuteInEditMode]
public class InAppPurchaseUIManager : UIController 
{
	public MYLabel titleLabel = null;
	public UIScrollView scrollView = null;
	public Vector4 scrollViewInset = Vector4.zero;
	
	public GameObject cellPrefab = null;
	
	private List<InAppPurchaseProduct> _products = null;
	
	protected override void DoAwake() 
	{
		base.DoAwake();
	}
	
	protected override void DoStart() 
	{
		base.DoStart();

		Reposition();

		if (Application.isPlaying)
		{			
			UILoadingIndicatorFactory.Hide();
			titleLabel.text = GameStrings.GetLocalizedString(GameStrings.InAppPurchase_Title);
			RefreshTable();
			
			if (!InitializeSmartFoxConnection())
			{
				Debug.LogWarning("Connection not initialized" );

				string alertTitle = GameStrings.GetLocalizedString(GameStrings.ErrorAV_Title);
				string alertMessage = GameStrings.GetLocalizedString(GameStrings.SFS_InitErrorAV_Message);
				string alertConfirmButtonTitle = GameStrings.GetLocalizedString(GameStrings.Common_Continue);
			
				UIAlertViewFactory.ShowAlert(alertTitle, alertMessage, alertConfirmButtonTitle, null, gameObject, "BackToLoginScene");
				return;
			}

			if (!InitializeStoreKit())
			{
				Debug.LogWarning("Store kit can't make payments" );
				
				string alertTitle = GameStrings.GetLocalizedString(GameStrings.ErrorAV_Title);
				string alertMessage = GameStrings.GetLocalizedString(GameStrings.InAppPurchase_InitStoreKitErrorAV_Message);
				string alertConfirmButtonTitle = GameStrings.GetLocalizedString(GameStrings.Common_Continue);

				UIAlertViewFactory.ShowAlert(alertTitle, alertMessage, alertConfirmButtonTitle, null, gameObject, "BackToLoginScene");
				return;
			}
			
			_products = new List<InAppPurchaseProduct>();
			Debug.Log("Sending SFS InAppGetProducts request.");
			SmartFoxConnection.InAppGetProducts();
		}
	}
	
	protected override void DoUpdate() 
	{
		base.DoUpdate();
		
		if (!Application.isPlaying)
		{
			Reposition();
		}
	}
		
	void Reposition()
	{
		scrollView.pivot = UIWidget.Pivot.TopLeft;
		float xPos = -camWidth/2f + scrollViewInset.x;
		float yPos = camHeight/2f - scrollViewInset.y;
		float w = camWidth - scrollViewInset.x - scrollViewInset.z;
		float h = camHeight - scrollViewInset.y - scrollViewInset.w; 
		scrollView.transform.localPosition = new Vector3(xPos, yPos, scrollView.transform.localPosition.z);
		scrollView.Size = new Vector2(w, h);
	}
		
	void RefreshTable()
	{
		UITableExt table = scrollView.content.GetComponent<UITableExt>() as UITableExt;
		table.RemoveAllCells();
		
		if (_products != null && _products.Count > 0)
		{
			foreach (InAppPurchaseProduct product in _products)
			{
				GameObject cell = NGUITools.AddChild(scrollView.content.gameObject, cellPrefab);
			
				InAppPurchaseUITableCellController cellCtrl = cell.GetComponent<InAppPurchaseUITableCellController>();
				cellCtrl.SetupCell(product);
				cellCtrl.onBuyButtonPressed = OnBuyButton;
			}
		}
		Reposition();
	}
	
	bool InitializeSmartFoxConnection()
	{
		bool result = false;
		
		if (SmartFoxConnection.IsInitialized && SmartFoxConnection.IsLoggedIn)
		{
			SmartFoxConnection.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnSFSExtensionResponse);
			SmartFoxConnection.AddEventListener(SFSEvent.CONNECTION_LOST, OnSFSConnectionLost);
			result = true;
		}
		return result;		
	}
	
	void RemoveSmartFoxEventListners()
	{
		if (SmartFoxConnection.IsInitialized)
		{
			SmartFoxConnection.RemoveEventListner(SFSEvent.EXTENSION_RESPONSE, OnSFSExtensionResponse);
			SmartFoxConnection.RemoveEventListner(SFSEvent.CONNECTION_LOST, OnSFSConnectionLost);
		}
	}
	
	void DestroySmartFoxConnection()
	{
		RemoveSmartFoxEventListners();
		SmartFoxConnection.Disconnect();
	}
	
	bool InitializeStoreKit()
	{
		bool result = false;
		bool canMakePayments = StoreKitBinding.canMakePayments();
		if (canMakePayments)
		{
			SKTransactionsObserver.Instance().purchaseSuccessfulEvent = OnSKPurchaseSuccessful;
			SKTransactionsObserver.Instance().purchaseCancelledEvent = OnSKPurchaseCancelled;
			SKTransactionsObserver.Instance().purchaseFailedEvent = OnSKPurchaseFailed;
			SKTransactionsObserver.Instance().productListReceivedEvent = OnSKProductListReceived;
			SKTransactionsObserver.Instance().productListRequestFailedEvent = OnSKProductListRequestFailed;
			SKTransactionsObserver.Instance().productPurchaseAwaitingConfirmationEvent = OnSKProductPurchaseAwaitingConfirmation;

			CreateCmdQueue();
			result = true;
		}
		return result;
	}
	
	void RemoveStoreKitEventListners()
	{
		SKTransactionsObserver.Instance().productPurchaseAwaitingConfirmationEvent = null;
		SKTransactionsObserver.Instance().purchaseSuccessfulEvent = null;
		SKTransactionsObserver.Instance().purchaseCancelledEvent = null;
		SKTransactionsObserver.Instance().purchaseFailedEvent = null;
		SKTransactionsObserver.Instance().productListReceivedEvent = null;
		SKTransactionsObserver.Instance().productListRequestFailedEvent = null;
		
		CleanUpCmdQueue();
	}
	
//	SMARTFOX EVENTS
	public void OnSFSExtensionResponse(BaseEvent evt) 
	{
		SFSObject dataObject = (SFSObject)evt.Params["params"];
		
		bool success = true;
		string cmd = (string)evt.Params["cmd"];
		switch ( cmd ) 
		{	
		case "GetProducts":
		{
			SFSObject products = dataObject.ContainsKey("products") ?dataObject.GetSFSObject("products") as SFSObject :null;
			if (products == null)
			{
				Debug.LogError("ERROR: InAppPurchaseManager: Can not parse GetProducts sfs response: products key is missed or invalid");
				success = false;
			}
			else
			{
				string[] productsIds = products.GetKeys();
				
				foreach (string productId in productsIds)
				{
					InAppPurchaseProduct purchaseItem = new InAppPurchaseProduct();
					
					SFSObject sfsProduct = products.GetSFSObject(productId) as SFSObject;

					purchaseItem.productId = sfsProduct.ContainsKey("storeProductId") ? sfsProduct.GetUtfString("storeProductId") : "";
					purchaseItem.title = sfsProduct.ContainsKey("title") ? sfsProduct.GetUtfString("title") : "";
					purchaseItem.icon = sfsProduct.ContainsKey("icon") ? sfsProduct.GetUtfString("icon") : "";
					purchaseItem.description = sfsProduct.ContainsKey("description") ? sfsProduct.GetUtfString("description") : "";
					purchaseItem.moneyProfit = new Dictionary<ECurrencyType, int>();

					SFSObject sfs_money = sfsProduct.ContainsKey("moneyAmount") ? sfsProduct.GetSFSObject("moneyAmount") as SFSObject: null;

					if (sfs_money == null)
					{
						Debug.LogError("ERROR: InAppPurchaseManager: Can not parse GetProducts sfs response: moneyAmount key is missed or invalid");
						success = false;
						break;
					}

					foreach (string currencyTypeStr in sfs_money.GetKeys())
					{
						int currency = int.Parse(currencyTypeStr);
						int val = sfs_money.GetInt(currencyTypeStr);

						if( !MoneyManager.IsCorrectCurrency(currency))
						{	
							Debug.LogError("ERROR: InAppPurchaseManager: Can not parse GetProducts sfs response: unknown currency type: " + currency);
							success = false;
							break;
						}
						else
						{	
							purchaseItem.moneyProfit[(ECurrencyType)currency] = val; 
						}
					}	

					if (!success)
						break;
					else
						_products.Add(purchaseItem);
				}

				if (success)
				{
					Debug.Log("Sending SK products list request");
					StoreKitBinding.requestProductData(productsIds);
				}
			}

			if (!success)
			{
				UILoadingIndicatorFactory.Hide();
				
				string alertTitle = GameStrings.GetLocalizedString(GameStrings.ErrorAV_Title);
				string alertMessage = GameStrings.GetLocalizedString(GameStrings.InAppPurchase_GetProductsListErrorAV_Message);
				string alertConfirmButtonTitle = GameStrings.GetLocalizedString(GameStrings.Common_Continue);
			
				UIAlertViewFactory.ShowAlert(alertTitle, alertMessage, alertConfirmButtonTitle, null, null, null);
			}	

			break;
		}
		case "Commit":
		{
			if (_products == null || _products.Count <= 0)
			{
				EnqueueCmd(() => OnSFSExtensionResponse(evt));
				return;
			}

			success = dataObject.ContainsKey("success")? dataObject.GetBool("success") : false;

			if (success)
			{
				string transactionId = dataObject.ContainsKey("transaction")?dataObject.GetUtfString("transaction") :null;
				if (string.IsNullOrEmpty( transactionId ))
				{
					Debug.LogError("ERROR: InAppPurchaseManager: Can not parse Commit sfs response: transaction key is missed or invalid");
					success = false;
				}
				else
				{
					SFSObject sfs_money = dataObject.ContainsKey("money") ? dataObject.GetSFSObject("money") as SFSObject: null;
					if (sfs_money == null)
					{
						Debug.LogError("ERROR: InAppPurchaseManager: Can not parse Commit sfs response: money key is missed or invalid");
						success = false;
					}
					else
					{
						foreach (string currencyTypeStr in sfs_money.GetKeys())
						{
							int currency = int.Parse(currencyTypeStr);
							int val = sfs_money.GetInt(currencyTypeStr);

							if( !MoneyManager.IsCorrectCurrency(currency))
							{	
								Debug.LogError("ERROR: InAppPurchaseManager: Can not parse Commit sfs response: unknown currency type: " + currency);
								success = false;
								break;
							}
							else
							{	
								User.GetMoneyManager.IncreaseMoney(new KeyValuePair<ECurrencyType, int>((ECurrencyType)currency, val));
							}
						}
					}
				}

				if (success)
				{
					Debug.Log("Sending SK finish pending transaction id:" + transactionId);
					StoreKitBinding.finishPendingTransaction(transactionId);
				}
			}

			if (!success)
			{
				string error = dataObject.ContainsKey("errorMessage") ?dataObject.GetUtfString("errorMessage") :"UNKNOWN_ERROR";
				Debug.LogWarning("OnSFSExtensionResponse purchase.Commit FAILED: " + error);
				
				UILoadingIndicatorFactory.Hide();
				
				string alertTitle = GameStrings.GetLocalizedString(GameStrings.ErrorAV_Title);
				string alertMessage = GameStrings.GetLocalizedString(GameStrings.InAppPurchase_PurchaseErrorAV_Message);
				string alertConfirmButtonTitle = GameStrings.GetLocalizedString(GameStrings.Common_Continue);
				
				UIAlertViewFactory.ShowAlert(alertTitle, alertMessage, alertConfirmButtonTitle, null, null, null);
			}
			break;
		}
		default:
			break;
		}
	}
	
	public void OnSFSConnectionLost(BaseEvent evt) 
	{	
		Debug.LogWarning("OnSFSConnectionLost");
		
		RemoveStoreKitEventListners();
		DestroySmartFoxConnection();
		
		UILoadingIndicatorFactory.Hide();
		
		string alertTitle = GameStrings.GetLocalizedString(GameStrings.ErrorAV_Title);
		string alertMessage = GameStrings.GetLocalizedString(GameStrings.SFS_ConnectionLostErrorAV_Message);
		string alertConfirmButtonTitle = GameStrings.GetLocalizedString(GameStrings.Common_Continue);
			
		UIAlertViewFactory.ShowAlert(alertTitle, alertMessage, alertConfirmButtonTitle, null, gameObject, "BackToLoginScene");
	}
	
//	STOREKIT EVENTS
	void OnSKProductListReceived( List<StoreKitProduct> productList )
	{
		Debug.Log( "OnSKProductListReceived. Total products received: " + productList.Count );
		UILoadingIndicatorFactory.Hide();
		
		bool success = productList == null?false:productList.Count>0;
		if (success)
		{
			List<InAppPurchaseProduct> commonProducts = new List<InAppPurchaseProduct>();
			
			foreach (StoreKitProduct product in productList)
			{
				for (int i = 0; i < _products.Count; ++i)
				{
					if (_products[i].productId == product.productIdentifier)
					{
						InAppPurchaseProduct item = _products[i];
						item.currencySymbol = product.currencySymbol;
						item.price = product.price;
						
						commonProducts.Add(item);
						_products.Remove(item);
						break;
					}
				}
			}
			
			_products = new List<InAppPurchaseProduct>(commonProducts.OrderBy(product => product.productId));
			
			RefreshTable();
			
			bool queueIsEmpty = true;
			do
			{
				queueIsEmpty = !DequeueCmd();
			}while(!queueIsEmpty);
		}
		else
		{
			
			
			string alertTitle = GameStrings.GetLocalizedString(GameStrings.ErrorAV_Title);
			string alertMessage = GameStrings.GetLocalizedString(GameStrings.InAppPurchase_GetProductsListErrorAV_Message);
			string alertConfirmButtonTitle = GameStrings.GetLocalizedString(GameStrings.Common_Continue);

			UIAlertViewFactory.ShowAlert(alertTitle, alertMessage, alertConfirmButtonTitle, null, gameObject, "BackToLoginScene");
		}
	}
	
	void OnSKProductListRequestFailed( string error )
	{
		Debug.LogWarning( "OnSKProductListRequestFailed with error: " + error );
		UILoadingIndicatorFactory.Hide();
			
		string alertTitle = GameStrings.GetLocalizedString(GameStrings.ErrorAV_Title);
		string alertMessage = GameStrings.GetLocalizedString(GameStrings.InAppPurchase_GetProductsListErrorAV_Message);
		string alertConfirmButtonTitle = GameStrings.GetLocalizedString(GameStrings.Common_Continue);

		UIAlertViewFactory.ShowAlert(alertTitle, alertMessage, alertConfirmButtonTitle, null, gameObject, "BackToLoginScene");
	}
	
	void OnSKPurchaseFailed( string error )
	{
		Debug.LogWarning( "OnSKPurchaseFailed with error: " + error );
		
		if (_products == null || _products.Count <= 0)
		{
			EnqueueCmd(() => OnSKPurchaseFailed(error));
			return;
		}
		
		UILoadingIndicatorFactory.Hide();
					
		string alertTitle = GameStrings.GetLocalizedString(GameStrings.ErrorAV_Title);
		string alertMessage = GameStrings.GetLocalizedString(GameStrings.InAppPurchase_PurchaseErrorAV_Message);
		string alertConfirmButtonTitle = GameStrings.GetLocalizedString(GameStrings.Common_Continue);
				
		UIAlertViewFactory.ShowAlert(alertTitle, alertMessage, alertConfirmButtonTitle, null, null, null);
	}

	void OnSKPurchaseCancelled( string error )
	{
		Debug.LogWarning( "OnSKPurchaseCancelled with error: " + error );
		if (_products == null || _products.Count <= 0)
		{
			EnqueueCmd(() => OnSKPurchaseCancelled(error));
			return;
		}
		
		UILoadingIndicatorFactory.Hide();
	}

	void OnSKProductPurchaseAwaitingConfirmation( StoreKitTransaction transaction )
	{
		Debug.Log( "OnSKProductPurchaseAwaitingConfirmation: " + transaction );
		
		if (_products == null || _products.Count <= 0)
		{
			EnqueueCmd(() => OnSKProductPurchaseAwaitingConfirmation(transaction));
			return;
		}

		if (SmartFoxConnection.IsLoggedIn)
		{
			UILoadingIndicatorFactory.Show();
			
			Debug.Log("Sending SmartFox purchase.Commit");
			SmartFoxConnection.InAppCommit(transaction.base64EncodedTransactionReceipt);
		}
		else
		{
			RemoveStoreKitEventListners();
			DestroySmartFoxConnection();
			
			UILoadingIndicatorFactory.Hide();
		
			string alertTitle = GameStrings.GetLocalizedString(GameStrings.ErrorAV_Title);
			string alertMessage = GameStrings.GetLocalizedString(GameStrings.SFS_ConnectionLostErrorAV_Message);
			string alertConfirmButtonTitle = GameStrings.GetLocalizedString(GameStrings.Common_Continue);
		
			UIAlertViewFactory.ShowAlert(alertTitle, alertMessage, alertConfirmButtonTitle, null, gameObject, "BackToLoginScene");
		}
	}
	
	void OnSKPurchaseSuccessful( StoreKitTransaction transaction )
	{
		Debug.Log( "OnSKPurchaseSuccessful: " + transaction.productIdentifier);
		
		if (_products == null || _products.Count <= 0)
		{
			EnqueueCmd(() => OnSKPurchaseSuccessful(transaction));
			return;
		}
		
		UILoadingIndicatorFactory.Hide();
		
		string alertTitle = GameStrings.GetLocalizedString(GameStrings.InAppPurchase_PurchaseCompletedNotificationAV_Title);
		string alertMessage = GameStrings.GetLocalizedString(GameStrings.InAppPurchase_PurchaseCompletedNotificationAV_DefaultMessage);
		string alertConfirmButtonTitle = GameStrings.GetLocalizedString(GameStrings.Common_Continue);
	
		foreach (InAppPurchaseProduct p in _products)
		{
			if ( p.productId.Equals(transaction.productIdentifier))
			{
				InformFlurryBuy(p);
				string msgFormat = GameStrings.GetLocalizedString(GameStrings.InAppPurchase_PurchaseCompletedNotificationAV_SpecifiedMessage);
				alertMessage = string.Format( msgFormat, p.title );
				break;
			}
		} 
	
		UIAlertViewFactory.ShowAlert(alertTitle, alertMessage, alertConfirmButtonTitle, null, null, null, false);
	}
//	
	void OnBackButton()
	{
		AudioManager.PlaySfxClip(AudioManager.button_click_sfx, false);
		LoadPreviousScene();
	}
	
	void OnFinishTransactionsButton()
	{
		Debug.Log ("FINISHING OLD TRANSACTIONS");
		StoreKitBinding.finishPendingTransactions();
	}
	
	void OnBuyButton(InAppPurchaseUITableCellController sender)
	{
		UILoadingIndicatorFactory.Show();
		
		Debug.Log("Sending SK product purchase request. Product: " + sender.product.productId);
		StoreKitBinding.purchaseProduct(sender.product.productId, 1);
	}
	
	void LoadPreviousScene()
	{
		RemoveStoreKitEventListners();
		RemoveSmartFoxEventListners();
		
		UIAlertViewFactory.Destroy();	
		FadeTransition.PopLevel();
	}
	
	public override void BackToLoginScene()
	{
		RemoveStoreKitEventListners();
		RemoveSmartFoxEventListners();
		
		UIAlertViewFactory.Destroy();
		FadeTransition.PopToLevel("Login");
	}
	
	//	CMD Queue
	Queue<System.Action> cmdQueue = null;
	void CreateCmdQueue()
	{
		if (cmdQueue == null)
		{
			Debug.Log("Creating cmd queue");
			cmdQueue = new Queue<System.Action>();
		}
	}
	bool DequeueCmd()
	{
		CreateCmdQueue();
	
		if (cmdQueue.Count > 0)
		{
			Debug.Log("Dequeue cmd");
			System.Action cmd = cmdQueue.Dequeue();
			cmd.Invoke();
			return true;
		}
		return false;
	}
	void EnqueueCmd(System.Action action)
	{
		CreateCmdQueue();
		
		cmdQueue.Enqueue(action);
		Debug.Log("Cmd enqueued. Count: " + cmdQueue.Count);
	}
	void CleanUpCmdQueue()
	{
		if (cmdQueue != null)
		{
			Debug.Log("Clean up cmd queue");
			cmdQueue.Clear();
		}
	}
	
	void InformFlurryBuy(InAppPurchaseProduct product)
	{
		var eventParams = new Dictionary<string, string>();
		eventParams.Add("name", product.title);
		eventParams.Add("price", product.price.ToString());
		eventParams.Add("productId", product.productId);
		eventParams.Add("level", User.GetExpManager.LevelCount.ToString());
		FlurryBinding.logEventWithParameters(GameConstants.FlurryEvent_BuyInApp, eventParams, false);
	}
}
*/
