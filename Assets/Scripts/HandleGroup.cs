using UnityEngine;
using System.Collections;

public class HandleGroup : MonoBehaviour {
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
        //if(MainController.control.isToGroup) {
        //    Debug.Log(MainController.control.isToGroup);
        //    CreateGroup();
            
        //}
	}


    public void CreateGroup() {
        
        //foreach (GameObject g in MainController.control.objSelectedNow) {
        //    MyGameObject addToGroup = new MyGameObject();
        //    addToGroup.gId = MainController.control.groupedObjectsIdNow;
        //    addToGroup.gGameObject = g;
        //    MainController.control.groupedObjects.Add(addToGroup);
        //}
        //MainController.control.groupedObjectsIdNow++;

        //foreach (MyGameObject mg in MainController.control.groupedObjects) {
        //    Debug.Log(mg.gGameObject.name);
        //}
    }


    public void UnGroup() {
        Debug.Log("Ungroup");
    }
}
