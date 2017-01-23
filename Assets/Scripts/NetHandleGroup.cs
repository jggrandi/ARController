using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetHandleGroup : NetworkBehaviour {

    DataSync dataSync;
    public void Start() {
        dataSync = GameObject.Find("MainHandler").GetComponent<DataSync>();
    }

    public int GetIndex(GameObject g) {
        return g.GetComponent<ObjectGroupId>().index;
    }

    public void CreateGroup() {
        foreach (int index in MainController.control.objSelected) {
            CmdNewGroup(index);
        }
        CmdIncrementGroupCount();
        MainController.control.isMultipleSelection = false;
    }


    public void UnGroup() {
        
        foreach (int index in MainController.control.objSelected) {
            CmdSetGroup(index, -1);
            var g = Utils.GetByIndex(index);
            //g.GetComponent<Renderer>().material = g.GetComponent<ObjectGroupId>().material;
            
        }
        
        MainController.control.objSelected.Clear();
        MainController.control.isMultipleSelection = false;
    }

    [Command]
    public void CmdSetGroup(int index, int group) {
        dataSync.Groups[index] = group;
    }

    [Command]
    public void CmdNewGroup(int index) {
        dataSync.Groups[index] = dataSync.GroupCount;
    }

    [Command]
    public void CmdIncrementGroupCount() {
        dataSync.GroupCount++;
    }


}
