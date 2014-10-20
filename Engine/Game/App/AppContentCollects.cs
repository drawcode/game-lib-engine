using System;
using System.Collections.Generic;
using System.IO;

public class AppContentCollects : BaseAppContentCollects<AppContentCollect> {
    private static volatile AppContentCollect current;
    private static volatile AppContentCollects instance;
    private static object syncRoot = new System.Object();
    /*
    public static string APP_STATE_BOOKS = "app-state-books";
    public static string APP_STATE_CARDS = "app-state-cards";
    public static string APP_STATE_GAMES = "app-state-games";
    public static string APP_STATE_SETTINGS = "app-state-settings";
    public static string APP_STATE_TROPHIES = "app-state-trophies";
    */
    
    public static string DATA_KEY = "app-content-collection-data";
        
    public static AppContentCollect Current {
        get {
            if (current == null) {
                lock (syncRoot) {
                    if (current == null) 
                        current = new AppContentCollect();
                }
            }
            
            return current;
        }
        set {
            current = value;
        }
    }
        
    public static AppContentCollects Instance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null) 
                        instance = new AppContentCollects(true);
                }
            }
            
            return instance;
        }
        set {
            instance = value;
        }
    }
            
    public AppContentCollects() {
        Reset();
        //ChangeState(APP_STATE_BOOKS);
    }
    
    public AppContentCollects(bool loadData) {
        Reset();
        path = "data/" + DATA_KEY + ".json";
        pathKey = DATA_KEY;
        LoadData();
    }

}

public class AppContentCollect : BaseAppContentCollect {

    
    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
            
    public AppContentCollect() {
        Reset();
    }
    
    public override void Reset() {
        base.Reset();
    }

    
}
