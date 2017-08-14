using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class HandleUsersConnected : NetworkBehaviour {

    public int usersConnected = 0;

    void FixedUpdate() {
        if (!isServer) return;

        var gameobjects = GameObject.FindGameObjectsWithTag("player");
        usersConnected = gameobjects.Length;
    }
}
