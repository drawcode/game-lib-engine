using System;
using System.Collections.Generic;
using System.IO;
using Engine.Data.Json;
using Engine.Utility;

public class GameDataObject : BaseMeta {

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    public int sort_order = 0;
    public int sort_order_type = 0;
    public string key;
    public string game_id;
    public string order_by;
    public string pack_code;
    public int pack_count;
    public int pack_sort;

    public GameDataObject() {
        Reset();
    }

    public override void Reset() {
        base.Reset();

        sort_order = 0;
        sort_order_type = 0;
        key = "";
        game_id = "";
        order_by = "";
        pack_code = "default";
        pack_count = 0;
    }

    public virtual void SetSettingValue(string code, object value) {
        DataAttribute att = new DataAttribute();
        att.val = value;
        att.code = code;
        att.name = code;
        att.type = "string";
        att.otype = "setting";
        SetAttribute(att);
    }

    public virtual string GetSettingValue(string code) {
        string currentValue = "";
        object objectValue = GetAttribute(code).val;
        if (objectValue != null) {
            currentValue = Convert.ToString(objectValue);
        }

        return currentValue;
    }

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    /*
    public double GetInitialDifficulty() {
        return GetAttributeDoubleValue(GameLevelKeys.LEVEL_INITIAL_DIFFICULTY);
    }

    public void SetInitialDifficulty(double val) {
        SetAttributeDoubleValue(GameLevelKeys.LEVEL_INITIAL_DIFFICULTY, val);
    }
    */
}