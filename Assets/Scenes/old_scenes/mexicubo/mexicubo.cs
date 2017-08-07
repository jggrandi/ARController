using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class mexicubo : NetworkBehaviour {

    GameObject mcubo;

	// Use this for initialization
	void Start () {
        mcubo = GameObject.FindGameObjectWithTag("box");
	}
	
	// Update is called once per frame
	void Update () {
        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;
        CmdMove(x, z);
    }

    [Command]
    void CmdMove(float x, float z) {
        mcubo.transform.Rotate(0, x, 0);
        mcubo.transform.Translate(0, 0, z);
    }

}
