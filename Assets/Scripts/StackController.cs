using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class StackController : NetworkBehaviour {

    GameObject trackedObjects;
    int[] objectsOrder;
    int objIndex;
    int halfObjects;
    Transform childMoving;
    Transform childStatic;

    Matrix4x4 movingObjMatrixTrans;
    Matrix4x4 movingObjMatrixRot;
    
    Matrix4x4 staticObjMatrixTrans;
    Matrix4x4 staticObjMatrixRot;
    

    void Start () {

        trackedObjects = GameObject.Find("TrackedObjects");
        halfObjects = trackedObjects.transform.childCount / 2; // The objs/2 values are the moving objects.
        objectsOrder = randomizeVector(halfObjects); // Randomize the blocks order. Store it in an array. The randomizer have to only choose even values

        foreach (Transform child in trackedObjects.transform) // Disable all objects.
            child.gameObject.SetActive(false);

        objIndex = 0;
       
    }
	
	void Update () {


        childMoving = trackedObjects.transform.GetChild(objectsOrder[objIndex]); // take the moving object 
        childStatic = trackedObjects.transform.GetChild(objectsOrder[objIndex] + halfObjects); // and its ghost
        //Debug.Log(objIndex + halfObjects);
        //Debug.Log(objectsOrder[objIndex] + " " + objectsOrder[objIndex] + halfObjects);

        childMoving.gameObject.SetActive(true);
        childStatic.gameObject.SetActive(true);



        movingObjMatrixTrans = Matrix4x4.TRS(childMoving.transform.position, Quaternion.identity, new Vector3(1.0f, 1.0f, 1.0f));
        movingObjMatrixRot = Matrix4x4.TRS(new Vector3(0, 0, 0), childMoving.transform.rotation, new Vector3(1.0f, 1.0f, 1.0f));

        staticObjMatrixTrans = Matrix4x4.TRS(childStatic.transform.position, Quaternion.identity, new Vector3(1.0f, 1.0f, 1.0f));
        staticObjMatrixRot = Matrix4x4.TRS(new Vector3(0, 0, 0), childStatic.transform.rotation, new Vector3(1.0f, 1.0f, 1.0f));

        //Debug.Log(Utils.distMatrices(movingObjMatrixTrans, staticObjMatrixTrans));
        //Debug.Log(Utils.distMatrices(movingObjMatrixRot, staticObjMatrixRot));

        if (Input.GetKeyDown("space")) {
            childMoving.gameObject.SetActive(false);
            childStatic.gameObject.SetActive(false);
            objIndex++;


            MainController.control.objSelected.Clear();
            foreach (var player in GameObject.FindGameObjectsWithTag("player")) 
                player.GetComponent<Lean.Touch.NetHandleSelectionTouch>().objSelectedShared.Clear();

            if (objIndex == trackedObjects.transform.childCount /2) {
                Debug.Log("AAAAA");
                SceneManager.LoadScene("SetupScene");

            }
        }


    }

    int[] randomizeVector(int size) {

        var randomNumbers = new int[size];
        var numbers = new List<int>(size);
       

        for (int i = 0; i < size; i++)
            numbers.Add(i);
        
        for (int i = 0; i < randomNumbers.Length; i++) {
            var thisNumber = Random.Range(0, numbers.Count);
            randomNumbers[i] = numbers[thisNumber];
            numbers.RemoveAt(thisNumber);
        }
        return randomNumbers;
    }

}
