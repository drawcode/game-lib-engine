using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

// using Engine.Data.Json;
using Engine.Utility;

public class BaseGameProfileProductAttributes {
    
    // RPG  
    public static string ATT_PROGRESS_XP = "progress-xp";
    public static string ATT_PROGRESS_HEALTH = "progress-health";
    public static string ATT_PROGRESS_ENERGY = "progress-energy";
    public static string ATT_PROGRESS_LEVEL = "progress-level";
}

public class BaseGameProfileProducts {
    private static volatile BaseGameProfileProduct current;
    private static volatile BaseGameProfileProducts instance;
    private static object syncRoot = new Object();
    public static string DEFAULT_USERNAME = "Player";
    
    public static BaseGameProfileProduct BaseCurrent {
        get {
            if (current == null) {
                lock (syncRoot) {
                    if (current == null) 
                        current = new BaseGameProfileProduct();
                }
            }
    
            return current;
        }
        set {
            current = value;
        }
    }
        
    public static BaseGameProfileProducts BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null) 
                        instance = new BaseGameProfileProducts();
                }
            }
    
            return instance;
        }
    }
    
    // TODO: Common profile actions, lookup, count, etc
}

public class BaseGameProfileProduct : DataObject {
    // BE CAREFUL adding properties as they will cause a need for a profile conversion
    // Best way to add items to the profile is the GetAttribute and SetAttribute class as 
    // that stores as a generic DataAttribute class.  Booleans, strings, objects, serialized json objects etc
    // all work well and cause no need to convert profile on updates. 
        
    public BaseGameProfileProduct() {
        //Reset();
    }
    
    public override void Reset() {
        base.Reset();
        //username = ProfileConfigs.defaultPlayerName;
    }
        
    // customizations       
    
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
        
    // UNLOCKS

    public bool GetPromoUnlocked() {
        return true;//Contents.AccessPermissions.Contains("unlocked");
    }
    
    public void SetPromoUnlocked(bool unlocked) {
        if (unlocked) {
            //Contents.AddGlobalAccessPermission("unlocked");
        }
        else {
            //Contents.RemoveGlobalAccessPermission("unlocked");
        }
    }
    
    public void SyncAccessPermissions() {
        //Contents.AccessPermissions.UnionWith(GetAccessPermissions());
    }
    
    // Profile specific content access, use Contents for global access 
    // saved outside of the profile.
        
    // Attribute based unlocks to switch to for profile based unlocks.
    public List<string> GetAccessPermissions() {
        List<string> permissions = new List<string>();
        DataAttribute attribute = GetAttribute("access-permissions");
        if (attribute != null) {
            permissions = attribute.val as List<string>;
            if (permissions != null)
                return permissions;
            else
                permissions = new List<string>();
        }
        return permissions;
    }
    
    public bool CheckIfAccessPermissionExists(string permission) {
        List<string> permissions = GetAccessPermissions();
        if (permissions.Contains(permission))
            return true;
        return false;
    }
    
    public void SetAccessPermissions(List<string> permissions) {
        DataAttribute attribute = new DataAttribute();
        attribute.code = "access-permissions";
        attribute.val = permissions;
        SetAttribute(attribute);
    }
    
    public void SetAccessPermission(string permission) {
        List<string> permissions = GetAccessPermissions();
        if (!permissions.Contains(permission)) {
            permissions.Add(permission);
        }
        SetAccessPermissions(permissions);
    }
    
    
    
}


