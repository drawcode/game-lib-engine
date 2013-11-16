using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public class BaseGameProfileRPGAttributes {
 
    // RPG   
    public static string ATT_PROGRESS_CURRENCY = "progress-currency";
    public static string ATT_PROGRESS_XP = "progress-xp";
    public static string ATT_PROGRESS_HEALTH = "progress-health";
    public static string ATT_PROGRESS_ENERGY = "progress-energy";
    public static string ATT_PROGRESS_LEVEL = "progress-level";
    public static string ATT_PROGRESS_UPGRADES_APPLIED = "progress-upgrades-applied";
    public static string ATT_PROGRESS_UPGRADES = "progress-upgrades";
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

public class GameItemRPG : DataObject {
    public double speed = 0.1;
    public double attack = 0.1;
    public double defense = 0.1;
    public double health = 0.1;
    public double energy = 0.1;
    public double jump = 0.1;
    public double fly = 0.1;
    public double boost = 0.1;
    public double attack_speed = 0.1;
    public double recharge_speed = 0.1;
    public double upgrades_applied = 0.0;
    public double upgrades = 0.0;
    public double xp = 10.0;
    public double level = 1.0;
    public double currency = 1.0;
    public string data = ""; 
}

public class GameItemRPGAttributes {
    public static string prefix = "game-item-rpg-";
    public static string upgrades_applied = prefix + "upgrades_applied";
    public static string upgrades = prefix + "upgrades";
    public static string speed = prefix + "speed";
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

public class GameProfilePlayerProgressItem : DataObject {

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

        Debug.Log("AddGamePlayerProgressXP:val:" + val);
        double v = GetGamePlayerProgressXP();

        v += val;
        Debug.Log("AddGamePlayerProgressXP:v:" + v);
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
        double attValue = defaultValue;
        if(CheckIfAttributeExists(BaseGameProfileRPGAttributes.ATT_PROGRESS_XP))
            attValue = GetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_XP);
        return attValue;
    }
 
    public void SetGamePlayerProgressXP(double attValue) {
        Debug.Log("SetGamePlayerProgressXP:attValue:" + attValue);
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
     
        double attValue = defaultValue;
        if(CheckIfAttributeExists(BaseGameProfileRPGAttributes.ATT_PROGRESS_LEVEL))
            attValue = GetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_LEVEL);
        return attValue;
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
        Debug.Log("AddGamePlayerProgressHealth:val:" + val);
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
     
        double attValue = defaultValue;
        if(CheckIfAttributeExists(BaseGameProfileRPGAttributes.ATT_PROGRESS_HEALTH))
            attValue = GetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_HEALTH);
        return attValue;
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
        Debug.Log("AddGamePlayerProgressEnergy:val:" + val);
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
     
        double attValue = defaultValue;
        if(CheckIfAttributeExists(BaseGameProfileRPGAttributes.ATT_PROGRESS_ENERGY))
            attValue = GetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_ENERGY);
        return attValue;
    }
 
    public void SetGamePlayerProgressEnergy(double attValue) {
        SetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_ENERGY, attValue);
    }
}

public class GameProfileRPGItem : DataObject {
 
    public GameProfileRPGItem() {
        Reset();
    }

    public override void Reset() {
        base.Reset();

      attributes = new Dictionary<string, DataAttribute>();
    }
 
    public void LoadFromGameItemRPG(GameItemRPG itemRPG) {
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
 
    public GameItemRPG GetGameItemRPG() {
        GameItemRPG itemRPG = new GameItemRPG();
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
        double attValue = defaultValue;
        if(CheckIfAttributeExists(GameItemRPGAttributes.xp))
            attValue = GetAttributeDoubleValue(GameItemRPGAttributes.xp);
        return attValue;
    }

    public void SetXP(double val) {
        SetAttributeDoubleValue(GameItemRPGAttributes.xp, val);
    }

    // level

    public double GetLevel() {
        return GetLevel(0.0);
    }

    public double GetLevel(double defaultValue) {
        double attValue = defaultValue;
        if(CheckIfAttributeExists(GameItemRPGAttributes.level))
            attValue = GetAttributeDoubleValue(GameItemRPGAttributes.level);
        return attValue;
    }

    public void SetLevel(double val) {
        SetAttributeDoubleValue(GameItemRPGAttributes.level, val);
    }

    // currency

    public double GetCurrency() {
        return GetCurrency(0.0);
    }

    public double GetCurrency(double defaultValue) {
        double attValue = defaultValue;
        if(CheckIfAttributeExists(GameItemRPGAttributes.xp))
            attValue = GetAttributeDoubleValue(GameItemRPGAttributes.xp);
        return attValue;
    }

    public void SetCurrency(double val) {
        SetAttributeDoubleValue(GameItemRPGAttributes.xp, val);
    }
 
    // upgrades_applied
 
    public double GetUpgradesApplied() {
        return GetUpgradesApplied(0.0);
    }
 
    public double GetUpgradesApplied(double defaultValue) {      
        double attValue = defaultValue;
        if(CheckIfAttributeExists(GameItemRPGAttributes.upgrades_applied))
            attValue = GetAttributeDoubleValue(GameItemRPGAttributes.upgrades_applied);
        return attValue;
    }
     
    public void SetUpgradesApplied(double val) {
        SetAttributeDoubleValue(GameItemRPGAttributes.upgrades_applied, val);
    }
 
    // upgrades
 
    public double GetUpgrades() {
        return GetUpgrades(0.0);
    }
 
    public double GetUpgrades(double defaultValue) {     
        double attValue = defaultValue;
        if(CheckIfAttributeExists(GameItemRPGAttributes.upgrades))
            attValue = GetAttributeDoubleValue(GameItemRPGAttributes.upgrades);
        return attValue;
    }
     
    public void SetUpgrades(double val) {
        SetAttributeDoubleValue(GameItemRPGAttributes.upgrades, val);
    }

    // jump

    public double GetJump() {
        return GetJump(0.1);
    }

    public double GetJump(double defaultValue) {
        double attValue = defaultValue;
        if(CheckIfAttributeExists(GameItemRPGAttributes.jump))
            attValue = GetAttributeDoubleValue(GameItemRPGAttributes.jump);
        return attValue;
    }

    public void SetJump(double val) {
        SetAttributeDoubleValue(GameItemRPGAttributes.jump, val);
    }

    // fly

    public double GetFly() {
        return GetFly(0.1);
    }

    public double GetFly(double defaultValue) {
        double attValue = defaultValue;
        if(CheckIfAttributeExists(GameItemRPGAttributes.fly))
            attValue = GetAttributeDoubleValue(GameItemRPGAttributes.fly);
        return attValue;
    }

    public void SetFly(double val) {
        SetAttributeDoubleValue(GameItemRPGAttributes.fly, val);
    }
 
    // speed
 
    public double GetSpeed() {
        return GetSpeed(0.1);
    }
 
    public double GetSpeed(double defaultValue) {        
        double attValue = defaultValue;
        if(CheckIfAttributeExists(GameItemRPGAttributes.speed))
            attValue = GetAttributeDoubleValue(GameItemRPGAttributes.speed);
        return attValue;
    }
     
    public void SetSpeed(double val) {
        SetAttributeDoubleValue(GameItemRPGAttributes.speed, val);
    }
 
    // attack
 
    public double GetAttack() {
        return GetAttack(0.1);
    }
 
    public double GetAttack(double defaultValue) {       
        double attValue = defaultValue;
        if(CheckIfAttributeExists(GameItemRPGAttributes.attack))
            attValue = GetAttributeDoubleValue(GameItemRPGAttributes.attack);
        return attValue;
    }
     
    public void SetAttack(double val) {
        SetAttributeDoubleValue(GameItemRPGAttributes.attack, val);
    }    
 
    // defense
     
    public double GetDefense() {
        return GetDefense(0.1);
    }
 
    public double GetDefense(double defaultValue) {      
        double attValue = defaultValue;
        if(CheckIfAttributeExists(GameItemRPGAttributes.defense))
            attValue = GetAttributeDoubleValue(GameItemRPGAttributes.defense);
        return attValue;
    }
     
    public void SetDefense(double val) {
        SetAttributeDoubleValue(GameItemRPGAttributes.defense, val);
    }
 
    // health
     
    public double GetHealth() {
        return GetHealth(0.1);
    }
 
    public double GetHealth(double defaultValue) {       
        double attValue = defaultValue;
        if(CheckIfAttributeExists(GameItemRPGAttributes.health))
            attValue = GetAttributeDoubleValue(GameItemRPGAttributes.health);
        return attValue;
    }
     
    public void SetHealth(double val) {
        SetAttributeDoubleValue(GameItemRPGAttributes.health, val);
    }
 
    // energy
 
    public double GetEnergy() {
        return GetEnergy(0.1);
    }
 
    public double GetEnergy(double defaultValue) {       
        double attValue = defaultValue;
        if(CheckIfAttributeExists(GameItemRPGAttributes.energy))
            attValue = GetAttributeDoubleValue(GameItemRPGAttributes.energy);
        return attValue;
    }
     
    public void SetEnergy(double val) {
        SetAttributeDoubleValue(GameItemRPGAttributes.energy, val);
    }
 
    // attack_speed
 
    public double GetAttackSpeed() {
        return GetAttackSpeed(0.1);
    }
 
    public double GetAttackSpeed(double defaultValue) {      
        double attValue = defaultValue;
        if(CheckIfAttributeExists(GameItemRPGAttributes.attack_speed))
            attValue = GetAttributeDoubleValue(GameItemRPGAttributes.attack_speed);
        return attValue;
    }
 
    public void SetAttackSpeed(double val) {
        SetAttributeDoubleValue(GameItemRPGAttributes.attack_speed, val);
    }
 
    // recharge_speed
 
    public double GetRechargeSpeed() {
        return GetRechargeSpeed(0.1);
    }
 
    public double GetRechargeSpeed(double defaultValue) {        
        double attValue = defaultValue;
        if(CheckIfAttributeExists(GameItemRPGAttributes.recharge_speed))
            attValue = GetAttributeDoubleValue(GameItemRPGAttributes.recharge_speed);
        return attValue;
    }
     
    public void SetRechargeSpeed(double val) {
        SetAttributeDoubleValue(GameItemRPGAttributes.recharge_speed, val);
    }
 
    // data
 
    public string GetData() {
        return GetData("");
    }
 
    public string GetData(string defaultValue) {     
        string attValue = defaultValue;
        if(CheckIfAttributeExists(GameItemRPGAttributes.data))
            attValue = GetAttributeStringValue(GameItemRPGAttributes.data);
        return attValue;
    }
         
    public void SetData(string val) {
        SetAttributeStringValue(GameItemRPGAttributes.data, val);
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

        for(int i = 0; i < presets.Count; i++ ) {            
            if(presets[i].code.ToLower() == code.ToLower()) {
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

        if(!found) {
            presets.Add(preset);
        }
    }


    public void SetPresetColor(GameProfileCustomPreset preset) {

        bool found = false;
        
        for(int i = 0; i < presets.Count; i++ ) {            
            if(presets[i].code.ToLower() == preset.code.ToLower()) {
                presets[i] = preset;
                found = true;
                return;
            }
        }
        
        if(!found) {
            presets.Add(preset);
        }
    
    }
}

public class GameProfileCustomItem : DataObject {

    public GameProfileCustomItem() {
        Reset();
    }

    public override void Reset() {
        base.Reset();

        attributes = new Dictionary<string, DataAttribute>();
    }

    // ----------------------------------------------------------
    // color items

    public virtual string GetCustomColorItemKey(string key) {
        string fullKey = BaseGameProfileAttributes.ATT_CUSTOM_COLOR_ITEM;
        fullKey = fullKey + "-" + key.ToLower().Trim().Replace(" ", "-");
        return fullKey;
    }

    public virtual void SetCustomColor(string colorKey, Color color) {
        CustomColorItem colorItem = GetCustomColorItem(colorKey);
        colorItem.FromColor(color);
        SetCustomColorItem(colorKey, colorItem);
    }

    public virtual void SetCustomColorItem(string colorKey, CustomColorItem colorItem) {
        string key = GetCustomColorItemKey(colorKey);
        string colorItemText = JsonMapper.ToJson(colorItem);
        LogUtil.Log("SetCustomColorItem: " + colorItemText);
        SetAttributeStringValue(key, colorItemText);
    }

    public virtual Color GetCustomColor(string colorKey) {
        return GetCustomColorItem(colorKey).GetColor();
    }

    public virtual CustomColorItem GetCustomColorItem(string colorKey) {
        CustomColorItem colorItem = new CustomColorItem();

        string key = GetCustomColorItemKey(colorKey);

        string json = GetAttributeStringValue(key);
        if(!string.IsNullOrEmpty(json)) {
            try {
                //LogUtil.Log("GetCustomColorItem: " + json);
                colorItem = JsonMapper.ToObject<CustomColorItem>(json);
            }
            catch(Exception e) {
                colorItem = new CustomColorItem();
                LogUtil.Log(e);
            }
        }
        return colorItem;
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


public class BaseGameProfileRPG : Profile {
    // BE CAREFUL adding properties as they will cause a need for a profile conversion
    // Best way to add items to the profile is the GetAttribute and SetAttribute class as 
    // that stores as a generic DataAttribute class.  Booleans, strings, objects, serialized json objects etc
    // all work well and cause no need to convert profile on updates. 
     
    public BaseGameProfileRPG() {
        Reset();
    }
 
    public override void Reset() {
        base.Reset();
        username = ProfileConfigs.defaultPlayerName;
    }

    // ----------------------------------------------------------
    // UPGRADES_APPLIED
 
    public virtual double GetUpgradesApplied() {
        return GetUpgradesApplied(0.0);
    }
 
    public virtual double GetUpgradesApplied(double defaultValue) {
        double attValue = defaultValue;
        if(CheckIfAttributeExists(GameItemRPGAttributes.upgrades_applied))
            attValue = GetAttributeDoubleValue(GameItemRPGAttributes.upgrades_applied);
        return attValue;
    }
     
    public virtual void SetUpgradesApplied(double val) {
        SetAttributeDoubleValue(GameItemRPGAttributes.upgrades_applied, val);
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
        double attValue = defaultValue;
        if(CheckIfAttributeExists(GameItemRPGAttributes.upgrades))
            attValue = GetAttributeDoubleValue(GameItemRPGAttributes.upgrades);
        return attValue;
    }
     
    public virtual void SetUpgrades(double val) {
        SetAttributeDoubleValue(GameItemRPGAttributes.upgrades, val);
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
        double attValue = defaultValue;
        if(CheckIfAttributeExists(BaseGameProfileAttributes.ATT_CURRENCY))
            attValue = GetAttributeDoubleValue(BaseGameProfileAttributes.ATT_CURRENCY);
        return attValue;
    }

    public virtual void SetCurrency(double attValue) {
        SetAttributeDoubleValue(BaseGameProfileAttributes.ATT_CURRENCY, attValue);
    }
     
    public virtual double GetGamePlayerProgressCurrency() {
        return GetGamePlayerProgressCurrency(10.0);
    }

    public virtual double GetGamePlayerProgressCurrency(double defaultValue) {
        double attValue = defaultValue;
        if(CheckIfAttributeExists(BaseGameProfileRPGAttributes.ATT_PROGRESS_CURRENCY))
            attValue = GetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_CURRENCY);
        return attValue;
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
        double attValue = defaultValue;
        if(CheckIfAttributeExists(BaseGameProfileRPGAttributes.ATT_PROGRESS_XP))
            attValue = GetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_XP);
        return attValue;
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
     
        double attValue = defaultValue;
        if(CheckIfAttributeExists(BaseGameProfileRPGAttributes.ATT_PROGRESS_LEVEL))
            attValue = GetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_LEVEL);
        return attValue;
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
     
        double attValue = defaultValue;
        if(CheckIfAttributeExists(BaseGameProfileRPGAttributes.ATT_PROGRESS_HEALTH))
            attValue = GetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_HEALTH);
        return attValue;
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
     
        double attValue = defaultValue;
        if(CheckIfAttributeExists(BaseGameProfileRPGAttributes.ATT_PROGRESS_ENERGY))
            attValue = GetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_ENERGY);
        return attValue;
    }
 
    public void SetGamePlayerProgressEnergy(double attValue) {
        SetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_ENERGY, attValue);
    }
 
 
}


