using UnityEngine;
using System.Collections;

public class HandleGroup : MonoBehaviour {
    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

        //if(MainController.control.isToGroup) {
        //    Debug.Log(MainController.control.isToGroup);
        //    CreateGroup();

        //}
    }

    /*
    public void CreateGroup() {
        foreach (GameObject g in MainController.control.objSelectedNow) {
            g.GetComponent<ObjectGroupId>().id = MainController.control.idAvaiableNow;
        }

        MainController.control.idAvaiableNow++;
        MainController.control.isMultipleSelection = false;
    }


    public void UnGroup() {

        foreach (GameObject g in MainController.control.objSelectedNow) {
            g.GetComponent<ObjectGroupId>().id = -1;
            g.GetComponent<Renderer>().material.color = Color.white;
        }

        MainController.control.objSelectedNow.Clear();
        MainController.control.isMultipleSelection = false;
    }*/
}
