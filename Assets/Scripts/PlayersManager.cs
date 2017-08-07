using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class PlayersManager : NetworkBehaviour {
    public static PlayersManager playerManager;
    public List<NetworkConnection> list;
    

    public NetworkConnection Get(int id) {
        foreach(NetworkConnection n in playerManager.list) {
            if(n.connectionId == id)
                return n;
        }
        return null;
    }


    public void Set(NetworkConnection obj) {
        playerManager.list.Add(obj);

    }

    // Use this for initialization
    void Start() {

    }

    void Awake() {
        //var parent = GameObject.Find("TrackedObjects").transform;

        //if (parent == null) return;

        //for (int i = 0; i < parent.childCount; i++) {
        //    list.Add(parent.GetChild(i).gameObject);
        //}
        //parent = GameObject.Find("TrainingObjects").transform;
        //if (parent == null) return;
        //for (int i = 0; i < parent.childCount; i++) {
        //    list.Add(parent.GetChild(i).gameObject);
        //}
        

        if (playerManager == null) {
            DontDestroyOnLoad(gameObject);
            playerManager = this;

        } else if (playerManager != this) {
            Destroy(gameObject);
        }

    }

    // Update is called once per frame
    void Update() {

    }
}
