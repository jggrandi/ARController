using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MainController : MonoBehaviour {

    public static MainController control;

    public int targetsTrackedNow = 0;
    public int totalTargets = 0;

    public int transformationNow;
    public bool lockTransform;
    public List<GameObject> objSelectedNow = new List<GameObject>();

    void Awake() {

        if (control == null) {
            DontDestroyOnLoad(gameObject);
            control = this;

        } else if (control != this) {
            Destroy(gameObject);
        }

        // Handle the screen orientation
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToPortraitUpsideDown = true;
        Screen.autorotateToPortrait = true;
        Screen.orientation = ScreenOrientation.AutoRotation;
    }



    // Update is called once per frame
    void Update () {
        foreach(GameObject g in objSelectedNow) {
            Debug.Log(g.name);
        }
        //Debug.Log("tracked now: " + targetsTrackedNow);
	}
}
