using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class BaseAppContentTips<T> : DataObjects<T> where T : DataObject, new() {
    private static T current;
    private static volatile BaseAppContentTips<T> instance;
    private static object syncRoot = new Object();
    private string BASE_DATA_KEY = "app-content-tip-data";

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

    public static BaseAppContentTips<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseAppContentTips<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseAppContentTips() {
        Reset();
    }

    public BaseAppContentTips(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }

    public void ChangeState(string code) {
        if (AppContentTips.Current.code != code) {
            AppContentTips.Current = AppContentTips.Instance.GetById(code);
        }
    }

    public List<AppContentTip> GetListByCodeAndPackCode(string assetCode, string packCode) {
        List<AppContentTip> filteredList = new List<AppContentTip>();
        foreach (AppContentTip obj in AppContentTips.Instance.GetListByPack(packCode)) {
            if (assetCode.ToLower() == obj.code.ToLower()) {
                filteredList.Add(obj);
            }
        }

        return filteredList;
    }
}

public class BaseAppContentTip : GameDataObject {    

    public BaseAppContentTip() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
        tags = new List<string>();
        keys = new List<string>();
    }

}