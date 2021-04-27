using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterSheet : MonoBehaviour {

    public Color usernameColor = Color.black;
    public int strengthScore;
    public int constitutionScore;
    public int dexterityScore;
    public int intelligenceScore;
    public int wisdomScore;
    public int charismaScore;

    public int armorClass = 0;
    public int fortitudeScore;
    public int reflexScore;
    public int willScore;
    public int baseDefense;
    public int healingSurgeValue = 0;

    public bool bleeding = false;
    public bool slowed = false;
    public bool justCrit = false;

    public bool isEnemy = false;

    public int proficiencyBonus = 0;
    public int wDmg = 0;
    public string equippedWeapon;
    public string weaponType;
    public int turnWaitTime = 10;
    public List<string> healingPotionsInventory;
    public List<string> manaPotionsInventory;

    public int currentHP = 99;
    public int maxHP = 99;
    public int currentMP = 99;
    public int maxMP = 99;
    public int hpGainedPerLevel = 0;
    public int mpGainedPerLevel = 0;
    public int totalXP = 0;
    public int myXPValue = 0;
    public int myGoldValue = 0;
    public int myGold = 0;
    public float characterLevel = 1;

    public string armor;
    public float armorBonus = 0;
    public int shieldBonus = 0;
    public float extraACBonuses = 0;

    public string ownerUsername;
    public string characterClass;
    public bool classIsAssigned = false;
    public string gender;
    public string race = "human";

    public Brain1 brain;

    public bool waitingForAction = false;
    public int waitingForActionTimeLeft = 0;
    public bool taunt = false;
    public bool addingXP = false;
    public bool addingGold = false;

    public int tempAttackBonus = 0;
    public int tempSavingThrowBonus = 0;

    public GameObject attachedPanel;
    public GameObject attachedBattlePosition;
    public GameObject attachedReadyText;
    public GameObject attachedNewDungeonNameGO;
    public GameObject attachedCompleteDungeonNameGO;
    public GameObject attachedWelcomeDungeonNameGO;
    public bool readyForNextBattle = false;
    public bool startedDyingRoutine = false;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (currentHP <= 0 && !startedDyingRoutine) {
            StartCoroutine(Die());
        }
        if (!taunt) {
            StartCoroutine(TauntRoutine());
        }
    }

    public void AssignAllDefenses() {
        if (!isEnemy) {
            baseDefense = CalculateBaseDefense();
            armorBonus = CalculateArmorBonus();
            armorClass = CalculateAC();
            fortitudeScore = CalculateFortitudeScore();
            reflexScore = CalculateReflexScore();
            willScore = CalculateWillScore();
            healingSurgeValue = CalculateHealingSurgeValue();
        }
    }

    public int CalculateAC() {
        return CalculateBaseDefense() + Mathf.RoundToInt(CalculateArmorBonus() + shieldBonus + extraACBonuses);
    }

    public int CalculateBaseDefense() {
        return Mathf.RoundToInt(10 + (characterLevel * 0.5f));
    }

    public int CalculateFortitudeScore() {
        int fortitudeScore = CalculateBaseDefense();
        if (GetAbilityScoreModifier(strengthScore) > GetAbilityScoreModifier(constitutionScore)) {
            fortitudeScore += GetAbilityScoreModifier(strengthScore);
        }
        else {
            fortitudeScore += GetAbilityScoreModifier(constitutionScore);
        }
        if (race == "human") {
            fortitudeScore += 1;
        }
        if (characterClass == "warrior") {
            fortitudeScore += 2;
        }
        else if (characterClass == "ranger") {
            fortitudeScore += 1;
        }
        return fortitudeScore;
    }

    public int CalculateReflexScore() {
        int reflexScore = CalculateBaseDefense();
        if (GetAbilityScoreModifier(dexterityScore) > GetAbilityScoreModifier(intelligenceScore)) {
            reflexScore += GetAbilityScoreModifier(dexterityScore);
        }
        else {
            reflexScore += GetAbilityScoreModifier(intelligenceScore);
        }
        reflexScore += shieldBonus;
        if (race == "human") {
            reflexScore += 1;
        }
        if (characterClass == "ranger") {
            reflexScore += 1;
        }
        else if (characterClass == "rogue") {
            reflexScore += 2;
        }
        return reflexScore;
    }

    public int CalculateWillScore() {
        int willScore = CalculateBaseDefense();
        if (GetAbilityScoreModifier(charismaScore) > GetAbilityScoreModifier(wisdomScore)) {
            willScore += GetAbilityScoreModifier(charismaScore);
        }
        else {
            willScore += GetAbilityScoreModifier(wisdomScore);
        }
        if (race == "human") {
            willScore += 1;
        }
        if (characterClass == "cleric" || characterClass == "wizard") {
            willScore += 2;
        }
        return willScore;
    }

    public int CalculateHealingSurgeValue() {
        return Mathf.RoundToInt(0.25f * maxHP);
    }

    public int CalculateArmorBonus() {
        int totalBonus = 0;
        if (armor == "scale") {
            totalBonus += 7;
        }
        else if (armor == "leather") {
            totalBonus += 2 + ReturnHigher(GetAbilityScoreModifier(dexterityScore), GetAbilityScoreModifier(intelligenceScore));
        }
        else if (armor == "cloth") {
            totalBonus += 0 + ReturnHigher(GetAbilityScoreModifier(dexterityScore), GetAbilityScoreModifier(intelligenceScore));
        }
        else if (armor == "chainmail") {
            totalBonus += 6;
        }
        else if (armor == "hide") {
            totalBonus += 3 + ReturnHigher(GetAbilityScoreModifier(dexterityScore), GetAbilityScoreModifier(intelligenceScore));
        }
        return totalBonus;
    }

    public IEnumerator TauntRoutine() {
        taunt = true;
        attachedBattlePosition.GetComponent<Animator>().SetBool("Flex", true);
        yield return new WaitForSeconds(Random.Range(4, 16));
        taunt = false;
    }

    public int ReturnHigher(int x, int y) {
        if (x > y) {
            return x;
        }
        else {
            return y;
        }
    }

    public void AssignValues() {
        if (characterClass == "warrior") {
            strengthScore = 16;
            if (race == "human") {
                strengthScore += 2; /// "+ 2" = human racial bonus
            }
            constitutionScore = 14;
            dexterityScore = 13;
            intelligenceScore = 10;
            wisdomScore = 12;
            charismaScore = 11;
            maxHP = 15 + constitutionScore;
            currentHP = maxHP;
            maxMP = 10 + intelligenceScore;
            currentMP = maxMP;
            hpGainedPerLevel = 6;
            mpGainedPerLevel = 4;
            equippedWeapon = "longsword";
            weaponType = "melee";
            armor = "scale";
            proficiencyBonus = 3; ///longsword proficiency bonus
            shieldBonus = 2; ///warrior heavy shield bonus to AC
        }
        else if (characterClass == "rogue") {
            strengthScore = 14;
            constitutionScore = 12;
            dexterityScore = 16;
            if (race == "human")
            {
                dexterityScore += 2; /// "+ 2" = human racial bonus
            }
            intelligenceScore = 11;
            charismaScore = 13;
            wisdomScore = 10;
            maxHP = 12 + constitutionScore;
            currentHP = maxHP;
            maxMP = 10 + intelligenceScore;
            currentMP = maxMP;
            hpGainedPerLevel = 5;
            mpGainedPerLevel = 5;
            equippedWeapon = "dagger";
            weaponType = "melee";
            armor = "leather";
            proficiencyBonus = 3; /// 3 = dagger proficiency bonus
        }
        else if (characterClass == "wizard") {
            strengthScore = 10;
            constitutionScore = 12;
            dexterityScore = 13;
            intelligenceScore = 16;
            if (race == "human") {
                intelligenceScore += 2; /// "+ 2" = human racial bonus
            }
            charismaScore = 11;
            wisdomScore = 14;
            maxHP = 10 + constitutionScore;
            currentHP = maxHP;
            maxMP = 12 + intelligenceScore;
            currentMP = maxMP;
            hpGainedPerLevel = 4;
            mpGainedPerLevel = 6;
            equippedWeapon = "quarterstaff";
            weaponType = "melee";
            armor = "cloth";
            proficiencyBonus = 2; /// 2 = quarterstaff proficiency bonus
            extraACBonuses += 1; ///wizard staff of defense class feature
        }
        else if (characterClass == "cleric") {
            strengthScore = 14;
            constitutionScore = 11;
            dexterityScore = 10;
            intelligenceScore = 12;
            charismaScore = 13;
            wisdomScore = 16;
            if (race == "human") {
                wisdomScore += 2; /// "+ 2" = human racial bonus
            }
            maxHP = 12 + constitutionScore;
            currentHP = maxHP;
            maxMP = 10 + intelligenceScore;
            currentMP = maxMP;
            hpGainedPerLevel = 5;
            mpGainedPerLevel = 5;
            equippedWeapon = "morningstar";
            weaponType = "melee";
            armor = "chainmail";
            proficiencyBonus = 2; /// 2 = morningstar proficiency bonus
        }
        else if (characterClass == "ranger") {
            strengthScore = 14;
            constitutionScore = 12;
            dexterityScore = 16;
            if (race == "human") {
                dexterityScore += 2; /// "+ 2" = human racial bonus
            }
            intelligenceScore = 11;
            charismaScore = 10;
            wisdomScore = 13;
            maxHP = 12 + constitutionScore;
            currentHP = maxHP;
            maxMP = 10 + intelligenceScore;
            currentMP = maxMP;
            hpGainedPerLevel = 5;
            mpGainedPerLevel = 5;
            equippedWeapon = "longbow";
            weaponType = "ranged";
            armor = "hide";
            proficiencyBonus = 2; /// 2 = longbow proficiency bonus
        }
        else if (ownerUsername.Contains("Orc Rogue")) { ///uses Goblin Blackblade stats
            strengthScore = 14;
            constitutionScore = 13;
            dexterityScore = 17;
            intelligenceScore = 8;
            charismaScore = 8;
            wisdomScore = 12;
            fortitudeScore = 12;
            reflexScore = 14;
            willScore = 11;
            armorClass = 16;
            maxHP = 25;
            currentHP = maxHP;
            maxMP = 10 + intelligenceScore;
            currentMP = maxMP;
            isEnemy = true;
            race = "orc";
            myXPValue = 100;
            myGoldValue = Random.Range(3, 11);
            turnWaitTime = 30;
        }
        else if (ownerUsername.Contains("Orc")) { ///uses Goblin Cutter stats
            strengthScore = 14;
            constitutionScore = 13;
            dexterityScore = 17;
            intelligenceScore = 8;
            charismaScore = 8;
            wisdomScore = 12;
            fortitudeScore = 12;
            reflexScore = 14;
            willScore = 11;
            armorClass = 16;
            maxHP = 1;
            currentHP = maxHP;
            maxMP = 10 + intelligenceScore;
            currentMP = maxMP;
            isEnemy = true;
            race = "orc";
            myXPValue = 25;
            myGoldValue = Random.Range(1, 5);
            turnWaitTime = 30;
        }
        else if (ownerUsername.Contains("Minotaur")) { ///uses Goblin Warrior stats
            strengthScore = 14;
            constitutionScore = 13;
            dexterityScore = 17;
            intelligenceScore = 8;
            charismaScore = 8;
            wisdomScore = 12;
            fortitudeScore = 13;
            reflexScore = 15;
            willScore = 12;
            armorClass = 17;
            maxHP = 29;
            currentHP = maxHP;
            maxMP = 10 + intelligenceScore;
            currentMP = maxMP;
            isEnemy = true;
            race = "minotaur";
            myXPValue = 125;
            myGoldValue = Random.Range(5, 21);
            turnWaitTime = 30;
        }
        healingPotionsInventory = new List<string>();
        manaPotionsInventory = new List<string>();
        healingPotionsInventory.Add("healing potion");
        manaPotionsInventory.Add("mana potion");
        AssignAllDefenses();
    }

    public string CharacterStats() {
        string charStats = "Owner/Name: " + ownerUsername + " | Gender: " + gender + " | Class: " + characterClass + " | XP: " + totalXP + " | Gold: " + myGold + " | Armor Class: " + armorClass + " | Strength: " + strengthScore + " | Dexterity: " + dexterityScore + " | Constitution: " + constitutionScore + " | Intelligence: " + intelligenceScore + " | Wisdom: " + wisdomScore + " | Charisma: " + charismaScore;
        return charStats;
    }

    public void DelayTrigger(int time) {
        StartCoroutine(ActionDelay(time));
    }

    public IEnumerator ActionDelay(int delayTime) {
        waitingForActionTimeLeft = delayTime;
        waitingForAction = true;
        for (int time = 0; time <= delayTime; time++) {
            yield return new WaitForSeconds(1);
            waitingForAction = true;
            waitingForActionTimeLeft = delayTime - time;
        }
        waitingForAction = false;
    }

    public void UsePotion(string type) {
        if (type == "mana"){
            if (manaPotionsInventory.Count > 0){
                manaPotionsInventory.RemoveAt(0);
                brain.floatingTextScript.SpawnFloatingCombatText("10", this.gameObject, Color.blue);
                brain.alertBoxScript.NewAlert(ownerUsername + " used a mana potion!", Color.white);
                AddMP(10);
                brain.twitchDungeonScript.ActionConfirmedAlert(ownerUsername);
            }
            else{
                brain.twitchDungeonScript.NoManaPotion(ownerUsername);
            }
        }
        else if (type == "healing"){
            if (healingPotionsInventory.Count > 0){
                healingPotionsInventory.RemoveAt(0);
                brain.floatingTextScript.SpawnFloatingCombatText("10", this.gameObject, Color.green);
                brain.alertBoxScript.NewAlert(ownerUsername + " used a health potion!", Color.white);
                AddHP(10);
                brain.twitchDungeonScript.ActionConfirmedAlert(ownerUsername);
            }
            else{
                brain.twitchDungeonScript.NoHealthPotion(ownerUsername);
            }
        }
    }

    public IEnumerator TempACChange(int amount, int time) {
        armorClass += amount;
        yield return new WaitForSeconds(time);
        armorClass -= amount;
    }

    public IEnumerator Bleed(int dmgAmount, float dmgDelay) {
        bleeding = true;
        while (bleeding == true) {
            yield return new WaitForSeconds(dmgDelay);
            if (SavingThrow()){
                bleeding = false;
                brain.floatingTextScript.SpawnFloatingCombatText("Save!", this.gameObject, Color.green);
            }
            else {
                currentHP -= dmgAmount;
                brain.floatingTextScript.SpawnFloatingCombatText("Bleed " + dmgAmount.ToString() + "!", this.gameObject, Color.red);
            }
        }
    }

    ///weapon damage database
    public float WeaponDamageQuarterstaff() {
        return Rolld8();
    }
    public float WeaponDamageDagger(){
        return Rolld4();
    }
    public float WeaponDamageLongsword(){
        return Rolld8();
    }
    public float WeaponDamageMorningstar(){
        return Rolld10();
    }
    public float WeaponDamageLongbow(){
        return Rolld10();
    }

    public float WeaponDamage(string weaponName, bool returnMax) {
        float damage = 0;
        float maxDamage = 1;
        if (weaponName == "morningstar" || weaponName == "longbow") {
            damage = WeaponDamageMorningstar();
            maxDamage = 10;
        }
        else if (weaponName == "longsword" || weaponName == "quarterstaff") {
            damage = WeaponDamageLongsword();
            maxDamage = 8;
        }
        else if (weaponName == "dagger") {
            damage = WeaponDamageDagger();
            maxDamage = 4;
        }
        if (returnMax) {
            return maxDamage;
        }
        return damage;
    }

    ///die method/function
    public IEnumerator Die() {
        startedDyingRoutine = true;
        attachedBattlePosition.GetComponent<Animator>().SetBool("Dead", true);
        brain.alertBoxScript.NewAlert(ownerUsername + " died!", Color.red);
        yield return new WaitForSeconds(5);
        if (isEnemy == false){
            brain.RemovePlayer(ownerUsername, true);
        }
        else {
            brain.RemoveEnemy(ownerUsername);
        }
    }

    ///function to calculate attack bonus for an attack
    public float AttackBonus(string abilityStatUsing, bool addProficienyBonus) {
        float bonus = (characterLevel * 0.5f) + tempAttackBonus;
        if (characterClass == "rogue" && equippedWeapon == "dagger") {
            bonus += 1;
        }
        if (addProficienyBonus) {
            bonus += proficiencyBonus;
            if (characterClass == "warrior") {
                bonus += 1;
            }
        }
        if (abilityStatUsing == "strength"){
            bonus += GetAbilityScoreModifier(strengthScore);
        }
        else if (abilityStatUsing == "dexterity"){
            bonus += GetAbilityScoreModifier(dexterityScore);
        }
        else if (abilityStatUsing == "intelligence"){
            bonus += GetAbilityScoreModifier(intelligenceScore);
        }
        else if (abilityStatUsing == "charisma"){
            bonus += GetAbilityScoreModifier(charismaScore);
        }
        else if (abilityStatUsing == "constitution"){
            bonus += GetAbilityScoreModifier(constitutionScore);
        }
        else if (abilityStatUsing == "wisdom"){
            bonus += GetAbilityScoreModifier(wisdomScore);
        }
        return bonus;
    }

    public void Attack(float power, GameObject defender, string powerName) {
        if (currentHP > 0){
            if (power != 0) {
                if (justCrit){
                    brain.floatingTextScript.SpawnFloatingCombatText("CRITICAL! " + power.ToString(), defender, Color.red);
                    brain.alertBoxScript.NewAlert(ownerUsername + " used " + powerName + " on " + defender.GetComponent<CharacterSheet>().ownerUsername + " and scored a critical for " + power.ToString() + " damage!", usernameColor);
                }
                else {
                    brain.floatingTextScript.SpawnFloatingCombatText(power.ToString(), defender, Color.red);
                    brain.alertBoxScript.NewAlert(ownerUsername + " used " + powerName + " on " + defender.GetComponent<CharacterSheet>().ownerUsername + " and hit for " + power.ToString() + " damage!", usernameColor);
                }
                defender.GetComponent<CharacterSheet>().currentHP -= Mathf.RoundToInt(power);
            }
            else{
                brain.alertBoxScript.NewAlert(ownerUsername + " used " + powerName + " on " + defender.GetComponent<CharacterSheet>().ownerUsername + " but missed!", usernameColor);
                brain.floatingTextScript.SpawnFloatingCombatText("MISSED!", defender, Color.blue);
            }
        }
        tempAttackBonus = 0;
        justCrit = false;
    }

    public void Buff(float power, GameObject buffee, string powerName){
        if (currentHP > 0){
            if (power != 0){
                brain.floatingTextScript.SpawnFloatingCombatText(power.ToString(), buffee, usernameColor);
                brain.alertBoxScript.NewAlert(ownerUsername + " used " + powerName + " on " + buffee.GetComponent<CharacterSheet>().ownerUsername + "!", usernameColor);
                buffee.GetComponent<CharacterSheet>().AddHP(Mathf.RoundToInt(power));
            }
            else{
                brain.alertBoxScript.NewAlert(ownerUsername + " used " + powerName + " on " + buffee.GetComponent<CharacterSheet>().ownerUsername + "!", usernameColor);
                brain.floatingTextScript.SpawnFloatingCombatText(powerName, buffee, usernameColor);
            }
        }
    }

    ///starting long list of attack dictionary

    ///Rogue's Sly Flourish / $backstab
    public float Backstab(int enemyAC) {
        bool nat20 = false;
        float dieResult = Rolld20();
        if (dieResult == 20) {
            nat20 = true;
        }
        float rollToHit = dieResult + AttackBonus("dexterity", true);
        if (rollToHit >= enemyAC || nat20) {
            float dmgRoll = 0;
            if (nat20 && rollToHit >= enemyAC){
                justCrit = true;
                dmgRoll = WeaponDamage(equippedWeapon, true) + GetAbilityScoreModifier(dexterityScore) + GetAbilityScoreModifier(charismaScore);
                if (characterLevel >= 21){
                    dmgRoll += WeaponDamage(equippedWeapon, true);
                }
            }
            else{
                dmgRoll = WeaponDamage(equippedWeapon, false) + GetAbilityScoreModifier(dexterityScore) + GetAbilityScoreModifier(charismaScore);
                if (characterLevel >= 21){
                    dmgRoll += WeaponDamage(equippedWeapon, false);
                }
            }
            return dmgRoll;
        }
        else {
            return 0;
        }
    }
    ///Rogue's PiercingStrike / $slice
    public float Slice(int enemyReflex){
        bool nat20 = false;
        float dieResult = Rolld20();
        if (dieResult == 20){
            nat20 = true;
        }
        float rollTohit = dieResult + AttackBonus("dexterity", true);
        if (rollTohit >= enemyReflex || nat20){
            float dmgRoll = 0;
            if (nat20 && rollTohit >= enemyReflex){
                justCrit = true;
                dmgRoll = WeaponDamage(equippedWeapon, true) + GetAbilityScoreModifier(dexterityScore);
                if (characterLevel >= 21){
                    dmgRoll += WeaponDamage(equippedWeapon, true);
                }
            }
            else{
                dmgRoll = WeaponDamage(equippedWeapon, false) + GetAbilityScoreModifier(dexterityScore);
                if (characterLevel >= 21){
                    dmgRoll += WeaponDamage(equippedWeapon, false);
                }
            }
            return dmgRoll;
        }
        else{
            return 0;
        }
    }
    ///Rogue's Torturous Strike / $twistingstab
    public float TwistingStab(int enemyAC) {
        currentMP -= 4;
        bool nat20 = false;
        float dieResult = Rolld20();
        if (dieResult == 20){
            nat20 = true;
        }
        float rollTohit = dieResult + AttackBonus("dexterity", true);
        if (rollTohit >= enemyAC || nat20) {
            float dmgRoll = 0;
            if (nat20 && rollTohit >= enemyAC){
                justCrit = true;
                dmgRoll = WeaponDamage(equippedWeapon, true) + WeaponDamage(equippedWeapon, true) + GetAbilityScoreModifier(dexterityScore) + GetAbilityScoreModifier(strengthScore);
            }
            else {
                dmgRoll = WeaponDamage(equippedWeapon, false) + WeaponDamage(equippedWeapon, false) + GetAbilityScoreModifier(dexterityScore) + GetAbilityScoreModifier(strengthScore);
            }
            return dmgRoll;
        }
        else {
            return 0;
        }
    }
    ///Rogue's Easy Target(ish) / $maim
    public float Maim(int enemyAC, GameObject enemy){
        currentMP -= 12;
        bool nat20 = false;
        float dieResult = Rolld20();
        if (dieResult == 20){
            nat20 = true;
        }
        float rollTohit = dieResult + AttackBonus("dexterity", true);
        if (rollTohit >= enemyAC || nat20){
            float dmgRoll = 0;
            if (nat20 && rollTohit >= enemyAC){
                justCrit = true;
                dmgRoll = WeaponDamage(equippedWeapon, true) + WeaponDamage(equippedWeapon, true) + GetAbilityScoreModifier(dexterityScore);
            }
            else {
                dmgRoll = WeaponDamage(equippedWeapon, false) + WeaponDamage(equippedWeapon, false) + GetAbilityScoreModifier(dexterityScore);
            }
            enemy.GetComponent<CharacterSheet>().turnWaitTime += 10;
            enemy.GetComponent<CharacterSheet>().slowed = true;
            enemy.GetComponent<CharacterSheet>().armorClass -= 2;
            return dmgRoll;
        }
        else{
            float missDmgRoll = Mathf.RoundToInt((0.5f * (WeaponDamage(equippedWeapon, false) + WeaponDamage(equippedWeapon, false) + GetAbilityScoreModifier(dexterityScore))));
            return missDmgRoll;
        }
    }
    ///Universal melee basic attack
    public float MeleeBasicAttack(int enemyAC){
        bool nat20 = false;
        float dieResult = Rolld20();
        if (dieResult == 20){
            nat20 = true;
        }
        float rollTohit = dieResult + AttackBonus("strength", true);
        if (rollTohit >= enemyAC || nat20){
            float dmgRoll = 0;
            if (nat20 && rollTohit >= enemyAC){
                justCrit = true;
                dmgRoll = WeaponDamage(equippedWeapon, true) + GetAbilityScoreModifier(strengthScore);
            }
            else {
                dmgRoll = WeaponDamage(equippedWeapon, false) + GetAbilityScoreModifier(strengthScore);
            }
            return dmgRoll;
        }
        else{
            return 0;
        }
    }
    ///Universal ranged basic attack
    public float RangedBasicAttack(int enemyAC){
        bool nat20 = false;
        float dieResult = Rolld20();
        if (dieResult == 20){
            nat20 = true;
        }
        float rollToHit = dieResult + AttackBonus("dexterity", true);
        if (rollToHit >= enemyAC || nat20){
            float dmgRoll = 0;
            if(nat20 && rollToHit >= enemyAC){
                justCrit = true;
                dmgRoll = WeaponDamage(equippedWeapon, true) + GetAbilityScoreModifier(dexterityScore);
            }
            else{
                dmgRoll = WeaponDamage(equippedWeapon, false) + GetAbilityScoreModifier(dexterityScore);
            }
            return dmgRoll;
        }
        else{
            return 0;
        }
    }
    ///Ranger's Twin Strike / $doubleshot
    public float Doubleshot(int enemyAC){
        waitingForAction = false;
        bool nat20 = false;
        float dieResult = Rolld20();
        if (dieResult == 20){
            nat20 = true;
        }
        float rollToHit = dieResult + AttackBonus("dexterity", true);
        if (rollToHit >= enemyAC || nat20){
            float dmgRoll = 0;
            if (nat20 && rollToHit >= enemyAC) {
                justCrit = true;
                dmgRoll = WeaponDamage(equippedWeapon, true);
                if (characterLevel >= 21){
                    dmgRoll += WeaponDamage(equippedWeapon, true);
                }
            }
            else{
                dmgRoll = WeaponDamage(equippedWeapon, false);
                if (characterLevel >= 21){
                    dmgRoll += WeaponDamage(equippedWeapon, false);
                }
            }
            return dmgRoll;
        }
        else{
            return 0;
        }
    }
    ///Ranger's Careful Attack / $sureshot
    public float Sureshot(int enemyAC){
        bool nat20 = false;
        float dieResult = Rolld20();
        if (dieResult == 20){
            nat20 = true;
        }
        float rollToHit = dieResult + AttackBonus("dexterity", true) + 2;
        if (rollToHit >= enemyAC || nat20){
            float dmgRoll = 0;
            if (nat20 && rollToHit >= enemyAC){
                justCrit = true;
                dmgRoll = WeaponDamage(equippedWeapon, true);
                if (characterLevel >= 21){
                    dmgRoll += WeaponDamage(equippedWeapon, true);
                }
            }
            else {
                dmgRoll = WeaponDamage(equippedWeapon, false);
                if (characterLevel >= 21){
                    dmgRoll += WeaponDamage(equippedWeapon, false);
                }
            }
            return dmgRoll;
        }
        else{
            return 0;
        }
    }
    ///Ranger's Two-Fanged Strike / $doublestrike
    public float Doublestrike(int enemyAC){
        currentMP -= 4;
        waitingForAction = false;
        bool firstNat20 = false;
        float firstDieResult = Rolld20();
        if (firstDieResult == 20){
            firstNat20 = true;
        }
        bool secondNat20 = false;
        float secondDieResult = Rolld20();
        if (secondDieResult == 20){
            secondNat20 = true;
        }
        float totalDMG = 0;
        bool firstHit = false;
        bool secondHit = false;
        float firstRollToHit = firstDieResult + AttackBonus("dexterity", true);
        float secondRollToHit = secondDieResult + AttackBonus("dexterity", true);
        if (firstRollToHit >= enemyAC || firstNat20){
            firstHit = true;
            float dmgRoll = 0;
            if (firstNat20 && firstRollToHit >= enemyAC){
                justCrit = true;
                dmgRoll = WeaponDamage(equippedWeapon, true) + GetAbilityScoreModifier(dexterityScore);
            }
            else {
                dmgRoll = WeaponDamage(equippedWeapon, false) + GetAbilityScoreModifier(dexterityScore);
            }
            totalDMG += dmgRoll;
        }
        if (secondRollToHit >= enemyAC || secondNat20){
            secondHit = true;
            float dmgRoll = 0;
            if (secondNat20 && secondRollToHit >= enemyAC){
                justCrit = true;
                dmgRoll = WeaponDamage(equippedWeapon, true) + GetAbilityScoreModifier(dexterityScore);
            }
            else {
                dmgRoll = WeaponDamage(equippedWeapon, false) + GetAbilityScoreModifier(dexterityScore);
            }
            totalDMG += dmgRoll;
        }
        if (firstHit == true && secondHit == true) {
            totalDMG += GetAbilityScoreModifier(wisdomScore);
        }
        return totalDMG;
    }

    ///Ranger's Hunter's Bear Trap / $criticalshot
    public float CriticalShot(int enemyAC, GameObject enemy){
        currentMP -= 12;
        bool nat20 = false;
        float dieResult = Rolld20();
        if (dieResult == 20){
            nat20 = true;
        }
        float rollToHit = dieResult + AttackBonus("dexterity", true);
        if (rollToHit >= enemyAC || nat20) {
            float dmgRoll = 0;
            if (nat20 && rollToHit >= enemyAC){
                justCrit = true;
                dmgRoll = WeaponDamage(equippedWeapon, true) + WeaponDamage(equippedWeapon, true) + GetAbilityScoreModifier(dexterityScore);
                enemy.GetComponent<CharacterSheet>().turnWaitTime += 11;
                enemy.GetComponent<CharacterSheet>().slowed = true;
                StartCoroutine(enemy.GetComponent<CharacterSheet>().Bleed(6, 20));
            }
            else {
                dmgRoll = WeaponDamage(equippedWeapon, false) + WeaponDamage(equippedWeapon, false) + GetAbilityScoreModifier(dexterityScore);
                enemy.GetComponent<CharacterSheet>().turnWaitTime += 10;
                enemy.GetComponent<CharacterSheet>().slowed = true;
                StartCoroutine(enemy.GetComponent<CharacterSheet>().Bleed(5, 20));
            }
            return dmgRoll;
        }
        else {
            float missDmgRoll = Mathf.RoundToInt((0.5f * (WeaponDamage(equippedWeapon, false) + WeaponDamage(equippedWeapon, false) + GetAbilityScoreModifier(dexterityScore))));
            enemy.GetComponent<CharacterSheet>().turnWaitTime += 8;
            enemy.GetComponent<CharacterSheet>().slowed = true;
            return missDmgRoll;
        }
    }

    ///Cleric's Healing Word / $heal
    public float Heal(GameObject buffee) {
        currentMP -= 8;
        int healAmount = buffee.GetComponent<CharacterSheet>().CalculateHealingSurgeValue();
        if (characterLevel >= 26){
            healAmount += Rolld6() + Rolld6() + Rolld6() + Rolld6() + Rolld6() + Rolld6();
        }
        else if (characterLevel >= 21){
            healAmount += Rolld6() + Rolld6() + Rolld6() + Rolld6() + Rolld6();
        }
        else if (characterLevel >= 16){
            healAmount += Rolld6() + Rolld6() + Rolld6() + Rolld6();
        }
        else if (characterLevel >= 11){
            healAmount += Rolld6() + Rolld6() + Rolld6();
        }
        else if (characterLevel >= 6){
            healAmount += Rolld6() + Rolld6();
        }
        else{
            healAmount += Rolld6();
        }
        return healAmount;
    }

    ///Cleric's Divine Fortune / $focus
    public float Focus(GameObject buffee){
        currentMP -= 4;
        buffee.GetComponent<CharacterSheet>().tempAttackBonus += 1;
        buffee.GetComponent<CharacterSheet>().tempSavingThrowBonus += 1;
        return 0;
    }

    ///Cleric's Lance of Faith / $weakeningblow
    public float WeakeningBlow(int enemyReflex, GameObject enemy){
        bool nat20 = false;
        float dieResult = Rolld20();
        if (dieResult == 20){
            nat20 = true;
        }
        float rollToHit = dieResult + 1 + AttackBonus("wisdom", false);
        if (rollToHit >= enemyReflex || nat20){
            float dmgRoll = 0;
            if (nat20 && rollToHit >= enemyReflex){
                justCrit = true;
                dmgRoll = 8 + GetAbilityScoreModifier(wisdomScore) + 1;
                if (characterLevel >= 21) {
                    dmgRoll += 8;
                }
            }
            else{
                dmgRoll = Rolld8() + GetAbilityScoreModifier(wisdomScore) + 1;
                if (characterLevel >= 21) {
                    dmgRoll += Rolld8();
                }
            }
            StartCoroutine(enemy.GetComponent<CharacterSheet>().TempACChange(-2, 20));
            brain.floatingTextScript.SpawnFloatingCombatText("DEFENSE DOWN!", enemy, Color.red);
            return dmgRoll;
        }
        else{
            return 0;
        }
    }

    ///Cleric's Priest's Shield / $shoutstrike
    public float ShoutStrike(int enemyAC, GameObject enemy){
        bool nat20 = false;
        float dieResult = Rolld20();
        if (dieResult == 20){
            nat20 = true;
        }
        float rollToHit = dieResult + AttackBonus("strength", true);
        if (rollToHit >= enemyAC || nat20){
            float dmgRoll = 0;
            if (nat20 && rollToHit >= enemyAC){
                justCrit = true;
                dmgRoll = WeaponDamage(equippedWeapon, true) + GetAbilityScoreModifier(strengthScore);
                if (characterLevel >= 21)
                {
                    dmgRoll += WeaponDamage(equippedWeapon, true);
                }
            }
            else{
                dmgRoll = WeaponDamage(equippedWeapon, false) + GetAbilityScoreModifier(strengthScore);
                if (characterLevel >= 21){
                    dmgRoll += WeaponDamage(equippedWeapon, false);
                }
            }
            StartCoroutine(TempACChange(1, 20));
            brain.floatingTextScript.SpawnFloatingCombatText("DEFENSE UP!", this.gameObject, Color.green);
            int randomID = Random.Range(0, brain.party.Count);
            StartCoroutine(brain.party[randomID].GetComponent<CharacterSheet>().TempACChange(1, 20));
            brain.floatingTextScript.SpawnFloatingCombatText("DEFENSE UP!", brain.party[randomID], Color.green);
            return dmgRoll;
        }
        else{
            return 0;
        }
    }

    ///Cleric's Healing Strike / $righteousfury
    public float RighteousFury(int enemyAC, GameObject enemy){
        currentMP -= 8;
        bool nat20 = false;
        float dieResult = Rolld20();
        if (dieResult == 20){
            nat20 = true;
        }
        float rollToHit = dieResult + AttackBonus("strength", true);
        if (rollToHit >= enemyAC || nat20){
            float dmgRoll = 0;
            if (nat20 && rollToHit >= enemyAC){
                justCrit = true;
                dmgRoll = WeaponDamage(equippedWeapon, true) + WeaponDamage(equippedWeapon, true) + GetAbilityScoreModifier(strengthScore);
            }
            else{
                dmgRoll = WeaponDamage(equippedWeapon, false) + WeaponDamage(equippedWeapon, false) + GetAbilityScoreModifier(strengthScore);
            }
            StartCoroutine(enemy.GetComponent<CharacterSheet>().TempACChange(-1, 20));
            brain.floatingTextScript.SpawnFloatingCombatText("DEFENSE DOWN!", enemy, Color.red);
            int randomID = Random.Range(0, brain.party.Count);
            brain.party[randomID].GetComponent<CharacterSheet>().AddHP(brain.party[randomID].GetComponent<CharacterSheet>().CalculateHealingSurgeValue());
            brain.floatingTextScript.SpawnFloatingCombatText(brain.party[randomID].GetComponent<CharacterSheet>().CalculateHealingSurgeValue().ToString(), brain.party[randomID], Color.green);
            return dmgRoll;
        }
        else{
            return 0;
        }
    }

    ///Cleric's Beacon of Hope / $prayer
    public float Prayer(int enemyWill, GameObject enemy){
        currentMP -= 12;
        bool nat20 = false;
        float dieResult = Rolld20();
        if (dieResult == 20){
            nat20 = true;
        }
        float rollToHit = dieResult + AttackBonus("wisdom", false) + 1;
        if (rollToHit >= enemyWill || nat20){
            float dmgRoll = 0;
            if (nat20 && rollToHit >= enemyWill){
                justCrit = true;
                dmgRoll = 4;
            }
            else{
                dmgRoll = 3;
            }
            foreach (GameObject enemyE in brain.currentBattleInstance.enemiesInCurrentParty) {
                enemyE.GetComponent<CharacterSheet>().armorClass -= 1;
                brain.floatingTextScript.SpawnFloatingCombatText("DEFENSE DOWN!", enemyE, Color.red);
            }
            foreach (GameObject player in brain.party) {
                player.GetComponent<CharacterSheet>().AddHP(15);
                brain.floatingTextScript.SpawnFloatingCombatText("15", player, Color.green);
            }
            return dmgRoll;
        }
        else{
            return 0;
        }
    }

    ///Warrior's Cleave / $cleave
    public float Cleave(int enemyAC, GameObject enemy){
        bool nat20 = false;
        float dieResult = Rolld20();
        if (dieResult == 20){
            nat20 = true;
        }
        float rollToHit = dieResult + AttackBonus("strength", true);
        if (rollToHit >= enemyAC || nat20){
            float dmgRoll = 0;
            if (nat20 && rollToHit >= enemyAC){
                justCrit = true;
                dmgRoll = WeaponDamage(equippedWeapon, true) + GetAbilityScoreModifier(strengthScore);
                if (characterLevel >= 21) {
                    dmgRoll += WeaponDamage(equippedWeapon, true);
                }
            }
            else{
                dmgRoll = WeaponDamage(equippedWeapon, false) + GetAbilityScoreModifier(strengthScore);
                if (characterLevel >= 21){
                    dmgRoll += WeaponDamage(equippedWeapon, false);
                }
            }
            int randomID = Random.Range(0, brain.currentBattleInstance.enemiesInCurrentParty.Count);
            brain.currentBattleInstance.enemiesInCurrentParty[randomID].GetComponent<CharacterSheet>().currentHP -= GetAbilityScoreModifier(strengthScore);
            brain.floatingTextScript.SpawnFloatingCombatText(GetAbilityScoreModifier(strengthScore).ToString(), brain.currentBattleInstance.enemiesInCurrentParty[randomID], Color.red);
            return dmgRoll;
        }
        else{
            return 0;
        }
    }

    ///Warrior's Sure Strike / $focusedstrike
    public float FocusedStrike(int enemyAC, GameObject enemy){
        bool nat20 = false;
        float dieResult = Rolld20();
        if (dieResult == 20){
            nat20 = true;
        }
        float rollToHit = dieResult + AttackBonus("strength", true) + 2;
        if (rollToHit >= enemyAC || nat20){
            float dmgRoll = 0;
            if (nat20 && rollToHit >= enemyAC){
                justCrit = true;
                dmgRoll = WeaponDamage(equippedWeapon, true);
                if (characterLevel >= 21){
                    dmgRoll += WeaponDamage(equippedWeapon, true);
                }
            }
            else{
                dmgRoll = WeaponDamage(equippedWeapon, false);
                if (characterLevel >= 21){
                    dmgRoll += WeaponDamage(equippedWeapon, false);
                }
            }
            return dmgRoll;
        }
        else{
            return 0;
        }
    }

    ///Warrior's Steel Serpent Strike/ $slowingstrike
    public float SlowingStrike(int enemyAC, GameObject enemy){
        currentMP -= 4;
        bool nat20 = false;
        float dieResult = Rolld20();
        if (dieResult == 20){
            nat20 = true;
        }
        float rollToHit = dieResult + AttackBonus("strength", true);
        if (rollToHit >= enemyAC || nat20){
            float dmgRoll = 0;
            if (nat20 && rollToHit >= enemyAC){
                justCrit = true;
                dmgRoll = WeaponDamage(equippedWeapon, true) + WeaponDamage(equippedWeapon, true) + GetAbilityScoreModifier(strengthScore);
            }
            else{
                dmgRoll = WeaponDamage(equippedWeapon, false) + WeaponDamage(equippedWeapon, false) + GetAbilityScoreModifier(strengthScore);   
            }
            enemy.GetComponent<CharacterSheet>().turnWaitTime += 10;
            enemy.GetComponent<CharacterSheet>().slowed = true;
            brain.floatingTextScript.SpawnFloatingCombatText("SLOWED!", enemy, Color.blue);
            return dmgRoll;
        }
        else{
            return 0;
        }
    }

    ///Warrior's Brute Strike / $shatteringstrike
    public float ShatteringStrike(int enemyAC, GameObject enemy){
        currentMP -= 12;
        bool nat20 = false;
        float dieResult = Rolld20();
        if (dieResult == 20){
            nat20 = true;
        }
        float rollToHit = dieResult + AttackBonus("strength", true);
        if (rollToHit >= enemyAC || nat20){
            float dmgRoll = 0;
            if (nat20 && rollToHit >= enemyAC){
                justCrit = true;
                dmgRoll = WeaponDamage(equippedWeapon, true) + WeaponDamage(equippedWeapon, true) + WeaponDamage(equippedWeapon, true) + GetAbilityScoreModifier(strengthScore);
            }
            else{
                dmgRoll = WeaponDamage(equippedWeapon, false) + WeaponDamage(equippedWeapon, false) + WeaponDamage(equippedWeapon, false) + GetAbilityScoreModifier(strengthScore);
            }
            return dmgRoll;
        }
        else{
            return 0;
        }
    }

    ///Wizard's magicmissile / $bolt
    public float Bolt(int enemyReflex, GameObject enemy){
        bool nat20 = false;
        float dieResult = Rolld20();
        if (dieResult == 20){
            nat20 = true;
        }
        float rollToHit = dieResult + AttackBonus("intelligence", false) + 1;
        if (rollToHit >= enemyReflex || nat20){
            float dmgRoll = 0;
            if (nat20 && rollToHit >= enemyReflex){
                justCrit = true;
                dmgRoll = 8 + GetAbilityScoreModifier(intelligenceScore) + 1;
                if (characterLevel >= 21) {
                    dmgRoll += 8;
                }
            }
            else{
                dmgRoll = Rolld4() + Rolld4() + GetAbilityScoreModifier(intelligenceScore) + 1;
                if (characterLevel >= 21){
                    dmgRoll += Rolld4() + Rolld4();
                }

            }
            return dmgRoll;
        }
        else{
            return 0;
        }
    }
    ///Wizard's Ray of Frost / $frost
    public float Frost(int enemyFortitude, GameObject enemy){
        bool nat20 = false;
        float dieResult = Rolld20();
        if (dieResult == 20){
            nat20 = true;
        }
        float rollToHit = dieResult + AttackBonus("intelligence", false) + 1;
        if (rollToHit >= enemyFortitude || nat20){
            float dmgRoll = 0;
            if (nat20 && rollToHit >= enemyFortitude){
                justCrit = true;
                dmgRoll = 6 + GetAbilityScoreModifier(intelligenceScore) + 1;
                if (characterLevel >= 21){
                    dmgRoll += 6;
                }
            }
            else{
                dmgRoll = Rolld6() + GetAbilityScoreModifier(intelligenceScore) + 1;
                if (characterLevel >= 21){
                    dmgRoll += Rolld6();
                }
            }
            enemy.GetComponent<CharacterSheet>().turnWaitTime += 10;
            enemy.GetComponent<CharacterSheet>().slowed = true;
            brain.floatingTextScript.SpawnFloatingCombatText("SLOWED!", enemy, Color.blue);
            return dmgRoll;
        }
        else{
            return 0;
        }
    }
    ///Wizard's Burning Hands / $fireblast
    public float FireBlast(int enemyReflex, GameObject enemy){
        currentMP -= 5;
        bool nat20 = false;
        float dieResult = Rolld20();
        if (dieResult == 20){
            nat20 = true;
        }
        float rollToHit = dieResult + AttackBonus("intelligence", false) + 1;
        if (rollToHit >= enemyReflex || nat20){
            float dmgRoll = 0;
            if (nat20 && rollToHit >= enemyReflex){
                justCrit = true;
                dmgRoll = 12 + GetAbilityScoreModifier(intelligenceScore) + 1;
            }
            else{
                dmgRoll = Rolld6() + Rolld6() + GetAbilityScoreModifier(intelligenceScore) + 1;
            }
            if (brain.currentBattleInstance.enemiesInCurrentParty.Count > 1) {
                int randomID = Random.Range(0, brain.currentBattleInstance.enemiesInCurrentParty.Count);
                while (brain.currentBattleInstance.enemiesInCurrentParty[randomID] == enemy) {
                    randomID = Random.Range(0, brain.currentBattleInstance.enemiesInCurrentParty.Count);
                }
                if (nat20) {
                    int secDMG = 12 + GetAbilityScoreModifier(intelligenceScore) + 1;
                    brain.currentBattleInstance.enemiesInCurrentParty[randomID].GetComponent<CharacterSheet>().currentHP -= secDMG;
                    brain.floatingTextScript.SpawnFloatingCombatText(secDMG.ToString(), brain.currentBattleInstance.enemiesInCurrentParty[randomID], Color.red);
                }
                else {
                    int secondDmg = Rolld6() + Rolld6() + GetAbilityScoreModifier(intelligenceScore) + 1;
                    brain.currentBattleInstance.enemiesInCurrentParty[randomID].GetComponent<CharacterSheet>().currentHP -= secondDmg;
                    brain.floatingTextScript.SpawnFloatingCombatText(secondDmg.ToString(), brain.currentBattleInstance.enemiesInCurrentParty[randomID], Color.red);
                }  
            }
            return dmgRoll;
        }
        else{
            return 0;
        }
    }
    ///Wizard's Acid Arrow / $bladestorm
    public float BladeStorm(int enemyReflex, GameObject enemy){
        currentMP -= 12;
        bool nat20 = false;
        float dieResult = Rolld20();
        if (dieResult == 20){
            nat20 = true;
        }
        float rollToHit = dieResult + AttackBonus("intelligence", false) + 1;
        if (rollToHit >= enemyReflex || nat20){
            float dmgRoll = 0;
            if (nat20 && rollToHit >= enemyReflex){
                justCrit = true;
                dmgRoll = 16 + GetAbilityScoreModifier(intelligenceScore) + 1;
            }
            else{
                dmgRoll = Rolld8() + Rolld8() + GetAbilityScoreModifier(intelligenceScore) + 1;
            }
            StartCoroutine(enemy.GetComponent<CharacterSheet>().Bleed(5, 20));
            if (brain.currentBattleInstance.enemiesInCurrentParty.Count > 1){
                int randomID = Random.Range(0, brain.currentBattleInstance.enemiesInCurrentParty.Count);
                while (brain.currentBattleInstance.enemiesInCurrentParty[randomID] == enemy){
                    randomID = Random.Range(0, brain.currentBattleInstance.enemiesInCurrentParty.Count);
                }
                if (nat20){
                    int secDmg = 8 + GetAbilityScoreModifier(intelligenceScore) + 1;
                    brain.currentBattleInstance.enemiesInCurrentParty[randomID].GetComponent<CharacterSheet>().currentHP -= secDmg;
                    brain.floatingTextScript.SpawnFloatingCombatText(secDmg.ToString(), brain.currentBattleInstance.enemiesInCurrentParty[randomID], Color.red);
                }
                else{
                    int secondDmg = Rolld8() + GetAbilityScoreModifier(intelligenceScore) + 1;
                    brain.currentBattleInstance.enemiesInCurrentParty[randomID].GetComponent<CharacterSheet>().currentHP -= secondDmg;
                    brain.floatingTextScript.SpawnFloatingCombatText(secondDmg.ToString(), brain.currentBattleInstance.enemiesInCurrentParty[randomID], Color.red);
                }
                StartCoroutine(brain.currentBattleInstance.enemiesInCurrentParty[randomID].GetComponent<CharacterSheet>().Bleed(5, 20));
            }
            return dmgRoll;
        }
        else{
            StartCoroutine(enemy.GetComponent<CharacterSheet>().Bleed(2, 20));
            return Mathf.RoundToInt(0.5f * (Rolld8() + Rolld8() + GetAbilityScoreModifier(intelligenceScore) + 1));
        }
    }
    ///Orc Rogue (Goblin Blackblade) Attack
    public float OrcRogueAttack(int enemyAC){
        bool nat20 = false;
        float dieResult = Rolld20();
        if (dieResult == 20){
            nat20 = true;
        }
        float rollTohit = dieResult + 5;
        if (rollTohit >= enemyAC || nat20){
            float dmgRoll = 0;
            if (nat20 && rollTohit >= enemyAC){
                justCrit = true;
                dmgRoll = 8;
            }
            else {
                dmgRoll = Rolld6() + 2;
            }
            return dmgRoll;
        }
        else{
            return 0;
        }
    }
    ///Orc (Goblin Cutter) Attack
    public float OrcAttack(int enemyAC){
        bool nat20 = false;
        float dieResult = Rolld20();
        if (dieResult == 20){
            nat20 = true;
        }
        float rollTohit = dieResult + 5;
        if (rollTohit >= enemyAC || nat20){
            float dmgRoll = 0;
            if (nat20 && rollTohit >= enemyAC){
                justCrit = true;
                dmgRoll = 5;
            }
            else {
                dmgRoll = 4;
            }
            return dmgRoll;
        }
        else{
            return 0;
        }
    }
    ///Minotaur (Goblin Warrior) Attack
    public float MinotaurAttack(int enemyAC){
        bool nat20 = false;
        float dieResult = Rolld20();
        if (dieResult == 20){
            nat20 = true;
        }
        float rollTohit = dieResult + 6;
        if (rollTohit >= enemyAC || nat20){
            float dmgRoll = 0;
            if (nat20 && rollTohit >= enemyAC){
                justCrit = true;
                dmgRoll = 10;
            }
            else {
                dmgRoll = Rolld8() + 2;
            }
            return dmgRoll;
        }
        else{
            return 0;
        }
    }

    public void AddXP(int xpAmount) {
        StartCoroutine(AddXPRoutine(xpAmount));
    }

    public void AddHP(int hpAmount) {
        StartCoroutine(AddHPRoutine(hpAmount));
    }
    public void AddMP(int mpAmount){
        StartCoroutine(AddMPRoutine(mpAmount));
    }
    public void AddGold(int goldAmount) {
        StartCoroutine(AddGoldRoutine(goldAmount));
    }

    public IEnumerator AddHPRoutine(int hpAmount) {
        for (int x = 0; x < hpAmount; x++) {
            if (currentHP < maxHP){
                currentHP += 1;
            }
            else {
                break;
            }
            yield return new WaitForSeconds(0.07f);
        }
    }
    public IEnumerator AddMPRoutine(int mpAmount){
        for (int x = 0; x < mpAmount; x++){
            if (currentMP < maxMP){
                currentMP += 1;
            }
            else{
                break;
            }
            yield return new WaitForSeconds(0.07f);
        }
    }

    public IEnumerator AddGoldRoutine(int goldAmount) {
        addingGold = true;
        for (int x = 0; x < goldAmount; x++) {
            myGold += 1;
            yield return new WaitForSeconds(0.07f);
        }
        addingGold = false;
    }

    public IEnumerator AddXPRoutine(int xpAmount) {
        addingXP = true;
        for (int x = 0; x < xpAmount; x++) {
            totalXP += 1;
            if (totalXP == 1000) {
                LevelUp(2);
            }
            else if (totalXP == 2250){
                LevelUp(3);
            }
            else if (totalXP == 3750){
                LevelUp(4);
            }
            else if (totalXP == 5500){
                LevelUp(5);
            }
            else if (totalXP == 7500){
                LevelUp(6);
            }
            else if (totalXP == 10000){
                LevelUp(7);
            }
            else if (totalXP == 13000){
                LevelUp(8);
            }
            else if (totalXP == 16500){
                LevelUp(9);
            }
            else if (totalXP == 20500){
                LevelUp(10);
            }
            else if (totalXP == 26000){
                LevelUp(11);
            }
            else if (totalXP == 32000){
                LevelUp(12);
            }
            else if (totalXP == 39000){
                LevelUp(13);
            }
            else if (totalXP == 47000){
                LevelUp(14);
            }
            else if (totalXP == 57000){
                LevelUp(15);
            }
            else if (totalXP == 69000){
                LevelUp(16);
            }
            else if (totalXP == 83000){
                LevelUp(17);
            }
            else if (totalXP == 99000){
                LevelUp(18);
            }
            else if (totalXP == 119000){
                LevelUp(19);
            }
            else if (totalXP == 143000){
                LevelUp(20);
            }
            else if (totalXP == 175000){
                LevelUp(21);
            }
            else if (totalXP == 210000){
                LevelUp(22);
            }
            else if (totalXP == 255000){
                LevelUp(23);
            }
            else if (totalXP == 310000){
                LevelUp(24);
            }
            else if (totalXP == 375000){
                LevelUp(25);
            }
            else if (totalXP == 450000){
                LevelUp(26);
            }
            else if (totalXP == 550000){
                LevelUp(27);
            }
            else if (totalXP == 675000){
                LevelUp(28);
            }
            else if (totalXP == 825000){
                LevelUp(29);
            }
            else if (totalXP == 1000000){
                LevelUp(30);
            }
            yield return new WaitForSeconds(0.07f);
        }
        addingXP = false;
    }

    public void LevelUp(int level) {
        attachedPanel.GetComponent<PlayerPanelScript>().attachedLevelUpBox.SetActive(true);
        StartCoroutine(attachedPanel.GetComponent<PlayerPanelScript>().attachedLevelUpBox.GetComponent<LevelUpBoxScript>().SelfDeactivate(3f));
        maxHP += hpGainedPerLevel;
        maxMP += mpGainedPerLevel;
        characterLevel += 1;
        brain.alertBoxScript.NewAlert(ownerUsername + " reached level " + level.ToString() + "!", Color.green);
        if (level == 2){
            ///gain one utility power; gain 1 feat
        }
        else if (level == 3) {
            ///gain one encounter attack power
        }
        else if (level == 4){
            /// gain 1 feat
            if (characterClass == "warrior"){
                strengthScore += 1;
                constitutionScore += 1;
            }
            else if (characterClass == "ranger") {
                dexterityScore += 1;
                strengthScore += 1;
            }
            else if (characterClass == "wizard"){
                intelligenceScore += 1;
                wisdomScore += 1;
            }
            else if (characterClass == "cleric"){
                wisdomScore += 1;
                strengthScore += 1;
            }
            else if (characterClass == "rogue"){
                dexterityScore += 1;
                strengthScore += 1;
            }
        }
        else if (level == 5){
            ///gain 1 daily attack power
        }
        else if (level == 6){
            ///gain 1 utility power, gain 1 feat
        }
        else if (level == 7){
            ///gain one encounter attack power
        }
        else if (level == 8){
            ///gain 1 feat
            if (characterClass == "warrior"){
                strengthScore += 1;
                constitutionScore += 1;
            }
            else if (characterClass == "ranger"){
                dexterityScore += 1;
                strengthScore += 1;
            }
            else if (characterClass == "wizard"){
                intelligenceScore += 1;
                wisdomScore += 1;
            }
            else if (characterClass == "cleric"){
                wisdomScore += 1;
                strengthScore += 1;
            }
            else if (characterClass == "rogue"){
                dexterityScore += 1;
                strengthScore += 1;
            }
        }
        else if (level == 9){
            ///gain 1 daily attack power
        }
        else if (level == 10){
            ///gain 1 utility power, gain 1 feat
        }
        else if (level == 11){
            ///paragon path features, gain 1 paragon path encounter attack power, gain 1 feat
            strengthScore += 1;
            wisdomScore += 1;
            dexterityScore += 1;
            constitutionScore += 1;
            charismaScore += 1;
            intelligenceScore += 1;
        }
        else if (level == 12){
            ///gain 1 paragon path utility power, gain 1 feat
        }
        else if (level == 13){
            ///replace 1 encounter attack power
        }
        else if (level == 14){
            ///gain 1 feat
            if (characterClass == "warrior"){
                strengthScore += 1;
                constitutionScore += 1;
            }
            else if (characterClass == "ranger"){
                dexterityScore += 1;
                strengthScore += 1;
            }
            else if (characterClass == "wizard"){
                intelligenceScore += 1;
                wisdomScore += 1;
            }
            else if (characterClass == "cleric"){
                wisdomScore += 1;
                strengthScore += 1;
            }
            else if (characterClass == "rogue"){
                dexterityScore += 1;
                strengthScore += 1;
            }
        }
        else if (level == 15){
            ///replace 1 daily attack power
        }
        else if (level == 16){
            ///paragon path feature, gain 1 utility power, gain 1 feat
        }
        else if (level == 17){
            ///replace 1 encounter attack power
        }
        else if (level == 18){
            ///gain 1 feat
            if (characterClass == "warrior"){
                strengthScore += 1;
                constitutionScore += 1;
            }
            else if (characterClass == "ranger"){
                dexterityScore += 1;
                strengthScore += 1;
            }
            else if (characterClass == "wizard"){
                intelligenceScore += 1;
                wisdomScore += 1;
            }
            else if (characterClass == "cleric"){
                wisdomScore += 1;
                strengthScore += 1;
            }
            else if (characterClass == "rogue"){
                dexterityScore += 1;
                strengthScore += 1;
            }
        }
        else if (level == 19){
            ///replace 1 daily attack power
        }
        else if (level == 20){
            ///gain one paragon path daily attack power, gain 1 feat
        }
        else if (level == 21){
            ///epic destiny feature, gain 1 feat
            strengthScore += 1;
            wisdomScore += 1;
            dexterityScore += 1;
            constitutionScore += 1;
            charismaScore += 1;
            intelligenceScore += 1;
        }
        else if (level == 22){
            ///gain 1 utility power, gain 1 feat
        }
        else if (level == 23){
            ///replace one encounter attack power
        }
        else if (level == 24){
            ///epic destiny feature, gain 1 feat
            if (characterClass == "warrior"){
                strengthScore += 1;
                constitutionScore += 1;
            }
            else if (characterClass == "ranger"){
                dexterityScore += 1;
                strengthScore += 1;
            }
            else if (characterClass == "wizard"){
                intelligenceScore += 1;
                wisdomScore += 1;
            }
            else if (characterClass == "cleric"){
                wisdomScore += 1;
                strengthScore += 1;
            }
            else if (characterClass == "rogue"){
                dexterityScore += 1;
                strengthScore += 1;
            }
        }
        else if (level == 25){
            ///replace 1 daily attack power
        }
        else if (level == 26){
            ///gain one epic destiny utility power, gain 1 feat
        }
        else if (level == 27){
            ///replace one encounter attack power
        }
        else if (level == 28){
            ///gain 1 feat
            if (characterClass == "warrior"){
                strengthScore += 1;
                constitutionScore += 1;
            }
            else if (characterClass == "ranger"){
                dexterityScore += 1;
                strengthScore += 1;
            }
            else if (characterClass == "wizard"){
                intelligenceScore += 1;
                wisdomScore += 1;
            }
            else if (characterClass == "cleric"){
                wisdomScore += 1;
                strengthScore += 1;
            }
            else if (characterClass == "rogue"){
                dexterityScore += 1;
                strengthScore += 1;
            }
        }
        else if (level == 29){
            ///replace one daily attack power
        }
        else if (level == 30){
            ///epic destiny feature, gain 1 feat
        }
        ///assign at end of levelup method below this line
        AssignAllDefenses();
    }

    public int Rolld20() {
        return Random.Range(1, 21);
    }
    public int Rolld10() {
        return Random.Range(1, 11);
    }
    public int Rolld4() {
        return Random.Range(1, 5);
    }
    public int Rolld8() {
        return Random.Range(1, 9);
    }
    public int Rolld6(){
        return Random.Range(1, 7);
    }

    public bool SavingThrow() {
        int result = tempSavingThrowBonus;
        result += Rolld20();
        tempSavingThrowBonus = 0;
        if (result >= 10){
            return true;
        }
        else {
            return false;
        }
    }

    public int GetAbilityScoreModifier(int abilityScore){
        if (abilityScore < 2){
            return -5;
        }
        else if (abilityScore < 4){
            return -4;
        }
        else if (abilityScore < 6){
            return -3;
        }
        else if (abilityScore < 8){
            return -2;
        }
        else if (abilityScore < 10){
            return -1;
        }
        else if (abilityScore < 12){
            return 0;
        }
        else if (abilityScore < 14){
            return 1;
        }
        else if (abilityScore < 16){
            return 2;
        }
        else if (abilityScore < 18){
            return 3;
        }
        else if (abilityScore < 20){
            return 4;
        }
        else if (abilityScore < 22){
            return 5;
        }
        else if (abilityScore < 24){
            return 6;
        }
        else if (abilityScore < 26){
            return 7;
        }
        else if (abilityScore < 28){
            return 8;
        }
        else if (abilityScore < 30){
            return 9;
        }
        else if (abilityScore < 32){
            return 10;
        }
        else if (abilityScore < 34){
            return 11;
        }
        else{
            return 12;
        }
    }

    public SheetSave CreateNewSheetSave()
    {
        SheetSave newSave = new SheetSave();
        newSave.strengthScore = strengthScore;
        newSave.constitutionScore = constitutionScore;
        newSave.dexterityScore = dexterityScore;
        newSave.intelligenceScore = intelligenceScore;
        newSave.wisdomScore = wisdomScore;
        newSave.charismaScore = charismaScore;
        newSave.armorClass = armorClass;
        newSave.fortitudeScore = fortitudeScore;
        newSave.reflexScore = reflexScore;
        newSave.willScore = willScore;
        newSave.baseDefense = baseDefense;
        newSave.healingSurgeValue = healingSurgeValue;
        newSave.bleeding = bleeding;
        newSave.slowed = slowed;
        newSave.justCrit = justCrit;
        newSave.isEnemy = isEnemy;
        newSave.proficiencyBonus = proficiencyBonus;
        newSave.wDmg = wDmg;
        newSave.equippedWeapon = equippedWeapon;
        newSave.weaponType = weaponType;
        newSave.turnWaitTime = turnWaitTime;
        newSave.healingPotionsInventory = healingPotionsInventory;
        newSave.manaPotionsInventory = manaPotionsInventory;
        newSave.currentHP = currentHP;
        newSave.maxHP = maxHP;
        newSave.currentMP = currentMP;
        newSave.maxMP = maxMP;
        newSave.hpGainedPerLevel = hpGainedPerLevel;
        newSave.mpGainedPerLevel = mpGainedPerLevel;
        newSave.totalXP = totalXP;
        newSave.myXPValue = myXPValue;
        newSave.myGoldValue = myGoldValue;
        newSave.myGold = myGold;
        newSave.characterLevel = characterLevel;
        newSave.armor = armor;
        newSave.armorBonus = armorBonus;
        newSave.shieldBonus = shieldBonus;
        newSave.extraACBonuses = extraACBonuses;
        newSave.ownerUsername = ownerUsername;
        newSave.characterClass = characterClass;
        newSave.classIsAssigned = classIsAssigned;
        newSave.gender = gender;
        newSave.race = race;
        newSave.waitingForAction = waitingForAction;
        newSave.waitingForActionTimeLeft = waitingForActionTimeLeft;
        newSave.taunt = taunt;
        newSave.addingXP = addingXP;
        newSave.addingGold = addingGold;
        newSave.tempAttackBonus = tempAttackBonus;
        newSave.tempSavingThrowBonus = tempSavingThrowBonus;
        newSave.readyForNextBattle = readyForNextBattle;
        newSave.startedDyingRoutine = startedDyingRoutine;
        return newSave;
    }

    public void LoadSheet(SheetSave saveSheet) {
        strengthScore = saveSheet.strengthScore;
        constitutionScore = saveSheet.constitutionScore;
        dexterityScore = saveSheet.dexterityScore;
        intelligenceScore = saveSheet.intelligenceScore;
        wisdomScore = saveSheet.wisdomScore;
        charismaScore = saveSheet.charismaScore;
        armorClass = saveSheet.armorClass;
        fortitudeScore = saveSheet.fortitudeScore;
        reflexScore = saveSheet.reflexScore;
        willScore = saveSheet.willScore;
        baseDefense = saveSheet.baseDefense;
        healingSurgeValue = saveSheet.healingSurgeValue;
        bleeding = saveSheet.bleeding;
        slowed = saveSheet.slowed;
        justCrit = saveSheet.justCrit;
        isEnemy = saveSheet.isEnemy;
        proficiencyBonus = saveSheet.proficiencyBonus;
        wDmg = saveSheet.wDmg;
        equippedWeapon = saveSheet.equippedWeapon;
        weaponType = saveSheet.weaponType;
        turnWaitTime = saveSheet.turnWaitTime;
        healingPotionsInventory = saveSheet.healingPotionsInventory;
        manaPotionsInventory = saveSheet.manaPotionsInventory;
        currentHP = saveSheet.currentHP;
        maxHP = saveSheet.maxHP;
        currentMP = saveSheet.currentMP;
        maxMP = saveSheet.maxMP;
        hpGainedPerLevel = saveSheet.hpGainedPerLevel;
        mpGainedPerLevel = saveSheet.mpGainedPerLevel;
        totalXP = saveSheet.totalXP;
        myXPValue = saveSheet.myXPValue;
        myGoldValue = saveSheet.myGoldValue;
        myGold = saveSheet.myGold;
        characterLevel = saveSheet.characterLevel;
        armor = saveSheet.armor;
        armorBonus = saveSheet.armorBonus;
        shieldBonus = saveSheet.shieldBonus;
        extraACBonuses = saveSheet.extraACBonuses;
        ownerUsername = saveSheet.ownerUsername;
        characterClass = saveSheet.characterClass;
        classIsAssigned = saveSheet.classIsAssigned;
        gender = saveSheet.gender;
        race = saveSheet.race;
        waitingForAction = false;
        waitingForActionTimeLeft = 0;
        taunt = saveSheet.taunt;
        addingXP = saveSheet.addingXP;
        addingGold = saveSheet.addingGold;
        tempAttackBonus = saveSheet.tempAttackBonus;
        tempSavingThrowBonus = saveSheet.tempSavingThrowBonus;
        readyForNextBattle = saveSheet.readyForNextBattle;
        startedDyingRoutine = saveSheet.startedDyingRoutine;
    }
}
    

