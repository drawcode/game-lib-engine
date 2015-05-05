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

    public GameProduct GetProductByPlaformProductCode(string code) {
        foreach (GameProduct product in GameProducts.Instance.GetAll()) {
            if (product.GetPlatformProductCode() == code) {
                return product;
            }
        }
        return null;
    }
}

public class BaseGameProductInfo : GameDataObject {
        
    public virtual string symbol {
        get { 
            return Get<string>(BaseDataObjectKeys.symbol, "$");
        }
        
        set {
            Set<string>(BaseDataObjectKeys.symbol, value);
        }
    }

    public virtual string locale {
        get { 
            return Get<string>(BaseDataObjectKeys.locale, PlatformKeys.any);
        }
        
        set {
            Set<string>(BaseDataObjectKeys.locale, value);
        }
    }
    
    public virtual string currency {
        get { 
            return Get<string>(BaseDataObjectKeys.currency, 
                               GameProductCurrencyType.currencyReal);
        }
        
        set {
            Set<string>(BaseDataObjectKeys.currency, value);
        }
    }

    public virtual double quantity {
        get { 
            return Get<double>(BaseDataObjectKeys.price, 1);
        }
        
        set {
            Set<double>(BaseDataObjectKeys.price, value);
        }
    }
    
    public virtual string cost {
        get { 
            return Get<string>(BaseDataObjectKeys.cost, "5000");
        }
        
        set {
            Set<string>(BaseDataObjectKeys.cost, value);
        }
    }

    public BaseGameProductInfo() {
        Reset();
    }

    public override void Reset() {
        cost = "5000";
        symbol = "$";
        display_name = "";
        description = "";
        locale = PlatformKeys.any;
        currency = GameProductCurrencyType.currencyReal;
        quantity = 1;
        code = "";
    }

    public string productPrice {
        get {
            return symbol + cost;
        }
    }
}

public class GameProductPlatformDatas : GameDataObject {

    public virtual List<GameProductInfo> meta {
        get { 
            return Get<List<GameProductInfo>>(
                BaseDataObjectKeys.meta, new List<GameProductInfo>());
        }
        
        set {
            Set<List<GameProductInfo>>(BaseDataObjectKeys.meta, value);
        }
    }

    public virtual List<GameDataObject> items {
        get {
            return Get<List<GameDataObject>>(BaseDataObjectKeys.items, new List<GameDataObject>());
        }
        
        set {
            Set<List<GameDataObject>>(BaseDataObjectKeys.items, value);
        }
    }

    public virtual List<GameProductPlatformData> platforms {
        get { 
            return Get<List<GameProductPlatformData>>(
                BaseDataObjectKeys.platforms, new List<GameProductPlatformData>());
        }
        
        set {
            Set<List<GameProductPlatformData> >(BaseDataObjectKeys.platforms, value);
        }
    }

    public GameProductPlatformData GetByPlatform(string platform) {
        foreach (GameProductPlatformData data in platforms) {
            if (data.platform == platform) {
                return data;
            }
        }
        return null;
    }
}

public class GameProductPlatformData : GameDataObject {

    public virtual string platform {
        get { 
            return Get<string>(BaseDataObjectKeys.platform, Platforms.CurrentPlatform);
        }
        
        set {
            Set<string>(BaseDataObjectKeys.platform, value);
        }
    }
}

public class GameProductInfo : BaseGameProductInfo {
    
}

public class GameProductType : BaseGameProductType {
    
}

public class GameProductCurrencyType {
    public static string currencyVirtual = "currency-virtual";
    public static string currencyReal = "currency-real";
}

public class BaseGameProductType {
    public static string pickup = "pickup";
    public static string powerup = "powerup";
    public static string item = "item";
    public static string rpgUpgrade = "rpg-upgrade";
    public static string feature = "feature";
    public static string character = "character";
    public static string characterSkin = "character-skin";
    public static string weapon = "weapon";
    public static string currency = "currency";
}

public class BaseGameProduct : GameDataObject {

    public virtual GameProductPlatformDatas data {
        get { 
            return Get<GameProductPlatformDatas>(BaseDataObjectKeys.data, new GameProductPlatformDatas());
        }
        
        set {
            Set<GameProductPlatformDatas>(BaseDataObjectKeys.data, value);
        }
    }


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
    
    public string GetPlatformProductCode() {
        return GetPlatformProductCode(Platforms.CurrentPlatform);
    }

    public string GetPlatformProductCode(string platform) {
        //string productId = "";

        GameProductPlatformData val = data.GetByPlatform(platform);
        if (val != null) {
            return val.code;
        }

        if (val == null) {
            val = data.GetByPlatform(PlatformKeys.any);
            if (val != null) {
                return val.code;
            }
        }

        return null;
    }
    
    public GameProductInfo GetDefaultProductInfoByLocale() {
        return GetProductInfoByLocale(PlatformKeys.any);
    }
    
    public GameProductInfo GetCurrentProductInfoByLocale() {
        return GetProductInfoByLocale(PlatformKeys.any);
    }
    
    public GameProductInfo GetProductInfoByLocale(string locale) {
        if (data != null) {
            if (data.meta != null) {

                foreach(GameProductInfo info in data.meta) {
                    if(info.locale == locale) {
                        return info;
                    }
                }
                
                foreach(GameProductInfo info in data.meta) {
                    if(info.locale == PlatformKeys.any) {
                        return info;
                    }
                }
            }
        }
        return null;
    }

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
}