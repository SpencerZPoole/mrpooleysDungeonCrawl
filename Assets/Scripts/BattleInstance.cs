using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BattleInstance : MonoBehaviour {

    public List<string> enemies;
    public List<GameObject> enemiesInCurrentParty;
    public int numberOfEnemiesAtStart;
    public Brain1 brain;
    public TwitchChatterDungeon dungeonChatter;
    public DungeonRunInstance dungeonParent;
    public int myTotalXPReward = 0;
    public int myTotalGoldValue = 0;


	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        if (enemiesInCurrentParty.Count <= 0){
            ManualDestroy();
        }
        else{
            foreach (GameObject enemy in enemiesInCurrentParty){
                if (!enemy.GetComponent<CharacterSheet>().waitingForAction && enemy.GetComponent<CharacterSheet>().currentHP > 0){
                    if (enemy.name.Contains("Orc Rogue")){
                        string defenderName = brain.party[Random.Range(0, brain.party.Count)].name;
                        StartCoroutine(ExecuteAttack(enemy.name, defenderName, enemy.GetComponent<CharacterSheet>().OrcRogueAttack(GetMemberInBattle(defenderName).GetComponent<CharacterSheet>().armorClass), "Cut", true));
                    }
                    else if (enemy.name.Contains("Orc")){
                        string defenderName = brain.party[Random.Range(0, brain.party.Count)].name;
                        StartCoroutine(ExecuteAttack(enemy.name, defenderName, enemy.GetComponent<CharacterSheet>().OrcAttack(GetMemberInBattle(defenderName).GetComponent<CharacterSheet>().armorClass), "Stab", true));
                    }
                    else if (enemy.name.Contains("Minotaur")){
                        string defenderName = brain.party[Random.Range(0, brain.party.Count)].name;
                        StartCoroutine(ExecuteAttack(enemy.name, defenderName, enemy.GetComponent<CharacterSheet>().MinotaurAttack(GetMemberInBattle(defenderName).GetComponent<CharacterSheet>().armorClass), "Pummel", true));
                    }
                }
            }
        }
	}

    public void ManualDestroy() {
        if (enemiesInCurrentParty.Count > 0){
            brain.usersParticipatedInThisBattle.Clear();
            foreach (GameObject enemy in enemiesInCurrentParty){
                if (enemy != null)
                {
                    brain.RemoveEnemy(enemy.name);
                }
            }
            dungeonParent.xpJustGained = 0;
            dungeonParent.goldJustGained = 0;
        }
        else{
            dungeonParent.roomsPassed += 1;
            if (dungeonParent.roomCount == dungeonParent.currentRoomNumber){
                dungeonParent.xpJustGained = 200 + myTotalXPReward;
                dungeonParent.goldJustGained = 200 + myTotalGoldValue;
                foreach (GameObject player in brain.party) {
                    player.GetComponent<CharacterSheet>().AddXP(200 + myTotalXPReward);
                    player.GetComponent<CharacterSheet>().AddGold(200 + myTotalGoldValue);
                }
            }
            else{
                dungeonParent.xpJustGained = myTotalXPReward;
                dungeonParent.goldJustGained = myTotalGoldValue;
                foreach (GameObject player in brain.party){
                    player.GetComponent<CharacterSheet>().AddXP(myTotalXPReward);
                    player.GetComponent<CharacterSheet>().AddGold(myTotalGoldValue);
                }
            }
        }
        foreach (GameObject player in brain.party){
            player.GetComponent<CharacterSheet>().attachedBattlePosition.GetComponent<BattlePositionScript>().hpChildText.enabled = false;
        }
        brain.inBattle = false;
        dungeonParent.currentlyInBattle = false;
        Destroy(this);
    }

    ///BattleInstance constructor
    public void InitializeBattleInstance(List<string> enemiesStrings, int numberofEnemies, Brain1 brainMaster) {
        brainMaster.readyUpPanel.SetActive(false);
        brainMaster.newDungeonPanel.SetActive(false);
        dungeonParent.currentlyInBattle = true;
        foreach (GameObject player in brainMaster.party){
            player.GetComponent<CharacterSheet>().attachedBattlePosition.GetComponent<BattlePositionScript>().hpChildText.enabled = true;
            player.GetComponent<CharacterSheet>().readyForNextBattle = false;
        }
        enemiesInCurrentParty = new List<GameObject>();
        enemies = enemiesStrings;
        numberOfEnemiesAtStart = numberofEnemies;
        brain = brainMaster;
        brain.inBattle = true;
        ///make list to keep track of users that have joined this battle already
        brain.usersParticipatedInThisBattle.Clear();
        foreach (GameObject user in brain.party) {
            brain.usersParticipatedInThisBattle.Add(user.name);
        }
        ///add enemies
        while (enemiesInCurrentParty.Count < numberOfEnemiesAtStart){
            float currentRoomNumber = dungeonParent.currentRoomNumber;
            float roomCount = dungeonParent.roomCount;
            if (currentRoomNumber / roomCount <= 0.33f || currentRoomNumber == 1){
                if (enemiesInCurrentParty.Count != 0){
                    bool justAdded = false;
                    foreach (GameObject enemy in enemiesInCurrentParty){
                        if (enemy.name.Contains("Orc Rogue")){
                            AddEnemy("Orc");
                            justAdded = true;
                            break;
                        }
                    }
                    if (!justAdded) {
                        AddEnemy(enemies[Random.Range(0, 2)]);
                        continue;
                    }
                }
                else{
                    AddEnemy(enemies[Random.Range(0, 2)]);
                    continue;
                }
            }
            else if (currentRoomNumber / roomCount > 0.33f && currentRoomNumber / roomCount <= 0.66f){
                        AddEnemy(enemies[Random.Range(0, 2)]);
                        continue;
            }
            else if (currentRoomNumber / roomCount > 0.66f && currentRoomNumber / roomCount < 1.0f){
                if (enemiesInCurrentParty.Count != 0){
                    bool justAdded = false;
                    foreach (GameObject enemy in enemiesInCurrentParty){
                        if (enemy.name.Contains("Minotaur")){
                            AddEnemy(enemies[Random.Range(0, 2)]);
                            justAdded = true;
                            break;
                        }
                    }
                    if (!justAdded) {
                        AddEnemy(enemies[Random.Range(0, 3)]);
                        continue;
                    }
                }
                else{
                    AddEnemy(enemies[Random.Range(0, 3)]);
                    continue;
                }
            }
            else{
                AddEnemy("Minotaur");
            }
        }
        foreach (GameObject enemy in enemiesInCurrentParty) {
            myTotalXPReward += enemy.GetComponent<CharacterSheet>().myXPValue;
            myTotalGoldValue += enemy.GetComponent<CharacterSheet>().myGoldValue;
        }
        brain.currentBattleInstance = this;
        brain.inBattle = true;
    }

    public IEnumerator ExecuteAttack(string attackerUsername, string defenderUsername, float power, string powerName, bool melee) {
        GameObject attacker = GetMemberInBattle(attackerUsername);
        GameObject defender = GetMemberInBattle(defenderUsername);
        if (!attacker.GetComponent<CharacterSheet>().waitingForAction) {
            attacker.GetComponent<CharacterSheet>().DelayTrigger(attacker.GetComponent<CharacterSheet>().turnWaitTime);
            if (!attacker.GetComponent<CharacterSheet>().isEnemy && powerName != "Doubleshot"){
                dungeonChatter.ActionConfirmedAlert(attackerUsername);
            }
            if (melee && attacker.GetComponent<CharacterSheet>().currentHP > 0){
                StartCoroutine(MeleeAttackLerp(GetMemberInBattle(attackerUsername), GetMemberInBattle(defenderUsername)));
            }
            else if(attacker.GetComponent<CharacterSheet>().currentHP > 0) {
                attacker.GetComponent<CharacterSheet>().attachedBattlePosition.GetComponent<Animator>().SetBool("Attack", true);
            }
            yield return new WaitForSeconds(1f);
            if (attacker.GetComponent<CharacterSheet>().currentHP > 0){
                attacker.GetComponent<CharacterSheet>().Attack(power, defender, powerName);
            }
        }
        else {
            if (!attacker.GetComponent<CharacterSheet>().isEnemy){
                dungeonChatter.CooldownAlert(attackerUsername);
            }
        }
    }

    public IEnumerator ExecuteBuff(string bufferUsername, string buffeeUsername, float power, string powerName){
        GameObject buffer = GetMemberInBattle(bufferUsername);
        GameObject buffee = GetMemberInBattle(buffeeUsername);
            if (!buffer.GetComponent<CharacterSheet>().isEnemy){
                dungeonChatter.ActionConfirmedAlert(bufferUsername);
            }
            if (buffer.GetComponent<CharacterSheet>().currentHP > 0){
                buffer.GetComponent<CharacterSheet>().attachedBattlePosition.GetComponent<Animator>().SetBool("Flex", true);
            }
            yield return new WaitForSeconds(1f);
            if (buffer.GetComponent<CharacterSheet>().currentHP > 0){
                buffer.GetComponent<CharacterSheet>().Buff(power, buffee, powerName);
            }
    }

    public GameObject GetMemberInBattle(string memberName) {
        foreach (GameObject player in brain.party) {
            if (player.name == memberName) {
                return player;
            }
        }
        foreach (GameObject enemy in enemiesInCurrentParty) {
            if (enemy.name == memberName) {
                return enemy;
            }
        }
        return null;
    }

    public List<GameObject> AllBattleMembers() {
        List<GameObject> allMembers = new List<GameObject>();
        foreach (GameObject player in brain.party) {
            allMembers.Add(player);
        }
        foreach (GameObject enemy in enemiesInCurrentParty) {
            allMembers.Add(enemy);
        }
        return allMembers;
    }

    public IEnumerator MeleeAttackLerp(GameObject attacker, GameObject defender) {
        Vector3 attackerStartPos = attacker.GetComponent<CharacterSheet>().attachedBattlePosition.transform.position;
        Vector3 defenderPos = defender.GetComponent<CharacterSheet>().attachedBattlePosition.transform.position;
        Vector3 destinationPos = new Vector3(0, 0, 0);
        if (!attacker.GetComponent<CharacterSheet>().isEnemy){
            destinationPos = new Vector3(defenderPos.x - 100, defenderPos.y, defenderPos.z);
        }
        else {
            destinationPos = new Vector3(defenderPos.x + 100, defenderPos.y, defenderPos.z);
        }
        attacker.GetComponent<CharacterSheet>().attachedBattlePosition.transform.position = destinationPos;
        attacker.GetComponent<CharacterSheet>().attachedBattlePosition.GetComponent<Animator>().SetBool("Attack", true);
        yield return new WaitForSeconds(1.5f);
        attacker.GetComponent<CharacterSheet>().attachedBattlePosition.transform.position = attackerStartPos;
    }

    
    ///function for adding an enemy to the battleinstance/current scene/current room/current enemy party
    public void AddEnemy(string enemyName) {
        
        ///start out with a loop and idNo numerator so we can appropriately name duplicates of enemies. i.e. "Orc" and "Orc 2" instead of "Orc" and "Orc"
        int idNo = 1;
        string finalName;
        foreach (GameObject enemyInParty in enemiesInCurrentParty){
            if (enemyInParty.name.Contains(enemyName)){
                if (enemyInParty.name.Contains("Orc Rogue") && enemyName == "Orc"){

                }
                else {
                    idNo += 1;
                }
            }
        }
        if (idNo != 1){
            finalName = enemyName + " " + idNo;
        }
        else {
            finalName = enemyName;
        }

        ///instantiate new enemy/enemyGameObject
        GameObject enemy = new GameObject(finalName);
        enemiesInCurrentParty.Add(enemy);
        CharacterSheet enemyCharacterSheet = enemy.AddComponent<CharacterSheet>();
        enemyCharacterSheet.ownerUsername = finalName;
        enemyCharacterSheet.brain = brain;
        enemyCharacterSheet.isEnemy = true;
        enemyCharacterSheet.usernameColor = Color.red;
        enemyCharacterSheet.DelayTrigger(Random.Range(20,31));

        ///attach enemy to a new GUI panel
        foreach (GameObject panel in brain.enemyPanels){
            if (panel.GetComponent<EnemyPanelScript>().currentEnemyGO == null){
                panel.GetComponent<EnemyPanelScript>().currentEnemyGO = enemy;
                Debug.Log("set panel enamy");
                enemyCharacterSheet.attachedPanel = panel;
                panel.GetComponent<EnemyPanelScript>().UpdateOwnerData();
                break;
            }
        }

        ///attach enemy to a new BattlePosition
        foreach (GameObject battlePos in brain.enemyBattlePositions){
            if (battlePos.GetComponent<BattlePositionScript>().ownerGO == null){
                battlePos.GetComponent<BattlePositionScript>().ownerGO = enemy;
                enemyCharacterSheet.attachedBattlePosition = battlePos;
                battlePos.GetComponent<Animator>().SetBool("Revive", true);
                battlePos.GetComponent<Animator>().SetBool("Dead", false);
                break;
            }
        }

        ///update/assign the correct enemy stats on Character Sheet
        enemyCharacterSheet.AssignValues();

        ///set the battleposition for the enemy to enabled and all battleposition variables good to go
        enemyCharacterSheet.attachedBattlePosition.GetComponent<BattlePositionScript>().ChooseRightAnimatorControllerEnemy(enemyName);
        enemyCharacterSheet.attachedBattlePosition.GetComponent<BattlePositionScript>().textChild.SetActive(true);
        enemyCharacterSheet.attachedBattlePosition.GetComponent<BattlePositionScript>().hpChild.SetActive(true);
        enemyCharacterSheet.attachedBattlePosition.GetComponent<BattlePositionScript>().ownerCharacterSheet = enemyCharacterSheet;
        GameObject hpChild = enemyCharacterSheet.attachedBattlePosition.GetComponent<BattlePositionScript>().hpChild;
        if (enemyCharacterSheet.race == "orc"){
            if (hpChild.transform.localScale.x > 0){
                hpChild.transform.localScale = new Vector3(hpChild.transform.localScale.x * -1, hpChild.transform.localScale.y, hpChild.transform.localScale.z);
            }
        }
        else {
            if (hpChild.transform.localScale.x < 0){
                hpChild.transform.localScale = new Vector3(hpChild.transform.localScale.x * -1, hpChild.transform.localScale.y, hpChild.transform.localScale.z);
            }
        }
        enemyCharacterSheet.attachedBattlePosition.GetComponent<BattlePositionScript>().textChild.GetComponent<Text>().text = finalName;
        enemyCharacterSheet.attachedBattlePosition.GetComponent<BattlePositionScript>().spriteRenderer.enabled = true;
    }
}
