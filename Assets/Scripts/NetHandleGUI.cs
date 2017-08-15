using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;


public class NetHandleGUI : NetworkBehaviour {

    public GameObject selectTranslate;
    public GameObject selectRotate;
    public GameObject selectScale;
    public GameObject btnOk;
    public GameObject guiOk;
    public GameObject btnGroup;
    public GameObject btnUngroup;
    public GameObject guiGroupUngroup;
    public GameObject trackedObjects;

    public GameObject playerObject;
    int uId;

    DataSync DataSyncRef;
    GameObject handler;
    GameObject NetManager;

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

    
    public void buttonOk() { // if user click in the ok button
        if (btnOk.activeInHierarchy) {
            if (TestController.tcontrol.sceneIndex == 0 ) { //if in trainning
                
                playerObject.GetComponent<HandleUsersConnected>().AddUsersDone(uId); //add the user id to the done list
                guiOk.SetActive(false); // deactivate the ok button.
            } else
                playerObject.GetComponent<StackController>().SetNextPiece();
        } 
    }

    public void toggleGroup() {
        if (!btnGroup.activeInHierarchy) {
            //MainController.control.groupButtonActive = false;
            //btnGroup.SetActive(true);
            //btnUngroup.SetActive(false);
            playerObject.GetComponent<NetHandleGroup>().UnGroup();

        } else {
            //MainController.control.groupButtonActive = true;
            //btnGroup.SetActive(false);
            //btnUngroup.SetActive(true);
            playerObject.GetComponent<NetHandleGroup>().CreateGroup();

        }
    }


    IEnumerator Start() {
        yield return new WaitForSeconds(1f); //Delay start, wait for the players.
        handler = GameObject.Find("MainHandler");
        NetManager = GameObject.Find("NetworkManager");
        //if (handler == null) return;
        //if (NetManager == null) return;

        DataSyncRef = handler.GetComponent<DataSync>();
        uId = int.Parse(NetManager.GetComponent<MyNetworkManager>().userID); //get the user id
        
        if (playerObject.GetComponent<HandleUsersConnected>().FindUser(uId)) { //if the user connected and he is on the list, it is possible that he was connected and already have clicked on the ok in the past
            guiOk.SetActive(false);
            btnOk.SetActive(false);
        }
        else if (TestController.tcontrol.sceneIndex == 0) { //if it is the trainning scene
            guiOk.SetActive(true);
            btnOk.SetActive(true);
        }



    }

    private void Update() {
        if (TestController.tcontrol.sceneIndex != 0) { // if it is not the trainning scene
            if (playerObject.gameObject.GetComponent<Lean.Touch.NetHandleSelectionTouch>().objSelected.Count > 0) {
                guiOk.SetActive(true);
                btnOk.SetActive(true);
            } else {
                guiOk.SetActive(false);
                btnOk.SetActive(false);
            }
        }
        if (playerObject.gameObject.GetComponent<Lean.Touch.NetHandleSelectionTouch>().objSelected.Count > 1) {

            int groupSelected = -2;
            bool sigleGroup = true;
            foreach (var index in playerObject.gameObject.GetComponent<Lean.Touch.NetHandleSelectionTouch>().objSelected) {
                int group = DataSyncRef.Groups[index];
                if (group < 0) {
                    sigleGroup = false;
                    break;
                }
                if (groupSelected == -2) {
                    groupSelected = group;
                } else if (group != groupSelected) {
                    sigleGroup = false;
                    break;
                }
            }
            guiGroupUngroup.SetActive(true);

            if (sigleGroup) {
                for (int i = 0; i < trackedObjects.transform.childCount; i++) {
                    if (DataSyncRef.Groups[i] != groupSelected) continue;
                    bool selected = false;
                    foreach (var index in playerObject.gameObject.GetComponent<Lean.Touch.NetHandleSelectionTouch>().objSelected) {
                        if (index == i) {
                            selected = true;
                            break;
                        }
                    }
                    if (!selected) {
                        sigleGroup = false;
                        break;
                    }
                }
            }
            btnGroup.SetActive(!sigleGroup);

        } else {
            guiGroupUngroup.SetActive(false);
        }



    }
    //public void CloseInstructions() {
    //    GameObject.Find("Canvas").transform.GetChild(0).gameObject.SetActive(true);
    //    GameObject.Find("Canvas").transform.GetChild(2).gameObject.SetActive(true);
    //    GameObject.Find("Instructions").SetActive(false);

    //}
}
