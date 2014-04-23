//
//  OSInfoPlugin.mm
//  Unity-iPhone
//
//  Created by Artem Hovaev on 10/14/13.
//
//

#import "CMKeyChainPassword.h"
#include "OSInfoPlugin.h"
extern "C"
{
    const int _GetMajorOsVersion()
    {
        NSString *version = [[UIDevice currentDevice] systemVersion];
        NSArray* components = [version componentsSeparatedByString:@"."];
        
        for (int i = 0; i < components.count; ++i)
            NSLog(@"%@", components[i]);
        
        return [components[0] intValue];
    }
    
    const int _GetMinorOsVersion()
    {
        NSString *version = [[UIDevice currentDevice] systemVersion];
        NSArray* components = [version componentsSeparatedByString:@"."];
        
        if (components.count > 0)
            return [components[1] intValue];
        else
            return 0;
    }
    
    const char * _GetCFBundleVersion()
    {
        NSString *version = [[NSBundle mainBundle] objectForInfoDictionaryKey:@"CFBundleVersion"];
        return strdup([version UTF8String]);
    }
    
    const char* _GetGUID()
    {
        CMKeyChainPassword* keyChainPassword = [[CMKeyChainPassword alloc] initWithIdentifier:@"chefsstory" secClass:kSecClassGenericPassword];
        
        NSString* guid = [keyChainPassword password];
        if (guid == NULL || guid.length == 0)
        {
            guid = [[[UIDevice currentDevice] identifierForVendor] UUIDString];
            
            [keyChainPassword setPassword:guid];
        }
        
        [keyChainPassword release];
        
        return strdup([guid UTF8String]);
    }
}

