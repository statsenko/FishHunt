using UnityEngine;
using System.Collections;

/*
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Util;
using Sfs2X.Entities;
using Sfs2X.Requests;
using Sfs2X.Logging;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Invitation;
using Sfs2X.Requests.Game;
using System.Collections.Generic;

// Statics for holding the connection to the SFS server end
// Can then be queried from the entire game to get the connection


public class SmartFoxConnection : MonoBehaviour
{	
	static string serverName = "gamechefsstory.belprog.com";
#if TEST_SERVER
	static int serverPort = 9935;
#elif DEV_SERVER
	static int serverPort = 9934;
#else
	static int serverPort = 9936;
#endif
	
	static float kPingTime = 60f;
	static string zone = "BeJeweledPvP";
	static string mainLobbyRoomName = "Main Lobby";
	static bool debug = false;
	
	private static SmartFoxConnection mInstance; 
	
	public static SmartFoxConnection Instance
	{
		get 
		{
			if (mInstance == null)
			{
				GameObject go = new GameObject("_SmartFoxConnection");
				mInstance = go.AddComponent(typeof(SmartFoxConnection)) as SmartFoxConnection;
				DontDestroyOnLoad(go);
            }
            return mInstance;
		}
	}
	
	public static int LagValue
	{
		get {return Instance.LagValue_;}
	}
	
	public static string Address
	{
		get {return serverName + ":"+serverPort;}
	}
	
	public static SmartFox Connection 
	{
		get {return Instance.SmartFox_;}
     	set 
		{
			Instance.SmartFox_ = value;
			if (Instance.SmartFox_ != null)
			{
				AddEventListener(SFSEvent.LOGIN, mInstance.OnSFSLogin);
				AddEventListener(SFSEvent.LOGOUT, mInstance.OnSFSLogout);
				AddEventListener(SFSEvent.PING_PONG, mInstance.OnSFSPingPong);
				AddEventListener(SFSEvent.CONNECTION_LOST, Instance.OnSFSConnection);
				AddEventListener(SFSEvent.CONNECTION, Instance.OnSFSConnection);
			} 
		}
	}
		
	public static bool Initialize()
	{
		string ErrorMessage = null;
		return Instance.Initialize_(out ErrorMessage);
	}
	
	public static bool Initialize(out string ErrorMessage)
	{
		return Instance.Initialize_(out ErrorMessage);
	}
	
	public static bool IsInitialized 
	{
		get {return Connection != null;}
	}
	
	public static void Connect( float maximumIdleTime )
	{
		Debug.Log(string.Format("SmartFoxConnection: Connecting to server: {0} at port: {1}", serverName, serverPort.ToString()));

		if (IsInitialized)
		{
			ConfigData config = new ConfigData();
			config.BlueBoxPollingRate = 500;
			config.Debug = debug;
			config.Host = serverName;
			config.Port = serverPort;
			config.HttpPort = 8080;
			config.UseBlueBox = true;
			config.Zone = zone;
		
			Connection.Connect(config);
			Instance.StartCoroutine("ConnectionIdleTimer", maximumIdleTime );
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not initialized. Call SmartFoxConnection.Initialize() first");
	}
	
	IEnumerator ConnectionIdleTimer(float maximumIdleTime)
	{	
		yield return new WaitForSeconds(maximumIdleTime);
		Connection.Disconnect();
		Connection.Dispatcher.DispatchEvent( new BaseEvent(SFSEvent.CONNECTION_LOST));
	}
	
	public static void Disconnect()
	{
		Debug.Log("SmartFoxConnection: Disconnect");
		
		if (IsInitialized)
		{
			SmartFoxConnection.RemoveEventListner(SFSEvent.LOGIN, Instance.OnSFSLogin);
			SmartFoxConnection.RemoveEventListner(SFSEvent.LOGOUT, Instance.OnSFSLogout);
			SmartFoxConnection.RemoveEventListner(SFSEvent.PING_PONG, Instance.OnSFSPingPong);
			SmartFoxConnection.RemoveEventListner(SFSEvent.CONNECTION_LOST, Instance.OnSFSConnection);
			SmartFoxConnection.RemoveEventListner(SFSEvent.CONNECTION, Instance.OnSFSConnection);
			if (IsConnected)
			{
				Debug.Log("SmartFoxConnection: Disconnecting from server...");

				Connection.Disconnect();
			}
			
			Debug.Log("SmartFoxConnection: Destroying SmartFoxConnection instance...");

			Destroy(mInstance.gameObject);
			mInstance = null;
		}
	}

	public static bool IsConnected
	{
		get
		{
			if (IsInitialized)
			{
				return Connection.IsConnected;
			}
			return false;
		}
	}
	
	public static void Logout()
	{
		Debug.Log(string.Format("SmartFoxConnection: Send LogoutRequest"));

		if (IsLoggedIn)
		{
			Connection.Send(new LogoutRequest());
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not logged in");
	}
	
	public static void RemoveGuestAccount()
	{
		Debug.Log(string.Format("SmartFoxConnection: Send character.RemoveGuest cmd"));

		if (IsLoggedIn)
		{
			SFSObject Params = new SFSObject();
			Connection.Send(new ExtensionRequest("character.RemoveGuest", Params));
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not logged in");
	}
	
	public static void Login(string userName, string password = null, AuthorizationType type = AuthorizationType.NA, string fbTitle = "", bool createIfNotExists = true)
	{
		Debug.Log(string.Format("SmartFoxConnection: Send LoginRequest: login = {0}", userName));

		if (IsConnected)
		{
			if (!IsLoggedIn)
			{
				ISFSObject params_ = new SFSObject();
				params_.PutInt("authType", (int)type);
				params_.PutBool("createIfNotExists", createIfNotExists);
				params_.PutUtfString("passwordHash", password);
				params_.PutBool("kickIfLogged", true);
				params_.PutUtfString("facebookTitle", fbTitle);
				params_.PutUtfString("version", OSInfo.BundleVersion);

				Connection.Send(new LoginRequest(userName, password, zone, params_));
			}
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not connected. Call SmartFoxConnection.Initialize(), than SmartFoxConnection.Connect() first");
	}
	
	public static bool IsLoggedIn
	{
		get 
		{
			if (IsConnected)
			{
				return Connection.MySelf != null;
			}
			return false;
		}
		
	}
	
	public static void JoinRoom(string roomName)
	{
		Debug.Log(string.Format("SmartFoxConnection: Send JoinRoomRequest. roomName = {0}", roomName));

		if (IsConnected)
			Connection.Send(new JoinRoomRequest(roomName));
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not connected. Call SmartFoxConnection.Initialize(), than SmartFoxConnection.Connect() first");
	}
	
	public static Room LastJoinedRoom
	{
		get
		{
			if (IsInitialized)
			{
				return Connection.LastJoinedRoom;
			}
			else
				Debug.LogError("ERROR: SmartFoxConnection: Can't get LastJoinedRoom. SmartFox not initialized. Call SmartFoxConnection.Initialize() first");

			return null;
		}
	}
	
	public static void JoinLobby()
	{
		if (IsConnected)
			JoinRoom(mainLobbyRoomName);
		else
			Debug.LogError("ERROR: SmartFoxConnection: Can't JoinLobby. SmartFox not connected. Call SmartFoxConnection.Initialize(), than SmartFoxConnection.Connect() first");
	}
	
	// Game Commands
	public static void InitGame() 
	{
		Debug.Log(string.Format("SmartFoxConnection: Send game.Init cmd"));

		if (IsLoggedIn)
		{
			SFSObject Params = new SFSObject();
			Connection.Send(new ExtensionRequest("game.Init", Params, Connection.LastJoinedRoom));
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not logged in");
	}
	
	public static void ReadyForGame()
	{
		Debug.Log(string.Format("SmartFoxConnection: Send game.Ready cmd"));
		
		if (IsLoggedIn)
		{
			SFSObject Params = new SFSObject();
			Connection.Send(new ExtensionRequest("game.Ready", Params, Connection.LastJoinedRoom));
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not logged in");
	}

	public static void JoinGameInTutorial()
	{
		Debug.Log("SmartFoxConnection: Send match.JoinGame in tutorial cmd");
		
		if (IsLoggedIn)
		{
			SFSObject Params = new SFSObject();
			Params.PutBool("isTutorial", true);
			Connection.Send(new ExtensionRequest("match.JoinGame", Params));
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not logged in");
	}
	
	public static void JoinGameAtCafe(int cafeId, bool isNeedBot)
	{
		Debug.Log(string.Format("SmartFoxConnection: Send match.JoinGame cmd. cafeId = {0}", cafeId));
		
		if (IsLoggedIn)
		{
			SFSObject Params = new SFSObject();
			Params.PutInt("cafeId", cafeId);
			Params.PutBool("isBot", isNeedBot);
			Connection.Send(new ExtensionRequest("match.JoinGame", Params));
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not logged in");
	}

	public static void JoinGameWithBoss(int bossId)
	{
		Debug.Log(string.Format("SmartFoxConnection: Send match.JoinGame cmd. bossId = {0}", bossId));
		
		if (IsLoggedIn)
		{
			SFSObject Params = new SFSObject();
			Params.PutInt("bossId", bossId);
			Connection.Send(new ExtensionRequest("match.JoinGame", Params));
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not logged in");
	}

	public static void StartTurn()
	{
		Debug.Log(string.Format("SmartFoxConnection: Send game.StartTurn cmd"));
		
		if (IsLoggedIn)
		{
			SFSObject Params = new SFSObject();
			Params.PutInt("lagValue", LagValue);
			Connection.Send(new ExtensionRequest("game.StartTurn", Params, Connection.LastJoinedRoom));
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not logged in");
	}
	
	public static void Turn(Vector2i fromPos, Vector2i toPos, int selectedAbility_)
	{
		Debug.Log (string.Format ("SmartFoxConnection: Send game.Turn cmd. colFrom = {0}, rowFrom = {1}, colTo = {2}, rowTo = {3}, ability = {4}", fromPos.col, fromPos.row, toPos.col, toPos.row, selectedAbility_));
		
		if (IsLoggedIn)
		{
			SFSObject Params = new SFSObject();
			Params.PutInt("colFrom", fromPos.col);
			Params.PutInt("rowFrom", fromPos.row);
			Params.PutInt("colTo", toPos.col);
			Params.PutInt("rowTo", toPos.row);
			if (selectedAbility_ != AbilitiesManager.UNSELECTED_ABILITY)
				Params.PutInt("ability", selectedAbility_);

			Params.PutSFSObject("purchasedPacks", User.GetBattleShopManager.TakePurchasedPacks());

			Connection.Send(new ExtensionRequest("game.Turn", Params, Connection.LastJoinedRoom));
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not logged in");
	}
	
	public static void StopGame()
	{
		Debug.Log(string.Format("SmartFoxConnection: Send game.StopGame cmd"));
		if (IsLoggedIn)
		{
			SFSObject Params = new SFSObject();
			Connection.Send(new ExtensionRequest("game.StopGame", Params, Connection.LastJoinedRoom));
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not logged in");
	}
	
	public static void SendLevelUpConfirmation(int itemId)
	{
		Debug.Log(string.Format("SmartFoxConnection: Send character.LevelUp cmd. itemId = {0}, value = {1}", itemId, 1));
		if (IsLoggedIn)
		{
			SFSObject Params = new SFSObject();
			Params.PutInt("itemId", itemId);
			Params.PutInt("value", 1);
			Connection.Send(new ExtensionRequest("character.LevelUp", Params));
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not logged in");
	}

	public static void SendPublicMessage(string message, string senderName)
	{
		Debug.Log(string.Format("SmartFoxConnection: Send public message - {0}", message));
		if (IsLoggedIn)
		{
			SFSObject Params = new SFSObject();
			Params.PutUtfString("senderName", senderName);
			Connection.Send(new PublicMessageRequest(message, Params));
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not logged in");
	}
	
	public static void SendFittedPerk(int perkId, bool isFitted)
	{
		Debug.Log (string.Format ("SmartFoxConnection: Send character.SetPerkFitted cmd. perkId = {0}, isFitted = {1}", perkId, isFitted));
		if (IsLoggedIn)
		{
			SFSObject Params = new SFSObject();
			Params.PutInt("perkId", perkId);
			Params.PutBool("isFitted", isFitted);
			Connection.Send(new ExtensionRequest("character.SetPerkFitted", Params));
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not logged in");
	}
	
	public static void UpdateAccountInformation(string email, string password)
	{
		Debug.Log (string.Format("SmartFoxConnection: Send character.BindMailAccount cmd"));
		if (IsLoggedIn)
		{
			SFSObject Params = new SFSObject();
			Params.PutUtfString("user", email);
			Params.PutUtfString("password", password);
			Connection.Send(new ExtensionRequest("character.BindMailAccount", Params));
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not logged in");
	}
	
	public static void UpdateFacebookInformation(string facebookId, string fbTitle)
	{
		Debug.Log (string.Format("SmartFoxConnection: Send character.BindFBAccount cmd"));
		if (IsLoggedIn)
		{
			SFSObject Params = new SFSObject();
			Params.PutUtfString("facebookId", facebookId);
			Params.PutUtfString("facebookTitle", fbTitle);
			if ( !string.IsNullOrEmpty(FB.AccessToken))
				Params.PutUtfString("AccessToken", FB.AccessToken);
			Connection.Send(new ExtensionRequest("character.BindFBAccount", Params));
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not logged in");
	}
	
	//Character commands
	public static void GetCharacterData()
	{
		Debug.Log (string.Format("SmartFoxConnection: Send character.getCharacterData cmd"));
		if (IsLoggedIn)
		{
			Connection.Send(new ExtensionRequest("character.getCharacterData", new SFSObject()));
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not logged in");
	}
	
	public static void GetPlayers()
	{
		Debug.Log(string.Format("SmartFoxConnection: Send game.GetPlayers cmd"));
		if (IsLoggedIn)
		{
			SFSObject Params = new SFSObject();
			Connection.Send(new ExtensionRequest("game.GetPlayers", Params,  Connection.LastJoinedRoom));
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not logged in");
	}
	
	public static void BuyCafe(int cafeId)
	{
		Debug.Log(string.Format("SmartFoxConnection: Send character.UpgradeBuilding cmd. id = {0}", cafeId));
		if (IsLoggedIn)
		{
			SFSObject Params = new SFSObject();
			Params.PutInt("id", cafeId);
			Connection.Send(new ExtensionRequest("character.UpgradeBuilding", Params));
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not logged in");
	}
	
	public static void EndGame(Dictionary<ECurrencyType,int> moneyProfit, int experienceProfit, int[] ingredientsProfit, bool gameWon, int opponentScore)
	{
		Debug.Log(string.Format("SmartFoxConnection: Send game.EndGame cmd"));
		if (IsLoggedIn)
		{
			SFSObject moneyProfitParams = new SFSObject();
			foreach(ECurrencyType currency in moneyProfit.Keys)
				moneyProfitParams.PutInt(currency.ToString("d"), moneyProfit[currency]);

			SFSObject Params = new SFSObject();
			Params.PutSFSObject("moneyProfit", moneyProfitParams);
			Params.PutInt("experienceProfit", experienceProfit);
			Params.PutBool("gameWon", gameWon);
			Params.PutInt("opponentScore", opponentScore);

			SFSObject Ingredients = new SFSObject();
			if (ingredientsProfit != null)
			{
				foreach (int ingrId in ingredientsProfit)
				{
					short count = Ingredients.ContainsKey(ingrId.ToString()) ? Ingredients.GetShort(ingrId.ToString()) :(short)0;
					Ingredients.PutShort(ingrId.ToString(), (short)(count+1));
				}
				Params.PutSFSObject("ingredientsProfit", Ingredients);
			}
			
			Params.PutSFSObject("purchasedPacks", User.GetBattleShopManager.TakePurchasedPacks());

			Connection.Send(new ExtensionRequest("game.EndGame", Params, Connection.LastJoinedRoom));
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not logged in");
	}
	
	public static void GrabGameProfit(Dictionary<ECurrencyType,int> moneyProfit, int experienceProfit, int[] ingredientsProfit, ECurrencyType priceCurrency, int priceVal)
	{
		Debug.Log(string.Format("SmartFoxConnection: Send character.GrabGameProfit cmd"));
		if (IsLoggedIn)
		{
			SFSObject moneyProfitParams = new SFSObject();
			foreach(ECurrencyType currency in moneyProfit.Keys)
				moneyProfitParams.PutInt(currency.ToString("d"), moneyProfit[currency]);

			SFSObject Params = new SFSObject();

			Params.PutSFSObject("moneyProfit", moneyProfitParams);
			Params.PutInt("experienceProfit", experienceProfit);
			
			SFSObject Ingredients = new SFSObject();
			if (ingredientsProfit != null)
			{
				foreach (int ingrId in ingredientsProfit)
				{
					string ingrIdstr = ingrId.ToString();
					short count = Ingredients.ContainsKey(ingrIdstr) ? Ingredients.GetShort(ingrIdstr) :(short)0;
					Ingredients.PutShort(ingrIdstr, (short)(count+1));
				}
				Params.PutSFSObject("ingredients", Ingredients);
			}

			SFSObject price = new SFSObject();
			price.PutInt(priceCurrency.ToString("d"), priceVal);
			Params.PutSFSObject("price", price);
			Connection.Send(new ExtensionRequest("character.GrabGameProfit", Params));
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not logged in");
	}
	
	public	static void GetBoard(List<int> explodedChests)
	{		
		Debug.Log(string.Format("SmartFoxConnection: Send game.GetBoard cmd"));
		if (IsLoggedIn)
		{
			SFSObject Params = new SFSObject();
			Params.PutIntArray("explodedChests", explodedChests.ToArray());
			Connection.Send(new ExtensionRequest("game.GetBoard", Params, Connection.LastJoinedRoom));
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not logged in");
	}

	public static void GetPrize(int prizeGeneratorId)
	{
		Debug.Log(string.Format("SmartFoxConnection: Send character.GetPrize cmd"));
		if (IsLoggedIn)
		{
			SFSObject Params = new SFSObject();
			Params.PutInt("id", prizeGeneratorId);
			Connection.Send(new ExtensionRequest("character.GetPrize", Params));
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not logged in");
	}

	// Commit extra bonus
	public static void CommitExtraBonus(int extraBonusID, Dictionary<int, int> ingredientsCount)
	{
		Debug.Log(string.Format("SmartFoxConnection: Send character.CommitExtraBonus cmd"));
		//
		if (IsLoggedIn)
		{
			// Init params object
			SFSObject Params = new SFSObject();

			// FILL PARAMS OBJECT
			// ID of extra bonus
			Params.PutInt("id", extraBonusID);
			// Count ingredients
			SFSObject IngredientsCountSFSObject = new SFSObject();
			foreach(KeyValuePair<int, int> ingredientCount in ingredientsCount) 
			{
				IngredientsCountSFSObject.PutInt(ingredientCount.Key.ToString(), ingredientCount.Value);
			}
			Params.PutSFSObject("ingredients", IngredientsCountSFSObject);

			// Send request
			Connection.Send(new ExtensionRequest("character.CommitExtraBonus", Params));

		// 
		} 
		else 
		{
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not logged in");
		}
	}



	//Tasks
	public static void StartTask(int taskId)
	{
		Debug.Log(string.Format("SmartFoxConnection: Send task.Start cmd. taskId = {0}", taskId));
		if (IsLoggedIn)
		{
			SFSObject Params = new SFSObject();
			Params.PutInt("id", taskId);
			Connection.Send(new ExtensionRequest("task.Start", Params));
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not logged in");
	}
	
	public static void SetTaskProgress(int taskId, int count)
	{
		Debug.Log(string.Format("SmartFoxConnection: Send task.Progress cmd. taskId = {0}, count = {1}", taskId, count));
		if (IsLoggedIn)
		{
			SFSObject Params = new SFSObject();
			Params.PutInt("id", taskId);
			Params.PutInt("count", count);
			Connection.Send(new ExtensionRequest("task.Progress", Params));
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not logged in");
	}

	public static void HarvestTask(int taskId)
	{
		Debug.Log(string.Format("SmartFoxConnection: Send task.Harvest cmd. taskId = {0}", taskId));
		if (IsLoggedIn)
		{
			SFSObject Params = new SFSObject();
			Params.PutInt("id", taskId);
			Connection.Send(new ExtensionRequest("task.Harvest", Params));
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not logged in");
	}

	//Cooking commands
	public static void CraftRecipe(int recipeId)
	{
		Debug.Log(string.Format("SmartFoxConnection: Send cooking.startCooking cmd. recipeId = {0}", recipeId));
		if (IsLoggedIn)
		{
			SFSObject Params = new SFSObject();
			Params.PutInt("recipeId", recipeId);
			Connection.Send(new ExtensionRequest("cooking.startCooking", Params));
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not logged in");
	}
	
	public static void FinishCooking(int recipeId)
	{
		Debug.Log(string.Format ("SmartFoxConnection: Send cooking.finishCooking cmd. recipeId = {0}", recipeId));
		if (IsLoggedIn)
		{
			SFSObject Params = new SFSObject();
			Params.PutInt("recipeId", recipeId);
			Connection.Send(new ExtensionRequest("cooking.finishCooking", Params));
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not logged in");
	}
	
	public static void BoostCooking(int recipeId)
	{
		Debug.Log(string.Format ("SmartFoxConnection: Send cooking.boostCooking cmd. recipeId = {0}", recipeId));
		if (IsLoggedIn)
		{
			SFSObject Params = new SFSObject();
			Params.PutInt("recipeId", recipeId);
			Connection.Send(new ExtensionRequest("cooking.boostCooking", Params));
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not logged in");
	}
		
	public static void BuyPack(EShopItemType type, int packId)
	{
		Debug.Log (string.Format ("SmartFoxConnection: Send shop.BuyPacks cmd, {0} type, packId = {1}", type, packId));
		if (IsLoggedIn)
		{
			SFSObject packDesc = new SFSObject();
			packDesc.PutInt(packId.ToString(), 1);

			SFSObject packs = new SFSObject();
			packs.PutSFSObject( ((int)type).ToString(), packDesc);

			SFSObject Params = new SFSObject();
			Params.PutSFSObject("packs", packs);
			Connection.Send(new ExtensionRequest("shop.BuyPacks", Params));
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not logged in");
	}

	public static void BuyItems(SFSObject sfs_data)
	{
		Debug.Log (string.Format ("SmartFoxConnection: Send shop.BuyItems cmd"));
		if (IsLoggedIn)
		{
			SFSObject Params = new SFSObject();
			Params.PutSFSObject("items", sfs_data);
			Connection.Send(new ExtensionRequest("shop.BuyItems", Params));
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not logged in");
	}

	//tutorial
	public static void SendCompleteTutorialStep(int stepId)
	{
		Debug.Log (string.Format ("SmartFoxConnection: Send tutorial.CompleteStep cmd"));
		if (IsLoggedIn)
		{
			SFSObject Params = new SFSObject();
			Params.PutInt("stepId", stepId);
			Connection.Send(new ExtensionRequest("tutorial.CompleteStep", Params));
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not logged in");
	}

	public static void SendCompleteTutoriaHint(int hintId)
	{
		Debug.Log (string.Format ("SmartFoxConnection: Send tutorial.CompleteHint cmd"));
		if (IsLoggedIn)
		{
			SFSObject Params = new SFSObject();
			Params.PutInt("hintId", hintId);
			Connection.Send(new ExtensionRequest("tutorial.CompleteHint", Params));
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not logged in");
	}

	//friends
	public static void CancelBuddyGame()
	{
		Debug.Log (string.Format ("SmartFoxConnection: Send buddy.CancelBuddyGame cmd"));
		if (IsLoggedIn)
		{
			SFSObject Params = new SFSObject();
			Connection.Send(new ExtensionRequest("buddy.CancelBuddyGame", Params));
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not logged in");
	}
	
	public static void SendInvite(string nameCharacter)
	{
		Debug.Log (string.Format ("SmartFoxConnection: Send buddy.SendInvite cmd. name = {0}", nameCharacter));
		if (IsLoggedIn)
		{
			SFSObject Params = new SFSObject();
			Params.PutUtfString("name", nameCharacter);
			Connection.Send(new ExtensionRequest("buddy.SendInvite", Params));
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not logged in");
	}
	
	public static void SendInviteToJoinInBattle(string nameCharacter)
	{
		Debug.Log (string.Format ("SmartFoxConnection: Send buddy.GameWithBuddy cmd. name = {0}", nameCharacter));
		if (IsLoggedIn)
		{
			SFSObject Params = new SFSObject();
			Params.PutUtfString("name", nameCharacter);
			Connection.Send(new ExtensionRequest("buddy.GameWithBuddy", Params));
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not logged in");
	}
	
	public static void AcceptInvite(string nameCharacter)
	{
		Debug.Log (string.Format ("SmartFoxConnection: Send buddy.AcceptInvite cmd. name = {0}", nameCharacter));
		if (IsLoggedIn)
		{
			SFSObject Params = new SFSObject();
			Params.PutUtfString("name", nameCharacter);
			Connection.Send(new ExtensionRequest("buddy.AcceptInvite", Params));
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not logged in");
	}
	
	public static void DeclineInvite(string nameCharacter)
	{
		Debug.Log (string.Format ("SmartFoxConnection: Send buddy.DeclineInvite cmd. name = {0}", nameCharacter));
		if (IsLoggedIn)
		{
			SFSObject Params = new SFSObject();
			Params.PutUtfString("name", nameCharacter);
			Connection.Send(new ExtensionRequest("buddy.DeclineInvite", Params));
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not logged in");
	}
	
	public static void RemoveBuddy(string nameCharacter)
	{
		Debug.Log (string.Format ("SmartFoxConnection: Send buddy.RemoveBuddy cmd. name = {0}", nameCharacter));
		if (IsLoggedIn)
		{
			SFSObject Params = new SFSObject();
			Params.PutUtfString("name", nameCharacter);
			Connection.Send(new ExtensionRequest("buddy.RemoveBuddy", Params));
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not logged in");
	}
	
	public static void AcceptGameInvite(SFSInvitation invi)
	{
		Debug.Log (string.Format ("SmartFoxConnection: Send InvitationReplyRequest ACCEPT"));
		if (IsInitialized)
		{
			Connection.Send(new InvitationReplyRequest(invi, InvitationReply.ACCEPT, new SFSObject()));
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not initialized. Call SmartFoxConnection.Initialize() first");
	}
	
	public static void DeclineGameInvite(SFSInvitation invi)
	{
		Debug.Log (string.Format ("SmartFoxConnection: Send InvitationReplyRequest REFUSE"));
		if (IsInitialized)
		{
			Connection.Send(new InvitationReplyRequest(invi, InvitationReply.REFUSE, new SFSObject()));
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not initialized. Call SmartFoxConnection.Initialize() first");
	}
	
	//in app
	public static void	InAppGetProducts()
	{
		Debug.Log (string.Format ("SmartFoxConnection: Send purchase.GetProducts cmd"));
		if (IsLoggedIn)
		{
			SFSObject ps = new SFSObject();			
			Connection.Send(new ExtensionRequest("purchase.GetProducts", ps));
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not logged in");
	}
	
	public static void	InAppCommit(string receipt)
	{
		Debug.Log (string.Format ("SmartFoxConnection: Send purchase.Commit cmd"));
		if (IsLoggedIn)
		{
			SFSObject ps = new SFSObject();
			ps.PutUtfString("receipt", receipt);
			
			Connection.Send(new ExtensionRequest("purchase.Commit", ps));
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not logged in");
	}
	
	// Character creation commands
	public static void CreateCharacter( string name, string prefabName, short head_id, short apron_id )
	{
		Debug.Log (string.Format ("SmartFoxConnection: Send createCharacter cmd. name = {0}, prefabName = {1}, head_id = {2}, apron_id = {3}", name, prefabName, head_id, apron_id));
		if (IsLoggedIn)
		{
			SFSObject characterData = new SFSObject();
			characterData.PutUtfString("name", name);
			characterData.PutUtfString("prefabName", prefabName);
			characterData.PutShort("head_id", head_id);
			characterData.PutShort("apron_id", apron_id);
						
			SFSObject params_ = new SFSObject();
			if ( !string.IsNullOrEmpty(FB.AccessToken))
				params_.PutUtfString("AccessToken", FB.AccessToken);
			params_.PutSFSObject("character", characterData);

			
			Connection.Send(new ExtensionRequest("createCharacter", params_));
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not logged in");
	}

	public static void Respec(Dictionary<int,int> gemLevelDictionary)
	{
		Debug.Log (string.Format("SmartFoxConnection: Send character.Respec cmd"));
		if (IsLoggedIn)
		{
			SFSObject Params = new SFSObject();
			foreach (int gemID in gemLevelDictionary.Keys)
			{
				Params.PutInt(gemID.ToString(), gemLevelDictionary[gemID]);
			}
			Connection.Send(new ExtensionRequest("character.Respec", Params));
		}
		else
			Debug.LogError("ERROR: SmartFoxConnection: SmartFox not logged in");
	}

	// EventListners management
	// ------------------------
	public static void RemoveEventListner(string eventType, EventListenerDelegate listener)
	{
		if (IsInitialized)
			Connection.RemoveEventListener(eventType, listener);
		else
			Debug.Log("SmartFoxConnection: SmartFox not initialized. Call SmartFoxConnection.Initialize() first");
	}
	
	public static void AddEventListener (string eventType, EventListenerDelegate listener)
	{
		if (IsInitialized)
			Connection.AddEventListener(eventType, listener);
		else
			Debug.Log("SmartFoxConnection: SmartFox not initialized. Call SmartFoxConnection.Initialize() first");
	}

	public static bool ValidateKey(ISFSObject sfs_object, string key, SFSDataType valueType)
	{
		return sfs_object.ContainsKey(key)? (SFSDataType)sfs_object.GetData(key).Type == valueType :false; 
	}

	
	// Instance members
	// ................
	private SmartFox smartFox;
	protected SmartFox SmartFox_
	{
		get {return smartFox;}
		set {smartFox = value;}
	}
	
	private int lagValue;
	protected int LagValue_
	{
		get {return lagValue;}
		set {lagValue = value;}
	}
	
	protected bool Initialize_(out string ErrorMessage)
	{
		ErrorMessage = null;
		if (!IsInitialized)
		{
			Debug.Log("SmartFoxConnection: Initializing smartFoxConnection object...");
			
			// In a webplayer (or editor in webplayer mode) we need to setup security policy negotiation with the server first
#if UNITY_WEBPLAYER 
			Debug.Log ("Prefetch Socket Policy...");
			if (!Security.PrefetchSocketPolicy(serverName, 8843, 200)) 
			{
               	ErrorMessage = "Security Exception. Policy file load failed!";
				return false;
			}
#endif
			Debug.Log("SmartFoxConnection: Create SmartFox object...");
			Connection = new SmartFox(debug);
		}
		
		if (IsInitialized != true && string.IsNullOrEmpty( ErrorMessage ) )
		{
			ErrorMessage = "Unknown Error. Can not create SmartFox object.";
			return false;
		}
		
		Debug.Log("SmartFoxConnection: SmartFoxConnection initialized");
		return true;
	}
		
	// Handle disconnection automagically
    void OnApplicationQuit()
	{ 
       	Disconnect();
    } 
	
	IEnumerator PingCoroutine()
	{
		while (IsLoggedIn)
		{
			SFSObject params_ = new SFSObject();
			Connection.Send(new ExtensionRequest("ping", params_));
			yield return new WaitForSeconds(kPingTime);
		}
	}
	void FixedUpdate()
	{
		if (IsInitialized)
		{
			smartFox.ProcessEvents();
		}
	}
	
	void OnSFSConnection(BaseEvent evt)
	{
		StopCoroutine("ConnectionIdleTimer");
	}
	
	void OnSFSLogin(BaseEvent evt)
	{
		Connection.EnableLagMonitor(true);
		StartCoroutine("PingCoroutine");
		
	}
	
	void OnSFSLogout(BaseEvent evt)
	{
		Connection.EnableLagMonitor(false);
		StopCoroutine("PingCoroutine");
	}
	
	void OnSFSPingPong(BaseEvent evt)
	{
		LagValue_ = Mathf.Abs( evt.Params.Contains("lagValue")? (int)evt.Params["lagValue"] :0 );
	}
	
	void OnApplicationPause(bool paused)
	{
		if (IsInitialized)
		{
			if (!paused && IsLoggedIn)
			{
				Connection.EnableLagMonitor(true);
				StartCoroutine("PingCoroutine");
			}
			else if (IsLoggedIn)
			{
				Connection.EnableLagMonitor(false);
				StopCoroutine("PingCoroutine");
			}
		}
	}
}

*/