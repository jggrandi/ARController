﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class HandleGUI : NetworkBehaviour {

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
        
        if (!btnGroup.activeInHierarchy) {
            //MainController.control.groupButtonActive = false;
            //btnGroup.SetActive(true);
            //btnUngroup.SetActive(false);
            gameObject.GetComponent<NetHandleGroup>().UnGroup();
            
        } else {
            //MainController.control.groupButtonActive = true;
            //btnGroup.SetActive(false);
            //btnUngroup.SetActive(true);
            gameObject.GetComponent<NetHandleGroup>().CreateGroup();
            
            
        }
    }

    private void Update() {
        if (MainController.control.objSelectedNow.Count > 1 ) {

            int groupSelected = -2;
            bool sigleGroup = true;
            foreach (GameObject g in MainController.control.objSelectedNow) {
                int group = g.transform.gameObject.GetComponent<ObjectGroupId>().id;
                if(group < 0) {
                    sigleGroup = false;
                    break;
                }
                if (groupSelected == -2) {
                    groupSelected = group;
                }else if(group != groupSelected) {
                    sigleGroup = false;
                    break;
                }
            }
            guiGroupUngroup.SetActive(true);
            btnGroup.SetActive(!sigleGroup);

        } else {
            guiGroupUngroup.SetActive(false);
        }

    }


}
