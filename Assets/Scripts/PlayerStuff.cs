using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerStuff : NetworkBehaviour {

	void Start () {
        if (!isLocalPlayer) return;

        GameObject go = GameObject.Find("MainHandler");
        if (go != null) 
            go.GetComponent<NetHandleGUI>().playerObject = this.gameObject;
        
    }

}
