using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraUtil {
    private static volatile CameraUtil instance;
    private static System.Object syncRoot = new System.Object();

    public Dictionary<string, int> fileStates;

    public static CameraUtil Instance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new CameraUtil();
                }
            }

            return instance;
        }
    }

    public CameraUtil() {
        CheckFileStates();
    }

    public void CheckFileStates() {
        if (fileStates == null) {
            fileStates = new Dictionary<string, int>();
        }
    }

    public void SetFilenameState(string filename, int increment) {
        CheckFileStates();

        if (fileStates.ContainsKey(filename)) {
            fileStates[filename] = increment;
        }
        else {
            fileStates.Add(filename, increment);
        }
    }

    public int GetFilenameState(string filename) {
        CheckFileStates();

        if (fileStates.ContainsKey(filename)) {
            return fileStates[filename];
        }

        return 1;
    }

    public static void SaveScreenshotEditor() {
#if UNITY_EDITOR
		CameraUtil.Instance.SaveScreenshot();
#endif
    }

    public void SaveScreenshot() {
#if UNITY_EDITOR
		string gameName = GamePacks.currentPacksGame;
		string levelCode = GameLevels.Current.code;

		if(levelCode == "default"
			|| string.IsNullOrEmpty(levelCode)
			|| Context.Current.ApplicationLoadedLevelName().IndexOf("UIScene") > -1) {
			levelCode = Context.Current.ApplicationLoadedLevelName();
		}

		string fileName = gameName + "-screen-" + levelCode + "-";
		int screenWidth = (int)Screen.width;
		int screenHeight = (int)Screen.height;

        int number = GetFilenameState(fileName);
        string name = screenWidth.ToString() + "-" + screenHeight.ToString() + "-" + number.ToString();

		string path = "../screenshots";
		if(!System.IO.Directory.Exists(path)) {
			System.IO.Directory.CreateDirectory(path);
		}

		string savePath = path + "/" + fileName + name + ".png";

		SetFilenameState(fileName + name, number);

        while (System.IO.File.Exists(savePath)) {
            number++;
            name = "" + number;
			savePath = path + "/" + fileName + name + ".png";
			SetFilenameState(fileName + name, number);
        }

        Application.CaptureScreenshot(savePath);
#endif
    }
}