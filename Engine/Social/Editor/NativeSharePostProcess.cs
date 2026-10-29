#if UNITY_IOS

using System.IO;

using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

using UnityEngine;

// The iOS share sheet (Engine.Social.NativeShare) offers "Save Image", and iOS terminates an app
// that writes to the photo library without NSPhotoLibraryAddUsageDescription in its Info.plist.
// Adds a generic one when the app has not set its own; an app's own text is never replaced.
public static class NativeSharePostProcess {

    public const string usageKey = "NSPhotoLibraryAddUsageDescription";

    public static string usageText = "Save game photos you share to your photo library.";

    [PostProcessBuild(1002)]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuildProject) {

        if (target != BuildTarget.iOS) {
            return;
        }

        string plistPath = Path.Combine(pathToBuildProject, "Info.plist");

        PlistDocument plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        if (plist.root[usageKey] != null) {
            return;
        }

        plist.root.SetString(usageKey, usageText);
        plist.WriteToFile(plistPath);

        Debug.Log("NativeSharePostProcess: " + usageKey + " added");
    }
}

#endif
