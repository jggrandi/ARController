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

    bool isTheFirstPiece = true;

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
    int modality = -2;

    void FixedUpdate() {
        
        if (!isServer ) return;

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
        
        if (countFrames % 5 == 0 && dataSync.pieceActiveNow < dataSync.piecesList.Count) {
            log.saveVerbose(dataSync.piecesList[dataSync.pieceActiveNow], isObjSelected, (int)dataSync.distancesList[dataSync.piecesList[dataSync.pieceActiveNow]] / 4, dataSync.distancesList[dataSync.pieceActiveNow] * 3, dataSync.rotationsList[dataSync.pieceActiveNow], modality, trackedObjects.transform.GetChild(dataSync.piecesList[dataSync.pieceActiveNow]).gameObject, camPos, dataSync.errorTranslation, dataSync.errorRotation, targetsTracked);
        }

        Debug.Log(previousPiece + " - " + dataSync.pieceActiveNow);
        if (previousPiece != dataSync.pieceActiveNow) {
            SaveResumed(previousPiece);
            previousPiece = dataSync.pieceActiveNow;
            time = 0.0f;
        }

        countFrames++;
    }

    public void SaveResumed(int pieceID) {
            time = Time.realtimeSinceStartup - time;
            log.saveResume(dataSync.piecesList[pieceID], (int)dataSync.distancesList[dataSync.piecesList[pieceID]] / 4, dataSync.rotationsList[pieceID], time, dataSync.errorTranslation, dataSync.errorRotation);
    }


	void OnApplicationQuit(){
        log.close ();
	}

}
