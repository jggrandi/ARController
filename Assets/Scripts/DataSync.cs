using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class DataSync : NetworkBehaviour {

    public SyncListInt Groups = new SyncListInt();
    public int GroupCount = 0;
	
	void Update () {

    }
    
    public override void OnStartServer() {
        for (int i = 0; i< GameObject.Find("TrackedObjects").transform.childCount; i++) {
            Groups.Add(-1);
        }
    }

    [Command]
    public void CmdSetGroup(int index, int group) {
        Groups[index] = group;
    }

    [Command]
    public void CmdNewGroup(int index) {
        Groups[index] = GroupCount;
    }

    [Command]
    public void CmdIncrementGroupCount() {
        GroupCount++;
    }

}
