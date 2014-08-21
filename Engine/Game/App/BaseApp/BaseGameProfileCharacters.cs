using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public class BaseGameProfileCharacterAttributes {    
    
    public static string ATT_CURRENT_CHARACTER_PROFILE_CODE = "att-current-character-profile-code";
    public static string ATT_CURRENT_CHARACTER_CODE = "att-current-character-code";
    public static string ATT_CHARACTER_CODE = "att-character-code";
    public static string ATT_CHARACTER_COSTUME_CODE = "att-character-costume-code";
    public static string ATT_CHARACTERS = "att-characters";
}

public class BaseGameProfileCharacters {
    private static volatile BaseGameProfileCharacter current;
    private static volatile BaseGameProfileCharacters instance;
    private static object syncRoot = new Object();
    public static string DEFAULT_USERNAME = "Player";
 
    public static BaseGameProfileCharacter BaseCurrent {
        get {
            if(current == null) {
                lock(syncRoot) {
                    if(current == null) 
                        current = new BaseGameProfileCharacter();
                }
            }
 
            return current;
        }
        set {
            current = value;
        }
    }
     
    public static BaseGameProfileCharacters BaseInstance {
        get {
            if(instance == null) {
                lock(syncRoot) {
                    if(instance == null) 
                        instance = new BaseGameProfileCharacters();
                }
            }
 
            return instance;
        }
    }
 
    // TODO: Common profile actions, lookup, count, etc
    
    public static GameProfileCharacterItem currentCharacter = null;

    public static GameProfilePlayerProgressItem currentProgress {
        get {
            return GameProfileCharacters.Current.GetCurrentCharacter().profilePlayerProgress;
        }
    }

    public static GameProfileRPGItem currentRPG {
        get {
            return GameProfileCharacters.Current.GetCurrentCharacter().profileRPGItem;
        }
    }

    public static GameProfileCustomItem currentCustom {
        get {
            return GameProfileCharacters.Current.GetCurrentCharacter().profileCustomItem;
        }
    }

    /*
    public static GameProfileCharacterItem currentCharacter = null;

    public static GameProfilePlayerProgressItem currentProgress {
        get {
            if(currentCharacter == null) {
                return null;
            }
            return currentCharacter.profilePlayerProgress;
        }
    }

    public static GameProfileRPGItem currentRPG {
        get {
            if(currentCharacter == null) {
                return null;
            }
            return currentCharacter.profileRPGItem;
            
        }
    }

    public static GameProfileCustomItem currentCustom {
        get {
            if(currentCharacter == null) {
                return null;
            }
            return currentCharacter.profileCustomItem;
        }
    }
    */

}

public class GameProfileCharacterItems {
 
    public List<GameProfileCharacterItem> items;
  
    public GameProfileCharacterItems() {
        Reset();
    }
 
    public void Reset() {
        items = new List<GameProfileCharacterItem>();
    }

    public GameProfileCharacterItem GetCharacter(string code) {
        //if(items == null) {
        //     items = new List<GameProfileCharacterItem>();
        //
        //    GameProfileCharacterItem item = new GameProfileCharacterItem();
        //    items.Add(item);
        //}

        if(items == null) {
            return null;
        }

        foreach(GameProfileCharacterItem item in items) {
            if(item.code.ToLower() == code.ToLower()) {
                return item;
            }
        }
        return null;
    }
 
    public void SetCharacter(string code, GameProfileCharacterItem item) {
        bool found = false;
     
        for(int i = 0; i < items.Count; i++) {
            if(items[i].code.ToLower() == code.ToLower()) {
                items[i] = item;
                found = true;
                break;
            }
        }
     
        if(!found) {
            items.Add(item);
        }
    }

    public void SetCharacterRPG(string code, GameProfileRPGItem item) {
        GameProfileCharacterItem character = GetCharacter(code);

        if(character == null) {
            character = new GameProfileCharacterItem();
        }

        character.profileRPGItem = item;
        SetCharacter(code, character);
    }

    public void SetCharacterCustom(string code, GameProfileCustomItem item) {
        GameProfileCharacterItem character = GetCharacter(code);

        if(character == null) {
            character = new GameProfileCharacterItem();
        }

        character.profileCustomItem = item;
        SetCharacter(code, character);
    }

    public void SetCharacterProgress(string code, GameProfilePlayerProgressItem item) {
        GameProfileCharacterItem character = GetCharacter(code);

        if(character == null) {
            character = new GameProfileCharacterItem();
        }

        character.profilePlayerProgress = item;
        SetCharacter(code, character);
    }
}

public class GameProfileCharacterItemKeys {
    public static string code = "code";
    public static string current = "current";
    public static string characterCode = "characterCode";
    public static string characterDisplayName = "characterDisplayName";
    public static string characterDisplayCode = "characterDisplayCode";
    public static string characterCostumeCode = "characterCostumeCode";
    public static string profileRPGItem = "profileRPGItem";
    public static string profilePlayerProgress = "profilePlayerProgress";
    public static string profileCustomItem = "profileCustomItem";
}

public class GameProfileCharacterItem : DataObject {

    // profile specific code

    public virtual string code {
        get { 
            return Get<string>(
                GameProfileCharacterItemKeys.code, 
                ProfileConfigs.defaultProfileCharacterCode);
        }
        
        set {
            Set(GameProfileCharacterItemKeys.code, value);
        }
    }

    // game character code

    public virtual string characterCode {
        get { 
            return Get<string>(
                GameProfileCharacterItemKeys.characterCode, 
                ProfileConfigs.defaultGameCharacterCode);
        }
        
        set {
            Set(GameProfileCharacterItemKeys.characterCode, value);
        }
    }

    // user visible name
    
    public virtual string characterDisplayName {
        get { 
            return Get<string>(
                GameProfileCharacterItemKeys.characterDisplayName, 
                ProfileConfigs.defaultGameCharacterDisplayName);
        }
        
        set {
            Set(GameProfileCharacterItemKeys.characterDisplayName, value);
        }
    }

    // user visible number
    
    public virtual string characterDisplayCode {
        get { 
            return Get<string>(
                GameProfileCharacterItemKeys.characterDisplayCode, 
                ProfileConfigs.defaultGameCharacterDisplayCode);
        }
        
        set {
            Set(GameProfileCharacterItemKeys.characterDisplayCode, value);
        }
    }
    
    public virtual GameProfileRPGItem profileRPGItem {
        get { 
            return Get<GameProfileRPGItem>(
                GameProfileCharacterItemKeys.profileRPGItem, new GameProfileRPGItem());
        }
        
        set {
            Set(GameProfileCharacterItemKeys.profileRPGItem, value);
        }
    }
    
    public virtual GameProfilePlayerProgressItem profilePlayerProgress {
        get { 
            return Get<GameProfilePlayerProgressItem>(
                GameProfileCharacterItemKeys.profilePlayerProgress, new GameProfilePlayerProgressItem());
        }
        
        set {
            Set(GameProfileCharacterItemKeys.profilePlayerProgress, value);
        }
    }
    
    public virtual GameProfileCustomItem profileCustomItem {
        get { 
            return Get<GameProfileCustomItem>(
                GameProfileCharacterItemKeys.profileCustomItem, new GameProfileCustomItem());
        }
        
        set {
            Set(GameProfileCharacterItemKeys.profileCustomItem, value);
        }
    }

    public GameProfileCharacterItem() {
        Reset();
    }
    
    public override void Reset() {
        base.Reset();

        //current = true;
        code = ProfileConfigs.defaultProfileCharacterCode;
        characterCode = ProfileConfigs.defaultGameCharacterCode;
        profileRPGItem = new GameProfileRPGItem();
        profilePlayerProgress = new GameProfilePlayerProgressItem();
        profileCustomItem = new GameProfileCustomItem();
    }

    public GameCharacter GetCharacterData() {
        return GameCharacters.Instance.GetById(characterCode);
    }
    
    public string GetCharacterDataModel() {
        GameCharacter gameCharacter = GetCharacterData();

        if(gameCharacter == null) {
            return ProfileConfigs.defaultGameCharacterCode;
        }

        if(gameCharacter.data == null) {
            return ProfileConfigs.defaultGameCharacterCode;
        }

        GameDataModel gameModelData = gameCharacter.data.GetModel();
        
        if(gameModelData == null) {
            return ProfileConfigs.defaultGameCharacterCode;
        }

        return gameModelData.code;
    }
}

public class BaseGameProfileCharacter : Profile {
    // BE CAREFUL adding properties as they will cause a need for a profile conversion
    // Best way to add items to the profile is the GetAttribute and SetAttribute class as 
    // that stores as a generic DataAttribute class.  Booleans, strings, objects, serialized json objects etc
    // all work well and cause no need to convert profile on updates. 
     
    public BaseGameProfileCharacter() {
        Reset();
    }
 
    public override void Reset() {
        base.Reset();
        username = ProfileConfigs.defaultPlayerName;
    }
 
    // characters

    public virtual GameProfileCharacterItems character_items {
        get {
            return Get<GameProfileCharacterItems>(BaseDataObjectKeys.character_items);
        }
        
        set {
            Set(BaseDataObjectKeys.character_items, value);
        }
    }
 
    public virtual void SetCharacters(GameProfileCharacterItems obj) {

        character_items = obj;

        Messenger.Broadcast(BaseGameProfileMessages.ProfileShouldBeSaved);
    }

    public virtual GameProfileCharacterItems GetCharacters() {       
        GameProfileCharacterItems obj = new GameProfileCharacterItems();
                
        obj = character_items;

        //UnityEngine.LogUtil.Log("GameProfileCharacterItems:obj:" + obj.ToJson());
     
        if(obj == null) {
            obj = new GameProfileCharacterItems();
        }
     
        if(obj.items.Count == 0) {
            // add default
            obj.SetCharacter(GameProfileCharacters.Current.GetCurrentCharacterProfileCode(), 
                             new GameProfileCharacterItem());
        }        
     
        return obj;
    }

    // helpers

    public void CurrentCharacterAddGamePlayerProgressXP(double val) {
        GameProfileCharacterItem character = GetCurrentCharacter();
        character.profilePlayerProgress.AddGamePlayerProgressXP(val);
        GamePlayerProgress.SetStatXP(val);
        SetCharacter(character);
    }

    public void CurrentCharacterAddGamePlayerProgressLevel(double val) {
        GameProfileCharacterItem character = GetCurrentCharacter();
        character.profilePlayerProgress.AddGamePlayerProgressLevel(val);
        SetCharacter(character);
    }

    public void CurrentCharacterAddGamePlayerProgressEnergy(double val) {
        GameProfileCharacterItem character = GetCurrentCharacter();
        character.profilePlayerProgress.AddGamePlayerProgressEnergy(val);
        SetCharacter(character);
    }

    public void CurrentCharacterAddGamePlayerProgressEnergyAndHealth(double valEnergy, double valHealth) {
        GameProfileCharacterItem character = GetCurrentCharacter();
        character.profilePlayerProgress.AddGamePlayerProgressEnergy(valEnergy);
        character.profilePlayerProgress.AddGamePlayerProgressHealth(valHealth);
        SetCharacter(character);
    }

    public void CurrentCharacterAddGamePlayerProgressEnergyAndHealthRuntime(double valEnergy, double valHealth) {
        BaseGameProfileCharacters.currentCharacter.profilePlayerProgress.AddGamePlayerProgressEnergy(valEnergy);
        BaseGameProfileCharacters.currentCharacter.profilePlayerProgress.AddGamePlayerProgressHealth(valHealth);
    }

    public void CurrentCharacterAddGamePlayerProgressHealth(double val) {
        GameProfileCharacterItem character = GetCurrentCharacter();
        character.profilePlayerProgress.AddGamePlayerProgressHealth(val);
        SetCharacter(character);
    }
 
    // character 
 
    public GameProfileRPGItem GetCurrentCharacterRPG() {
        return GetCurrentCharacter().profileRPGItem;
    }

    public GameProfileCustomItem GetCurrentCharacterCustom() {
        return GetCurrentCharacter().profileCustomItem;
    }

    public GameProfilePlayerProgressItem GetCurrentCharacterProgress() {
        return GetCurrentCharacter().profilePlayerProgress;
    }
 
    public GameProfileCharacterItem GetCurrentCharacter() {
        return GetCharacter(GameProfileCharacters.Current.GetCurrentCharacterProfileCode());
    }
 
    public GameProfileRPGItem GetCharacterRPG(string code) {
        return GetCharacter(code).profileRPGItem;
    }

    public GameProfileCustomItem GetCharacterCustom(string code) {
        return GetCharacter(code).profileCustomItem;
    }

    public GameProfilePlayerProgressItem GetCharacterProgress(string code) {
        return GetCharacter(code).profilePlayerProgress;
    }
 
    public GameProfileCharacterItem GetCharacter(string code) {

        if(BaseGameProfileCharacters.currentCharacter == null) {
            GameProfileCharacterItem item = GetCharacters().GetCharacter(code);
    
            // TODO check this to be sure initing is no problem in sync
            if(item == null && code != "default") {
               item = new GameProfileCharacterItem();
               GetCharacters().SetCharacter(code, item);
            }

            if(item != null) {
                if(item.profileCustomItem != null && GameCustomController.Instance != null) {
                    item.profileCustomItem = 
                        GameCustomController.CheckCustomColorInit(item.profileCustomItem, BaseDataObjectKeys.character);
				}
                BaseGameProfileCharacters.currentCharacter = item;
            }
        }

        return BaseGameProfileCharacters.currentCharacter;
    }

    public void AddCharacter(string characterCode) {
        GameCharacter gameCharacter = GameCharacters.Instance.GetById(characterCode);

        if(gameCharacter != null) {

            int countSameType = 0;

            string characterNameTemp = "";

            foreach(GameProfileCharacterItem _item in GetCharacters().items) {
                if(_item.characterDisplayName.Contains(gameCharacter.display_name)) {
                    countSameType = countSameType + 1;
                }
            }

            characterNameTemp = gameCharacter.display_name + " #" + (countSameType + 1);

            GameProfileCharacterItem item = new GameProfileCharacterItem();
            item.characterCode = characterCode;
            item.characterDisplayName = characterNameTemp;
            item.characterDisplayCode = "";
            item.code = UniqueUtil.Instance.CreateUUID4(); // allows multiple of same type
            SetCharacter(item.code, item);
        }
    }
    
    public void SetCharacter(string code, GameProfileCharacterItem item) {
        SetCharacter(code, item, true);
    }
 
    public void SetCharacter(string code, GameProfileCharacterItem item, bool setAsCurrent) {

        GameProfileCharacterItems characters = GetCharacters();

        if(setAsCurrent) {
            BaseGameProfileCharacters.currentCharacter = item;
            GameProfileCharacters.Current.SetCurrentCharacterProfileCode(code);
        }

        characters.SetCharacter(code, item);
        SetCharacters(characters);
    }

    public void SetCharacter(GameProfileCharacterItem item) {
        SetCharacter(GameProfileCharacters.Current.GetCurrentCharacterProfileCode(), item);
    }

    public void SetCharacterRPG(string code, GameProfileRPGItem item) {
        GameProfileCharacterItems characters = GetCharacters();
        characters.SetCharacterRPG(code, item);
        SetCharacters(characters);
    }

    public void SetCharacterProgress(string code, GameProfilePlayerProgressItem item) {
        GameProfileCharacterItems characters = GetCharacters();
        characters.SetCharacterProgress(code, item);
        SetCharacters(characters);
    }

    public void SetCharacterCustom(string code, GameProfileCustomItem item) {
        GameProfileCharacterItems characters = GetCharacters();
        characters.SetCharacterCustom(code, item);
        SetCharacters(characters);
    }

    public void SetCharacterRPG(GameProfileRPGItem item) {
        SetCharacterRPG(GameProfileCharacters.Current.GetCurrentCharacterProfileCode(), item);
    }

    public void SetCharacterCustom(GameProfileCustomItem item) {
        SetCharacterCustom(GameProfileCharacters.Current.GetCurrentCharacterProfileCode(), item);
    }
     
    // customizations        
 
    public virtual void SetValue(string code, object value) {
        DataAttribute att = new DataAttribute();
        att.val = value;
        att.code = code;
        att.name = "";
        att.type = "bool";
        att.otype = "character";
        SetAttribute(att);
    }
 
    public virtual bool GetValue(string code) {
        bool currentValue = false;
        object objectValue = GetAttribute(code).val;
        if(objectValue != null) {
            currentValue = Convert.ToBoolean(objectValue);
        }
     
        return currentValue;
    }
 
    public virtual List<DataAttribute> GetList() {
        return GetAttributesList("character");
    }    
 
    // CHARACTER - Player specific 
 
    public string GetCurrentCharacterProfileCode() {
        return GetCurrentCharacterProfileCode(ProfileConfigs.defaultProfileCharacterCode);
    }
 
    public string GetCurrentCharacterProfileCode(string defaultValue) {
        string attValue = defaultValue;
        if(CheckIfAttributeExists(BaseGameProfileCharacterAttributes.ATT_CURRENT_CHARACTER_PROFILE_CODE))
            attValue = GetAttributeStringValue(BaseGameProfileCharacterAttributes.ATT_CURRENT_CHARACTER_PROFILE_CODE);
        return attValue;
    }
 
    public void SetCurrentCharacterProfileCode(string attValue) {
        SetAttributeStringValue(BaseGameProfileCharacterAttributes.ATT_CURRENT_CHARACTER_PROFILE_CODE, attValue);
    }
}
