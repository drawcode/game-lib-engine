using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Career : DataObject {
    public string username;
    public string uuid;
    public string udid;

    public int loginCount;

    public string fileName;
    public string filePath;
    public string fileFullPath;

    public Career() {
        Reset();
    }

    public void ChangeUser(string name) {
        Reset();

        username = name;
    }

    public void ChangeUserNoReset(string name) {
        username = name;
    }

    public override void Reset() {
        base.Reset();

        username = GetDefaultPlayer();
        uuid = "";//Puid.New();
        udid = "";
        loginCount = 0;
    }
}