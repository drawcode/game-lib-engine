using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;

using UnityEngine;

using Engine.Utility;

namespace Engine.Social {

    // The platform share sheet (iOS UIActivityViewController, Android ACTION_SEND chooser): text,
    // an optional link and an optional image, handed to whatever apps the player has. No SDK and no
    // account: the OS owns the targets, so there is nothing to keep in step with a social network.
    //
    // Game code builds a NativeShareRequest and calls NativeShare.Share; the platform provider is
    // picked once and can be replaced (tests, another platform, a store that forbids sharing).
    public class NativeShareRequest {
        public string text;
        public string subject;
        public string url;
        public string imagePath;    // a PNG/JPG on disk, e.g. from NativeShare.CaptureScreen
    }

    public interface INativeShareProvider {
        bool isAvailable { get; }
        bool Share(NativeShareRequest request);
    }

    public static class NativeShare {

        static INativeShareProvider _provider;

        public static INativeShareProvider provider {
            get {
                if (_provider == null) {
                    _provider = CreateDefaultProvider();
                }
                return _provider;
            }
            set {
                _provider = value;
            }
        }

        // What was last handed to the sheet, for probes and tests (the sheet itself is native).
        public static NativeShareRequest lastRequest;

        public static event Action<NativeShareRequest> shared;

        public static bool isAvailable {
            get {
                return provider != null && provider.isAvailable;
            }
        }

        public static bool Share(NativeShareRequest request) {

            if (request == null) {
                return false;
            }

            if (!string.IsNullOrEmpty(request.imagePath) && !File.Exists(request.imagePath)) {
                LogUtil.LogError("NativeShare: no image at " + request.imagePath + " - sharing without it");
                request.imagePath = null;
            }

            if (string.IsNullOrEmpty(request.text) && string.IsNullOrEmpty(request.url)
                && string.IsNullOrEmpty(request.imagePath)) {
                return false;
            }

            if (!isAvailable) {
                LogUtil.Log("NativeShare: no share sheet on this platform");
                return false;
            }

            lastRequest = request;

            bool ok = provider.Share(request);

            if (ok && shared != null) {
                shared(request);
            }

            return ok;
        }

        static INativeShareProvider CreateDefaultProvider() {
#if UNITY_EDITOR
            return new NativeShareProviderEditor();
#elif UNITY_IOS
            return new NativeShareProviderIOS();
#elif UNITY_ANDROID
            return new NativeShareProviderAndroid();
#else
            return null;
#endif
        }

        // ------------------------------------------------------------------------------------
        // SCREEN CAPTURE
        //
        // The whole screen as drawn this frame (cameras AND overlay UI), read back after rendering,
        // written as PNG. Run it on the caller's MonoBehaviour: StartCoroutine(CaptureScreen(...)).
        // The texture is the caller's to show and to Destroy.

        public static string GetCapturePath(string fileName) {
            string dir = Path.Combine(Application.persistentDataPath, "share");
            Directory.CreateDirectory(dir);
            return Path.Combine(dir, fileName);
        }

        public static IEnumerator CaptureScreen(string path, Action<Texture2D> done) {

            yield return new WaitForEndOfFrame();

            Texture2D tex = ScreenCapture.CaptureScreenshotAsTexture();

            try {
                File.WriteAllBytes(path, tex.EncodeToPNG());
            }
            catch (Exception e) {
                LogUtil.LogError("NativeShare: could not write " + path + ": " + e.Message);
                path = null;
            }

            if (done != null) {
                done(path != null ? tex : null);
            }

            if (path == null) {
                UnityEngine.Object.Destroy(tex);
            }
        }
    }

    // Editor: nothing native to open. Logs the request so a probe can read it.
    public class NativeShareProviderEditor : INativeShareProvider {

        public bool isAvailable {
            get {
                return true;
            }
        }

        public bool Share(NativeShareRequest request) {
            LogUtil.Log("NativeShare (editor): text=" + request.text + " url=" + request.url
                + " image=" + request.imagePath);
            return true;
        }
    }

    // iOS: UIActivityViewController over Unity's view controller (EngineNativeShare.mm).
    public class NativeShareProviderIOS : INativeShareProvider {

#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        static extern void EngineNativeShare_Share(string text, string subject, string url, string imagePath);
#endif

        public bool isAvailable {
            get {
#if UNITY_IOS && !UNITY_EDITOR
                return true;
#else
                return false;
#endif
            }
        }

        public bool Share(NativeShareRequest request) {
#if UNITY_IOS && !UNITY_EDITOR
            EngineNativeShare_Share(request.text ?? "", request.subject ?? "", request.url ?? "",
                request.imagePath ?? "");
            return true;
#else
            return false;
#endif
        }
    }

    // Android: an ACTION_SEND chooser. An image goes through MediaStore (API 29+: no permission and
    // no FileProvider entry in the manifest), which also leaves a copy in the player's Pictures.
    // Below API 29 the share is text only.
    public class NativeShareProviderAndroid : INativeShareProvider {

        public bool isAvailable {
            get {
#if UNITY_ANDROID && !UNITY_EDITOR
                return true;
#else
                return false;
#endif
            }
        }

        public bool Share(NativeShareRequest request) {
#if UNITY_ANDROID && !UNITY_EDITOR
            try {
                using (AndroidJavaClass player = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (AndroidJavaObject activity = player.GetStatic<AndroidJavaObject>("currentActivity"))
                using (AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent")) {

                    intent.Call<AndroidJavaObject>("setAction", "android.intent.action.SEND");

                    string type = "text/plain";
                    AndroidJavaObject imageUri = string.IsNullOrEmpty(request.imagePath)
                        ? null : InsertImage(activity, request.imagePath);

                    if (imageUri != null) {
                        intent.Call<AndroidJavaObject>("putExtra", "android.intent.extra.STREAM", imageUri);
                        intent.Call<AndroidJavaObject>("addFlags", 1);  // FLAG_GRANT_READ_URI_PERMISSION
                        type = "image/png";
                    }

                    string text = request.text ?? "";

                    if (!string.IsNullOrEmpty(request.url)) {
                        text = string.IsNullOrEmpty(text) ? request.url : text + " " + request.url;
                    }

                    if (!string.IsNullOrEmpty(text)) {
                        intent.Call<AndroidJavaObject>("putExtra", "android.intent.extra.TEXT", text);
                    }

                    if (!string.IsNullOrEmpty(request.subject)) {
                        intent.Call<AndroidJavaObject>("putExtra", "android.intent.extra.SUBJECT", request.subject);
                    }

                    intent.Call<AndroidJavaObject>("setType", type);

                    using (AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent"))
                    using (AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>(
                        "createChooser", intent, request.subject ?? "")) {
                        activity.Call("startActivity", chooser);
                    }

                    if (imageUri != null) {
                        imageUri.Dispose();
                    }
                }

                return true;
            }
            catch (Exception e) {
                LogUtil.LogError("NativeShare (android): " + e.Message);
                return false;
            }
#else
            return false;
#endif
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        static AndroidJavaObject InsertImage(AndroidJavaObject activity, string path) {

            using (AndroidJavaClass version = new AndroidJavaClass("android.os.Build$VERSION")) {
                if (version.GetStatic<int>("SDK_INT") < 29) {
                    return null;
                }
            }

            byte[] bytes = File.ReadAllBytes(path);

            using (AndroidJavaObject values = new AndroidJavaObject("android.content.ContentValues"))
            using (AndroidJavaObject resolver = activity.Call<AndroidJavaObject>("getContentResolver"))
            using (AndroidJavaClass media = new AndroidJavaClass("android.provider.MediaStore$Images$Media"))
            using (AndroidJavaObject collection = media.GetStatic<AndroidJavaObject>("EXTERNAL_CONTENT_URI")) {

                values.Call("put", "_display_name", Path.GetFileName(path));
                values.Call("put", "mime_type", "image/png");
                values.Call("put", "relative_path", "Pictures/" + Application.productName);

                AndroidJavaObject uri = resolver.Call<AndroidJavaObject>("insert", collection, values);

                if (uri == null) {
                    return null;
                }

                using (AndroidJavaObject stream = resolver.Call<AndroidJavaObject>("openOutputStream", uri)) {
                    stream.Call("write", (sbyte[])(Array)bytes);
                    stream.Call("close");
                }

                return uri;
            }
        }
#endif
    }
}
