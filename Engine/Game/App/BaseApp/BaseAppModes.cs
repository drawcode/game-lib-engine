using System;
using System.Collections.Generic;
using System.IO;

public class BaseAppModes<T> : DataObjects<T> where T : new() {
    private static T current;
    private static volatile BaseAppModes<T> instance;
    private static object syncRoot = new Object();

    private string BASE_DATA_KEY = "app-mode-data";

    public static T BaseCurrent {
        get {
            if (current == null) {
                lock (syncRoot) {
                    if (current == null)
                        current = new T();
                }
            }

            return current;
        }
        set {
            current = value;
        }
    }

    public static BaseAppModes<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseAppModes<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseAppModes() {
        Reset();
    }

    public BaseAppModes(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }

    public void ChangeState(string code) {

        if(AppModes.Current.code != code) {

            AppMode appMode = AppModes.Instance.GetByCode(code);

            if(appMode != null) {
                AppModes.Current = appMode;
                LogUtil.Log("AppModes:ChangeState:code:" + AppModes.Current.code);
            }
        }
    }
}

public class BaseAppMode : GameDataObject {

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    public BaseAppMode() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }
}