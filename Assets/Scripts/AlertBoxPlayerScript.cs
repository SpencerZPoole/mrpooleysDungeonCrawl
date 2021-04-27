using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class AlertBoxPlayerScript : MonoBehaviour {
    public GameObject ownerGO = null;
    public Text attachedReadyText;
    public Text thisText;
	// Use this for initialization
	void Start () {
        thisText = this.gameObject.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        if (ownerGO == null)
        {
            thisText.text = "";
            attachedReadyText.text = "";
        }
        else {
            thisText.text = ownerGO.name;
            thisText.color = ownerGO.GetComponent<CharacterSheet>().usernameColor;
            if (!ownerGO.GetComponent<CharacterSheet>().classIsAssigned) {
                attachedReadyText.text = "Still need to assign gender and class!";
                attachedReadyText.color = Color.red;
            }
            else if (ownerGO.GetComponent<CharacterSheet>().readyForNextBattle && ownerGO.GetComponent<CharacterSheet>().classIsAssigned)
            {
                attachedReadyText.text = "Ready!";
                attachedReadyText.color = Color.green;
            }
            
            else {
                attachedReadyText.text = "Not Ready!";
                attachedReadyText.color = Color.red;
            }
        }
	}
}
