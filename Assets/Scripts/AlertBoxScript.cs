using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class AlertBoxScript : MonoBehaviour {
    public Text alertline1;
    public Text alertline2;
    public Text alertline3;
    public Text alertline4;
    public Text alertline5;
    public Text alertline6;
    public Text alertline7;
    public List<Text> alertLines;



    // Use this for initialization
    void Start () {
        foreach (Text line in alertLines) {
            line.text = "";
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void NewAlert(string alert, Color color) {
        if (alert.Length > 34)
        {
            string[] choppedAlert = SafeWordChop(alert, 34);
            foreach (string chop in choppedAlert)
            {
                UpdateLine(chop, color);
            }
        }
        else {
            UpdateLine(alert, color);
        }
    }

    public void UpdateLine(string inputString, Color color) {
        alertline7.text = alertline6.text;
        alertline7.color = alertline6.color;
        alertline6.text = alertline5.text;
        alertline6.color = alertline5.color;
        alertline5.text = alertline4.text;
        alertline5.color = alertline4.color;
        alertline4.text = alertline3.text;
        alertline4.color = alertline3.color;
        alertline3.text = alertline2.text;
        alertline3.color = alertline2.color;
        alertline2.text = alertline1.text;
        alertline2.color = alertline1.color;
        alertline1.text = inputString;
        alertline1.color = color;
    }
    
    public static string[] SafeWordChop(string sentence, int maximimumStringLength) {
        string[] sentenceSplit = sentence.Split(' ');
        int indexMarker = 0;
        List<string> strList = new List<string>();
        while (indexMarker <= sentenceSplit.Length - 1)
        {
            string outString = "";
            while (!((outString + sentenceSplit[indexMarker]).Length > maximimumStringLength))
            {
                outString += " " + sentenceSplit[indexMarker];
                indexMarker += 1;
                if (indexMarker > sentenceSplit.Length - 1) {
                    break;
                }
            }
            strList.Add(outString);
        }
        return strList.ToArray();
    }
    
}
