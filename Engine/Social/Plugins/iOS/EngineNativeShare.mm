// The iOS share sheet for Engine.Social.NativeShare (NativeShareProviderIOS): text, an optional
// link and an optional image file, presented over Unity's view controller. On iPad the sheet is
// a popover, anchored here to the middle of the screen with no arrow.

#import <UIKit/UIKit.h>

extern UIViewController* UnityGetGLViewController();

static NSString* EngineNativeShare_String(const char* value) {
    if (value == NULL) {
        return nil;
    }
    NSString* s = [NSString stringWithUTF8String:value];
    return s.length > 0 ? s : nil;
}

extern "C" {

    void EngineNativeShare_Share(const char* text, const char* subject, const char* url, const char* imagePath) {

        NSMutableArray* items = [NSMutableArray array];

        NSString* textValue = EngineNativeShare_String(text);
        if (textValue != nil) {
            [items addObject:textValue];
        }

        NSString* urlValue = EngineNativeShare_String(url);
        if (urlValue != nil) {
            NSURL* link = [NSURL URLWithString:urlValue];
            if (link != nil) {
                [items addObject:link];
            }
        }

        NSString* pathValue = EngineNativeShare_String(imagePath);
        if (pathValue != nil) {
            UIImage* image = [UIImage imageWithContentsOfFile:pathValue];
            if (image != nil) {
                [items addObject:image];
            }
        }

        if (items.count == 0) {
            return;
        }

        UIViewController* root = UnityGetGLViewController();

        if (root == nil) {
            return;
        }

        UIActivityViewController* sheet = [[UIActivityViewController alloc] initWithActivityItems:items
                                                                            applicationActivities:nil];

        NSString* subjectValue = EngineNativeShare_String(subject);
        if (subjectValue != nil) {
            [sheet setValue:subjectValue forKey:@"subject"];
        }

        UIPopoverPresentationController* popover = sheet.popoverPresentationController;
        if (popover != nil) {
            popover.sourceView = root.view;
            popover.sourceRect = CGRectMake(CGRectGetMidX(root.view.bounds), CGRectGetMidY(root.view.bounds), 1, 1);
            popover.permittedArrowDirections = 0;
        }

        [root presentViewController:sheet animated:YES completion:nil];
    }
}
