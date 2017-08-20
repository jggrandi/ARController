using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using Vuforia;


public class PlayerStuff : NetworkBehaviour {

    GameObject trackableMan;
    [SyncVar]
    public int targetsTracked = 0;

    [SyncVar]
    public int userID = -1;

    void Start () {
        if (!isLocalPlayer) return;

        if (MainController.control == null) return;
        userID = MainController.control.userID;
        CmdUpdateUserID(userID);
        
        GameObject go = GameObject.Find("MainHandler");
        if (go != null) 
            go.GetComponent<NetHandleGUI>().playerObject = this.gameObject;
        
    }

    int qnt = 0;
    int prevQnt = 0;
    void Update() {
        if (!isLocalPlayer) return;
        
        qnt = MainController.control.trackedTargets.Count;
        if (prevQnt != qnt) {
            CmdUpdateTrackedTargets(qnt);
            prevQnt = qnt;
        }
    }

    [Command]
    void CmdUpdateTrackedTargets(int value) {
        targetsTracked = value;
    }

    [Command]
    void CmdUpdateUserID(int id) {
        userID = id;
    }

}
