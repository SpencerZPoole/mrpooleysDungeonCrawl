using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class DungeonRunInstance : MonoBehaviour {
    public int roomCount;
    public int currentRoomNumber;
    public List<string> enemiesToUse;
    public Brain1 brain;
    public TwitchChatterDungeon tChatterDungeon;
    public bool currentlyInBattle = false;
    public bool partyIsReady = false;
    public int xpJustGained = 0;
    public int goldJustGained = 0;
    public BattleInstance currentBattle;
    public int roomsPassed = 0;
    public bool wiped = false;

    // Use this for initialization
    void Start () {
        currentRoomNumber = 0;
        roomsPassed = 0;
        brain.roomCountText.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
        brain.roomCountText.text = "Room " + currentRoomNumber + "/" + roomCount;
        if (brain.party.Count == 0) {
            wiped = true;
            brain.WipeMethod();
        }
        else if (currentRoomNumber < roomCount && !currentlyInBattle){
            List<GameObject> currentParty = brain.party;
            if (currentRoomNumber <= 0){
                brain.newDungeonPanel.SetActive(true);
                brain.newDungeonPanel.GetComponent<NewDungeonPanelScript>().roomLengthText.text = "Dungeon length: " + roomCount + " rooms";
            }
            else{
                brain.readyUpPanel.SetActive(true);
                brain.readyUpPanel.GetComponent<UpdateBoxScript>().roomCountText.text = "Room " + currentRoomNumber + "/" + roomCount;
                brain.readyUpPanel.GetComponent<UpdateBoxScript>().xpGainText.text = "The party gains " + xpJustGained + " xp and " + goldJustGained + " gold!";
            }
            if (brain.ReadyCheck(currentParty)){
                currentRoomNumber += 1;
                int numberOfEnemies = 4;
                if (currentParty.Count == 1){
                        numberOfEnemies = Random.Range(1, 3);  
                }
                else if (currentParty.Count == 2){
                    numberOfEnemies = Random.Range(2, 4);
                }
                else if (currentParty.Count == 3 || currentParty.Count == 4){
                    numberOfEnemies = Random.Range(3, 5);
                }
                StartBattle(enemiesToUse, numberOfEnemies);
            }
        }
        else if (currentRoomNumber == roomCount && !currentlyInBattle) {
            List<GameObject> currentParty = brain.party;
            brain.dungeonCompletePanel.SetActive(true);
            brain.dungeonCompletePanel.GetComponent<NewDungeonPanelScript>().roomLengthText.text = "Rooms completed: " + roomsPassed;
            brain.dungeonCompletePanel.GetComponent<NewDungeonPanelScript>().partyGainsText.text = "The party gains " + xpJustGained + " xp and " + goldJustGained + " gold!";
            if (brain.ReadyCheck(currentParty)) {
                ManualDestroy();
            }
        }
	}
    public void ManualDestroy(){
        if (brain.party.Count > 0){
            foreach (GameObject player in brain.party){
                player.GetComponent<CharacterSheet>().readyForNextBattle = false;
            }
        }
        brain.dungeonExistsRightNow = false;
        brain.dungeonCompletePanel.SetActive(false);
        brain.roomCountText.enabled = false;
        if (!wiped){
            brain.MakeNewDungeon(brain.tempEnemiesStringList);
        }
        Destroy(this);
    }
    public void StartBattle(List<string> enemyList, int numberOfEnemies){
        currentlyInBattle = true;
        brain.inBattle = true;
        GameObject battleGO = new GameObject("Battle");
        currentBattle = battleGO.AddComponent<BattleInstance>();
        currentBattle.dungeonParent = this;
        currentBattle.dungeonChatter = tChatterDungeon;
        currentBattle.InitializeBattleInstance(enemyList, numberOfEnemies, brain);

    }
}
