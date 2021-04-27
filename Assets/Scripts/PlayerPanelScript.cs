using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class PlayerPanelScript : MonoBehaviour {

    public GameObject currentPlayerGO;
    public CharacterSheet ownerCharacterSheet;
    public GameObject[] slotOpenGOs;
    public GameObject[] pleaseAssignGOs;
    public GameObject[] playerDataGOs;
    public GameObject[] waitingGOs;
    public Text usernameTxt;
    public Text hpIntTxt;
    public Text mpIntTxt;
    public Text lvlIntTxt;
    public Text waitingIntTxt;
    public GameObject readyGO;
    public GameObject[] userNameGOs;
    public Text xpIntText;
    public GameObject attachedLevelUpBox;
    public Text goldIntTxt;
    public Text manapotionsIntText;
    public Text healingpotionsIntTxt;

    // Use this for initialization
    void Start () {
        currentPlayerGO = null;
    }
	
	// Update is called once per frame
	void Update ()
    {
        ///make sure the right panel elements are always displayed depending on the 
        ///situation (whether the slot is open or not, whether the player still needs to pick a class)
        if (currentPlayerGO == null)
        {
            ActiveSwitch(false, pleaseAssignGOs);
            ActiveSwitch(false, playerDataGOs);
            ActiveSwitch(false, userNameGOs);
            ActiveSwitch(true, slotOpenGOs);
            ActiveSwitch(false, waitingGOs);
            readyGO.SetActive(false);
        }

        else if (!ownerCharacterSheet.classIsAssigned)
        {
            ActiveSwitch(true, pleaseAssignGOs);
            ActiveSwitch(true, userNameGOs);
            ActiveSwitch(false, playerDataGOs);
            readyGO.SetActive(false);
            ActiveSwitch(false, slotOpenGOs);
        }
        else {
            ActiveSwitch(false, pleaseAssignGOs);
            ActiveSwitch(true, playerDataGOs);
            ActiveSwitch(true, userNameGOs);
            ActiveSwitch(false, slotOpenGOs);
            KeepDataUpdated();
        }

        
       
	}

    public void UpdateOwnerData() {
        ownerCharacterSheet = currentPlayerGO.GetComponent<CharacterSheet>();
        usernameTxt.color = ownerCharacterSheet.usernameColor;
        usernameTxt.text = ownerCharacterSheet.ownerUsername;
    }

    public void ActiveSwitch(bool active, GameObject[] gameObjects) {
        foreach (GameObject gameObject in gameObjects) {
            gameObject.SetActive(active);
        }
    }

    public void KeepDataUpdated() {
        if (ownerCharacterSheet.waitingForAction)
        {
            readyGO.SetActive(false);
            ActiveSwitch(true, waitingGOs);
        }
        else
        {
            readyGO.SetActive(true);
            ActiveSwitch(false, waitingGOs);
        }
        usernameTxt.text = ownerCharacterSheet.ownerUsername;
        usernameTxt.color = ownerCharacterSheet.usernameColor;
        int currHP = ownerCharacterSheet.currentHP;
        if (currHP < 0)
        {
            currHP = 0;
        }
        hpIntTxt.text = currHP + "/" + ownerCharacterSheet.maxHP;
        xpIntText.text = ownerCharacterSheet.totalXP.ToString();
        mpIntTxt.text = ownerCharacterSheet.currentMP + "/" + ownerCharacterSheet.maxMP;
        lvlIntTxt.text = ownerCharacterSheet.characterLevel.ToString();
        waitingIntTxt.text = ownerCharacterSheet.waitingForActionTimeLeft.ToString();
        goldIntTxt.text = ownerCharacterSheet.myGold.ToString();
        manapotionsIntText.text = ownerCharacterSheet.manaPotionsInventory.Count.ToString();
        healingpotionsIntTxt.text = ownerCharacterSheet.healingPotionsInventory.Count.ToString();
        
    }
}
