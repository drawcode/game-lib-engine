using System;
using System.IO;

public class Config : DataObject {
    public string lastLoggedOnUser;
    public string fileName;
    public string filePath;
    public string fileFullPath;

    public Config() {
        Reset();
    }

    public override void Reset() {
        base.Reset();

        lastLoggedOnUser = GetDefaultPlayer();

        //fileName = "config.json";
        //filePath = PathUtil.AppPersistencePath;
        //fileFullPath = PathUtil.Combine(filePath, fileName);
    }
}