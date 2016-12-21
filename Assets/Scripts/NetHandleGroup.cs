using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetHandleGroup : NetworkBehaviour {
    // Use this for initialization

    

    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
        //if(MainController.control.isToGroup) {
        //    Debug.Log(MainController.control.isToGroup);
        //    CreateGroup();

        //}
    }



    public void CreateGroup() {
        Debug.Log("Group");
        foreach (GameObject g in MainController.control.objSelectedNow) {
            MainController.control.netFunc.CmdSetGroup(g);
        }
        MainController.control.netFunc.CmdIncrementCount();
    }


    public void UnGroup() {

        foreach (GameObject g in MainController.control.objSelectedNow) {
            MainController.control.netFunc.CmdSetGroup2(g, -1);
            g.GetComponent<Renderer>().material.color = Color.white;
        }
        
        MainController.control.objSelectedNow.Clear();
    }
}
