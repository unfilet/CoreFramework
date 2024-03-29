#import <Foundation/Foundation.h>
#import <AuthenticationServices/AuthenticationServices.h>

#if TARGET_OS_TV || TARGET_OS_IOS
#import "UnityAppController.h"
#endif

struct UserInfo
{
    const char * userId;
    const char * email;
    const char * displayName;

    const char * idToken;
    const char * error;

    ASUserDetectionStatus userDetectionStatus;
};

typedef void (*SignInWithAppleCallback)(int result, struct UserInfo s1);
typedef void (*CredentialStateCallback)(ASAuthorizationAppleIDProviderCredentialState state);

@interface UnitySignInWithApple : NSObject<ASAuthorizationControllerDelegate, ASAuthorizationControllerPresentationContextProviding>

@property (nonatomic) SignInWithAppleCallback loginCallback;
@property (nonatomic) CredentialStateCallback credentialStateCallback;

@end

static UnitySignInWithApple* _unitySignInWithAppleInstance;

@implementation UnitySignInWithApple
{
    ASAuthorizationAppleIDRequest* request;
}

+(UnitySignInWithApple*)instance
{
    if (_unitySignInWithAppleInstance == nil)
        _unitySignInWithAppleInstance = [[UnitySignInWithApple alloc] init];
    return _unitySignInWithAppleInstance;
}

-(void)startRequestWithNonce: (NSString *) nonce
{
    if (@available(iOS 13.0, tvOS 13.0, macOS 10.15, *)) {
        ASAuthorizationAppleIDProvider* provider = [[ASAuthorizationAppleIDProvider alloc] init];
        request = [provider createRequest];
        [request setRequestedScopes: @[ASAuthorizationScopeEmail, ASAuthorizationScopeFullName]];
        [request setNonce: nonce];

        ASAuthorizationController* controller = [[ASAuthorizationController alloc] initWithAuthorizationRequests:@[request]];
        controller.delegate = self;
        controller.presentationContextProvider = self;
        [controller performRequests];
    } else {
        // Fallback on earlier versions
    }
}

- (void)getCredentialState:(NSString *)userID
{
    ASAuthorizationAppleIDProvider* provider = [[ASAuthorizationAppleIDProvider alloc] init];
    [provider getCredentialStateForUserID:userID
                               completion:^(ASAuthorizationAppleIDProviderCredentialState credentialState, NSError * _Nullable error) {
        dispatch_async(dispatch_get_main_queue(), ^{
            self.credentialStateCallback(credentialState);
        });
    }];
}

-(ASPresentationAnchor)presentationAnchorForAuthorizationController:(ASAuthorizationController *)controller
{
#if TARGET_OS_TV || TARGET_OS_IOS
    return _UnityAppController.window;
#else
    return [[NSApplication sharedApplication] mainWindow];
#endif
}

-(void)authorizationController:(ASAuthorizationController *)controller didCompleteWithAuthorization:(ASAuthorization *)authorization
{
    if (self.loginCallback)
    {
        struct UserInfo data;

        if (@available(iOS 13.0, tvOS 13.0, macOS 10.15, *)) {
            ASAuthorizationAppleIDCredential* credential = (ASAuthorizationAppleIDCredential*)authorization.credential;
            NSString* idToken = [[NSString alloc] initWithData:credential.identityToken encoding:NSUTF8StringEncoding];
            NSPersonNameComponents* name = credential.fullName;

            data.idToken = [idToken UTF8String];

            data.displayName = [[NSPersonNameComponentsFormatter localizedStringFromPersonNameComponents:name
                                                                                                   style:NSPersonNameComponentsFormatterStyleDefault
                                                                                                 options:0] UTF8String];
            // Email gets treated a little differently. Rather than an empty string it will return nil for no value.
            data.email = credential.email == nil ? "" : [credential.email UTF8String];
            data.userId = [credential.user UTF8String];
            data.userDetectionStatus = credential.realUserStatus;
            data.error = "";
            self.loginCallback(1, data);
        } else {
            // Fallback on earlier versions
        }
    }
}

-(void)authorizationController:(ASAuthorizationController *)controller didCompleteWithError:(NSError *)error
{
    if (self.loginCallback)
    {
        // All members need to be set to a non-null value.
        struct UserInfo data;
        data.idToken = "";
        data.displayName = "";
        data.email = "";
        data.userId = "";
        data.userDetectionStatus = 1;
        data.error = [error.localizedDescription UTF8String];
        self.loginCallback(0, data);
    }
}

@end

void UnitySignInWithApple_Login(const char* _Nullable nonceCStr, SignInWithAppleCallback callback)
{
    if (@available(iOS 13.0, tvOS 13.0, macOS 10.15, *)) {
        NSString *nonce = nonceCStr != NULL ? [NSString stringWithUTF8String:nonceCStr] : nil;
        UnitySignInWithApple* login = [UnitySignInWithApple instance];
        login.loginCallback = callback;
        [login startRequestWithNonce: nonce];
    } else {
        // Fallback on earlier versions
    }
}

void UnitySignInWithApple_GetCredentialState(const char *userID, CredentialStateCallback callback)
{
    if (@available(iOS 13.0, tvOS 13.0, macOS 10.15, *)) {
        UnitySignInWithApple* login = [UnitySignInWithApple instance];
        login.credentialStateCallback = callback;
        [login getCredentialState: [NSString stringWithUTF8String: userID]];
    } else {
        // Fallback on earlier versions
    }
}
