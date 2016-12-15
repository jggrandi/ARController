using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MainController : MonoBehaviour {

    public static MainController control;

    public GameObject trackedObjects;

    public int targetsTrackedNow = 0;
    public int totalTargets = 0;

    public int transformationNow = 0;
    public bool lockTransform = false;
    public bool groupButtonActive = false;
    public List<GameObject> objSelectedNow = new List<GameObject>();

    public int idAvaiableNow = 0;

    void Awake() {

        if (control == null) {
            DontDestroyOnLoad(gameObject);
            control = this;

        } else if (control != this) {
            Destroy(gameObject);
        }

        for (int i = 0; i < trackedObjects.transform.childCount; i++)
            trackedObjects.transform.GetChild(i).transform.gameObject.AddComponent<ObjectGroupId>();
            


            // Handle the screen orientation
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToPortraitUpsideDown = true;
        Screen.autorotateToPortrait = true;
        Screen.orientation = ScreenOrientation.AutoRotation;
    }



    // Update is called once per frame
    void Update () {
        //foreach(GameObject g in objSelectedNow) {
        //    Debug.Log(g.name);
        //}
        //Debug.Log("tracked now: " + targetsTrackedNow);
	}
}
