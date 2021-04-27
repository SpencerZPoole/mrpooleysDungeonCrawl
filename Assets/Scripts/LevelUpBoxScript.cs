using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class LevelUpBoxScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public IEnumerator SelfDeactivate(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        this.gameObject.SetActive(false);
    }
}
