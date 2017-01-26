﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class DataSync : NetworkBehaviour {

    [SyncVar]
    public int pieceActiveNow = 0;

    public SyncListInt Groups = new SyncListInt();
    public SyncListInt piecesList = new SyncListInt();
    public int GroupCount = 0;
    public GameObject playerObject;

    
    
    public override void OnStartServer() {
        GameObject trackedObjects = GameObject.Find("TrackedObjects");
        for (int i = 0; i< trackedObjects.transform.childCount; i++) {
            Groups.Add(-1);
        }

        List<int> randomizedList = Utils.randomizeVector(trackedObjects.transform.childCount/2); // Randomize the blocks order. Store it in an array. The randomizer have to only choose even values
        listToSyncList(ref randomizedList, ref piecesList);

        
    }

    void listToSyncList(ref List<int> list, ref SyncListInt syncList) {
        syncList.Clear();
        for (int i = 0; i < list.Count; i++) {
            syncList.Add(list[i]);
        }
    }



}
