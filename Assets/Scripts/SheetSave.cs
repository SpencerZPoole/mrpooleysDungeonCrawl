using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SheetSave{
    public int strengthScore;
    public int constitutionScore;
    public int dexterityScore;
    public int intelligenceScore;
    public int wisdomScore;
    public int charismaScore;

    public int armorClass;
    public int fortitudeScore;
    public int reflexScore;
    public int willScore;
    public int baseDefense;
    public int healingSurgeValue;

    public bool bleeding;
    public bool slowed;
    public bool justCrit;

    public bool isEnemy;

    public int proficiencyBonus;
    public int wDmg;
    public string equippedWeapon;
    public string weaponType;
    public int turnWaitTime;
    public List<string> healingPotionsInventory;
    public List<string> manaPotionsInventory;

    public int currentHP;
    public int maxHP;
    public int currentMP;
    public int maxMP;
    public int hpGainedPerLevel;
    public int mpGainedPerLevel;
    public int totalXP;
    public int myXPValue;
    public int myGoldValue;
    public int myGold;
    public float characterLevel;

    public string armor;
    public float armorBonus;
    public int shieldBonus;
    public float extraACBonuses;

    public string ownerUsername;
    public string characterClass;
    public bool classIsAssigned;
    public string gender;
    public string race;

    public bool waitingForAction;
    public int waitingForActionTimeLeft;
    public bool taunt;
    public bool addingXP;
    public bool addingGold;

    public int tempAttackBonus;
    public int tempSavingThrowBonus;

    public bool readyForNextBattle;
    public bool startedDyingRoutine;

}
