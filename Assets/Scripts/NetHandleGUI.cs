using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetHandleGUI : NetworkBehaviour {

    public GameObject selectTranslate;
    public GameObject selectRotate;
    public GameObject selectScale;
    public GameObject btnGroup;
    public GameObject btnUngroup;
    public GameObject guiGroupUngroup;
    public GameObject trackedObjects;

    public GameObject playerObject;

   
    public void buttonLock () {
        MainController.control.lockTransform = true;
    }

    public void buttonUnlock() {
        MainController.control.lockTransform = false;
    }


    public void buttonTranslate() {
        selectTranslate.SetActive(true);
        selectRotate.SetActive(false);
        selectScale.SetActive(false);
        MainController.control.transformationNow = Utils.Transformations.Translation;
    }
    public void buttonRotate() {
        selectTranslate.SetActive(false);
        selectRotate.SetActive(true);
        selectScale.SetActive(false);
        MainController.control.transformationNow = Utils.Transformations.Rotation;

    }
    public void buttonScale() {
        selectTranslate.SetActive(false);
        selectRotate.SetActive(false);
        selectScale.SetActive(true);
        MainController.control.transformationNow = Utils.Transformations.Scale;

    }

    public void toggleGroup() {
        if (btnGroup.activeInHierarchy) {
            playerObject.GetComponent<StackController>().SetNextPiece();
        } 
    }

    //DataSync DataSyncRef;
    public void Start() {
        //DataSyncRef = GameObject.Find("MainHandler").GetComponent<DataSync>();
        if (TestController.tcontrol.taskOrder[TestController.tcontrol.sceneIndex] == 0) {
            GameObject.Find("lock").gameObject.SetActive(false);
        }
    }

    private void Update() {
        if (MainController.control.objSelected.Count > 0) {
            guiGroupUngroup.SetActive(true);
            btnGroup.SetActive(true);
        } else {
            guiGroupUngroup.SetActive(false);
            btnGroup.SetActive(false);
        }
    }
    public void CloseInstructions() {
        GameObject.Find("Canvas").transform.GetChild(0).gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.GetChild(2).gameObject.SetActive(true);
        GameObject.Find("Instructions").SetActive(false);

    }
}
