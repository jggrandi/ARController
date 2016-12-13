using UnityEngine;
using System.Collections;

public class HandleSelectionTouch : MonoBehaviour {

    public GameObject selectedObject;
    public GameObject trackedObjects;

    Ray ray;
    RaycastHit hit;
    GameObject objToRemove;

    // Use this for initialization
    void Start () {
	}

    // Update is called once per frame
    void Update() {

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {
            ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            Debug.DrawRay(ray.origin, ray.direction * 20, Color.red);

            bool objIsSelected = false;
            
            if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
                foreach (GameObject g in MainController.control.objSelectedNow) {
                    if (g.name == hit.transform.gameObject.name) {
                        objIsSelected = true;
                        objToRemove = g;
                        break;
                    }
                }

                if (objIsSelected) {
                    MainController.control.objSelectedNow.Remove(objToRemove);
                    hit.transform.GetComponent<Renderer>().material.color = Color.white;

                } else {
                    hit.transform.GetComponent<Renderer>().material.color = Color.yellow;
                    MainController.control.objSelectedNow.Add(hit.transform.gameObject);
                }
            }
        }
    }

}
