using System;
using System.Collections.Generic;
using System.IO;

public class AppModeTypeDefaults {
    public static string gameModeTypeArcade = "game-mode-type-arcade";
}

public class AppModeTypes : BaseAppModeTypes<AppModeType> {

    private static volatile AppModeType current;
    private static volatile AppModeTypes instance;
    private static System.Object syncRoot = new System.Object();
    
    public static string DATA_KEY = "app-mode-type-data";
    
    public static AppModeType Current {

        get  {
            if (current == null) {
                lock (syncRoot)  {
                    if (current == null)
                        current = new AppModeType();
                }
            }
    
            return current;
        }

        set {
            current = value;
        }
    }
     
    public static AppModeTypes Instance {
        get  {
            if (instance == null) {
                lock (syncRoot)  {
                    if (instance == null)
                        instance = new AppModeTypes(true);
                }
            }
    
            return instance;
        }
        set {
            instance = value;
        }
    }
         
    public AppModeTypes() {
        Reset();
        ChangeState(AppModeTypeDefaults.gameModeTypeArcade);
    }

    public AppModeTypes(bool loadData) {
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

public class AppModeType : BaseAppModeType {
 
    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
         
    public AppModeType () {
        Reset();
    }
    
    public override void Reset() {
        base.Reset();
    }
}
