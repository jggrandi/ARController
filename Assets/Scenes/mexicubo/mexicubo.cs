using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class mexicubo : NetworkBehaviour {

    
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;
        CmdMove(x, z);
    }

    [Command]
    void CmdMove(float x, float z) {
        GameObject.FindGameObjectWithTag("box").transform.Rotate(0, x, 0);
        GameObject.FindGameObjectWithTag("box").transform.Translate(0, 0, z);
    }

}
