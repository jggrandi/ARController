using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerStuff : NetworkBehaviour {

	// Use this for initialization
	void Start () {
        if (!isLocalPlayer) return;
        GameObject go = GameObject.Find("MainHandler");
        go.GetComponent<NetHandleGUI>().playerObject = this.gameObject;
        go.GetComponent<DataSync>().playerObject = this.gameObject;
    }
	
}
