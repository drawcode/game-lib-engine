//#define USE_FEATURE_ETCETERA

using System;
using UnityEngine;

using Prime31;

public class PlatformKeys {
    public static string any = "any";
    public static string ios = "ios";
    public static string android = "android";
    public static string web = "web";
    public static string desktop = "desktop";
}

public class Platforms {
    private static volatile Platforms instance;
    private static System.Object syncRoot = new System.Object();
        
    public static Platforms Instance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null) 
                        instance = new Platforms();
                }
            }
            
            return instance;
        }
    }
    
    public Platforms() {
        
    }

    public static string CurrentPlatform {
        get {
#if UNITY_IPHONE
            return PlatformKeys.ios;
#elif UNITY_ANDROID
            return PlatformKeys.android;
#elif UNITY_WEBPLAYER
            return PlatformKeys.web;
#else
            return PlatformKeys.desktop;
#endif
        
        }
    }
    
    public static bool IsAmazonDevice() {
        if (SystemInfo.deviceModel.IndexOf("Kindle") > -1) {
            return true;
        }
        return false;
    }
    
    public static void ShowWebView(string title, string url) {
        Instance.showWebView(title, url);
    }
    
    public void showWebView(string title, string url) {

#if UNITY_EDITOR
        Application.OpenURL(url);
#elif UNITY_ANDROID && USE_FEATURE_ETCETERA
        EtceteraAndroid.showWebView(url);
#elif UNITY_IPHONE      
        EtceteraBinding.showWebPage(url, false);
#else
        Application.OpenURL(url);
#endif
    }

    public static void PlayMovie(string url, bool showControls, bool supportLandscape, bool supportPortrait) {
        Instance.playMovie(url, showControls, supportLandscape, supportPortrait);
    }
    
    public void playMovie(string url, bool showControls, bool supportLandscape, bool supportPortrait) {
        
#if UNITY_EDITOR
        Application.OpenURL(url);
#elif UNITY_ANDROID     
        Application.OpenURL(url);
#elif UNITY_IPHONE
        Application.OpenURL(url);
        //EtceteraTwoBinding.playMovie(url, showControls, supportLandscape, supportPortrait);
#endif
    }

    public static void ShowReviewAskView() {        
#if UNITY_ANDROID
        Platforms.AskForReview(
            AppConfigs.appGameDisplayName, 
            AppConfigs.appBundleId);
#elif UNITY_IPHONE  
        Platforms.AskForReview(
            AppConfigs.appGameDisplayName, 
            AppConfigs.appStoreId);
#endif     
    }
    
    public static void AskForReview(string appName, string bundleId) {
        Instance.askForReview(appName, bundleId);
    }
    
    public void askForReview(string appName, string bundleId) {
#if UNITY_ANDROID && USE_FEATURE_ETCETERA
        EtceteraAndroid.askForReviewNow("Review " + appName + "!", "Review " + appName + " if you like it.");
#elif UNITY_IPHONE && USE_FEATURE_ETCETERA 
        //EtceteraBinding.openAppStoreReviewPage(AppConfigs.pu
        EtceteraBinding.askForReview("Review " + appName + "!", "Review " + appName + " if you like it.", bundleId);
#endif      
    }

    public static void ShowReviewPage() {
        Instance.showReviewPage();
    }
    
    public void showReviewPage() {
#if UNITY_ANDROID && USE_FEATURE_ETCETERA
        bool isAmazon = AppConfigs.platformIsAmazon;
        EtceteraAndroid.openReviewPageInPlayStore( isAmazon );
#elif UNITY_IPHONE && USE_FEATURE_ETCETERA
        EtceteraBinding.openAppStoreReviewPage(AppConfigs.appStoreId);
#endif
    }

    public static void AskForReview(string appName, string bundleId, int launchCount, int hoursBetweenPrompts) {
        Instance.askForReview(appName, bundleId, launchCount, hoursBetweenPrompts);
    }
    
    public void askForReview(string appName, string bundleId, int launchCount, int hoursBetweenPrompts) {
#if UNITY_ANDROID && USE_FEATURE_ETCETERA
        EtceteraAndroid.askForReview(3, 0, 3, "Review " + appName + "!", "Review " + appName + " if you like it.", false);
#elif UNITY_IPHONE && USE_FEATURE_ETCETERA
        EtceteraBinding.askForReview(launchCount, hoursBetweenPrompts, 
                                     "Review " + appName + "!", "Review " + appName + " if you like it.", bundleId);
#endif
    }

    public static void ShowEmailView(string to, string subject, string body, bool isHtml) {
        Instance.showEmailView(to, subject, body, isHtml);
    }
    
    public void showEmailView(string to, string subject, string body, bool isHtml) {
#if UNITY_IPHONE && USE_FEATURE_ETCETERA
        if(EtceteraBinding.isEmailAvailable()) {
            EtceteraBinding.showMailComposer(to, subject, body + " -- Sent From iOS", isHtml); 
        }
        else {
            EtceteraBinding.showWebPage("mailto:" + to, false);
        }
#elif UNITY_ANDROID && USE_FEATURE_ETCETERA
        EtceteraAndroid.showEmailComposer(to, subject, body + " -- Sent From Android", isHtml);
#else
        Application.OpenURL("mailto:" + to);
#endif
    }
    
    public static void SaveImageToLibrary(string name, string fileToSave) {
        Instance.saveImageToLibrary(name, fileToSave);
    }
    
    public void saveImageToLibrary(string name, string fileToSave) {
#if UNITY_IPHONE && USE_FEATURE_ETCETERA
        EtceteraBinding.saveImageToPhotoAlbum(fileToSave);
#elif UNITY_ANDROID && USE_FEATURE_ETCETERA
        EtceteraAndroid.saveImageToGallery(fileToSave, name);
#else
        // TODO save to server on web
        // TODO save to desktop photos
#endif  
    }
}

