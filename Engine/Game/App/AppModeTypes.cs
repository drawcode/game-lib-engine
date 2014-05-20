using System;
using System.Collections.Generic;
using System.IO;

public class AppModeTypeMeta : BaseAppModeTypeMeta {
    //public static string appModeTypeGameDefault = "app-mode-type-game-default";
}

public enum AppModeTypeChoiceFlowState {
    AppModeTypeChoiceOverview,
    AppModeTypeChoiceDisplayItem,
    AppModeTypeChoiceResultItem,
    AppModeTypeChoiceResults,
}

public enum AppOverviewFlowState {
    Mode,
    GameplayTips,
    GeneralTips,
    Tutorial
}

public enum AppModeTypeCollectionFlowState {
    AppModeTypeCollectionOverview,
    AppModeTypeCollectionDisplayItem,
    AppModeTypeCollectionResultItem,
    AppModeTypeCollectionResults,
}

public class AppModeTypes : BaseAppModeTypes<AppModeType> {

    private static volatile AppModeType current;
    private static volatile AppModeTypes instance;
    private static object syncRoot = new System.Object();
    
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
    }

    public AppModeTypes(bool loadData) {
        Reset();
        path = "data/" + DATA_KEY + ".json";
        pathKey = DATA_KEY;
        LoadData();
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
