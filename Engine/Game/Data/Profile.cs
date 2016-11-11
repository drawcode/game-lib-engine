using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ProfileKeys {
    public static string login_count = "login_count";
}

public class Profile : DataObject {
    
    public virtual int login_count {
        get {
            return Get<int>(ProfileKeys.login_count);
        }
        
        set {
            Set(ProfileKeys.login_count, value);
        }
    }
    
    public virtual string username {
        get {
            return Get<string>(BaseDataObjectKeys.username);
        }
        
        set {
            Set(BaseDataObjectKeys.username, value);
        }
    }
    
    public virtual string uuid {
        get {

            //string val = Get<string>(BaseDataObjectKeys.uuid);
            //if(string.IsNullOrEmpty(val)) {
            //    val = UniqueUtil.Instance.CreateUUID4();
            //    Set(BaseDataObjectKeys.uuid, val);
            //}

            return Get<string>(BaseDataObjectKeys.uuid, 
                               UniqueUtil.CreateUUID4());
        }
        
        set {
            Set(BaseDataObjectKeys.uuid, value);
        }
    }

    public Profile() {
        Reset();
    }

    public void ChangeUser(string name) {

        if(name == username) {
            return;
        }

        Reset();

        username = name;
    }

    public void ChangeUserNoReset(string name) {
        username = name;
    }

    public override void Reset() {
        base.Reset();

        username = "Player";
        uuid = "";
    }
}