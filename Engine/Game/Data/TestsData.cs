// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Engine.Data.Json;

public class DataKeyedObjectLeaf : DataKeyedObject {

    public string otherProperty = "";
}

public class TestsData {

    public static void Advance(string name) {
        Debug.Log(name + "\r\n----------------------------------\r\n\r\n");
    }

    public static void RunTest() {

        Advance("Running tests...");
        
        ContentsConfig.contentRootFolder = "drawlabs";
        ContentsConfig.contentAppFolder = "game-drawlabs-brainball";
        ContentsConfig.contentDefaultPackFolder = "game-drawlabs-brainball-1";
        ContentsConfig.contentVersion = "1.0";
        ContentsConfig.contentIncrement = 2;
        
        Advance("Creating Contents cache paths");
        ContentPaths.CreateCachePaths();
        
        Advance("Loading Profile");
        GameState.LoadProfile();

        //Advance("TestGameCharacterSkin");
        //TestGameCharacterSkin();

        //Advance("TestGameCharacterSkinLoadData");
        //TestGameCharacterSkinLoadData();
        
        //Advance("TestGameState_LoadProfile");
        //TestGameState_LoadProfile();

        
        //Advance("TestGameState_SaveProfile");
        //TestGameState_SaveProfile();

        //Advance("TestGameProfileCharacter_GetCharacter");
        //TestGameProfileCharacter_GetCharacter();
        
        //Advance("TestGameProfileCharacter_GetCurrentCharacter");
        //TestGameProfileCharacter_GetCurrentCharacter();
        
        //Advance("TestGameProfileCharacter_currentCharacter");
        //TestGameProfileCharacter_currentCharacter();
                
        //Advance("TestGameProfileCharacter_currentProgress");
        //TestGameProfileCharacter_currentProgress();
        
        //Advance("TestGameColors_List");
        //TestGameColors_List();
        
        //Advance("TestGameColors_Code");
        //TestGameColors_Code();

        
        //Advance("TestAppContentAssetModels_List");
        //TestAppContentAssetModels_List();

        
        //Advance("TestAppContentAssetModels_Load");
        //TestAppContentAssetModels_Load();
                
        //("TestAppContentAssetCustomItems_List");
        //TestAppContentAssetCustomItems_List();

        
        Advance("TestGameColorPresets_List");
        TestGameColorPresets_List();
        
        Advance("TestGameCharacters_List");
        //TestGameCharacters_List();
                
        Advance("TestGameCharacters_Load");
        //TestGameCharacters_Load();
    }

    public static void DumpObj(string name, string oname, object o) {        
        Debug.Log(string.Format("{0} : {1}  : {2} ", name, oname, o));
    }

    public static bool AssertEquals(string name, object a, object b) {
        string dataA = a.ToJson();
        string dataB = b.ToJson();
        bool equal = false;
        if(dataA == dataB) {            
            equal = true;
            Debug.Log(name + ": SUCCESS :" + equal);
        }
        else {       
            Debug.LogError(name + ": FAIL :" + equal);
        }

        DumpObj(name, "dataA", dataA);
        DumpObj(name, "dataB", dataB);

        return equal;
    }
        
    public static void TestGameColorPresets_List() {
        
        string name = "TestGameColorPresets_List";
        
        Debug.Log(name);
        
        List<GameColorPreset> items = GameColorPresets.Instance.GetAll();
        DumpObj(name, "items", items);
        
        //AssertEquals(name, username, "Player");
        
        foreach(GameColorPreset item in items) {  
            
            Debug.Log("item:code:" + item.code);         
            Debug.Log("item:type:" + item.type);
            
            Debug.Log("item:json:" + item.ToJson());  
            
            Dictionary<string, string> data = item.data;
            
            if(data != null) {
                foreach(KeyValuePair<string, string> pair in data) {                    
                    Debug.Log("pair:Key:" + pair.Key);
                    Debug.Log("pair:Value:" + pair.Value);                    
                }
            }
        }
        
        DumpObj(name, "items.Count", items.Count);
    }

    
    public static void TestGameCharacters_List() {
        
        string name = "TestGameCharacters_List";
        
        Debug.Log(name);
        
        List<GameCharacter> items = GameCharacters.Instance.GetAll();
        DumpObj(name, "items", items);
        
        //AssertEquals(name, username, "Player");
        
        foreach(GameCharacter item in items) {  
            
            Debug.Log("item:code:" + item.code);         
            Debug.Log("item:type:" + item.type);

            Debug.Log("item:json:" + item.ToJson());  

            GameDataCharacter data = item.character_data;

            if(data != null) {
                foreach(GameDataCharacterModel model in data.models) {
                    string modelCode = model.code;
                    
                    Debug.Log("model:code:" + model.code);
                    Debug.Log("model:type:" + model.type);
                    Debug.Log("model:textures:" + model.textures);
                    Debug.Log("model:colors:" + model.colors);

                    GameProfileCustomItem profileCustomItem = GameProfileCharacters.currentCustom;

                    GameColorPreset preset = GameColorPresets.Instance.GetByCode("game-college-baylor-bears-home");

                    GameObject playerObject = item.Load();

                    if(playerObject != null) {
                        GameCustomController.SetMaterialColors(playerObject, profileCustomItem);
                    }

                    break;

                }
            }
            break;
        }
        
        DumpObj(name, "items.Count", items.Count);
    }
    
    public static void TestGameCharacters_Load() {
        
        string name = "TestGameCharacters_Load";
        
        Debug.Log(name);
        
        List<AppContentAssetModel> items = AppContentAssetModels.Instance.GetAll();
        DumpObj(name, "items", items);
        
        //AssertEquals(name, username, "Player");
        
        foreach(AppContentAssetModel item in items) {            
            Debug.Log("item:code:" + item.code);         
            Debug.Log("item:display_name:" + item.display_name);
            
            Debug.Log("item:json:" + item.ToJson());
            
            if(item.custom_materials != null) {         
                
                Debug.Log("item.data.custom_materials.Count:" + item.custom_materials.Count);
                
                foreach(AppContentAssetCustomItemProperty prop 
                        in item.custom_materials) {
                    
                    Debug.Log("prop:code:" + prop.code);  
                    Debug.Log("prop:name:" + prop.name); 
                    foreach(string type in prop.types) {
                        Debug.Log("prop:type:s:" + type);
                    }
                }
            }
            else {                
                Debug.Log("data was NULL" + item.ToJson());
            }
            
            AppContentAssetCustomItem customItem = item.GetCustomItems();
            
            Debug.Log("customItem:json:" + customItem.ToJson());
            
            if(customItem != null) {
                if(customItem.properties != null) {
                    foreach(AppContentAssetCustomItemProperty prop in customItem.properties) {
                        Debug.Log("prop:code:" + prop.code); 
                    }
                }
            }
            
            item.LoadModel();
            
            break;
        }
        
        DumpObj(name, "items.Count", items.Count);
    }

    //
    
    public static void TestAppContentAssetModels_Load() {
        
        string name = "TestAppContentAssetModels_Load";
        
        Debug.Log(name);

        List<AppContentAssetModel> items = AppContentAssetModels.Instance.GetAll();
        DumpObj(name, "items", items);
        
        //AssertEquals(name, username, "Player");
        
        foreach(AppContentAssetModel item in items) {            
            Debug.Log("item:code:" + item.code);         
            Debug.Log("item:display_name:" + item.display_name);
            
            Debug.Log("item:json:" + item.ToJson());
            
            if(item.custom_materials != null) {         
                
                Debug.Log("item.data.custom_materials.Count:" + item.custom_materials.Count);
                
                foreach(AppContentAssetCustomItemProperty prop 
                        in item.custom_materials) {
                    
                    Debug.Log("prop:code:" + prop.code);  
                    Debug.Log("prop:name:" + prop.name); 
                    foreach(string type in prop.types) {
                        Debug.Log("prop:type:s:" + type);
                    }
                }
            }
            else {                
                Debug.Log("data was NULL" + item.ToJson());
            }
            
            AppContentAssetCustomItem customItem = item.GetCustomItems();
            
            Debug.Log("customItem:json:" + customItem.ToJson());
            
            if(customItem != null) {
                if(customItem.properties != null) {
                    foreach(AppContentAssetCustomItemProperty prop in customItem.properties) {
                        Debug.Log("prop:code:" + prop.code); 
                    }
                }
            }

            item.LoadModel();

            break;
        }
        
        DumpObj(name, "items.Count", items.Count);
    }

    public static void TestAppContentAssetCustomItems_List() {
        
        string name = "TestAppContentAssetCustomItems_List";
        
        Debug.Log(name);
        
        List<AppContentAssetCustomItem> items = AppContentAssetCustomItems.Instance.GetAll();
        DumpObj(name, "items", items);
        
        //AssertEquals(name, username, "Player");
        
        foreach(AppContentAssetCustomItem item in items) {  

            Debug.Log("item:code:" + item.code);         
            Debug.Log("item:type:" + item.type);

            if(item.properties != null) {         
                
                Debug.Log("item.data.properties.Count:" + item.properties.Count);

                foreach(AppContentAssetCustomItemProperty prop 
                        in item.properties) {
                    
                    Debug.Log("prop:code:" + prop.code);  
                    foreach(string type in prop.types) {
                        Debug.Log("prop:type:s:" + type);
                    }
                }
            }
            else {                
                Debug.Log("data was NULL" + item.ToJson());
            }

        }
        
        DumpObj(name, "items.Count", items.Count);
    }

    //
    
    public static void TestAppContentAssetModels_List() {
        
        string name = "TestAppContentAssetModels_List";
        
        Debug.Log(name);
        
        List<AppContentAssetModel> items = AppContentAssetModels.Instance.GetAll();
        DumpObj(name, "items", items);
        
        //AssertEquals(name, username, "Player");
        
        foreach(AppContentAssetModel item in items) {            
            Debug.Log("item:code:" + item.code);         
            Debug.Log("item:display_name:" + item.display_name);
            
            Debug.Log("item:json:" + item.ToJson());

            if(item.custom_materials != null) {         
                
                Debug.Log("item.data.custom_materials.Count:" + item.custom_materials.Count);
                
                foreach(AppContentAssetCustomItemProperty prop 
                        in item.custom_materials) {
                    
                    Debug.Log("prop:code:" + prop.code);  
                    Debug.Log("prop:name:" + prop.name); 
                    foreach(string type in prop.types) {
                        Debug.Log("prop:type:s:" + type);
                    }
                }
            }
            else {                
                Debug.Log("data was NULL" + item.ToJson());
            }

            AppContentAssetCustomItem customItem = item.GetCustomItems();
            
            Debug.Log("customItem:json:" + customItem.ToJson());

            if(customItem != null) {
                if(customItem.properties != null) {
                    foreach(AppContentAssetCustomItemProperty prop in customItem.properties) {
                        Debug.Log("prop:code:" + prop.code); 
                    }
                }
            }
        }
        
        DumpObj(name, "items.Count", items.Count);
    }
    
    public static void TestGameColors_List() {
        
        string name = "TestGameColors_List";
        
        Debug.Log(name);
        
        List<GameColor> colors = GameColors.Instance.GetAll();

        foreach(GameColor color in colors) {
            Color colorTo = ColorHelper.FromRGB(color.color.rgba);
            
            Debug.Log("color:code:" + color.code);
            
            Debug.Log("color:color:" + colorTo);
        }
                
        DumpObj(name, "colors.Count", colors.Count);
    }

    
    
    public static void TestGameColors_Code() {
        
        string name = "TestGameColors_Code";
        
        Debug.Log(name);
        
        GameColor color = GameColors.Instance.GetByCode("game-ucf-knights-gold");

        if(color != null) {
        
            Debug.Log("color:color:" + color.code);
            Debug.Log("color:color:" + color.GetColor());
        }
        else {
            
            Debug.Log("color:NOT FOUND:");
        }


    }

    public static void TestGameState_LoadProfile() {
        
        string name = "TestGameState_LoadProfile";
        
        Debug.Log(name);

        GameState.LoadProfile();

        string username = GameProfiles.Current.username;
            DumpObj(name, "username", username);

        AssertEquals(name, username, "Player");
    }

    public static void TestGameState_SaveProfile() {
        
        string name = "TestGameState_SaveProfile";
        
        Debug.Log(name);
        
        GameState.SaveProfile();
        
        string username = GameProfiles.Current.username;
        DumpObj(name, "username", username);
        
        AssertEquals(name, username, "Player");
    }
    
    public static void TestGameProfileCharacter_GetCharacter() {
        
        string name = "TestGameProfileCharacter_GetCharacter";
        
        Debug.Log(name);

        string characterCode = "default";
                
        GameProfileCharacterItem characterItem = GameProfileCharacters.Current.GetCharacter(characterCode);


        if(characterItem == null) {
            
            DumpObj(name, "characterItem:NULL", characterItem);
        }
        else {
            
            DumpObj(name, "characterItem:EXISTS", characterItem);
            
            DumpObj(name, "characterItem:characterCode", characterItem.characterCode);
            DumpObj(name, "characterItem:characterCostumeCode", characterItem.characterCostumeCode);
            DumpObj(name, "characterItem:code", characterItem.code);

            DumpObj(name, "characterItem:characterCode.profileRPGItem.GetAttack()", 
                    characterItem.profileRPGItem.GetAttack());
        }

        DumpObj(name, "characterItem", characterItem);

        //Debug.Break();
    }
    
    public static void TestGameProfileCharacter_GetCurrentCharacter() {
        
        string name = "TestGameProfileCharacter_GetCurrentCharacter";
        
        Debug.Log(name);
        
        string characterCode = "default";

        GameProfileCharacterItem item = GameProfileCharacters.Current.GetCurrentCharacter();

        item.characterCode = "testercode";
        DataAttribute d = new DataAttribute();
        d.code = "ddd";
        item.SetAttribute(d);

        
        DataAttribute a = new DataAttribute();
        a.code = "aaa";
        item.SetAttribute(a);
                
        if(item == null) {
            
            DumpObj(name, "item:NULL", item);
        }
        else {
            
            DumpObj(name, "item:EXISTS", item.ToJson());
            
            DumpObj(name, "item:characterCode", item.characterCode);
            DumpObj(name, "item:characterCostumeCode", item.characterCostumeCode);
            DumpObj(name, "item:code", item.code);
            
            //DumpObj(name, "characterItem:characterCode.profileCustomItem.code", 
            //        characterItem.profileCustomItem.code);
            //DumpObj(name, "characterItem:characterCode.profileRPGItem.GetAttack()", 
            //        characterItem.profileRPGItem.GetAttack());
        }
        
        DumpObj(name, "item", item);
        
        //Debug.Break();        
    }    
    
    public static void TestGameProfileCharacter_currentCharacter() {
        
        string name = "TestGameProfileCharacter_currentCharacter";
        
        Debug.Log(name);
        
        string characterCode = "default";
        
        GameProfileCharacterItem characterItem = GameProfileCharacters.currentCharacter;
        
        
        if(characterItem == null) {
            
            DumpObj(name, "characterItem:NULL", characterItem);
        }
        else {
            
            DumpObj(name, "characterItem:EXISTS", characterItem);
            
            DumpObj(name, "characterItem:characterCode", characterItem.characterCode);
            DumpObj(name, "characterItem:characterCostumeCode", characterItem.characterCostumeCode);
            DumpObj(name, "characterItem:code", characterItem.code);
            
            //DumpObj(name, "characterItem:characterCode.profileCustomItem.code", 
            //        characterItem.profileCustomItem.code);
            //DumpObj(name, "characterItem:characterCode.profileRPGItem.GetAttack()", 
            //        characterItem.profileRPGItem.GetAttack());
        }
        
        DumpObj(name, "characterItem", characterItem);
        
        //Debug.Break();        
    }

    
    public static void TestGameProfileCharacter_currentProgress() {
        
        string name = "TestGameProfileCharacter_currentCharacter";
        
        Debug.Log(name);
        
        string characterCode = "default";
        
        GameProfilePlayerProgressItem item = GameProfileCharacters.currentProgress;
        
        
        if(item == null) {
            
            DumpObj(name, "item:NULL", item);
        }
        else {
            
            DumpObj(name, "item:EXISTS", item);

            //DumpObj(name, "characterItem:characterCode.profileCustomItem.code", 
            //        characterItem.profileCustomItem.code);
            //DumpObj(name, "characterItem:characterCode.profileRPGItem.GetAttack()", 
            //        characterItem.profileRPGItem.GetAttack());
        }
        
        DumpObj(name, "characterItem", item);
        
        //Debug.Break();
        
    }


    // -----------------------------------------------------------------
    // GAME CHARACTER SKIN

    public static void TestGameCharacterSkin() {

        string name = "TestGameCharacterSkin";
        
        Debug.Log(name);
        
        GameCharacterSkin obj1 = new GameCharacterSkin();
        GameCharacterSkin obj2 = new GameCharacterSkin();
        
        obj1.active = true;
        
        obj1.display_name = "tester";
        
        string obj1Data = JsonMapper.ToJson(obj1);
                
        obj2 = JsonMapper.ToObject<GameCharacterSkin>(obj1Data);
        
        string obj2Data = JsonMapper.ToJson(obj2);

        AssertEquals(name, obj1, obj2);
    }
    
    public static void TestGameCharacterSkinLoadData() {
        
        string name = "TestGameCharacterSkinLoadData";
        
        Debug.Log(name);

        try {
            GameCharacterSkins.Instance.LoadData();

            Debug.Log(name + ":GameCharacterSkins:" + GameCharacterSkins.Instance.items.Count);
            Debug.Log(name + ":SUCCESS:" + true);
        }
        catch(Exception e) {

            Debug.Log(e);
        }
    }







    /*
    
    public static void TestDefault() {
        
        DataKeyedObjectLeaf leaf = new DataKeyedObjectLeaf();
        
        leaf.active = true;
        
        leaf.display_name = "tester";
        
        Debug.Log("DataKeyedObjectLeaf:leaf:display_name:" + leaf.display_name);
        Debug.Log("DataKeyedObjectLeaf:leaf:display_name2:" + leaf.Get(BaseDataObjectKeys.display_name));
        
        string leafData = JsonMapper.ToJson(leaf);
        
        Debug.Log("DataKeyedObjectLeaf:leafData:" + leafData);
        
        DataKeyedObjectLeaf leaf2 = new DataKeyedObjectLeaf();
        
        leaf2 = JsonMapper.ToObject<DataKeyedObjectLeaf>(leafData);
        
        Debug.Log("DataKeyedObjectLeaf:display_name:" + leaf2.display_name);
        
        string leaf2Data = JsonMapper.ToJson(leaf2);
        
        Debug.Log("DataKeyedObjectLeaf:leaf2Data:" + leaf2Data);
        
        
        AssertEquals("DataKeyedObjectLeaf", leaf, leaf2);
    }
    */
    
}


