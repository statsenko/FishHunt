using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public static class GameConstants
{
	public static float defaultSfxVolume = 0.75f;
	public static float defaultMusicVolume = 0.75f;

	public static int secondsPerHour = 3600;
	public static int secondsPerHalfHour = 1800;
	public static int secondsPerMinute = 60;
	
	public static int PasswordMinLength = 4;
	public static int NameMinLength = 3;
	
	public static int TimeToShowGameTurnHint = 4;
	public static int TimeToShowGameTurnHintInTutorial = 1;
	public static int TimeToHideGameTurnHint = 1;
	
	public static float ConnectionMaxIdleTime = 15f;
	
	public static string CharactersPrefabsPath = "Prefabs/Characters/";
	public static string CafePrefabsPath = "Prefabs/Cafe/";
	public static string RecipesCraftIngredientIconPrefabsPath = "Prefabs/UI/RecipesCraftIngredientIcon";
	public static string IngredientCellPrefabPath = "Prefabs/UI/IngredientCell";

	public static string CafeBackgroundPathComponent = "/background";
	public static string CafeMapItemPathComponent = "/mapImage";
	public static string CafeIconPathComponent = "/icon";
	public static string CafeBigImagePathComponent = "/bigImage";
	public static string PerksIconsPath = "PerkIcons/!images/";
	public static string CafesIconsPath = "CafeIcons/!images/";
	public static string RecipesIconsPath = "RecipeIcons/!images/";
	public static string AbilitiesIconsPath = "AbilityIcons/!images/";
	public static string IngredientsIconsPath = "IngredientIcons/!images/";
	public static string TasksIconsPath = "TaskIcons/!images/";
	public static string CommonImagesPath = "CommonImages/";
  	public static string GemsIconsPath = "GemIcons/";
	public static string ShopPacksIconsPath = "ShopPacksIcons/";
	public static string GameBoardBackgroundsPath = "GameBoardBackgrounds/";
	public static string SmileIconsPath = "Smiles/";

	public static string DefaultGameBoardBackground = "GameBoardBackgrounds/default";
	// CommonImagesNames
	public static string count_bg_1_imgName = "count_bg";
	public static string count_bg_2_imgName = "count_bg_perks";
	public static string count_bg_orange_stroke = "count_bg_orange_stroke";
	public static string count_bg_white = "count_bg_white";

	public static string exp_icon_1_imgName = "level_icon_big";
	public static string exp_icon_2_imgName = "level_icon_small";
	public static string exp_icon_3_imgName = "level_icon_big_red";

	public static string lifes_icon_1_imgName = "lifes_icon_big";

	public static string defMoney_icon_1_imgName = "money_icon_big";
	public static string defMoney_icon_2_imgName = "money_icon_small";
	public static string defMoney_icon_3_imgName = "money_icon_big_red";

	public static string premMoney_icon_1_imgName = "premium_icon_big";
	public static string premMoney_icon_2_imgName = "premium_icon_small";
	public static string premMoney_icon_3_imgName = "premium_icon_big_red";

	public static string boost_icon_imgName = "boost_icon";
	public static string ready_icon_imgName = "recipe_ready_icon";
	public static string time_icon_imgName = "time_icon";
	public static string task_icon_bg_imgName = "task_icon_bg";
	public static string badge_icon_imgName = "badge";
	public static string check_mark_imgName = "check_mark";
	public static string plus_mark_imgName = "plus";

	public static string BattleWin_imgName = "battle_win_av_img";
	public static string BattleLoose_imgName = "battle_loose_av_img";
	public static string BattleLooseFriends_imgName = "battle_loose_friends_av_img";

	public static string SmileBaloon_imgName = "smile_baloon";

	public static string tile_background = "tile_background";
	public static string tile_background_fade = "tile_background_fade";

	public static string tile_border_concave_concave = GameConstants.CommonImagesPath + "tile_border_concave_concave";
	public static string tile_border_convex_concave = GameConstants.CommonImagesPath + "tile_border_convex_concave";
	public static string tile_border_convex_convex = GameConstants.CommonImagesPath + "tile_border_convex_convex";
	public static string tile_border_convex_rect = GameConstants.CommonImagesPath + "tile_border_convex_rect";
	public static string tile_border_rect_concave = GameConstants.CommonImagesPath + "tile_border_rect_concave";
	public static string tile_border_rect_rect = GameConstants.CommonImagesPath + "tile_border_rect_rect";
	
	public static string cell_single_2 = GameConstants.CommonImagesPath + "cell_single_2";
	public static string cell_single_5 = GameConstants.CommonImagesPath + "cell_single_5";
	public static string cell_single_6 = GameConstants.CommonImagesPath + "cell_single_6";
	public static string Sprite_cell_single_7 =  "cell_single_7";
	public static string Sprite_cell_single_7_checked = "cell_single_7_checked";
	public static string ingredients_back_texture_light = GameConstants.CommonImagesPath + "ingredients_back_texture_light";

	public static string ingredients_back_texture = "ingredients_back_texture";
	public static string ingredients_back_label = "ingredients_back_label";

	public static string lottery_background = GameConstants.CommonImagesPath + "lottery_background";
	public static string ingredientCellLocked_background = GameConstants.CommonImagesPath + "ingredientCellLocked_background";
	public static string ingredientCellUnlocked_background = GameConstants.CommonImagesPath + "ingredientCellUnlocked_background";
	public static string level_icon_4 = GameConstants.CommonImagesPath + "level_icon_4";
	public static string icon_win = GameConstants.CommonImagesPath + "icon_win";
	public static string icon_extrabonus_ingredient_select = GameConstants.CommonImagesPath + "Add_button_press";

	public static string cafe_plate_imgName = "cafe_purchase_plate";
	public static string cafe_progressBar_bg_imgName = "cafe_progressBar_bg";
	public static string cafe_progressBar_fg_imgName = "cafe_progressBar_fg";
	public static string cafe_progressIcon_imgName = "cafe_progressIcon";
	public static string cafe_playBtn_imgName = "cafe_btn_play";
	public static string cafe_infoBtn_imgName = "cafe_btn_info";


	public static string AppStoreAppId = "633879175";
	static string iOSUpgradeBuildURLFormat	= "https://itunes.apple.com/app/idAPP_ID";
	public static string UpgradeBuildURL
	{
		get
		{
			string urlFormat = iOSUpgradeBuildURLFormat;
			string url = urlFormat.Replace("APP_ID", AppStoreAppId);
			return url;
		}
	}
	static string iOS7RateURLFormat 	= "itms-apps://itunes.apple.com/app/idAPP_ID";
	static string iOS71RateURLFormat 	= "itms-apps://itunes.apple.com/WebObjects/MZStore.woa/wa/viewContentsUserReviews?type=Purple+Software&id=APP_ID";
	static string iOSRateURLFormat 		= "itms-apps://ax.itunes.apple.com/WebObjects/MZStore.woa/wa/viewContentsUserReviews?type=Purple+Software&id=APP_ID";
	public static string RateUrl
	{
		get 
		{
			string urlFormat = (OSInfo.MajorOsVersion >= 7) ?((OSInfo.MinorOsVersion >= 1) ?iOS71RateURLFormat :iOS7RateURLFormat) :iOSRateURLFormat;
			string url = urlFormat.Replace("APP_ID", AppStoreAppId);
			return url;
		}
	}
	public static string HelpURL 			= "http://ntgamesltd.com/luckychef/startpage/en/";
	public static string SupportURL 		= "http://ntgamesltd.com/luckychef/support/";
	public static string ResetPswdURL		= "http://ntgamesltd.com/luckychef/password/";
	
	public static int RateNotificateLaunchNumber = 5;
	public static int AccountNotificateLaunchNumber = 7;

	public static string FlurryEvent_BuyPrizeGeneratorFromShop = "Buy PG From Shop";
	public static string FlurryEvent_BuyPrizeGeneratorFromCraft = "Buy PG From Craft";
	public static string FlurryEvent_Reroll = "Reroll";
	public static string FlurryEvent_CraftRecipe = "Craft Recipe";
	public static string FlurryEvent_CraftPaywall = "Craft Paywall";
	public static string FlurryEvent_TaskDone = "Task Done";
	public static string FlurryEvent_BuyCafe = "Buy Cafe";
	public static string FlurryEvent_GameWithFriend = "Game With Frined";
	public static string FlurryEvent_GameInCafe = "Game In Cafe";
	public static string FlurryEvent_MasteringCafe = "Mastering cafe";
	public static string FlurryEvent_UseAbility = "Use Ability";
	public static string FlurryEvent_ScreenshotSend = "Screenshot";
	public static string FlurryEvent_LevelUp = "Level Up";
	public static string FlurryEvent_TutorialDone = "Tutorial Done";
	public static string FlurryEvent_UseGrabButton = "Grab Button";
	public static string FlurryEvent_BuyInApp = "Buy In App";	
	public static string FlurryEvent_BuyPack = "Buy Pack";
	public static string FlurryEvent_BuyPowerBeforeGame = "Buy PowerUp Before Game";
	public static string FlurryEvent_BuyPackWithAbility = "Buy Pack With Ability";
	public static string FlurryEvent_BuyLifesBeforeGame = "Buy Lifes Before Game";
	public static string FlurryEvent_Respec = "Respec";
	public static string FlurryEvent_TechLoose = "Tech Loose";
	public static string FlurryEvent_TechVictory = "Tech Victory";
	public static string FlurryEvent_StopGame = "Stop Game";
	public static string FlurryEvent_ConnectionLost = "Connection Lost";
	public static string FlurryEvent_BadResponse = "Bad Response";
	public static string FlurryEvent_ResponseTimeout = "Response Timeout";

	public static string FlurryEvent_WinBoss = "Win Boss";
	public static string FlurryEvent_WinAngryBoss = "Win AngryBoss";
	public static string FlurryEvent_WinBroccoliBoss = "Win BroccoliBoss";
	public static string FlurryEvent_WinCheesyBoss = "Win CheesyBoss";
	public static string FlurryEvent_WinMushroomBoss = "Win MushroomBoss";
	public static string FlurryEvent_WinBossOfBosses = "Win BossOfBosses";

	public static string FlurryKey = "5TDB2FGTZFG7ZPMQFGGS";
	

	public static string FBShare_levelUp_imgPath = "http://ntgamesltd.com/luckychef/levelup/";

	public static int[] FBShare_levels = {5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100, 110, 120};

	public static string iSmartNewsUrlString = "http://ntgamesltd.com/luckychef/news/SmartNews.plist";

	public static string FBAchievment_Start			= "1480715088808445"; 

	public static string FBAchievment_Boss_1_Defeat = "812557425438905";
	public static string FBAchievment_Boss_2_Defeat = "274502256050334";
	public static string FBAchievment_Boss_3_Defeat = "305231756296938";
	public static string FBAchievment_Boss_4_Defeat = "1419596348300403";
	public static string FBAchievment_Boss_5_Defeat = "279436738898607";
	public static string FBAchievment_Boss_6_Defeat = "1411662682433429";

	public static string FBAchievment_Cafe_2_Buy	= "224170157779746";
	public static string FBAchievment_Cafe_3_Buy	= "273980216096320";
	public static string FBAchievment_Cafe_4_Buy	= "457789267700713";
	public static string FBAchievment_Cafe_5_Buy	= "616663831754230";
	public static string FBAchievment_Cafe_6_Buy	= "510845922357394";
	public static string FBAchievment_Cafe_7_Buy	= "1487797548098434";
	public static string FBAchievment_Cafe_8_Buy	= "1440132699565608";

	public static string FBAchievment_LevelUp_5		= "587093101386079";
	public static string FBAchievment_LevelUp_10	= "259323767572287";
	public static string FBAchievment_LevelUp_15	= "1379469872316053";
	public static string FBAchievment_LevelUp_20	= "1585423948350352";
	public static string FBAchievment_LevelUp_25	= "279371885555974";
	public static string FBAchievment_LevelUp_30	= "1406507299621710";
	public static string FBAchievment_LevelUp_35	= "369756079828835";
	public static string FBAchievment_LevelUp_40	= "1400640540212759";
	public static string FBAchievment_LevelUp_45	= "239651186220786";
	public static string FBAchievment_LevelUp_50	= "373959219412921";
	public static string FBAchievment_LevelUp_55	= "1440861616153461";
	public static string FBAchievment_LevelUp_60	= "634807479922035";

	public static string FBAchievment_LevelUp_65	= "294530067371093";
	public static string FBAchievment_LevelUp_70	= "606903349386840";
	public static string FBAchievment_LevelUp_75	= "306550036162842";

	public static string FBAchievment_LevelUp_80	= "731902883521226";
	public static string FBAchievment_LevelUp_85	= "239124786211370";
	public static string FBAchievment_LevelUp_90	= "228106660722494";

	public static string FBAchievment_LevelUp_95	= "232259206982123";
	public static string FBAchievment_LevelUp_100	= "643990125655308";
	public static string FBAchievment_LevelUp_110	= "782212675124691";

	public static string FBAchievment_LevelUp_120	= "486291224834070";
 
	// Shop windows
	public static string ShopScene_IngredientTab = "ShopScene_IngredientTab";

	public static int minTimeToSendAISmile = 2;
	public static int maxTimeToSendAISmile = 4;

	//scenes
	public static string WinLooseScene = "WinLoose";
	public static string WinLooseWithFriends = "WinLooseFriends";
}
