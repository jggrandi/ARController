using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TestController : MonoBehaviour {

    public static TestController tcontrol;

	int tasksToPermute = 3;

	int[] taskOrder;

    public string userID = "325";
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


		taskOrder = Utils.selectUserTaskSequence (int.Parse(userID), tasksToPermute);
		for (int i = 0; i < tasksToPermute; i++)
			Debug.Log (taskOrder[i]);

    }




    // Update is called once per frame
    void Update() {
        //foreach(GameObject g in objSelectedNow) {
        //    Debug.Log(g.name);
        //}
        //Debug.Log("tracked now: " + targetsTrackedNow);


    }



}
	

