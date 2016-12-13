using UnityEngine;
using System.Collections;

public class HandleTransformations : MonoBehaviour {


    public GameObject trackedObjetecs;
    public GameObject selectedObjects;
    
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (MainController.control.lockTransform) {
            foreach(GameObject g in MainController.control.objSelectedNow) {
                g.transform.parent = selectedObjects.transform;
            }
        } 
        else {
            foreach (GameObject g in MainController.control.objSelectedNow) {
                g.transform.parent = trackedObjetecs.transform;
            }
        }

    }
}
