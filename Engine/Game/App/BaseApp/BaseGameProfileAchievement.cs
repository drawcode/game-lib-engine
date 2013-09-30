using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Engine.Data.Json;
using Engine.Utility;

public class BaseGameProfileAchievementAttributes {	

}	

public class BaseGameProfileAchievements {
    private static volatile BaseGameProfileAchievement current;
    private static volatile BaseGameProfileAchievements instance;
    private static object syncRoot = new Object();

    public static string DEFAULT_USERNAME = "Player";

    public static BaseGameProfileAchievement Current {
        get {
            if (current == null) {
                lock (syncRoot) {
                    if (current == null)
                        current = new BaseGameProfileAchievement();
                }
            }

            return current;
        }
        set {
            current = value;
        }
    }

    public static BaseGameProfileAchievements Instance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseGameProfileAchievements();
                }
            }

            return instance;
        }
    }

    // TODO: Common profile actions, lookup, count, etc
}

public class BaseGameProfileAchievement : Profile {

    // BE CAREFUL adding properties as they will cause a need for a profile conversion
    // Best way to add items to the profile is the GetAttribute and SetAttribute class as
    // that stores as a generic DataAttribute class.  Booleans, strings, objects, serialized json objects etc
    // all work well and cause no need to convert profile on updates.

    public BaseGameProfileAchievement() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
        username = ProfileConfigs.defaultPlayerName;
    }

    // ACHIEVEMENTS

    public virtual void SetAchievementValue(string code, object value) {
        DataAttribute att = new DataAttribute();
        att.val = value;
        att.code = code;
        att.name = "";
        att.type = "bool";
        att.otype = "achievement";
        SetAttribute(att);
    }

    public virtual bool GetAchievementValue(string code) {
        bool currentValue = false;
        object objectValue = GetAttribute(code).val;
        if (objectValue != null) {
            currentValue = Convert.ToBoolean(objectValue);
        }

        return currentValue;
    }

	public virtual List<DataAttribute> GetList() {
        return GetAttributesList("achievement");
    }
}