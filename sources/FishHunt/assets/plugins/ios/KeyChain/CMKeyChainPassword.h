//
//  CMKeyChainPassword.h
//  MyFolder
//
//  Created by Artem Shimanski on 11.12.12.
//
//

#import <Foundation/Foundation.h>

@interface CMKeyChainPassword : NSObject
@property (nonatomic, retain) NSString* password;
- (id) initWithIdentifier:(NSString*) identifier secClass:(CFTypeRef) secClass;
- (void) setObject:(id) object forKey:(id<NSCopying>) key;
- (id) objectForKey:(id<NSCopying>) key;

@end
