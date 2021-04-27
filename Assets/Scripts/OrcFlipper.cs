using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OrcFlipper : MonoBehaviour {
    public Animator anim;
    public GameObject selfGO;
    public GameObject childText;
    // Use this for initialization
    void Start () {
        FlipCheck();
	}
	
	// Update is called once per frame
	void Update () {
        FlipCheck();
    }
    public void FlipCheck() {
        float scale = selfGO.transform.localScale.x;
        float textScale = childText.transform.localScale.x;
        if (anim.runtimeAnimatorController.name == "OrcAnimator" || anim.runtimeAnimatorController.name == "OrcRogueAnimator")
        {
            if (scale > 0)
            {
                scale = scale * -1;
                selfGO.transform.localScale = new Vector3(scale, selfGO.transform.localScale.y, selfGO.transform.localScale.z);
            }
            if (textScale > 0)
            {
                textScale = textScale * -1;
                childText.transform.localScale = new Vector3(textScale, childText.transform.localScale.y, childText.transform.localScale.z);
            }
        }
        else {
            if (scale < 0) {
                scale = scale * -1;
                selfGO.transform.localScale = new Vector3(scale, selfGO.transform.localScale.y, selfGO.transform.localScale.z);
            }
            if (textScale < 0)
            {
                textScale = textScale * -1;
                childText.transform.localScale = new Vector3(textScale, childText.transform.localScale.y, childText.transform.localScale.z);
            }
        }
    }
}
