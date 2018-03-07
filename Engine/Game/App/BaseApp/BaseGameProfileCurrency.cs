using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

// using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public class BaseGameProfileCurrencyAttributes {
 
    // RPG   
    public static string ATT_PROGRESS_CURRENCY = "progress-currency";
}

public class BaseGameProfileCurrencies {
    private static volatile BaseGameProfileCurrency current;
    private static volatile BaseGameProfileCurrencies instance;
    private static System.Object syncRoot = new System.Object();
 
    public static BaseGameProfileCurrency BaseCurrent {
        get {
            if (current == null) {
                lock (syncRoot) {
                    if (current == null) 
                        current = new BaseGameProfileCurrency();
                }
            }
 
            return current;
        }
        set {
            current = value;
        }
    }
     
    public static BaseGameProfileCurrencies BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null) 
                        instance = new BaseGameProfileCurrencies();
                }
            }
 
            return instance;
        }
    }

}

public class BaseGameProfileCurrency : DataObject {
    // BE CAREFUL adding properties as they will cause a need for a profile conversion
    // Best way to add items to the profile is the GetAttribute and SetAttribute class as 
    // that stores as a generic DataAttribute class.  Booleans, strings, objects, serialized json objects etc
    // all work well and cause no need to convert profile on updates. 
     
    public BaseGameProfileCurrency() {
        //Reset();
    }
 
    public override void Reset() {
        base.Reset();
        //username = ProfileConfigs.defaultPlayerName;
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
        return GetAttributesList("currency");
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
        if (CheckIfAttributeExists(BaseGameProfileAttributes.ATT_CURRENCY))
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
        if (CheckIfAttributeExists(BaseGameProfileCurrencyAttributes.ATT_PROGRESS_CURRENCY))
            attValue = GetAttributeDoubleValue(BaseGameProfileCurrencyAttributes.ATT_PROGRESS_CURRENCY);
        return attValue;
    }

    public virtual void SetGamePlayerProgressCurrency(double attValue) {
        SetAttributeDoubleValue(BaseGameProfileCurrencyAttributes.ATT_PROGRESS_CURRENCY, attValue);
    } 
}