using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/*
 using Prime31;

public class SKTransactionsObserver : MonoBehaviour 
{
	private static SKTransactionsObserver mInstance; 	
	public static void Init()
	{
		if (mInstance == null)
		{
			Debug.Log("SKTransactionsObserver: Initialize SKTransactionsObserver...");
            mInstance = new GameObject("_SKTransactionsObserver").AddComponent(typeof(SKTransactionsObserver)) as SKTransactionsObserver;
			
			StoreKitManager.autoConfirmTransactions = false;
			DontDestroyOnLoad(mInstance.gameObject);
        }
	}
	
	public static SKTransactionsObserver Instance()
	{
		SKTransactionsObserver.Init();
		return mInstance;
	}
	
	Dictionary<string, StoreKitTransaction> notConfirmedTransactions = new Dictionary<string, StoreKitTransaction>();
	
	public delegate void OnProductListReceivedEvent(List<StoreKitProduct> productsList_);
	public OnProductListReceivedEvent productListReceivedEvent = null;
	
	public delegate void OnProductListRequestFailedEvent(string error);
	public OnProductListRequestFailedEvent productListRequestFailedEvent = null;
	
	public delegate void OnProductPurchaseAwaitingConfirmationEvent(StoreKitTransaction transaction);
	
	private OnProductPurchaseAwaitingConfirmationEvent productPurchaseAwaitingConfirmationEvent_;
	public OnProductPurchaseAwaitingConfirmationEvent productPurchaseAwaitingConfirmationEvent
	{
		get 
		{
			return productPurchaseAwaitingConfirmationEvent_;
		}
		set 
		{
			productPurchaseAwaitingConfirmationEvent_ = value;
	
			if (productPurchaseAwaitingConfirmationEvent_ != null)
			{
				foreach (StoreKitTransaction transaction in notConfirmedTransactions.Values)
				{
					productPurchaseAwaitingConfirmationEvent_(transaction);
				}
			}
		}
	}
	
	public delegate void OnPurchaseSuccessfulEvent(StoreKitTransaction transaction);
	public OnPurchaseSuccessfulEvent purchaseSuccessfulEvent = null;
	
	public delegate void OnPurchaseFailedEvent(string error);
	public OnPurchaseFailedEvent purchaseFailedEvent = null;
	
	public delegate void OnPurchaseCancelledEvent(string error);
	public OnPurchaseCancelledEvent purchaseCancelledEvent = null;

	void OnEnable()
	{
		if (StoreKitBinding.canMakePayments())
		{
			// Listens to all the StoreKit events.  All event listeners MUST be removed before this object is disposed!
			StoreKitManager.productPurchaseAwaitingConfirmationEvent += productPurchaseAwaitingConfirmation;
			StoreKitManager.purchaseSuccessfulEvent += purchaseSuccessful;
			StoreKitManager.purchaseCancelledEvent += purchaseCancelled;
			StoreKitManager.purchaseFailedEvent += purchaseFailed;
			
			StoreKitManager.productListReceivedEvent += productListReceived;
			StoreKitManager.productListRequestFailedEvent += productListRequestFailed;
		}
		else
		{
			Debug.LogWarning("WARNING: SKTransactionsObserver: Can not make payments");
		}
		
	}
	
	
	void OnDisable()
	{
		// Remove all the event handlers
		StoreKitManager.productPurchaseAwaitingConfirmationEvent -= productPurchaseAwaitingConfirmation;
		StoreKitManager.purchaseSuccessfulEvent -= purchaseSuccessful;
		StoreKitManager.purchaseCancelledEvent -= purchaseCancelled;
		StoreKitManager.purchaseFailedEvent -= purchaseFailed;
		
		StoreKitManager.productListReceivedEvent -= productListReceived;
		StoreKitManager.productListRequestFailedEvent -= productListRequestFailed;
	}
	
	
	void productListReceived( List<StoreKitProduct> productList )
	{
		Debug.Log( "SKTransactionsObserver: productListReceivedEvent. total products received: " + productList.Count );
		if (productListReceivedEvent != null)
			productListReceivedEvent(productList);
	
	}
	
	void productListRequestFailed( string error )
	{
		Debug.LogError( "ERROR: SKTransactionsObserver: productListRequestFailed: " + error );
		if (productListRequestFailedEvent != null)
			productListRequestFailedEvent (error);
	}

	void purchaseFailed( string error )
	{
		Debug.LogError( "ERROR: SKTransactionsObserver: purchase failed with error: " + error );
		if (purchaseFailedEvent != null)
			purchaseFailedEvent(error);
	}
	
	void purchaseCancelled( string error )
	{
		Debug.Log( "SKTransactionsObserver: purchase cancelled with error: " + error );
		if (purchaseCancelledEvent != null)
			purchaseCancelledEvent(error);
	}

	void productPurchaseAwaitingConfirmation( StoreKitTransaction transaction )
	{
		Debug.Log( "SKTransactionsObserver: productPurchaseAwaitingConfirmationEvent: " + transaction );
		notConfirmedTransactions.Add(transaction.transactionIdentifier, transaction);

		if (productPurchaseAwaitingConfirmationEvent_ != null)
		{
			productPurchaseAwaitingConfirmationEvent_(transaction);
		}
	}
	
	void purchaseSuccessful( StoreKitTransaction transaction )
	{
		Debug.Log( "SKTransactionsObserver: purchased product: " + transaction );

		notConfirmedTransactions.Remove(transaction.transactionIdentifier);
		
		if (purchaseSuccessfulEvent != null)
			purchaseSuccessfulEvent(transaction);
	}
}
*/
