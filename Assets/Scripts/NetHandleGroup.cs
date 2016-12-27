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


    [Command]
    public void CmdSetGroup(GameObject obj) {
        Debug.Log("SetGroup");
        obj.transform.gameObject.GetComponent<ObjectGroupId>().id = MainController.control.idAvaiableNow;
    }

    [Command]
    public void CmdSetGroup2(GameObject obj, int id) {
        Debug.Log("SetGroup2");
        obj.transform.gameObject.GetComponent<ObjectGroupId>().id = id;
    }

    [Command]
    public void CmdIncrementCount() {
        Debug.Log("INC");
        MainController.control.idAvaiableNow++;
    }



    public void CreateGroup() {
        Debug.Log("Group");
        
        foreach (GameObject g in MainController.control.objSelectedNow) {
            CmdSetGroup(g);
        }
        CmdIncrementCount();
    }


    public void UnGroup() {
        
        foreach (GameObject g in MainController.control.objSelectedNow) {
            CmdSetGroup2(g, -1);
            g.GetComponent<Renderer>().material.color = Color.white;
        }
        
        MainController.control.objSelectedNow.Clear();
    }
}
