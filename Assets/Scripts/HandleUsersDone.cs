using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class HandleUsersDone : NetworkBehaviour {

    DataSync DataSyncRef;
    GameObject handler;
    public SyncListInt usersDone = new SyncListInt();

    public bool FindUser(int id) {
        if (usersDone.Contains(id))
            return true;
        return false;
    }

    [Command]
    void CmdAdd(int id) {
        usersDone.Add(id);
    }

    public bool AddUsersDone(int id) {
        if (!FindUser(id)) {
            CmdAdd(id);
            return true;
        }
        return false;
    }

    void Start() {
        handler = GameObject.Find("MainHandler");
        if (handler == null) return;
        DataSyncRef = handler.GetComponent<DataSync>();
    }



    void FixedUpdate() {
        if (!isServer) return;

        var gameobjects = GameObject.FindGameObjectsWithTag("player");
        if(gameobjects.Length != 0)
            DataSyncRef.usersConnected = gameobjects.Length;

        //if (TestController.tcontrol.sceneIndex == 0 && isServer) { // if it is the how to use scene
        //    if (usersDone.Count == 0) return;
        //    if (usersDone.Count == 2) { //hard coded for 2 players.. it will not work for players != 2 
        //        DataSyncRef.changeScene = true;
        //    }
        //}

    }
}
