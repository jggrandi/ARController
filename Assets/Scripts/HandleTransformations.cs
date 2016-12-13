using UnityEngine;
using System.Collections;

public class HandleTransformations : MonoBehaviour {


    public GameObject trackedObjetecs;
    public GameObject lockedObjects;

    private float rotSpeed = 1.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (MainController.control.lockTransform) {
            foreach(GameObject g in MainController.control.objSelectedNow) {
                g.transform.parent = lockedObjects.transform;
            }
        } 
        else {
            foreach (GameObject g in MainController.control.objSelectedNow) {
                g.transform.parent = trackedObjetecs.transform;
            }
        }

        switch (MainController.control.transformationNow) {
            case (int)Utils.Transformations.Translation:
                handleTranslation();
                break;
            case (int)Utils.Transformations.Rotation:
                handleRotation();
                break;
            case (int)Utils.Transformations.Scale:
                handleScale();
                break;
            default:
                break;
        }

    }

    void handleTranslation() {

        Vector3 dir = new Vector3();

        dir = Camera.main.transform.forward;

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) {
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;

            foreach (GameObject g in MainController.control.objSelectedNow) {
                dir.y = g.transform.position.y;

                g.transform.Translate(dir * rotSpeed * Time.deltaTime);// );
            }
        }
    }


    void handleRotation() {
        
        //if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) {
        //    float rotX = Input.GetTouch(0). * rotSpeed * Mathf.Deg2Rad;
        //    float rotY = Input.GetAxis("Vertical") * rotSpeed * Mathf.Deg2Rad;
        //    Debug.Log("AAA");
        //    foreach (GameObject g in MainController.control.objSelectedNow) {
        //        g.transform.Rotate(Vector3.up, -rotX);
        //        g.transform.Rotate(Vector3.right, rotY);
        //    }
        //}
    }

    void handleScale() {
        
    }

}
