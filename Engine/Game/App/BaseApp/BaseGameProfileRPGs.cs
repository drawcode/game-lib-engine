using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

// using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public class BaseGameProfileRPGAttributes {

    public static string ATT_PROGRESS_CURRENCY = "progress-currency";
    public static string ATT_PROGRESS_XP = "progress-xp";
    public static string ATT_PROGRESS_HEALTH = "progress-health";
    public static string ATT_PROGRESS_ENERGY = "progress-energy";
    public static string ATT_PROGRESS_LEVEL = "progress-level";
    public static string ATT_PROGRESS_UPGRADES_APPLIED = "progress-upgrades-applied";
    public static string ATT_PROGRESS_UPGRADES = "progress-upgrades";
    public static string ATT_PROGRESS_POINTS = "progress-points";
    public static string ATT_PROGRESS_STARS = "progress-stars";
    public static string ATT_PROGRESS_COINS = "progress-coins";
}

public class BaseGameProfileRPGs {
    private static volatile BaseGameProfileRPG current;
    private static volatile BaseGameProfileRPGs instance;
    private static System.Object syncRoot = new System.Object();
    public static string DEFAULT_USERNAME = "Player";

    public static BaseGameProfileRPG BaseCurrent {
        get {
            if(current == null) {
                lock(syncRoot) {
                    if(current == null)
                        current = new BaseGameProfileRPG();
                }
            }

            return current;
        }
        set {
            current = value;
        }
    }

    public static BaseGameProfileRPGs BaseInstance {
        get {
            if(instance == null) {
                lock(syncRoot) {
                    if(instance == null)
                        instance = new BaseGameProfileRPGs();
                }
            }

            return instance;
        }
    }

    // TODO: Common profile actions, lookup, count, etc
}

public class GameDataItemRPG : GameDataObject {


    // RPG

    public virtual double duration {
        get {
            return Get<double>(BaseDataObjectKeys.duration, 0.0);
        }

        set {
            Set<double>(BaseDataObjectKeys.duration, value);
        }
    }

    public virtual double scale {
        get {
            return Get<double>(BaseDataObjectKeys.scale, 1.0);
        }

        set {
            Set<double>(BaseDataObjectKeys.scale, value);
        }
    }

    public virtual double speed {
        get {
            return Get<double>(BaseDataObjectKeys.speed, 1.0);
        }

        set {
            Set<double>(BaseDataObjectKeys.speed, value);
        }
    }

    public virtual double attack {
        get {
            return Get<double>(BaseDataObjectKeys.attack, 1.0);
        }

        set {
            Set<double>(BaseDataObjectKeys.attack, value);
        }
    }

    public virtual double defense {
        get {
            return Get<double>(BaseDataObjectKeys.defense, 1.0);
        }

        set {
            Set<double>(BaseDataObjectKeys.defense, value);
        }
    }

    public virtual double health {
        get {
            return Get<double>(BaseDataObjectKeys.health, 1.0);
        }

        set {
            Set<double>(BaseDataObjectKeys.health, value);
        }
    }

    public virtual double energy {
        get {
            return Get<double>(BaseDataObjectKeys.energy, 1.0);
        }

        set {
            Set<double>(BaseDataObjectKeys.energy, value);
        }
    }

    public virtual double jump {
        get {
            return Get<double>(BaseDataObjectKeys.jump, 1.0);
        }

        set {
            Set<double>(BaseDataObjectKeys.jump, value);
        }
    }

    public virtual double fly {
        get {
            return Get<double>(BaseDataObjectKeys.fly, 1.0);
        }

        set {
            Set<double>(BaseDataObjectKeys.fly, value);
        }
    }

    public virtual double boost {
        get {
            return Get<double>(BaseDataObjectKeys.boost, 1.0);
        }

        set {
            Set<double>(BaseDataObjectKeys.boost, value);
        }
    }

    public virtual double attack_speed {
        get {
            return Get<double>(BaseDataObjectKeys.attack_speed, 1.0);
        }

        set {
            Set<double>(BaseDataObjectKeys.attack_speed, value);
        }
    }

    public virtual double recharge_speed {
        get {
            return Get<double>(BaseDataObjectKeys.recharge_speed, 1.0);
        }

        set {
            Set<double>(BaseDataObjectKeys.recharge_speed, value);
        }
    }

    public virtual double upgrades_applied {
        get {
            return Get<double>(BaseDataObjectKeys.upgrades_applied, 0.0);
        }

        set {
            Set<double>(BaseDataObjectKeys.upgrades_applied, value);
        }
    }

    public virtual double upgrades {
        get {
            return Get<double>(BaseDataObjectKeys.upgrades, 0.0);
        }

        set {
            Set<double>(BaseDataObjectKeys.upgrades, value);
        }
    }

    public virtual double xp {
        get {
            return Get<double>(BaseDataObjectKeys.xp, 0.0);
        }

        set {
            Set<double>(BaseDataObjectKeys.xp, value);
        }
    }

    public virtual double level {
        get {
            return Get<double>(BaseDataObjectKeys.level, 1.0);
        }

        set {
            Set<double>(BaseDataObjectKeys.level, value);
        }
    }

    public virtual double power {
        get {
            return Get<double>(BaseDataObjectKeys.power, 1.0);
        }

        set {
            Set<double>(BaseDataObjectKeys.power, value);
        }
    }

    public virtual double currency {
        get {
            return Get<double>(BaseDataObjectKeys.currency, 0.0);
        }

        set {
            Set<double>(BaseDataObjectKeys.currency, value);
        }
    }

    public virtual string data {
        get {
            return Get<string>(BaseDataObjectKeys.data);
        }

        set {
            Set<string>(BaseDataObjectKeys.data, value);
        }
    }
}

public class GameDataItemRPGAttributes {
    public static string prefix = "game-item-rpg-";
    public static string upgrades_applied = prefix + "upgrades_applied";
    public static string upgrades = prefix + "upgrades";
    public static string speed = prefix + "speed";
    public static string scale = prefix + "scale";
    public static string attack = prefix + "attack";
    public static string defense = prefix + "defense";
    public static string health = prefix + "health";
    public static string jump = prefix + "jump";
    public static string fly = prefix + "fly";
    public static string boost = prefix + "boost";
    public static string energy = prefix + "energy";
    public static string attack_speed = prefix + "attack_speed";
    public static string recharge_speed = prefix + "recharge_speed";
    public static string xp = prefix + "xp";
    public static string level = prefix + "level";
    public static string currency = prefix + "currency";
    public static string data = prefix + "data";
}

public class GameProfilePlayerProgressItem : DataObjectItem {

    public GameProfilePlayerProgressItem() {
        Reset();
    }

    public override void Reset() {
        attributes = new Dictionary<string, DataAttribute>();
    }

    // ----------------------------------------------------------
    // RPG - Player Character specific

    // -------------
    // XP

    public virtual double AddGamePlayerProgressXP(double val) {

        //LogUtil.Log("AddGamePlayerProgressXP:val:" + val);
        double v = GetGamePlayerProgressXP();

        v += val;
        //LogUtil.Log("AddGamePlayerProgressXP:v:" + v);
        SetGamePlayerProgressXP(v);
        return v;
    }

    public virtual double SubtractGamePlayerProgressXP(double val) {
        return AddGamePlayerProgressXP(-val);
    }
 
    public double GetGamePlayerProgressXP() {
        return GetGamePlayerProgressXP(10.0);
    }
 
    public double GetGamePlayerProgressXP(double defaultValue) {
        //double attValue = defaultValue;
        if (CheckIfAttributeExists(BaseGameProfileRPGAttributes.ATT_PROGRESS_XP))
            return GetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_XP);
        return defaultValue;
    }
 
    public void SetGamePlayerProgressXP(double attValue) {
        //LogUtil.Log("SetGamePlayerProgressXP:attValue:" + attValue);
        SetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_XP, attValue);
    }

    // -------------
    // LEVEL

    public virtual double AddGamePlayerProgressLevel(double val) {
        double v = GetGamePlayerProgressLevel();
        v += val;
        SetGamePlayerProgressLevel(v);
        return v;
    }

    public virtual double SubtractGamePlayerProgressLevel(double val) {
        return AddGamePlayerProgressLevel(-val);
    }
 
    public double GetGamePlayerProgressLevel() {
        return GetGamePlayerProgressLevel(0.0);
    }
 
    public double GetGamePlayerProgressLevel(double defaultValue) {
     
        //double attValue = defaultValue;
        if (CheckIfAttributeExists(BaseGameProfileRPGAttributes.ATT_PROGRESS_LEVEL))
            return GetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_LEVEL);
        return defaultValue;
    }
 
    public void SetGamePlayerProgressLevel(double attValue) {
        SetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_LEVEL, attValue);
    }

    // -------------
    // HEALTH

    public virtual double AddGamePlayerProgressHealth(double val) {
        double v = GetGamePlayerProgressHealth();
        v += val;
        v = Mathf.Clamp((float)v, 0f, 1f);
        //LogUtil.Log("AddGamePlayerProgressHealth:val:" + val);
        SetGamePlayerProgressHealth(v);
        return v;
    }

    public virtual double SubtractGamePlayerProgressHealth(double val) {
        return AddGamePlayerProgressHealth(-val);
    }
 
    public double GetGamePlayerProgressHealth() {
        return GetGamePlayerProgressHealth(1.0);
    }
 
    public double GetGamePlayerProgressHealth(double defaultValue) {
     
        //double attValue = defaultValue;
        if (CheckIfAttributeExists(BaseGameProfileRPGAttributes.ATT_PROGRESS_HEALTH))
            return GetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_HEALTH);
        return defaultValue;
    }
 
    public void SetGamePlayerProgressHealth(double attValue) {
        SetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_HEALTH, attValue);
    }

    // -------------
    // ENERGY

    public virtual double AddGamePlayerProgressEnergy(double val) {
        double v = GetGamePlayerProgressEnergy();
        v += val;
        v = Mathf.Clamp((float)v, 0f, 1f);
        //LogUtil.Log("AddGamePlayerProgressEnergy:val:" + val);
        SetGamePlayerProgressEnergy(v);
        return v;
    }

    public virtual double SubtractGamePlayerProgressEnergy(double val) {
        return AddGamePlayerProgressEnergy(-val);
    }
 
    public double GetGamePlayerProgressEnergy() {
        return GetGamePlayerProgressEnergy(0.0);
    }
 
    public double GetGamePlayerProgressEnergy(double defaultValue) {
     
        //double attValue = defaultValue;
        if (CheckIfAttributeExists(BaseGameProfileRPGAttributes.ATT_PROGRESS_ENERGY))
            return GetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_ENERGY);
        return defaultValue;
    }
 
    public void SetGamePlayerProgressEnergy(double attValue) {
        SetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_ENERGY, attValue);
    }
}

public class GameProfileRPGItem : DataObjectItem {
 
    public GameProfileRPGItem() {
        Reset();
    }

    public override void Reset() {
        base.Reset();

        attributes = new Dictionary<string, DataAttribute>();
    }
 
    public void LoadFromGameDataItemRPG(GameDataItemRPG itemRPG) {
        SetSpeed(itemRPG.speed);
        SetAttack(itemRPG.attack);
        SetDefense(itemRPG.defense);
        SetHealth(itemRPG.health);
        SetEnergy(itemRPG.energy);
        SetAttackSpeed(itemRPG.attack_speed);
        SetRechargeSpeed(itemRPG.recharge_speed);
        SetUpgradesApplied(itemRPG.upgrades_applied);
        SetUpgrades(itemRPG.upgrades);
        SetXP(itemRPG.xp);
        SetLevel(itemRPG.level);
        SetCurrency(itemRPG.currency);
        SetJump(itemRPG.jump);
        SetFly(itemRPG.fly);
        SetData(itemRPG.data);
    }
 
    public GameDataItemRPG GetGameDataItemRPG() {
        GameDataItemRPG itemRPG = new GameDataItemRPG();
        itemRPG.speed = GetSpeed();
        itemRPG.attack = GetAttack();
        itemRPG.defense = GetDefense();
        itemRPG.health = GetHealth();
        itemRPG.energy = GetEnergy();
        itemRPG.attack_speed = GetAttackSpeed();
        itemRPG.recharge_speed = GetRechargeSpeed();
        itemRPG.upgrades_applied = GetUpgradesApplied();
        itemRPG.upgrades = GetUpgrades();
        itemRPG.xp = GetXP();
        itemRPG.level = GetLevel();
        itemRPG.currency = GetCurrency();
        itemRPG.jump = GetJump();
        itemRPG.fly = GetFly();
        itemRPG.data = GetData();
        return itemRPG;
    }
 
    // properties

    // xp

    public double GetXP() {
        return GetXP(0.0);
    }

    public double GetXP(double defaultValue) {
        //double attValue = defaultValue;
        if (CheckIfAttributeExists(GameDataItemRPGAttributes.xp))
            return GetAttributeDoubleValue(GameDataItemRPGAttributes.xp);
        return defaultValue;
    }

    public void SetXP(double val) {
        SetAttributeDoubleValue(GameDataItemRPGAttributes.xp, val);
    }

    // level

    public double GetLevel() {
        return GetLevel(0.0);
    }

    public double GetLevel(double defaultValue) {
        //double attValue = defaultValue;
        if (CheckIfAttributeExists(GameDataItemRPGAttributes.level))
            return GetAttributeDoubleValue(GameDataItemRPGAttributes.level);
        return defaultValue;
    }

    public void SetLevel(double val) {
        SetAttributeDoubleValue(GameDataItemRPGAttributes.level, val);
    }

    // currency

    public double GetCurrency() {
        return GetCurrency(0.0);
    }

    public double GetCurrency(double defaultValue) {
        //double attValue = defaultValue;
        if (CheckIfAttributeExists(GameDataItemRPGAttributes.xp))
            return GetAttributeDoubleValue(GameDataItemRPGAttributes.xp);
        return defaultValue;
    }

    public void SetCurrency(double val) {
        SetAttributeDoubleValue(GameDataItemRPGAttributes.xp, val);
    }
 
    // upgrades_applied
 
    public double GetUpgradesApplied() {
        return GetUpgradesApplied(0.0);
    }
 
    public double GetUpgradesApplied(double defaultValue) {      
        //double attValue = defaultValue;
        if (CheckIfAttributeExists(GameDataItemRPGAttributes.upgrades_applied))
            return GetAttributeDoubleValue(GameDataItemRPGAttributes.upgrades_applied);
        return defaultValue;
    }
     
    public void SetUpgradesApplied(double val) {
        SetAttributeDoubleValue(GameDataItemRPGAttributes.upgrades_applied, val);
    }
 
    // upgrades
 
    public double GetUpgrades() {
        return GetUpgrades(0.0);
    }
 
    public double GetUpgrades(double defaultValue) {     
        //double attValue = defaultValue;
        if (CheckIfAttributeExists(GameDataItemRPGAttributes.upgrades))
            return GetAttributeDoubleValue(GameDataItemRPGAttributes.upgrades);
        return defaultValue;
    }
     
    public void SetUpgrades(double val) {
        SetAttributeDoubleValue(GameDataItemRPGAttributes.upgrades, val);
    }

    // jump

    public double GetJump() {
        return GetJump(0.1);
    }

    public double GetJump(double defaultValue) {
        //double attValue = defaultValue;
        if (CheckIfAttributeExists(GameDataItemRPGAttributes.jump))
            return GetAttributeDoubleValue(GameDataItemRPGAttributes.jump);
        return defaultValue;
    }

    public void SetJump(double val) {
        SetAttributeDoubleValue(GameDataItemRPGAttributes.jump, val);
    }

    // fly

    public double GetFly() {
        return GetFly(0.1);
    }

    public double GetFly(double defaultValue) {
        //double attValue = defaultValue;
        if (CheckIfAttributeExists(GameDataItemRPGAttributes.fly))
            return GetAttributeDoubleValue(GameDataItemRPGAttributes.fly);
        return defaultValue;
    }

    public void SetFly(double val) {
        SetAttributeDoubleValue(GameDataItemRPGAttributes.fly, val);
    }
 
    // speed
 
    public double GetSpeed() {
        return GetSpeed(0.1);
    }
 
    public double GetSpeed(double defaultValue) {        
        //double attValue = defaultValue;
        if (CheckIfAttributeExists(GameDataItemRPGAttributes.speed))
            return GetAttributeDoubleValue(GameDataItemRPGAttributes.speed);
        return defaultValue;
    }
     
    public void SetSpeed(double val) {
        SetAttributeDoubleValue(GameDataItemRPGAttributes.speed, val);
    }

    // scale
    
    public double GetScale() {
        return GetScale(1.0);
    }
    
    public double GetScale(double defaultValue) {        
        //double attValue = defaultValue;
        if (CheckIfAttributeExists(GameDataItemRPGAttributes.scale))
            return GetAttributeDoubleValue(GameDataItemRPGAttributes.scale);
        return defaultValue;
    }
    
    public void SetScale(double val) {
        SetAttributeDoubleValue(GameDataItemRPGAttributes.scale, val);
    }
 
    // attack
 
    public double GetAttack() {
        return GetAttack(0.1);
    }
 
    public double GetAttack(double defaultValue) {       
        //double attValue = defaultValue;
        if (CheckIfAttributeExists(GameDataItemRPGAttributes.attack))
            return GetAttributeDoubleValue(GameDataItemRPGAttributes.attack);
        return defaultValue;
    }
     
    public void SetAttack(double val) {
        SetAttributeDoubleValue(GameDataItemRPGAttributes.attack, val);
    }    
 
    // defense
     
    public double GetDefense() {
        return GetDefense(0.1);
    }
 
    public double GetDefense(double defaultValue) {      
        //double attValue = defaultValue;
        if (CheckIfAttributeExists(GameDataItemRPGAttributes.defense))
            return GetAttributeDoubleValue(GameDataItemRPGAttributes.defense);
        return defaultValue;
    }
     
    public void SetDefense(double val) {
        SetAttributeDoubleValue(GameDataItemRPGAttributes.defense, val);
    }
 
    // health
     
    public double GetHealth() {
        return GetHealth(0.1);
    }
 
    public double GetHealth(double defaultValue) {       
        //double attValue = defaultValue;
        if (CheckIfAttributeExists(GameDataItemRPGAttributes.health))
            return GetAttributeDoubleValue(GameDataItemRPGAttributes.health);
        return defaultValue;
    }
     
    public void SetHealth(double val) {
        SetAttributeDoubleValue(GameDataItemRPGAttributes.health, val);
    }
 
    // energy
 
    public double GetEnergy() {
        return GetEnergy(0.1);
    }
 
    public double GetEnergy(double defaultValue) {       
        //double attValue = defaultValue;
        if (CheckIfAttributeExists(GameDataItemRPGAttributes.energy))
            return GetAttributeDoubleValue(GameDataItemRPGAttributes.energy);
        return defaultValue;
    }
     
    public void SetEnergy(double val) {
        SetAttributeDoubleValue(GameDataItemRPGAttributes.energy, val);
    }
 
    // attack_speed
 
    public double GetAttackSpeed() {
        return GetAttackSpeed(0.1);
    }
 
    public double GetAttackSpeed(double defaultValue) {      
        //double attValue = defaultValue;
        if (CheckIfAttributeExists(GameDataItemRPGAttributes.attack_speed))
            return GetAttributeDoubleValue(GameDataItemRPGAttributes.attack_speed);
        return defaultValue;
    }
 
    public void SetAttackSpeed(double val) {
        SetAttributeDoubleValue(GameDataItemRPGAttributes.attack_speed, val);
    }
 
    // recharge_speed
 
    public double GetRechargeSpeed() {
        return GetRechargeSpeed(0.1);
    }
 
    public double GetRechargeSpeed(double defaultValue) {        
        //double attValue = defaultValue;
        if (CheckIfAttributeExists(GameDataItemRPGAttributes.recharge_speed))
            return GetAttributeDoubleValue(GameDataItemRPGAttributes.recharge_speed);
        return defaultValue;
    }
     
    public void SetRechargeSpeed(double val) {
        SetAttributeDoubleValue(GameDataItemRPGAttributes.recharge_speed, val);
    }
 
    // data
 
    public string GetData() {
        return GetData("");
    }
 
    public string GetData(string defaultValue) {     
        //string attValue = defaultValue;
        if (CheckIfAttributeExists(GameDataItemRPGAttributes.data))
            return GetAttributeStringValue(GameDataItemRPGAttributes.data);
        return defaultValue;
    }
         
    public void SetData(string val) {
        SetAttributeStringValue(GameDataItemRPGAttributes.data, val);
    }    
}

public class GameProfileCustomPreset {
    public string code = "";
    public string name = "";
    public GameProfileCustomItem customItem = new GameProfileCustomItem();
}

public class GameProfileCustomPresets {

    public List<GameProfileCustomPreset> presets;

    public GameProfileCustomPresets() {
        Reset();
    }

    public void Reset() {
        presets = new List<GameProfileCustomPreset>();
    }

    public void SetPresetColorKey(string code, string name, string key, Color color) {
                
        GameProfileCustomPreset preset = new GameProfileCustomPreset();
        bool found = false;

        for (int i = 0; i < presets.Count; i++) {            
            if (presets[i].code.ToLower() == code.ToLower()) {
                presets[i].customItem.SetCustomColor(key, color);
                found = true;
                //preset = presets[i];
                //break;
                return;
            }
        }

        preset.code = code;
        preset.name = name;
        preset.customItem.SetCustomColor(key, color);

        if (!found) {
            presets.Add(preset);
        }
    }

    public void SetPresetColor(GameProfileCustomPreset preset) {

        bool found = false;
        
        for (int i = 0; i < presets.Count; i++) {            
            if (presets[i].code.ToLower() == preset.code.ToLower()) {
                presets[i] = preset;
                found = true;
                return;
            }
        }
        
        if (!found) {
            presets.Add(preset);
        }
    
    }
}

public class CustomColorPresets {
    public List<CustomColorItem> items = new List<CustomColorItem>();
    
    public bool Has(string code) {
        return items.Exists(u => u.colorCode == code);
    }

    public CustomColorItem Get(string code) {
        return items.Find(u => u.colorCode == code);
    }

    public void Set(CustomColorItem itemTo) {
    
        bool found = false;

        for (int i = 0; i < items.Count; i++) {
            if (items[i].colorCode == itemTo.colorCode) {
                
                items[i].a = itemTo.a;
                items[i].attributes = itemTo.attributes;
                items[i].b = itemTo.b;
                items[i].colorCode = itemTo.colorCode;
                items[i].g = itemTo.g;
                items[i].r = itemTo.r;

                found = true;
                break;
            }
        }
        
        if (!found && itemTo.colorCode != "default") {
            items.Add(itemTo);
        }
    }
}

public class CustomTexturePresets {
    public List<CustomTextureItem> items = new List<CustomTextureItem>();
    
    public bool Has(string code) {
        return items.Exists(u => u.textureCode == code);
    }
    
    public CustomTextureItem Get(string code) {
        return items.Find(u => u.textureCode == code);
    }
    
    public void Set(CustomTextureItem itemTo) {
        
        bool found = false;
        
        for (int i = 0; i < items.Count; i++) {
            if (items[i].textureCode == itemTo.textureCode) {

                items[i].attributes = itemTo.attributes;
                items[i].textureCode = itemTo.textureCode;
                items[i].textureName = itemTo.textureName;
                
                found = true;
                break;
            }
        }
        
        if (!found && itemTo.textureCode != "default") {
            items.Add(itemTo);
        }
    }
}

public class GameProfileCustomItem : DataObject {

    public GameProfileCustomItem() {
        Reset();
    }

    public override void Reset() {
        base.Reset();

        color_presets = new CustomColorPresets();
        texture_presets = new CustomTexturePresets();
    }

    public bool HasData() {
        if (attributes.Count > 0 
            || color_presets.items.Count > 0
            || texture_presets.items.Count > 0) {
            return true;
        }
        return false;
    }
    
    public virtual string current_texture_preset {
        get {
            return Get<string>(BaseDataObjectKeys.current_texture_preset);
        }
        
        set {
            Set(BaseDataObjectKeys.current_texture_preset, value);
        }
    }

    public virtual string current_color_preset {
        get {
            return Get<string>(BaseDataObjectKeys.current_color_preset);
        }
        
        set {
            Set(BaseDataObjectKeys.current_color_preset, value);
        }
    }

    public virtual CustomColorPresets color_presets {
        get {
            return Get<CustomColorPresets>(BaseDataObjectKeys.color_presets);
        }
        
        set {
            Set(BaseDataObjectKeys.color_presets, value);
        }
    }

    public virtual CustomTexturePresets texture_presets {
        get {
            return Get<CustomTexturePresets>(BaseDataObjectKeys.texture_presets);
        }
        
        set {
            Set(BaseDataObjectKeys.texture_presets, value);
        }
    }

    // PRESET CODES

    public virtual void SetCustomTexturePreset(string texturePresetCode) {
        current_texture_preset = texturePresetCode;
    }
    
    public virtual void SetCustomColorPreset(string colorPresetCode) {
        current_color_preset = colorPresetCode;
    }
    
    public virtual string GetCustomTexturePreset() {
        return current_texture_preset;
    }
    
    public virtual string GetCustomColorPreset() {
        return current_color_preset;
    }

    // ITEMS

    public virtual void SetCustomTexture(string key, string textureName) {
        CustomTextureItem item = GetCustomTextureItem(key);
        item.textureCode = key;
        item.textureName = textureName;
        SetCustomTextureItem(key, item);
    }

    public virtual void SetCustomColor(string key, Color color) {
        CustomColorItem colorItem = GetCustomColorItem(key);
        colorItem.FromColor(color);
        colorItem.colorCode = key;
        SetCustomColorItem(key, colorItem);
    }

    public virtual void SetCustomTextureItem(string key, CustomTextureItem itemTo) {
      
        /*
        DataObject item = texture_presets;
        item.Set(key, itemTo);
        texture_presets = item;
        */
        CustomTexturePresets items = texture_presets;        
        if (items != null) {  
            items.Set(itemTo);
        }
        texture_presets = items;
    }

    public virtual void SetCustomColorItem(string key, CustomColorItem itemTo) {
        /*
        DataObject item = color_presets;
        item.Set(key, itemTo);
        color_presets = item;
        */

        CustomColorPresets items = color_presets;        
        if (items != null) {  
            items.Set(itemTo);
        }
        color_presets = items;
    }

    public virtual string GetCustomTexture(string key) {
        return GetCustomTextureItem(key).textureName;
    }

    public virtual Color GetCustomColor(string key) {
        return GetCustomColorItem(key).GetColor();
    }

    public virtual CustomTextureItem GetCustomTextureItem(string key) {
        CustomTextureItem item = new CustomTextureItem();

        /*
        item = texture_presets.Get<CustomTextureItem>(key);

        if(item == null) {
            item = new CustomTextureItem();
        }

        */

        CustomTexturePresets items = texture_presets;
        
        if (items != null) {            
            if (items.Has(key)) {
                item = items.Get(key);
            }
        }

        return item;
    }

    public virtual CustomColorItem GetCustomColorItem(string key) {
        CustomColorItem item = new CustomColorItem();

        /*
        item = color_presets.Get<CustomColorItem>(key);
        
        if(item == null) {
            item = new CustomColorItem();
        }
        */

        CustomColorPresets items = color_presets;

        if (items != null) {            
            if (items.Has(key)) {
                item = items.Get(key);
            }
        }
                
        return item;
    }

    // ----------------------------------------------------------
    // color items


    /*

public class CustomPlayerColorsRunner : DataObject {
    public string colorCode;
    public string colorDisplayName;

    public CustomColorItem helmetColor;
    public CustomColorItem helmetFacemaskColor;
    public CustomColorItem helmetHighlightColor;
    public CustomColorItem jerseyColor;
    public CustomColorItem jerseyHighlightColor;
    public CustomColorItem pantsColor;
 
    public CustomColorItem extra1Color;
    public CustomColorItem extra2Color;
    public CustomColorItem extra3Color;

    public CustomPlayerColorsRunner() {
        Reset();
    }

    public override void Reset() {
        base.Reset();

        colorCode = "default";
        colorDisplayName = "Default";
     //SetMaterialColor name:helmet-facemask color:RGBA(0.838, 1.000, 0.595, 1.000)
     //SetMaterialColor name:helmet-main color:RGBA(1.000, 0.189, 0.192, 1.000)


        helmetColor = new CustomColorItem();
     helmetColor.FromColor(new Color(0.979f, 0.943f, 0.938f, 1.000f));
     
        helmetFacemaskColor = new CustomColorItem();
     helmetFacemaskColor.FromColor(new Color(0.973f, 0.974f, 0.991f, 1.000f));
     
        helmetHighlightColor = new CustomColorItem();
     helmetHighlightColor.FromColor(new Color(0.442f, 0.114f, 0.067f, 1.000f));
     
        jerseyColor = new CustomColorItem();
     jerseyColor.FromColor(new Color(0.448f, 0.093f, 0.042f, 1.000f));
     
        jerseyHighlightColor = new CustomColorItem();
     jerseyHighlightColor.FromColor(new Color(0.974f, 0.955f, 0.952f, 1.000f));
     
        pantsColor = new CustomColorItem();
     pantsColor.FromColor(new Color(0.979f, 0.943f, 0.938f, 1.000f));
     
        extra1Color = new CustomColorItem();
     extra1Color.FromColor(new Color(1.000f, 0.189f, 0.192f, 1.000f));
     
        extra2Color = new CustomColorItem();
     extra2Color.FromColor(new Color(1.000f, 0.189f, 0.192f, 1.000f));
     
        extra3Color = new CustomColorItem();
     extra3Color.FromColor(new Color(1.000f, 0.189f, 0.192f, 1.000f));
    }

    public override string ToString() {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append(String.Format("CustomPlayerColors name: {0}", colorDisplayName));
        sb.Append(String.Format("\r\n\r\n helmetColor: {0}", helmetColor.ToString()));
        sb.Append(String.Format("\r\n\r\n helmetFacemaskColor: {0}", helmetFacemaskColor.ToString()));
        sb.Append(String.Format("\r\n\r\n helmetHighlightColor: {0}", helmetHighlightColor.ToString()));
        sb.Append(String.Format("\r\n\r\n jerseyColor: {0}", jerseyColor.ToString()));
        sb.Append(String.Format("\r\n\r\n jerseyHighlightColor: {0}", jerseyHighlightColor.ToString()));
        sb.Append(String.Format("\r\n\r\n pantsColor: {0}", pantsColor.ToString()));
        sb.Append(String.Format("\r\n\r\n extra1Color: {0}", extra1Color.ToString()));
        sb.Append(String.Format("\r\n\r\n extra2Color: {0}", extra2Color.ToString()));
        sb.Append(String.Format("\r\n\r\n extra3Color: {0}", extra3Color.ToString()));
        return sb.ToString();
    }
}
*/
}

public class BaseGameProfileRPG : DataObject {
    // BE CAREFUL adding properties as they will cause a need for a profile conversion
    // Best way to add items to the profile is the GetAttribute and SetAttribute class as 
    // that stores as a generic DataAttribute class.  Booleans, strings, objects, serialized json objects etc
    // all work well and cause no need to convert profile on updates. 

    public BaseGameProfileRPG() {
        //Reset();
    }

    public override void Reset() {
        base.Reset();
        //username = ProfileConfigs.defaultPlayerName;
    }

    // ----------------------------------------------------------
    // UPGRADES_APPLIED

    public virtual double GetUpgradesApplied() {
        return GetUpgradesApplied(0.0);
    }

    public virtual double GetUpgradesApplied(double defaultValue) {
        //double attValue = defaultValue;
        if(CheckIfAttributeExists(GameDataItemRPGAttributes.upgrades_applied))
            return GetAttributeDoubleValue(GameDataItemRPGAttributes.upgrades_applied);
        return defaultValue;
    }

    public virtual void SetUpgradesApplied(double val) {
        SetAttributeDoubleValue(GameDataItemRPGAttributes.upgrades_applied, val);
    }

    // ----------------------------------------------------------
    // UPGRADES

    public virtual double AddUpgrades(double val) {
        double upgrades = GetUpgrades();
        upgrades += val;
        SetUpgrades(upgrades);
        return upgrades;
    }

    public virtual double SubtractUpgrades(double val) {
        double upgrades = GetUpgrades();
        if(val > 0) {
            val = -val;
        }
        upgrades += val;
        SetUpgrades(upgrades);
        return upgrades;
    }

    public virtual double GetUpgrades() {
        return GetUpgrades(3.0);
    }

    public virtual double GetUpgrades(double defaultValue) {
        //double attValue = defaultValue;
        if(CheckIfAttributeExists(GameDataItemRPGAttributes.upgrades))
            return GetAttributeDoubleValue(GameDataItemRPGAttributes.upgrades);
        return defaultValue;
    }

    public virtual void SetUpgrades(double val) {
        SetAttributeDoubleValue(GameDataItemRPGAttributes.upgrades, val);
    }

    // ----------------------------------------------------------
    // CUSTOMIZATIONS

    public virtual void SetValue(string code, object value) {
        DataAttribute att = new DataAttribute();
        att.val = value;
        att.code = code;
        att.name = "";
        att.type = "bool";
        att.otype = "rpg";
        SetAttribute(att);
    }

    public virtual List<DataAttribute> GetList() {
        return GetAttributesList("rpg");
    }

    // ----------------------------------------------------------
    // CURRENCY

    public virtual double AddCurrency(double val) {
        double v = GetCurrency();
        v += val;
        SetCurrency(v);
        return v;
    }

    public virtual double SubtractCurrency(double val) {
        return AddCurrency(-val);
    }

    public virtual double GetCurrency() {
        return GetCurrency(10.0);
    }

    public virtual double GetCurrency(double defaultValue) {
        //double attValue = defaultValue;
        if(CheckIfAttributeExists(BaseGameProfileAttributes.ATT_CURRENCY))
            return GetAttributeDoubleValue(BaseGameProfileAttributes.ATT_CURRENCY);
        return defaultValue;
    }

    public virtual void SetCurrency(double attValue) {
        SetAttributeDoubleValue(BaseGameProfileAttributes.ATT_CURRENCY, attValue);
    }

    public virtual double GetGamePlayerProgressCurrency() {
        return GetGamePlayerProgressCurrency(10.0);
    }

    public virtual double GetGamePlayerProgressCurrency(double defaultValue) {
        //double attValue = defaultValue;
        if(CheckIfAttributeExists(BaseGameProfileRPGAttributes.ATT_PROGRESS_CURRENCY))
            return GetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_CURRENCY);
        return defaultValue;
    }

    public virtual void SetGamePlayerProgressCurrency(double attValue) {
        SetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_CURRENCY, attValue);
    }

    // ----------------------------------------------------------
    // RPG - Player specific

    // -------------
    // XP

    public virtual double AddGamePlayerProgressXP(double val) {
        double v = GetGamePlayerProgressXP();
        v += val;
        SetGamePlayerProgressXP(v);
        return v;
    }

    public virtual double SubtractGamePlayerProgressXP(double val) {
        return AddGamePlayerProgressXP(-val);
    }

    public double GetGamePlayerProgressXP() {
        return GetGamePlayerProgressXP(10.0);
    }

    public double GetGamePlayerProgressXP(double defaultValue) {
        //double attValue = defaultValue;
        if(CheckIfAttributeExists(BaseGameProfileRPGAttributes.ATT_PROGRESS_XP))
            return GetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_XP);
        return defaultValue;
    }

    public void SetGamePlayerProgressXP(double attValue) {
        SetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_XP, attValue);
    }

    // -------------
    // LEVEL

    public virtual double AddGamePlayerProgressLevel(double val) {
        double v = GetGamePlayerProgressLevel();
        v += val;
        SetGamePlayerProgressLevel(v);
        return v;
    }

    public virtual double SubtractGamePlayerProgressLevel(double val) {
        return AddGamePlayerProgressLevel(-val);
    }

    public double GetGamePlayerProgressLevel() {
        return GetGamePlayerProgressLevel(0.0);
    }

    public double GetGamePlayerProgressLevel(double defaultValue) {

        //double attValue = defaultValue;
        if(CheckIfAttributeExists(BaseGameProfileRPGAttributes.ATT_PROGRESS_LEVEL))
            return GetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_LEVEL);
        return defaultValue;
    }

    public void SetGamePlayerProgressLevel(double attValue) {
        SetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_LEVEL, attValue);
    }

    // -------------
    // HEALTH

    public virtual double AddGamePlayerProgressHealth(double val) {
        double v = GetGamePlayerProgressHealth();
        v += val;
        v = Mathf.Clamp((float)v, 0, 1);
        SetGamePlayerProgressHealth(v);
        return v;
    }

    public virtual double SubtractGamePlayerProgressHealth(double val) {
        return AddGamePlayerProgressHealth(-val);
    }

    public double GetGamePlayerProgressHealth() {
        return GetGamePlayerProgressHealth(1.0);
    }

    public double GetGamePlayerProgressHealth(double defaultValue) {

        //double attValue = defaultValue;
        if(CheckIfAttributeExists(BaseGameProfileRPGAttributes.ATT_PROGRESS_HEALTH))
            return GetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_HEALTH);
        return defaultValue;
    }

    public void SetGamePlayerProgressHealth(double attValue) {
        SetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_HEALTH, attValue);
    }

    // -------------
    // ENERGY

    public virtual double AddGamePlayerProgressEnergy(double val) {

        double v = GetGamePlayerProgressEnergy();

        v += val;
        v = Mathf.Clamp((float)v, 0, 1);

        SetGamePlayerProgressEnergy(v);

        return v;
    }

    public virtual double SubtractGamePlayerProgressEnergy(double val) {
        return AddGamePlayerProgressEnergy(-val);
    }

    public double GetGamePlayerProgressEnergy() {
        return GetGamePlayerProgressEnergy(0.0);
    }

    public double GetGamePlayerProgressEnergy(double defaultValue) {

        //double attValue = defaultValue;
        if(CheckIfAttributeExists(BaseGameProfileRPGAttributes.ATT_PROGRESS_ENERGY))
            return GetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_ENERGY);
        return defaultValue;
    }

    public void SetGamePlayerProgressEnergy(double attValue) {
        SetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_ENERGY, attValue);
    }

    // -------------
    // POINTS

    // point-uuids

    public virtual void SetGamePlayerProgressPointData(string uuid, double points) {

        GamePlayerProgressPointDatas datas = GetGamePlayerProgressPointDatas();

        GamePlayerProgressPointData data = datas.GetGamePlayerProgressData(uuid);

        if(data != null) {

            data.collected = true;
            data.uuid = uuid;
            data.points = points;
        }
        else {

            data = new GamePlayerProgressPointData();
            data.collected = true;
            data.uuid = uuid;
            data.points = points;
        }

        datas.SetGamePlayerProgressData(uuid, data);

        SetGamePlayerProgressPointDatas(datas);
    }

    public virtual GamePlayerProgressPointData GetGamePlayerProgressPointData(string uuid) {

        GamePlayerProgressPointDatas datas = GetGamePlayerProgressPointDatas();

        GamePlayerProgressPointData data = datas.GetGamePlayerProgressData(uuid);

        return data;
    }

    public double GetGamePlayerProgressPointDataTotal() {

        GamePlayerProgressPointDatas datas = GetGamePlayerProgressPointDatas();

        double totalPoints = 0.0;

        if(datas.pointItems != null) {
            foreach(GamePlayerProgressPointData data in datas.pointItems.Values) {
                totalPoints += data.points;
            }
        }

        return totalPoints;
    }

    public void SyncGamePlayerProgressPointDataStats() {

        double totalPoints = GetGamePlayerProgressPointDataTotal();

        UpdateGamePlayerProgressPointDataStats(totalPoints);
    }

    public void UpdateGamePlayerProgressPointDataStats(double totalPoints) {

        GameProfileStatistics.Current.SetAttributeDoubleValue(
            GameProfileStatisticAttributes.ATT_TOTAL_POINTS, totalPoints);
    }

    public virtual bool CheckGamePlayerProgressPointData(string uuid) {

        GamePlayerProgressPointData data = GetGamePlayerProgressPointData(uuid);

        if(!string.IsNullOrEmpty(data.uuid)) {
            return true;
        }
        return false;
    }

    public virtual void SetGamePlayerProgressPointDatas(GamePlayerProgressPointDatas points) {

        string pointsText = points.ToJson();//JsonMapper.ToJson(points);

        LogUtil.Log("SetGamePlayerProgressPointDatas: " + pointsText);

        SetAttributeStringValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_POINTS, pointsText);

        SyncGamePlayerProgressPointDataStats();
    }

    public virtual GamePlayerProgressPointDatas GetGamePlayerProgressPointDatas() {

        GamePlayerProgressPointDatas points = new GamePlayerProgressPointDatas();

        string key = BaseGameProfileRPGAttributes.ATT_PROGRESS_POINTS;

        if(!CheckIfAttributeExists(key)) {
            // add default colors
            SetGamePlayerProgressPointDatas(points);
            Messenger.Broadcast(BaseGameProfileMessages.ProfileShouldBeSaved);
        }

        string json = GetAttributeStringValue(key);

        if(!string.IsNullOrEmpty(json)) {
            try {
                LogUtil.Log("GetGamePlayerProgressPoints: " + json);
                points = json.FromJson<GamePlayerProgressPointDatas>();// JsonMapper.ToObject<GamePlayerProgressPointDatas>(json);
            }
            catch(Exception e) {
                points = new GamePlayerProgressPointDatas();
                LogUtil.Log(e);
            }
        }
        return points;
    }
}