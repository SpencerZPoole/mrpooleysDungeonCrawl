using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class EnemyPanelScript : MonoBehaviour {
    public GameObject currentEnemyGO;
    public CharacterSheet ownerCharacterSheet;
    public Text usernameTxt;
    public Text hpIntTxt;
    public Text mpIntTxt;
    public Text lvlIntTxt;
    public GameObject[] userNameGOs;
    public GameObject[] noEnemyGOs;
    public GameObject[] enemyDataGOs;
    public GameObject panel;

    // Use this for initialization
    void Start () {
        currentEnemyGO = null;
	}
	
	// Update is called once per frame
	void Update () {

        ///make sure the right panel elements are always displayed depending on the 
        ///situation (whether the slot is open or not, whether the player still needs to pick a class)
        if (currentEnemyGO == null)
        {
            ActiveSwitch(false, noEnemyGOs);
            ActiveSwitch(false, enemyDataGOs);
            ActiveSwitch(false, userNameGOs);
            panel.GetComponent<Image>().enabled = false;
        }

        else
        {
            ActiveSwitch(false, noEnemyGOs);
            ActiveSwitch(true, enemyDataGOs);
            ActiveSwitch(true, userNameGOs);
            panel.GetComponent<Image>().enabled = true;
            KeepDataUpdated();
        }

    }

    public void UpdateOwnerData()
    {
        ownerCharacterSheet = currentEnemyGO.GetComponent<CharacterSheet>();
        usernameTxt.text = ownerCharacterSheet.ownerUsername;
    }

    ///method for activating/deactive a group/array of GameObjects
    public void ActiveSwitch(bool active, GameObject[] gameObjects)
    {
        foreach (GameObject gameObject in gameObjects)
        {
            gameObject.SetActive(active);
        }
    }

    public void KeepDataUpdated()
    {
        usernameTxt.text = ownerCharacterSheet.ownerUsername;
        int currHP = ownerCharacterSheet.currentHP;
        if (currHP < 0) {
            currHP = 0;
        }
        hpIntTxt.text = currHP + "/" + ownerCharacterSheet.maxHP;
        mpIntTxt.text = ownerCharacterSheet.currentMP + "/" + ownerCharacterSheet.maxMP;
        lvlIntTxt.text = ownerCharacterSheet.characterLevel.ToString();
    }
}
