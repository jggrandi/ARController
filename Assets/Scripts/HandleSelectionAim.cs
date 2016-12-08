using UnityEngine;
using System.Collections;

public class HandleSelectionAim : MonoBehaviour {

    public GameObject selectedObject;
    public GameObject trackedObjects;

    Ray ray;
    RaycastHit hit;
    public GameObject selectedNow;
    public GameObject overNow;
    
    // Use this for initialization
    void Start () {
        selectedNow = new GameObject();
	}
	
	// Update is called once per frame
	void Update () {
        
        ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        Debug.DrawRay(ray.origin, ray.direction * 20, Color.red);
       
        if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
            //Debug.Log(Input.GetMouseButton(0));
            overNow = hit.transform.gameObject;
            if (Input.touchCount <= 0 || Input.GetMouseButton(0) == false)
                overNow.transform.GetComponent<Renderer>().material.color = Color.yellow;
            if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetMouseButton(0) == true) {
                selectedNow = overNow;
                selectedNow.transform.gameObject.transform.parent = selectedObject.transform;
                selectedNow.transform.GetComponent<Renderer>().material.color = Color.red;
            }
            if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) || Input.GetMouseButton(0) == false) {
                selectedNow.transform.gameObject.transform.parent = trackedObjects.transform;
                selectedNow = null;
            }

        } else {
            //overNow.transform.GetComponent<Renderer>().material.color = Color.white;
            overNow = null;
            //selectedNow.transform.gameObject.transform.parent = trackedObjects.transform;
        }
    }
}
