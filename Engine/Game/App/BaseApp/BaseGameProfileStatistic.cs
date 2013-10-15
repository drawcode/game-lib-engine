using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Engine.Data.Json;
using Engine.Utility;

public class BaseGameProfileStatisticAttributes {	

}	

public class BaseGameProfileStatistics {
    private static volatile BaseGameProfileStatistic current;
    private static volatile BaseGameProfileStatistics instance;
    private static object syncRoot = new Object();

    public static string DEFAULT_USERNAME = "Player";

    public static BaseGameProfileStatistic BaseCurrent {
        get {
            if (current == null) {
                lock (syncRoot) {
                    if (current == null)
                        current = new BaseGameProfileStatistic();
                }
            }

            return current;
        }
        set {
            current = value;
        }
    }

    public static BaseGameProfileStatistics BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseGameProfileStatistics();
                }
            }

            return instance;
        }
    }

    // TODO: Common profile actions, lookup, count, etc
}

public class BaseGameProfileStatistic : Profile {

    // BE CAREFUL adding properties as they will cause a need for a profile conversion
    // Best way to add items to the profile is the GetAttribute and SetAttribute class as
    // that stores as a generic DataAttribute class.  Booleans, strings, objects, serialized json objects etc
    // all work well and cause no need to convert profile on updates.

    public BaseGameProfileStatistic() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
        //username = ProfileConfigs.defaultPlayerName;
    }

    // STATISTICS

    public virtual void SetStatisticValue(string code, object value) {
        double convertedValue = 0;
        if (value != null)
            convertedValue = Convert.ToDouble(value);
        DataAttribute att = new DataAttribute();
        att.val = convertedValue;
        att.code = code;
        att.name = "";
        att.type = "double";
        att.otype = "statistic";
        SetAttribute(att);
    }

    public virtual double GetStatisticValue(string code) {
        double currentValue = 0;
        object objectValue = GetAttribute(code).val;
        if (objectValue != null) {
            currentValue = Convert.ToDouble(objectValue);
            if (currentValue < 0) {
                currentValue = 0;
            }
        }

        return currentValue;
    }

	public virtual List<DataAttribute> GetList() {
        return GetAttributesList("statistic");
    }
}