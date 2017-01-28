using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class HandleLog : NetworkBehaviour {

    public Log log;
    private int countFrames = 0;
    DataSync dataSync;
    GameObject trackedObjects;
    public bool closeRecord = false;

    // Use this for initialization
    void Start () {

        
        if (!isServer ) return;

        dataSync = gameObject.GetComponent<DataSync>();
        trackedObjects = GameObject.Find("TrackedObjects").gameObject;
        log = new Log(TestController.tcontrol.userID.ToString());
	}
	

    void FixedUpdate() {
        
        if (!isServer ) return;
        
        if (countFrames % 5 == 0) {
            //foreach (var player in GameObject.FindGameObjectsWithTag("player"))
            //    if(player.GetComponent<Lean.Touch.NetHandleSelectionTouch>().objSelectedShared.Count == 0) return;


            log.save(dataSync.piecesList[dataSync.pieceActiveNow], trackedObjects.transform.GetChild(0).gameObject, Camera.main.transform.position, dataSync.errorTranslation, dataSync.errorRotation);
        }
        countFrames++;
    }

   

}
