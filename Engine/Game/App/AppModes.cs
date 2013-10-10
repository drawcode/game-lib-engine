using System;
using System.Collections.Generic;
using System.IO;

public class AppModeDefaults {
    public static string gameModeTypeArcade = "game-mode-type-arcade";
}

public class AppModes : BaseAppModes<AppMode> {

    private static volatile AppMode current;
    private static volatile AppModes instance;
    private static System.Object syncRoot = new System.Object();
    
    public static string DATA_KEY = "app-mode-type-data";
    
    public static AppMode Current {

        get  {
            if (current == null) {
                lock (syncRoot)  {
                    if (current == null)
                        current = new AppMode();
                }
            }
    
            return current;
        }

        set {
            current = value;
        }
    }
     
    public static AppModes Instance {
        get  {
            if (instance == null) {
                lock (syncRoot)  {
                    if (instance == null)
                        instance = new AppModes(true);
                }
            }
    
            return instance;
        }
        set {
            instance = value;
        }
    }
         
    public AppModes() {
        Reset();
        ChangeState(AppModeDefaults.gameModeTypeArcade);
    }

    public AppModes(bool loadData) {
        Reset();
        path = "data/" + DATA_KEY + ".json";
        pathKey = DATA_KEY;
        LoadData();
    }
    
    public void ChangeState(string code) {
        if(Current.code != code) {
            Current = GetById(code);
        }
    }
}

public class AppMode : BaseAppMode {
 
    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
         
    public AppMode () {
        Reset();
    }
    
    public override void Reset() {
        base.Reset();
    }
}
