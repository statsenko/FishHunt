//
//  CustomKeyboardPlugin.mm
//  Unity-iPhone
//
//  Created by Hovaev Artem on 9/13/13.
//
//

#import "CustomKeyboardPlugin.h"
extern "C"
{
    void _SetAutoCapitalization(bool autocapitalization)
    {
        UIView *iv = (UIView*)[[KeyboardDelegate Instance] valueForKey:@"inputView"];
        
        if (iv != NULL)
        {
            NSLog(@"Found");
            UITextView *tv = [iv isKindOfClass:[UITextView class]]?(UITextView*)iv:NULL;
            UITextField *tf = [iv isKindOfClass:[UITextField class]]?(UITextField*)iv:NULL;
            UITextAutocapitalizationType autocapType = autocapitalization ?UITextAutocapitalizationTypeWords :UITextAutocapitalizationTypeNone;
            if (tv!=NULL)
            {
                tv.autocapitalizationType = autocapType;
                if ([tv isFirstResponder])
                {
                    [tv resignFirstResponder];
                    [tv becomeFirstResponder];
                }
            }
            if (tf!=NULL)
            {
                tf.autocapitalizationType = autocapType;
                if ([tf isFirstResponder])
                {
                    [tf resignFirstResponder];
                    [tf becomeFirstResponder];
                }
            }
        }
    }
}

