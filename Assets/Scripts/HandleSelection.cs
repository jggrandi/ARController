using UnityEngine;
using System.Collections;

public class HandleSelection : MonoBehaviour {

    public GameObject selectedObject;
    public GameObject trackedObjects;

    Ray ray;
    RaycastHit hit;
    GameObject selected;
    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        
        ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        Debug.DrawRay(ray.origin, ray.direction * 20, Color.red);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
			Debug.Log(Input.GetMouseButton(0));
            selected = hit.transform.gameObject ;
			if(Input.touchCount <= 0 || Input.GetMouseButton(0) == false)
                selected.transform.GetComponent<Renderer>().material.color = Color.yellow;
			if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) ||Input.GetMouseButton(0) == true) {
                selected.transform.gameObject.transform.parent = selectedObject.transform;
                selected.transform.GetComponent<Renderer>().material.color = Color.red;
            }
			if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) || Input.GetMouseButton(0) == false) {
                selected.transform.gameObject.transform.parent = trackedObjects.transform;
            }

        }
        else
            selected.transform.GetComponent<Renderer>().material.color = Color.white;
    }
}
