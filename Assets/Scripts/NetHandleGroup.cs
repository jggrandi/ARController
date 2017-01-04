using UnityEngine;
using System.Collections;

public class NetHandleGroup : MonoBehaviour {

    DataSync DataSyncRef;

    public void Start() {
        DataSyncRef = GameObject.Find("MainHandler").GetComponent<DataSync>();
    }

    public int GetIndex(GameObject g) {
        return g.GetComponent<ObjectGroupId>().index;
    }

    public void CreateGroup() {
        foreach (GameObject g in MainController.control.objSelectedNow) {
            DataSyncRef.CmdNewGroup(GetIndex(g));
        }
        DataSyncRef.CmdIncrementGroupCount();
        MainController.control.isMultipleSelection = false;
    }


    public void UnGroup() {
        
        foreach (GameObject g in MainController.control.objSelectedNow) {
            DataSyncRef.CmdSetGroup(GetIndex(g), -1);
            g.GetComponent<Renderer>().material.color = Color.white;
        }
        
        MainController.control.objSelectedNow.Clear();
        MainController.control.isMultipleSelection = false;
    }
}
