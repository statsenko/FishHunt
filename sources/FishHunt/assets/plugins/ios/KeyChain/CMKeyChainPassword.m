//
//  CMKeyChainPassword.m
//  MyFolder
//
//  Created by Artem Shimanski on 11.12.12.
//
//

#import "CMKeyChainPassword.h"

@interface CMKeyChainPassword()
@property (nonatomic, retain) NSMutableDictionary* query;
@property (nonatomic, retain) NSMutableDictionary* keychainItemData;

- (void)resetKeychainItem;
- (OSStatus)writeToKeychain;

@end

@implementation CMKeyChainPassword

- (id) initWithIdentifier:(NSString*) identifier secClass:(CFTypeRef) secClass {
	if (self = [super init]) {
		self.query = [NSMutableDictionary dictionary];
		[self.query setObject:(id) secClass forKey:(id) kSecClass];
		[self.query setObject:identifier forKey:(id) kSecAttrGeneric];
		[self.query setObject:(id)kSecMatchLimitOne forKey:(id)kSecMatchLimit];
        [self.query setObject:(id)kCFBooleanTrue forKey:(id)kSecReturnAttributes];
        
        NSMutableDictionary *outDictionary = nil;
		OSStatus result = SecItemCopyMatching((CFDictionaryRef)self.query, (CFTypeRef *)&outDictionary);
        if (result != noErr)
        {
            NSError *error = [NSError errorWithDomain:NSOSStatusErrorDomain code:result userInfo:nil];
            NSLog(@"%@", error.debugDescription);
            [self resetKeychainItem];
		}
		else
        {
			NSMutableDictionary* queryData = [NSMutableDictionary dictionaryWithDictionary:self.query];
			[queryData setObject:(id)kCFBooleanTrue forKey:(id)kSecReturnData];
			OSStatus result = SecItemCopyMatching((CFDictionaryRef)queryData, (CFTypeRef *)&outDictionary);
			if (result == noErr)
            {
				self.keychainItemData = [NSMutableDictionary dictionaryWithDictionary:outDictionary];
				[self.keychainItemData setObject:(id) secClass forKey:(id) kSecClass];
			}
			else {
				[self release];
				return nil;
			}
		}
		
	}
	return self;
}

- (void) dealloc {
	[_query release];
	[_keychainItemData release];
	[super dealloc];
}

- (void) setObject:(id) object forKey:(id<NSCopying>) key {
	[self.keychainItemData setObject:object forKey:key];
    OSStatus status = [self writeToKeychain];
    if (status != noErr)
    {
        NSError *error = [NSError errorWithDomain:NSOSStatusErrorDomain code:status userInfo:nil];
        NSLog(@"%@", error.debugDescription);
    }
}

- (id) objectForKey:(id<NSCopying>) key {
	return [self.keychainItemData objectForKey:key];
}

- (NSString*) password {
	NSData* data = [self.keychainItemData objectForKey:(id) kSecValueData];
	return [[[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding] autorelease];
}

- (void) setPassword:(NSString*) password {
	[self setObject:[password dataUsingEncoding:NSUTF8StringEncoding] forKey:(id)kSecValueData];
}

#pragma mark - Private

- (void)resetKeychainItem
{
    if (!self.keychainItemData)
        self.keychainItemData = [NSMutableDictionary dictionary];
    else
    {
		SecItemDelete((CFDictionaryRef)self.keychainItemData);
    }
    
    [self.keychainItemData setObject:@"" forKey:(id)kSecAttrAccount];
    [self.keychainItemData setObject:@"" forKey:(id)kSecAttrLabel];
    [self.keychainItemData setObject:@"" forKey:(id)kSecAttrDescription];
    [self.keychainItemData setObject:[@"" dataUsingEncoding:NSUTF8StringEncoding] forKey:(id)kSecValueData];
	[self.keychainItemData setObject:[self.query objectForKey:(id)kSecClass] forKey:(id)kSecClass];
	[self.keychainItemData setObject:[self.query objectForKey:(id)kSecAttrGeneric] forKey:(id)kSecAttrGeneric];
}

- (OSStatus)writeToKeychain
{
    NSDictionary *attributes = NULL;
    NSMutableDictionary *updateItem = NULL;
    
    if (SecItemCopyMatching((CFDictionaryRef)self.query, (CFTypeRef *)&attributes) == noErr)
    {
        updateItem = [NSMutableDictionary dictionaryWithDictionary:attributes];
        [updateItem setObject:[self.keychainItemData objectForKey:(id)kSecClass] forKey:(id)kSecClass];
        
        NSMutableDictionary *tempCheck = [NSMutableDictionary dictionaryWithDictionary:self.keychainItemData];
        [tempCheck removeObjectForKey:(id)kSecClass];
        
        return SecItemUpdate((CFDictionaryRef)updateItem, (CFDictionaryRef)tempCheck);
    }
    else
    {
        return SecItemAdd((CFDictionaryRef)self.keychainItemData, NULL);
    }
}

@end
