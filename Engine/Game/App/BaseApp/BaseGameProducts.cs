using System;
using System.Collections.Generic;
using System.IO;
using Engine.Data.Json;
using Engine.Utility;

public class BaseGameProducts<T> : DataObjects<T> where T : DataObject, new() {
    private static T current;
    private static volatile BaseGameProducts<T> instance;
    private static object syncRoot = new Object();

    public static string BASE_DATA_KEY = "game-product-data";

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

    public static BaseGameProducts<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseGameProducts<T>(true);
                }
            }

            return instance;
        }
    }

    public BaseGameProducts() {
        Reset();
    }

    public BaseGameProducts(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }

    public virtual void ChangeCurrentGameProduct(string code) {
        BaseCurrent = GetById(code);
        LogUtil.Log("Changing Product: code:" + code);
    }
}

public class BaseGameProductInfo {
    public string price;
    public string currencySymbol;
    public string title;
    public string description;
    public string locale;
    public string productId;

    public BaseGameProductInfo() {
        Reset();
    }

    public void Reset() {
        price = ".99";
        currencySymbol = "$";
        title = "";
        description = "";
        locale = "en";
    }

    public string productPrice {
        get {
            return currencySymbol + price;
        }
    }
}

public class BaseGameProduct : GameDataObject {

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    public BaseGameProduct() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }

    public void Clone(BaseGameProduct toCopy) {
        base.Clone(toCopy);
    }

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
}