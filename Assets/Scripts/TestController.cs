using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;

public class TestController : NetworkBehaviour {

    public static TestController tcontrol;

	public int tasksToPermute = 3;

    public SyncListInt taskOrder = new SyncListInt();
    
    [SyncVar]
    public int userID = 0;
    [SyncVar]
    public int sceneIndex = 0;
    [SyncVar]
    public int sceneIdNow = 0;

    public float[] spawnDistances = {
        -0.9719447f,  0.7384897f,   0.0f,
        -0.5855348f,  0.642159f,    0.7983765f,
        -0.4039211f,  0.6972844f,   -0.8934861f,
        0.7585833f,   0.5974794f,   -0.6934476f,
        0.7324734f,   0.6683413f,   0.6792539f,
        1.324226f,    0.5784369f,   0.7516428f,
        -1.19018f,    0.645997f,    0.8342187f,
        -0.9268596f,  0.7322414f,   -1.130795f,
        1.088868f,    0.6498964f,   -1.067111f,
        1.749623f,    0.7544146f,   -1.002392f,
        -1.909679f,   0.6670301f,   0.3865005f,
        -1.814323f,   0.6799691f,   -0.7294753f };



    void Awake() {

        if (tcontrol == null) {
            DontDestroyOnLoad(gameObject);
            tcontrol = this;

        } else if (tcontrol != this) {
            Destroy(gameObject);
        }


    }

}
	

