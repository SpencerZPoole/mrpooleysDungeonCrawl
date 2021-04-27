using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
[System.Serializable]
public class Brain1 : MonoBehaviour
{
    public TwitchChatterDungeon twitchDungeonScript;
    public List<GameObject> party = new List<GameObject>();
    public BattleInstance currentBattleInstance;
    public List<String> viewersWaitingToJoin = new List<String>();
    public List<String> savedCharacters = new List<String>();
    public GameObject partyGO;
    public bool waitingForPlayerToAccept = false;
    public string playerWaitingFor;
    public bool confirmed = false;
    public GameObject[] playerPanels;
    public GameObject[] playerBattlePositions;
    public GameObject[] enemyPanels;
    public GameObject[] enemyBattlePositions;
    public List<string> tempEnemiesStringList;
    public bool inBattle = false;
    public string tempColor;
    public Color convertedColor;
    public FloatingCombatTextScript floatingTextScript;
    public AlertBoxScript alertBoxScript;
    public Text roomCountText;
    public GameObject readyUpPanel;
    public GameObject newDungeonPanel;
    public GameObject dungeonCompletePanel;
    public GameObject welcomePanel;
    public GameObject[] readyUpNameGOs;
    public GameObject[] newDungeonPanelNameGOs;
    public GameObject[] dungeonCompletePanelNameGOs;
    public GameObject[] welcomePanelNameGOs;
    public DungeonRunInstance currentDungeon;
    public bool dungeonExistsRightNow = false;
    public GameObject wipeAlert;
    public Text wipeAlertRoomsPassedText;
    public string difficulty = "easy";
    public List<string> usersParticipatedInThisBattle;

    private void Awake()
    {
        Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
    }

    // Use this for initialization
    void Start()
    {
        welcomePanel.SetActive(true);
        readyUpPanel.SetActive(false);
        dungeonCompletePanel.SetActive(false);
        newDungeonPanel.SetActive(false);
        roomCountText.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (party.Count > 0 && !dungeonExistsRightNow) {
            if (party.Count < 4 && viewersWaitingToJoin.Count != 0)
            {

            }
            else if(ReadyCheck(party))
            {
                StartCoroutine(FromMainMenuDelay());
            }
        }
        ///test to see if we should try and add someone to the party
        if (party.Count < 4 && waitingForPlayerToAccept == false) {
            if (viewersWaitingToJoin.Count > 0) {
                StartCoroutine(AddSomeoneToParty(viewersWaitingToJoin[0]));
            }
        }

    }

    public void SaveCharacterSheet(SheetSave saveSheet) {
        if (System.IO.File.Exists(Application.dataPath + "/" + saveSheet.ownerUsername + ".pdc")) {
            FileUtil.DeleteFileOrDirectory(Application.dataPath + "/" + saveSheet.ownerUsername + ".pdc");
        }
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.dataPath + "/" + saveSheet.ownerUsername + ".pdc");
        bf.Serialize(file, saveSheet);
        file.Close();
    }

    public void DeletePlayer(string username) {
        Debug.Log("DELTE ETSET");
        if (System.IO.File.Exists(Application.dataPath + "/" + username + ".pdc"))
        {
            FileUtil.DeleteFileOrDirectory(Application.dataPath + "/" + username + ".pdc");
        }

    }

    public SheetSave LoadCharacterSheet(string username)
    {
        if (System.IO.File.Exists(Application.dataPath + "/" + username + ".pdc"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.dataPath + "/" + username + ".pdc", FileMode.Open);
            SheetSave loadedSheet = (SheetSave)bf.Deserialize(file);
            file.Close();
            return loadedSheet;
        }
        else return null;
        
    }

    ///method that generates/returns a string of the current members in the party
    public string GetPartyMembersString() {
        string partyMembers = "";
        if (party.Count <= 0) {
            partyMembers += "The party is empty!";
        }
        else {
            partyMembers += "The current party members are: ";
            foreach (GameObject member in party) {
                partyMembers += member.name + " ";
            }
        }
        return partyMembers;
    }

    public bool ReadyCheck(List<GameObject> players)
    {
        bool allReady = true;
        if (players.Count == 0)
        {
            allReady = false;
        }
        foreach (GameObject player in players)
        {
            if (player.GetComponent<CharacterSheet>().readyForNextBattle == false || player.GetComponent<CharacterSheet>().classIsAssigned == false || player.GetComponent<CharacterSheet>().addingXP == true || player.GetComponent<CharacterSheet>().addingGold == true)
            {
                allReady = false;
            }
        }
        return allReady;
    }

    public IEnumerator FromMainMenuDelay() {
        welcomePanel.SetActive(false);
        foreach (GameObject player in party)
        {
            player.GetComponent<CharacterSheet>().readyForNextBattle = false;
        }
        yield return new WaitForSeconds(3);
        MakeNewDungeon(tempEnemiesStringList);
        
    }

    public void MakeNewDungeon(List<string> enemiesStringsList) {
        GameObject dungeon = new GameObject("DungeonGO");
        DungeonRunInstance dungeonInstance = dungeon.AddComponent<DungeonRunInstance>();
        currentDungeon = dungeonInstance;
        dungeonExistsRightNow = true;
        welcomePanel.SetActive(false);
        dungeonInstance.tChatterDungeon = twitchDungeonScript;
        if (difficulty == "easy")
        {
            dungeonInstance.roomCount = 3;
        }
        else if (difficulty == "medium") {
            dungeonInstance.roomCount = 6;
        }
        else
        {
            dungeonInstance.roomCount = 10;
        }
        dungeonInstance.currentRoomNumber = 1;
        dungeonInstance.brain = this;
        dungeonInstance.enemiesToUse = enemiesStringsList;
    }
    ///method that generates/returns a string of the current viewers waiting to join the party
    public string GetWaitingViewersString()
    {
        string waitingString = "";
        if (viewersWaitingToJoin.Count <= 0)
        {
            waitingString += "The waiting list is empty!";
        }
        else
        {
            waitingString += "The current viewers waiting to join are: ";
            foreach (String member in viewersWaitingToJoin)
            {
                waitingString += member + " ";
            }
        }
        return waitingString;
    }

    ///method that returns true or false if the user is or isn't in the party - got tired of rewriting this test
    public bool InParty(string userName) {
        bool inParty = false;
        foreach (GameObject player in party) {
            if (player.name == userName) {
                inParty = true;
                break;
            }
        }
        return inParty;
    }

    public void WipeMethod() {
        StartCoroutine(FlashWipeAlert());
    }

    public IEnumerator FlashWipeAlert() {
        currentBattleInstance.ManualDestroy();
        wipeAlertRoomsPassedText.text = "";
        currentDungeon.ManualDestroy();
        wipeAlert.SetActive(true);
        welcomePanel.SetActive(false);
        readyUpPanel.SetActive(false);
        dungeonCompletePanel.SetActive(false);
        newDungeonPanel.SetActive(false);
        roomCountText.enabled = false;
        
        
        yield return new WaitForSeconds(10);
        wipeAlert.SetActive(false);
        welcomePanel.SetActive(true);
    }

    public GameObject GetMemberInParty(string memberName)
    {
        foreach (GameObject player in party)
        {
            if (player.name == memberName)
            {
                return player;
            }
        }
        return null;
    }

        public void RemovePlayer(string username, bool savePlayer) {
        foreach (GameObject player in party) {
            if (player.name == username) {
                party.Remove(player);
                player.GetComponent<CharacterSheet>().brain = null;
                player.GetComponent<CharacterSheet>().attachedPanel.GetComponent<PlayerPanelScript>().currentPlayerGO = null;
                player.GetComponent<CharacterSheet>().attachedBattlePosition.GetComponent<BattlePositionScript>().ownerGO = null;
                player.GetComponent<CharacterSheet>().attachedBattlePosition.GetComponent<BattlePositionScript>().textChild.SetActive(false);
                player.GetComponent<CharacterSheet>().attachedBattlePosition.GetComponent<BattlePositionScript>().hpChild.SetActive(false);
                player.GetComponent<CharacterSheet>().attachedBattlePosition.GetComponent<BattlePositionScript>().spriteRenderer.enabled = false;
                player.GetComponent<CharacterSheet>().attachedReadyText.GetComponent<AlertBoxPlayerScript>().ownerGO = null;
                player.GetComponent<CharacterSheet>().attachedNewDungeonNameGO.GetComponent<AlertBoxPlayerScript>().ownerGO = null;
                player.GetComponent<CharacterSheet>().attachedCompleteDungeonNameGO.GetComponent<AlertBoxPlayerScript>().ownerGO = null;
                player.GetComponent<CharacterSheet>().attachedWelcomeDungeonNameGO.GetComponent<AlertBoxPlayerScript>().ownerGO = null;
                if (savePlayer)
                {
                    SaveCharacterSheet(player.GetComponent<CharacterSheet>().CreateNewSheetSave());
                }
                GameObject.Destroy(player);
                
            }
        }

    }

    public void RemoveEnemy(string enemyName)
    {
        foreach (GameObject enemy in currentBattleInstance.enemiesInCurrentParty)
        {
            if (enemy.name == enemyName)
            {
                currentBattleInstance.enemiesInCurrentParty.Remove(enemy);
                enemy.GetComponent<CharacterSheet>().attachedPanel.GetComponent<EnemyPanelScript>().currentEnemyGO = null;
                enemy.GetComponent<CharacterSheet>().attachedBattlePosition.GetComponent<BattlePositionScript>().ownerGO = null;
                enemy.GetComponent<CharacterSheet>().attachedBattlePosition.GetComponent<BattlePositionScript>().textChild.SetActive(false);
                enemy.GetComponent<CharacterSheet>().attachedBattlePosition.GetComponent<BattlePositionScript>().hpChild.SetActive(false);
                enemy.GetComponent<CharacterSheet>().attachedBattlePosition.GetComponent<BattlePositionScript>().spriteRenderer.enabled = false;
                GameObject.Destroy(enemy);

            }
        }

    }

    public Color HtmlColorConvert(string htmlColor) {
        Color outColor = Color.black;
        if (ColorUtility.TryParseHtmlString(htmlColor, out convertedColor)) {
            outColor = convertedColor;
            Debug.Log(outColor);
        }
        return outColor;
    }

    public IEnumerator ExecuteBuffOutsideOfBattle(string bufferUsername, string buffeeUsername, float power, string powerName)
    {
        GameObject buffer = GetMemberInParty(bufferUsername);
        GameObject buffee = GetMemberInParty(buffeeUsername);
        if (!buffer.GetComponent<CharacterSheet>().isEnemy)
        {
            twitchDungeonScript.ActionConfirmedAlert(bufferUsername);
        }
        if (buffer.GetComponent<CharacterSheet>().currentHP > 0)
        {
            buffer.GetComponent<CharacterSheet>().attachedBattlePosition.GetComponent<Animator>().SetBool("Flex", true);
        }
        yield return new WaitForSeconds(1f);
        if (buffer.GetComponent<CharacterSheet>().currentHP > 0)
        {
            buffer.GetComponent<CharacterSheet>().Buff(power, buffee, powerName);
        }
    }



    ///method for adding a new player to the party
    public IEnumerator AddSomeoneToParty(string viewerWaitingToJoin) {
        if (party.Count < 4)
        {
            waitingForPlayerToAccept = true;
            confirmed = false;
            int numerator = -1;
            twitchDungeonScript.ConfirmJoin(viewerWaitingToJoin);
            playerWaitingFor = viewerWaitingToJoin;

            while (confirmed == false)
            {
                numerator += 1;
                if (numerator == 20)
                {
                    twitchDungeonScript.WaitingOnConfirmAlert(40, viewerWaitingToJoin);
                }
                else if (numerator == 40) {
                    twitchDungeonScript.WaitingOnConfirmAlert(20, viewerWaitingToJoin);
                }
                else if (numerator == 50)
                {
                    twitchDungeonScript.WaitingOnConfirmAlert(10, viewerWaitingToJoin);
                }

                if (numerator >= 60)
                {
                    break;
                }
       
                yield return new WaitForSeconds(1);
                Debug.Log("Tick " + numerator);
            }

            if (confirmed == true)
            {
                GameObject player;
                bool loadedASheet = false;
                CharacterSheet playerCharacterSheet;
                if (!System.IO.File.Exists(Application.dataPath + "/" + viewerWaitingToJoin + ".pdc"))
                {
                    ///create a fresh character - pre-existing sheet not found for player
                    player = new GameObject(viewerWaitingToJoin);
                    twitchDungeonScript.PleaseAssignClassAndGender(viewerWaitingToJoin);
                    playerCharacterSheet = player.AddComponent<CharacterSheet>();
                }
               
                else {
                    ///still need to implement saving and loading characters as gameobjects somehow
                    player = new GameObject(viewerWaitingToJoin);
                    playerCharacterSheet = player.AddComponent<CharacterSheet>();
                    SheetSave loadedSheet = LoadCharacterSheet(viewerWaitingToJoin);
                    playerCharacterSheet.LoadSheet(loadedSheet);
                    loadedASheet = true;
                }
                player.transform.parent = partyGO.transform;
                party.Add(player);
                playerCharacterSheet.ownerUsername = viewerWaitingToJoin;
                if (ColorUtility.TryParseHtmlString(tempColor, out convertedColor)) {
                    playerCharacterSheet.usernameColor = convertedColor;
                }
                tempColor = null;
                
                playerCharacterSheet.brain = this;

                ///attach player to a new GUI panel
                foreach (GameObject panel in playerPanels) {
                    if (panel.GetComponent<PlayerPanelScript>().currentPlayerGO == null) {
                        panel.GetComponent<PlayerPanelScript>().currentPlayerGO = player;
                        playerCharacterSheet.attachedPanel = panel;
                        panel.GetComponent<PlayerPanelScript>().UpdateOwnerData();
                        break;
                    }
                }
                ///attach player to a new BattlePosition
                foreach (GameObject battlePos in playerBattlePositions) {
                    if (battlePos.GetComponent<BattlePositionScript>().ownerGO == null) {
                        battlePos.GetComponent<BattlePositionScript>().ownerGO = player;
                        battlePos.GetComponent<BattlePositionScript>().textChild.GetComponent<Text>().color = convertedColor;
                        battlePos.GetComponent<Animator>().SetBool("Revive", true);
                        battlePos.GetComponent<Animator>().SetBool("Dead", false);
                        playerCharacterSheet.attachedBattlePosition = battlePos;
                        break;
                    }
                }
                convertedColor = Color.black;

                ///attach player to ready Text on AlertBoxScreen and newDungeonPanel
                foreach (GameObject readyTextGO in readyUpNameGOs) {
                    if (readyTextGO.GetComponent<AlertBoxPlayerScript>().ownerGO == null) {
                        readyTextGO.GetComponent<AlertBoxPlayerScript>().ownerGO = player;
                        player.GetComponent<CharacterSheet>().attachedReadyText = readyTextGO;
                        break;
                    }
                }
                foreach (GameObject newDungeonText in newDungeonPanelNameGOs) {
                    if(newDungeonText.GetComponent<AlertBoxPlayerScript>().ownerGO == null){
                        newDungeonText.GetComponent<AlertBoxPlayerScript>().ownerGO = player;
                        player.GetComponent<CharacterSheet>().attachedNewDungeonNameGO = newDungeonText;
                        break;
                    }
                }
                foreach (GameObject completeDungeonText in dungeonCompletePanelNameGOs)
                {
                    if (completeDungeonText.GetComponent<AlertBoxPlayerScript>().ownerGO == null)
                    {
                        completeDungeonText.GetComponent<AlertBoxPlayerScript>().ownerGO = player;
                        player.GetComponent<CharacterSheet>().attachedCompleteDungeonNameGO = completeDungeonText;
                        break;
                    }
                }
                foreach (GameObject welcomeDungeonText in welcomePanelNameGOs)
                {
                    if (welcomeDungeonText.GetComponent<AlertBoxPlayerScript>().ownerGO == null)
                    {
                        welcomeDungeonText.GetComponent<AlertBoxPlayerScript>().ownerGO = player;
                        player.GetComponent<CharacterSheet>().attachedWelcomeDungeonNameGO = welcomeDungeonText;
                        break;
                    }
                }
                usersParticipatedInThisBattle.Add(player.name);
                if (loadedASheet) {
                    playerCharacterSheet.attachedBattlePosition.GetComponent<BattlePositionScript>().ChooseRightAnimatorController(playerCharacterSheet.gender, playerCharacterSheet.characterClass);
                    playerCharacterSheet.attachedBattlePosition.GetComponent<BattlePositionScript>().textChild.SetActive(true);
                    playerCharacterSheet.attachedBattlePosition.GetComponent<BattlePositionScript>().hpChild.SetActive(true);
                    playerCharacterSheet.attachedBattlePosition.GetComponent<BattlePositionScript>().ownerCharacterSheet = playerCharacterSheet;
                    playerCharacterSheet.attachedBattlePosition.GetComponent<BattlePositionScript>().textChild.GetComponent<Text>().text = playerCharacterSheet.ownerUsername;
                    playerCharacterSheet.attachedBattlePosition.GetComponent<BattlePositionScript>().spriteRenderer.enabled = true;
                }
            }

            else
            {
                twitchDungeonScript.NeverConfirmed(viewerWaitingToJoin);
            }

            viewersWaitingToJoin.Remove(viewerWaitingToJoin);
            waitingForPlayerToAccept = false;
            playerWaitingFor = "foobar";
            confirmed = false;
        }

        else
        {
            Debug.Log("Tried to Add Someone to the party, but the party count is " + party.Count);
        }
    }
}
