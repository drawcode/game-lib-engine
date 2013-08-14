using System;
using System.Collections.Generic;
using System.IO;
using Engine.Data.Json;
using Engine.Utility;

public class BaseGameProducts<T> : DataObjects<T> where T : new() {
    private static T current;
    private static volatile BaseGameProducts<T> instance;
    private static object syncRoot = new Object();

    public string BASE_DATA_KEY = "game-product-data";

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

public class BaseGameProduct : DataObject {

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    public int sort_order = 0;
    public int sort_order_type = 0;
    public string type;
    public string key;
    public string game_id;
    public string order_by;
    public string code;
    public string uuid;
    public bool active;

    public BaseGameProduct() {
        Reset();
    }

    public override void Reset() {
        base.Reset();

        sort_order = 0;
        sort_order_type = 0;
        type = "default";
        key = "default";
        game_id = "default";
        order_by = "default";
        code = "default";
        uuid = "default";
        active = true;
    }

    public void Clone(BaseGameProduct toCopy) {
        base.Clone(toCopy);

        active = toCopy.active;
        code = toCopy.code;
        game_id = toCopy.game_id;
        key = toCopy.key;
        order_by = toCopy.order_by;
        sort_order = toCopy.sort_order;
        sort_order_type = toCopy.sort_order_type;
        type = toCopy.type;
        uuid = toCopy.uuid;
    }

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
}