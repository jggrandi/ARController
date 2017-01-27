using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MainController : MonoBehaviour {

    public static MainController control;

    public GameObject trackedObjects;

    public int targetsTrackedNow = 0;
    public bool isMultipleSelection = false;

    //public int transformationNow = 0;
    public Utils.Transformations transformationNow = 0;
    public bool lockTransform = false;
    public bool groupButtonActive = false;
    public List<int> objSelected = new List<int>();

    public int idAvaiableNow = 0;

    public bool isTapForTransform = false;

    void Awake() {

        control = this;
        //if (control == null) {
        //    DontDestroyOnLoad(gameObject);
        //    control = this;

        //} else if (control != this) {
        //    Destroy(gameObject);
        //}


        for (int i = 0; i < trackedObjects.transform.childCount; i++) {
            GameObject obj = trackedObjects.transform.GetChild(i).transform.gameObject;
            obj.AddComponent<ObjectGroupId>();
            //obj.GetComponent<ObjectGroupId>().material = obj.GetComponent<Renderer>().material;
            obj.GetComponent<ObjectGroupId>().index = i;
        }
            

            // Handle the screen orientation
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToPortrait = false;
        Screen.orientation = ScreenOrientation.AutoRotation;
    }



    // Update is called once per frame
    void Update () {
        if (Input.GetKey("space")) {
            MyNetworkManager.singleton.ServerChangeScene("SetupScene");
        }
	}

}
