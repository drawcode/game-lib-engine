using System;
using System.Collections.Generic;
using System.IO;

using Engine.Data.Json;
using Engine.Utility;

public class BaseGamePacks<T> : DataObjects<T> where T : DataObject, new() {
    private static T current;
    private static volatile BaseGamePacks<T> instance;
    private static object syncRoot = new Object();

    public static string BASE_DATA_KEY = "game-pack-data";

#if UNITY_IPHONE
	public static string currentPacksPlatform = "ios";
#elif UNITY_ANDROID
	public static string currentPacksPlatform = "android";
#else
    public static string currentPacksPlatform = "web";
#endif

#if UNITY_IPHONE
	public static int currentPacksIncrement = 3007; // Used for unity cache
#elif UNITY_ANDROID
	public static int currentPacksIncrement = 1001; // Used for unity cache
#else
    public static int currentPacksIncrement = 50; // Used for unity cache
#endif

    public static string currentPacksVersion = "1.1"; // used for actual version and on dlc storage
    public static string currentPacksGame = "game-drawlabs-template";
    public static string currentGameBundle = "com.drawk.template";

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

    public static BaseGamePacks<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseGamePacks<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public static string PACK_DEFAULT = "default";

    public BaseGamePacks() {
        Reset();
    }

    public BaseGamePacks(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }

    public virtual void ChangeCurrentGamePack(string code) {
        BaseCurrent = GetById(code);
        LogUtil.Log("Changing Pack: code:" + code);
    }

    /*
    public bool OwnsAllPacks() {
        foreach(BaseGamePack pack in GetAll()) {

            //if(pack.code != GameProducts.ITEM_PACK_SX_2012_ALL
            //	&& pack.code != GamePacks.PACK_DEFAULT) {
                if(!Contents.CheckGlobalContentAccess(pack.code)) {
                    return false;
                }

            //}
        }
        return true;
    }

    public bool OwnsAtLeastOnePack() {
        foreach(BaseGamePack pack in GetAll()) {

            //if(pack.code != GameProducts.ITEM_PACK_SX_2012_ALL
            //	&& pack.code != GamePacks.PACK_DEFAULT) {
                if(Contents.CheckGlobalContentAccess(pack.code)) {
                    return true;
                }

            //}
        }
        return false;
    }
    */
}

public class BaseGamePack : GameDataObject {

    public BaseGamePack() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }
}