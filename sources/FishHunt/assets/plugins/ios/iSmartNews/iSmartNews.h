/*!
 @file       iSmartNews.h
 @author     Sergey Ladeiko (Belprog Company)
 @date       12/12/11
 @version    1.6
 @verbatim
 New in version 1.2:
 @li Added support for application run counter.
 
 New in version 1.3:
 @li Added support for 'gate' flag.
 
 New in version 1.4 (21 Sep 2013):
 @li Added support for 'queue' flag.
 
 New in version 1.5 (20 Sep 2013):
 @li Now is ARC compatible
 @li Added support iSmartReview
 
 New in version 1.6 (13 Feb 2014):
 @li clearAndHideAlert added
 
 New in version 1.7 (25 Feb 2014) - (28 Feb 2014):
 @li Added support for web html content.
 @li Added support for device resoluton keys.
 
 @endverbatim
 */

/*!
 @mainpage iSmartNews library
 
 @section History History
 
 New in version 1.2:
 - Added support for application run counter.
 
 New in version 1.3:
 - Added support for 'gate' flag.
 
 New in version 1.4 (21 Sep 2013):
 - Added support for 'queue' flag.
 
 New in version 1.5 (20 Sep 2013):
 - Now is ARC compatible
 - Added support iSmartReview
 
 New in version 1.6 (13 Feb 2014):
 - clearAndHideAlert added
 
 New in version 1.7 (28 Feb 2014):
 - Added support for web html content.
 - Added support for device resoluton keys.
 
 @section Purpose Purpose
 Library contains code that helps to integrate
 support of downloading of some news into iOS applications.
 News are downloaded from specified addresses and
 then presented to user. Message can be presented
 only once or many times. Message can contain title,
 text, link to open, and one or two buttons.
 
 @section   PLISTFileSepcialStructure Structure of settings section in PLIST news file
 
 Special section of file is ordinary dictionary, which does not conatin 'title' & 'text' keys.
 Keys accepted inside special section:
 
 @li gate   - AS INTEGER. [OPTIONAL].
 Sets number of news shown per one load.
 For example, 2 means that only two news will be show at current time,
 next ones will be shown later (when 'upload' is called).
 
 @li queues   - AS ARRAY of DICTIONARIES. [OPTIONAL].
 Value is supported since version 1.4.
 It is used when some messages were grouped with 'queue' key (see below).
 In other case it is ignored.
 Each dictionary contains keys: 'name' - name of queue, 'timeout' - time distance between queues (in seconds).
 For example: we have to queues 'A' and 'B'. 'B' has timeout = 40, i.e. that messages from 'B' will be show
 after 40 seconds when last alert from 'A' is dismissed.
 
 
 @section   PLISTFileStructure Structure of PLIST news file
 
 File should be an array of dictionaries.
 Each dictionary can contain keys and values:
 
 @li start   - AS DATE. [OPTIONAL].
 Start date.
 
 @li end     - AS DATE. [OPTIONAL].
 End date.
 
 @li counter - AS NUMBER. [OPTIONAL]. Value is supported since version 1.2.
 If presents, then ts is compared with number of 'update' calls.
 If value is not 0 and is less then number of 'update' calls, then
 message will not be shown.
 
 @li title   - AS TEXT. [OPTIONAL].
 Title of message. At least text or title should present.
 @note Since version 1.7: if type of message is 'web', then this key is ignored.
 
 @li text    - AS TEXT. [OPTIONAL].
 Message itself. At least text or title should present.
 
 @li link    - AS TEXT. [OPTIONAL].
 Link to be opened if not cancel button was pressed.
 @note Since version 1.7: if type of message is 'web', then this key is ignored.
 
 @li cancel  - AS TEXT. [OPTIONAL].
 Title to display on 'Cancel' button.
 If not set, then localized version of 'Cancel' will be peeked
 from localization file.
 @note Since version 1.7: if type of message is 'web', then this key is ignored.
 
 @li ok      - AS TEXT. [OPTIONAL].
 Title to display on 'Ok' button.
 If not set, then localized version of 'Ok' will be peeked
 from localization file.
 @note Since version 1.7: if type of message is 'web', then this key is ignored.
 
 @li repeat  - AS BOOLEAN or NUMBER. [OPTIONAL].
 If YES, then message will be shown every time until 'Ok' is pressed.
 If NO, then message will be shown only once even if user presses 'Cancel'.
 If value not set, then NO is treated as default.
 
 @li always  - AS BOOLEAN or NUMBER. [OPTIONAL].
 If YES, then message will be shown every time even if user pressed 'Ok' or 'Cancel'.
 
 @li queue  - some string identifier, which groups messages and makes possible to rotate messages independently. [OPTIONAL].
 Value is supported since version 1.4.
 Identifiers are sorted alphabetically ignoring case. It means message with queue 'A' will be shown before message with
 queue 'B' even if message with 'B' is located before 'A' in plist file.
 If some message does not have 'queue' key or it is empty, then it is assigned to default queue (this queue will be shown first).
 
 @li type - sets content type to special value, for example, 'web'.
 NOTE: Type 'web' is supported in iOS 6 or higher only!!!
 This option is supported since version 1.7.
 If type is set to 'web', then module will show special popup window instead of alert view.
 Popup window will have 'Cancel' button. HTML content is loaded from URL specified in 'text' key.
 All clickable links in HTML content will be treated as OK click, and as result logic of OK-button
 will be applied to click. For web content 'title' key is ignored. Click to CANCEL will close popup.
 Click to link (if it exists) will also close popup window. Link will be opened in external program.
 
 @section MessageQueues Message queues (supported since version 1.4)
 Message queue - special feature allowing to group messages for displaying.
 Messages are sorted according to 'queue' attribute.
 
 For example, we have 4 messages:
 @li - message 1 assinged to default queue (queue is not defined to it)
 @li - message 2 assinged to 'A' queue
 @li - message 3 assinged to 'A' queue
 @li - message 4 assinged to 'B' queue
 
 'A' queue timeout is set to 30 (seconds). As result message 1 will be shown first.
 Message 2 will be shown after 30 seconds timeout, at last message 4 will be shown.
 Next update will show messages in this sequence: message 1 -> message 3 -> message 4.
 Next update will show messages in this sequence: message 1 -> message 2 -> message 4,
 etc.
 
 @section Notes Notes
 Do not forget that plist file with news has XML structure, therefore be accurate
 while using such characters like '&' in text. Each such entity should be replaced
 with proper equivalent: '&' -> '&amp;', etc.
 
 @section PLIST_LocalizationSupport Support of localization in PLIST.
 
 For support of localization you can create the same keys,
 but with '_lang[_country]' suffix.
 @li For example, to show some message in russian just for Russia you should create key 'text_ru_RU'.
 @li For example, to show some message in russian you should create key 'text_ru'.
 If localized key was not found, then default one will be used (e.g. 'text').
 The same rule is applied to all other keys: start, end, etc.
 Firstly, module searches for '_lang_country', then '_lang' and
 only then default key is used: text_es_ES -> text_es -> text.
 
 @section PLIST_DeviceResolutionSupport Support of devices with different resolutions in PLIST.
 
 Any key has support for device resolution association.
 For example, if you want to use 'text' key under non-retina iPad, then
 just use 'text' key as 'text_768x1024'.
 Also you can use combination of localization suffixes with device resolutions.
 
 
 Example of 'text' keys in order of precedence:
 
 text_PT_pt_768x1024
 text_PT_pt
 text_PT_768x1024
 text_PT
 text_768x1024
 text
 
 @section PLIST_Example Example of news PLIST file.
 
 @verbatim
 
 <?xml version="1.0" encoding="UTF-8"?>
 <!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
 <plist version="1.0">
 <array>
 <dict>                              <--! special section -->
 <key>gate</key>
 <integer>2</integer>
 <key>queues</key>
 <array>
 <dict>
 <key>name</key>         <--! new since version 1.4 -->
 <string>A</string>      <--! new since version 1.4 -->
 <key>timeout</key>      <--! new since version 1.4 -->
 <integer>30</integer>   <--! new since version 1.4 -->
 </dict>
 <dict>
 <key>name</key>         <--! new since version 1.4 -->
 <string>B</string>      <--! new since version 1.4 -->
 <key>timeout</key>      <--! new since version 1.4 -->
 <integer>30</integer>   <--! new since version 1.4 -->
 </dict>
 </array>
 </dict>
 <dict>
 <key>queue</key>                <--! new since version 1.4 -->
 <string>A</string>              <--! new since version 1.4 -->
 <key>cancel</key>
 <string>cancel</string>
 <key>start</key>
 <date>2011-10-03T19:30:42Z</date>
 <key>end</key>
 <date>2011-10-05T19:30:44Z</date>
 <key>link</key>
 <string>http://google.com</string>
 <key>link_PT_pt_768x1024</key>          <--! EXAMPLE of DEVICE RESOLUTION and LOCALIZATION KEY. New since version 1.7 -->
 <string>http://google.com</string>
 <key>ok</key>
 <string>open url</string>
 <key>text</key>
 <string>Some very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very long message!</string>
 <key>text_ru</key>
 <string>Простое и длинное длинное длинное длинное длинное длинное длинное длинное длинное длинное длинное длинное длинное длинное длинное длинное длинное длинное длинное сообщение!</string>
 <key>title</key>
 <string>title</string>
 <key>repeat</key>
 <true/>
 <key>counter</key>
 <integer>12</integer>
 <key>always</key>
 <true/>
 </dict>
 <dict>
 <key>queue</key>                    <--! new since version 1.4 -->
 <string>A</string>                  <--! new since version 1.4 -->
 <key>type</key>                     <--! new since version 1.7 -->
 <string>web</string>                <--! new since version 1.7 -->
 <key>start</key>
 <date>2011-10-03T19:30:42Z</date>
 <key>end</key>
 <date>2011-10-05T19:30:44Z</date>
 <key>text</key>
 <string>http://google.com</string>
 <key>text_ru</key>
 <string>http://google.com.ru</string>
 <key>repeat</key>
 <true/>
 <key>counter</key>
 <integer>12</integer>
 <key>always</key>
 <true/>
 </dict>
 </array>
 </plist>
 
 @endverbatim
 
 @section Usage Typical usage
 
 If you do not want to process calls manually and do not show alert view, then
 just place appropriate macros inside UIApplicationDelegate object methods.
 
 @code
 
 #import "iSmartNews.h"
 
 - (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions
 {
 ...
 iSmartNews_ApplicationDidFinishLaunchingAction([NSArray arrayWithObject:@"http://www.news.com/news.plist"]);
 ...
 }
 
 - (void)applicationDidEnterBackground:(UIApplication *)application
 {
 ...
 iSmartNews_ApplicationDidEnterBackgroundAction();
 ...
 }
 
 - (void)applicationWillEnterForeground:(UIApplication *)application
 {
 ...
 iSmartNews_ApplicationWillEnterForegroundAction();
 ...
 }
 
 - (void)applicationWillTerminate:(UIApplication *)application
 {
 ...
 iSmartNews_ApplicationWillTerminateAction();
 ...
 }
 
 @endcode
 */

#import <Foundation/Foundation.h>

@class iSmartNews;

typedef BOOL (^CanIShowAlertViewRightNowHandler)(iSmartNews* smartNews);

/*!
 @addtogroup iSmartNewsMessageKeys Keys used in description of message.
 @{
 */

/*! @brief
 If value with specified key presents, then should contain title of message as NSString object.
 */
extern NSString*    iSmartNewsMessageTitleKey;

/*! @brief
 If value with specified key presents, then should contain text of message as NSString object.
 */
extern NSString*    iSmartNewsMessageTextKey;

/*! @brief
 If value with specified key presents, then should contain title of cancel button as NSString object.
 In another case localized version of 'Cancel' will be used.
 */
extern NSString*    iSmartNewsMessageCancelKey;

/*! @brief
 If value with specified key presents, then should contain title of Ok button as NSString object.
 In another case localized version of 'Ok' will be used. If no iSmartNewsMessageUrlKey
 was found, then this button is also hidden, because nothing to open.
 */
extern NSString*    iSmartNewsMessageActionKey;

/*! @brief
 If value with specified key presents, then should contain text of link as NSString object.
 That link will be opened if Ok is pressed.
 */
extern NSString*    iSmartNewsMessageUrlKey;

/*! @brief
 If value with specified key presents, then should contain minimum NSDate object.
 If found, then module compares current date with that one and if current date
 is equal or older that start date, then message can be shown.
 */
extern NSString*    iSmartNewsMessageStartDateKey;

/*! @brief
 If value with specified key presents, then should contain maximum NSDate object.
 If found, then module compares current date with that one and if current date
 is equal or earlier that end date, then message can be shown.
 */
extern NSString*    iSmartNewsMessageEndDateKey;

/*! @brief
 If value with specified key presents, then should contain NSNumber object.
 If found, then if it is NO, then message is shown only once event if user
 clicks 'Cancel', in another case message will be shown until 'Ok' is pressed.
 By default NO is assumed.
 */
extern NSString*    iSmartNewsMessageRepeatKey;

/*! @brief
 If value with specified key presents, then should contain NSNumber object.
 If found, then if it is YES, then message is shown every time even if user pressed 'Ok' or 'Cancel'.
 By default NO is assumed.
 */
extern NSString*    iSmartNewsMessageAlwaysKey;

/*! @brief
 If value with specified key presents, then should contain NSNumber object.
 Contains number of calls to update required to show news.
 Default value is assumed to be 0.
 @note Value is supported only in shared instance of iSmartNews object.
 */
extern NSString*    iSmartNewsMessageCounterKey;

/*! @brief
 Assigns message to message queue.
 @note Value is supported only in shared instance of iSmartNews object.
 @since Version 1.4
 */
extern NSString*    iSmartNewsMessageQueueKey;

/*! @brief
 Sets special type of content.
 @see  iSmartNewsContentTypeWeb.
 @since Version 1.7
 */
extern NSString*   iSmartNewsMessageTypeKey;

/*! @brief
 Sets special type of content to web.
 @see  iSmartNewsMessageTypeKey.
 @since Version 1.7
 */
extern NSString*   iSmartNewsContentTypeWeb;

/*!
 @}
 */

@class iSmartNews;
/*!
 @protocol iSmartNewsDelegate
 @brief    Protocol used for notifications emitted by iSmartNews objects.
 */
@protocol iSmartNewsDelegate <NSObject>
@optional
/*!
 @brief
 Optional method. If method was not found in delegate object,
 then news module will display news using custom alert view.
 Notifies about some news downloaded from passed URLs.
 
 @param _module
 News object itself.
 
 @param _descriptions
 Array of dictionaries with keys from 'iSmartNewsMessageKeys',
 please remember that most of the keys are optional.
 
 @note
 All news are marked as viewed.
 */
- (void)smartNewsModule:(iSmartNews*)_module didLoadMessages:(NSArray*)_descriptions;

/*!
 @brief
 Notifies about apperarence of UIAlertView.
 
 @since
 Version 1.1
 */
- (void)smartNewsModule:(iSmartNews*)_module didShowAlertView:(UIAlertView*)_alertView;

/*!
 @brief
 It is called when all messages where shown to user.
 
 */
- (void)smartNewsModuleDidCompleteMessageCycle:(iSmartNews*)_module;

// for log
- (void)smartNewsModule:(iSmartNews*)_module didCatchError:(NSError*)_error;

@end

/*!
 @class  iSmartNews
 @brief  Class helps to integrate some quick news
 functionality into iPhone/iPad application.
 @note
 Class is NOT thread safe.
 */
@interface iSmartNews : NSObject {
    
    /*! @cond SkipThis  */
    
    /*! @internal */
    NSTimer*                timer_;
    
    /*! @internal */
    BOOL                    start_;
    
    /*! @internal */
    NSInvocation*           timerInvocation_;
    
    /*! @internal */
    NSURLConnection*        connection_;
    
    /*! @internal */
    NSMutableData*          currentData_;
    
    /*! @internal */
    NSMutableArray*         newsData_;
    
    /*! @internal */
    NSMutableArray*         currentUrls_;
    
    /*! @internal */
    NSMutableArray*         loadedNews_;
    
    /*! @internal */
    UIAlertView*            alertView_;
    
    /*! @internal */
    /*! @since Version 1.3 */
    NSUInteger              gate_;
    
    /*! @endcond  */
}

/*!
 @brief
 Delegate.
 */
@property (nonatomic,assign)                  id<iSmartNewsDelegate>  delegate;

/*!
 @brief
 List of URLs to checked while news downloading.
 */
@property (nonatomic,copy)                    NSArray*                urls;

/*!
 @brief
 Update interval for automatic mode.
 Minimum allowed interval is 1 second.
 
 @note
 Default value is 24 * 60 * 60 seconds (once per day).
 */
@property (nonatomic,assign)                  CFTimeInterval          updateInterval;

/*!
 @brief
 Controls automatic update of news.
 If YES, then automatic update is enabled and update interval is
 controled by 'updateInterval' property. You can force update
 by calling 'update'.
 */
@property (nonatomic,assign)                  BOOL                    automaticMode;

/*!
 @brief
 Returns shared instance of news downloader.
 If you want, then you can create your own instance.
 
 @note
 Method should be called only from main thread.
 */
+ (iSmartNews*)sharedNews;

/*!
 @brief
 Forces to download news right now.
 If automatic update is enabled, then next update
 will take place after 'updateInterval'.
 
 @note
 Method should be called only from main thread.
 */
- (void)update;

/*!
 @brief
 Clears cache which keeps hashes to determine if some message was
 already shown. Use it accurately.
 
 @note
 Method should be called only from main thread.
 */
+ (void)clearCache;

/*!
 @brief
 Reset counter of 'update' calls.
 */
+ (void)resetRunCounter;

/*!
 @brief
 Sets handler that can be called to prevent news alert view from showing right now.
 If handler returns YES, then popup will be shown, in another case popup will be shown later.
 */
+ (void)setCanIShowAlertViewRightNowHandler:(CanIShowAlertViewRightNowHandler)CanIShowAlertViewRightNow;

/*!
 @brief
 Prevents currently loaded messages from showing.
 */
- (void)clear;

/*!
 @brief
 Prevents currently loaded messages from showing. Also close alert if shown.
 */
- (void)clearAndHideAlert;

/*! @internal
 Should be used only by special modules.
 */
- (BOOL)isAlertShown;

@end

/*!
 @addtogroup    iSmartNewsHelperMacros Some helpful macros.
  @{
 */

/*!
 @brief Just place this macro inside:
 @li - (void)applicationDidFinishLaunching:(UIApplication *)application;
 or:
 @li - (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions;
 @note @ref Usage
 */
#define iSmartNews_ApplicationDidFinishLaunchingAction(_urls)                   \
[[iSmartNews sharedNews] setUrls:_urls];                                    \
[[iSmartNews sharedNews] update];

/*!
 @brief Just place this macro inside:
 @li  - (void)applicationDidEnterBackground:(UIApplication *)application;
 @note @ref Usage
 */
#define iSmartNews_ApplicationDidEnterBackgroundAction()

/*!
 @brief Just place this macro inside:
 @li  - (void)applicationWillEnterForeground:(UIApplication *)application;
 @note @ref Usage
 */
#define iSmartNews_ApplicationWillEnterForegroundAction()                       \
[[iSmartNews sharedNews] update];

/*!
 @brief Just place this macro inside:
 @li - (void)applicationWillTerminate:(UIApplication *)application;
 @note @ref Usage
 */
#define iSmartNews_ApplicationWillTerminateAction()


/*!
 @brief Set new news URLs.
 */
#define iSmartNews_UpdateUrls(urls)                                             \
[[iSmartNews sharedNews] setUrls:urls];

/*!
 @brief Starts manual update of shared news module.
 */
#define iSmartNews_Update()                                                     \
[[iSmartNews sharedNews] update];

/*!
 @brief Clears news modules cache (it is common for all modules).
 */
#define iSmartNews_ClearCache()                                                 \
[iSmartNews clearCache];

/*!
 @}
 */
