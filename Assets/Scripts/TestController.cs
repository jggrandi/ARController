using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TestController : MonoBehaviour {

    public static TestController tcontrol;

	int tasksToPermute = 3;

	int[] taskOrder;

    public string userID = "5";
    public string sceneID = "0";


    void Awake() {

        if (tcontrol == null) {
            DontDestroyOnLoad(gameObject);
            tcontrol = this;

        } else if (tcontrol != this) {
            Destroy(gameObject);
        }



        GameObject.Find("InputFieldUserID").GetComponent<InputField>().text = userID;

        GenerateTaskOrder();


        //GameObject.Find("InputFieldSceneID").GetComponent<InputField>().text = taskOrder[int.Parse(sceneID)].ToString();

        //for (int i = 0; i < tasksToPermute; i++)
        //	Debug.Log (taskOrder[i]);

    }




    // Update is called once per frame
    void Update() {
        //foreach(GameObject g in objSelectedNow) {
        //    Debug.Log(g.name);
        //}
        //Debug.Log("tracked now: " + targetsTrackedNow);


    }

    public void GenerateTaskOrder() {
        if (GameObject.Find("InputFieldUserID").GetComponent<InputField>().text == "") return;
        userID = GameObject.Find("InputFieldUserID").GetComponent<InputField>().text;
        taskOrder = Utils.selectUserTaskSequence(int.Parse(userID), tasksToPermute);
        GameObject.Find("InputFieldSceneID").GetComponent<InputField>().text = taskOrder[int.Parse(sceneID)].ToString();
    }

}
	

