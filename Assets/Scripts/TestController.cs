using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;

public class TestController : NetworkBehaviour {

    public static TestController tcontrol;

	public int tasksToPermute = 3;

	public int[] taskOrder;
    [SyncVar]
    public int userID = 0;
    [SyncVar]
    public int sceneID = 0;
    [SyncVar]
    public int idNow = 0;

    void Awake() {

        if (tcontrol == null) {
            DontDestroyOnLoad(gameObject);
            tcontrol = this;

        } else if (tcontrol != this) {
            Destroy(gameObject);
        }


    }

}
	

