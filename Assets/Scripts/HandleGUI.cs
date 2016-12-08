using UnityEngine;
using System.Collections;

public class HandleGUI : MonoBehaviour {

    public GameObject selectTranslate;
    public GameObject selectRotate;
    public GameObject selectScale;
    public GameObject btnGroup;
    public GameObject btnUngroup;

    // Use this for initialization
    public void buttonLock () {
        Debug.Log("AAA");	
	}
    public void buttonTranslate() {
        selectTranslate.SetActive(true);
        selectRotate.SetActive(false);
        selectScale.SetActive(false);
    }
    public void buttonRotate() {
        selectTranslate.SetActive(false);
        selectRotate.SetActive(true);
        selectScale.SetActive(false);

    }
    public void buttonScale() {
        selectTranslate.SetActive(false);
        selectRotate.SetActive(false);
        selectScale.SetActive(true);

    }

    public void buttonGroup() {
        Debug.Log("eee");
    }


}
