/*!
 @file       iSmartNews.m
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

#import "iSmartNews.h"
#import <CommonCrypto/CommonDigest.h>
#include <stdio.h>
#import <objc/runtime.h>
#import <objc/message.h>
#import <UIKit/UIKit.h>
#import <QuartzCore/QuartzCore.h>

@interface UIApplication(iSmartNews)
- (void)iSmartNews_hideStatusbar:(BOOL)iSmartNews_hideStatusbar animated:(BOOL)animated;
@end

@implementation UIApplication(iSmartNews)

- (BOOL)original_isStatusBarHidden{
    NSNumber* num = objc_getAssociatedObject(self, "iSmartNews_hideStatusbar_original");
    if (!num){
        num = @([self iSmartNews_isStatusBarHidden]);
        objc_setAssociatedObject(self, "iSmartNews_hideStatusbar_original", num, OBJC_ASSOCIATION_RETAIN_NONATOMIC);
    }
    return [num boolValue];
}

- (void)iSmartNews_hideStatusbar:(BOOL)iSmartNews_hideStatusbar animated:(BOOL)animated{
    if ([self original_isStatusBarHidden] == iSmartNews_hideStatusbar)
        return;
    
    objc_setAssociatedObject(self, "iSmartNews_hideStatusbar", @(iSmartNews_hideStatusbar), OBJC_ASSOCIATION_RETAIN_NONATOMIC);
    [self iSmartNews_updateStatusbarVisibility:animated];
}

- (void)iSmartNews_updateStatusbarVisibility:(BOOL)animated{
    if ([objc_getAssociatedObject(self, "iSmartNews_hideStatusbar") boolValue]){
        [self iSmartNews_setStatusBarHidden:YES animated:animated];
    }
    else{
        [self iSmartNews_setStatusBarHidden:[self original_isStatusBarHidden] animated:animated];
    }
}

- (void)iSmartNews_updateStatusbarVisibilityWithAnimation:(UIStatusBarAnimation)animation{
    if ([objc_getAssociatedObject(self, "iSmartNews_hideStatusbar") boolValue]){
        [self iSmartNews_setStatusBarHidden:YES withAnimation:animation];
    }
    else{
        [self iSmartNews_setStatusBarHidden:[self original_isStatusBarHidden] withAnimation:animation];
    }
}

- (void)iSmartNews_setStatusBarHidden:(BOOL)hidden animated:(BOOL)animated{
    objc_setAssociatedObject(self, "iSmartNews_hideStatusbar_original", @(hidden), OBJC_ASSOCIATION_RETAIN_NONATOMIC);
    [self iSmartNews_updateStatusbarVisibility:animated];
}

- (void)iSmartNews_setStatusBarHidden:(BOOL)hidden withAnimation:(UIStatusBarAnimation)animation{
    objc_setAssociatedObject(self, "iSmartNews_hideStatusbar_original", @(hidden), OBJC_ASSOCIATION_RETAIN_NONATOMIC);
    [self iSmartNews_updateStatusbarVisibilityWithAnimation:animation];
}

- (BOOL)iSmartNews_isStatusBarHidden{
    return [self original_isStatusBarHidden];
}

@end

static void SwizzleInstanceMethod(Class class, SEL old, SEL newSelector)
{
    Method oldMethod = class_getInstanceMethod(class, old);
    Method newMethod = class_getInstanceMethod(class, newSelector);
    
    if(class_addMethod(class, old, method_getImplementation(newMethod), method_getTypeEncoding(newMethod)))
    {
        class_replaceMethod(class, newSelector, method_getImplementation(oldMethod), method_getTypeEncoding(oldMethod));
    }
    else
    {
        method_exchangeImplementations(oldMethod, newMethod);
    }
}

#define ENSURE_MAIN_THREAD         assert([NSThread isMainThread] && "Should be called from main thread only!")

#if defined(DEBUG) && 1
# define DBGLOG(...)        NSLog(@"iSmartNews: %@",[NSString stringWithFormat:__VA_ARGS__])
#else
# define DBGLOG(...)        ((void)0)
#endif

#if __has_feature(objc_arc)
# define USES_ARC           1
# define AUTORELEASE(...)   __VA_ARGS__
# define RETAIN(...)        __VA_ARGS__
# define RELEASE(...)       __VA_ARGS__
#else
# define USES_ARC           0
# define AUTORELEASE(...)   [__VA_ARGS__ autorelease]
# define RETAIN(...)        [__VA_ARGS__ retain]
# define RELEASE(...)       [__VA_ARGS__ release]
#endif

#if USES_ARC
# define iSmartNews_retain      strong
# define iSmartNews_assign      weak
#else
# define iSmartNews_retain      retain
# define iSmartNews_assign      assign
#endif

@class iSmartNewsUAModalPanel;
@class iSmartNewsUARoundedRectView;


@class iSmartNewsUAModalPanel;

@protocol iSmartNewsUAModalPanelDelegate
@optional
- (void)willShowModalPanel:(iSmartNewsUAModalPanel *)modalPanel;
- (void)didShowModalPanel:(iSmartNewsUAModalPanel *)modalPanel;
- (void)didSelectActionButton:(iSmartNewsUAModalPanel *)modalPanel;
- (BOOL)shouldCloseModalPanel:(iSmartNewsUAModalPanel *)modalPanel;
- (void)willCloseModalPanel:(iSmartNewsUAModalPanel *)modalPanel;
- (void)didCloseModalPanel:(iSmartNewsUAModalPanel *)modalPanel;
@end

typedef void (^iSmartNewsUAModalDisplayPanelEvent)(iSmartNewsUAModalPanel* panel);
typedef void (^iSmartNewsUAModalDisplayPanelAnimationComplete)(BOOL finished);

@interface iSmartNewsUAModalPanel : UIView {
    
#if __has_feature(objc_arc)
    __weak
#endif
	NSObject<iSmartNewsUAModalPanelDelegate>	*delegate;
	
	UIView			*contentContainer;
	UIView			*roundedRect;
	UIButton		*closeButton;
	UIButton		*actionButton;
	UIView			*contentView;
	
	CGPoint			startEndPoint;
	
	UIEdgeInsets	margin;
	UIEdgeInsets	padding;
	
	UIColor			*borderColor;
	CGFloat			borderWidth;
	CGFloat			cornerRadius;
	UIColor			*contentColor;
	BOOL			shouldBounce;
	
}

@property (nonatomic,
#if !__has_feature(objc_arc)
           assign
#else
           weak
#endif
) NSObject<iSmartNewsUAModalPanelDelegate>	*delegate;

@property (nonatomic, retain) UIView		*contentContainer;
@property (nonatomic, retain) UIView		*roundedRect;
@property (nonatomic, retain) UIButton		*closeButton;
@property (nonatomic, retain) UIButton		*actionButton;
@property (nonatomic, retain) UIView		*contentView;

// Margin between edge of container frame and panel. Default = {20.0, 20.0, 20.0, 20.0}
@property (nonatomic, assign) UIEdgeInsets	margin;
// Padding between edge of panel and the content area. Default = {20.0, 20.0, 20.0, 20.0}
@property (nonatomic, assign) UIEdgeInsets	padding;
// Border color of the panel. Default = [UIColor whiteColor]
@property (nonatomic, retain) UIColor		*borderColor;
// Border width of the panel. Default = 1.5f
@property (nonatomic, assign) CGFloat		borderWidth;
// Corner radius of the panel. Default = 4.0f
@property (nonatomic, assign) CGFloat		cornerRadius;
// Color of the panel itself. Default = [UIColor colorWithWhite:0.0 alpha:0.8]
@property (nonatomic, retain) UIColor		*contentColor;
// Shows the bounce animation. Default = YES
@property (nonatomic, assign) BOOL			shouldBounce;

@property (readwrite, copy)	iSmartNewsUAModalDisplayPanelEvent onClosePressed;
@property (readwrite, copy)	iSmartNewsUAModalDisplayPanelEvent onActionPressed;

- (void)show;
- (void)showFromPoint:(CGPoint)point;
- (void)hide;
- (void)hideWithOnComplete:(iSmartNewsUAModalDisplayPanelAnimationComplete)onComplete;

- (CGRect)roundedRectFrame;
- (CGRect)closeButtonFrame;
- (CGRect)contentViewFrame;

@end


@interface iSmartNewsUARoundedRectView : UIView {
	NSInteger	radius;
	CGFloat		*colorComponents;
}

@property (nonatomic, assign) NSInteger	radius;

- (void)setColors:(CGFloat *)components;

@end





#define DEFAULT_MARGIN				20.0f
#define DEFAULT_BACKGROUND_COLOR	[UIColor colorWithWhite:0.0 alpha:0.8]
#define DEFAULT_CORNER_RADIUS		4.0f
#define DEFAULT_BORDER_WIDTH		1.5f
#define DEFAULT_BORDER_COLOR		[UIColor whiteColor]
#define DEFAULT_BOUNCE				YES

static const UInt8 close_png[] = {
    0x89,0x50,0x4E,0x47,0x0D,0x0A,0x1A,0x0A,0x00,0x00,0x00,0x0D,0x49,0x48,0x44,0x52,0x00,0x00,0x00,0x16,0x00,0x00,0x00,0x16,0x08,
    0x06,0x00,0x00,0x00,0xC4,0xB4,0x6C,0x3B,0x00,0x00,0x00,0x19,0x74,0x45,0x58,0x74,0x53,0x6F,0x66,0x74,0x77,0x61,0x72,0x65,0x00,
    0x41,0x64,0x6F,0x62,0x65,0x20,0x49,0x6D,0x61,0x67,0x65,0x52,0x65,0x61,0x64,0x79,0x71,0xC9,0x65,0x3C,0x00,0x00,0x03,0x22,0x69,
    0x54,0x58,0x74,0x58,0x4D,0x4C,0x3A,0x63,0x6F,0x6D,0x2E,0x61,0x64,0x6F,0x62,0x65,0x2E,0x78,0x6D,0x70,0x00,0x00,0x00,0x00,0x00,
    0x3C,0x3F,0x78,0x70,0x61,0x63,0x6B,0x65,0x74,0x20,0x62,0x65,0x67,0x69,0x6E,0x3D,0x22,0xEF,0xBB,0xBF,0x22,0x20,0x69,0x64,0x3D,
    0x22,0x57,0x35,0x4D,0x30,0x4D,0x70,0x43,0x65,0x68,0x69,0x48,0x7A,0x72,0x65,0x53,0x7A,0x4E,0x54,0x63,0x7A,0x6B,0x63,0x39,0x64,
    0x22,0x3F,0x3E,0x20,0x3C,0x78,0x3A,0x78,0x6D,0x70,0x6D,0x65,0x74,0x61,0x20,0x78,0x6D,0x6C,0x6E,0x73,0x3A,0x78,0x3D,0x22,0x61,
    0x64,0x6F,0x62,0x65,0x3A,0x6E,0x73,0x3A,0x6D,0x65,0x74,0x61,0x2F,0x22,0x20,0x78,0x3A,0x78,0x6D,0x70,0x74,0x6B,0x3D,0x22,0x41,
    0x64,0x6F,0x62,0x65,0x20,0x58,0x4D,0x50,0x20,0x43,0x6F,0x72,0x65,0x20,0x35,0x2E,0x30,0x2D,0x63,0x30,0x36,0x30,0x20,0x36,0x31,
    0x2E,0x31,0x33,0x34,0x37,0x37,0x37,0x2C,0x20,0x32,0x30,0x31,0x30,0x2F,0x30,0x32,0x2F,0x31,0x32,0x2D,0x31,0x37,0x3A,0x33,0x32,
    0x3A,0x30,0x30,0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x22,0x3E,0x20,0x3C,0x72,0x64,0x66,0x3A,0x52,0x44,0x46,0x20,0x78,0x6D,
    0x6C,0x6E,0x73,0x3A,0x72,0x64,0x66,0x3D,0x22,0x68,0x74,0x74,0x70,0x3A,0x2F,0x2F,0x77,0x77,0x77,0x2E,0x77,0x33,0x2E,0x6F,0x72,
    0x67,0x2F,0x31,0x39,0x39,0x39,0x2F,0x30,0x32,0x2F,0x32,0x32,0x2D,0x72,0x64,0x66,0x2D,0x73,0x79,0x6E,0x74,0x61,0x78,0x2D,0x6E,
    0x73,0x23,0x22,0x3E,0x20,0x3C,0x72,0x64,0x66,0x3A,0x44,0x65,0x73,0x63,0x72,0x69,0x70,0x74,0x69,0x6F,0x6E,0x20,0x72,0x64,0x66,
    0x3A,0x61,0x62,0x6F,0x75,0x74,0x3D,0x22,0x22,0x20,0x78,0x6D,0x6C,0x6E,0x73,0x3A,0x78,0x6D,0x70,0x3D,0x22,0x68,0x74,0x74,0x70,
    0x3A,0x2F,0x2F,0x6E,0x73,0x2E,0x61,0x64,0x6F,0x62,0x65,0x2E,0x63,0x6F,0x6D,0x2F,0x78,0x61,0x70,0x2F,0x31,0x2E,0x30,0x2F,0x22,
    0x20,0x78,0x6D,0x6C,0x6E,0x73,0x3A,0x78,0x6D,0x70,0x4D,0x4D,0x3D,0x22,0x68,0x74,0x74,0x70,0x3A,0x2F,0x2F,0x6E,0x73,0x2E,0x61,
    0x64,0x6F,0x62,0x65,0x2E,0x63,0x6F,0x6D,0x2F,0x78,0x61,0x70,0x2F,0x31,0x2E,0x30,0x2F,0x6D,0x6D,0x2F,0x22,0x20,0x78,0x6D,0x6C,
    0x6E,0x73,0x3A,0x73,0x74,0x52,0x65,0x66,0x3D,0x22,0x68,0x74,0x74,0x70,0x3A,0x2F,0x2F,0x6E,0x73,0x2E,0x61,0x64,0x6F,0x62,0x65,
    0x2E,0x63,0x6F,0x6D,0x2F,0x78,0x61,0x70,0x2F,0x31,0x2E,0x30,0x2F,0x73,0x54,0x79,0x70,0x65,0x2F,0x52,0x65,0x73,0x6F,0x75,0x72,
    0x63,0x65,0x52,0x65,0x66,0x23,0x22,0x20,0x78,0x6D,0x70,0x3A,0x43,0x72,0x65,0x61,0x74,0x6F,0x72,0x54,0x6F,0x6F,0x6C,0x3D,0x22,
    0x41,0x64,0x6F,0x62,0x65,0x20,0x50,0x68,0x6F,0x74,0x6F,0x73,0x68,0x6F,0x70,0x20,0x43,0x53,0x35,0x20,0x4D,0x61,0x63,0x69,0x6E,
    0x74,0x6F,0x73,0x68,0x22,0x20,0x78,0x6D,0x70,0x4D,0x4D,0x3A,0x49,0x6E,0x73,0x74,0x61,0x6E,0x63,0x65,0x49,0x44,0x3D,0x22,0x78,
    0x6D,0x70,0x2E,0x69,0x69,0x64,0x3A,0x43,0x42,0x30,0x45,0x33,0x31,0x41,0x43,0x41,0x31,0x42,0x44,0x31,0x31,0x45,0x30,0x42,0x34,
    0x43,0x38,0x42,0x45,0x44,0x43,0x41,0x36,0x32,0x35,0x37,0x42,0x39,0x43,0x22,0x20,0x78,0x6D,0x70,0x4D,0x4D,0x3A,0x44,0x6F,0x63,
    0x75,0x6D,0x65,0x6E,0x74,0x49,0x44,0x3D,0x22,0x78,0x6D,0x70,0x2E,0x64,0x69,0x64,0x3A,0x43,0x42,0x30,0x45,0x33,0x31,0x41,0x44,
    0x41,0x31,0x42,0x44,0x31,0x31,0x45,0x30,0x42,0x34,0x43,0x38,0x42,0x45,0x44,0x43,0x41,0x36,0x32,0x35,0x37,0x42,0x39,0x43,0x22,
    0x3E,0x20,0x3C,0x78,0x6D,0x70,0x4D,0x4D,0x3A,0x44,0x65,0x72,0x69,0x76,0x65,0x64,0x46,0x72,0x6F,0x6D,0x20,0x73,0x74,0x52,0x65,
    0x66,0x3A,0x69,0x6E,0x73,0x74,0x61,0x6E,0x63,0x65,0x49,0x44,0x3D,0x22,0x78,0x6D,0x70,0x2E,0x69,0x69,0x64,0x3A,0x43,0x42,0x30,
    0x45,0x33,0x31,0x41,0x41,0x41,0x31,0x42,0x44,0x31,0x31,0x45,0x30,0x42,0x34,0x43,0x38,0x42,0x45,0x44,0x43,0x41,0x36,0x32,0x35,
    0x37,0x42,0x39,0x43,0x22,0x20,0x73,0x74,0x52,0x65,0x66,0x3A,0x64,0x6F,0x63,0x75,0x6D,0x65,0x6E,0x74,0x49,0x44,0x3D,0x22,0x78,
    0x6D,0x70,0x2E,0x64,0x69,0x64,0x3A,0x43,0x42,0x30,0x45,0x33,0x31,0x41,0x42,0x41,0x31,0x42,0x44,0x31,0x31,0x45,0x30,0x42,0x34,
    0x43,0x38,0x42,0x45,0x44,0x43,0x41,0x36,0x32,0x35,0x37,0x42,0x39,0x43,0x22,0x2F,0x3E,0x20,0x3C,0x2F,0x72,0x64,0x66,0x3A,0x44,
    0x65,0x73,0x63,0x72,0x69,0x70,0x74,0x69,0x6F,0x6E,0x3E,0x20,0x3C,0x2F,0x72,0x64,0x66,0x3A,0x52,0x44,0x46,0x3E,0x20,0x3C,0x2F,
    0x78,0x3A,0x78,0x6D,0x70,0x6D,0x65,0x74,0x61,0x3E,0x20,0x3C,0x3F,0x78,0x70,0x61,0x63,0x6B,0x65,0x74,0x20,0x65,0x6E,0x64,0x3D,
    0x22,0x72,0x22,0x3F,0x3E,0xEA,0xD7,0x5C,0xE3,0x00,0x00,0x03,0x74,0x49,0x44,0x41,0x54,0x78,0xDA,0x9C,0x95,0x4F,0x2C,0x9B,0x61,
    0x1C,0xC7,0xDF,0xB7,0xB4,0xB4,0xF5,0xAF,0x13,0xC5,0xC8,0x76,0x20,0x98,0x03,0x09,0xCB,0x0E,0x1C,0x68,0xD7,0xEC,0x80,0x45,0x22,
    0x1C,0x08,0x73,0x58,0xA2,0x31,0x87,0x26,0x5B,0x1C,0xB6,0xCB,0x16,0x17,0x07,0x09,0x73,0xE0,0x60,0x1B,0x5B,0x1C,0x49,0x1C,0xC4,
    0x46,0x96,0x59,0x49,0x58,0x24,0x9D,0xC4,0x61,0x22,0x93,0x25,0x92,0x32,0xA5,0x0E,0xFA,0x87,0x52,0xF4,0xB7,0xEF,0xEF,0xCD,0xDA,
    0xF5,0xEF,0xCC,0xDE,0xE4,0xE9,0xFB,0xBE,0x4F,0x7F,0xCF,0xE7,0xFD,0xFE,0xFE,0x3D,0x8F,0xE8,0x74,0x3A,0x85,0x58,0x17,0x11,0xE5,
    0xC6,0xC5,0xC5,0xE9,0x13,0x12,0x12,0xEE,0xE0,0x5E,0x2D,0x8A,0xE2,0x75,0x9F,0xCF,0xB7,0x7C,0x76,0x76,0xB6,0xE8,0xF5,0x7A,0x2D,
    0x30,0x99,0x8F,0xB5,0x56,0x8C,0x06,0x06,0x50,0x2B,0x97,0xCB,0x8D,0x4A,0xA5,0xB2,0xF9,0xF8,0xF8,0xF8,0xD6,0xF6,0xF6,0x36,0x1D,
    0x1E,0x1E,0x0A,0x00,0x8A,0x49,0x49,0x49,0x94,0x91,0x91,0x21,0x66,0x67,0x67,0xBB,0x2E,0x2E,0x2E,0xE6,0x3D,0x1E,0xCF,0x4B,0x2C,
    0x31,0x47,0x40,0x18,0x1C,0x36,0xEE,0x43,0xD5,0x8F,0x83,0x83,0x03,0x1A,0x18,0x18,0xA0,0xCA,0xCA,0x4A,0xD2,0x6A,0xB5,0x04,0xC5,
    0xC4,0xDF,0x64,0x70,0x7E,0x7E,0x3E,0xB5,0xB7,0xB7,0xD3,0xD2,0xD2,0x12,0x34,0x90,0x0F,0xF0,0xD7,0x0E,0x87,0x43,0x13,0xCC,0x09,
    0x87,0x3E,0x02,0xD4,0x3B,0x3D,0x3D,0x4D,0x25,0x25,0x25,0x12,0xE8,0x6F,0x23,0x31,0x31,0x91,0xBA,0xBA,0xBA,0xC8,0x6E,0xB7,0x13,
    0x42,0xB3,0x0C,0x78,0x6E,0x34,0xF0,0x5D,0x7C,0xDD,0x33,0x32,0x32,0x42,0x0A,0x85,0xE2,0x52,0x68,0xF0,0xA8,0xAA,0xAA,0x22,0x0E,
    0xD7,0xE9,0xE9,0xE9,0x7B,0xC0,0xE5,0xC1,0x60,0x2D,0x94,0x5A,0xE7,0xE6,0xE6,0x48,0xA5,0x52,0x5D,0x09,0xEA,0x1F,0xB5,0xB5,0xB5,
    0x84,0x7C,0x90,0xDB,0xED,0x7E,0x16,0x00,0x63,0xE2,0xC5,0xFE,0xFE,0x3E,0x15,0x14,0x14,0x84,0x18,0x97,0x96,0x96,0x52,0x53,0x53,
    0x13,0x25,0x27,0x27,0x87,0xCC,0x1B,0x0C,0x06,0x6A,0x6C,0x6C,0x24,0x24,0x37,0x64,0x7E,0x70,0x70,0x90,0x63,0x6E,0x87,0xEA,0x1C,
    0x01,0x3F,0x37,0xF0,0xF2,0xBD,0xBF,0xBF,0x3F,0xC4,0x28,0x3D,0x3D,0x9D,0x36,0x37,0x37,0xD9,0x90,0xD8,0x13,0x3F,0xBC,0xB3,0xB3,
    0x93,0x5D,0x96,0xE6,0x8D,0x46,0x63,0xC8,0x9A,0xBC,0xBC,0x3C,0xB2,0xD9,0x6C,0xAC,0xFC,0x89,0x00,0xE9,0x0F,0x8E,0x8E,0x8E,0xA4,
    0xEC,0x07,0x1B,0xA1,0xDC,0xFC,0x0A,0xA4,0x6B,0x68,0x68,0x88,0xEA,0xEA,0xEA,0x02,0xD0,0x8D,0x8D,0x0D,0x2A,0x2E,0x2E,0x8E,0x08,
    0xC9,0xC4,0xC4,0x04,0xA1,0x0C,0x3F,0x72,0x6D,0xBE,0x59,0x5F,0x5F,0xF7,0xB1,0xC2,0x70,0x23,0x4E,0xE2,0xE4,0xE4,0xA4,0x04,0x42,
    0x0E,0x08,0xDE,0x49,0xCF,0x7B,0x7B,0x7B,0x54,0x5E,0x5E,0x1E,0x35,0xD6,0x26,0x93,0x89,0x6D,0xBF,0x71,0x33,0xD8,0xB8,0x1E,0x63,
    0x25,0x25,0x25,0x25,0x85,0x16,0x16,0x16,0x02,0xCA,0xD9,0xBB,0x9A,0x9A,0x9A,0x98,0xF6,0xF5,0xF5,0xF5,0x6C,0xE6,0x96,0xE1,0xE5,
    0x1A,0xA4,0xC7,0x6C,0x6B,0xA8,0x16,0xE2,0xE3,0xE3,0xFF,0xB4,0xAA,0x28,0x0A,0xA8,0x9C,0x98,0xF6,0x08,0x15,0xDF,0x54,0x02,0xF7,
    0xBE,0xC5,0x62,0xF1,0x61,0x3F,0x88,0xF8,0x3A,0x97,0xDE,0xEC,0xEC,0xAC,0xA4,0x94,0x63,0xCB,0x89,0xE1,0x0B,0xED,0x4D,0x3A,0x9D,
    0x2E,0xAA,0xE2,0x96,0x96,0x16,0x36,0xD9,0x15,0x4E,0x4E,0x4E,0x9E,0xEE,0xEC,0xEC,0x48,0x19,0x0D,0x36,0x50,0xAB,0xD5,0x34,0x3E,
    0x3E,0x1E,0x08,0x41,0x6F,0x6F,0x2F,0x55,0x54,0x54,0x04,0xE2,0xBC,0xB5,0xB5,0x45,0x65,0x65,0x65,0x11,0xE0,0xBE,0xBE,0x3E,0x8E,
    0xF1,0x17,0xAE,0x63,0x1D,0x1E,0x5C,0x6D,0x6D,0x6D,0x21,0x06,0x5C,0xD3,0x08,0x91,0x04,0x99,0x9A,0x9A,0x22,0xBF,0x47,0xAD,0xAD,
    0xAD,0x81,0xCA,0xE0,0x8F,0x05,0xAF,0x61,0x1B,0x78,0xCF,0xED,0xFD,0x8E,0xC1,0xF1,0xA8,0x8C,0x19,0xB3,0xD9,0x1C,0xD8,0x68,0x78,
    0x20,0xAE,0xD4,0xD1,0xD1,0x41,0xDD,0xDD,0xDD,0x94,0x96,0x96,0x16,0xD1,0x65,0x3D,0x3D,0x3D,0x54,0x58,0x58,0x18,0x32,0xDF,0xD0,
    0xD0,0x40,0x60,0x11,0x98,0x7A,0xA9,0xF3,0x5C,0x2E,0x97,0x81,0xD5,0x71,0xF1,0xFF,0x4F,0x3B,0xF3,0x48,0x4D,0x4D,0xA5,0xD5,0xD5,
    0x55,0x06,0x7F,0x02,0x53,0xEE,0xDF,0x2B,0x64,0xE8,0x96,0x57,0x9C,0x14,0xBD,0x5E,0x7F,0x65,0x28,0x7B,0x37,0x3A,0x3A,0xCA,0xD1,
    0x01,0xCA,0x79,0x3B,0x64,0x77,0x43,0x52,0x94,0x88,0xCD,0xA2,0xD5,0x6A,0x95,0x3A,0xEC,0x5F,0xA1,0x1A,0x8D,0x86,0xC6,0xC6,0xC6,
    0x38,0x61,0x3E,0x70,0x1E,0x46,0xDD,0x8F,0x01,0xBF,0x89,0xC4,0x7C,0xE0,0x26,0x18,0x1E,0x1E,0x8E,0xD8,0x94,0xC2,0x13,0xC5,0xA5,
    0xB5,0xB6,0xB6,0xC6,0x4A,0x1D,0x08,0xA7,0x31,0x98,0x15,0xED,0x68,0x52,0xC8,0x64,0xB2,0xC7,0x28,0x37,0x13,0x5A,0x37,0x0B,0x5D,
    0x27,0xAC,0xAC,0xAC,0x08,0xF0,0x84,0x77,0x41,0x21,0x33,0x33,0x53,0x28,0x2A,0x2A,0x12,0xAA,0xAB,0xAB,0x05,0x94,0x1B,0x77,0xEE,
    0x0C,0x4E,0x90,0xE7,0x58,0xF7,0xF5,0xD2,0x33,0xEF,0xF7,0xB9,0x97,0x83,0x8D,0xA8,0x19,0xCA,0xEE,0xA1,0xDB,0xB2,0x30,0x72,0x31,
    0xAD,0xC6,0xD8,0xC5,0x7F,0x3F,0xCF,0xCF,0xCF,0x37,0xE1,0xDD,0x5B,0x3C,0x7F,0x8E,0xB6,0xFE,0x97,0x00,0x03,0x00,0x39,0x3E,0x76,
    0xF4,0x97,0x5A,0x5B,0xBE,0x00,0x00,0x00,0x00,0x49,0x45,0x4E,0x44,0xAE,0x42,0x60,0x82};

static const UInt8 close2_png[] = {
    0x89,0x50,0x4E,0x47,0x0D,0x0A,0x1A,0x0A,0x00,0x00,0x00,0x0D,0x49,0x48,0x44,0x52,0x00,0x00,0x00,0x2C,0x00,0x00,0x00,0x2C,0x08,
    0x06,0x00,0x00,0x00,0x1E,0x84,0x5A,0x01,0x00,0x00,0x00,0x19,0x74,0x45,0x58,0x74,0x53,0x6F,0x66,0x74,0x77,0x61,0x72,0x65,0x00,
    0x41,0x64,0x6F,0x62,0x65,0x20,0x49,0x6D,0x61,0x67,0x65,0x52,0x65,0x61,0x64,0x79,0x71,0xC9,0x65,0x3C,0x00,0x00,0x03,0x22,0x69,
    0x54,0x58,0x74,0x58,0x4D,0x4C,0x3A,0x63,0x6F,0x6D,0x2E,0x61,0x64,0x6F,0x62,0x65,0x2E,0x78,0x6D,0x70,0x00,0x00,0x00,0x00,0x00,
    0x3C,0x3F,0x78,0x70,0x61,0x63,0x6B,0x65,0x74,0x20,0x62,0x65,0x67,0x69,0x6E,0x3D,0x22,0xEF,0xBB,0xBF,0x22,0x20,0x69,0x64,0x3D,
    0x22,0x57,0x35,0x4D,0x30,0x4D,0x70,0x43,0x65,0x68,0x69,0x48,0x7A,0x72,0x65,0x53,0x7A,0x4E,0x54,0x63,0x7A,0x6B,0x63,0x39,0x64,
    0x22,0x3F,0x3E,0x20,0x3C,0x78,0x3A,0x78,0x6D,0x70,0x6D,0x65,0x74,0x61,0x20,0x78,0x6D,0x6C,0x6E,0x73,0x3A,0x78,0x3D,0x22,0x61,
    0x64,0x6F,0x62,0x65,0x3A,0x6E,0x73,0x3A,0x6D,0x65,0x74,0x61,0x2F,0x22,0x20,0x78,0x3A,0x78,0x6D,0x70,0x74,0x6B,0x3D,0x22,0x41,
    0x64,0x6F,0x62,0x65,0x20,0x58,0x4D,0x50,0x20,0x43,0x6F,0x72,0x65,0x20,0x35,0x2E,0x30,0x2D,0x63,0x30,0x36,0x30,0x20,0x36,0x31,
    0x2E,0x31,0x33,0x34,0x37,0x37,0x37,0x2C,0x20,0x32,0x30,0x31,0x30,0x2F,0x30,0x32,0x2F,0x31,0x32,0x2D,0x31,0x37,0x3A,0x33,0x32,
    0x3A,0x30,0x30,0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x22,0x3E,0x20,0x3C,0x72,0x64,0x66,0x3A,0x52,0x44,0x46,0x20,0x78,0x6D,
    0x6C,0x6E,0x73,0x3A,0x72,0x64,0x66,0x3D,0x22,0x68,0x74,0x74,0x70,0x3A,0x2F,0x2F,0x77,0x77,0x77,0x2E,0x77,0x33,0x2E,0x6F,0x72,
    0x67,0x2F,0x31,0x39,0x39,0x39,0x2F,0x30,0x32,0x2F,0x32,0x32,0x2D,0x72,0x64,0x66,0x2D,0x73,0x79,0x6E,0x74,0x61,0x78,0x2D,0x6E,
    0x73,0x23,0x22,0x3E,0x20,0x3C,0x72,0x64,0x66,0x3A,0x44,0x65,0x73,0x63,0x72,0x69,0x70,0x74,0x69,0x6F,0x6E,0x20,0x72,0x64,0x66,
    0x3A,0x61,0x62,0x6F,0x75,0x74,0x3D,0x22,0x22,0x20,0x78,0x6D,0x6C,0x6E,0x73,0x3A,0x78,0x6D,0x70,0x3D,0x22,0x68,0x74,0x74,0x70,
    0x3A,0x2F,0x2F,0x6E,0x73,0x2E,0x61,0x64,0x6F,0x62,0x65,0x2E,0x63,0x6F,0x6D,0x2F,0x78,0x61,0x70,0x2F,0x31,0x2E,0x30,0x2F,0x22,
    0x20,0x78,0x6D,0x6C,0x6E,0x73,0x3A,0x78,0x6D,0x70,0x4D,0x4D,0x3D,0x22,0x68,0x74,0x74,0x70,0x3A,0x2F,0x2F,0x6E,0x73,0x2E,0x61,
    0x64,0x6F,0x62,0x65,0x2E,0x63,0x6F,0x6D,0x2F,0x78,0x61,0x70,0x2F,0x31,0x2E,0x30,0x2F,0x6D,0x6D,0x2F,0x22,0x20,0x78,0x6D,0x6C,
    0x6E,0x73,0x3A,0x73,0x74,0x52,0x65,0x66,0x3D,0x22,0x68,0x74,0x74,0x70,0x3A,0x2F,0x2F,0x6E,0x73,0x2E,0x61,0x64,0x6F,0x62,0x65,
    0x2E,0x63,0x6F,0x6D,0x2F,0x78,0x61,0x70,0x2F,0x31,0x2E,0x30,0x2F,0x73,0x54,0x79,0x70,0x65,0x2F,0x52,0x65,0x73,0x6F,0x75,0x72,
    0x63,0x65,0x52,0x65,0x66,0x23,0x22,0x20,0x78,0x6D,0x70,0x3A,0x43,0x72,0x65,0x61,0x74,0x6F,0x72,0x54,0x6F,0x6F,0x6C,0x3D,0x22,
    0x41,0x64,0x6F,0x62,0x65,0x20,0x50,0x68,0x6F,0x74,0x6F,0x73,0x68,0x6F,0x70,0x20,0x43,0x53,0x35,0x20,0x4D,0x61,0x63,0x69,0x6E,
    0x74,0x6F,0x73,0x68,0x22,0x20,0x78,0x6D,0x70,0x4D,0x4D,0x3A,0x49,0x6E,0x73,0x74,0x61,0x6E,0x63,0x65,0x49,0x44,0x3D,0x22,0x78,
    0x6D,0x70,0x2E,0x69,0x69,0x64,0x3A,0x43,0x42,0x30,0x45,0x33,0x31,0x41,0x38,0x41,0x31,0x42,0x44,0x31,0x31,0x45,0x30,0x42,0x34,
    0x43,0x38,0x42,0x45,0x44,0x43,0x41,0x36,0x32,0x35,0x37,0x42,0x39,0x43,0x22,0x20,0x78,0x6D,0x70,0x4D,0x4D,0x3A,0x44,0x6F,0x63,
    0x75,0x6D,0x65,0x6E,0x74,0x49,0x44,0x3D,0x22,0x78,0x6D,0x70,0x2E,0x64,0x69,0x64,0x3A,0x43,0x42,0x30,0x45,0x33,0x31,0x41,0x39,
    0x41,0x31,0x42,0x44,0x31,0x31,0x45,0x30,0x42,0x34,0x43,0x38,0x42,0x45,0x44,0x43,0x41,0x36,0x32,0x35,0x37,0x42,0x39,0x43,0x22,
    0x3E,0x20,0x3C,0x78,0x6D,0x70,0x4D,0x4D,0x3A,0x44,0x65,0x72,0x69,0x76,0x65,0x64,0x46,0x72,0x6F,0x6D,0x20,0x73,0x74,0x52,0x65,
    0x66,0x3A,0x69,0x6E,0x73,0x74,0x61,0x6E,0x63,0x65,0x49,0x44,0x3D,0x22,0x78,0x6D,0x70,0x2E,0x69,0x69,0x64,0x3A,0x43,0x42,0x30,
    0x45,0x33,0x31,0x41,0x36,0x41,0x31,0x42,0x44,0x31,0x31,0x45,0x30,0x42,0x34,0x43,0x38,0x42,0x45,0x44,0x43,0x41,0x36,0x32,0x35,
    0x37,0x42,0x39,0x43,0x22,0x20,0x73,0x74,0x52,0x65,0x66,0x3A,0x64,0x6F,0x63,0x75,0x6D,0x65,0x6E,0x74,0x49,0x44,0x3D,0x22,0x78,
    0x6D,0x70,0x2E,0x64,0x69,0x64,0x3A,0x43,0x42,0x30,0x45,0x33,0x31,0x41,0x37,0x41,0x31,0x42,0x44,0x31,0x31,0x45,0x30,0x42,0x34,
    0x43,0x38,0x42,0x45,0x44,0x43,0x41,0x36,0x32,0x35,0x37,0x42,0x39,0x43,0x22,0x2F,0x3E,0x20,0x3C,0x2F,0x72,0x64,0x66,0x3A,0x44,
    0x65,0x73,0x63,0x72,0x69,0x70,0x74,0x69,0x6F,0x6E,0x3E,0x20,0x3C,0x2F,0x72,0x64,0x66,0x3A,0x52,0x44,0x46,0x3E,0x20,0x3C,0x2F,
    0x78,0x3A,0x78,0x6D,0x70,0x6D,0x65,0x74,0x61,0x3E,0x20,0x3C,0x3F,0x78,0x70,0x61,0x63,0x6B,0x65,0x74,0x20,0x65,0x6E,0x64,0x3D,
    0x22,0x72,0x22,0x3F,0x3E,0x63,0xF9,0x49,0x84,0x00,0x00,0x06,0xDB,0x49,0x44,0x41,0x54,0x78,0xDA,0xCC,0x59,0x6B,0x48,0x55,0x59,
    0x14,0x5E,0x6A,0x99,0x8F,0x29,0x0B,0xD2,0x99,0x32,0xA5,0x32,0xCB,0x7C,0x90,0x54,0x66,0xFE,0x10,0x2A,0xE8,0x49,0x30,0x44,0x3F,
    0x66,0x90,0x8A,0x8A,0x70,0x46,0x86,0x99,0x66,0x86,0xE6,0x47,0xCC,0x8F,0x20,0x18,0x26,0x30,0xE6,0x47,0x60,0xA3,0xF4,0x18,0x24,
    0x2B,0x08,0x86,0x41,0x2A,0xA2,0xE8,0x31,0xF3,0x23,0x1B,0x91,0x62,0xAE,0xAF,0x7C,0x82,0x99,0x66,0xA3,0xF4,0xD4,0xF2,0x71,0xD5,
    0x35,0xEB,0xDB,0x73,0xCE,0xE5,0x78,0xEE,0xB9,0xE7,0xDC,0x87,0x3A,0xB3,0xE1,0xF3,0x5E,0xEE,0xD9,0x67,0xAD,0x6F,0x6F,0xD7,0x5E,
    0xAF,0x1D,0xF6,0xF6,0xED,0x5B,0x0A,0x71,0xC4,0x08,0xD2,0x05,0xD9,0x82,0x4C,0x41,0xAA,0x20,0x57,0x7B,0x56,0x2D,0x68,0x15,0xD4,
    0x0B,0xFE,0x12,0x34,0x0A,0xDE,0x87,0xA2,0x2C,0x2C,0x48,0xC2,0x91,0x82,0xCF,0x04,0xDF,0x09,0x3E,0x84,0x1C,0x13,0xC2,0xB5,0x79,
    0xE3,0x02,0x36,0xE1,0x6F,0x41,0xB1,0xA0,0x4C,0x30,0x32,0x1D,0x84,0x4B,0x04,0x7B,0x04,0x51,0x20,0x1E,0x1E,0x1E,0x4E,0x3A,0xC2,
    0xC2,0xC2,0xD4,0x04,0x7C,0x57,0x6C,0xC7,0xC7,0xD5,0x27,0x33,0xAB,0xEF,0x3A,0x34,0xA2,0x43,0x82,0x0A,0xC1,0x17,0x53,0x45,0xF8,
    0x7B,0xC1,0xE7,0xD8,0x51,0x21,0x36,0x53,0x06,0x01,0x8F,0x1F,0x3F,0xA6,0xBB,0x77,0xEF,0x52,0x43,0x43,0x03,0x3D,0x7D,0xFA,0x94,
    0xFA,0xFA,0xFA,0xD4,0x27,0x46,0x52,0x52,0x12,0xC5,0xC7,0xC7,0xAB,0xCF,0x8C,0x8C,0x0C,0xDA,0xB4,0x69,0x13,0xAD,0x5C,0xB9,0x92,
    0xDC,0x6E,0xB7,0x82,0x2C,0xC4,0xAD,0xED,0x78,0xA9,0xE0,0x87,0xC9,0x22,0xFC,0x91,0xA0,0x52,0x90,0x25,0x88,0x8E,0x89,0x89,0xA1,
    0xC1,0xC1,0x41,0xBA,0x74,0xE9,0x12,0x9D,0x3F,0x7F,0x9E,0x9A,0x9B,0x9B,0x69,0x64,0x64,0x84,0x46,0x47,0x47,0xCD,0xBB,0x48,0xC6,
    0xDD,0x9F,0x31,0x63,0x06,0x45,0x46,0x46,0xD2,0x8A,0x15,0x2B,0xE8,0xE0,0xC1,0x83,0x54,0x50,0x50,0x40,0xD1,0xD1,0xD1,0xF4,0xFE,
    0xBD,0x32,0xE9,0x41,0x41,0x9D,0xE0,0x63,0xC1,0x73,0x5B,0x36,0x20,0x6C,0x83,0x4F,0x04,0xCD,0x6F,0xDE,0xBC,0x19,0x1F,0x1A,0x1A,
    0x62,0x8C,0x8B,0x17,0x2F,0xF2,0x9A,0x35,0x6B,0x78,0xF6,0xEC,0xD9,0x2C,0x44,0xD8,0xC2,0x46,0x6D,0x81,0x77,0xF0,0x2E,0x64,0x40,
    0x16,0x06,0x64,0x43,0x07,0x74,0x69,0x3A,0x7D,0x72,0xB2,0x23,0xFB,0x8D,0xA0,0x4B,0x04,0x29,0xA1,0xF5,0xF5,0xF5,0xBC,0x6D,0xDB,
    0x36,0x9E,0x37,0x6F,0x1E,0x8B,0x49,0x04,0x4C,0xD4,0x0C,0xC8,0x80,0x2C,0xC8,0x84,0x6C,0x0C,0xE8,0x82,0x4E,0xC1,0xD7,0x81,0x12,
    0xCE,0x13,0x74,0xE8,0x64,0xAF,0x5C,0xB9,0xC2,0x99,0x99,0x99,0x1C,0x11,0x11,0x11,0x32,0x51,0x33,0x20,0x13,0xB2,0xA1,0xC3,0x44,
    0xFA,0x53,0x7F,0x09,0xC7,0x09,0x5C,0xF8,0x17,0x41,0xC0,0xD9,0xB3,0x67,0x79,0xE9,0xD2,0xA5,0x93,0x4E,0xD4,0x0C,0xE8,0x80,0x2E,
    0x03,0xE9,0x26,0x41,0xA2,0x3F,0x84,0xAB,0xF4,0x9D,0xBD,0x73,0xE7,0x0E,0xA7,0xA6,0xA6,0x4E,0x39,0x59,0x23,0x69,0xD3,0x4E,0xFF,
    0xE9,0x44,0xF8,0x47,0xC1,0xB8,0x78,0x01,0x6E,0x6B,0x6B,0xE3,0x75,0xEB,0xD6,0x4D,0x8A,0xBD,0x06,0x82,0xAC,0xAC,0x2C,0xAE,0xAB,
    0xAB,0x53,0x07,0x51,0xB8,0x8C,0x69,0x9C,0x2C,0x09,0xCF,0x12,0x3C,0xEF,0xEF,0xEF,0x67,0x71,0x53,0xBC,0x63,0xC7,0x8E,0x69,0x27,
    0xAB,0x1F,0xC6,0xAD,0x5B,0xB7,0x2A,0x0E,0xE0,0x02,0x4E,0x1A,0x37,0x2F,0xC2,0xE5,0xD8,0x5D,0x19,0x5C,0x52,0x52,0xC2,0xB1,0xB1,
    0xB1,0xB6,0x82,0xF1,0x1C,0x27,0x7C,0xC3,0x86,0x0D,0x1C,0x15,0x15,0xE5,0x48,0x24,0x2D,0x2D,0x8D,0x77,0xED,0xDA,0xC5,0x9B,0x37,
    0x6F,0xF6,0x4B,0x36,0x38,0x80,0x0B,0x38,0x69,0xDC,0x26,0x10,0xC6,0x0A,0xFA,0xB1,0xA2,0xD7,0xAF,0x5F,0xAB,0x53,0x6B,0xB7,0xBB,
    0xC9,0xC9,0xC9,0x5C,0x5E,0x5E,0xCE,0xED,0xED,0xED,0xDC,0xD2,0xD2,0xC2,0xF7,0xEF,0xDF,0x57,0xE4,0x7D,0xCD,0x2F,0x2A,0x2A,0x62,
    0x97,0xCB,0xC5,0x4F,0x9E,0x3C,0x51,0xA6,0x76,0xEE,0xDC,0x39,0x96,0x08,0x68,0xBB,0xCB,0xE0,0x00,0x2E,0xDA,0x2E,0xF7,0xEB,0xBB,
    0xAC,0x13,0xFE,0x52,0xE0,0x86,0xDD,0x94,0x96,0x96,0xB2,0x44,0x20,0xDB,0x1D,0x38,0x7E,0xFC,0xB8,0x5A,0xBD,0x71,0xB4,0xB6,0xB6,
    0x2A,0x33,0x32,0xCF,0xDD,0xB7,0x6F,0x1F,0xF7,0xF4,0xF4,0x4C,0x98,0x8B,0x33,0x72,0xE4,0xC8,0x11,0x5B,0x1D,0xE0,0x00,0x2E,0x9A,
    0x2D,0xBB,0x35,0x8E,0x1E,0xC2,0x9D,0xD8,0xFA,0xB1,0xB1,0x31,0xCE,0xCD,0xCD,0x75,0x8C,0x60,0x07,0x0E,0x1C,0x50,0xAB,0x37,0x8F,
    0xEA,0xEA,0x6A,0x4E,0x49,0x49,0xF1,0xCC,0xC3,0x02,0x3A,0x3B,0x3B,0xBD,0xE6,0xBD,0x7C,0xF9,0x92,0x77,0xEF,0xDE,0xED,0x18,0x11,
    0xC1,0x05,0x9C,0x34,0xB3,0xE8,0xD4,0x09,0xC7,0x0A,0x78,0x60,0x60,0x40,0x45,0x9C,0xB9,0x73,0xE7,0x3A,0xDA,0xE3,0x9C,0x39,0x73,
    0xD4,0xEA,0xCD,0x03,0xBB,0x7E,0xED,0xDA,0x35,0x5E,0xB0,0x60,0x01,0xA7,0xA7,0xA7,0x73,0x53,0x53,0x93,0xD7,0x9C,0x77,0xEF,0xDE,
    0xF1,0xD1,0xA3,0x47,0x59,0x72,0x12,0x47,0x3D,0xE0,0x02,0x4E,0xE0,0x06,0x8E,0xE0,0x0A,0xC2,0x39,0x82,0x11,0x49,0x42,0xF8,0xD4,
    0xA9,0x53,0x8E,0xE6,0xA0,0x63,0xF1,0xE2,0xC5,0x1E,0x9F,0x69,0x1E,0x65,0x65,0x65,0x7C,0xE1,0xC2,0x05,0xAF,0xDF,0xA1,0xE3,0xC4,
    0x89,0x13,0x1C,0x17,0x17,0xE7,0x97,0x0E,0x70,0x01,0x27,0xBC,0x07,0x8E,0xE0,0xAA,0xDB,0xEF,0x28,0x6C,0x65,0xFF,0xFE,0xFD,0x2C,
    0x19,0x95,0xDF,0x2E,0x08,0x41,0xA5,0xA6,0xA6,0xC6,0x8B,0x18,0x64,0x41,0x89,0x79,0x60,0x11,0x76,0x87,0xCD,0x0C,0x70,0x01,0x27,
    0xCD,0x8E,0x47,0xC1,0x15,0x84,0x7F,0x85,0x83,0x1E,0x1E,0x1E,0xE6,0xBC,0xBC,0xBC,0x80,0x33,0xB0,0xFC,0xFC,0x7C,0x4F,0xF2,0x62,
    0x37,0x6E,0xDF,0xBE,0xCD,0xCB,0x97,0x2F,0x0F,0x38,0xB3,0x03,0x27,0x70,0xD3,0x82,0xC8,0xAF,0xA4,0xC5,0xEC,0x71,0x38,0xEA,0x60,
    0x73,0x86,0x9D,0x3B,0x77,0xB2,0xE4,0xC5,0x3E,0xC9,0x56,0x55,0x55,0xA9,0x74,0x32,0xD8,0x70,0x0D,0x6E,0xDA,0xC1,0x6B,0x42,0x2D,
    0x93,0x8C,0x44,0x1E,0xE5,0x8D,0xC4,0xEF,0xA0,0x0A,0x43,0x39,0x68,0x54,0x5B,0x5B,0xEB,0xF3,0xB9,0xEC,0x2E,0x3D,0x7C,0xF8,0x30,
    0x28,0xD9,0xF0,0x0C,0x5A,0xE9,0x85,0x3F,0x49,0xE1,0x34,0x09,0x63,0xEF,0xDE,0xBD,0x94,0x9D,0x9D,0xED,0xF3,0xB9,0x04,0x15,0x92,
    0x08,0x17,0x94,0x6C,0xBD,0x7A,0xD1,0xC6,0xCC,0x90,0x4D,0x62,0xFB,0xF6,0xED,0xFC,0xEC,0xD9,0x33,0x47,0x1B,0x86,0x9D,0xAF,0x5A,
    0xB5,0x2A,0x60,0xF9,0xF0,0x46,0x9A,0x49,0x00,0x6D,0x21,0x1D,0x3A,0x38,0x76,0x2B,0x5F,0x8B,0xA0,0xF2,0xE2,0xC5,0x0B,0xAF,0xDF,
    0x6F,0xDC,0xB8,0xA1,0x72,0x8A,0x40,0x0E,0xDD,0xFA,0xF5,0xEB,0x75,0xC2,0x38,0x74,0xBF,0xC1,0x24,0x7E,0xFF,0xB7,0x12,0x67,0x55,
    0x20,0xA2,0x58,0xF4,0x67,0x60,0x6E,0x71,0x71,0xB1,0xFA,0x34,0x0E,0x09,0x0C,0x74,0xF2,0xE4,0x49,0x55,0xA0,0x5A,0x99,0xC6,0xB1,
    0x63,0xC7,0x28,0x21,0x21,0xC1,0x2F,0x1D,0xE0,0x22,0x9E,0x45,0x37,0x0B,0x2C,0xE2,0x0F,0xAF,0xC0,0xE1,0x4F,0x04,0x42,0x14,0xAB,
    0xAC,0xAC,0xB4,0xFC,0xD7,0x9F,0x39,0x73,0x46,0xF9,0x4F,0x29,0xED,0xF9,0xE6,0xCD,0x9B,0x5E,0xCF,0x91,0x47,0xC8,0x42,0x79,0xFE,
    0xFC,0xF9,0x8E,0x7A,0xC0,0xC5,0x2A,0x70,0x4C,0x08,0xCD,0xFE,0x44,0xA1,0x8A,0x8A,0x0A,0x4B,0xB2,0xD7,0xAF,0x5F,0xE7,0x65,0xCB,
    0x96,0x79,0xE6,0xC9,0x41,0xE4,0xC6,0xC6,0x46,0xCB,0xC0,0x72,0xF8,0xF0,0x61,0x47,0x3D,0xE0,0x62,0x15,0x9A,0x3D,0xC9,0x0F,0x72,
    0x81,0xB5,0x6B,0xD7,0x3A,0xDA,0xB1,0xD5,0xCE,0x3D,0x7A,0xF4,0xC8,0x32,0x30,0x20,0x19,0xEF,0xE8,0xE8,0xF0,0x9A,0x2F,0x26,0xE3,
    0x68,0xBF,0xE0,0x62,0xC8,0x89,0x3B,0x8D,0xD9,0xDA,0x57,0x48,0xE1,0x70,0xF0,0x4E,0x9F,0x3E,0xED,0x98,0x90,0x17,0x16,0x16,0x72,
    0x77,0x77,0xB7,0x3A,0x0C,0xAF,0x5E,0xBD,0x62,0xF1,0xB1,0x8A,0x98,0xAF,0xF9,0x05,0x05,0x05,0x6A,0xA7,0x90,0xDB,0x62,0x77,0x11,
    0x64,0xF0,0x9B,0x9D,0x0E,0x70,0x00,0x17,0x2D,0xCA,0xB9,0x35,0x8E,0x9E,0xCE,0x4F,0xB4,0xA0,0x57,0x1C,0xF4,0x07,0x92,0xCE,0x91,
    0x78,0x0B,0x92,0xD3,0xAF,0x7A,0x62,0x56,0x23,0x31,0x31,0x91,0xA4,0x7A,0x20,0x71,0x39,0x24,0xB9,0x2E,0x5D,0xBD,0x7A,0x95,0x24,
    0x91,0xB7,0x3D,0x40,0x39,0x39,0x39,0x24,0x8B,0x22,0xA9,0x26,0x48,0xCC,0x84,0xC4,0x7C,0x48,0xD2,0x4C,0xEB,0x76,0x94,0x04,0x0A,
    0xF1,0x26,0xF4,0xE0,0xC1,0x03,0x8A,0x88,0x88,0x00,0x8F,0x01,0xF9,0x39,0x41,0x75,0x88,0x82,0x2D,0x91,0xA6,0x12,0xFE,0x94,0x48,
    0xFF,0xAB,0x22,0x74,0xCB,0x96,0x2D,0x3E,0x8B,0x50,0x63,0x68,0x1E,0x16,0xFC,0x82,0x03,0x01,0xB3,0x10,0x77,0x42,0xAB,0x57,0xAF,
    0xF6,0xB4,0x50,0xA7,0x6B,0x88,0xCB,0x54,0xFE,0x1D,0xBE,0x17,0x35,0x01,0x38,0x69,0xDC,0x7C,0x36,0x03,0x3D,0x8D,0x94,0x5B,0xB7,
    0x6E,0x05,0x9C,0x12,0x86,0x82,0x25,0x4B,0x96,0xF0,0xE5,0xCB,0x97,0x8D,0x8D,0x94,0x6A,0x7F,0x5B,0x55,0xB5,0x7A,0xAB,0x0A,0x81,
    0x00,0x82,0xA6,0x83,0xAC,0xA9,0x55,0xD5,0x22,0x48,0xF6,0xB7,0x19,0x98,0x6F,0x6C,0x06,0x62,0xD5,0x53,0xDD,0x0C,0x34,0xED,0x6C,
    0xB7,0x60,0x4F,0x30,0xED,0xD6,0x1E,0x73,0xBB,0x15,0x85,0xE1,0x64,0xB5,0x5B,0x21,0xCB,0xA2,0xDD,0xDA,0x13,0x4C,0xBB,0x55,0x47,
    0xA1,0xA0,0xD5,0xD8,0xD0,0x46,0x5D,0x26,0x87,0x51,0x35,0xA5,0x83,0x21,0x8E,0x77,0xF0,0x2E,0x64,0xE8,0x85,0xAA,0xA1,0xA1,0xDD,
    0xAA,0xE9,0xF4,0xC9,0x29,0xA8,0x2B,0x03,0x51,0x40,0xA2,0x8C,0xC4,0xE6,0xA8,0xBD,0xBD,0x7D,0xC2,0x95,0x01,0x3C,0x8C,0x1E,0x70,
    0xE0,0x61,0xE0,0xF8,0x8D,0x57,0x06,0x29,0x29,0x29,0x74,0xE8,0xD0,0x21,0x95,0xF4,0x4B,0x34,0x0B,0xF8,0xCA,0x20,0xD0,0x4B,0x99,
    0x22,0x44,0x1C,0xFD,0x52,0x06,0x24,0x10,0xB5,0xEE,0xDD,0xBB,0x47,0x2E,0x97,0x8B,0x24,0x91,0xA7,0xDE,0xDE,0x5E,0xEA,0xEA,0xEA,
    0x52,0x2F,0x2C,0x5A,0xB4,0x48,0xA5,0x92,0x0B,0x17,0x2E,0x24,0x49,0xDE,0x69,0xE3,0xC6,0x8D,0xCA,0x6D,0x61,0x71,0x86,0x4B,0x99,
    0x5E,0xC1,0xCF,0x93,0x79,0x29,0x13,0xD0,0xB5,0x97,0x0E,0xFD,0xBA,0x4B,0xC7,0x7F,0x71,0xED,0x65,0x1C,0xB3,0x34,0x45,0xDF,0x0A,
    0xE2,0x03,0xBC,0x58,0xEC,0x13,0xFC,0xA4,0x2D,0x7C,0x78,0xBA,0x6E,0x42,0x8D,0x23,0x56,0x90,0x81,0xFC,0x46,0x90,0x86,0x62,0xC4,
    0x74,0x75,0xDB,0x2C,0x68,0x12,0xD4,0x08,0x1A,0x50,0x94,0x84,0xA2,0xEC,0x1F,0x01,0x06,0x00,0x21,0x66,0x94,0xE1,0xE6,0x84,0xB7,
    0x60,0x00,0x00,0x00,0x00,0x49,0x45,0x4E,0x44,0xAE,0x42,0x60,0x82};

@implementation iSmartNewsUAModalPanel

@synthesize roundedRect, closeButton, actionButton, delegate, contentView, contentContainer;
@synthesize margin, padding, cornerRadius, borderWidth, borderColor, contentColor, shouldBounce;
@synthesize onClosePressed, onActionPressed;


- (id)initWithFrame:(CGRect)frame {
	self = [super initWithFrame:frame];
	if (self != nil) {
		delegate = nil;
		roundedRect = nil;
		closeButton = nil;
		actionButton = nil;
		contentView = nil;
		startEndPoint = CGPointZero;
		
		margin = UIEdgeInsetsMake(DEFAULT_MARGIN, DEFAULT_MARGIN, DEFAULT_MARGIN, DEFAULT_MARGIN);
		padding = UIEdgeInsetsMake(DEFAULT_MARGIN, DEFAULT_MARGIN, DEFAULT_MARGIN, DEFAULT_MARGIN);
		cornerRadius = DEFAULT_CORNER_RADIUS;
		borderWidth = DEFAULT_BORDER_WIDTH;
		borderColor =
#if !__has_feature(objc_arc)
        [
#endif
         DEFAULT_BORDER_COLOR
#if !__has_feature(objc_arc)
         retain]
#endif
        ;
		contentColor =
#if !__has_feature(objc_arc)
        [
#endif
         DEFAULT_BACKGROUND_COLOR
#if !__has_feature(objc_arc)
         retain]
#endif
        ;
        
		shouldBounce = DEFAULT_BOUNCE;
		
		self.autoresizingMask = UIViewAutoresizingFlexibleWidth|UIViewAutoresizingFlexibleHeight;
		self.autoresizesSubviews = YES;
		
		self.contentContainer =
#if !__has_feature(objc_arc)
        [
#endif
         [[UIView alloc] initWithFrame:self.bounds]
#if !__has_feature(objc_arc)
         autorelease]
#endif
        ;
		self.contentContainer.autoresizingMask = UIViewAutoresizingFlexibleWidth|UIViewAutoresizingFlexibleHeight;
		self.contentContainer.autoresizesSubviews = YES;
		[self addSubview:self.contentContainer];
		
		[self setBackgroundColor:[UIColor colorWithWhite:0.0 alpha:0.5]]; // Fixed value, the bacground mask.
        
		[self.contentView setBackgroundColor:[UIColor clearColor]];
		self.delegate = nil;
        
		self.tag = (arc4random() % 32768);
		
	}
	return self;
}

- (void)dealloc {
	self.roundedRect = nil;
	self.closeButton = nil;
	self.actionButton = nil;
	self.contentContainer = nil;
	self.borderColor = nil;
	self.contentColor = nil;
	self.onActionPressed = nil;
	self.onClosePressed = nil;
	self.delegate = nil;
#if !__has_feature(objc_arc)
	[super dealloc];
#endif
}

#pragma mark - Description

- (NSString *)description {
	return [NSString stringWithFormat:@"<%@ %d>", [[self class] description], self.tag];
}

#pragma mark - Accessors

- (void)setCornerRadius:(CGFloat)newRadius {
	cornerRadius = newRadius;
	self.roundedRect.layer.cornerRadius = cornerRadius;
}
- (void)setBorderWidth:(CGFloat)newWidth {
	borderWidth = newWidth;
	self.roundedRect.layer.borderWidth = borderWidth;
}
- (void)setBorderColor:(UIColor *)newColor {
#if !__has_feature(objc_arc)
	[newColor retain];
	[borderColor release];
#endif
	borderColor = newColor;
	
	self.roundedRect.layer.borderColor = [borderColor CGColor];
}
- (void)setContentColor:(UIColor *)newColor {
#if !__has_feature(objc_arc)
	[newColor retain];
	[contentColor release];
#endif
	contentColor = newColor;
	
	self.roundedRect.backgroundColor = contentColor;
}

- (UIView *)roundedRect {
	if (!roundedRect) {
		self.roundedRect =
#if !__has_feature(objc_arc)
        [
#endif
         [[UIView alloc] initWithFrame:CGRectZero]
#if !__has_feature(objc_arc)
         autorelease]
#endif
        ;
		self.roundedRect.layer.masksToBounds = YES;
		self.roundedRect.backgroundColor = self.contentColor;
		self.roundedRect.layer.borderColor = [self.borderColor CGColor];
		self.roundedRect.layer.borderWidth = self.borderWidth;
		self.roundedRect.layer.cornerRadius = self.cornerRadius;
        
		[self.contentContainer insertSubview:self.roundedRect atIndex:0];
	}
	return roundedRect;
}
- (UIButton*)closeButton {
	if (!closeButton) {
		self.closeButton = [UIButton buttonWithType:UIButtonTypeCustom];
        
        NSData* data = [NSData dataWithBytes:(([[UIScreen mainScreen] scale] > 1.f)?close2_png:close_png)
                                      length:(([[UIScreen mainScreen] scale] > 1.f)?sizeof(close2_png):sizeof(close_png))];
        
        UIImage* image1 = [UIImage imageWithData:data];
        UIImage* image = [UIImage imageWithCGImage:image1.CGImage scale:[[UIScreen mainScreen] scale] orientation:image1.imageOrientation];
        
		[self.closeButton setImage:image forState:UIControlStateNormal];
        
		[self.closeButton setFrame:CGRectMake(0, 0, 44, 44)];
		self.closeButton.layer.shadowColor = [[UIColor blackColor] CGColor];
		self.closeButton.layer.shadowOffset = CGSizeMake(0,4);
		self.closeButton.layer.shadowOpacity = 0.3;
		
		[closeButton addTarget:self action:@selector(closePressed:) forControlEvents:UIControlEventTouchUpInside];
		[self.contentContainer insertSubview:closeButton aboveSubview:self.contentView];
	}
	return closeButton;
}

- (UIButton*)actionButton {
	if (!actionButton) {
		UIImage *image = [UIImage imageNamed:@"modalButton.png"];
		UIImage *stretch = (([UIImage respondsToSelector:@selector(resizableImageWithCapInsets:)]) ?
                            [image resizableImageWithCapInsets:UIEdgeInsetsMake(0, image.size.width/2.0, 0, image.size.width/2.0)] :
                            [image stretchableImageWithLeftCapWidth:image.size.width/2.0 topCapHeight:image.size.width/2.0]);
		UIImage *image2 = [UIImage imageNamed:@"modalButton-selected.png"];
		UIImage *stretch2 = (([UIImage respondsToSelector:@selector(resizableImageWithCapInsets:)]) ?
                             [image2 resizableImageWithCapInsets:UIEdgeInsetsMake(0, image2.size.width/2.0, 0, image2.size.width/2.0)] :
                             [image stretchableImageWithLeftCapWidth:image2.size.width/2.0 topCapHeight:image2.size.width/2.0]);
		self.actionButton = [UIButton buttonWithType:UIButtonTypeCustom];
		[self.actionButton setBackgroundImage:stretch forState:UIControlStateNormal];
		[self.actionButton setBackgroundImage:stretch2 forState:UIControlStateHighlighted];
		self.actionButton.layer.shadowColor = [[UIColor blackColor] CGColor];
		self.actionButton.layer.shadowOffset = CGSizeMake(0,4);
		self.actionButton.layer.shadowOpacity = 0.3;
		self.actionButton.titleLabel.font = [UIFont boldSystemFontOfSize:11];
		[self.actionButton setTitleColor:[UIColor whiteColor] forState:UIControlStateNormal];
		[self.actionButton setTitleColor:[UIColor grayColor] forState:UIControlStateHighlighted];
		self.actionButton.contentEdgeInsets = UIEdgeInsetsMake(4, 8, 4, 8);
		
		[actionButton addTarget:self action:@selector(actionPressed:) forControlEvents:UIControlEventTouchUpInside];
		[self.contentContainer insertSubview:actionButton aboveSubview:self.contentView];
	}
	return actionButton;
}

- (UIView *)contentView {
	if (!contentView) {
		self.contentView =
#if !__has_feature(objc_arc)
        [
#endif
         [[UIView alloc] initWithFrame:CGRectZero]
#if !__has_feature(objc_arc)
         autorelease]
#endif
        ;
		self.contentView.autoresizingMask = UIViewAutoresizingFlexibleWidth|UIViewAutoresizingFlexibleHeight;
		self.contentView.autoresizesSubviews = YES;
		[self.contentContainer insertSubview:contentView aboveSubview:self.roundedRect];
	}
	return contentView;
}

- (CGRect)roundedRectFrame {
    
	return CGRectMake(self.margin.left + self.frame.origin.x,
					  self.margin.top + self.frame.origin.y,
					  self.frame.size.width - self.margin.left - self.margin.right,
					  self.frame.size.height - self.margin.top - self.margin.bottom);
}

- (CGRect)closeButtonFrame {
	CGRect f = [self roundedRectFrame];
	return CGRectMake(f.origin.x,// - floor(closeButton.frame.size.width*0.5),
					  f.origin.y,// - floor(closeButton.frame.size.height*0.5),
					  closeButton.frame.size.width,
					  closeButton.frame.size.height);
}

- (CGRect)actionButtonFrame {
	if (![[self.actionButton titleForState:UIControlStateNormal] length])
		return CGRectZero;
	
	[self.actionButton sizeToFit];
	CGRect f = [self roundedRectFrame];
	return CGRectMake(f.origin.x + f.size.width - self.actionButton.frame.size.width + 11,
					  f.origin.y - floor(actionButton.frame.size.height*0.5),
					  self.actionButton.frame.size.width,
					  self.actionButton.frame.size.height);
}

- (CGRect)contentViewFrame {
	CGRect roundedRectFrame = [self roundedRectFrame];
	return CGRectMake(self.padding.left + roundedRectFrame.origin.x,
					  self.padding.top + roundedRectFrame.origin.y,
					  roundedRectFrame.size.width - self.padding.left - self.padding.right,
					  roundedRectFrame.size.height - self.padding.top - self.padding.bottom);
}


- (void)layoutSubviews {
	[super layoutSubviews];
	
	self.roundedRect.frame	= [self roundedRectFrame];
	self.closeButton.frame	= [self closeButtonFrame];
	self.actionButton.frame	= [self actionButtonFrame];
	self.contentView.frame	= [self contentViewFrame];
}

- (void)closePressed:(id)sender {
	
	// Using Delegates
	if ([delegate respondsToSelector:@selector(shouldCloseModalPanel:)]) {
		if ([delegate shouldCloseModalPanel:self]) {
			[self hide];
		}
		
        
        // Using blocks
	} else if (self.onClosePressed) {
		self.onClosePressed(self);
		
        // No delegate or blocks. Just close myself
	} else {
		[self hide];
	}
}

- (void)actionPressed:(id)sender {
	
	// Using Delegates
	if ([delegate respondsToSelector:@selector(didSelectActionButton:)]) {
		[delegate didSelectActionButton:self];
		
		
		// Using blocks
	} else if (self.onActionPressed) {
		self.onActionPressed(self);
		
		// No delegate or blocks. Do nothing!
	} else {
		// no-op
	}
}


- (void)showAnimationStarting {};		//subclasses override
- (void)showAnimationPart1Finished {};	//subclasses override
- (void)showAnimationPart2Finished {};	//subclasses override
- (void)showAnimationPart3Finished {};	//subclasses override
- (void)showAnimationFinished {};		//subclasses override
- (void)show {
	
	if ([delegate respondsToSelector:@selector(willShowModalPanel:)])
		[delegate willShowModalPanel:self];
	
	[self showAnimationStarting];
	self.alpha = 0.0;
	self.contentContainer.transform = CGAffineTransformMakeScale(0.00001, 0.00001);
	
	
	void (^animationBlock)(BOOL) = ^(BOOL finished) {
		[self showAnimationPart1Finished];
		// Wait one second and then fade in the view
		[UIView animateWithDuration:0.1
						 animations:^{
							 self.contentContainer.transform = CGAffineTransformMakeScale(0.95, 0.95);
						 }
						 completion:^(BOOL finished){
							 
							 [self showAnimationPart2Finished];
							 // Wait one second and then fade in the view
							 [UIView animateWithDuration:0.1
											  animations:^{
												  self.contentContainer.transform = CGAffineTransformMakeScale(1.02, 1.02);
											  }
											  completion:^(BOOL finished){
												  
												  [self showAnimationPart3Finished];
												  // Wait one second and then fade in the view
												  [UIView animateWithDuration:0.1
																   animations:^{
																	   self.contentContainer.transform = CGAffineTransformIdentity;
																   }
																   completion:^(BOOL finished){
																	   [self showAnimationFinished];
																	   if ([delegate respondsToSelector:@selector(didShowModalPanel:)])
																		   [delegate didShowModalPanel:self];
																   }];
											  }];
						 }];
	};
	
	// Show the view right away
    [UIView animateWithDuration:0.3
						  delay:0.0
						options:UIViewAnimationOptionCurveEaseOut
					 animations:^{
						 self.alpha = 1.0;
						 self.contentContainer.center = self.center;
						 self.contentContainer.transform = CGAffineTransformMakeScale((shouldBounce ? 1.05 : 1.0), (shouldBounce ? 1.05 : 1.0));
					 }
					 completion:(shouldBounce ? animationBlock : ^(BOOL finished) {
        [self showAnimationFinished];
        if ([delegate respondsToSelector:@selector(didShowModalPanel:)])
            [delegate didShowModalPanel:self];
    })];
    
}
- (void)showFromPoint:(CGPoint)point {
	startEndPoint = point;
	self.contentContainer.center = point;
	[self show];
}

- (void)hide {
	// Hide the view right away
	if ([delegate respondsToSelector:@selector(willCloseModalPanel:)])
		[delegate willCloseModalPanel:self];
	
    [UIView animateWithDuration:0.3
					 animations:^{
						 self.alpha = 0;
						 if (startEndPoint.x != CGPointZero.x || startEndPoint.y != CGPointZero.y) {
							 self.contentContainer.center = startEndPoint;
						 }
						 self.contentContainer.transform = CGAffineTransformMakeScale(0.0001, 0.0001);
					 }
					 completion:^(BOOL finished){
						 if ([delegate respondsToSelector:@selector(didCloseModalPanel:)]) {
							 [delegate didCloseModalPanel:self];
						 }
						 [self removeFromSuperview];
					 }];
}


- (void)hideWithOnComplete:(iSmartNewsUAModalDisplayPanelAnimationComplete)onComplete {
	// Hide the view right away
    [UIView animateWithDuration:0.3
					 animations:^{
						 self.alpha = 0;
						 if (startEndPoint.x != CGPointZero.x || startEndPoint.y != CGPointZero.y) {
							 self.contentContainer.center = startEndPoint;
						 }
						 self.contentContainer.transform = CGAffineTransformMakeScale(0.0001, 0.0001);
					 }
					 completion:^(BOOL finished){
						 if (onComplete)
                             onComplete(finished);
					 }];
}

@end





@implementation iSmartNewsUARoundedRectView

@synthesize radius;

+ (Class)layerClass {
	return [CAGradientLayer class];
}
- (id)initWithFrame:(CGRect)frame {
    if ((self = [super initWithFrame:frame])) {
		self.backgroundColor = [UIColor clearColor];
		
		colorComponents = NSZoneMalloc(NSDefaultMallocZone(), 8*sizeof(CGFloat));
		for (int i = 0; i < 8; i++) {
			colorComponents[i] = 1.0;
		}
    }
    return self;
}

- (void)setColors:(CGFloat *)components {
	for (int i = 0; i < 8; i++) {
		colorComponents[i] = components[i];
	}
	
	CAGradientLayer *gradientLayer = (CAGradientLayer *)self.layer;
	gradientLayer.colors =
	[NSArray arrayWithObjects:
	 (id)[UIColor colorWithRed:colorComponents[0] green:colorComponents[1] blue:colorComponents[2] alpha:colorComponents[3]].CGColor,
	 (id)[UIColor colorWithRed:colorComponents[4] green:colorComponents[5] blue:colorComponents[6] alpha:colorComponents[7]].CGColor,
	 nil];
	
}

- (void)setRadius:(NSInteger)rad {
	radius = rad;
	CAGradientLayer *gradientLayer = (CAGradientLayer *)self.layer;
	gradientLayer.cornerRadius = rad*1.0f;
}

- (void)dealloc {
	NSZoneFree(NSDefaultMallocZone(), colorComponents);
#if !__has_feature(objc_arc)
    [super dealloc];
#endif
}


@end





static CanIShowAlertViewRightNowHandler gCanIShowAlertViewRightNow = nil;

/*! @internal */
static NSObject* getMessageKey(NSDictionary* _dict, NSString* _key)
{
    NSArray*  preferredLanguages   = [NSLocale preferredLanguages];
    NSString* lang    = [preferredLanguages count] ? [preferredLanguages objectAtIndex:0] : [[NSLocale currentLocale] objectForKey:NSLocaleLanguageCode];
    NSString* country = [[NSLocale currentLocale] objectForKey:NSLocaleCountryCode];
    NSObject* retVal = nil;
    
    lang = [lang stringByReplacingOccurrencesOfString:@"-" withString:@"_"];
    
    DBGLOG(@"lang = %@, country = %@",lang,country);
    
    const CGFloat scale = [[UIScreen mainScreen] respondsToSelector:@selector(scale)] ? [[UIScreen mainScreen] scale] : 1.0f;
    NSString* screenSize = [NSString stringWithFormat:@"%dx%d",
                            (int)(scale * [[UIScreen mainScreen] bounds].size.width),
                            (int)([[UIScreen mainScreen] bounds].size.height * scale)
                            ];
    
    // try to get variant for full locale + size
    if (!retVal)
        retVal = [_dict objectForKey:[_key stringByAppendingFormat:@"_%@_%@_%@",lang,country,screenSize]];
    
    // try to get variant for full locale
    if (!retVal)
        retVal = [_dict objectForKey:[_key stringByAppendingFormat:@"_%@_%@",lang,country]];
    
    // try to get variant for language + size
    if (!retVal)
        retVal = [_dict objectForKey:[_key stringByAppendingFormat:@"_%@_%@",lang,screenSize]];
    
    // try to get variant for language
    if (!retVal)
        retVal = [_dict objectForKey:[_key stringByAppendingFormat:@"_%@",lang]];
    
    // try to get variant for size
    if (!retVal)
        retVal = [_dict objectForKey:[_key stringByAppendingFormat:@"_%@",screenSize]];
    
    // peek default
    if (!retVal)
        retVal = [_dict objectForKey:_key];
    
    return retVal;
}

/*! @internal */
static NSString* md5ForArray(NSArray* _array)
{
    CC_MD5_CTX  ctx;
    CC_MD5_Init(&ctx);
    
    for (NSString* value in _array)
    {
        const void* data = 0;
        int         len  = 0;
        
        if ([value isKindOfClass:[NSString class]])
        {
            data = [(NSString*)value UTF8String];
            len  = (int)strlen((const  char*)data);
        }
        else if ([value isKindOfClass:[NSDate class]])
        {
            NSDateFormatter *dateFormatter = [[NSDateFormatter alloc] init];
            [dateFormatter setTimeStyle:NSDateFormatterFullStyle];
            [dateFormatter setDateStyle:NSDateFormatterFullStyle];
            NSString* date = [dateFormatter stringFromDate:(NSDate*)value];
#if !USES_ARC
            [dateFormatter release];
#endif
            data = [date UTF8String];
            len  = (int)strlen((const  char*)data);
        }
        else if ([value isKindOfClass:[NSNumber class]])
        {
            NSString* s = [(NSNumber*)value stringValue];
            data = [s UTF8String];
            len  = (int)strlen((const  char*)data);
        }
        
        CC_MD5_Update(&ctx,data,len);
    }
    
    unsigned char digest[CC_MD5_DIGEST_LENGTH];
    CC_MD5_Final(digest, &ctx);
    
    return [NSString stringWithFormat: @"%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x",
            digest[0],  digest[1],
            digest[2],  digest[3],
            digest[4],  digest[5],
            digest[6],  digest[7],
            digest[8],  digest[9],
            digest[10], digest[11],
            digest[12], digest[13],
            digest[14], digest[15]];
}

/*! @internal */
static NSString* md5ForDictionary(NSDictionary* _dict)
{
    CC_MD5_CTX  ctx;
    CC_MD5_Init(&ctx);
    
    for (NSString* k in [_dict allKeys])
    {
        {
            const void* data = [k UTF8String];
            int         len  = (int)strlen((const  char*)data);
            CC_MD5_Update(&ctx,data,len);
        }
        
        {
            const void* data = 0;
            int         len  = 0;
            
            NSObject* value = [_dict objectForKey:k];
            
            if ([value isKindOfClass:[NSString class]])
            {
                data = [(NSString*)value UTF8String];
                len  = (int)strlen((const  char*)data);
            }
            else if ([value isKindOfClass:[NSDate class]])
            {
                NSDateFormatter *dateFormatter = [[NSDateFormatter alloc] init];
                [dateFormatter setTimeStyle:NSDateFormatterFullStyle];
                [dateFormatter setDateStyle:NSDateFormatterFullStyle];
                NSString* date = [dateFormatter stringFromDate:(NSDate*)value];
#if !USES_ARC
                [dateFormatter release];
#endif
                data = [date UTF8String];
                len  = (int)strlen((const  char*)data);
            }
            else if ([value isKindOfClass:[NSNumber class]])
            {
                NSString* s = [(NSNumber*)value stringValue];
                data = [s UTF8String];
                len  = (int)strlen((const  char*)data);
            }
            
            CC_MD5_Update(&ctx,data,len);
        }
    }
    
    unsigned char digest[CC_MD5_DIGEST_LENGTH];
    CC_MD5_Final(digest, &ctx);
    
    return [NSString stringWithFormat: @"%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x",
            digest[0],  digest[1],
            digest[2],  digest[3],
            digest[4],  digest[5],
            digest[6],  digest[7],
            digest[8],  digest[9],
            digest[10], digest[11],
            digest[12], digest[13],
            digest[14], digest[15]];
}

NSString*    iSmartNewsMessageTitleKey = @"iSmartNewsMessageTitleKey";          //  NSString, message title
NSString*    iSmartNewsMessageTextKey = @"iSmartNewsMessageTextKey";            //  NSString, message text
NSString*    iSmartNewsMessageCancelKey = @"iSmartNewsMessageCancelKey";        //  NSString, title for 'cancel' button
NSString*    iSmartNewsMessageActionKey = @"iSmartNewsMessageActionKey";        //  NSString, title for 'ok' button
NSString*    iSmartNewsMessageUrlKey = @"iSmartNewsMessageUrlKey";              //  NSString, url to open if 'ok' was pressed
NSString*    iSmartNewsMessageStartDateKey = @"iSmartNewsMessageStartDateKey";  //  NSDate
NSString*    iSmartNewsMessageEndDateKey = @"iSmartNewsMessageEndDateKey";      //  NSDate
NSString*    iSmartNewsMessageRepeatKey = @"iSmartNewsMessageRepeatKey";        //  NSNumber (as bool)
NSString*    iSmartNewsMessageAlwaysKey = @"iSmartNewsMessageAlwaysKey";        //  NSNumber (as bool)
NSString*    iSmartNewsMessageCounterKey = @"iSmartNewsMessageCounterKey";      //  NSNumber
NSString*   iSmartNewsMessageQueueKey = @"iSmartNewsMessageQueueKey";           //  NSString, name of queue

NSString*   iSmartNewsMessageTypeKey = @"iSmartNewsMessageTypeKey";             //  NSString, type of message. "web" for web content
NSString*   iSmartNewsContentTypeWeb = @"web";


static iSmartNews*          giSmartNews = nil;
static NSMutableDictionary* giSharedSettings = nil;


@interface iSmartNewsPopupNavigationController : UINavigationController

@end

@implementation iSmartNewsPopupNavigationController


- (BOOL)shouldAutorotate{
    return UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad;
}

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)toInterfaceOrientation{
    if (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad)
        return YES;
    
    return toInterfaceOrientation == UIInterfaceOrientationPortrait;
}

- (NSUInteger)supportedInterfaceOrientations
{
    if (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad)
        return UIInterfaceOrientationMaskAll;
    
    return UIInterfaceOrientationMaskPortrait;
}

@end

@implementation UIView (iSmartNewsFindFirstResponder)
- (id)iSmartNewsFindFirstResponder_findFirstResponder
{
    if (self.isFirstResponder) {
        return self;
    }
    for (UIView *subView in self.subviews) {
        id responder = [subView iSmartNewsFindFirstResponder_findFirstResponder];
        if (responder) return responder;
    }
    return nil;
}
@end

@interface iSmartNewsPopupViewController : UIViewController
@property (nonatomic,strong) iSmartNewsUAModalPanel* panel;
@property (nonatomic,strong) UIWebView* webView;
@end

@implementation iSmartNewsPopupViewController{
    BOOL _statusBarHooked;
}

- (iSmartNewsUAModalPanel*)panel{
    if (!_panel)
        [self view];
    return _panel;
}

- (void)loadView
{
    UIView* view = [[UIView alloc] initWithFrame:[[UIScreen mainScreen] bounds]];
#if !USES_ARC
    [view autorelease];
#endif
    self.view = view;
    [self.view setBackgroundColor:[UIColor clearColor]];
}

- (void)disableBadGestureRecognizer:(UIView*)view
{
    for (UIView * v in view.subviews)
    {
        if (v.gestureRecognizers)
        {
            for (UIGestureRecognizer * gr in v.gestureRecognizers)
            {
                if ([ gr isKindOfClass:NSClassFromString(@"_UIWebHighlightLongPressGestureRecognizer") ])
                    gr.enabled = NO;
                else if ([ gr isKindOfClass:NSClassFromString(@"UILongPressGestureRecognizer") ])
                    gr.enabled = NO;
            }
        }
        [ self disableBadGestureRecognizer:v ];
    }
}

- (void)dealloc
{
    if (_statusBarHooked)
    {
        _statusBarHooked = NO;
        [[UIApplication sharedApplication] iSmartNews_hideStatusbar:NO animated:NO];
    }
    
#if !USES_ARC
    [super dealloc];
#endif
}

- (void)viewDidLoad
{
    [super viewDidLoad];
    
    if ([[[UIDevice currentDevice] systemVersion] floatValue] < 7.0)
        self.wantsFullScreenLayout = YES;
    
    _panel = [[iSmartNewsUAModalPanel alloc] initWithFrame:[self.view bounds]];
    [_panel setPadding:UIEdgeInsetsZero];
    [_panel setMargin:UIEdgeInsetsZero];
    [_panel setBorderWidth:0];
    [self.view addSubview:_panel];
    
    [self.webView setAutoresizingMask:UIViewAutoresizingFlexibleWidth|UIViewAutoresizingFlexibleHeight];
    [self.webView setBackgroundColor:[UIColor clearColor]];
    
    [self disableBadGestureRecognizer:self.webView];
    
    [_panel.contentView addSubview:self.webView];
}

- (void)viewWillDisappear:(BOOL)animated
{
    [super viewWillDisappear:animated];
    
    if (_statusBarHooked)
    {
        _statusBarHooked = NO;
        [[UIApplication sharedApplication] iSmartNews_hideStatusbar:NO animated:animated];
    }
}

- (void)viewWillAppear:(BOOL)animated
{
    [super viewWillAppear:animated];
    [self.navigationController setNavigationBarHidden:YES];
    
    if (!_statusBarHooked){
        _statusBarHooked = YES;
        [[UIApplication sharedApplication] iSmartNews_hideStatusbar:YES animated:animated];
    }
    
    for (UIWindow* window in [[UIApplication sharedApplication] windows])
        [[window iSmartNewsFindFirstResponder_findFirstResponder] resignFirstResponder];
}

- (void)viewDidAppear:(BOOL)animated
{
    [super viewDidAppear:animated];
    [_panel showFromPoint:CGPointMake(_panel.superview.bounds.size.width/2, _panel.superview.bounds.size.height/2)];
}

- (BOOL)prefersStatusBarHidden
{
    return YES;
}

@end

/*! @cond SkipThis
 ----------------------------------------------------------------------------
 */
@interface iSmartNews()

@property (nonatomic,strong) UIWindow* previousWindow;
@property (nonatomic,strong) UIWindow*                           popupWindow;
@property (nonatomic,strong) UIWebView*                          popupWebView;
@property (nonatomic,strong) iSmartNewsPopupViewController*      popupViewController;

/*! @internal */
- (void)_update;
/*! @internal */
+ (NSString*)settingsPath;
/*! @internal */
- (void)restartTimerIfNeeded;
/*! @internal */
- (void)showNextMessage;
/*! @internal */
- (void)startTimerIfNeeded;
/*! @internal */
- (void)parse;
/*! @internal */
- (BOOL)pumpUrls;
/*! @internal */
+ (NSString*)cachePath;
/*! @internal */
+ (BOOL)checkIfMessageWasAlreadyShown:(NSDictionary*)_message;
/*! @internal */
+ (void)setCacheValue:(NSDictionary*)_value;
/*! @internal */
+ (void)save;
@end
/*! ----------------------------------------------------------------------------
 @endcond 
 */

@implementation iSmartNews {
    NSString* currentQueue_;
    NSMutableDictionary* queuesInfo_;
    NSMutableDictionary* queuesTimeouts_;
    NSTimer* queueTimer_;
    NSTimer* retryTimer_;
    BOOL isFirst_;
}

@synthesize previousWindow;
@synthesize popupWindow;
@synthesize popupWebView;
@synthesize popupViewController;
@synthesize delegate = delegate_;
@synthesize updateInterval = updateInterval_;
@synthesize urls = urls_;

- (BOOL)isSharedInstance
{
    return self == giSmartNews;
}

+ (iSmartNews*)sharedNews
{
    ENSURE_MAIN_THREAD;
    
    if ( !giSmartNews ){
        iSmartNews* varToPassCompilerWarnings = [[self class] new];
        [varToPassCompilerWarnings fake];
    }
    
	return giSmartNews;
}

- (void)fake{}

+ (NSMutableDictionary*)settings
{
    if (!giSharedSettings)
    {
        giSharedSettings = [[NSMutableDictionary alloc] initWithContentsOfFile:[iSmartNews settingsPath]];
        if (!giSharedSettings)
            giSharedSettings = [[NSMutableDictionary alloc] init];
        
        DBGLOG(@"giSharedSettings: %@",giSharedSettings);                        
    }
    
    return giSharedSettings;
}

+ (void)setCanIShowAlertViewRightNowHandler:(CanIShowAlertViewRightNowHandler)CanIShowAlertViewRightNow
{
#if !USES_ARC
    if (gCanIShowAlertViewRightNow){
        Block_release(gCanIShowAlertViewRightNow);
        gCanIShowAlertViewRightNow = nil;
    }
    
    if (CanIShowAlertViewRightNow)
        gCanIShowAlertViewRightNow = Block_copy(CanIShowAlertViewRightNow);
#else
    gCanIShowAlertViewRightNow = [CanIShowAlertViewRightNow copy];
#endif
}

+ (void)resetRunCounter
{
    [[iSmartNews settings] setObject:[NSNumber numberWithUnsignedLongLong:0] forKey:@"counter"];
    [iSmartNews save];  
}

+ (void)save
{
    if (giSharedSettings)
    {
        [giSharedSettings writeToFile:[iSmartNews settingsPath] atomically:YES];
        DBGLOG(@"giSharedSettings saved");        
    }
}

/*! @brief Contructor */
- (id)init
{
	self = [super init];

	if ( self )
	{
        if (giSmartNews == nil)
            giSmartNews = self;
        
        [iSmartNews settings];// load
        
        if ([self isSharedInstance])
        {
            queuesTimeouts_ = [NSMutableDictionary new];
            [self loadQueuesInfo];
        }
        
        gate_ = UINT_MAX;
        loadedNews_ = [[NSMutableArray alloc] init];
		newsData_ = [[NSMutableArray alloc] init];
		timerInvocation_ = [NSInvocation invocationWithMethodSignature:[self methodSignatureForSelector:@selector(timerUpdate)]];
        
#if !USES_ARC
        [timerInvocation_ retain];
#endif
        
		[timerInvocation_ setTarget:self];
		updateInterval_ = 24. /* hours per day */ *
		                  60. /* minutes per hour */ *
		                  60. /* seconds per minute */;
        
        [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(UIWindowDidBecomeKeyNotification) name:UIWindowDidBecomeKeyNotification object:nil];
	}

	return self;
}

- (void)UIWindowDidBecomeKeyNotification
{
#if 0
    if (popupView_){
        [popupView_ hide];
#if !USES_ARC
        [alertView_ release];
        [popupView_ release];
#endif
        popupView_ = nil;
        alertView_ = nil;
        [self showNextMessage];
    }
#endif
    /*
     [popupView_.superview bringSubviewToFront:popupView_];
     [popupView_.contentContainer.superview bringSubviewToFront:popupView_.contentContainer];
     */
}

/*! @brief Destructor */
- (void)dealloc
{   
    [iSmartNews save];
    
    [timer_ invalidate];
    [connection_ cancel];
    
#if !USES_ARC
    [currentData_ release];
    [timerInvocation_ release];
	[urls_ release];
	[currentUrls_ release];
	[newsData_ release];
    [loadedNews_ release];
    
    [currentQueue_ release];
    [queuesTimeouts_ release];
    [queuesInfo_ release];
#endif
    
    if (self == giSmartNews)
        giSmartNews = nil;
    
#if !USES_ARC
	[super dealloc];
#endif
}

#pragma mark -
#pragma mark Cache

+ (NSString*)settingsPath
{
    return [[NSSearchPathForDirectoriesInDomains(NSLibraryDirectory, NSUserDomainMask, YES) 
             objectAtIndex:0] 
            stringByAppendingPathComponent:@"iSmartNewsCacheSettings.plist"];
}

+ (NSString*)cachePath
{
    return [[NSSearchPathForDirectoriesInDomains(NSLibraryDirectory, NSUserDomainMask, YES) 
                objectAtIndex:0] 
                    stringByAppendingPathComponent:@"iSmartNewsCache.txt"];
}

+ (void)clearCache
{
    ENSURE_MAIN_THREAD;
    
    [[NSFileManager defaultManager] removeItemAtPath:[self cachePath] error:NULL];
    
    DBGLOG(@"clearCache");        
}

+ (void)setCacheValue:(NSDictionary*)_value
{
    FILE* file = fopen([[self cachePath] UTF8String], "a");
    if (file)
    {
        const char* v = [md5ForDictionary(_value) UTF8String];
        fwrite(v, strlen(v), 1, file);
        fwrite("\n", 1, 1, file);
        fflush(file);
        fclose(file);
        DBGLOG(@"setCacheValue:%@ -> %s",_value,v);
    }
}

+ (BOOL)checkIfMessageWasAlreadyShown:(NSDictionary*)_message
{
    FILE* cache = fopen([[self cachePath] UTF8String], "r");
    if (!cache)
        return NO;
    
    rewind(cache);

    const char* key = [md5ForDictionary(_message) UTF8String];
    BOOL retVal = NO;
    
    while(!retVal)
    {
        char buf[129];
        memset(buf,0,sizeof(buf));
        if (!fgets(buf, sizeof(buf)-1, cache))
            break;
        
        size_t bytesRead = strlen(buf);
        
        for (; bytesRead && (buf[bytesRead-1]=='\n' || buf[bytesRead-1]=='\r'); --bytesRead )
            buf[bytesRead-1]=0;
        
        if (bytesRead != CC_MD5_DIGEST_LENGTH * 2)
            continue;
        
        if (!memcmp(key, buf, CC_MD5_DIGEST_LENGTH * 2))
            retVal = YES;
    }
    
    fclose(cache);
    
    DBGLOG(@"checkIfMessageWasAlreadyShown:%@ -> %d",_message,retVal);
    
    return retVal;
}

#pragma mark -
#pragma mark Logic

- (void)timerUpdate
{
    DBGLOG(@"timerUpdate");    
    
    [timer_ invalidate];
    timer_ = 0;
    [self _update];
}

- (BOOL)pumpUrls
{
    assert(!connection_);
    assert([NSThread isMainThread]);
    
    while([currentUrls_ count])
    {
        NSString* urlString = [[currentUrls_ objectAtIndex:0] copy];
        
#if !USES_ARC
        [urlString autorelease];
#endif
        
        [currentUrls_ removeObjectAtIndex:0];
        
        NSURL* url = [NSURL URLWithString:urlString];
        if (url)
        {
            NSURLRequest* request = [NSURLRequest requestWithURL:url 
                                                     cachePolicy:NSURLRequestReloadIgnoringLocalAndRemoteCacheData // NO CACHE!!!
                                                 timeoutInterval:7];
            if (request)
            {
                connection_ = [NSURLConnection connectionWithRequest:request delegate:self];

                if (connection_)
                {
                    DBGLOG(@"pumpUrls, connection created for %@",url);                                    
                    return YES;
                }
            }
        }
        
        DBGLOG(@"Failed to create NSURLConnection for %@", url);
    }

    DBGLOG(@"pumpUrls failed");
    
    return NO;
}

- (void)_update
{
    ENSURE_MAIN_THREAD;
    
    DBGLOG(@"Update called");
    
    if (!alertView_)// if nothing is shown at current moment
    {
        [timer_ invalidate];
        timer_ = nil;
        
        if ( connection_ )
        {
            [connection_ cancel];
            connection_ = nil;
            
            DBGLOG(@"Previous connection canceled");
        }
        
#if !USES_ARC
        [currentUrls_ release];
#endif
        
        currentUrls_ = [[NSMutableArray alloc] initWithArray:urls_];
        [newsData_ removeAllObjects];
        
        if (![self pumpUrls])
            [self restartTimerIfNeeded];
    }
    else
    {
        DBGLOG(@"Some alert view is already on screen, therefore update is skipped");
    }
}

- (void)clearAndHideAlert
{
    [retryTimer_ invalidate];
    retryTimer_ = nil;
    
    [queueTimer_ invalidate];
    queueTimer_ = nil;
    
#if !USES_ARC
    [currentQueue_ release];
#endif
    currentQueue_ = nil;
    
    [loadedNews_ removeAllObjects];
    [queuesTimeouts_ removeAllObjects];
    
    [self closeUI:NO];
}

- (void)clear
{
    [retryTimer_ invalidate];
    retryTimer_ = nil;
    
    [queueTimer_ invalidate];
    queueTimer_ = nil;

#if !USES_ARC
    [currentQueue_ release];
#endif
    currentQueue_ = nil;
    
    [loadedNews_ removeAllObjects];
    [queuesTimeouts_ removeAllObjects];
}

- (void)update
{
    if ([self isSharedInstance])
    {
        const UInt64 counter = [[[iSmartNews settings] objectForKey:@"counter"] unsignedLongLongValue];
        [[iSmartNews settings] setObject:[NSNumber numberWithUnsignedLongLong:(counter + 1)] forKey:@"counter"];
        [iSmartNews save];        
    }
    [self _update];
}

- (void)restartTimerIfNeeded
{
    [timer_ invalidate];
    timer_ = nil;
    [self startTimerIfNeeded];
}

- (void)startTimerIfNeeded
{
    if (start_ && !timer_)
    {
        timer_ = [NSTimer scheduledTimerWithTimeInterval:updateInterval_ target:timerInvocation_ selector:@selector(invoke) userInfo:nil repeats:NO];
        DBGLOG(@"startTimerIfNeeded: timer created for %f secs", updateInterval_);
    }
}

- (void)parse
{
    NSString* tmpFile = [NSTemporaryDirectory() stringByAppendingPathComponent:@"iSmartNewsTempFile.tmp"];
    
    [queuesTimeouts_ removeAllObjects];
    
    [retryTimer_ invalidate];
    retryTimer_ = nil;
    
    [queueTimer_ invalidate];
    queueTimer_ = nil;
    
    [loadedNews_ removeAllObjects];
    gate_ = UINT_MAX;
    
#if !USES_ARC
    [currentQueue_ release];
#endif
    
    currentQueue_ = nil;
    
    isFirst_ = YES;
    
    for (NSData* data in newsData_)
    {
#if !USES_ARC
        DBGLOG(@"parse iteration for data: %@", [[[NSString alloc] initWithBytes:[data bytes] length:[data length] encoding:NSUTF8StringEncoding] autorelease]);
#else
        DBGLOG(@"parse iteration for data: %@", [[NSString alloc] initWithBytes:[data bytes] length:[data length] encoding:NSUTF8StringEncoding]);
#endif
        
        [[NSFileManager defaultManager] removeItemAtPath:tmpFile error:0];
        [data writeToFile:tmpFile atomically:YES];
        NSArray* news = [[NSArray alloc] initWithContentsOfFile:tmpFile];
        [[NSFileManager defaultManager] removeItemAtPath:tmpFile error:0];
        
        if ([news isKindOfClass:[NSArray class]])
        {
            for (NSDictionary* desc in news)
            {
                NSMutableDictionary* message = [[NSMutableDictionary alloc] init];
                @try
                {
                    NSDate* startDate = (NSDate*)getMessageKey(desc,@"start");
                    NSDate* endDate = (NSDate*)getMessageKey(desc,@"end");
                    NSString* title = (NSString*)getMessageKey(desc,@"title");
                    NSString* text = (NSString*)getMessageKey(desc,@"text");
                    NSString* link = (NSString*)getMessageKey(desc,@"link");
                    NSString* cancel = (NSString*)getMessageKey(desc,@"cancel");
                    NSString* ok = (NSString*)getMessageKey(desc,@"ok");
                    NSNumber* once = (NSNumber*)getMessageKey(desc,@"repeat");
                    NSNumber* always = (NSNumber*)getMessageKey(desc,@"always");
                    NSNumber* counter = (NSNumber*)getMessageKey(desc,@"counter");
                    NSString* messageType = (NSString*)getMessageKey(desc, @"type");
                    
                    if (startDate && [startDate isKindOfClass:[NSDate class]])
                        [message setObject:startDate forKey:iSmartNewsMessageStartDateKey];
                    
                    if (endDate && [endDate isKindOfClass:[NSDate class]])
                        [message setObject:endDate forKey:iSmartNewsMessageEndDateKey];
                    
                    if (title && [title isKindOfClass:[NSString class]])
                        [message setObject:title forKey:iSmartNewsMessageTitleKey];
                    
                    if (text && [text isKindOfClass:[NSString class]])
                        [message setObject:text forKey:iSmartNewsMessageTextKey];

                    if (link && [link isKindOfClass:[NSString class]])
                        [message setObject:link forKey:iSmartNewsMessageUrlKey];

                    if (cancel && [cancel isKindOfClass:[NSString class]])
                        [message setObject:cancel forKey:iSmartNewsMessageCancelKey];

                    if (ok && [ok isKindOfClass:[NSString class]])
                        [message setObject:ok forKey:iSmartNewsMessageActionKey];

                    if (once && [once isKindOfClass:[NSNumber class]])
                        [message setObject:once forKey:iSmartNewsMessageRepeatKey];
                    
                    if (always && [always isKindOfClass:[NSNumber class]])
                        [message setObject:always forKey:iSmartNewsMessageAlwaysKey];
                    
                    if (messageType && [messageType isKindOfClass:[NSString class]])
                        [message setObject:messageType forKey:iSmartNewsMessageTypeKey];
                    
                    // -- since version 1.4
                    if ([self isSharedInstance])
                    {
                        NSString* queue = (NSString*)getMessageKey(desc,@"queue");
                        if (queue && [queue isKindOfClass:[NSString class]])
                        {
                            queue = [queue stringByTrimmingCharactersInSet:[NSCharacterSet whitespaceAndNewlineCharacterSet]];
                            if ([queue length])
                                [message setObject:queue forKey:iSmartNewsMessageQueueKey];
                        }
                    }
                    // --
                    
                    // -- since version 1.3
                    if (![message objectForKey:iSmartNewsMessageTitleKey] 
                        && ![message objectForKey:iSmartNewsMessageTextKey] )
                    {
                        // special section
                        NSNumber* gate = (NSNumber*)getMessageKey(desc,@"gate");
                        if ([gate isKindOfClass:[NSNumber class]])
                            gate_ = [gate unsignedIntValue];
                        
                        // -- since version 1.4
                        if ([self isSharedInstance])
                        {
                            NSArray* queues = (NSArray*)getMessageKey(desc,@"queues");
                            if ([queues isKindOfClass:[NSArray class]])
                            {
                                for (NSDictionary* dict in queues)
                                {
                                    NSString* name = [dict objectForKey:@"name"];
                                    NSNumber* timeout = [dict objectForKey:@"timeout"];
                                    
                                    if ([name isKindOfClass:[NSString class]]
                                        && [timeout isKindOfClass:[NSNumber class]])
                                    {
                                        [queuesTimeouts_ setObject:timeout forKey:name];
                                    }
                                }
                            }
                        }
                        // --
                    }

                    // --                    
                    // new since version 1.2
                    if ([self isSharedInstance])
                    {
                        if (counter && [counter isKindOfClass:[NSNumber class]])
                            [message setObject:counter forKey:iSmartNewsMessageCounterKey];
                    }
                    // --

                    if (([message objectForKey:iSmartNewsMessageTitleKey] || [message objectForKey:iSmartNewsMessageTextKey])
                        && ![iSmartNews checkIfMessageWasAlreadyShown:message])
                    {
                        if (!messageType
                            || ([[[UIDevice currentDevice] systemVersion] floatValue] >= 6.0)
                            || ![messageType isEqualToString:iSmartNewsContentTypeWeb])
                        {
                        	[loadedNews_ addObject:message];
                            DBGLOG(@"Message parsed: %@",message);
                        }
                    }
                    else
                    {
                        DBGLOG(@"parse iteration: message skipped, because title, text key were not found or message was already shown");
                    }
                }
                @catch(...){}
                
#if !USES_ARC
                [message release];
#endif
            }
        }
        else{
            DBGLOG(@"Not NSArray object, skipped");
            if ([delegate_ respondsToSelector:@selector(smartNewsModule:didCatchError:)])
                [delegate_ smartNewsModule:self didCatchError:[NSError errorWithDomain:NSStringFromClass([self class])
                                                                                  code:-1
                                                                              userInfo:[NSDictionary dictionaryWithObjectsAndKeys:@"Invalid PLIST format",NSLocalizedDescriptionKey, nil]]];
        }
#if !USES_ARC
        [news release];
#endif
    }
    
    // -- since 1.4
    if ([self isSharedInstance])
    {
        [loadedNews_ sortUsingComparator:^NSComparisonResult(id o1, id o2){
            NSDictionary* m1 = o1;
            NSDictionary* m2 = o2;
            
            NSString* q1 = [m1 objectForKey:iSmartNewsMessageQueueKey];
            NSString* q2 = [m2 objectForKey:iSmartNewsMessageQueueKey];
            
            if (!q1 && !q2)
            {
                return NSOrderedSame;
            }
            else if (!q1 && q2)
            {
                return NSOrderedAscending;
            }
            else if (q1 && !q2)
            {
                return NSOrderedDescending;
            }
            else
            {
                return [q1 caseInsensitiveCompare:q2];
            }
        }];
        DBGLOG(@"NEW SORTED: %@",loadedNews_);
    }
    // --
    
    [newsData_ removeAllObjects];
    
    if ([loadedNews_ count])
    {
        if ([(NSObject*)delegate_ respondsToSelector:@selector(smartNewsModule:didLoadMessages:)])
        {
            for ( NSDictionary* message in loadedNews_)
                [iSmartNews setCacheValue:message];
            
            [delegate_ smartNewsModule:self didLoadMessages:[NSArray arrayWithArray:loadedNews_]];
            [self restartTimerIfNeeded];
        }
        else
        {
            [self showNextMessage];
        }
    }
    else
    {
        DBGLOG(@"No messages");
        [self restartTimerIfNeeded];
    }
}

- (BOOL)isAlertShown
{
    return alertView_ != nil;
}

- (void)showNextMessage
{
    if (self.popupWindow)
        return;
    
    [retryTimer_ invalidate];
    retryTimer_ = nil;
    
    [queueTimer_ invalidate];
    queueTimer_ = nil;
    
    if ([self isSharedInstance] && [self isAnyOtherModuleShowingAlert])
    {
#if !USES_ARC
        [currentQueue_ release];
#endif
        currentQueue_ = nil;
        
        [loadedNews_ removeAllObjects];
        
        [self restartTimerIfNeeded];
        
        if ([delegate_ respondsToSelector:@selector(smartNewsModuleDidCompleteMessageCycle:)])
            [delegate_ smartNewsModuleDidCompleteMessageCycle:self];
        return;
    }
    
    // counter logic, new since version 1.2
    const UInt64 counter = [[[iSmartNews settings] objectForKey:@"counter"] unsignedLongLongValue];
    
    NSDate* currentDate = [NSDate dateWithTimeIntervalSinceNow:0];
    for (;
         [loadedNews_ count];
         [loadedNews_ removeObjectAtIndex:0]
         )
    {
        NSDictionary* description = [loadedNews_ objectAtIndex:0];
        
        DBGLOG(@"checking message: %@",description);            
        
        NSDate* from = [description objectForKey:iSmartNewsMessageStartDateKey];
        if (from && [currentDate timeIntervalSinceDate:from] < 0){
            DBGLOG(@"start data not crossed, message will not be shown");            
            continue;
        }
            
        NSDate* to = [description objectForKey:iSmartNewsMessageEndDateKey];
        if (to && [currentDate timeIntervalSinceDate:to] >= 0){
            DBGLOG(@"end date crossed, message will not be shown");
            continue;        
        }
        
        if ([self isSharedInstance])
        {
            if (counter)
            {
                NSNumber* limitCounter = [description objectForKey:iSmartNewsMessageCounterKey];
                if (limitCounter && ([limitCounter unsignedLongLongValue] > 0) && (counter < [limitCounter unsignedLongLongValue]))
                {
                    DBGLOG(@"counter limit not crossed, message will not be shown");
                    continue;        
                }
            }
        }
        else
        {
            DBGLOG(@"not shared instance of iSmartNews");
        }
        
        NSString* title = [description objectForKey:iSmartNewsMessageTitleKey];
        NSString* message = [description objectForKey:iSmartNewsMessageTextKey];            
        NSString* cancel = [description objectForKey:iSmartNewsMessageCancelKey];                        
        NSString* ok = [description objectForKey:iSmartNewsMessageActionKey];
        NSString* url = [description objectForKey:iSmartNewsMessageUrlKey];

        if ([self isSharedInstance])
        {
            NSString* queue = [description objectForKey:iSmartNewsMessageQueueKey];
            if (queue)
            {
                if (!currentQueue_ || ![queue isEqualToString:currentQueue_])
                {
#if !USES_ARC
                    [currentQueue_ release];
#endif
                    currentQueue_ = [queue copy];

                    NSUInteger nQueued = 0;
                    
                    for (NSDictionary* m in loadedNews_)
                    {
                        NSString* queue = [m objectForKey:iSmartNewsMessageQueueKey];
                        if ([queue isEqualToString:currentQueue_])
                            nQueued++;
                    }
                    
                    NSMutableDictionary* q = [queuesInfo_ objectForKey:@"indexes"];
                    if (!q || ![q isKindOfClass:[NSMutableDictionary class]])
                    {
                        q = [NSMutableDictionary new];
#if !USES_ARC
                        [q autorelease];
#endif
                        [queuesInfo_ setObject:q forKey:@"indexes"];
                    }
                    
                    NSNumber* n = [q objectForKey:queue];
                    if (!n)
                    {
                        n = @(0);
                        [q setObject:n forKey:queue];
                    }
                    
                    if ([n unsignedIntValue] >= nQueued)
                    {
                        n = @(0);
                        [q setObject:n forKey:queue];
                    }
                    
                    [loadedNews_ removeObjectsInRange:NSMakeRange(0, [n unsignedIntValue])];
                    [loadedNews_ removeObjectsInRange:NSMakeRange(1, nQueued - [n unsignedIntValue] - 1)];
                    
                    n = @([n unsignedIntValue] + 1);
                    [q setObject:n forKey:queue];
                    
                    [self saveQueuesInfo];
                    
                    DBGLOG(@"NEXT INDEX %@",n);
                    
                    NSNumber* timeout = [queuesTimeouts_ objectForKey:queue];
                    if (timeout)
                    {
                        queueTimer_ = [NSTimer scheduledTimerWithTimeInterval:(isFirst_ ? 0.001 : [timeout doubleValue])
                                                                       target:self
                                                                     selector:@selector(showNextMessage)
                                                                     userInfo:nil repeats:NO];
                        return;
                    }
                }
            }
        }
        
        if (gCanIShowAlertViewRightNow && !gCanIShowAlertViewRightNow(self)){
            retryTimer_ = [NSTimer scheduledTimerWithTimeInterval:1
                                                           target:self
                                                         selector:@selector(showNextMessage)
                                                         userInfo:nil repeats:NO];
            return;
        }
        
        isFirst_ = NO;
        
        alertView_ = [[UIAlertView alloc] initWithTitle:title
                                                message:message
                                               delegate:self
                                      cancelButtonTitle:(cancel?cancel:NSLocalizedString(@"Cancel", @"Cancel"))
                                      otherButtonTitles:(url?(ok?ok:NSLocalizedString(@"Ok", @"Ok")):nil),nil];
        
        
        NSString * messageType = [description objectForKey:iSmartNewsMessageTypeKey];
        
        if ([messageType isEqualToString:iSmartNewsContentTypeWeb])
        {
            [self closeUI:NO];
            
#if 0// code is used to dump images into C arrays
            NSString* p1 = [[NSBundle mainBundle] pathForResource:@"close" ofType:@"png"];
            NSString* p2 = [[NSBundle mainBundle] pathForResource:@"close@2x" ofType:@"png"];
            
            {
                NSData* d = [NSData dataWithContentsOfFile:p1];
                const UInt8* c = [d bytes];
                const NSUInteger n = [d length];
                
                FILE* f = fopen([[NSTemporaryDirectory() stringByAppendingPathComponent:@"close_png.txt"] UTF8String], "wb");
                if (f){
                    fwrite("const UInt8 close_png[] = {\n", 1, sizeof("const UInt8 close_png[] = {\n"), f);
                    int v = 25;
                    for (NSUInteger i = 0; i < n; ++i){
                        static const char hex[] = {'0','1','2','3','4','5','6','7','8','9','A','B','C','D','E','F'};
                        char t[] = {'0','x',hex[(c[i] >> 4) & 0X0F],hex[c[i] & 0X0F],','};
                        fwrite(t, 1, 5, f);
                        if (--v == 0){
                            v = 25;
                            fwrite("\n", 1, 1, f);
                        }
                    }
                    fwrite("}\n", 1, 2, f);
                    fclose(f);
                }
            }
            
            {
                NSData* d = [NSData dataWithContentsOfFile:p2];
                const UInt8* c = [d bytes];
                const NSUInteger n = [d length];
                
                FILE* f = fopen([[NSTemporaryDirectory() stringByAppendingPathComponent:@"close2_png.txt"] UTF8String], "wb");
                if (f){
                    fwrite("const UInt8 close2_png[] = {\n", 1, sizeof("const UInt8 close_png[] = {\n"), f);
                    int v = 25;
                    for (NSUInteger i = 0; i < n; ++i){
                        static const char hex[] = {'0','1','2','3','4','5','6','7','8','9','A','B','C','D','E','F'};
                        char t[] = {'0','x',hex[(c[i] >> 4) & 0X0F],hex[c[i] & 0X0F],','};
                        fwrite(t, 1, 5, f);
                        if (--v == 0){
                            v = 25;
                            fwrite("\n", 1, 1, f);
                        }
                    }
                    fwrite("}\n", 1, 2, f);
                    fclose(f);
                }
            }
#endif
            self.popupWebView = AUTORELEASE([[UIWebView alloc] initWithFrame:CGRectMake(0,0,1,1)]);
            [self.popupWebView setDelegate:(id<UIWebViewDelegate>)self];
            [self.popupWebView loadRequest:[NSURLRequest requestWithURL:[NSURL URLWithString:message]]];
            
#if !USES_ARC
            alertView_.tag = 123;
#endif
        }
        else
        {
#if !USES_ARC
            [alertView_ retain];
#endif
            dispatch_after(dispatch_time(DISPATCH_TIME_NOW, (int64_t)(0.1 * NSEC_PER_SEC)), dispatch_get_main_queue(), ^{
                [alertView_ show];
                
#if !USES_ARC
                [alertView_ autorelease];
#endif
            });
            //            [alertView_ show];
#if !USES_ARC
            [alertView_ release];
#endif
        }
        
        if ([delegate_ respondsToSelector:@selector(smartNewsModule:didShowAlertView:)])
            [delegate_ smartNewsModule:self didShowAlertView:alertView_];
        
        return;
    }
    
    [self restartTimerIfNeeded];
    
    if ([delegate_ respondsToSelector:@selector(smartNewsModuleDidCompleteMessageCycle:)])
        [delegate_ smartNewsModuleDidCompleteMessageCycle:self];
}

#pragma mark -

- (void)cleanupUI
{
    if (alertView_)
    {
#if !USES_ARC
        if (alertView_.tag == 123)
            [alertView_ release];
#endif
        alertView_ = nil;
    }
    
    self.popupViewController.panel.delegate = nil;
    
    if (![self.previousWindow isKeyWindow])
        [self.previousWindow makeKeyAndVisible];
    
    self.previousWindow = nil;
    
    [self.popupWebView stopLoading];
    self.popupWebView = nil;
    
    if (self.popupWindow != [[UIApplication sharedApplication] keyWindow])
        [self.popupWindow removeFromSuperview];
    
    self.popupWindow = nil;
    self.popupViewController = nil;
}

- (void)closeUI:(BOOL)animated
{
    if (animated)
    {
        if (self.popupViewController.panel)
        {
            [self.popupViewController.panel hide];
        }
        else
        {
            [self cleanupUI];
        }
    }
    else
    {
        [self cleanupUI];
    }
}

- (void)cancelWasPressed
{
    _iSmartNews_OnUserChoice("CancelWasPressed");
    [self closeUI:YES];
    
    if (![loadedNews_ count])
        return;
    
    NSDictionary* message = [loadedNews_ objectAtIndex:0];
    
    DBGLOG(@"CANCEL button clicked");
    
    if (![[message objectForKey:iSmartNewsMessageAlwaysKey] boolValue]
        && ![[message objectForKey:iSmartNewsMessageRepeatKey] boolValue])
    {
        [iSmartNews setCacheValue:message];
    }
    
    [loadedNews_ removeObjectAtIndex:0];
    
    //--
    // new since version 1.3
    if (--gate_ == 0)
    {
        [loadedNews_ removeAllObjects];
        
        [self restartTimerIfNeeded];
        
        if ([delegate_ respondsToSelector:@selector(smartNewsModuleDidCompleteMessageCycle:)])
            [delegate_ smartNewsModuleDidCompleteMessageCycle:self];
        
        return;
    }
    //--
    
    [self showNextMessage];
}

- (void)actionWasPressed
{
    _iSmartNews_OnUserChoice("ActionWasPressed");
    [self closeUI:YES];
    
    if (![loadedNews_ count])
        return;
    
    NSDictionary* message = [loadedNews_ objectAtIndex:0];
    
    DBGLOG(@"OK button clicked");
    
    if (![[message objectForKey:iSmartNewsMessageAlwaysKey] boolValue])
        [iSmartNews setCacheValue:message];
    
    // Use URL only if not 'web' type
    NSString * messageType = [message objectForKey:iSmartNewsMessageTypeKey];
    
    if (!messageType || ![messageType isEqualToString:iSmartNewsContentTypeWeb])
    {
        NSString* urlString = [message objectForKey:iSmartNewsMessageUrlKey];
        if (urlString)
        {
            DBGLOG(@"Opening URL: %@",urlString);
            NSURL* url = [NSURL URLWithString:urlString];
            if (url)
                [[UIApplication sharedApplication] openURL:url];
        }
    }
    else{
        DBGLOG(@"URL IGNORED BECAUSE WEB");
    }
    
    [loadedNews_ removeObjectAtIndex:0];
    
    //--
    // new since version 1.3
    if (--gate_ == 0)
    {
        [loadedNews_ removeAllObjects];
        
        [self restartTimerIfNeeded];
        
        if ([delegate_ respondsToSelector:@selector(smartNewsModuleDidCompleteMessageCycle:)])
            [delegate_ smartNewsModuleDidCompleteMessageCycle:self];
        
        return;
    }
    //--
    
    [self showNextMessage];
}


- (void)nothingWasPressed
{
    _iSmartNews_OnUserChoice("NothingWasPressed");
    [self closeUI:NO];
    
    if (![loadedNews_ count])
        return;
    
    [loadedNews_ removeObjectAtIndex:0];
    
    //--
    // new since version 1.3
    if (--gate_ == 0)
    {
        [loadedNews_ removeAllObjects];
        
        [self restartTimerIfNeeded];
        
        if ([delegate_ respondsToSelector:@selector(smartNewsModuleDidCompleteMessageCycle:)])
            [delegate_ smartNewsModuleDidCompleteMessageCycle:self];
        
        return;
    }
    //--
    
    [self showNextMessage];
}

- (void)skipThisMessageWithoutProcessing
{
#if !USES_ARC
    if (alertView_.tag == 123)
        [alertView_ release];
#endif
    
    alertView_ = nil;
    
    if (![loadedNews_ count])
        return;
    
    [loadedNews_ removeObjectAtIndex:0];
    
    //--
    // new since version 1.3
    if (--gate_ == 0)
    {
        [loadedNews_ removeAllObjects];
        
        [self restartTimerIfNeeded];
        
        if ([delegate_ respondsToSelector:@selector(smartNewsModuleDidCompleteMessageCycle:)])
            [delegate_ smartNewsModuleDidCompleteMessageCycle:self];
        
        return;
    }
    //--
    
    [self showNextMessage];
}

#pragma mark -
#pragma mark UIAlerViewDelegate

- (void)alertViewCancel:(UIAlertView *)alertView
{
    if (alertView!=alertView_)
        return;
    
    [self nothingWasPressed];
}

- (void)alertView:(UIAlertView *)alertView didDismissWithButtonIndex:(NSInteger)buttonIndex
{
    if (alertView!=alertView_)
        return;
    
    if (alertView.cancelButtonIndex == buttonIndex)
        [self cancelWasPressed];
    else
        [self actionWasPressed];
}

#pragma mark -
#pragma mark Properties

- (void)setAutomaticMode:(BOOL)start
{
    ENSURE_MAIN_THREAD;
    
    DBGLOG(@"setAutomaticMode: %d",start);
    
    if ( start )
    {
        start_ = YES;
        [self restartTimerIfNeeded];
    }
    else
    {
        start_ = NO;
        [timer_ invalidate];
        timer_ = nil;
    }
}

- (BOOL)automaticMode
{
    return start_;
}

- (void)setUpdateInterval:(CFTimeInterval)updateInterval
{
    ENSURE_MAIN_THREAD;
    
    DBGLOG(@"setUpdateInterval: %f",updateInterval);
    
    if ( updateInterval < 10. )
        updateInterval = 10.;

    updateInterval_ = updateInterval;

    [self restartTimerIfNeeded];
}

- (CFTimeInterval)updateInterval
{
    return updateInterval_;
}

- (void)setUrls:(NSArray *)urls
{
    ENSURE_MAIN_THREAD;
    
    DBGLOG(@"setUrls: %@",urls);
    
    if (urls==urls_)
        return;
    
#if !USES_ARC
    [urls_ release];
#endif
    urls_ = [urls copy];
    
    if ([self isSharedInstance])
    {
        NSString* urlsMD5 = [queuesInfo_ objectForKey:@"URLS"];
        NSString* currentMd5 = md5ForArray(urls_);
        
        if (![urlsMD5 isEqualToString:currentMd5])
        {
            NSString* path = [[self class] queuesInfoSavePath];
            
            if ([[NSFileManager defaultManager] fileExistsAtPath:path])
            {
                [[NSFileManager defaultManager] removeItemAtPath:path error:NULL];
#if !USES_ARC
                [queuesInfo_ release];
#endif
                queuesInfo_ = [NSMutableDictionary new];
            }
        }
        
        [queuesInfo_ setObject:md5ForArray(urls_) forKey:@"URLS"];
        [self saveQueuesInfo];
    }
    
    [self restartTimerIfNeeded];
}

- (NSArray*)urls
{
    NSArray* i = [urls_ copy];
#if !USES_ARC
    [i autorelease];
#endif
    return i;
}

#pragma mark -
#pragma mark NSURLConnection delegate

- (void)connection:(NSURLConnection *)connection didReceiveResponse:(NSURLResponse *)response
{
    currentData_ = [[NSMutableData alloc] init];
}

- (void)connection:(NSURLConnection *)connection didReceiveData:(NSData *)data
{
	[currentData_ appendData:data];
}

- (void)connection:(NSURLConnection *)connection didFailWithError:(NSError *)error
{
    DBGLOG(@"Connection did fail %@",[error localizedDescription]);
    
    connection_ = nil;
#if !USES_ARC
    [currentData_ release];
#endif
    currentData_ = nil;

    if ([delegate_ respondsToSelector:@selector(smartNewsModule:didCatchError:)])
        [delegate_ smartNewsModule:self didCatchError:error];
    
    if ([self pumpUrls])
        return;
    
    [self parse];
    
}

#if DEBUG || ADHOC
- (void)showTestMessage{
    [newsData_ addObject:[
                          @"<?xml version=\"1.0\" encoding=\"UTF-8\"?>"
                          @"<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd\">"
                          @"<plist version=\"1.0\">"
                          @"<array>"
                          @"<dict>"
                          @"<key>gate</key>"
                          @"<integer>20</integer>"
                          @"</dict>"
                          @"<dict>"
                          @"<key>cancel</key>"
                          @"<string>cancel</string>"
                          @"<key>start</key>"
                          @"<date>2011-10-03T19:30:42Z</date>"
                          @"<key>end</key>"
                          @"<date>2017-10-05T19:30:44Z</date>"
                          @"<key>link</key>"
                          @"<string>http://google.com</string>"
                          @"<key>link_PT_pt_768x1024</key>"
                          @"<string>http://google.com</string>"
                          @"<key>ok</key>"
                          @"<string>open url</string>"
                          @"<key>text</key>"
                          @"<string>Some very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very long message!</string>"
                          @"<key>text_ru</key>"
                          @"<string>Простое и длинное длинное длинное длинное длинное длинное длинное длинное длинное длинное длинное длинное длинное длинное длинное длинное длинное длинное длинное сообщение!</string>"
                          @"<key>title</key>"
                          @"<string>title</string>"
                          @"<key>repeat</key>"
                          @"<true/>"
                          @"<key>counter</key>"
                          @"<integer>1</integer>"
                          @"<key>always</key>"
                          @"<true/>"
                          @"</dict>"
                          @"<dict>"
                          @"<key>type</key>"
                          @"<string>web</string>"
                          @"<key>start</key>"
                          @"<date>2011-10-03T19:30:42Z</date>"
                          @"<key>end</key>"
                          @"<date>2017-10-05T19:30:44Z</date>"
                          @"<key>text</key>"
                          @"<string>http://google.com</string>"
                          @"<key>text_ru</key>"
                          @"<string>http://google.com.ru</string>"
                          @"<key>repeat</key>"
                          @"<true/>"
                          @"<key>counter</key>"
                          @"<integer>1</integer>"
                          @"<key>always</key>"
                          @"<true/>"
                          @"</dict>"
                          @"</array>"
                          @"</plist>"
                          dataUsingEncoding:NSUTF8StringEncoding]];
    [self parse];
}
#endif

- (void)connectionDidFinishLoading:(NSURLConnection *)connection
{
    DBGLOG(@"Connection did finish loading");
    
    connection_ = nil;
    [newsData_ addObject:currentData_];
#if !USES_ARC
    [currentData_ release];
#endif
    currentData_ = nil;

    if ([self pumpUrls])
        return;

    [self parse];
}

- (NSCachedURLResponse *)connection:(NSURLConnection *)connection willCacheResponse:(NSCachedURLResponse *)cachedResponse
{
	DBGLOG(@"willCacheResponse: return nil");
	return nil;
}

#pragma mark - Queues support

+ (NSString*)queuesInfoSavePath
{
    return [[NSSearchPathForDirectoriesInDomains(NSLibraryDirectory, NSUserDomainMask, YES)
             objectAtIndex:0]
            stringByAppendingPathComponent:@"iSmartNewsQueuesSettings.plist"];
}

- (void)loadQueuesInfo
{
#if !USES_ARC
    [queuesInfo_ release];
#endif
    
    NSString* path = [[self class] queuesInfoSavePath];
    queuesInfo_ = [NSDictionary dictionaryWithContentsOfFile:path];
#if !USES_ARC
    [queuesInfo_ retain];
#endif
    if (!queuesInfo_ || ![[queuesInfo_ objectForKey:@"URLS"] isKindOfClass:[NSString class]])
    {
        if ([[NSFileManager defaultManager] fileExistsAtPath:path])
        {
            [[NSFileManager defaultManager] removeItemAtPath:path error:NULL];
        }
        
#if !USES_ARC
        [queuesInfo_ release];
#endif
        queuesInfo_ = [NSMutableDictionary new];
    }
    else
    {
        NSString* urlsMD5 = [queuesInfo_ objectForKey:@"URLS"];
        NSString* currentMd5 = md5ForArray(urls_);
        
        if (![urlsMD5 isEqualToString:currentMd5])
        {
            if ([[NSFileManager defaultManager] fileExistsAtPath:path])
            {
                [[NSFileManager defaultManager] removeItemAtPath:path error:NULL];
            }
#if !USES_ARC
            [queuesInfo_ release];
#endif
            queuesInfo_ = [NSMutableDictionary new];
        }
    }
}

- (void)saveQueuesInfo
{
    if ([urls_ count] == 0)
        return;
    
    NSString* path = [[self class] queuesInfoSavePath];
    
    if ([[NSFileManager defaultManager] fileExistsAtPath:path])
        [[NSFileManager defaultManager] removeItemAtPath:path error:NULL];
    
    [queuesInfo_ writeToFile:path atomically:YES];
}

#pragma mark -

- (BOOL)isAnyOtherModuleShowingAlert
{
    if ([self isiSmartReviewShowingAlert])
        return YES;
    
    return NO;
}

#pragma mark - iSmartReview alert detection

- (BOOL)isiSmartReviewShowingAlert
{
    Class iSmartReviewClass = NSClassFromString(@"iSmartReview");
    if (iSmartReviewClass)
    {
        typedef id (*SharedImpl)(id, SEL);
        
        SEL sharedSelector = NSSelectorFromString(@"shared");
        Method sharedMethod = class_getClassMethod(iSmartReviewClass,sharedSelector);
        if (sharedMethod)
        {
            SharedImpl sharedImpl = (SharedImpl)method_getImplementation(sharedMethod);
            if (sharedImpl)
            {
                id shared = sharedImpl(iSmartReviewClass,sharedSelector);
                
                typedef BOOL (*IsAlertShownImpl)(id, SEL);
                SEL isAlertShownSelector = NSSelectorFromString(@"isAlertShown");
                IsAlertShownImpl isAlertShownImpl = (IsAlertShownImpl)class_getMethodImplementation(iSmartReviewClass, isAlertShownSelector);
                if (isAlertShownImpl)
                {
                    const BOOL shown = isAlertShownImpl(shared,isAlertShownSelector);
                    if (shown)
                        return YES;
                }
            }
        }
    }
    return NO;
}

- (void)disableBadGestureRecognizer:(UIView*)view
{
    for (UIView * v in view.subviews)
    {
        if (v.gestureRecognizers)
        {
            for (UIGestureRecognizer * gr in v.gestureRecognizers)
            {
                if ([ gr isKindOfClass:NSClassFromString(@"_UIWebHighlightLongPressGestureRecognizer") ])
                    gr.enabled = NO;
                else if ([ gr isKindOfClass:NSClassFromString(@"UILongPressGestureRecognizer") ])
                    gr.enabled = NO;
            }
        }
        [ self disableBadGestureRecognizer:v ];
    }
}

#pragma mark -

- (void)webViewDidFinishLoad:(UIWebView *)webView
{
    if (self.popupWebView != webView)
        return;
    
    self.previousWindow = [[UIApplication sharedApplication] keyWindow];
    self.popupWindow = AUTORELEASE([[UIWindow alloc] initWithFrame:[[UIScreen mainScreen] bounds]]);
    self.popupWindow.windowLevel = UIWindowLevelAlert;
    [self.popupWindow setBackgroundColor:[UIColor clearColor]];
    
    self.popupViewController = AUTORELEASE([iSmartNewsPopupViewController new]);
    
    __block __weak id wself = self;
    
    self.popupViewController.webView = self.popupWebView;
    self.popupViewController.panel.onClosePressed = ^(iSmartNewsUAModalPanel* panel){
        [wself cancelWasPressed];
    };
    
    self.popupViewController.panel.delegate = (NSObject<iSmartNewsUAModalPanelDelegate>*)self;
    
    iSmartNewsPopupNavigationController* ctrl = AUTORELEASE([[iSmartNewsPopupNavigationController alloc] initWithRootViewController:self.popupViewController]);
    self.popupWindow.rootViewController = ctrl;
    [self.popupWindow makeKeyAndVisible];
    
    [self.popupWebView setScalesPageToFit:YES];
    [self.popupWebView setContentMode:UIViewContentModeScaleAspectFit];
    
    if ([self.popupWebView respondsToSelector:@selector(scrollView)])
    {
        UIScrollView *scroll=[self.popupWebView scrollView];
        
        const float zoom1=self.popupWebView.bounds.size.width/scroll.contentSize.width;
        const float zoom2=self.popupWebView.bounds.size.height/scroll.contentSize.height;
        [scroll setZoomScale:MIN(zoom1,zoom2) animated:YES];
        
        NSString *jsCommand = [NSString stringWithFormat:@"document.body.style.zoom = %f;",MIN(zoom1,zoom2)];
        [self.popupWebView stringByEvaluatingJavaScriptFromString:jsCommand];
    }
    
    [UIView animateWithDuration:0.35 animations:^ { [self.popupWebView setAlpha:1.f]; }];
}

- (void)webView:(UIWebView *)webView didFailLoadWithError:(NSError *)error
{
    if (self.popupWebView != webView)
        return;
    
    [self nothingWasPressed];
}

-(BOOL)webView:(UIWebView *)webView shouldStartLoadWithRequest:(NSURLRequest *)request navigationType:(UIWebViewNavigationType)navigationType;
{
    if (self.popupWebView != webView)
        return YES;
    
    NSURL *requestURL =[request URL];
    
    if (([[requestURL scheme] hasPrefix:@"http"]) && (navigationType == UIWebViewNavigationTypeLinkClicked))
    {
        [self actionWasPressed];
        
        return ![[UIApplication sharedApplication] openURL:requestURL];
    }
    
    return YES;
}

#pragma mark -

- (void)didCloseModalPanel:(iSmartNewsUAModalPanel *)modalPanel{
    [self cleanupUI];
    [self showNextMessage];
}

- (void)UIApplicationDidEnterBackgroundNotification{
    
    [self closeUI:NO];
    
    [queuesTimeouts_ removeAllObjects];
    
    [retryTimer_ invalidate];
    retryTimer_ = nil;
    
    [queueTimer_ invalidate];
    queueTimer_ = nil;
    
    [loadedNews_ removeAllObjects];
}

+ (void)load
{
    SwizzleInstanceMethod([UIApplication class], @selector(setStatusBarHidden:animated:),@selector(iSmartNews_setStatusBarHidden:animated:));
    SwizzleInstanceMethod([UIApplication class], @selector(setStatusBarHidden:withAnimation:),@selector(iSmartNews_setStatusBarHidden:withAnimation:));
    SwizzleInstanceMethod([UIApplication class], @selector(isStatusBarHidden),@selector(iSmartNews_isStatusBarHidden));
    
    [[NSNotificationCenter defaultCenter] addObserverForName:UIApplicationDidEnterBackgroundNotification
                                                      object:nil
                                                       queue:[NSOperationQueue mainQueue]
                                                  usingBlock:^(NSNotification* notification){
                                                      [[iSmartNews sharedNews] UIApplicationDidEnterBackgroundNotification];
                                                  }];
    
#if DEBUG || ADHOC
    [[NSNotificationCenter defaultCenter] addObserverForName:UIApplicationDidFinishLaunchingNotification
                                                      object:nil
                                                       queue:[NSOperationQueue mainQueue]
                                                  usingBlock:^(NSNotification* notification){
                                                      [self debug_integrateWithDebugDefaultKit];
                                                  }];
#endif
}

#if DEBUG || ADHOC
+ (void)debug_integrateWithDebugDefaultKit
{
    /****/
    Class iSmartDebugKitClass = NSClassFromString(@"iSmartDebugKit");
    
    typedef id (*defaultKit)(id self, SEL sel);
    
    SEL defaultKitSEL = NSSelectorFromString(@"defaultKit");
    if (defaultKitSEL)
    {
        Method defaultKitMethod = class_getClassMethod(iSmartDebugKitClass,defaultKitSEL);
        if (defaultKitMethod)
        {
            defaultKit defaultKitImpl = (defaultKit)method_getImplementation(defaultKitMethod);
            if (defaultKitImpl){
                id defaultKit = defaultKitImpl(iSmartDebugKitClass,defaultKitSEL);
                if (defaultKit){
                    
#if 0
                    SEL addTunableDoubleNamedSEL = NSSelectorFromString(@"addTunableDoubleNamed:setter:getter:minValue:maxValue:");
                    if (addTunableDoubleNamedSEL)
                    {
                        Method addTunableDoubleNamedMethod = class_getInstanceMethod(iSmartDebugKitClass, addTunableDoubleNamedSEL);
                        
                        typedef double (^getter)();
                        typedef void (^setter)(double);
                        typedef id (*addTunableDoubleNamed)(id self, SEL sel, NSString* name, setter s, getter g, double min, double max);
                        
                        addTunableDoubleNamed addTunableDoubleNamedMethodImpl = (addTunableDoubleNamed)method_getImplementation(addTunableDoubleNamedMethod);
                        if (addTunableDoubleNamedMethodImpl)
                        {
                            addTunableDoubleNamedMethodImpl(defaultKit,addTunableDoubleNamedSEL,
                                                            @"iSmartReview interval key",
                                                            ^(double v) {
                                                                [[NSUserDefaults standardUserDefaults] setObject:@(v) forKey:iSmartReviewIntervalKey];
                                                                [[NSUserDefaults standardUserDefaults] synchronize];
                                                            },
                                                            ^double{ return [[NSUserDefaults standardUserDefaults] doubleForKey:iSmartReviewIntervalKey]; },
                                                            1.0, 31 * 24 * 3600.0
                                                            );
                        }
                    }
#endif
                    
                    SEL addCustomActionSEL = NSSelectorFromString(@"addCustomAction:named:");
                    if (addCustomActionSEL)
                    {
                        Method addCustomActionMethod = class_getInstanceMethod(iSmartDebugKitClass, addCustomActionSEL);
                        
                        typedef void (^action)();
                        typedef id (*addCustomAction)(id self, SEL sel, action a, NSString* name);
                        
                        addCustomAction addCustomActionIMPL = (addCustomAction)method_getImplementation(addCustomActionMethod);
                        if (addCustomActionIMPL)
                        {
                            objc_msgSend(defaultKit,addCustomActionSEL,
                                         //addCustomActionIMPL(defaultKit,addCustomActionSEL,
                                         ^() {
                                             [[iSmartNews sharedNews] showTestMessage];
                                         },@"Show Test iSmartNews message"
                                         );
                        }
                    }
                }
            }
        }
    }
    /*
     [[iSmartDebugKit defaultKit] addTunableDoubleNamed:@"Tunable double"
     setter:^(double v) { tunableDouble = v; NSLog (@"Double changed to %f", tunableDouble); }
     getter:^double { return tunableDouble; }
     minValue:-5
     maxValue:5];
     */
    /****/
}
#endif

// methods to use as plugin

void _initWithUrlString (const char* string)
{
    NSString *urlString = [NSString stringWithUTF8String: string];
    NSArray * newsArray =  [NSArray arrayWithObject: urlString];
    iSmartNews_ApplicationDidFinishLaunchingAction(newsArray);
}


void _iSmartNews_ApplicationWillEnterForegroundAction()
{
    iSmartNews_ApplicationWillEnterForegroundAction();
}


void _iSmartNews_OnUserChoice(const char* string)
{
    UnitySendMessage("_iSmartNewsBridge", "OnUserChoice", string);
}

@end
