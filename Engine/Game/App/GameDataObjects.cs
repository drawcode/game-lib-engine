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

public class GameDataDirectorType {
    public static string enemy = "enemy";
    public static string sidekick = "sidekick";
    public static string item = "item";
    public static string weapon = "weapon";
    public static string custom = "custom";
}

public class GameDataDirector : GameDataObject {
    
    public virtual bool run {
        get {
            return Get<bool>(BaseDataObjectKeys.run, true);
        }
        
        set {
            Set(BaseDataObjectKeys.run, value);
        }
    }
}

public class GameDataColorPreset : GameDataObject {
    
}

public class GameDataTexturePreset : GameDataObject {
    
}

public class GameDataItemPreset : GameDataObject {
    
}

public class GameDataWeaponPreset : GameDataObject {

}

public class GameDataTerrainPreset : GameDataObject {
    
}

public class GameDataCharacterPreset : GameDataObject {
    
}

public class GameDataAssetPreset : GameDataObject {
    
}

public class GameDataLayoutPreset : GameDataObject {
    
}

public class GameDataActionKeys {
    
    public static string goal_range_1 = "goal_range_1";
    public static string goal_range_2 = "goal_range_2";
    public static string goal_range_3 = "goal_range_3";
    public static string goal_range_4 = "goal_range_4";
    public static string player_out_of_bounds = "player_out_of_bounds";
    public static string player_action_good = "player_action_good";
    public static string player_action_bad = "player_action_bad";
    public static string player_start = "player_start";
    public static string player_end = "player_end";
    public static string level_start = "level_start";
    public static string level_end = "level_end";
    public static string music_ui_intro = "music_ui_intro";
    public static string music_ui_loop = "music_ui_loop";
    public static string music_game = "music_game";
    public static string scores = "scores";
    public static string score = "score";
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
    public static string walljump = "walljump";
    public static string attack = "attack";
    public static string attack_near = "attack_near";
    public static string attack_far = "attack_far";
    public static string attack_alt = "attack_alt";
    public static string attack_left = "attack_left";
    public static string attack_right = "attack_right";
    public static string hit = "hit";
    public static string death = "death";
    public static string skill = "skill";
    public static string boost = "boost";
    public static string idle = "idle";
    public static string start = "start";
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


    [JsonIgnore]
    public bool isPlayTypeLoop {
        get {
            return data_type == GameDataPlayType.loop
                || data_type == GameDataPlayType.loop_reverse
                || play_type == GameDataPlayType.loop
                || play_type == GameDataPlayType.loop_reverse
                    ? true : false;
        }
    }

    [JsonIgnore]
    public bool isPlayTypeOnce {
        get {
            return data_type == GameDataPlayType.once
                || data_type == GameDataPlayType.once_reverse
                || play_type == GameDataPlayType.once
                || play_type == GameDataPlayType.once_reverse
                    ? true : false;
        }
    }

}

public class GameDataSocial : GameDataObject {   

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
    public static string goalFly = "goal-fly";  
    public static string weapon = "weapon";
    public static string letter = "letter";
    public static string specials = "specials";
    public static string score = "score";
    public static string scores = "scores";
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

    public virtual List<GameDataDirector> directors {
        get {
            return Get<List<GameDataDirector>>(BaseDataObjectKeys.directors);
        }
        
        set {
            Set<List<GameDataDirector>>(BaseDataObjectKeys.directors, value);
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

    public virtual List<GameDataWeaponPreset> weapon_presets {
        get {
            return Get<List<GameDataWeaponPreset>>(BaseDataObjectKeys.weapon_presets);
        }

        set {
            Set<List<GameDataWeaponPreset>>(BaseDataObjectKeys.weapon_presets, value);
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
    
    public virtual List<GameDataCharacterPreset> character_presets {
        get {
            return Get<List<GameDataCharacterPreset>>(BaseDataObjectKeys.character_presets);
        }
        
        set {
            Set<List<GameDataCharacterPreset>>(BaseDataObjectKeys.character_presets, value);
        }
    }
    
    public virtual List<GameDataAssetPreset> asset_presets {
        get {
            return Get<List<GameDataAssetPreset>>(BaseDataObjectKeys.asset_presets);
        }
        
        set {
            Set<List<GameDataAssetPreset>>(BaseDataObjectKeys.asset_presets, value);
        }
    }

    public virtual List<GameDataAssetPreset> preload_presets {
        get {
            return Get<List<GameDataAssetPreset>>(BaseDataObjectKeys.preload_presets);
        }

        set {
            Set<List<GameDataAssetPreset>>(BaseDataObjectKeys.preload_presets, value);
        }
    }
    
    public virtual List<GameDataLayoutPreset> layout_presets {
        get {
            return Get<List<GameDataLayoutPreset>>(BaseDataObjectKeys.layout_presets);
        }
        
        set {
            Set<List<GameDataLayoutPreset>>(BaseDataObjectKeys.layout_presets, value);
        }
    }

    //character_presets

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
        foreach (string role in roles) {
            if (role == roleTo) {
                return true;
            }
        }
        return false;
    }

    // projectiles

    public bool HasProjectiles() {

        if (projectiles != null) {
            if (projectiles.Count > 0) {
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
        
        if (effects != null) {
            if (effects.Count > 0) {
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
        
        if (sounds != null) {
            if (sounds.Count > 0) {
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
        return GetItemsRandom<GameDataSound>(sounds, type);
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

        if (gameDataSound != null) {
            GameAudio.PlayEffect(gameDataSound.code);
        }
    }

    // animations
    
    public bool HasAnimations() {
        
        if (animations != null) {
            if (animations.Count > 0) {
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
    
    public GameDataAnimation GetAnimationsByTypeStart() {
        return GetAnimationByType(GameDataActionKeys.start);
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

    public GameDataAnimation GetAnimationsByTypeSlide() {
        return GetAnimationByType(GameDataActionKeys.slide);
    }

    // models

    public bool HasModels() {
        
        if (models != null) {
            if (models.Count > 0) {
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
        
        if (color_presets != null) {
            if (color_presets.Count > 0) {
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
        
        if (texture_presets != null) {
            if (texture_presets.Count > 0) {
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
        
        if (terrain_presets != null) {
            if (terrain_presets.Count > 0) {
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

    // character presets
    
    public bool HasCharacterPresets() {
        
        if (character_presets != null) {
            if (character_presets.Count > 0) {
                return true;
            }
        }
        
        return false;
    }
    
    public GameDataCharacterPreset GetCharacterPreset() {
        return GetCharacterPreset(GameDataItemTypeKeys.defaultType);
    }
    
    public GameDataCharacterPreset GetCharacterPreset(string code) {
        return GetItem<GameDataCharacterPreset>(character_presets, code);
    }


    // item presets

    public bool HasItemPresets() {

        if (item_presets != null) {
            if (item_presets.Count > 0) {
                return true;
            }
        }

        return false;
    }

    public GameDataItemPreset GetItemPreset() {
        return GetItemPreset(GameDataItemTypeKeys.defaultType);
    }

    public GameDataItemPreset GetItemPreset(string code) {
        return GetItem<GameDataItemPreset>(item_presets, code);
    }

    // weapon presets

    public bool HasWeaponPresets() {

        if (weapon_presets != null) {
            if (weapon_presets.Count > 0) {
                return true;
            }
        }

        return false;
    }

    public GameDataWeaponPreset GetWeaponPreset() {
        return GetWeaponPreset(GameDataItemTypeKeys.defaultType);
    }

    public GameDataWeaponPreset GetWeaponPreset(string code) {
        return GetItem<GameDataWeaponPreset>(weapon_presets, code);
    }

    // asset presets
    
    public bool HasAssetPresets() {
        
        if (asset_presets != null) {
            if (asset_presets.Count > 0) {
                return true;
            }
        }
        
        return false;
    }
    
    public GameDataAssetPreset GetAssetPreset() {
        return GetAssetPreset(GameDataItemTypeKeys.defaultType);
    }
    
    public GameDataAssetPreset GetAssetPreset(string code) {
        return GetItem<GameDataAssetPreset>(asset_presets, code);
    }
        
    // layout presets
    
    public bool HasLayoutPresets() {
        
        if (layout_presets != null) {
            if (layout_presets.Count > 0) {
                return true;
            }
        }
        
        return false;
    }
    
    public GameDataLayoutPreset GetLayoutPreset() {
        return GetLayoutPreset(GameDataItemTypeKeys.defaultType);
    }
    
    public GameDataLayoutPreset GetLayoutPreset(string code) {
        return GetItem<GameDataLayoutPreset>(layout_presets, code);
    }
    
    // rpgs
    
    public bool HasRPGs() {
        
        if (rpgs != null) {
            if (rpgs.Count > 0) {
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
        
        if (rewards != null) {
            if (rewards.Count > 0) {
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
        
        foreach (T item in items) {
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
        app_state = GameFilterIncludeType.none;
        app_content_state = GameFilterIncludeType.none;
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

public class GameDataObjectLocalized : GameDataObject {

    
    /*
    public override V Get<V>(string code, V defaultValue) {                
        V val = base.Get<V>(code, defaultValue);

        if(EqualityComparer<V>.Default.Equals(val, default(V))) {
            return val;
        }

        if(val.GetType() == typeof(string)) {
            // Localize if it is a string

            string valToLocalize = (string)Convert.ChangeType(val, typeof(string));//Convert.ToString(defaultValue);

            if(!string.IsNullOrEmpty(valToLocalize)) {
                string valLocalized = Locos.GetReplaceLocalized(valToLocalize);
                val = (V)Convert.ChangeType(valLocalized, typeof(V));
            }
        }

        return val;
    }
    */
        
    public string GetLocalized(string code, string defaultValue) {          
        string valTo = Get<string>(code, defaultValue);
        
        if (!string.IsNullOrEmpty(valTo)) {
            valTo = Locos.GetReplaceLocalized(valTo);
        }
        
        return valTo;
    }
    
    //[JsonIgnore]
    public override string display_name {
        get {
            return GetLocalized(BaseDataObjectKeys.display_name, "");
        }
        
        set {
            Set<string>(BaseDataObjectKeys.display_name, value);
        }
    }
    
    public override string action_display_name {
        get {
            return GetLocalized(BaseDataObjectKeys.action_display_name, "");
        }
        
        set {
            Set<string>(BaseDataObjectKeys.action_display_name, value);
        }
    }  
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public override string name {
        get {
            return GetLocalized(BaseDataObjectKeys.name, "");//Get<string>(BaseDataObjectKeys.name);
        }
        
        set {
            Set<string>(BaseDataObjectKeys.name, value);
        }
    }   
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public override string description {
        get {
            return GetLocalized(BaseDataObjectKeys.description, "");//Get<string>(BaseDataObjectKeys.description);
        }
        
        set {
            Set<string>(BaseDataObjectKeys.description, value);
        }
    }
    
    public override string action_description {
        get {
            return GetLocalized(BaseDataObjectKeys.action_description, "");//Get<string>(BaseDataObjectKeys.action_description);
        }
        
        set {
            Set<string>(BaseDataObjectKeys.action_description, value);
        }
    }

    public override string content {
        get {
            return GetLocalized(BaseDataObjectKeys.content, "");//Get<string>(BaseDataObjectKeys.content);
        }
        
        set {
            Set<string>(BaseDataObjectKeys.content, value);
            
            hash = CryptoUtil.CalculateBase64SHA1ASCII(value);
        }
    } 
}

[Serializable]
public class GameDataObject : DataObject {

    // Dataobject handles keys

    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string uuid {
        get {
            return Get<string>(BaseDataObjectKeys.uuid, "");//UniqueUtil.CreateUUID4());
        }

        set {
            Set<string>(BaseDataObjectKeys.uuid, value);
        }
    }

    public virtual string uid {
        get {
            return Get<string>(BaseDataObjectKeys.uid, "");
        }

        set {
            Set<string>(BaseDataObjectKeys.uid, value);
        }
    }

    public virtual string profile_id {
        get {
            return Get<string>(BaseDataObjectKeys.profile_id, "");
        }

        set {
            Set<string>(BaseDataObjectKeys.profile_id, value);
        }
    }

    public virtual string game_id {
        get {
            return Get<string>(BaseDataObjectKeys.game_id, "");
        }

        set {
            Set<string>(BaseDataObjectKeys.game_id, value);
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
    public virtual string hash {
        get {
            return Get<string>(BaseDataObjectKeys.hash, "");
        }

        set {
            Set<string>(BaseDataObjectKeys.hash, value);
        }
    }

    public virtual string email {
        get {
            return Get<string>(BaseDataObjectKeys.email);
        }

        set {
            Set<string>(BaseDataObjectKeys.email, value);
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

    public virtual string action_display_name {
        get {
            return Get<string>(BaseDataObjectKeys.action_display_name, "");
        }

        set {
            Set<string>(BaseDataObjectKeys.action_display_name, value);
        }
    }

    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string name {
        get {
            return Get<string>(BaseDataObjectKeys.name, "");//Get<string>(BaseDataObjectKeys.name);
        }

        set {
            Set<string>(BaseDataObjectKeys.name, value);
        }
    }

    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string description {
        get {
            return Get<string>(BaseDataObjectKeys.description, "");//Get<string>(BaseDataObjectKeys.description);
        }

        set {
            Set<string>(BaseDataObjectKeys.description, value);
        }
    }

    public virtual string action_description {
        get {
            return Get<string>(BaseDataObjectKeys.action_description, "");//Get<string>(BaseDataObjectKeys.action_description);
        }

        set {
            Set<string>(BaseDataObjectKeys.action_description, value);
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

    public virtual string path {
        get {
            return Get<string>(BaseDataObjectKeys.path);
        }

        set {
            Set<string>(BaseDataObjectKeys.path, value);
        }
    }

    public virtual string content {
        get {
            return Get<string>(BaseDataObjectKeys.content, "");//Get<string>(BaseDataObjectKeys.content);
        }

        set {
            Set<string>(BaseDataObjectKeys.content, value);

            hash = CryptoUtil.CalculateBase64SHA1ASCII(value);
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

    public virtual string network_id {
        get {
            return Get<string>(BaseDataObjectKeys.network_id);
        }

        set {
            Set<string>(BaseDataObjectKeys.network_id, value);
        }
    }

    public virtual string network_username {
        get {
            return Get<string>(BaseDataObjectKeys.network_username, "");
        }

        set {
            Set<string>(BaseDataObjectKeys.network_username, value);
        }
    }

    public virtual string network_type {
        get {
            return Get<string>(BaseDataObjectKeys.network_type, "");
        }

        set {
            Set<string>(BaseDataObjectKeys.network_type, value);
        }
    }

    public virtual string network_token {
        get {
            return Get<string>(BaseDataObjectKeys.network_token, "");
        }

        set {
            Set<string>(BaseDataObjectKeys.network_token, value);
        }
    }

    public virtual object network_data {
        get {
            return Get<object>(BaseDataObjectKeys.network_data);
        }

        set {
            Set<object>(BaseDataObjectKeys.network_data, value);
        }
    }

    //

    public virtual string mission_type {
        get {
            return Get<string>(BaseDataObjectKeys.mission_type, "");
        }

        set {
            Set<string>(BaseDataObjectKeys.mission_type, value);
        }
    }

    public virtual string mission_key {
        get {
            return Get<string>(BaseDataObjectKeys.mission_key, "");
        }

        set {
            Set<string>(BaseDataObjectKeys.mission_key, value);
        }
    }

    public virtual string mission_code {
        get {
            return Get<string>(BaseDataObjectKeys.mission_code, "");
        }

        set {
            Set<string>(BaseDataObjectKeys.mission_code, value);
        }
    }

    public virtual object mission_data {
        get {
            return Get<object>(BaseDataObjectKeys.mission_data, null);
        }

        set {
            Set<object>(BaseDataObjectKeys.mission_data, value);
        }
    }

    // 


    public virtual string action_type {
        get {
            return Get<string>(BaseDataObjectKeys.action_type, "");
        }

        set {
            Set<string>(BaseDataObjectKeys.action_type, value);
        }
    }

    public virtual string action_key {
        get {
            return Get<string>(BaseDataObjectKeys.action_key, "");
        }

        set {
            Set<string>(BaseDataObjectKeys.action_key, value);
        }
    }

    public virtual string action_code {
        get {
            return Get<string>(BaseDataObjectKeys.action_code, "");
        }

        set {
            Set<string>(BaseDataObjectKeys.action_code, value);
        }
    }

    public virtual object action_data {
        get {
            return Get<object>(BaseDataObjectKeys.action_data, null);
        }

        set {
            Set<object>(BaseDataObjectKeys.action_data, value);
        }
    }

    //

    public virtual string collection_type {
        get {
            return Get<string>(BaseDataObjectKeys.collection_type, "");
        }

        set {
            Set<string>(BaseDataObjectKeys.collection_type, value);
        }
    }

    public virtual string collection_code {
        get {
            return Get<string>(BaseDataObjectKeys.collection_code, "");
        }

        set {
            Set<string>(BaseDataObjectKeys.collection_code, value);
        }
    }

    public virtual object collection_data {
        get {
            return Get<object>(BaseDataObjectKeys.collection_data, null);
        }

        set {
            Set<object>(BaseDataObjectKeys.collection_data, value);
        }
    }

    //

    public virtual string choice_type {
        get {
            return Get<string>(BaseDataObjectKeys.choice_type, "");
        }

        set {
            Set<string>(BaseDataObjectKeys.choice_type, value);
        }
    }

    public virtual string choice_code {
        get {
            return Get<string>(BaseDataObjectKeys.choice_code, "");
        }

        set {
            Set<string>(BaseDataObjectKeys.choice_code, value);
        }
    }

    public virtual object choice_data {
        get {
            return Get<object>(BaseDataObjectKeys.choice_data, null);
        }

        set {
            Set<object>(BaseDataObjectKeys.choice_data, value);
        }
    }

    //

    public virtual int level_num {
        get {
            return Get<int>(BaseDataObjectKeys.level_num, 1);
        }

        set {
            Set<int>(BaseDataObjectKeys.level_num, value);
        }
    }

    public virtual string level_code {
        get {
            return Get<string>(BaseDataObjectKeys.level_code, "");
        }

        set {
            Set<string>(BaseDataObjectKeys.level_code, value);
        }
    }

    //

    public virtual int world_num {
        get {
            return Get<int>(BaseDataObjectKeys.world_num, 1);
        }

        set {
            Set<int>(BaseDataObjectKeys.world_num, value);
        }
    }

    public virtual string world_code {
        get {
            return Get<string>(BaseDataObjectKeys.world_code, "");
        }

        set {
            Set<string>(BaseDataObjectKeys.world_code, value);
        }
    }

    public virtual string world_type {
        get {
            return Get<string>(BaseDataObjectKeys.world_type, null);
        }

        set {
            Set<string>(BaseDataObjectKeys.world_type, value);
        }
    }

    public virtual object world_data {
        get {
            return Get<object>(BaseDataObjectKeys.world_data, "");
        }

        set {
            Set<object>(BaseDataObjectKeys.world_data, value);
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

    public virtual string compare_type {
        get {
            return Get<string>(BaseDataObjectKeys.compare_type, "");
        }

        set {
            Set<string>(BaseDataObjectKeys.compare_type, value);
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

    public virtual string display_type {
        get {
            return Get<string>(BaseDataObjectKeys.display_type, "");
        }

        set {
            Set<string>(BaseDataObjectKeys.display_type, value);
        }
    }

    public virtual string load_type {
        get {
            return Get<string>(BaseDataObjectKeys.load_type, "");
        }

        set {
            Set<string>(BaseDataObjectKeys.load_type, value);
        }
    }

    public virtual string ui {
        get {
            return Get<string>(BaseDataObjectKeys.ui, "");
        }

        set {
            Set<string>(BaseDataObjectKeys.ui, value);
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
            return Get<double>(BaseDataObjectKeys.max, 1);
        }

        set {
            Set<double>(BaseDataObjectKeys.max, value);
        }
    }

    public virtual double min {
        get {
            return Get<double>(BaseDataObjectKeys.min, 0);
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
            return Convert.ToDouble(val);
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

    public virtual double delta {
        get {
            return Get<double>(BaseDataObjectKeys.delta);
        }

        set {
            Set<double>(BaseDataObjectKeys.delta, value);
        }
    }

    public virtual double last_time {
        get {
            return Get<double>(BaseDataObjectKeys.last_time);
        }

        set {
            Set<double>(BaseDataObjectKeys.last_time, value);
        }
    }

    public virtual double last_update {
        get {
            return Get<double>(BaseDataObjectKeys.last_update);
        }

        set {
            Set<double>(BaseDataObjectKeys.last_update, value);
        }
    }

    public virtual double modifier {
        get {
            return Get<double>(BaseDataObjectKeys.modifier, 1.0);
        }

        set {
            Set<double>(BaseDataObjectKeys.modifier, value);
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

    public virtual string app_state {
        get {
            return Get<string>(BaseDataObjectKeys.app_state);
        }

        set {
            Set(BaseDataObjectKeys.app_state, value);
        }
    }

    public virtual string app_content_state {
        get {
            return Get<string>(BaseDataObjectKeys.app_content_state);
        }

        set {
            Set(BaseDataObjectKeys.app_content_state, value);
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

    public virtual string play_type {
        get {
            return Get<string>(BaseDataObjectKeys.play_type);
        }

        set {
            Set<string>(BaseDataObjectKeys.play_type, value);
        }
    }

    public virtual double play_delay {
        get {
            return Get<double>(BaseDataObjectKeys.play_delay, 1.0);
        }

        set {
            Set<double>(BaseDataObjectKeys.play_delay, value);
        }
    }

    // app level

    public virtual List<string> tags {
        get {
            return Get<List<string>>(BaseDataObjectKeys.tags);
        }

        set {
            Set(BaseDataObjectKeys.tags, value);
        }
    }

    public virtual List<string> app_states {
        get {
            return Get<List<string>>(BaseDataObjectKeys.app_states);
        }

        set {
            Set(BaseDataObjectKeys.app_states, value);
        }
    }

    public virtual List<string> app_content_states {
        get {
            return Get<List<string>>(BaseDataObjectKeys.app_content_states);
        }

        set {
            Set(BaseDataObjectKeys.app_content_states, value);
        }
    }

    public virtual Dictionary<string, List<string>> required_assets {
        get {
            return Get<Dictionary<string, List<string>>>(BaseDataObjectKeys.required_assets);
        }

        set {
            Set(BaseDataObjectKeys.required_assets, value);
        }
    }

    public virtual List<string> required_packs {
        get {
            return Get<List<string>>(BaseDataObjectKeys.required_packs);
        }

        set {
            Set(BaseDataObjectKeys.required_packs, value);
        }
    }

    public virtual List<string> required_trackers {
        get {
            return Get<List<string>>(BaseDataObjectKeys.required_trackers);
        }

        set {
            Set(BaseDataObjectKeys.required_trackers, value);
        }
    }

    public virtual List<string> keys {
        get {
            return Get<List<string>>(BaseDataObjectKeys.keys);
        }

        set {
            Set(BaseDataObjectKeys.keys, value);
        }
    }

    public virtual Dictionary<string, object> content_attributes {
        get {
            return Get<Dictionary<string, object>>(BaseDataObjectKeys.content_attributes);
        }

        set {
            Set(BaseDataObjectKeys.content_attributes, value);
        }
    }

    public virtual List<string> types {
        get {
            return Get<List<string>>(BaseDataObjectKeys.types);
        }

        set {
            Set<List<string>>(BaseDataObjectKeys.types, value);
        }
    }

    public virtual double delay {
        get {
            return Get<double>(BaseDataObjectKeys.delay);
        }

        set {
            Set<double>(BaseDataObjectKeys.delay, value);
        }
    }

    public virtual double time {
        get {
            return Get<double>(BaseDataObjectKeys.time);
        }

        set {
            Set<double>(BaseDataObjectKeys.time, value);
        }
    }

    public virtual double points {
        get {

            double valFinal = 0;
            object valObj = Get<object>(BaseDataObjectKeys.points);

            if (valObj == null) {
                return valFinal;
            }

            if (valObj.GetType() == typeof(double)) {
                valFinal = Get<double>(BaseDataObjectKeys.points);
            }
            else if (valObj.GetType() == typeof(int)) {
                valFinal = (double)Get<int>(BaseDataObjectKeys.points);
            }

            return valFinal;
        }

        set {
            if (value.GetType() == typeof(double)) {
                Set<double>(BaseDataObjectKeys.points, value);
            }
            else if (value.GetType() == typeof(int)) {
                Set<double>(BaseDataObjectKeys.points, (double)value);
            }
        }
    }

    public virtual bool complete {
        get {
            return Get<bool>(BaseDataObjectKeys.complete);
        }

        set {
            Set<bool>(BaseDataObjectKeys.complete, value);
        }
    }

    public virtual string ease_type { // linear, easeInOut, easeIn, easeOut
        get {
            return Get<string>(BaseDataObjectKeys.ease_type);
        }

        set {
            Set<string>(BaseDataObjectKeys.ease_type, value);
        }
    }

    public virtual Vector3Data local_position_data {
        get {
            return Get<Vector3Data>(BaseDataObjectKeys.local_position_data);
        }

        set {
            Set<Vector3Data>(BaseDataObjectKeys.local_position_data, value);
        }
    }

    public virtual Vector3Data local_rotation_data {
        get {
            return Get<Vector3Data>(BaseDataObjectKeys.local_rotation_data);
        }

        set {
            Set<Vector3Data>(BaseDataObjectKeys.local_rotation_data, value);

        }
    }

    public virtual Vector3Data position_data {
        get {
            return Get<Vector3Data>(BaseDataObjectKeys.position_data);
        }

        set {
            Set<Vector3Data>(BaseDataObjectKeys.position_data, value);
        }
    }

    public virtual Vector3Data grid_data {
        get {
            return Get<Vector3Data>(BaseDataObjectKeys.grid_data);
        }

        set {
            Set<Vector3Data>(BaseDataObjectKeys.grid_data, value);
        }
    }

    public virtual Vector3Data rotation_data {
        get {
            return Get<Vector3Data>(BaseDataObjectKeys.rotation_data);
        }

        set {
            Set<Vector3Data>(BaseDataObjectKeys.rotation_data, value);

        }
    }

    public virtual Vector3Data scale_data {
        get {
            return Get<Vector3Data>(BaseDataObjectKeys.scale_data);
        }

        set {
            Set<Vector3Data>(BaseDataObjectKeys.scale_data, value);
        }
    }

    public virtual List<string> data_list {
        get {
            return Get<List<string>>(BaseDataObjectKeys.data_list);
        }

        set {
            Set<List<string>>(BaseDataObjectKeys.data_list, value);
        }
    }

    public virtual Dictionary<string, object> data_dict {
        get {
            return Get<Dictionary<string, object>>(BaseDataObjectKeys.data_dict);
        }

        set {
            Set<Dictionary<string, object>>(BaseDataObjectKeys.data_dict, value);
        }
    }

    public virtual List<Dictionary<string, object>> data_dicts {
        get {
            return Get<List<Dictionary<string, object>>>(BaseDataObjectKeys.data_dicts);
        }

        set {
            Set<List<Dictionary<string, object>>>(BaseDataObjectKeys.data_dicts, value);
        }
    }

    public virtual List<GameDataObject> data_game_objects {
        get {
            return Get<List<GameDataObject>>(BaseDataObjectKeys.data_game_objects);
        }

        set {
            Set<List<GameDataObject>>(BaseDataObjectKeys.data_game_objects, value);
        }
    }

    public virtual List<GameDataObject> data_objects {
        get {
            return Get<List<GameDataObject>>(BaseDataObjectKeys.data_objects);
        }

        set {
            Set<List<GameDataObject>>(BaseDataObjectKeys.data_objects, value);
        }
    }

    public virtual Dictionary<string, GameDataObject> data_object {
        get {
            return Get<Dictionary<string, GameDataObject>>(BaseDataObjectKeys.data_object);
        }

        set {
            Set<Dictionary<string, GameDataObject>>(BaseDataObjectKeys.data_object, value);
        }
    }


    // helpers

    public bool IsTypeTexture() {
        return IsType("texture");
    }

    public bool IsTypeColor() {
        return IsType("color");
    }

    public bool IsType(string typeTo) {

        bool isTypes = false;

        if (types != null) {
            isTypes = types.Contains(typeTo) ? true : false;
        }

        bool isType = type == typeTo ? true : false;

        return isType || isTypes ? true : false;
    }

    public bool HasTag(string tagTo) {
        if (tags == null) {
            return false;
        }

        return tags.Contains(tagTo);
    }

    public bool IsCode(string val) {
        return code == val ? true : false;
    }

    public bool IsDataType(string val) {
        return data_type == val ? true : false;
    }

    public bool IsValGreaterThanOrEqual(double valTo) {
        if (valDouble >= valTo) {
            return true;
        }

        return false;
    }

    //

    public GameDataObject() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }

    // helpers

    public List<T> GetItemsRandom<T>(List<T> list, string type) where T : GameDataObject, new() {

        List<T> objs = new List<T>();

        List<T> items = GetItems<T>(list, type);

        foreach (T item in items) {
            if (item.data_type == "preset") {
                // lookup preset
                GamePreset preset = GamePresets.Get(item.code);
                if (preset != null) {
                    GamePresetItem presetItem = GetItemRandomByProbability(preset.data.items);
                    T obj = new T();
                    obj.type = item.type;
                    obj.code = presetItem.code;
                    obj.layer = item.layer;
                    obj.play_delay = item.play_delay;
                    obj.play_type = item.play_type;
                    objs.Add(obj);
                }
            }
            else {

                objs.Add(GetItemRandomByType(list, type));
            }
        }

        return objs;
    }

    public T GetItemRandom<T>(List<T> list, string type) where T : GameDataObject, new() {

        T obj = default(T);

        List<T> items = GetItems<T>(list, type);

        foreach (T item in items) {
            if (item.data_type == "preset") {
                // lookup preset
                GamePreset preset = GamePresets.Get(item.code);
                if (preset != null) {
                    GamePresetItem presetItem = GetItemRandomByProbability(preset.data.items);
                    obj = new T();
                    obj.type = item.type;
                    obj.code = presetItem.code;
                    obj.layer = item.layer;
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

        if (items != null) {
            if (items.Count > 0) {
                int index = UnityEngine.Random.Range(0, items.Count);
                obj = items[index];
            }
        }

        return obj;
    }

    public List<T> GetItems<T>(List<T> list, string type) where T : GameDataObject {

        List<T> filteredList = new List<T>();

        if (list == null)
            return filteredList;

        if (list.Count > 0) {
            foreach (T item in list) {
                if (item.type == type) {
                    filteredList.Add(item);
                }
            }
        }

        return filteredList;
    }

    public T GetItem<T>(List<T> list, string code) where T : GameDataObject {

        if (list == null)
            return default(T);

        if (list.Count > 0) {
            foreach (T item in list) {
                if (item.code == code) {
                    return item;
                }
            }

            foreach (T item in list) {
                return item;
            }
        }

        return null;
    }

    public T GetDataDictValue<T>(string key) {

        if (data_dict != null) {
            if (data_dict.ContainsKey(key)) {
                return data_dict.Get<T>(key);
            }
        }

        return default(T);
    }

    public Dictionary<string, object> GetDataDictsDictByKeyValue(string key, string val) {

        if (data_dicts != null) {

            foreach (Dictionary<string, object> dict in data_dicts) {

                string keyValue = dict.Get<string>(key);

                if (keyValue == val) {
                    return dict;
                }
            }
        }

        return null;
    }
}