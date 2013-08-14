using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Profile : DataObject {
    public string username;
    public string uuid;
    public string udid;

    public string fileName;
    public string filePath;
    public string fileFullPath;

    public int loginCount = 0;

    public Profile() {
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
        uuid = "";
        udid = "";
    }
}