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
        if (TestController.tcontrol.sceneIndex == 0) return;

        dataSync = gameObject.GetComponent<DataSync>();
        trackedObjects = GameObject.Find("TrackedObjects").gameObject;
        int task = TestController.tcontrol.taskOrder[TestController.tcontrol.sceneIndex];
        log = new Log(TestController.tcontrol.userID.ToString(), task);

		previousPiece = dataSync.pieceActiveNow;
	}

    public bool isObjSelected = false;
    Vector3 camPos;
    int targetsTracked;
    int modality = -2;

    void FixedUpdate() {
        
        if (!isServer ) return;
        if (TestController.tcontrol.sceneIndex == 0 || dataSync.pieceTraining < 2) return;

        foreach (var player in GameObject.FindGameObjectsWithTag("player")) {
            if (!player.GetComponent<NetworkIdentity>().isLocalPlayer) {
                camPos = player.GetComponent<Lean.Touch.NetHandleSelectionTouch>().CameraPosition;
                targetsTracked = player.GetComponent<Lean.Touch.NetHandleSelectionTouch>().targetsTracked;
                modality = player.GetComponent<Lean.Touch.NetHandleTransformations>().modality;
                if (player.GetComponent<Lean.Touch.NetHandleSelectionTouch>().objSelectedShared.Count != 0) {
                    isObjSelected = true;
                    if (time == 0.0f)
                        time = Time.realtimeSinceStartup;
                } else {
                    isObjSelected = false;
                }
            }
        }
        
		if (countFrames % 5 == 0 ) { //&& dataSync.pieceActiveNow < dataSync.piecesList.Count
			log.saveVerbose(dataSync.piecesList[dataSync.pieceActiveNow], isObjSelected, dataSync.posList[dataSync.piecesList[dataSync.pieceActiveNow]] / 4, dataSync.posList[dataSync.pieceActiveNow] * 3, dataSync.rotationsList[dataSync.pieceActiveNow], modality, trackedObjects.transform.GetChild(dataSync.piecesList[dataSync.pieceActiveNow]).gameObject, camPos, dataSync.errorTranslation, dataSync.errorRotation, dataSync.errorRotationAngle, targetsTracked);
        }

        if (dataSync.saveResumed) {
            SaveResumed();
        }

        //if (previousPiece != dataSync.pieceActiveNow) {
        //    SaveResumed(previousPiece);
        //    previousPiece = dataSync.pieceActiveNow;
        //    time = 0.0f;
        //}

        countFrames++;
    }

    public void SaveResumed() {
        time = Time.realtimeSinceStartup - time;
        this.log.saveResume(dataSync.piecesList[dataSync.pieceActiveNow], dataSync.posList[dataSync.piecesList[dataSync.pieceActiveNow]] / 4, dataSync.rotationsList[dataSync.pieceActiveNow], time, dataSync.errorTranslation, dataSync.errorRotation, dataSync.errorRotationAngle);
        time = 0.0f;
        dataSync.saveResumed = false;
    }


	void OnApplicationQuit(){
        log.close ();
	}

}
