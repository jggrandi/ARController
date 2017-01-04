using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class DataSync : NetworkBehaviour {

    public SyncListInt Groups = new SyncListInt();
    public int GroupCount = 0;
    public GameObject playerObject;

    void Update () {

    }
    
    public override void OnStartServer() {
        for (int i = 0; i< GameObject.Find("TrackedObjects").transform.childCount; i++) {
            Groups.Add(-1);
        }
    }

}
