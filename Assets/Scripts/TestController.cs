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

	public float[] spawnDistances = {-0.5072f, 0.3213f, -1.2994f,
									 -0.7071f, 0.2229f, -0.7370f,
									 -0.3286f, 0.2722f,	-0.2332f,
									  0.4105f, 0.2780f,	-0.2873f,
									  1.4755f, 0.2408f,	-0.7281f,
									  0.0256f, 0.6168f,	-2.2170f,
									 -0.9924f, 0.4576f,	 0.1774f,
									  1.0824f, 0.6730f,	-1.6407f,
									  2.2277f, 0.3076f,	-0.9219f,
									 -0.0225f, 0.6544f,	-3.0025f,
									 -1.7709f, 0.5819f,	 0.4100f,
									 -1.4733f, 0.4898f,	-2.4784f};


    void Awake() {

        if (tcontrol == null) {
            DontDestroyOnLoad(gameObject);
            tcontrol = this;

        } else if (tcontrol != this) {
            Destroy(gameObject);
        }


    }

}
	

