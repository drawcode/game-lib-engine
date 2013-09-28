using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Engine.Data.Json;
using Engine.Utility;

public class BaseGameProfileRPGAttributes {
	
	// RPG	
	public static string ATT_PROGRESS_CURRENCY = "progress-currency";
	public static string ATT_PROGRESS_XP = "progress-xp";
	public static string ATT_PROGRESS_HEALTH = "progress-health";
	public static string ATT_PROGRESS_ENERGY = "progress-energy";
	public static string ATT_PROGRESS_LEVEL = "progress-level";
	
	public static string ATT_PROGRESS_UPGRADES_APPLIED = "progress-upgrades-applied";
	public static string ATT_PROGRESS_UPGRADES = "progress-upgrades";
}

public class BaseGameProfileRPGs {
	private static volatile BaseGameProfileRPG current;
	private static volatile BaseGameProfileRPGs instance;
	private static object syncRoot = new Object();
	
	public static string DEFAULT_USERNAME = "Player";
	
	public static BaseGameProfileRPG BaseCurrent {
		get {
	    	if (current == null) {
	        	lock (syncRoot) {
	           		if (current == null) 
	              		current = new BaseGameProfileRPG();
	        	}
	     	}
	
	     	return current;
	  	}
		set {
			current = value;
		}
	}
		
	public static BaseGameProfileRPGs BaseInstance {
	  get {
	     if (instance == null) {
	        lock (syncRoot) {
	           if (instance == null) 
	              instance = new BaseGameProfileRPGs();
	        }
	     }
	
	     return instance;
	  }
	}
	
	// TODO: Common profile actions, lookup, count, etc
}

public class GameItemRPG : DataObject {
	public double speed = 0.1;
	public double attack = 0.1;
	public double defense = 0.1;
	public double health = 0.1;
	public double energy = 0.1;
	public double jump = 0.1;
	public double fly = 0.1;
	public double boost = 0.1;
	public double attack_speed = 0.1;
	public double recharge_speed = 0.1;
	public double upgrades_applied = 0.0;
	public double upgrades = 0.0;
	public string data = "";	
}

public class GameItemRPGAttributes {
	public static string prefix = "game-item-rpg-";
	public static string upgrades_applied = prefix + "upgrades_applied";
	public static string upgrades = prefix + "upgrades";
	public static string speed = prefix + "speed";
	public static string attack = prefix + "attack";
	public static string defense = prefix + "defense";
	public static string health = prefix + "health";
	public static string jump = prefix + "jump";
	public static string fly = prefix + "fly";
	public static string boost = prefix + "boost";
	public static string energy = prefix + "energy";
	public static string attack_speed = prefix + "attack_speed";
	public static string recharge_speed = prefix + "recharge_speed";
	public static string data = prefix + "data";
}

public class GameProfileRPGItem : DataObject {
	
	public GameProfileRPGItem() {
		
	}
	
	public void LoadFromGameItemRPG(GameItemRPG itemRPG) {
		SetSpeed(itemRPG.speed);
		SetAttack(itemRPG.attack);
		SetDefense(itemRPG.defense);
		SetHealth(itemRPG.health);
		SetEnergy(itemRPG.energy);
		SetAttackSpeed(itemRPG.attack_speed);
		SetRechargeSpeed(itemRPG.recharge_speed);
		SetUpgradesApplied(itemRPG.upgrades_applied);
		SetUpgrades(itemRPG.upgrades);
		SetData(itemRPG.data);
	}
	
	public GameItemRPG GetGameItemRPG() {
		GameItemRPG itemRPG = new GameItemRPG();
		itemRPG.speed = GetSpeed();
		itemRPG.attack = GetAttack();
		itemRPG.defense = GetDefense();
		itemRPG.health = GetHealth();
		itemRPG.energy = GetEnergy();
		itemRPG.attack_speed = GetAttackSpeed();
		itemRPG.recharge_speed = GetRechargeSpeed();
		itemRPG.upgrades_applied = GetUpgradesApplied();
		itemRPG.upgrades = GetUpgrades();
		itemRPG.data = GetData();
		return itemRPG;
	}
	
	// properties
	
	// upgrades_applied
	
	public double GetUpgradesApplied(){
		return GetUpgradesApplied(0.0);
	}
	
	public double GetUpgradesApplied(double defaultValue){		
		double attValue = defaultValue;
		if(CheckIfAttributeExists(GameItemRPGAttributes.upgrades_applied))
			attValue = GetAttributeDoubleValue(GameItemRPGAttributes.upgrades_applied);
		return attValue;
	}
		
	public void SetUpgradesApplied(double val) {
		SetAttributeDoubleValue(GameItemRPGAttributes.upgrades_applied, val);
	}
	
	// upgrades
	
	public double GetUpgrades(){
		return GetUpgrades(0.0);
	}
	
	public double GetUpgrades(double defaultValue){		
		double attValue = defaultValue;
		if(CheckIfAttributeExists(GameItemRPGAttributes.upgrades))
			attValue = GetAttributeDoubleValue(GameItemRPGAttributes.upgrades);
		return attValue;
	}
		
	public void SetUpgrades(double val) {
		SetAttributeDoubleValue(GameItemRPGAttributes.upgrades, val);
	}
	
	// speed
	
	public double GetSpeed(){
		return GetSpeed(0.1);
	}
	
	public double GetSpeed(double defaultValue){		
		double attValue = defaultValue;
		if(CheckIfAttributeExists(GameItemRPGAttributes.speed))
			attValue = GetAttributeDoubleValue(GameItemRPGAttributes.speed);
		return attValue;
	}
		
	public void SetSpeed(double val) {
		SetAttributeDoubleValue(GameItemRPGAttributes.speed, val);
	}
	
	// attack
	
	public double GetAttack(){
		return GetAttack(0.1);
	}
	
	public double GetAttack(double defaultValue){		
		double attValue = defaultValue;
		if(CheckIfAttributeExists(GameItemRPGAttributes.attack))
			attValue = GetAttributeDoubleValue(GameItemRPGAttributes.attack);
		return attValue;
	}
		
	public void SetAttack(double val) {
		SetAttributeDoubleValue(GameItemRPGAttributes.attack, val);
	}	
	
	// defense
		
	public double GetDefense(){
		return GetDefense(0.1);
	}
	
	public double GetDefense(double defaultValue){		
		double attValue = defaultValue;
		if(CheckIfAttributeExists(GameItemRPGAttributes.defense))
			attValue = GetAttributeDoubleValue(GameItemRPGAttributes.defense);
		return attValue;
	}
		
	public void SetDefense(double val) {
		SetAttributeDoubleValue(GameItemRPGAttributes.defense, val);
	}
	
	// health
		
	public double GetHealth(){
		return GetHealth(0.1);
	}
	
	public double GetHealth(double defaultValue){		
		double attValue = defaultValue;
		if(CheckIfAttributeExists(GameItemRPGAttributes.health))
			attValue = GetAttributeDoubleValue(GameItemRPGAttributes.health);
		return attValue;
	}
		
	public void SetHealth(double val) {
		SetAttributeDoubleValue(GameItemRPGAttributes.health, val);
	}
	
	// energy
	
	public double GetEnergy(){
		return GetEnergy(0.1);
	}
	
	public double GetEnergy(double defaultValue){		
		double attValue = defaultValue;
		if(CheckIfAttributeExists(GameItemRPGAttributes.energy))
			attValue = GetAttributeDoubleValue(GameItemRPGAttributes.energy);
		return attValue;
	}
		
	public void SetEnergy(double val) {
		SetAttributeDoubleValue(GameItemRPGAttributes.energy, val);
	}
	
	// attack_speed
	
	public double GetAttackSpeed(){
		return GetAttackSpeed(0.1);
	}
	
	public double GetAttackSpeed(double defaultValue){		
		double attValue = defaultValue;
		if(CheckIfAttributeExists(GameItemRPGAttributes.attack_speed))
			attValue = GetAttributeDoubleValue(GameItemRPGAttributes.attack_speed);
		return attValue;
	}
	
	public void SetAttackSpeed(double val) {
		SetAttributeDoubleValue(GameItemRPGAttributes.attack_speed, val);
	}
	
	// recharge_speed
	
	public double GetRechargeSpeed(){
		return GetRechargeSpeed(0.1);
	}
	
	public double GetRechargeSpeed(double defaultValue){		
		double attValue = defaultValue;
		if(CheckIfAttributeExists(GameItemRPGAttributes.recharge_speed))
			attValue = GetAttributeDoubleValue(GameItemRPGAttributes.recharge_speed);
		return attValue;
	}
		
	public void SetRechargeSpeed(double val) {
		SetAttributeDoubleValue(GameItemRPGAttributes.recharge_speed, val);
	}
	
	// data
	
	public string GetData(){
		return GetData("");
	}
	
	public string GetData(string defaultValue){		
		string attValue = defaultValue;
		if(CheckIfAttributeExists(GameItemRPGAttributes.data))
			attValue = GetAttributeStringValue(GameItemRPGAttributes.data);
		return attValue;
	}
			
	public void SetData(string val) {
		SetAttributeStringValue(GameItemRPGAttributes.data, val);
	}	
}

public class BaseGameProfileRPG : Profile  {
	// BE CAREFUL adding properties as they will cause a need for a profile conversion
	// Best way to add items to the profile is the GetAttribute and SetAttribute class as 
	// that stores as a generic DataAttribute class.  Booleans, strings, objects, serialized json objects etc
	// all work well and cause no need to convert profile on updates. 
		
	public BaseGameProfileRPG() {
		Reset();
	}
	
	public override void Reset() {
		base.Reset();
		username = "Player";// + UnityEngine.Random.Range(1, 9999999);
	}
	
	// upgrades_applied
	
	public virtual double GetUpgradesApplied(){
		return GetUpgradesApplied(0.0);
	}
	
	public virtual double GetUpgradesApplied(double defaultValue){		
		double attValue = defaultValue;
		if(CheckIfAttributeExists(GameItemRPGAttributes.upgrades_applied))
			attValue = GetAttributeDoubleValue(GameItemRPGAttributes.upgrades_applied);
		return attValue;
	}
		
	public virtual void SetUpgradesApplied(double val) {
		SetAttributeDoubleValue(GameItemRPGAttributes.upgrades_applied, val);
	}
	
	// upgrades
	
	public virtual double GetUpgrades(){
		return GetUpgrades(3.0);
	}
	
	public virtual double GetUpgrades(double defaultValue){		
		double attValue = defaultValue;
		if(CheckIfAttributeExists(GameItemRPGAttributes.upgrades))
			attValue = GetAttributeDoubleValue(GameItemRPGAttributes.upgrades);
		return attValue;
	}
		
	public virtual void SetUpgrades(double val) {
		SetAttributeDoubleValue(GameItemRPGAttributes.upgrades, val);
	}
		
	// customizations		
	
	public virtual void SetValue(string code, object value) {
		DataAttribute att = new DataAttribute();
		att.val = value;
		att.code = code;
		att.name = "";
		att.type = "bool";
		att.otype = "rpg";
		SetAttribute(att);
	}
		
	public virtual List<DataAttribute> GetList() {
		return GetAttributesList("rpg");
	}
	
	
	// CURRENCY
		
    public virtual double GetGamePlayerProgressCurrency() {
        return GetGamePlayerProgressCurrency(10.0);
    }

    public virtual double GetGamePlayerProgressCurrency(double defaultValue) {
        double attValue = defaultValue;
        if (CheckIfAttributeExists(BaseGameProfileRPGAttributes.ATT_PROGRESS_CURRENCY))
            attValue = GetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_CURRENCY);
        return attValue;
    }

    public virtual void SetGamePlayerProgressCurrency(double attValue) {
        SetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_CURRENCY, attValue);
    }
	
		// RPG - Player specific
	
	public double GetGamePlayerProgressXP(){
		return GetGamePlayerProgressXP(10.0);
	}
	
	public double GetGamePlayerProgressXP(double defaultValue){
		double attValue = defaultValue;
		if(CheckIfAttributeExists(BaseGameProfileRPGAttributes.ATT_PROGRESS_XP))
			attValue = GetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_XP);
		return attValue;
	}
	
	public void SetGamePlayerProgressXP(double attValue) {
		SetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_XP, attValue);
	}
	
	public double GetGamePlayerProgressLevel(){
		return GetGamePlayerProgressLevel(0.0);
	}
	
	public double GetGamePlayerProgressLevel(double defaultValue){
		
		double attValue = defaultValue;
		if(CheckIfAttributeExists(BaseGameProfileRPGAttributes.ATT_PROGRESS_LEVEL))
			attValue = GetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_LEVEL);
		return attValue;
	}
	
	public void SetGamePlayerProgressLevel(double attValue) {
		SetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_LEVEL, attValue);
	}	
	
	public double GetGamePlayerProgressHealth(){
		return GetGamePlayerProgressHealth(1.0);
	}
	
	public double GetGamePlayerProgressHealth(double defaultValue){
		
		double attValue = defaultValue;
		if(CheckIfAttributeExists(BaseGameProfileRPGAttributes.ATT_PROGRESS_HEALTH))
			attValue = GetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_HEALTH);
		return attValue;
	}
	
	public void SetGamePlayerProgressHealth(double attValue) {
		SetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_HEALTH, attValue);
	}	
	
	
	public double GetGamePlayerProgressEnergy(){
		return GetGamePlayerProgressEnergy(0.0);
	}
	
	public double GetGamePlayerProgressEnergy(double defaultValue){
		
		double attValue = defaultValue;
		if(CheckIfAttributeExists(BaseGameProfileRPGAttributes.ATT_PROGRESS_ENERGY))
			attValue = GetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_ENERGY);
		return attValue;
	}
	
	public void SetGamePlayerProgressEnergy(double attValue) {
		SetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_ENERGY, attValue);
	}
	
	
}


