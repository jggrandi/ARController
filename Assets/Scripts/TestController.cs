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
    public int groupID = 0;
    [SyncVar]
    public int sceneIndex = 0;

    public int userID = 0;


    //public float[] spawnDistances = {
    //    -0.9719447f,  0.7384897f,   0.0f,        //1-near
    //    -0.5855348f,  0.642159f,    0.7983765f,  //1-near
    //    -0.4039211f,  0.6972844f,   -0.8934861f, //1-near
    //    0.7585833f,   0.5974794f,   -0.6934476f, //1-near
    //    0.7324734f,   0.6683413f,   0.6792539f,  //2-med
    //    1.324226f,    0.5784369f,   0.7516428f,  //2-med
    //    -1.19018f,    0.645997f,    0.8342187f,  //2-med
    //    -0.9268596f,  0.7322414f,   -1.130795f,  //2-med
    //    1.088868f,    0.6498964f,   -1.067111f,  //3-far
    //    1.749623f,    0.7544146f,   -1.002392f,  //3-far
    //    -1.909679f,   0.6670301f,   0.3865005f,  //3-far
    //    -1.814323f,   0.6799691f,   -0.7294753f }; //3-far

    public float[] spawnDistances = {
        1.088868f,    0.6498964f,   -1.067111f,  //2-med
        1.324226f,    0.5784369f,   0.7516428f,  //2-med
        -1.19018f,    0.645997f,    0.8342187f,  //2-med
        -0.9268596f,  0.7322414f,   -1.130795f,  //2-med
    };


    //Use Quaternion.AngleAxis(<angle>,<axis>) to rotate with a determied angle and axis an object.
    //public float[] spawnRotations = { //random angle and axis
    //    -0.8f, -0.3f,  0.4f, 0.1f,
    //    -0.6f, -0.1f, -0.7f, 0.4f,
    //     0.4f,  0.5f,  0.6f, 0.4f,
    //     0.6f, -0.8f,  0.1f, 0.3f,
    //};

    //public float[] spawnRotations = { // 30 degrees with random axis
    //     0.6f, -0.1f, -0.3f, -0.7f,
    //     0.3f, -0.4f, -0.5f, -0.7f,
    //    -0.5f, -0.1f,  0.5f, -0.7f,
    //    -0.5f,  0.4f, -0.1f, -0.7f         
    //};

    //public float[] spawnRotations = { // 120 degrees with random axis
    //     0.0f,  0.6f,  0.6f, 0.5f,
    //    -0.7f,  0.2f, -0.4f, 0.5f,
    //     0.4f,  0.7f,  0.3f, 0.5f,
    //     0.3f, -0.7f, -0.4f, 0.5f
    //};

    //public float[] spawnRotations = { 
    //    -0.1f,  0.3f,  0.2f, 0.9f, //45
    //     0.2f, -0.2f,  0.2f, 0.9f, //45
    //    -0.2f, -0.3f,  0.1f, 0.9f, //45
    //    -0.3f,  0.4f, -0.5f, 0.7f, //90
    //    -0.5f, -0.4f, -0.2f, 0.7f, //90
    //    -0.4f,  0.5f,  0.3f, 0.7f, //90
    //    -0.1f,  0.3f,  0.2f, 0.9f, //45
    //     0.2f, -0.2f,  0.2f, 0.9f, //45
    //    -0.2f, -0.3f,  0.1f, 0.9f, //45
    //    -0.3f,  0.4f, -0.5f, 0.7f, //90
    //    -0.5f, -0.4f, -0.2f, 0.7f, //90
    //    -0.4f,  0.5f,  0.3f, 0.7f  //90
    //};

    public float[] spawnRotations = {
        -0.1f,  0.3f,  0.2f, 0.9f, //45
         0.2f, -0.2f,  0.2f, 0.9f, //45
        -0.3f,  0.4f, -0.5f, 0.7f, //90
        -0.5f, -0.4f, -0.2f, 0.7f, //90
        -0.1f,  0.3f,  0.2f, 0.9f, //45
         0.2f, -0.2f,  0.2f, 0.9f, //45
        -0.3f,  0.4f, -0.5f, 0.7f, //90
        -0.5f, -0.4f, -0.2f, 0.7f, //90
    };


    void Awake() {
		
        if (tcontrol == null) {
            DontDestroyOnLoad(gameObject);
            tcontrol = this;

        } else if (tcontrol != this) {
            Destroy(gameObject);
        }

        //for (int i = 0; i < 6; i++) {
        //    Vector3 vec = new Vector3(UnityEngine.Random.Range(-10.0f, 10.0f), UnityEngine.Random.Range(-10.0f, 10.0f), UnityEngine.Random.Range(-10.0f, 10.0f));
        //    Debug.Log(Quaternion.AngleAxis(90, vec));
        //}

    }

}
	

