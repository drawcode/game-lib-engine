using System;
using System.Collections.Generic;
using System.IO;
using Engine.Data.Json;
using Engine.Utility;


public class GameDataModel : GameDataObject {
    
    public virtual string textures {
        get {
            return Get<string>(BaseDataObjectKeys.textures);
        }
        
        set {
            Set<string>(BaseDataObjectKeys.textures, value);
        }
    } 
    
    public virtual string colors {
        get {
            return Get<string>(BaseDataObjectKeys.colors);
        }
        
        set {
            Set<string>(BaseDataObjectKeys.colors, value);
        }
    }
}

public class GameDataColorPreset : GameDataObject {
    
}

public class GameDataTexturePreset : GameDataObject {
    
}

public class GameDataItemPreset : GameDataObject {
    
}

public class GameDataTerrainPreset : GameDataObject {
    
}

public class GameDataActionKeys {

    public static string load = "load";
    public static string collect = "collect";
    public static string reward = "reward";
    public static string spin = "spin";

    
    public static string footsteps = "footsteps";
    public static string speed = "speed";
    public static string mount = "mount";
    public static string use = "use";
    public static string walk = "walk";
    public static string run = "run";
    
    public static string walk_back = "walk_back";
    public static string run_back = "run_back";

    public static string attack = "attack";
    public static string attack_near = "attack_near";
    public static string attack_far = "attack_far";
    public static string attack_alt = "attack_alt";
    public static string attack_left = "attack_left";
    public static string attack_right = "attack_right";
    public static string hit = "hit";
    public static string death = "death";
    public static string skill = "skill";
    public static string idle = "idle";
    public static string jump = "jump";
    public static string win = "win";
    public static string lose = "lost";
    public static string fail = "fail";
    public static string emote = "emote";
    public static string strafe = "strafe";
    public static string strafe_left = "strafe_left";
    public static string strafe_right = "strafe_right";
    public static string defend = "defend";
    public static string defend_far = "defend_far";
    public static string defend_near = "defend_near";
    public static string defend_alt = "defend_alt";
    public static string defend_left = "defend_left";
    public static string defend_right = "defend_right";
    public static string boost = "boost";
    public static string pickup = "pickup";

    public static string punch = "punch";
    public static string kick = "kick";
    public static string slide = "slide";
    public static string crouch = "crouch";
    public static string crouch_run = "crouch_run";
    public static string crouch_walk = "crouch_walk";

}

public class GameDataAnimation : GameDataObject {

}


public class GameDataSound : GameDataObject {

}

public class GameDataItemEffect : GameDataObject {
    
}

public class GameDataItemProjectile : GameDataObject {
    
}

public class GameDataItemReward : GameDataObject {

    public static string xp = "xp";
    public static string currency = "currency";
    public static string currencyDouble = "currency-double";
    public static string health = "health";    
}

public class GameDataItemTypeKeys {
    public static string defaultType = "default";
}

public class GameDataItemKeys {
    public static string prepareType = "prepare";
    public static string postType = "post";
    public static string runType = "run";

    public static string shotType = "shot";
    public static string loadType = "load";
}

public class GameDataObjectItem : GameDataObject {
    
    public virtual List<string> roles {
        get {
            return Get<List<string>>(BaseDataObjectKeys.roles);
        }
        
        set {
            Set<List<string>>(BaseDataObjectKeys.roles, value);
        }
    } 
    
    public virtual List<GameDataModel> models {
        get {
            return Get<List<GameDataModel>>(BaseDataObjectKeys.models);
        }
        
        set {
            Set<List<GameDataModel>>(BaseDataObjectKeys.models, value);
        }
    } 
    
    public virtual List<GameDataColorPreset> color_presets {
        get {
            return Get<List<GameDataColorPreset>>(BaseDataObjectKeys.color_presets);
        }
        
        set {
            Set<List<GameDataColorPreset>>(BaseDataObjectKeys.color_presets, value);
        }
    } 
    
    public virtual List<GameDataTexturePreset> texture_presets {
        get {
            return Get<List<GameDataTexturePreset>>(BaseDataObjectKeys.texture_presets);
        }
        
        set {
            Set<List<GameDataTexturePreset>>(BaseDataObjectKeys.texture_presets, value);
        }
    } 
    
    public virtual List<GameDataItemPreset> item_presets {
        get {
            return Get<List<GameDataItemPreset>>(BaseDataObjectKeys.item_presets);
        }
        
        set {
            Set<List<GameDataItemPreset>>(BaseDataObjectKeys.item_presets, value);
        }
    } 
        
    public virtual List<GameDataTerrainPreset> terrain_presets {
        get {
            return Get<List<GameDataTerrainPreset>>(BaseDataObjectKeys.terrain_presets);
        }
        
        set {
            Set<List<GameDataTerrainPreset>>(BaseDataObjectKeys.terrain_presets, value);
        }
    } 
    
    public virtual List<GameDataSound> sounds {
        get {
            return Get<List<GameDataSound>>(BaseDataObjectKeys.sounds);
        }
        
        set {
            Set<List<GameDataSound>>(BaseDataObjectKeys.sounds, value);
        }
    }    

    public virtual List<GameDataAnimation> animations {
        get {
            return Get<List<GameDataAnimation>>(BaseDataObjectKeys.animations);
        }
        
        set {
            Set<List<GameDataAnimation>>(BaseDataObjectKeys.animations, value);
        }
    }    
    
    public virtual List<GameDataItemProjectile> projectiles {
        get {
            return Get<List<GameDataItemProjectile>>(BaseDataObjectKeys.projectiles);
        }
        
        set {
            Set<List<GameDataItemProjectile>>(BaseDataObjectKeys.projectiles, value);
        }
    } 
        
    public virtual List<GameDataItemEffect> effects {
        get {
            return Get<List<GameDataItemEffect>>(BaseDataObjectKeys.effects);
        }
        
        set {
            Set<List<GameDataItemEffect>>(BaseDataObjectKeys.effects, value);
        }
    } 
    
    public virtual List<GameDataItemRPG> rpgs {
        get {
            return Get<List<GameDataItemRPG>>(BaseDataObjectKeys.rpgs);
        }
        
        set {
            Set<List<GameDataItemRPG>>(BaseDataObjectKeys.rpgs, value);
        }
    } 

    
    public virtual List<GameDataItemReward> rewards {
        get {
            return Get<List<GameDataItemReward>>(BaseDataObjectKeys.rewards);
        }
        
        set {
            Set<List<GameDataItemReward>>(BaseDataObjectKeys.rewards, value);
        }
    } 

    // roles

    [JsonIgnore]
    public bool isHero {
        get {
            return IsRole("hero");
        }
    }
    
    [JsonIgnore]
    public bool isEnemy {
        get {
            return IsRole("enemy");
        }
    }
    
    [JsonIgnore]
    public bool isSidekick {
        get {
            return IsRole("sidekick");
        }
    }

    public bool IsRole(string roleTo) {
        foreach(string role in roles) {
            if(role == roleTo) {
                return true;
            }
        }
        return false;
    }

    // projectiles

    public bool HasProjectiles() {

        if(projectiles != null) {
            if(projectiles.Count > 0) {
                return true;
            }
        }

        return false;
    }
    
    public GameDataItemProjectile GetProjectile() {
        return GetProjectile(GameDataItemTypeKeys.defaultType);
    }
    
    public GameDataItemProjectile GetProjectile(string code) {
        return GetItem<GameDataItemProjectile>(projectiles, code);
    }
    
    public List<GameDataItemProjectile> GetProjectileListByType(string type) {
        return GetItems<GameDataItemProjectile>(projectiles, type);
    }
    
    public GameDataItemProjectile GetProjectileByType(string type) {
        // get random item
        return GetItemRandomByType<GameDataItemProjectile>(projectiles, type);
    }
    
    public GameDataItemProjectile GetProjectilesByTypeShot() {
        return GetProjectileByType(GameDataItemKeys.shotType);
    }
    
    public GameDataItemProjectile GetProjectilesByTypeLoad() {
        return GetProjectileByType(GameDataItemKeys.loadType);
    }
        
    // effects
    
    public bool HasEffects() {
        
        if(effects != null) {
            if(effects.Count > 0) {
                return true;
            }
        }
        
        return false;
    }
    
    public GameDataItemEffect GetEffect() {
        return GetEffect(GameDataItemTypeKeys.defaultType);
    }
    
    public GameDataItemEffect GetEffect(string code) {
        return GetItem<GameDataItemEffect>(effects, code);
    }
    
    public List<GameDataItemEffect> GetEffectListByType(string type) {
        return GetItems<GameDataItemEffect>(effects, type);
    }
    
    public GameDataItemEffect GetEffectByType(string type) {
        // get random item
        return GetItemRandomByType<GameDataItemEffect>(effects, type);
    }
    
    public GameDataItemEffect GetEffectsByTypeShot() {
        return GetEffectByType(GameDataItemKeys.shotType);
    }
    
    public GameDataItemEffect GetEffectsByTypeLoad() {
        return GetEffectByType(GameDataItemKeys.loadType);
    }


    // sounds
    
    public bool HasSounds() {
        
        if(sounds != null) {
            if(sounds.Count > 0) {
                return true;
            }
        }
        
        return false;
    }
    
    public GameDataSound GetSound() {
        return GetSound(GameDataItemTypeKeys.defaultType);
    }
    
    public GameDataSound GetSound(string code) {
        return GetItem<GameDataSound>(sounds, code);
    }

    public List<GameDataSound> GetSoundListByType(string type) {
        return GetItems<GameDataSound>(sounds, type);
    }

    public GameDataSound GetSoundByType(string type) {
        // get random item
        return GetItemRandom<GameDataSound>(sounds, type);
    }

    public GameDataSound GetSoundsByTypeShot() {
        return GetSoundByType(GameDataActionKeys.attack);
    }
    
    public GameDataSound GetSoundsByTypeLoad() {
        return GetSoundByType(GameDataActionKeys.load);
    }

    public void PlaySoundType(string type) {
        
        GameDataSound gameDataSound = GetSoundByType(GameDataActionKeys.reward);

        if(gameDataSound != null) {
            GameAudio.PlayEffect(gameDataSound.code);
        }
    }

    // animations
    
    public bool HasAnimations() {
        
        if(animations != null) {
            if(animations.Count > 0) {
                return true;
            }
        }
        
        return false;
    }
    
    public GameDataAnimation GetAnimation() {
        return GetAnimation(GameDataItemTypeKeys.defaultType);
    }
    
    public GameDataAnimation GetAnimation(string code) {
        return GetItem<GameDataAnimation>(animations, code);
    }
    
    public List<GameDataAnimation> GetAnimationListByType(string type) {
        return GetItems<GameDataAnimation>(animations, type);
    }
    
    public GameDataAnimation GetAnimationByType(string type) {
        // get random item
        return GetItemRandom<GameDataAnimation>(animations, type);
    }

    public GameDataAnimation GetAnimationsByTypeIdle() {
        return GetAnimationByType(GameDataActionKeys.idle);
    }
    
    public GameDataAnimation GetAnimationsByTypeAttack() {
        return GetAnimationByType(GameDataActionKeys.attack);
    }       
    
    public GameDataAnimation GetAnimationsByTypeAttackNear() {
        return GetAnimationByType(GameDataActionKeys.attack_near);
    }
    
    public GameDataAnimation GetAnimationsByTypeAttackFar() {
        return GetAnimationByType(GameDataActionKeys.attack_far);
    }
    
    public GameDataAnimation GetAnimationsByTypeHit() {
        return GetAnimationByType(GameDataActionKeys.hit);
    }
        
    public GameDataAnimation GetAnimationsByTypeJump() {
        return GetAnimationByType(GameDataActionKeys.jump);
    }
            
    public GameDataAnimation GetAnimationsByTypeRun() {
        return GetAnimationByType(GameDataActionKeys.run);
    }
    
    public GameDataAnimation GetAnimationsByTypeWalk() {
        return GetAnimationByType(GameDataActionKeys.walk);
    }
    
    public GameDataAnimation GetAnimationsByTypeDeath() {
        return GetAnimationByType(GameDataActionKeys.death);
    }
    
    public GameDataAnimation GetAnimationsByTypeSkill() {
        return GetAnimationByType(GameDataActionKeys.skill);
    }
    
    public GameDataAnimation GetAnimationsByTypeWin() {
        return GetAnimationByType(GameDataActionKeys.win);
    }
    
    public GameDataAnimation GetAnimationsByTypeFail() {
        return GetAnimationByType(GameDataActionKeys.fail);
    }
    
    public GameDataAnimation GetAnimationsByTypeEmote() {
        return GetAnimationByType(GameDataActionKeys.emote);
    }

    // models
        
    public bool HasModels() {
        
        if(models != null) {
            if(models.Count > 0) {
                return true;
            }
        }
        
        return false;
    }
    
    public GameDataModel GetModel() {
        return GetModel(GameDataItemTypeKeys.defaultType);
    }
    
    public GameDataModel GetModel(string code) {
        return GetItem<GameDataModel>(models, code);
    }
    
    // color presets
    
    public bool HasColorPresets() {
        
        if(color_presets != null) {
            if(color_presets.Count > 0) {
                return true;
            }
        }
        
        return false;
    }
    
    public GameDataColorPreset GetColorPreset() {
        return GetColorPreset(GameDataItemTypeKeys.defaultType);
    }
    
    public GameDataColorPreset GetColorPreset(string code) {
        return GetItem<GameDataColorPreset>(color_presets, code);
    }
    
    // texture presets
    
    public bool HasTexturePresets() {
        
        if(texture_presets != null) {
            if(texture_presets.Count > 0) {
                return true;
            }
        }
        
        return false;
    }
    
    public GameDataTexturePreset GetTexturePreset() {
        return GetTexturePreset(GameDataItemTypeKeys.defaultType);
    }
    
    public GameDataTexturePreset GetTexturePreset(string code) {
        return GetItem<GameDataTexturePreset>(texture_presets, code);
    }

    // terrain presets
    
    public bool HasTerrainPresets() {
        
        if(terrain_presets != null) {
            if(terrain_presets.Count > 0) {
                return true;
            }
        }
        
        return false;
    }
    
    public GameDataTexturePreset GetTerrainPreset() {
        return GetTexturePreset(GameDataItemTypeKeys.defaultType);
    }
    
    public GameDataTerrainPreset GetTerrainPreset(string code) {
        return GetItem<GameDataTerrainPreset>(terrain_presets, code);
    }
    
    // rpgs
    
    public bool HasRPGs() {
        
        if(rpgs != null) {
            if(rpgs.Count > 0) {
                return true;
            }
        }
        
        return false;
    }
    
    public GameDataItemRPG GetRPG() {
        return GetRPG(GameDataItemTypeKeys.defaultType);
    }
    
    public GameDataItemRPG GetRPG(string code) {
        return GetItem<GameDataItemRPG>(rpgs, code);
    }

    
    // rewards
    
    public bool HasRewards() {
        
        if(rewards != null) {
            if(rewards.Count > 0) {
                return true;
            }
        }
        
        return false;
    }
    
    public GameDataItemReward GetReward() {
        return GetReward(GameDataItemTypeKeys.defaultType);
    }
    
    public GameDataItemReward GetReward(string code) {
        return GetItem<GameDataItemReward>(rewards, code);
    }
    
    public List<GameDataItemReward> GetRewardListByType(string type) {
        return GetItems<GameDataItemReward>(rewards, type);
    }
    
    public GameDataItemReward GetRewardByType(string type) {
        // get random item
        return GetItemRandomByType<GameDataItemReward>(rewards, type);
    }
    
    public GameDataItemReward GetRewardsByTypeCurrency() {
        return GetRewardByType(GameDataItemReward.currency);
    }
    
    public GameDataItemReward GetRewardsByTypeXP() {
        return GetRewardByType(GameDataItemReward.xp);
    }
    
    public GameDataItemReward GetRewardsByTypeHealth() {
        return GetRewardByType(GameDataItemReward.health);
    }
    
    public GameDataItemReward GetRewardsByTypeCurrencyDouble() {
        return GetRewardByType(GameDataItemReward.currencyDouble);
    }
}

public class GameDataPresetsObject<T> : GameDataObject where T : GameDataObject, new() {
    
    public virtual List<T> items {
        get {
            return Get<List<T>>(BaseDataObjectKeys.items);
        }
        
        set {
            Set<List<T>>(BaseDataObjectKeys.items, value);
        }
    }
    
    public T ChooseProbabilistic() {
                
        List<float> probs = new List<float>();
        
        foreach(T item in items) {
            probs.Add((float)item.probability);
        }
        
        T selectByProbabilityItem = 
            MathUtil.ChooseProbability<T>(items, probs);  
        
        return selectByProbabilityItem;
    }
}

public class GamePresetItems<T> : GameDataPresetsObject<T> where T : GameDataObject, new() {

    public GamePresetItems() {

    }
}

public class GamePresetItem : GameDataObject {
    
}

public class GameFilterType {
    public static string statisticSingle = "statistic-single";
    public static string statisticAll = "statistic-all";
    public static string statisticLike = "statistic-like";
    public static string statisticCompare = "statistic-compare";
    public static string statisticSet = "statistic-set";
    public static string achievementSet = "achievement-set";
}

public class GameFilterIncludeType {
    public static string none = "none";
    public static string current = "current";
    public static string all = "all";
}

public class GameCompareType {
    public static string like = "like";
    public static string equal = "equal";
    public static string startsWith = "startsWith";
    public static string endsWith = "endsWith";
    public static string all = "all";
}

public class GameFilterIncludeKeys : GameDataObject {
    
    public GameFilterIncludeKeys() {
        Reset();
    }
    
    public override void Reset() {
        base.Reset();
        
        defaultKey = GameFilterIncludeType.none;
        pack = GameFilterIncludeType.none;
        tracker = GameFilterIncludeType.none;
        action = GameFilterIncludeType.none;
        appState = GameFilterIncludeType.none;
        appContentState = GameFilterIncludeType.none;
        custom = GameFilterIncludeType.none;
    }
}


public class GameFilterBase : GameDataObject {    
    
    public GameFilterBase() {
        Reset();
    }
    
    public override void Reset() {
        base.Reset();
        codeType = GameCompareType.equal;
        compareType = StatEqualityTypeString.STAT_GREATER_THAN;
        compareValue = 1.0;
        includeKeys = new GameFilterIncludeKeys();
    }
    
    public virtual GameFilterIncludeKeys includeKeys {
        get {
            return Get<GameFilterIncludeKeys>(BaseDataObjectKeys.includeKeys);
        }
        
        set {
            Set(BaseDataObjectKeys.includeKeys, value);
        }
    }
    
    public virtual List<string> codes {
        get {
            return Get<List<string>>(BaseDataObjectKeys.codes);
        }
        
        set {
            Set(BaseDataObjectKeys.codes, value);
        }
    }
    
    public virtual string codeType {
        get {
            return Get<string>(BaseDataObjectKeys.codeType);
        }
        
        set {
            Set(BaseDataObjectKeys.codeType, value);
        }
    }
    
    public virtual string compareType {
        get {
            return Get<string>(BaseDataObjectKeys.compareType);
        }
        
        set {
            Set(BaseDataObjectKeys.compareType, value);
        }
    }
    
    public virtual double compareValue {
        get {
            return Get<double>(BaseDataObjectKeys.compareValue);
        }
        
        set {
            Set(BaseDataObjectKeys.compareValue, value);
        }
    }
    
    
    public virtual string codeCompareTo {
        get {
            return Get<string>(BaseDataObjectKeys.codeCompareTo);
        }
        
        set {
            Set<string>(BaseDataObjectKeys.codeCompareTo, value);
        }
    }
    
    public virtual string codeLike {
        get {
            return Get<string>(BaseDataObjectKeys.codeLike);
        }
        
        set {
            Set<string>(BaseDataObjectKeys.codeLike, value);
        }
    }
    
}

public class GameFilterStatisticSingle : GameFilterBase {
    
}

public class GameFilterStatisticSet : GameFilterBase {
    
}

public class GameFilterStatisticLike : GameFilterBase {
    
}

public class GameFilterStatisticCompare : GameFilterBase {
    
}

public class GameFilterStatisticAll : GameFilterBase {
    
}

public class GameFilterAchievementSet : GameFilterBase {
    
}

public class GameFilter : GameDataObject {
    
    public virtual GameFilterBase data {
        get {
            return Get<GameFilterBase>(BaseDataObjectKeys.data);
        }
        
        set {
            Set(BaseDataObjectKeys.data, value);
        }
    }
}

public class GameDataObjectMeta : GameDataObject {

    public virtual GameDataObjectItem data {
        get {
            return Get<GameDataObjectItem>(BaseDataObjectKeys.data);
        }
        
        set {
            Set<GameDataObjectItem>(BaseDataObjectKeys.data, value);
        }
    } 
}

public class GameDataObject : DataObject {

    // Dataobject handles keys
        
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string uuid {
        get { 
            return Get<string>(BaseDataObjectKeys.uuid, UniqueUtil.Instance.CreateUUID4());
        }
        
        set {
            Set<string>(BaseDataObjectKeys.uuid, value);
        }
    }
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string code {
        get {
            return Get<string>(BaseDataObjectKeys.code, "");
        }
        
        set {
            Set<string>(BaseDataObjectKeys.code, value);
        }
    }
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string display_name {
        get {
            return Get<string>(BaseDataObjectKeys.display_name, "");
        }
        
        set {
            Set<string>(BaseDataObjectKeys.display_name, value);
        }
    }    
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string name {
        get {
            return Get<string>(BaseDataObjectKeys.name);
        }
        
        set {
            Set<string>(BaseDataObjectKeys.name, value);
        }
    }   
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string description {
        get {
            return Get<string>(BaseDataObjectKeys.description);
        }
        
        set {
            Set<string>(BaseDataObjectKeys.description, value);
        }
    }   

    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string url {
        get {
            return Get<string>(BaseDataObjectKeys.url, "");
        }
        
        set {
            Set<string>(BaseDataObjectKeys.url, value);
        }
    }

    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string host {
        get {
            return Get<string>(BaseDataObjectKeys.host, "");
        }
        
        set {
            Set<string>(BaseDataObjectKeys.host, value);
        }
    }
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    /*
    public virtual object data {
        get {
            return Get<object>(BaseDataObjectKeys.data);
        }
        
        set {
            Set<object>(BaseDataObjectKeys.data, value);
        }
    }       
    */
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual int sort_order {
        get {
            return Get<int>(BaseDataObjectKeys.sort_order, 0);
        }
        
        set {
            Set<int>(BaseDataObjectKeys.sort_order, value);
        }
    }  
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual int sort_order_type {
        get {
            return Get<int>(BaseDataObjectKeys.sort_order_type, 0);
        }
        
        set {
            Set<int>(BaseDataObjectKeys.sort_order_type, value);
        }
    }  
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual bool active {
        get {
            return Get<bool>(BaseDataObjectKeys.active, true);
        }
        
        set {
            Set<bool>(BaseDataObjectKeys.active, value);
        }
    }      
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string key {
        get {
            return Get<string>(BaseDataObjectKeys.key, "");
        }
        
        set {
            Set<string>(BaseDataObjectKeys.key, value);
        }
    }    
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string game_id {
        get {
            return Get<string>(BaseDataObjectKeys.game_id, UniqueUtil.Instance.CreateUUID4());
        }
        
        set {
            Set<string>(BaseDataObjectKeys.game_id, value);
        }
    }    
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string type {
        get {
            return Get<string>(BaseDataObjectKeys.type, "");
        }
        
        set {
            Set<string>(BaseDataObjectKeys.type, value);
        }
    }    

    public virtual string data_type {
        get {
            return Get<string>(BaseDataObjectKeys.data_type, "");
        }
        
        set {
            Set<string>(BaseDataObjectKeys.data_type, value);
        }
    }
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string order_by {
        get {
            return Get<string>(BaseDataObjectKeys.order_by, "");
        }
        
        set {
            Set<string>(BaseDataObjectKeys.order_by, value);
        }
    }      
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string status {
        get {
            return Get<string>(BaseDataObjectKeys.status, "");
        }
        
        set {
            Set<string>(BaseDataObjectKeys.status, value);
        }
    }  
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string pack_code {
        get {
            return Get<string>(BaseDataObjectKeys.pack_code, "");
        }
        
        set {
            Set<string>(BaseDataObjectKeys.pack_code, value);
        }
    }  
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual int pack_sort {
        get {
            return Get<int>(BaseDataObjectKeys.pack_sort, 0);
        }
        
        set {
            Set<int>(BaseDataObjectKeys.pack_sort, value);
        }
    } 
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual DateTime date_created {
        get {
            return Get<DateTime>(BaseDataObjectKeys.date_created, DateTime.Now);
        }
        
        set {
            Set<DateTime>(BaseDataObjectKeys.date_created, value);
        }
    } 
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual DateTime date_modified {
        get {
            return Get<DateTime>(BaseDataObjectKeys.date_modified, DateTime.Now);
        }
        
        set {
            Set<DateTime>(BaseDataObjectKeys.date_modified, value);
        }
    }
        
    public virtual string username {
        get {
            return Get<string>(BaseDataObjectKeys.username);
        }
        
        set {
            Set<string>(BaseDataObjectKeys.username, value);
        }
    }
    
    public virtual string udid {
        get {
            return Get<string>(BaseDataObjectKeys.udid);
        }
        
        set {
            Set<string>(BaseDataObjectKeys.udid, value);
        }
    }
    
    public virtual string file_name {
        get {
            return Get<string>(BaseDataObjectKeys.file_name);
        }
        
        set {
            Set<string>(BaseDataObjectKeys.file_name, value);
        }
    }
    
    public virtual string file_path {
        get {
            return Get<string>(BaseDataObjectKeys.file_path);
        }
        
        set {
            Set<string>(BaseDataObjectKeys.file_path, value);
        }
    }
    
    public virtual string file_full_path {
        get {
            return Get<string>(BaseDataObjectKeys.file_full_path);
        }
        
        set {
            Set<string>(BaseDataObjectKeys.file_full_path, value);
        }
    }

    // game

    public virtual string asset {
        get {
            return Get<string>(BaseDataObjectKeys.asset);
        }
        
        set {
            Set<string>(BaseDataObjectKeys.asset, value);
        }
    }
        
    public virtual double limit {
        get {
            return Get<double>(BaseDataObjectKeys.limit);
        }
        
        set {
            Set<double>(BaseDataObjectKeys.limit, value);
        }
    }
        
    public virtual double max {
        get {
            return Get<double>(BaseDataObjectKeys.max);
        }
        
        set {
            Set<double>(BaseDataObjectKeys.max, value);
        }
    }
    
    public virtual double min {
        get {
            return Get<double>(BaseDataObjectKeys.min);
        }
        
        set {
            Set<double>(BaseDataObjectKeys.min, value);
        }
    }

    public virtual double frequency {
        get {
            return Get<double>(BaseDataObjectKeys.frequency);
        }
        
        set {
            Set<double>(BaseDataObjectKeys.frequency, value);
        }
    }    
    
    public virtual double probability {
        get {
            return Get<double>(BaseDataObjectKeys.probability);
        }
        
        set {
            Set<double>(BaseDataObjectKeys.probability, value);
        }
    }
        
    public virtual object val {
        get {
            return Get<object>(BaseDataObjectKeys.val);
        }
        
        set {
            Set<object>(BaseDataObjectKeys.val, value);
        }
    }
        
    public virtual string valString {
        get {
            return Get<string>(BaseDataObjectKeys.val);
        }
        
        set {
            Set<string>(BaseDataObjectKeys.val, value);
        }
    }
    
    public virtual int valInt {
        get {
            return Get<int>(BaseDataObjectKeys.val);
        }
        
        set {
            Set<int>(BaseDataObjectKeys.val, value);
        }
    }
        
    public virtual double valDouble {
        get {
            return Get<double>(BaseDataObjectKeys.val);
        }
        
        set {
            Set<double>(BaseDataObjectKeys.val, value);
        }
    }
    
    public virtual float valFloat {
        get {
            return Get<float>(BaseDataObjectKeys.val);
        }
        
        set {
            Set<float>(BaseDataObjectKeys.val, value);
        }
    }

    // achievements

    public virtual string defaultKey {
        get {
            return Get<string>(BaseDataObjectKeys.defaultKey);
        }
        
        set {
            Set(BaseDataObjectKeys.defaultKey, value);
        }
    }
    
    public virtual string pack {
        get {
            return Get<string>(BaseDataObjectKeys.pack);
        }
        
        set {
            Set(BaseDataObjectKeys.pack, value);
        }
    }
    
    
    public virtual string tracker {
        get {
            return Get<string>(BaseDataObjectKeys.tracker);
        }
        
        set {
            Set(BaseDataObjectKeys.tracker, value);
        }
    }
    
    public virtual string action {
        get {
            return Get<string>(BaseDataObjectKeys.action);
        }
        
        set {
            Set(BaseDataObjectKeys.action, value);
        }
    }
    
    public virtual string appState {
        get {
            return Get<string>(BaseDataObjectKeys.appState);
        }
        
        set {
            Set(BaseDataObjectKeys.appState, value);
        }
    }
    
    public virtual string appContentState {
        get {
            return Get<string>(BaseDataObjectKeys.appContentState);
        }
        
        set {
            Set(BaseDataObjectKeys.appContentState, value);
        }
    }
    
    public virtual string custom {
        get {
            return Get<string>(BaseDataObjectKeys.custom);
        }
        
        set {
            Set(BaseDataObjectKeys.custom, value);
        }
    }
    
    
    public virtual int layer {
        get { 
            return Get<int>(BaseDataObjectKeys.layer, 1);
        }
        
        set {
            Set<int>(BaseDataObjectKeys.layer, value);
        }
    }

    //

    public GameDataObject() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }
    
    // helpers

    
    public T GetItemRandom<T>(List<T> list, string type) where T : GameDataObject, new() {
        
        T obj = default(T);
        
        List<T> items = GetItems<T>(list, type);

        foreach(T item in items) {
            if(item.data_type == "preset") {
                // lookup preset
                GamePreset preset = GamePresets.Get(item.code);
                if(preset != null) {
                    GamePresetItem presetItem = GetItemRandomByProbability(preset.data.items);
                    obj = new T();
                    obj.type = item.type;
                    obj.code = presetItem.code;
                    return obj;
                }
            }
            else {
                // randomly pick from sets
                return GetItemRandomByType(list, type);
            }
        }
        
        return obj;

    }
    
    public T GetItemRandomByProbability<T>(List<T> list) where T : GameDataObject {
        
        T obj = default(T);
                
        List<float> probs = new List<float>();
        foreach (T item in list) {
            probs.Add((float)item.probability);
        }
        
        obj = MathUtil.ChooseProbability<T>(list, probs); 
        
        return obj;
    }

    
    public T GetItemRandomByProbability<T>(List<T> list, string type) where T : GameDataObject {
        
        T obj = default(T);
        
        List<T> items = GetItems<T>(list, type);
        
        List<float> probs = new List<float>();
        foreach (T item in items) {
            probs.Add((float)item.probability);
        }

        obj = MathUtil.ChooseProbability<T>(items, probs); 

        return obj;
    }

    public T GetItemRandomByType<T>(List<T> list, string type) where T : GameDataObject {
        
        T obj = default(T);
        
        List<T> items = GetItems<T>(list, type);
        
        if(items != null) {
            if(items.Count > 0) {
                int index = UnityEngine.Random.Range(0, items.Count);
                obj = items[index];
            }
        }
        
        return obj;
    }
    
    public List<T> GetItems<T>(List<T> list, string type) where T : GameDataObject {
        
        List<T> filteredList = new List<T>();
        
        if(list == null) 
            return filteredList;
        
        if(list.Count > 0) {
            foreach(T item in list) {
                if(item.type == type) {
                    filteredList.Add(item);
                }
            }
        }
        
        return filteredList;
    }
    
    public T GetItem<T>(List<T> list, string code) where T : GameDataObject {
        
        if(list == null) 
            return default(T);
        
        if(list.Count > 0) {
            foreach(T item in list) {
                if(item.code == code) {
                    return item;
                }
            }
            
            foreach(T item in list) {
                return item;
            }
        }
        
        return null;
    }
}