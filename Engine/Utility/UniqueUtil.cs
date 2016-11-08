using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class UniqueUtil {
    private static volatile UniqueUtil instance;
    private static System.Object syncRoot = new System.Object();

    public static string DATA_KEY = "unique-util";

    public static UniqueUtil Instance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new UniqueUtil();
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    // Device specific uuid by app install
    public string currentUniqueId {
        get {
            string currentId = "";

            if (SystemPrefUtil.HasLocalSetting(DATA_KEY)) {
                currentId = SystemPrefUtil.GetLocalSettingString(DATA_KEY);
            }
            else {
                currentId = CreateUUID4();
                SystemPrefUtil.SetLocalSettingString(DATA_KEY, currentId);
                SystemPrefUtil.Save();
            }

            return currentId;
        }
    }

    public UniqueUtil() {
    }



    // Create a UUID4 format
    // http://www.ietf.org/rfc/rfc4122.txt

    public static string CreateUUID4() {
        if (Instance != null) {
            return Instance.createUUID4();
        }
        return "";
    }

    // Create a UUID4 format
    // http://www.ietf.org/rfc/rfc4122.txt

    public string createUUID4() {
        return System.Guid.NewGuid().ToString();
        /*
         * private int randomSeed = 1;
        char[] CHARS = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();
        char[] uuidList = new char[36];

        int rnd = 0;
        int r = 0;

        for(int i = 0; i < 36; i++) {
            if(i == 8 || i == 13 || i == 18 || i == 23) {
                uuidList[i] = '-';
            }
            else if (i == 14) {
                uuidList[i] = '4';
            }
            else {
                if(rnd <= 0x02) {
                    int currentInt = new System.Random(randomSeed++).Next(0x1000000);
                    double currentDouble = new System.Random(currentInt).NextDouble();
                    rnd = 0x2000000 + (int)(currentDouble*0x1000000) | 0;
                }
                r = rnd & 0xf;
                rnd = rnd >> 4;
                int index = (i == 19) ? (r & 0x3) | 0x8 : r;
                uuidList[i] = CHARS[index];
            }
        }

        return new string(uuidList).ToLower();
        */
    }

    /*
    public string NewGuid() {
        string puid = "";
        for(int c=0; c < 40; ++c)
            puid += String.Format("{0:x}", UnityEngine.Random.Range(0, 16));
        return puid;
    }

    var CHARS = '0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz'.split('');

     Math.uuidFast = function() {
    var chars = CHARS, uuid = new Array(36), rnd=0, r;
    for (var i = 0; i < 36; i++) {
      if (i==8 || i==13 ||  i==18 || i==23) {
        uuid[i] = '-';
      } else if (i==14) {
        uuid[i] = '4';
      } else {
        if (rnd <= 0x02) rnd = 0x2000000 + (Math.random()*0x1000000)|0;
        r = rnd & 0xf;
        rnd = rnd >> 4;
        uuid[i] = chars[(i == 19) ? (r & 0x3) | 0x8 : r];
      }
    }
    return uuid.join('');

    if (i==8 || i==13 ||  i==18 || i==23) {
        uuid[i] = '-';
      } else if (i==14) {
        uuid[i] = '4';
      } else {
        if (rnd <= 0x02) rnd = 0x2000000 + (Math.random()*0x1000000)|0;
        r = rnd & 0xf;
        rnd = rnd >> 4;
        uuid[i] = chars[(i == 19) ? (r & 0x3) | 0x8 : r];
      }
  };

    public static string CreateUUID4Fast() {
        string uuid = "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx";
        uuid = Regex.Replace(uuid, "/[xy]/g");

    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
      var r = Math.random()*16|0, v = c == 'x' ? r : (r&0x3|0x8);
      return v.toString(16);
    });

        return uuid;
    }
    */
}