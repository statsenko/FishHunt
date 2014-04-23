using UnityEngine;
using System.Collections;
using System.Reflection;

public class GameStrings
{
	protected static GameStrings mInstance = null;
	public static GameStrings Instance 
	{
		get 
		{
			if (Application.isEditor && !Application.isPlaying)
				return null;
			
            if (mInstance == null)
			{
				Debug.Log("GameStrings: Init gameStrings...");
				mInstance = new GameStrings();
				Localization.instance.currentLanguage = "Localization/English";
				
				CheckLocalization();
				Debug.Log("GameStrings: Init gameStrings completed");
            }
            return mInstance;
        }
	}

	public static string GetLocalizedString(string key)
	{
		if (Application.isPlaying)
			return Instance.GetLocalizedString_(key);
		else
			return string.Empty;
	}
	
	protected string GetLocalizedString_(string key)
	{
		string localizedString = Localization.instance.Get(key);
		if (localizedString.Equals(key))
			Debug.LogWarning("WARNING: GameStrings: Localized string not found: " + key);

		return localizedString;
	}
	
	public static void CheckLocalization()
	{
		FieldInfo[] fields = Instance.GetType().GetFields(BindingFlags.Static | BindingFlags.Public);
		foreach(FieldInfo field in fields)
		{
			if (field.FieldType == typeof(string))
			{
				string key = (string)field.GetValue(null);
				string localizedString = Localization.instance.Get(key);
				if (string.IsNullOrEmpty( localizedString ) || localizedString.Equals(key))
					Debug.LogWarning("WARNING: Localized string not found for key: \"" + key + "\" in Language: \"" + Localization.instance.currentLanguage + "\"");
			}
		}
	}


	/////////////////////////////////////// 
	/// 								///
	/// 	HISTORY COMMICS		 		///
	///									///
	///////////////////////////////////////
	
	public static string HistoryComics_Page01_SubTitle = "HistoryComics_Page01_SubTitle";
		

	/////////////////////////////////////// 
	/// 								///
	/// 	CHARACTER CREATION 			///
	///									///
	///////////////////////////////////////

	public static string CharacterCreation_DoneBtn = "CharacterCreation_DoneBtn";
	public static string CharacterCreation_Title = "CharacterCreation_Title";
	public static string CharacterCreation_DefaultInputText = "CharacterCreation_DefaultInputText";
	public static string CharacterCreation_RandomBtn = "CharacterCreation_RandomBtn";
	
	public static string CharacterCreation_ShortNameInputErrorAV_Message = "CharacterCreation_ShortNameInputErrorAV_Message";
	
	public static string CharacterCreation_EmptyInputErrorAV_Message = "CharacterCreation_EmptyInputErrorAV_Message";
	
	public static string CharacterCreation_UserBadNameErrorAV_Message = "CharacterCreation_UserBadNameErrorAV_Message";
	
	public static string CharacterCreation_CharacterAlreadyExistsErrorAV_Message = "CharacterCreation_CharacterAlreadyExistsErrorAV_Message";
	
	public static string CharacterCreation_CreationCommonErrorAV_Message = "CharacterCreation_CreationCommonErrorAV_Message";


	/////////////////////////////////////// 
	/// 								///
	/// 		LOGIN 					///
	///									///
	///////////////////////////////////////

	public static string Login_LaunchLoading_Title = "Login_LaunchLoading_Title";

	public static string Login_AutoLoginErrorAV_Message = "Login_AutoLoginErrorAV_Message";
	
	public static string Login_OldVersionErrorAV_Message = "Login_OldVersionErrorAV_Message";
	public static string Login_OldVersionErrorAV_ConfirmBtn = "Login_OldVersionErrorAV_ConfirmBtn";
	
	public static string Login_AccountLoginErrorAV_Message = "Login_AccountLoginErrorAV_Message";
	
	public static string Login_GuestLoginErrorAV_Message = "Login_GuestLoginErrorAV_Message";
	
	public static string Login_FBLoginErrorAV_Message = "Login_FBLoginErrorAV_Message";
	
	public static string Login_UserAlreadyLoggedErrorAV_Message = "Login_UserAlreadyLoggedErrorAV_Message";
	
	public static string Login_AccountNotExistsErrorAV_Message = "Login_AccountNotExistsErrorAV_Message";
	
	public static string Login_BadEmailAddressErrorAV_Message = "Login_BadEmailAddressErrorAV_Message";
	
	public static string Login_AuthErrorAV_Message = "Login_AuthErrorAV_Message";
	
	public static string Login_AccountCreationErrorAV_Message = "Login_AccountCreationErrorAV_Message";
	
	public static string Login_AccountLogin_Title = "Login_AccountLogin_Title";
	public static string Login_AccountLogin_Description = "Login_AccountLogin_Description";
	public static string Login_AccountLogin_ForgotPasswordButton = "Login_AccountLogin_ForgotPasswordButton";
	public static string Login_AccountLogin_DoneButton = "Login_AccountLogin_DoneButton";
	public static string Login_AccountLogin_DefaultEmailInputText = "Login_AccountLogin_DefaultEmailInputText";
	public static string Login_AccountLogin_DefaultPswdInputText = "Login_AccountLogin_DefaultPswdInputText";
	
	public static string Login_AccountLogin_EmptyInputErrorAV_Message = "Login_AccountLogin_EmptyInputErrorAV_Message";
	
	public static string Login_AccountLogin_ShortPswdErrorAV_Message = "Login_AccountLogin_ShortPswdErrorAV_Message";
	
	public static string Login_LoginOptions_Title = "Login_LoginOptions_Title";
	public static string Login_LoginOptions_Description = "Login_LoginOptions_Description";
	public static string Login_LoginOptions_CreateNewButton = "Login_LoginOptions_CreateNewButton";
	public static string Login_LoginOptions_FbButton = "Login_LoginOptions_FbButton";
	public static string Login_LoginOptions_LoginButton = "Login_LoginOptions_LoginButton";


	/////////////////////////////////////// 
	/// 								///
	/// 	MAIN LOBBY 					///
	///									///
	///////////////////////////////////////

	public static string Lobby_PlayButton = "Lobby_PlayButton";

	public static string Lobby_FullTime_Label = "Lobby_FullTime_Label";

	public static string Lobby_GameInviteNotificationAV_Title = "Lobby_GameInviteNotificationAV_Title";
	public static string Lobby_GameInviteNotificationAV_Message = "Lobby_GameInviteNotificationAV_Message";
	public static string Lobby_GameInviteNotificationAV_ConfirmationButton = "Lobby_GameInviteNotificationAV_ConfirmationButton";
	
	public static string Lobby_GameInviteAwaitingBuddyAV_Title = "Lobby_GameInviteAwaitingBuddyAV_Title";
	public static string Lobby_GameInviteAwaitingBuddyAV_Message = "Lobby_GameInviteAwaitingBuddyAV_Message";
	
	public static string Lobby_GameInviteSendErrorAV_Message = "Lobby_GameInviteSendErrorAV_Message";
	
	public static string Lobby_GameInviteGameCreationErrorAV_Message = "Lobby_GameInviteGameCreationErrorAV_Message";
	
	public static string Lobby_GameInviteReplyErrorAV_Message = "Lobby_GameInviteReplyErrorAV_Message";
	
	public static string Lobby_CharacterLoadingErrorAV_Message = "Lobby_CharacterLoadingErrorAV_Message";

	public static string Lobby_RateReviewAV_Title = "Lobby_RateReviewAV_Title";
	public static string Lobby_RateReviewAV_Message = "Lobby_RateReviewAV_Message";
	public static string Lobby_RateReviewAV_ConfirmBtn = "Lobby_RateReviewAV_ConfirmBtn";
	public static string Lobby_RateReviewAV_CancelBtn = "Lobby_RateReviewAV_CancelBtn";
	
	public static string Lobby_CreateAccountAV_Title = "Lobby_CreateAccountAV_Title";
	public static string Lobby_CreateAccountAV_Message = "Lobby_CreateAccountAV_Message";
	public static string Lobby_CreateAccountAV_ConfirmBtn = "Lobby_CreateAccountAV_ConfirmBtn";

	public static string Lobby_Friends_Title = "Lobby_Friends_Title";
	public static string Lobby_Friends_DefaultInputText = "Lobby_Friends_DefaultInputText";

	public static string Lobby_Friends_FriendAllreadyExistsInvitationErrorAV_Message = "Lobby_Friends_FriendAllreadyExistsInvitationErrorAV_Message";

	public static string Lobby_Friends_FriendRemoveErrorAV_Message = "Lobby_Friends_FriendRemoveErrorAV_Message";

	public static string Lobby_Friends_FriendInvitationSentNotificationAV_Title = "Lobby_Friends_FriendInvitationSentNotificationAV_Title";
	public static string Lobby_Friends_FriendInvitationSentNotificationAV_Message = "Lobby_Friends_FriendInvitationSentNotificationAV_Message";

	public static string Lobby_Friends_FriendInviteErrorAV_Message = "Lobby_Friends_FriendInviteErrorAV_Message";
	public static string Lobby_Friends_FriendNotFoundInviteErrorAV_Message = "Lobby_Friends_FriendNotFoundInviteErrorAV_Message";

	public static string Lobby_Friends_FriendInvitationDeclineErrorAV_Message = "Lobby_Friends_FriendInvitationDeclineErrorAV_Message";

	public static string Lobby_Friends_FriendInvitationAcceptErrorAV_CommonMessage = "Lobby_Friends_FriendInvitationAcceptErrorAV_CommonMessage";
	public static string Lobby_Friends_FriendInvitationAcceptErrorAV_MyListFullMessage = "Lobby_Friends_FriendInvitationAcceptErrorAV_MyListFullMessage";
	public static string Lobby_Friends_FriendInvitationAcceptErrorAV_TheirListFullMessage = "Lobby_Friends_FriendInvitationAcceptErrorAV_TheirListFullMessage";

	public static string Lobby_Friends_FriendRemoveConfirmationAV_Message = "Lobby_Friends_FriendRemoveConfirmationAV_Message";


	/////////////////////////////////////// 
	/// 								///
	/// 	GAME SELECTION 		 		///
	///									///
	///////////////////////////////////////

	public static string GameSelection_ConfirmProposedPerkPurchaseAV_Message = "GameSelection_ConfirmProposedPerkPurchaseAV_Message";
	public static string GameSelection_ConfirmConsumedPerkPurchaseAV_Message = "GameSelection_ConfirmConsumedPerkPurchaseAV_Message";
	
	public static string GameSelection_PerkPurchasedAV_Title = "GameSelection_PerkPurchasedAV_Title";
	public static string GameSelection_PerkPurchasedAV_Message = "GameSelection_PerkPurchasedAV_Message";
	public static string GameSelection_PerkPurchasedAV_ConfirmBtn = "GameSelection_PerkPurchasedAV_ConfirmBtn";
	
	public static string GameSelection_ShopPurchaseConfirmAV_Title = "GameSelection_ShopPurchaseConfirmAV_Title";

	/////////////////////////////////////// 
	/// 								///
	/// 	GAME LOADING 		 		///
	///									///
	/////////////////////////////////////// 
	
	public static string GameLoading_LookingForOpponent = "GameLoading_LookingForOpponent";
	public static string GameLoading_GetReady = "GameLoading_GetReady";
	public static string GameLoading_FriendsGame = "GameLoading_FriendsGame";
	
	public static string GameLoading_JoinRoomErrorAV_Message = "GameLoading_JoinRoomErrorAV_Message";
	
	public static string GameLoading_CannotLoadPlayerInfoErrorAV_Message = "GameLoading_CannotLoadPlayerInfoErrorAV_Message";
	
	public static string GameLoading_GetCafeErrorAV_Message = "GameLoading_GetCafeErrorAV_Message";


	/////////////////////////////////////// 
	/// 								///
	/// 		 GAME 		 			///
	///									///
	///////////////////////////////////////
	
	public static string Game_NoMovesAvailableMsg = "Game_NoMovesAvailableMsg";
	public static string Game_UserMoveMsg = "Game_UserMoveMsg";
	public static string Game_OpponentMoveMsg = "Game_OpponentMoveMsg";
	public static string Game_ExtraMoveMsg = "Game_ExtraMoveMsg";
	public static string Game_TimeOutMsg = "Game_TimeOutMsg";
	public static string Game_PowerUpUsedMsg_Blender = "Game_PowerUpUsedMsg_Blender";
	public static string Game_PowerUpUsedMsg_Tricky = "Game_PowerUpUsedMsg_Tricky";  
	public static string Game_AbilityUsedMsg_LineAbility = "Game_AbilityUsedMsg_LineAbility"; 
	public static string Game_AbilityUsedMsg_BombAbility = "Game_AbilityUsedMsg_BombAbility"; 
	public static string Game_AbilityUsedMsg_CrossAbility = "Game_AbilityUsedMsg_CrossAbility"; 
	public static string Game_AbilityUsedMsg_EqualsAbility = "Game_AbilityUsedMsg_EqualsAbility"; 
	public static string Game_AbilityUsedMsg_ExplosionAbility = "Game_AbilityUsedMsg_ExplosionAbility";

	public static string Game_WinAV_Title = "Game_WinAV_Title";
	public static string Game_LooseAV_Title = "Game_LooseAV_Title";

	public static string Game_TechVictoryAV_Title = "Game_TechVictoryAV_Title";
	public static string Game_TechVictoryAV_Message = "Game_TechVictoryAV_Message";

	public static string Game_TechLooseAV_Title = "Game_TechLooseAV_Title";
	public static string Game_TechLooseAV_Message = "Game_TechLooseAV_Message";

	public static string Game_StopGameErrorAV_Message = "Game_StopGameErrorAV_Message";

	public static string Game_CommitProfitErrorAV_Message = "Game_CommitProfitErrorAV_Message";

	public static string Game_Info_Next_Button = "Game_Info_Next_Button";
	public static string Game_Info_x4_PrizeTitle = "Game_Info_x4_PrizeTitle";
	public static string Game_Info_x5_PrizeTitle = "Game_Info_x5_PrizeTitle";
	public static string Game_Info_x6_PrizeTitle = "Game_Info_x6_PrizeTitle";
	public static string Game_Info_lt_PrizeTitle = "Game_Info_lt_PrizeTitle";
	

	/////////////////////////////////////// 
	/// 								///
	/// 	WIN LOOSE			 		///
	///									///
	/////////////////////////////////////// 
	
	public static string WinLoose_WinTitle = "WinLoose_WinTitle";
	public static string WinLoose_LooseTitle = "WinLoose_LooseTitle";
	public static string WinLoose_LooseDescriptionTitle = "WinLoose_LooseDescriptionTitle";

	public static string WinLoose_ProfitTitle = "WinLoose_ProfitTitle";
	public static string WinLoose_GrabBtnTitle = "WinLoose_GrabBtnTitle";
	public static string WinLoose_PrizeItemsTitle = "WinLoose_PrizeItemsTitle";
	public static string WinLoose_NAPrizeItemsDescriptionTitle = "WinLoose_NAPrizeItemsDescriptionTitle";
	public static string WinLoose_DefMoneyProfitTitle = "WinLoose_DefMoneyProfitTitle";
	public static string WinLoose_ExpProfitTitle = "WinLoose_ExpProfitTitle";
	public static string WinLoose_CafeMasteringTitle = "WinLoose_CafeMasteringTitle";
	public static string WinLoose_GrabItemsTitle = "WinLoose_GrabItemsTitle";

	public static string WinLoose_GrabProfitErrorAV_Message = "WinLoose_GrabProfitErrorAV_Message";

	public static string WinLoose_NewLevelAV_Title = "WinLoose_NewLevelAV_Title";
	public static string WinLoose_NewLevelAV_Message = "WinLoose_NewLevelAV_Message";

	public static string WinLooseFriends_WinTitle = "WinLooseFriends_WinTitle";
	public static string WinLooseFriends_LooseTitle = "WinLooseFriends_LooseTitle";


	/////////////////////////////////////// 
	/// 								///
	/// 	LEVEL UP			 		///
	///									///
	/////////////////////////////////////// 
	
	public static string LevelUp_Title = "LevelUp_Title";
	public static string LevelUp_SelectGemTitle = "LevelUp_SelectGemTitle";
	
	public static string LevelUp_LevelUpCommonErrorAV_Message = "LevelUp_LevelUpCommonErrorAV_Message";
	
	public static string LevelUp_GemNotSelectedErrorAV_Message = "LevelUp_GemNotSelectedErrorAV_Message";

	public static string LevelUp_DonBtn = "LevelUp_DonBtn";


	/////////////////////////////////////// 
	/// 								///
	/// 		SETTINGS 			 	///
	///									///
	///////////////////////////////////////

	public static string Settings_Title = "Settings_Title";
	public static string Settings_Description = "Settings_Description";

	public static string Settings_DeleteCharacterBtnTitle = "Settings_DeleteCharacterBtnTitle";
	public static string Settings_ConnectToFacebookBtnTitle = "Settings_ConnectToFacebookBtnTitle";
	public static string Settings_CreateAccountBtnTitle = "Settings_CreateAccountBtnTitle";
	public static string Settings_FaqBtnTitle = "Settings_FaqBtnTitle";
	public static string Settings_SupportBtnTitle = "Settings_SupportBtnTitle";
	public static string Settings_LogoutBtnTitle = "Settings_LogoutBtnTitle";
	
	public static string Settings_RemoveGuestErrorAV_Message = "Settings_RemoveGuestErrorAV_Message";

	public static string Settings_AccountCreateErrorAV_Message = "Settings_AccountCreateErrorAV_Message";

	public static string Settings_BadEmailAddressErrorAV_Message = "Settings_BadEmailAddressErrorAV_Message";

	public static string Settings_RemoveCharacterWarningAV_Message = "Settings_RemoveCharacterWarningAV_Message";

	public static string Settings_AccountCreate_Title = "Settings_AccountCreate_Title";
	public static string Settings_AccountCreate_Description = "Settings_AccountCreate_Description";
	public static string Settings_AccountCreate_DoneButton = "Settings_AccountCreate_DoneButton";
	public static string Settings_AccountCreate_DefaultEmailInputText = "Settings_AccountCreate_DefaultEmailInputText";
	public static string Settings_AccountCreate_DefaultPswdInputText = "Settings_AccountCreate_DefaultPswdInputText";


	/////////////////////////////////////// 
	/// 								///
	/// 	IN APP PURCHAISES 			///
	///									///
	///////////////////////////////////////
	
	public static string InAppPurchase_Title = "InAppPurchase_Title";

	public static string InAppPurchase_InitStoreKitErrorAV_Message = "InAppPurchase_InitStoreKitErrorAV_Message";

	public static string InAppPurchase_GetProductsListErrorAV_Message = "InAppPurchase_GetProductsListErrorAV_Message";

	public static string InAppPurchase_PurchaseErrorAV_Message = "InAppPurchase_PurchaseErrorAV_Message";

	public static string InAppPurchase_PurchaseCompletedNotificationAV_Title = "InAppPurchase_PurchaseCompletedNotificationAV_Title";
	public static string InAppPurchase_PurchaseCompletedNotificationAV_DefaultMessage = "InAppPurchase_PurchaseCompletedNotificationAV_DefaultMessage";
	public static string InAppPurchase_PurchaseCompletedNotificationAV_SpecifiedMessage = "InAppPurchase_PurchaseCompletedNotificationAV_SpecifiedMessage";


	/////////////////////////////////////// 
	/// 								///
	/// 		SHOP 					///
	///									///
	///////////////////////////////////////
	
	public static string Shop_WearShop_Title = "Shop_WearShop_Title";
	public static string Shop_WearShop_CommingSoonTitle = "Shop_WearShop_CommingSoonTitle";
	public static string Shop_WearShop_RespecButton = "Shop_WearShop_RespecButton";
	
	public static string Shop_Inventory_Section1_Title = "Shop_Inventory_Section1_Title";
	public static string Shop_Inventory_Section2_Title = "Shop_Inventory_Section2_Title";
	public static string Shop_Inventory_Section3_Title = "Shop_Inventory_Section3_Title";

	public static string Shop_NALevelForRespecAV_Message = "Shop_NALevelForRespecAV_Message";


	/////////////////////////////////////// 
	/// 								///
	/// 	RECIPES CRAFT 		 		///
	///									///
	///////////////////////////////////////

	public static string RecipesCraft_PremiumBoosts_Label = "RecipesCraft_PremiumBoosts_Label";
	public static string RecipesCraft_IngredientsExisted_Label = "RecipesCraft_IngredientsExisted_Label";

	public static string RecipesCraft_Cell_LockedTitle = "RecipesCraft_Cell_LockedTitle";
	public static string RecipesCraft_Cell_MasteredTitle = "RecipesCraft_Cell_MasteredTitle";
	public static string RecipesCraft_Cell_CompletedTitle = "RecipesCraft_Cell_CompletedTitle";
	public static string RecipesCraft_Cell_HarvestBaloonTitle = "RecipesCraft_Cell_HarvestBaloonTitle";
	
	public static string RecipesCraft_NACafeAV_Message = "RecipesCraft_NACafeAV_Message";

	public static string RecipesCraft_PayWallAV_Title = "RecipesCraft_PayWallAV_Title";
	public static string RecipesCraft_PayWallAV_Message = "RecipesCraft_PayWallAV_Message";
	public static string RecipesCraft_PayWallAV_ConfirmBtn = "RecipesCraft_PayWallAV_ConfirmBtn";
	

	/////////////////////////////////////// 
	/// 								///
	/// 		LEVEL RESPEC		 	///
	///									///
	///////////////////////////////////////

	public static string LevelRespec_Title = "LevelRespec_Title";
	public static string LevelRespec_Description = "LevelRespec_Description";

	public static string LevelRespec_ConfirmationAV_Title = "LevelRespec_ConfirmationAV_Title";
	public static string LevelRespec_ConfirmationAV_Message = "LevelRespec_ConfirmationAV_Message";
	
	public static string LevelRespec_RespecErrorAV_Message = "LevelRespec_RespecErrorAV_Message";

	public static string LevelRespec_AutoAssign_Button = "LevelRespec_AutoAssign_Button";
	
	
	
	
	/////////////////////////////////////// 
	/// 								///
	/// 		EXTRA BONUS			 	///
	///									///
	///////////////////////////////////////
	
	public static string ExtraBonus_Title = "ExtraBonus_Title";
	public static string ExtraBonus_Message = "ExtraBonus_Message";
	public static string ExtraBonus_Text_Count = "ExtraBonus_Text_Count";
	public static string ExtraBonus_Claim_Button = "ExtraBonus_Claim_Button";
	public static string ExtraBonus_SuccessAV_Title = "ExtraBonus_SuccessAV_Title";
	public static string ExtraBonus_SuccessAV_Message = "ExtraBonus_SuccessAV_Message";
	public static string ExtraBonus_ErrorAV_Title = "ExtraBonus_ErrorAV_Title";
	public static string ExtraBonus_ErrorAV_Message = "ExtraBonus_ErrorAV_Message";
	public static string ExtraBonus_ErrorCommitAV_Message = "ExtraBonus_ErrorCommitAV_Message";
	public static string ExtraBonus_NoExtraBonusErrorAV_Message = "ExtraBonus_NoExtraBonusErrorAV_Message";
	
	
	
	
	/////////////////////////////////////// 
	/// 								///
	/// 		CAFE MASTERING		 	///
	///									///
	///////////////////////////////////////
	
	public static string CafeMastering_Title = "CafeMastering_Title";
	public static string CafeMastering_Message = "CafeMastering_Message";
	public static string CafeMastering_GetPrize_Button = "CafeMastering_GetPrize_Button";
	public static string CafeMastering_NoCafeIdErrorAV_Message = "CafeMastering_NoCafeIdErrorAV_Message";

	/////////////////////////////////////// 
	/// 								///
	/// 	PRIZE GENERATION			///
	///									///
	///////////////////////////////////////
	
	public static string PrizeGeneration_SpinButton = "PrizeGeneration_SpinButton";
	public static string PrizeGeneration_RerollButton = "PrizeGeneration_RerollButton";
	
	public static string PrizeGeneration_NoPrizeGeneratorErrorAV_Message = "PrizeGeneration_NoPrizeGeneratorErrorAV_Message";


	/////////////////////////////////////// 
	/// 								///
	/// 		CAFE MAP				///
	///									///
	///////////////////////////////////////

	public static string CafeMap_CafePurchaseAV_Recipes_Title = "CafeMap_CafePurchaseAV_Recipes_Title";
	public static string CafeMap_CafePurchaseAV_Prizes_Title = "CafeMap_CafePurchaseAV_Prizes_Title";

	public static string CafeMap_CafeInfoAV_Recipes_Title = "CafeMap_CafeInfoAV_Recipes_Title";
	public static string CafeMap_CafeInfoAV_Prizes_Title = "CafeMap_CafeInfoAV_Prizes_Title";
	public static string CafeMap_CafeInfoAV_ConfirmBtn = "CafeMap_CafeInfoAV_ConfirmBtn";


	/////////////////////////////////////// 
	/// 								///
	/// 	LCOAL NOTIFICATIONS	 		///
	///									///
	///////////////////////////////////////
	
	public static string LocalNotifications_LaunchMe24Hours = "LocalNotifications_LaunchMe24Hours";
	public static string LocalNotifications_LaunchMe48Hours = "LocalNotifications_LaunchMe48Hours";
	public static string LocalNotifications_LaunchMe7Days = "LocalNotifications_LaunchMe7Days";
	public static string LocalNotifications_AllLifesRestored = "LocalNotifications_AllLifesRestored";

	public static string LocalNotifications_CommonText = "LocalNotifications_CommonText";


	/////////////////////////////////////// 
	/// 								///
	/// 			TASKS		 		///
	///									///
	///////////////////////////////////////
	
	public static string TaskUI_WellDoneTitle = "TaskUI_WellDoneTitle";
	public static string TaskUI_WellDoneDescription = "TaskUI_WellDoneDescription";
	public static string TaskUI_HarvestButtonTitle = "TaskUI_HarvestButtonTitle";
	public static string TaskUI_StartButtonTitle = "TaskUI_StartButtonTitle";
	public static string TaskUI_StartButtonExpiredTitle = "TaskUI_StartButtonExpiredTitle";
	public static string TaskUI_CraftActionTitle = "TaskUI_CraftActionTitle";
	public static string TaskUI_FbActionTitle = "TaskUI_FbActionTitle";
	public static string TaskUI_BossActionTitle = "TaskUI_BossActionTitle";


	/////////////////////////////////////// 
	/// 								///
	/// 		RECIPES		 			///
	///									///
	///////////////////////////////////////
	
	public static string Recipe_WithLifes_Title = "Recipe_WithLifes_Title";
	public static string Recipe_WithExp_Title = "Recipe_WithExp_Title";
	public static string Recipe_WithMoney_Title = "Recipe_WithMoney_Title";
	public static string Recipe_WithMoneyAndExp_Title = "Recipe_WithMoneyAndExp_Title";
	
	public static string Recipe_WithLifes_Description = "Recipe_WithLifes_Description";
	public static string Recipe_WithExp_Description = "Recipe_WithExp_Description";
	public static string Recipe_WithMoney_Description = "Recipe_WithMoney_Description";
	public static string Recipe_WithMoneyAndExp_Description = "Recipe_WithMoneyAndExp_Description";
	

	/////////////////////////////////////// 
	/// 								///
	///  	COMMON ALERTS				///
	///									///
	///////////////////////////////////////

	public static string ErrorAV_Title = "ErrorAV_Title";
	public static string WarningAV_Title = "WarningAV_Title";
	public static string OopsAV_Title = "OopsAV_Title";

	public static string ShopPurchaseConfirmAV_Title = "ShopPurchaseConfirmAV_Title";

	public static string NALifesAV_Title = "NALifesAV_Title";
	public static string NALifesAV_Message = "NALifesAV_Message";
	public static string NALifesAV_ConfirmBtn = "NALifesAV_ConfirmBtn";

	public static string NAMoneyAV_Title = "NAMoneyAV_Title";
	public static string NAMoneyAV_Message = "NAMoneyAV_Message";

	public static string NAIngredientsAV_Title = "NAIngredientsAV_Title";
	public static string NAIngredientsAV_WinItBtn = "NAIngredientsAV_WinItBtn";


	/////////////////////////////////////// 
	/// 								///
	///  	COMMON STRINGS				///
	///									///
	///////////////////////////////////////

	public static string Common_NA_Money = "Common_NA_Money";
	public static string Common_NA_Level = "Common_NA_Level";
	public static string Common_NA_MoneyAndLevel = "Common_NA_MoneyAndLevel";
	public static string Common_NA_Ingredients = "Common_NA_Ingredients";
	public static string Common_Buy = "Common_Buy";
	public static string Common_AddMoney = "Common_AddMoney";
	public static string Common_Close = "Common_Close";
	public static string Common_Cancel = "Common_Cancel";
	public static string Common_Continue = "Common_Continue";
	public static string Common_Ok = "Common_Ok";
	public static string Common_Yes = "Common_Yes";
	public static string Common_No = "Common_No";
	public static string Common_NotNow = "Common_NotNow";
	public static string Common_Later = "Common_Later";
	

	/////////////////////////////////////// 
	/// 								///
	/// 	FACEBOOK 					///
	///									///
	///////////////////////////////////////
	
	public static string FB_ConnectErrorAV_Message = "FB_ConnectErrorAV_Message";
	public static string FB_ConnectErrorAV_AlreadyBind_Message = "FB_ConnectErrorAV_AlreadyBind_Message";
	
	public static string FB_ShareScreenshotAV_Title = "FB_ShareScreenshotAV_Title";
	public static string FB_ShareScreenshotAV_Message = "FB_ShareScreenshotAV_Message";
	public static string FB_ShareScreenshot_message = "FB_ShareScreenshot_message";
	
	public static string FB_ShareProgressAV_Title = "FB_ShareProgressAV_Title";
	public static string FB_ShareProgressAV_Message = "FB_ShareProgressAV_Message";
	public static string FB_ShareLevelUp_message = "FB_ShareLevelUp_message";
	
	
	/////////////////////////////////////// 
	/// 								///
	/// 	SMARTFOX CONNECTION 		///
	///									///
	///////////////////////////////////////
	
	public static string SFS_ConnectionCreateErrorAV_Message = "SFS_ConnectionCreateErrorAV_Message";
	
	public static string SFS_InitErrorAV_Message = "SFS_InitErrorAV_Message";
	
	public static string SFS_BadResponseErrorAV_Message = "SFS_BadResponseErrorAV_Message";
	
	public static string SFS_ConnectionLostErrorAV_Message = "SFS_ConnectionLostErrorAV_Message";
	
	public static string SFS_CommonErrorAV_Message = "SFS_CommonErrorAV_Message";
	
	public static string SFS_ConnectionResponseTimeoutAV_Message = "SFS_ConnectionResponseTimeoutAV_Message";
}
