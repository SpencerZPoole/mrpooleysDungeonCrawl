using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class BattlePositionScript : MonoBehaviour {

    public GameObject ownerGO = null;
    public GameObject textChild;
    public GameObject hpChild;
    public Text hpChildText;
    public SpriteRenderer spriteRenderer;
    public RuntimeAnimatorController[] animatorControllers;
    public Animator myAnimator;
    public CharacterSheet ownerCharacterSheet;
    public GameObject bloodSprite;
    public GameObject slowSprite;


	// Use this for initialization
	void Start () {
        hpChildText = hpChild.GetComponent<Text>();
        textChild.SetActive(false);
        hpChild.SetActive(false);
        bloodSprite.SetActive(false);
        slowSprite.SetActive(false);
        spriteRenderer.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (hpChild.activeSelf) {
            int currHP = ownerCharacterSheet.currentHP;
            if (currHP < 0) {
                currHP = 0;
            }
            hpChildText.text = "HP: " + currHP + "/" + ownerCharacterSheet.maxHP;
        }
        if (ownerCharacterSheet != null){
            if (ownerCharacterSheet.bleeding){
                bloodSprite.SetActive(true);
            }
            else{
                bloodSprite.SetActive(false);
            }
            if (ownerCharacterSheet.slowed){
                slowSprite.SetActive(true);
            }
            else{
                slowSprite.SetActive(false);
            }
        }
        else {
            slowSprite.SetActive(false);
            bloodSprite.SetActive(false);
        }
    }

    public void ChooseRightAnimatorController(string gender, string characterClass) {
        if (characterClass == "warrior") {
            if (gender == "male")
            {
                foreach (RuntimeAnimatorController animatorController in animatorControllers) {
                    if (animatorController.name == "MaleWarriorAnimator") {
                        myAnimator.runtimeAnimatorController = animatorController;
                        break;
                    }
                }
            }
            else {
                foreach (RuntimeAnimatorController animatorController in animatorControllers)
                {
                    if (animatorController.name == "FemaleWarriorAnimator")
                    {
                        myAnimator.runtimeAnimatorController = animatorController;
                        break;
                    }
                }
            }
        }
        else if (characterClass == "rogue")
        {
            if (gender == "male")
            {
                foreach (RuntimeAnimatorController animatorController in animatorControllers)
                {
                    if (animatorController.name == "MaleRogueAnimator")
                    {
                        myAnimator.runtimeAnimatorController = animatorController;
                        break;
                    }
                }
            }
            else
            {
                foreach (RuntimeAnimatorController animatorController in animatorControllers)
                {
                    if (animatorController.name == "FemaleRogueAnimator")
                    {
                        myAnimator.runtimeAnimatorController = animatorController;
                        break;
                    }
                }
            }
        }
        else if (characterClass == "wizard")
        {
            if (gender == "male")
            {
                foreach (RuntimeAnimatorController animatorController in animatorControllers)
                {
                    if (animatorController.name == "MaleWizardAnimator")
                    {
                        myAnimator.runtimeAnimatorController = animatorController;
                        break;
                    }
                }
            }
            else
            {
                foreach (RuntimeAnimatorController animatorController in animatorControllers)
                {
                    if (animatorController.name == "FemaleWizardAnimator")
                    {
                        myAnimator.runtimeAnimatorController = animatorController;
                        break;
                    }
                }
            }
        }
        else if (characterClass == "cleric")
        {
            if (gender == "male")
            {
                foreach (RuntimeAnimatorController animatorController in animatorControllers)
                {
                    if (animatorController.name == "MaleClericAnimator")
                    {
                        myAnimator.runtimeAnimatorController = animatorController;
                        break;
                    }
                }
            }
            else
            {
                foreach (RuntimeAnimatorController animatorController in animatorControllers)
                {
                    if (animatorController.name == "FemaleClericAnimator")
                    {
                        myAnimator.runtimeAnimatorController = animatorController;
                        break;
                    }
                }
            }
        }
        else if (characterClass == "ranger")
        {
            if (gender == "male")
            {
                foreach (RuntimeAnimatorController animatorController in animatorControllers)
                {
                    if (animatorController.name == "MaleRangerAnimator")
                    {
                        myAnimator.runtimeAnimatorController = animatorController;
                        break;
                    }
                }
            }
            else
            {
                foreach (RuntimeAnimatorController animatorController in animatorControllers)
                {
                    if (animatorController.name == "FemaleRangerAnimator")
                    {
                        myAnimator.runtimeAnimatorController = animatorController;
                        break;
                    }
                }
            }
        }
    }

    public void ChooseRightAnimatorControllerEnemy(string enemyName)
    {
        if (enemyName.Contains("Orc Rogue"))
        {

            foreach (RuntimeAnimatorController animatorController in animatorControllers)
            {
                if (animatorController.name == "OrcRogueAnimator")
                {
                    myAnimator.runtimeAnimatorController = animatorController;
                    break;
                }
            }

        }
        else if (enemyName.Contains("Orc"))
        {

            foreach (RuntimeAnimatorController animatorController in animatorControllers)
            {
                if (animatorController.name == "OrcAnimator")
                {
                    myAnimator.runtimeAnimatorController = animatorController;
                    break;
                }
            }

        }
        else if (enemyName.Contains("Minotaur"))
        {

            foreach (RuntimeAnimatorController animatorController in animatorControllers)
            {
                if (animatorController.name == "MinotaurAnimator")
                {
                    myAnimator.runtimeAnimatorController = animatorController;
                    break;
                }
            }

        }

    }
    }
