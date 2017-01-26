using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TestController : MonoBehaviour {

    public static TestController tcontrol;

    int[] taskOrder = { 1, 2, 3,
                        1, 3, 2,
                        2, 1, 3,
                        2, 3, 1,
                        3, 1, 2,
                        3, 2, 1,
    };

    public string userID;
    public string sceneID;


    void Awake() {

        if (tcontrol == null) {
            DontDestroyOnLoad(gameObject);
            tcontrol = this;

        } else if (tcontrol != this) {
            Destroy(gameObject);
        }
        //for(int i = 0; i<15;i++)
        //    Debug.Log(i % 6 );
    }



    // Update is called once per frame
    void Update() {
        //foreach(GameObject g in objSelectedNow) {
        //    Debug.Log(g.name);
        //}
        //Debug.Log("tracked now: " + targetsTrackedNow);
    }

}

