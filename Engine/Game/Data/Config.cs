using System;
using System.IO;

public class ConfigKeys {
    public static string lastLoggedOnUser = "lastLoggedOnUser";
}

public class Config : DataObject {
    public string fileName;
    public string filePath;
    public string fileFullPath;

    public virtual string lastLoggedOnUser {
        get {
            return Get<string>(ConfigKeys.lastLoggedOnUser, "");
        }
        
        set {
            Set(ConfigKeys.lastLoggedOnUser, value);
        }
    }

    public Config() {
        Reset();
    }

    public override void Reset() {
        base.Reset();

        lastLoggedOnUser = "Player";

        //fileName = "config.json";
        //filePath = PathUtil.AppPersistencePath;
        //fileFullPath = PathUtil.Combine(filePath, fileName);
    }
}