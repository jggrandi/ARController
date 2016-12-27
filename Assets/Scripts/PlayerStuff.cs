using UnityEngine;
using System.Collections;

public class PlayerStuff : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameObject go = GameObject.Find("MainHandler");
        go.GetComponent<NetHandleGUI>().playerObject = this.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
