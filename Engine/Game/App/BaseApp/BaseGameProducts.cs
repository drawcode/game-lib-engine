using System;
using System.Collections.Generic;
using System.IO;
using Engine.Game.Data;
using Engine.Utility;

namespace Engine.Game.App.BaseApp {
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
#if USE_GAME_LIB_GAMES

        public GameProduct GetProductByPlaformProductCode(string code) {
            foreach (GameProduct product in GameProducts.Instance.GetAll()) {
                if (product.GetPlatformProductCode() == code) {
                    return product;
                }
            }
            return null;
        }
#endif
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

        // ADDITIVE localization route (content, not UI strings). Read by
        // BaseGameUIPanelProducts.loadDataProductsToolkit -> LabelName/LabelDescription on
        // migrated (toolkitViewKey) store screens. `code` here is stamped by the owning
        // BaseGameProduct.WithLocalizationCode (this record has none of its own in the JSON), so
        // both getters resolve once that has run. Both display_name and description are ordinary
        // sales copy, not proper nouns, so both are keyed. Key convention:
        // game_product_<code>_name / game_product_<code>_desc. TrOrDefault falls back to the raw
        // English value whenever the key is absent, so any other game on this shared lib that
        // ships no such key sees its data unchanged. Key strings cached per `code`, not rebuilt
        // per call.
        private static readonly Dictionary<string, string> nameLocKeys = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> descLocKeys = new Dictionary<string, string>();

        public override string display_name {
            get {
                return LocalizedField(base.display_name, nameLocKeys, "_name");
            }

            set {
                base.display_name = value;
            }
        }

        public override string description {
            get {
                return LocalizedField(base.description, descLocKeys, "_desc");
            }

            set {
                base.description = value;
            }
        }

        private string LocalizedField(
                string raw, Dictionary<string, string> keyCache, string suffix) {

            string c = code;

            if (string.IsNullOrEmpty(c)) {
                return raw;
            }

            string key;

            if (!keyCache.TryGetValue(c, out key)) {
                key = "game_product_" + c.Replace('-', '_') + suffix;
                keyCache[c] = key;
            }

            return L10n.TrOrDefault(key, raw);
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
                Set<List<GameProductPlatformData>>(BaseDataObjectKeys.platforms, value);
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
        public static string access = "access";
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

                    foreach (GameProductInfo info in data.meta) {
                        if (info.locale == locale) {
                            return WithLocalizationCode(info);
                        }
                    }

                    foreach (GameProductInfo info in data.meta) {
                        if (info.locale == PlatformKeys.any) {
                            return WithLocalizationCode(info);
                        }
                    }
                }
            }
            return null;
        }

        // The per-locale meta record carries no `code` of its own (see game-product-data.json.txt:
        // "meta": [{ "display_name": ..., ... }], no "code" key) -- it is only ever reached through
        // the owning product. Stamp the product's code onto it before handing it back so
        // BaseGameProductInfo.display_name/description (additive localization route below) has a
        // stable key to build on. Harmless if already set; this data is display-only and never
        // written back to disk.
        private GameProductInfo WithLocalizationCode(GameProductInfo info) {

            if (info != null && string.IsNullOrEmpty(info.code)) {
                info.code = code;
            }

            return info;
        }

        // Attributes that are added or changed after launch should be like this to prevent
        // profile conversions.
    }

    // OVERRIDE TO CUSTOMIZE IN GAME

    // public class GameProduct : BaseGameProduct
    // {
    //     public GameProduct()
    //     {
    //         Reset();
    //     }

    //     public override void Reset()
    //     {
    //         base.Reset();
    //     }
    // }
}