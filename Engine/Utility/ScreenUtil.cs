using System.Collections;
using System.IO;
using Engine;

// Saves screenshot as PNG file.
using UnityEngine;
using UnityEngine.Networking;

public class ScreenUtil {

    // -------------------------------------------------------------------
    // Singleton access

    private static volatile ScreenUtil instance;
    private static object syncRoot = new Object();

    public float defaultDesiredWidth = 960f;
    public float defaultDesiredHeight = 640f;

    private ScreenUtil() {
    }

    public static ScreenUtil Instance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new ScreenUtil();
                }
            }

            return instance;
        }
    }

    // -------------------------------------------------------------------
    // Unity auto called methods

    private void Start() {

        //UploadPNG();
    }

    // SCREEN SIZE

    public static float relativeWidth {
        get {
            return GetRelativeWidth();
        }
    }

    public static float relativeHeight {
        get {
            return GetRelativeHeight();
        }
    }

    public static float GetRelativeWidth() {
        return Instance.getRelativeWidth();
    }

    public float getRelativeWidth() {
        return Screen.width / defaultDesiredWidth;
    }

    public static float GetRelativeWidth(float desiredWidth) {
        return Instance.getRelativeWidth(desiredWidth);
    }

    public float getRelativeWidth(float desiredWidth) {
        return Screen.width / desiredWidth;
    }

    public static float GetRelativeHeight() {
        return Instance.getRelativeHeight();
    }

    public float getRelativeHeight() {
        return Screen.height / defaultDesiredHeight;
    }

    public static float GetRelativeHeight(float desiredHeight) {
        return Instance.getRelativeHeight(desiredHeight);
    }

    public float getRelativeHeight(float desiredHeight) {
        return Screen.height / desiredHeight;
    }

    // SCREENSHOT IMAGES

    public IEnumerator UploadImage() {

        // We should only read the screen bufferafter rendering is complete
        yield return new WaitForEndOfFrame();

        // Create a texture the size of the screen, RGB24 format
        var width = Screen.width;
        var height = Screen.height;
        var tex = new Texture2D(width, height, TextureFormat.RGB24, false);

        // Read screen contents into the texture
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        // Encode texture into PNG
        byte[] bytes = tex.EncodeToPNG();

        //Destroy(tex);

        // For testing purposes, also write to a file in the project folder
        // File.WriteAllBytes(Application.dataPath + "/../SavedScreen.png", bytes);

        // Create a Web Form
        var form = new WWWForm();
        form.AddField("frameCount", Time.frameCount.ToString());
        form.AddBinaryData("fileUpload", bytes);

        // Upload to a cgi script
        UnityWebRequest w = UnityWebRequest.Post("http://tools.host.com/screenshots/upload", form);

        yield return w.SendWebRequest();

        if (w.error != null) {

            //print(w.error);
        }
        else {

            //print("Finished Uploading Screenshot");
        }
    }
}