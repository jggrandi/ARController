﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class HandleUsersConnected : NetworkBehaviour {

    DataSync DataSyncRef;
    GameObject handler;
    //public List<int> usersDone;

    public bool FindUser(int id) {
        foreach (int i in DataSyncRef.usersDone)
            if (i == id)
                return true;
        return false;
    }

    [Command]
    void CmdAdd(int id) {
        DataSyncRef.usersDone.Add(id);
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
    }
}
