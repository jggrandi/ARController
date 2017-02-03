using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class HandleLog : NetworkBehaviour {

    public Log log;
    private int countFrames = 0;
    DataSync dataSync;
    GameObject trackedObjects;
    public bool closeRecord = false;

	public int previousPiece = -1;
    public float time = 0;

    // Use this for initialization
    void Start () {
        if (!isServer ) return;

        dataSync = gameObject.GetComponent<DataSync>();
        trackedObjects = GameObject.Find("TrackedObjects").gameObject;
        int task = TestController.tcontrol.taskOrder[TestController.tcontrol.sceneIndex];
        log = new Log(TestController.tcontrol.userID.ToString(), task);

		previousPiece = dataSync.pieceActiveNow;
	}

    public bool isObjSelected = false;
    Vector3 camPos;
    int targetsTracked;

    void FixedUpdate() {
        
        if (!isServer ) return;

        foreach (var player in GameObject.FindGameObjectsWithTag("player")) {
            if (!player.GetComponent<NetworkIdentity>().isLocalPlayer) {
                camPos = player.GetComponent<Lean.Touch.NetHandleSelectionTouch>().CameraPosition;
                targetsTracked = player.GetComponent<Lean.Touch.NetHandleSelectionTouch>().targetsTracked;
                if (player.GetComponent<Lean.Touch.NetHandleSelectionTouch>().objSelectedShared.Count != 0) {
                    isObjSelected = true;
                    if (time == 0.0f)
                        time = Time.realtimeSinceStartup;
                } else {
                    isObjSelected = false;
                }
            }
        }
        
        if (countFrames % 5 == 0) {
            log.saveVerbose(dataSync.piecesList[dataSync.pieceActiveNow], isObjSelected, dataSync.transformationModality, trackedObjects.transform.GetChild(dataSync.piecesList[dataSync.pieceActiveNow]).gameObject, camPos, dataSync.errorTranslation, dataSync.errorRotation, targetsTracked);
        }

		if (previousPiece != dataSync.pieceActiveNow && dataSync.pieceActiveNow <= dataSync.piecesList.Count) {
            time = Time.realtimeSinceStartup - time;
            log.saveResume (dataSync.piecesList [dataSync.pieceActiveNow -1], time, dataSync.errorTranslation, dataSync.errorRotation);
			previousPiece = dataSync.pieceActiveNow;
            time = 0.0f;
		}
        countFrames++;
    }

	void OnApplicationQuit(){
		log.close ();
	}

}
