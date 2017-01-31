using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class EndTaskStuff : NetworkBehaviour {

    // Use this for initialization
    void Start() {
        foreach (var player in GameObject.FindGameObjectsWithTag("player")) {
            player.gameObject.SetActive(false);
        }
    }

}
