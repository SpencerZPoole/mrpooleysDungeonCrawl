using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TwitchChatter;
[System.Serializable]
public class TwitchChatterDungeon : MonoBehaviour {
    public Brain1 brainScript;

    ///viewer chat commands so far = $joinparty, $confirm, $listparty, $leaveparty, $assign gender class (e.g. '$assign female wizard'), $mystats, $listwaiting, $basicattack 'enemyName'
    ///joke commands = $killmrpooley92, $attackmrpooley92
    
    // Use this for initialization
    void Start () {    
        // Add a chat listener.
        TwitchChatClient.singleton.AddChatListener(OnChatMessage);
        // Set your credentials. If you're not planning on sending messages,
        // you can remove these lines.
        TwitchChatClient.singleton.userName = "PooleyDungeonBot";
        TwitchChatClient.singleton.oAuthPassword = "oauth:~~~~~~~~~~~~";
        // Join some channels.
        TwitchChatClient.singleton.JoinChannel("mrpooley92");
        // If you set your credentials and you'd like to receive whispers,
        //  call EnableWhispers to allow for sending/receiving whispers.
        TwitchChatClient.singleton.EnableWhispers();
        // Then, add any whisper listeners you'd like.
        TwitchChatClient.singleton.AddWhisperListener(OnWhisper);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    // You'd define your chat message callback like this:
    public void OnChatMessage(ref TwitchChatMessage msg) {
        ///test to make sure this is a PooleyDungeon command (starts with '$')
        if (msg.chatMessagePlainText[0].ToString() == "$") {
            ///-------------------------------------ALL COMMANDS IF MSG OWNER IS ACTUALLY IN THE PARTY ONLY
            if (brainScript.InParty(msg.userName)) {
                ///implementation of $mygold command
                if (msg.chatMessagePlainText.Equals("$mygold"))
                {
                    TwitchChatClient.singleton.SendMessage("mrpooley92", msg.userName + " you have " + brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().myGold.ToString() + " gold.");
                }
                ///implementation of $leaveparty command
                else if (msg.chatMessagePlainText.Equals("$leaveparty") && !brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().addingXP && !brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().addingGold)
                {
                    brainScript.RemovePlayer(msg.userName, true);
                    TwitchChatClient.singleton.SendMessage("mrpooley92", msg.userName + " left the party.");
                    brainScript.alertBoxScript.NewAlert(msg.userName + " left the party.", Color.white);
                }
                
                //////implementation of $mystats command
                else if (msg.chatMessagePlainText.Equals("$mystats"))
                {
                    TwitchChatClient.singleton.SendMessage("mrpooley92", msg.userName + " your stats are: " + brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().CharacterStats());
                }
                ///implementation of $assign gender class command
                else if (msg.chatMessagePlainText.Contains("$assign"))
                {
                    CharacterSheet playerSheet = brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>();
                    if (playerSheet.classIsAssigned == true)
                    {
                        TwitchChatClient.singleton.SendMessage("mrpooley92", msg.userName + " you have already assigned your class and gender!");
                    }
                    else
                    {
                        string[] msgSplit = msg.chatMessagePlainText.Split(' ');
                        if (msgSplit.Length != 3 || (msgSplit[2] != "warrior" && msgSplit[2] != "rogue" && msgSplit[2] != "wizard" && msgSplit[2] != "cleric" && msgSplit[2] != "ranger") || (msgSplit[1] != "male" && msgSplit[1] != "female"))
                        {
                            TwitchChatClient.singleton.SendMessage("mrpooley92", msg.userName + " I don't think you typed that right.");
                        }
                        else
                        {
                            playerSheet.characterClass = msgSplit[2];
                            playerSheet.gender = msgSplit[1];
                            playerSheet.classIsAssigned = true;
                            playerSheet.AssignValues();
                            playerSheet.attachedBattlePosition.GetComponent<BattlePositionScript>().ChooseRightAnimatorController(playerSheet.gender, playerSheet.characterClass);
                            playerSheet.attachedBattlePosition.GetComponent<BattlePositionScript>().textChild.SetActive(true);
                            playerSheet.attachedBattlePosition.GetComponent<BattlePositionScript>().hpChild.SetActive(true);
                            playerSheet.attachedBattlePosition.GetComponent<BattlePositionScript>().ownerCharacterSheet = playerSheet;
                            playerSheet.attachedBattlePosition.GetComponent<BattlePositionScript>().textChild.GetComponent<Text>().text = msg.userName;
                            playerSheet.attachedBattlePosition.GetComponent<BattlePositionScript>().spriteRenderer.enabled = true;
                            TwitchChatClient.singleton.SendMessage("mrpooley92", msg.userName + " assignment successful. You are a " + playerSheet.gender + " " + playerSheet.characterClass);
                        }
                    }
                }
                ///implementation of $ready command
                else if (msg.chatMessagePlainText.Equals("$ready") && brainScript.inBattle == false && brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().classIsAssigned){
                    brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().readyForNextBattle = true;
                }
                ///implementation of $use command
                else if (msg.chatMessagePlainText.Contains("$use") && brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().classIsAssigned){
                    string[] msgSplit = msg.chatMessagePlainText.Split(' ');
                    if (msgSplit[0] == "$use"){
                        if (msgSplit[1] == "manapotion"){
                            brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().UsePotion("mana");
                        }
                        else if (msgSplit[1] == "healingpotion"){
                            brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().UsePotion("healing");
                        }
                    }
                }
                ///implementation of $buy command
                else if (msg.chatMessagePlainText.Contains("$buy") && !brainScript.inBattle) {
                    string[] msgSplit = msg.chatMessagePlainText.Split(' ');
                    if (msgSplit[0] == "$buy") {
                        if (brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().myGold >= 100) {
                            if (msgSplit[1] == "manapotion"){
                                    brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().manaPotionsInventory.Add("mana potion");
                                    brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().myGold -= 100;
                            }
                            if (msgSplit[1] == "healingpotion"){    
                                    brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().healingPotionsInventory.Add("healing potion");
                                    brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().myGold -= 100;
                            }
                        }
                        else{
                            NotEnoughGold(msg.userName);
                        }
                    }
                }
                ///implementation of $heal command (Healing Word) for Cleric (placing it here so it can be used outside of battle)
                if (msg.chatMessagePlainText.Contains("$heal") && brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().characterClass == "cleric" && brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().currentMP >= 8) {
                    string[] msgSplit = msg.chatMessagePlainText.Split(' ');
                    if (msgSplit.Length != 2 && msgSplit.Length != 3 && msgSplit.Length != 4) {
                        TwitchChatClient.singleton.SendMessage("mrpooley92", msg.userName + " I don't think you typed that right.");
                    }
                    else {
                        if (brainScript.inBattle) {
                            foreach (GameObject character in brainScript.currentBattleInstance.AllBattleMembers()) {
                                if (NameMatch(character.name, msgSplit)) {
                                    StartCoroutine(brainScript.currentBattleInstance.ExecuteBuff(msg.userName, character.name, brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().Heal(character), "Heal"));
                                    break;
                                }
                            }
                        }
                        else {
                            foreach (GameObject player in brainScript.party) {
                                if (NameMatch(player.name, msgSplit)) {
                                    string name = player.name;
                                    StartCoroutine(brainScript.ExecuteBuffOutsideOfBattle(msg.userName, player.name, brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().Heal(player), "Heal"));
                                    break;
                                }
                            }
                        }
                    }
                }
                ////-------------BELOW COMMANDS ONLY PASS IF CURRENTLY IN BATTLE
                if (brainScript.inBattle) {
                    ///implementation of basic attack command
                    if (msg.chatMessagePlainText.Contains("$basicattack")) {
                        if (brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().waitingForAction == false){
                            string[] msgSplit = msg.chatMessagePlainText.Split(' ');
                            if (msgSplit.Length != 2 && msgSplit.Length != 3 && msgSplit.Length != 4){
                                TwitchChatClient.singleton.SendMessage("mrpooley92", msg.userName + " I don't think you typed that right.");
                            }
                            else{
                                List<GameObject> enemyParty = brainScript.currentBattleInstance.enemiesInCurrentParty;
                                foreach (GameObject enemy in enemyParty){
                                    if (NameMatch(enemy.name, msgSplit)){
                                        if (brainScript.currentBattleInstance.GetMemberInBattle(msg.userName).GetComponent<CharacterSheet>().weaponType == "melee"){
                                            StartCoroutine(brainScript.currentBattleInstance.ExecuteAttack(msg.userName, enemy.name, brainScript.currentBattleInstance.GetMemberInBattle(msg.userName).GetComponent<CharacterSheet>().MeleeBasicAttack(enemy.GetComponent<CharacterSheet>().armorClass), "Melee Basic Attack", true));
                                            break;
                                        }
                                        else if (brainScript.currentBattleInstance.GetMemberInBattle(msg.userName).GetComponent<CharacterSheet>().weaponType == "ranged"){
                                            StartCoroutine(brainScript.currentBattleInstance.ExecuteAttack(msg.userName, enemy.name, brainScript.currentBattleInstance.GetMemberInBattle(msg.userName).GetComponent<CharacterSheet>().RangedBasicAttack(enemy.GetComponent<CharacterSheet>().armorClass), "Ranged Basic Attack", false));
                                            break;
                                        }
                                        else{
                                            Debug.Log("failed to match weapon type - attack error");
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else{
                            CooldownAlert(msg.userName);
                        }
                    }
                    /////---------------BELOW IS COMMANDS FOR CLASS SPECIFIC POWERS/ATTACKS
                    string charClass = brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().characterClass;
                    ///warrior powers/abilities
                    if (charClass == "warrior"){
                        ///implementation of Cleave command (Cleave)
                        if (msg.chatMessagePlainText.Contains("$cleave")){
                            if (brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().waitingForAction == false){
                                string[] msgSplit = msg.chatMessagePlainText.Split(' ');
                                if (msgSplit.Length != 2 && msgSplit.Length != 3 && msgSplit.Length != 4){
                                    TwitchChatClient.singleton.SendMessage("mrpooley92", msg.userName + " I don't think you typed that right.");
                                }
                                else{
                                    List<GameObject> enemyParty = brainScript.currentBattleInstance.enemiesInCurrentParty;
                                    foreach (GameObject enemy in enemyParty){
                                        if (NameMatch(enemy.name, msgSplit)){
                                            StartCoroutine(brainScript.currentBattleInstance.ExecuteAttack(msg.userName, enemy.name, brainScript.currentBattleInstance.GetMemberInBattle(msg.userName).GetComponent<CharacterSheet>().Cleave(enemy.GetComponent<CharacterSheet>().armorClass, enemy), "Cleave", true));
                                            break;
                                        }
                                    }
                                }
                            }
                            else{
                                CooldownAlert(msg.userName);
                            }
                        }
                        ///implementation of Focused Strike command (Sure Strike)
                        else if (msg.chatMessagePlainText.Contains("$focusedstrike")){
                            if (brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().waitingForAction == false){
                                string[] msgSplit = msg.chatMessagePlainText.Split(' ');
                                if (msgSplit.Length != 2 && msgSplit.Length != 3 && msgSplit.Length != 4){
                                    TwitchChatClient.singleton.SendMessage("mrpooley92", msg.userName + " I don't think you typed that right.");
                                }
                                else{
                                    List<GameObject> enemyParty = brainScript.currentBattleInstance.enemiesInCurrentParty;
                                    foreach (GameObject enemy in enemyParty){
                                        if (NameMatch(enemy.name, msgSplit)){
                                            StartCoroutine(brainScript.currentBattleInstance.ExecuteAttack(msg.userName, enemy.name, brainScript.currentBattleInstance.GetMemberInBattle(msg.userName).GetComponent<CharacterSheet>().FocusedStrike(enemy.GetComponent<CharacterSheet>().armorClass, enemy), "Focused Strike", true));
                                            break;
                                        }
                                    }
                                }
                            }
                            else{
                                CooldownAlert(msg.userName);
                            }
                        }
                        ///implementation of Slowing Strike command (Steel Serpent Strike)
                        else if (msg.chatMessagePlainText.Contains("$slowingstrike") && brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().currentMP >= 4){
                            if (brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().waitingForAction == false){
                                string[] msgSplit = msg.chatMessagePlainText.Split(' ');
                                if (msgSplit.Length != 2 && msgSplit.Length != 3 && msgSplit.Length != 4){
                                    TwitchChatClient.singleton.SendMessage("mrpooley92", msg.userName + " I don't think you typed that right.");
                                }
                                else{
                                    List<GameObject> enemyParty = brainScript.currentBattleInstance.enemiesInCurrentParty;
                                    foreach (GameObject enemy in enemyParty){
                                        if (NameMatch(enemy.name, msgSplit)){
                                            StartCoroutine(brainScript.currentBattleInstance.ExecuteAttack(msg.userName, enemy.name, brainScript.currentBattleInstance.GetMemberInBattle(msg.userName).GetComponent<CharacterSheet>().SlowingStrike(enemy.GetComponent<CharacterSheet>().armorClass, enemy), "Slowing Strike", true));
                                            break;
                                        }
                                    }
                                }
                            }
                            else{
                                CooldownAlert(msg.userName);
                            }
                        }
                        ///implementation of Shattering Strike command (Brute Strike)
                        else if (msg.chatMessagePlainText.Contains("$shatteringstrike") && brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().currentMP >= 12){
                            if (brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().waitingForAction == false){
                                string[] msgSplit = msg.chatMessagePlainText.Split(' ');
                                if (msgSplit.Length != 2 && msgSplit.Length != 3 && msgSplit.Length != 4){
                                    TwitchChatClient.singleton.SendMessage("mrpooley92", msg.userName + " I don't think you typed that right.");
                                }
                                else{
                                    List<GameObject> enemyParty = brainScript.currentBattleInstance.enemiesInCurrentParty;
                                    foreach (GameObject enemy in enemyParty){
                                        if (NameMatch(enemy.name, msgSplit)){
                                            StartCoroutine(brainScript.currentBattleInstance.ExecuteAttack(msg.userName, enemy.name, brainScript.currentBattleInstance.GetMemberInBattle(msg.userName).GetComponent<CharacterSheet>().ShatteringStrike(enemy.GetComponent<CharacterSheet>().armorClass, enemy), "Shattering Strike", true));
                                            break;
                                        }
                                    }
                                }
                            }
                            else{
                                CooldownAlert(msg.userName);
                            }
                        }
                    }

                    ///ranger powers/abilities
                    else if (charClass == "ranger") {
                        ///implementation of $doubleshot command (Twin Strike)
                        if (msg.chatMessagePlainText.Contains("$doubleshot")){
                            if (brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().waitingForAction == false){
                                string[] msgSplit = msg.chatMessagePlainText.Split(' ');
                                if (msgSplit.Length != 2 && msgSplit.Length != 3 && msgSplit.Length != 4){
                                    TwitchChatClient.singleton.SendMessage("mrpooley92", msg.userName + " I don't think you typed that right.");
                                }
                                else{
                                    List<GameObject> enemyParty = brainScript.currentBattleInstance.enemiesInCurrentParty;
                                    foreach (GameObject enemy in enemyParty){
                                        if (NameMatch(enemy.name, msgSplit)){
                                            int randomEnemyid = Random.Range(0, brainScript.currentBattleInstance.enemiesInCurrentParty.Count);
                                            StartCoroutine(brainScript.currentBattleInstance.ExecuteAttack(msg.userName, enemy.name, brainScript.currentBattleInstance.GetMemberInBattle(msg.userName).GetComponent<CharacterSheet>().Doubleshot(enemy.GetComponent<CharacterSheet>().armorClass), "Doubleshot", false));
                                            StartCoroutine(brainScript.currentBattleInstance.ExecuteAttack(msg.userName, brainScript.currentBattleInstance.enemiesInCurrentParty[randomEnemyid].name, brainScript.currentBattleInstance.GetMemberInBattle(msg.userName).GetComponent<CharacterSheet>().Doubleshot(brainScript.currentBattleInstance.enemiesInCurrentParty[randomEnemyid].GetComponent<CharacterSheet>().armorClass), "Doubleshot", false));
                                            ActionConfirmedAlert(msg.userName);
                                            break;
                                        }
                                    }
                                }
                            }
                            else {
                                CooldownAlert(msg.userName);
                            }
                        }
                        ///implementation of $sureshot command (Careful Attack)
                        else if (msg.chatMessagePlainText.Contains("$sureshot")) {
                            if (brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().waitingForAction == false){
                                string[] msgSplit = msg.chatMessagePlainText.Split(' ');
                                if (msgSplit.Length != 2 && msgSplit.Length != 3 && msgSplit.Length != 4){
                                    TwitchChatClient.singleton.SendMessage("mrpooley92", msg.userName + " I don't think you typed that right.");
                                }
                                else{
                                    List<GameObject> enemyParty = brainScript.currentBattleInstance.enemiesInCurrentParty;
                                    foreach (GameObject enemy in enemyParty){
                                        if (NameMatch(enemy.name, msgSplit)){
                                            StartCoroutine(brainScript.currentBattleInstance.ExecuteAttack(msg.userName, enemy.name, brainScript.currentBattleInstance.GetMemberInBattle(msg.userName).GetComponent<CharacterSheet>().Sureshot(enemy.GetComponent<CharacterSheet>().armorClass), "Sureshot", false));
                                            break;
                                        }
                                    }
                                }
                            }
                            else{
                                CooldownAlert(msg.userName);
                            }
                        }
                        ///implementation of $doublestrike command (Two-Fanged Strike)
                        else if (msg.chatMessagePlainText.Contains("$doublestrike") && brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().currentMP >= 4) {
                            if (brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().waitingForAction == false){
                                string[] msgSplit = msg.chatMessagePlainText.Split(' ');
                                if (msgSplit.Length != 2 && msgSplit.Length != 3 && msgSplit.Length != 4){
                                    TwitchChatClient.singleton.SendMessage("mrpooley92", msg.userName + " I don't think you typed that right.");
                                }
                                else{
                                    List<GameObject> enemyParty = brainScript.currentBattleInstance.enemiesInCurrentParty;
                                    foreach (GameObject enemy in enemyParty){
                                        if (NameMatch(enemy.name, msgSplit)){
                                            StartCoroutine(brainScript.currentBattleInstance.ExecuteAttack(msg.userName, enemy.name, brainScript.currentBattleInstance.GetMemberInBattle(msg.userName).GetComponent<CharacterSheet>().Doublestrike(enemy.GetComponent<CharacterSheet>().armorClass), "Double-Strike", false));
                                            break;
                                        }
                                    }
                                }
                            }
                            else{
                                CooldownAlert(msg.userName);
                            }
                        }
                        ///implementation of $criticalshot command (Hunter's Bear Trap)
                        else if (msg.chatMessagePlainText.Contains("$criticalshot") && brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().currentMP >= 12) {
                            if (brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().waitingForAction == false){
                                string[] msgSplit = msg.chatMessagePlainText.Split(' ');
                                if (msgSplit.Length != 2 && msgSplit.Length != 3 && msgSplit.Length != 4){
                                    TwitchChatClient.singleton.SendMessage("mrpooley92", msg.userName + " I don't think you typed that right.");
                                }
                                else{
                                    List<GameObject> enemyParty = brainScript.currentBattleInstance.enemiesInCurrentParty;
                                    foreach (GameObject enemy in enemyParty){
                                        if (NameMatch(enemy.name, msgSplit)){
                                            StartCoroutine(brainScript.currentBattleInstance.ExecuteAttack(msg.userName, enemy.name, brainScript.currentBattleInstance.GetMemberInBattle(msg.userName).GetComponent<CharacterSheet>().CriticalShot(enemy.GetComponent<CharacterSheet>().armorClass, enemy), "Critical Shot", false));
                                            break;
                                        }
                                    }
                                }
                            }
                            else{
                                CooldownAlert(msg.userName);
                            }
                        }
                    }
                    ///cleric powers/abilities
                    else if (charClass == "cleric") {
                        ///implementation of $focus command (Divine Fortune) for Cleric
                        if (msg.chatMessagePlainText.Contains("$focus") && brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().currentMP >= 4) {
                            string[] msgSplit = msg.chatMessagePlainText.Split(' ');
                            if (msgSplit.Length != 2 && msgSplit.Length != 3 && msgSplit.Length != 4) {
                                TwitchChatClient.singleton.SendMessage("mrpooley92", msg.userName + " I don't think you typed that right.");
                            }
                            else {
                                foreach (GameObject character in brainScript.currentBattleInstance.AllBattleMembers()) {
                                    if (NameMatch(character.name, msgSplit)) {
                                        StartCoroutine(brainScript.currentBattleInstance.ExecuteBuff(msg.userName, character.name, brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().Focus(character), "Focus"));
                                        break;
                                    }
                                }
                            }
                        }
                        ///implementation of WeakeningBlow command (Lance of Faith)
                        else if (msg.chatMessagePlainText.Contains("$weakeningblow")){
                            if (brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().waitingForAction == false){
                                string[] msgSplit = msg.chatMessagePlainText.Split(' ');
                                if (msgSplit.Length != 2 && msgSplit.Length != 3 && msgSplit.Length != 4){
                                    TwitchChatClient.singleton.SendMessage("mrpooley92", msg.userName + " I don't think you typed that right.");
                                }
                                else{
                                    List<GameObject> enemyParty = brainScript.currentBattleInstance.enemiesInCurrentParty;
                                    foreach (GameObject enemy in enemyParty){
                                        if (NameMatch(enemy.name, msgSplit)){
                                            StartCoroutine(brainScript.currentBattleInstance.ExecuteAttack(msg.userName, enemy.name, brainScript.currentBattleInstance.GetMemberInBattle(msg.userName).GetComponent<CharacterSheet>().WeakeningBlow(enemy.GetComponent<CharacterSheet>().reflexScore, enemy), "Weakening Blow", true));
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                CooldownAlert(msg.userName);
                            }
                        }
                        ///implementation of Shout Strike command (Priest's Shield)
                        else if (msg.chatMessagePlainText.Contains("$shoutstrike")){
                            if (brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().waitingForAction == false){
                                string[] msgSplit = msg.chatMessagePlainText.Split(' ');
                                if (msgSplit.Length != 2 && msgSplit.Length != 3 && msgSplit.Length != 4){
                                    TwitchChatClient.singleton.SendMessage("mrpooley92", msg.userName + " I don't think you typed that right.");
                                }
                                else{
                                    List<GameObject> enemyParty = brainScript.currentBattleInstance.enemiesInCurrentParty;
                                    foreach (GameObject enemy in enemyParty){
                                        if (NameMatch(enemy.name, msgSplit)){
                                            StartCoroutine(brainScript.currentBattleInstance.ExecuteAttack(msg.userName, enemy.name, brainScript.currentBattleInstance.GetMemberInBattle(msg.userName).GetComponent<CharacterSheet>().ShoutStrike(enemy.GetComponent<CharacterSheet>().armorClass, enemy), "Shout Strike", true));
                                            break;
                                        }
                                    }
                                }
                            }
                            else{
                                CooldownAlert(msg.userName);
                            }
                        }
                        ///implementation of Righteous Fury command (Healing Strike)
                        else if (msg.chatMessagePlainText.Contains("$righteousfury") && brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().currentMP >= 8){
                            if (brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().waitingForAction == false){
                                string[] msgSplit = msg.chatMessagePlainText.Split(' ');
                                if (msgSplit.Length != 2 && msgSplit.Length != 3 && msgSplit.Length != 4){
                                    TwitchChatClient.singleton.SendMessage("mrpooley92", msg.userName + " I don't think you typed that right.");
                                }
                                else{
                                    List<GameObject> enemyParty = brainScript.currentBattleInstance.enemiesInCurrentParty;
                                    foreach (GameObject enemy in enemyParty){
                                        if (NameMatch(enemy.name, msgSplit)){
                                            StartCoroutine(brainScript.currentBattleInstance.ExecuteAttack(msg.userName, enemy.name, brainScript.currentBattleInstance.GetMemberInBattle(msg.userName).GetComponent<CharacterSheet>().RighteousFury(enemy.GetComponent<CharacterSheet>().armorClass, enemy), "Righteous Fury", true));
                                            break;
                                        }
                                    }
                                }
                            }
                            else{
                                CooldownAlert(msg.userName);
                            }
                        }
                        ///implementation of Prayer command (Beacon of Hope)
                        else if (msg.chatMessagePlainText.Contains("$prayer") && brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().currentMP >= 12){
                            if (brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().waitingForAction == false){
                                string[] msgSplit = msg.chatMessagePlainText.Split(' ');
                                if (msgSplit.Length != 2 && msgSplit.Length != 3 && msgSplit.Length != 4){
                                    TwitchChatClient.singleton.SendMessage("mrpooley92", msg.userName + " I don't think you typed that right.");
                                }
                                else{
                                    List<GameObject> enemyParty = brainScript.currentBattleInstance.enemiesInCurrentParty;
                                    foreach (GameObject enemy in enemyParty){
                                        if (NameMatch(enemy.name, msgSplit)){
                                            StartCoroutine(brainScript.currentBattleInstance.ExecuteAttack(msg.userName, enemy.name, brainScript.currentBattleInstance.GetMemberInBattle(msg.userName).GetComponent<CharacterSheet>().Prayer(enemy.GetComponent<CharacterSheet>().willScore, enemy), "Prayer", true));
                                            break;
                                        }
                                    }
                                }
                            }
                            else{
                                CooldownAlert(msg.userName);
                            }
                        }
                    }
                    ///rogue powers/abilities
                    else if (charClass == "rogue") {
                        ///implementation of TwistingStab command (Torturous Strike)
                        if (msg.chatMessagePlainText.Contains("$twistingstab") && brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().currentMP >= 4){
                            if (brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().waitingForAction == false){
                                string[] msgSplit = msg.chatMessagePlainText.Split(' ');
                                if (msgSplit.Length != 2 && msgSplit.Length != 3 && msgSplit.Length != 4){
                                    TwitchChatClient.singleton.SendMessage("mrpooley92", msg.userName + " I don't think you typed that right.");
                                }
                                else{
                                    List<GameObject> enemyParty = brainScript.currentBattleInstance.enemiesInCurrentParty;
                                    foreach (GameObject enemy in enemyParty){
                                        if (NameMatch(enemy.name, msgSplit)){
                                            StartCoroutine(brainScript.currentBattleInstance.ExecuteAttack(msg.userName, enemy.name, brainScript.currentBattleInstance.GetMemberInBattle(msg.userName).GetComponent<CharacterSheet>().TwistingStab(enemy.GetComponent<CharacterSheet>().armorClass), "Twisting Stab", true));
                                            break;
                                        }
                                    }
                                }
                            }
                            else{
                                CooldownAlert(msg.userName);
                            }
                        }
                        ///implementation of Maim command (Easy Target)
                        else if (msg.chatMessagePlainText.Contains("$maim") && brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().currentMP >= 12) {
                            if (brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().waitingForAction == false){
                                string[] msgSplit = msg.chatMessagePlainText.Split(' ');
                                if (msgSplit.Length != 2 && msgSplit.Length != 3 && msgSplit.Length != 4){
                                    TwitchChatClient.singleton.SendMessage("mrpooley92", msg.userName + " I don't think you typed that right.");
                                }
                                else{
                                    List<GameObject> enemyParty = brainScript.currentBattleInstance.enemiesInCurrentParty;
                                    foreach (GameObject enemy in enemyParty){
                                        if (NameMatch(enemy.name, msgSplit)){
                                            StartCoroutine(brainScript.currentBattleInstance.ExecuteAttack(msg.userName, enemy.name, brainScript.currentBattleInstance.GetMemberInBattle(msg.userName).GetComponent<CharacterSheet>().Maim(enemy.GetComponent<CharacterSheet>().armorClass, enemy), "Maim", true));
                                            break;
                                        }
                                    }
                                }
                            }
                            else{
                                CooldownAlert(msg.userName);
                            }
                        }
                        ///implementation of Backstab command (Sly Flourish)
                        else if (msg.chatMessagePlainText.Contains("$backstab")){
                            if (brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().waitingForAction == false){
                                string[] msgSplit = msg.chatMessagePlainText.Split(' ');
                                if (msgSplit.Length != 2 && msgSplit.Length != 3 && msgSplit.Length != 4){
                                    TwitchChatClient.singleton.SendMessage("mrpooley92", msg.userName + " I don't think you typed that right.");
                                }
                                else{
                                    List<GameObject> enemyParty = brainScript.currentBattleInstance.enemiesInCurrentParty;
                                    foreach (GameObject enemy in enemyParty){
                                        if (NameMatch(enemy.name, msgSplit)){
                                            StartCoroutine(brainScript.currentBattleInstance.ExecuteAttack(msg.userName, enemy.name, brainScript.currentBattleInstance.GetMemberInBattle(msg.userName).GetComponent<CharacterSheet>().Backstab(enemy.GetComponent<CharacterSheet>().armorClass), "Backstab", true));
                                            break;
                                        }
                                    }
                                }
                            }
                            else{
                                CooldownAlert(msg.userName);
                            }
                        }
                        ///implementation of Slice command (Piercing Strike)
                        else if (msg.chatMessagePlainText.Contains("$slice")) {
                            if (brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().waitingForAction == false){
                                string[] msgSplit = msg.chatMessagePlainText.Split(' ');
                                if (msgSplit.Length != 2 && msgSplit.Length != 3 && msgSplit.Length != 4){
                                    TwitchChatClient.singleton.SendMessage("mrpooley92", msg.userName + " I don't think you typed that right.");
                                }
                                else{
                                    List<GameObject> enemyParty = brainScript.currentBattleInstance.enemiesInCurrentParty;
                                    foreach (GameObject enemy in enemyParty){
                                        if (NameMatch(enemy.name, msgSplit)){
                                            StartCoroutine(brainScript.currentBattleInstance.ExecuteAttack(msg.userName, enemy.name, brainScript.currentBattleInstance.GetMemberInBattle(msg.userName).GetComponent<CharacterSheet>().Slice(enemy.GetComponent<CharacterSheet>().reflexScore), "Slice", true));
                                            break;
                                        }
                                    }
                                }
                            }
                            else{
                                CooldownAlert(msg.userName);
                            }
                        }
                    }
                    ///wizard powers/abilities
                    else if (charClass == "wizard") {
                        ///implementation of bolt command (Magic Missile)
                        if (msg.chatMessagePlainText.Contains("$bolt")){
                            if (brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().waitingForAction == false){
                                string[] msgSplit = msg.chatMessagePlainText.Split(' ');
                                if (msgSplit.Length != 2 && msgSplit.Length != 3 && msgSplit.Length != 4){
                                    TwitchChatClient.singleton.SendMessage("mrpooley92", msg.userName + " I don't think you typed that right.");
                                }
                                else{
                                    List<GameObject> enemyParty = brainScript.currentBattleInstance.enemiesInCurrentParty;
                                    foreach (GameObject enemy in enemyParty){
                                        if (NameMatch(enemy.name, msgSplit)){
                                            StartCoroutine(brainScript.currentBattleInstance.ExecuteAttack(msg.userName, enemy.name, brainScript.currentBattleInstance.GetMemberInBattle(msg.userName).GetComponent<CharacterSheet>().Bolt(enemy.GetComponent<CharacterSheet>().reflexScore, enemy), "Bolt", false));
                                            break;
                                        }
                                    }
                                }
                            }
                            else{
                                CooldownAlert(msg.userName);
                            }
                        }
                        ///implementation of frost command (Ray of Frost)
                        if (msg.chatMessagePlainText.Contains("$frost")){
                            if (brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().waitingForAction == false){
                                string[] msgSplit = msg.chatMessagePlainText.Split(' ');
                                if (msgSplit.Length != 2 && msgSplit.Length != 3 && msgSplit.Length != 4){
                                    TwitchChatClient.singleton.SendMessage("mrpooley92", msg.userName + " I don't think you typed that right.");
                                }
                                else{
                                    List<GameObject> enemyParty = brainScript.currentBattleInstance.enemiesInCurrentParty;
                                    foreach (GameObject enemy in enemyParty){
                                        if (NameMatch(enemy.name, msgSplit)){
                                            StartCoroutine(brainScript.currentBattleInstance.ExecuteAttack(msg.userName, enemy.name, brainScript.currentBattleInstance.GetMemberInBattle(msg.userName).GetComponent<CharacterSheet>().Frost(enemy.GetComponent<CharacterSheet>().fortitudeScore, enemy), "Frost", false));
                                            break;
                                        }
                                    }
                                }
                            }
                            else{
                                CooldownAlert(msg.userName);
                            }
                        }
                        ///implementation of fireblast command (Burning Hands)
                        if (msg.chatMessagePlainText.Contains("$fireblast") && brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().currentMP >= 5){
                            if (brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().waitingForAction == false){
                                string[] msgSplit = msg.chatMessagePlainText.Split(' ');
                                if (msgSplit.Length != 2 && msgSplit.Length != 3 && msgSplit.Length != 4){
                                    TwitchChatClient.singleton.SendMessage("mrpooley92", msg.userName + " I don't think you typed that right.");
                                }
                                else{
                                    List<GameObject> enemyParty = brainScript.currentBattleInstance.enemiesInCurrentParty;
                                    foreach (GameObject enemy in enemyParty){
                                        if (NameMatch(enemy.name, msgSplit)){
                                            StartCoroutine(brainScript.currentBattleInstance.ExecuteAttack(msg.userName, enemy.name, brainScript.currentBattleInstance.GetMemberInBattle(msg.userName).GetComponent<CharacterSheet>().FireBlast(enemy.GetComponent<CharacterSheet>().reflexScore, enemy), "Fire Blast", false));
                                            break;
                                        }
                                    }
                                }
                            }
                            else{
                                CooldownAlert(msg.userName);
                            }
                        }
                        ///implementation of Blade Storm command (Acid Arrow)
                        if (msg.chatMessagePlainText.Contains("$bladestorm") && brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().currentMP >= 12){
                            if (brainScript.GetMemberInParty(msg.userName).GetComponent<CharacterSheet>().waitingForAction == false){
                                string[] msgSplit = msg.chatMessagePlainText.Split(' ');
                                if (msgSplit.Length != 2 && msgSplit.Length != 3 && msgSplit.Length != 4){
                                    TwitchChatClient.singleton.SendMessage("mrpooley92", msg.userName + " I don't think you typed that right.");
                                }
                                else{
                                    List<GameObject> enemyParty = brainScript.currentBattleInstance.enemiesInCurrentParty;
                                    foreach (GameObject enemy in enemyParty){
                                        if (NameMatch(enemy.name, msgSplit)){
                                            StartCoroutine(brainScript.currentBattleInstance.ExecuteAttack(msg.userName, enemy.name, brainScript.currentBattleInstance.GetMemberInBattle(msg.userName).GetComponent<CharacterSheet>().BladeStorm(enemy.GetComponent<CharacterSheet>().reflexScore, enemy), "Blade Storm", false));
                                            break;
                                        }
                                    }
                                }
                            }
                            else{
                                CooldownAlert(msg.userName);
                            }
                        }
                    }
                    ///-----ABOVE IS COMMANDS FOR CLASS SPECIFIC POWERS/ATTACKS
                }
                ////----------------ABOVE COMMANDS ONLY PASS IF CURRENTLY IN BATTLE
            }
            ///-------------------ABOVE IS ALL COMMANDS ONLY PASS IF MSG OWNER IS IN ACTUALLY IN THE PARTY ONLY

            ///implementation of $joinparty command
            if (msg.chatMessagePlainText.Equals("$joinparty")) {
                if (!brainScript.viewersWaitingToJoin.Contains(msg.userName)){
                    if (!brainScript.InParty(msg.userName)) {
                        if (!brainScript.inBattle){
                            brainScript.viewersWaitingToJoin.Add(msg.userName);
                            TwitchChatClient.singleton.SendMessage("mrpooley92", msg.userName + " has joined the waiting list!");
                        }
                        else{
                            bool playerInList = false;
                            foreach (string username in brainScript.usersParticipatedInThisBattle) {
                                if (username == msg.userName) {
                                    playerInList = true;
                                    TwitchChatClient.singleton.SendMessage("mrpooley92", msg.userName + " please try to join again once this battle is over.");
                                    break;
                                }
                            }
                            if (!playerInList) {
                                brainScript.viewersWaitingToJoin.Add(msg.userName);
                            }
                        }
                    }
                }
            }
            ///implementation of $confirm command to join the party
            else if (msg.chatMessagePlainText.Equals("$confirm") && brainScript.playerWaitingFor == msg.userName && brainScript.waitingForPlayerToAccept == true) {
                TwitchChatClient.singleton.SendMessage("mrpooley92", msg.userName + " joined the party!");
                brainScript.alertBoxScript.NewAlert(msg.userName + " joined the party!", brainScript.HtmlColorConvert(msg.userNameColor));
                brainScript.tempColor = msg.userNameColor;
                brainScript.confirmed = true;
            }
            ///implementation of $listparty command
            else if (msg.chatMessagePlainText.Equals("$listparty")) {
                ListPartyMembers();
            }
            ///implementation of $listwaiting
            else if (msg.chatMessagePlainText.Equals("$listwaiting")) {
                ListWaitingViewers();
            }
            ///implementation of $DELETEME command
            else if (msg.chatMessagePlainText.Equals("$DELETEME"))
            {
                brainScript.DeletePlayer(msg.userName);
                if (brainScript.InParty(msg.userName)){
                    brainScript.RemovePlayer(msg.userName, false);
                }
                
                TwitchChatClient.singleton.SendMessage("mrpooley92", msg.userName + " DELETED!");
                brainScript.alertBoxScript.NewAlert(msg.userName + " DELETED!", Color.white);
            }
            ///implementation of joke command
            else if (msg.chatMessagePlainText.Equals("$kill mrpooley92") || msg.chatMessagePlainText.Equals("$killmrpooley92")) {
                TwitchChatClient.singleton.SendMessage("mrpooley92", "Yea, nice try " + msg.userName);
            }
            ///implementation of joke command
            else if (msg.chatMessagePlainText.Equals("$attack mrpooley92") || msg.chatMessagePlainText.Equals("$basicattack mrpooley92")) {
                TwitchChatClient.singleton.SendMessage("mrpooley92", "Ouch " + msg.userName + " . Very ouch.");
            }
        }

        ///---------------------if message did not start with '$'
        else {
            //DONT RESPOND BECAUSE WE'RE ONLY ACCEPTING COMMANDS THAT START WITH $
        }
    }
    public void ConfirmJoin(string userName) {
        TwitchChatClient.singleton.SendMessage("mrpooley92", userName + " is up next! " + userName + " please confirm by typing '$confirm'. You have 60 seconds to do so.");
    }
    public void NoManaPotion(string userName)
    {
        TwitchChatClient.singleton.SendMessage("mrpooley92", userName + " you don't have a mana potion!");
    }
    public void NoHealthPotion(string userName)
    {
        TwitchChatClient.singleton.SendMessage("mrpooley92", userName + " you don't have a health potion!");
    }
    public void NotEnoughGold(string userName)
    {
        TwitchChatClient.singleton.SendMessage("mrpooley92", userName + " you don't have enough gold!");
    }
    public void CooldownAlert(string userName){
        TwitchChatClient.singleton.SendMessage("mrpooley92", userName + " you are still on cooldwown!");
    }
    public void ActionConfirmedAlert(string userName){
        TwitchChatClient.singleton.SendMessage("mrpooley92", userName + " : Action confirmed.");
    }
    public void NeverConfirmed(string userName) {
        TwitchChatClient.singleton.SendMessage("mrpooley92", "Did not receive confirmation from " + userName + " . Please rejoin the waiting list.");
    }
    public void ListPartyMembers() {
        TwitchChatClient.singleton.SendMessage("mrpooley92", brainScript.GetPartyMembersString());
    }
    public void ListWaitingViewers() {
        TwitchChatClient.singleton.SendMessage("mrpooley92", brainScript.GetWaitingViewersString());
    }
    public void WaitingOnConfirmAlert(int secondsLeft, string viewerWaitingOn) {
        TwitchChatClient.singleton.SendMessage("mrpooley92", viewerWaitingOn + " you have " + secondsLeft + " seconds left to confirm. Confirm by typing '$confirm'.");
    }
    public void PleaseAssignClassAndGender(string userName) {
        TwitchChatClient.singleton.SendMessage("mrpooley92", userName + " please assign your gender and class by typing '$assign gender class'. (E.g. '$assign male wizard')");
    }
    public bool NameMatch(string enemyName, string[] enteredName) {
        string finalEnemyName = "";
        if (enteredName.Length == 2){
            finalEnemyName = enteredName[1];
        }
        else if (enteredName.Length == 3){
            finalEnemyName = enteredName[1] + " " + enteredName[2];
        }
        else if (enteredName.Length == 4){
            finalEnemyName = enteredName[1] + " " + enteredName[2] + " " + enteredName[3];
        }
        if (enemyName.ToLower() == finalEnemyName || enemyName == finalEnemyName){
            return true;
        }
        else {
            return false;
        }
    }

    // You'd define your whisper callback like this:
    public void OnWhisper(ref TwitchChatMessage msg){    
        // Do something with the whisper here.
    }

    void Cleanup(){
        // When you're done, leave the channels and remove the chat listeners.
        TwitchChatClient.singleton.LeaveChannel("mrpooley92");
        TwitchChatClient.singleton.RemoveChatListener(OnChatMessage);
        // Also remove any whisper listeners you've added.
        TwitchChatClient.singleton.RemoveWhisperListener(OnWhisper);
    }
}
