using UnityEngine;
using System.Collections;

public class HandleGUI : MonoBehaviour {

    public GameObject selectTranslate;
    public GameObject selectRotate;
    public GameObject selectScale;
    public GameObject btnGroup;
    public GameObject btnUngroup;
    public GameObject guiGroupUngroup;

    // Use this for initialization
    public void buttonLock () {
        MainController.control.lockTransform = true;
    }

    public void buttonUnlock() {
        MainController.control.lockTransform = false;
        Debug.Log("Aqui!");
    }


    public void buttonTranslate() {
        selectTranslate.SetActive(true);
        selectRotate.SetActive(false);
        selectScale.SetActive(false);
        MainController.control.transformationNow = (int)Utils.Transformations.Translation;
    }
    public void buttonRotate() {
        selectTranslate.SetActive(false);
        selectRotate.SetActive(true);
        selectScale.SetActive(false);
        MainController.control.transformationNow = (int)Utils.Transformations.Rotation;

    }
    public void buttonScale() {
        selectTranslate.SetActive(false);
        selectRotate.SetActive(false);
        selectScale.SetActive(true);
        MainController.control.transformationNow = (int)Utils.Transformations.Scale;

    }

    public void toggleGroup() {
        if (MainController.control.groupButtonActive) {
            MainController.control.groupButtonActive = false;
            btnGroup.SetActive(true);
            btnUngroup.SetActive(false);
            gameObject.GetComponent<HandleGroup>().UnGroup();

        } else {
            MainController.control.groupButtonActive = true;
            btnGroup.SetActive(false);
            btnUngroup.SetActive(true);
            gameObject.GetComponent<HandleGroup>().CreateGroup();
            
            
        }
    }

    private void Update() {
       if(MainController.control.objSelectedNow.Count > 1) {
            guiGroupUngroup.SetActive(true);
        } else {
            guiGroupUngroup.SetActive(false);
        }

    }


}
