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
            //MainController.control.groupButtonActive = false;
            //btnGroup.SetActive(true);
            //btnUngroup.SetActive(false);
            playerObject.GetComponent<StackController>().SetNextPiece();

        } else {
            //MainController.control.groupButtonActive = true;
            //btnGroup.SetActive(false);
            //btnUngroup.SetActive(true);
            playerObject.GetComponent<NetHandleGroup>().CreateGroup();
            
        }
    }

    DataSync DataSyncRef;
    public void Start() {
        DataSyncRef = GameObject.Find("MainHandler").GetComponent<DataSync>();
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
            //        int groupSelected = -2;
            //        bool sigleGroup = true;
            //        foreach (var index in MainController.control.objSelected) {
            //            int group = DataSyncRef.Groups[index];
            //            if(group < 0) {
            //                sigleGroup = false;
            //                break;
            //            }
            //            if (groupSelected == -2) {
            //                groupSelected = group;
            //            }else if(group != groupSelected) {
            //                sigleGroup = false;
            //                break;
            //            }
            //        }
            //        guiGroupUngroup.SetActive(true);

            //        if (sigleGroup) {
            //            for (int i = 0; i < trackedObjects.transform.childCount; i++) {
            //                for (int j = 0; i < trackedObjects.transform.GetChild(i).childCount; j++) {
            //                    if (DataSyncRef.Groups[j] != groupSelected) continue;
            //                    bool selected = false;
            //                    foreach (var index in MainController.control.objSelected) {
            //                        if (index == j) {
            //                            selected = true;
            //                            break;
            //                        }
            //                    }
            //                    if (!selected) {
            //                        sigleGroup = false;
            //                        break;
            //                    }
            //                }
            //            }
            //        }
            //        btnGroup.SetActive(!sigleGroup);

            //    } else {
            //        guiGroupUngroup.SetActive(false);
            //    }

       
    }


}
