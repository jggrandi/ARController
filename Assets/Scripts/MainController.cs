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
    

    public int idAvaiableNow = 0;

    public bool isTapForTransform = false;

    public static bool Landscape = true;
    void Awake() {

        control = this;

        for (int i = 0; i < trackedObjects.transform.childCount; i++) {
            GameObject obj = trackedObjects.transform.GetChild(i).transform.gameObject;
            obj.AddComponent<ObjectGroupId>();
            //obj.GetComponent<ObjectGroupId>().material = obj.GetComponent<Renderer>().material;
            obj.GetComponent<ObjectGroupId>().index = i;
        }

             
        // Handle the screen orientation
        
        Screen.autorotateToLandscapeLeft = Landscape;
        Screen.autorotateToLandscapeRight = Landscape;
        Screen.autorotateToPortraitUpsideDown = !Landscape;
        Screen.autorotateToPortrait = !Landscape;
        Screen.orientation = Landscape?ScreenOrientation.Landscape: ScreenOrientation.Portrait;
    }

    public float dist = 0.75f;
    void OnApplicationPause(bool pause) {
        if (pause) {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }

    Vector3 prevPos;
    // Update is called once per frame
    void Update () {
        if (Input.GetKey("space")) {
            MyNetworkManager.singleton.ServerChangeScene("SetupTest");
        }
    }

}
