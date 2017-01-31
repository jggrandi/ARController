using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class HandleLog : NetworkBehaviour {

    public Log log;
    private int countFrames = 0;
    DataSync dataSync;
    GameObject trackedObjects;
    public bool closeRecord = false;

	int previousPiece = -1;

    // Use this for initialization
    void Start () {
        if (!isServer ) return;

        dataSync = gameObject.GetComponent<DataSync>();
        trackedObjects = GameObject.Find("TrackedObjects").gameObject;
        log = new Log(TestController.tcontrol.userID.ToString());

		previousPiece = dataSync.pieceActiveNow;
	}
	

    void FixedUpdate() {
        
        if (!isServer ) return;
        
        if (countFrames % 5 == 0) {
            foreach (var player in GameObject.FindGameObjectsWithTag("player"))
				if(!player.GetComponent<NetworkIdentity>().isLocalPlayer)
                	if(player.GetComponent<Lean.Touch.NetHandleSelectionTouch>().objSelectedShared.Count != 0) 
						log.saveVerbose(dataSync.piecesList[dataSync.pieceActiveNow], trackedObjects.transform.GetChild(dataSync.piecesList[dataSync.pieceActiveNow]).gameObject, player.GetComponent<Lean.Touch.NetHandleSelectionTouch>().CameraPosition, dataSync.errorTranslation, dataSync.errorRotation);
        }

		if (previousPiece != dataSync.pieceActiveNow && dataSync.pieceActiveNow < dataSync.piecesList.Count) { 
			log.saveResume (dataSync.piecesList [dataSync.pieceActiveNow], dataSync.errorTranslation, dataSync.errorRotation);
			previousPiece = dataSync.pieceActiveNow;
		}
        countFrames++;
    }

	void OnApplicationQuit(){
		log.close ();
	}

}
