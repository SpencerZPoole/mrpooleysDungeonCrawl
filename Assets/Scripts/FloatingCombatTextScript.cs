using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class FloatingCombatTextScript : MonoBehaviour {
    public GameObject playerBattlePosition1;
    public GameObject playerBattlePosition2;
    public GameObject playerBattlePosition3;
    public GameObject playerBattlePosition4;
    public GameObject enemyBattlePosition1;
    public GameObject enemyBattlePosition2;
    public GameObject enemyBattlePosition3;
    public GameObject enemyBattlePosition4;
    public Text textPrefab;
    public GameObject testCharacter;
    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SpawnFloatingCombatText(string txt, GameObject character, Color color) {
        Text newText = Instantiate(textPrefab, character.GetComponent<CharacterSheet>().attachedBattlePosition.transform) as Text;
        newText.transform.SetParent(character.GetComponent<CharacterSheet>().attachedBattlePosition.transform, false);
        newText.GetComponent<Text>().fontSize = 4;
        newText.GetComponent<Text>().color = color;
        if (character.GetComponent<CharacterSheet>().race == "orc") {
            newText.transform.localScale = new Vector3(newText.transform.localScale.x *-1, newText.transform.localScale.y, newText.transform.localScale.z);
        }
        newText.GetComponent<Text>().text = txt;

        StartCoroutine(LerpText(newText));
    }

    public IEnumerator LerpText(Text textGO) {
        int numerator = 0;
        while (numerator < 60) {
            textGO.transform.position = new Vector3(textGO.transform.position.x, textGO.transform.position.y + 1, textGO.transform.position.z);
            numerator += 1;
            yield return new WaitForSeconds(0.05f);
        }
        GameObject.Destroy(textGO);
    }
}
