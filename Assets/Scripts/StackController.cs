using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class StackController : NetworkBehaviour {


    GameObject trackedObjects;
    //int[] objectsOrder;
    
    int halfObjects;
    Transform childMoving;
    Transform childStatic;

    Matrix4x4 movingObjMatrixTrans;
    Matrix4x4 movingObjMatrixRot;

    Matrix4x4 staticObjMatrixTrans;
    Matrix4x4 staticObjMatrixRot;


    DataSync dataSync;

    void Start() {
        dataSync = GameObject.Find("MainHandler").GetComponent<DataSync>();

        trackedObjects = GameObject.Find("TrackedObjects");
        halfObjects = trackedObjects.transform.childCount / 2; // The objs/2 values are the moving objects.

        foreach (Transform child in trackedObjects.transform) // Disable all objects.
            child.gameObject.SetActive(false);

    }

    void Update() {
        if (!isLocalPlayer) return;
        for (int i = 0; i < halfObjects; i++) {
            if (i != dataSync.pieceActiveNow) {
                trackedObjects.transform.GetChild(i).gameObject.SetActive(false);
                trackedObjects.transform.GetChild(i + halfObjects).gameObject.SetActive(false);
            }
        }

        childMoving = trackedObjects.transform.GetChild(dataSync.piecesList[dataSync.pieceActiveNow]); // take the moving object 
        childStatic = trackedObjects.transform.GetChild(dataSync.piecesList[dataSync.pieceActiveNow] + halfObjects); // and its ghost
        //Debug.Log(objIndex + halfObjects);
        //Debug.Log(objectsOrder[objIndex] + " " + objectsOrder[objIndex] + halfObjects);

        childMoving.gameObject.SetActive(true);
        childStatic.gameObject.SetActive(true);

        movingObjMatrixTrans = Matrix4x4.TRS(childMoving.transform.position, Quaternion.identity, new Vector3(1.0f, 1.0f, 1.0f));
        movingObjMatrixRot = Matrix4x4.TRS(new Vector3(0, 0, 0), childMoving.transform.rotation, new Vector3(1.0f, 1.0f, 1.0f));

        staticObjMatrixTrans = Matrix4x4.TRS(childStatic.transform.position, Quaternion.identity, new Vector3(1.0f, 1.0f, 1.0f));
        staticObjMatrixRot = Matrix4x4.TRS(new Vector3(0, 0, 0), childStatic.transform.rotation, new Vector3(1.0f, 1.0f, 1.0f));

        dataSync.errorTranslation = Utils.distMatrices(movingObjMatrixTrans, staticObjMatrixTrans);
        dataSync.errorRotation = Utils.distMatrices(movingObjMatrixRot, staticObjMatrixRot);


    }
    
    [Command]
    void CmdChangeScene() {
        
        MyNetworkManager.singleton.ServerChangeScene("SetupScene");
        //TestController.tcontrol.sceneIdNow++;
    }


    [ClientRpc]
    void RpcIncrementPieceActiveNow() {
        dataSync.pieceActiveNow++;
    }

    [Command]
    void CmdPieceActiveNow() {
        RpcIncrementPieceActiveNow();
    }

    [ClientRpc]
    void RpcClearSelection() {
        foreach (var player in GameObject.FindGameObjectsWithTag("player"))
            player.GetComponent<Lean.Touch.NetHandleSelectionTouch>().objSelectedShared.Clear();

    }

    [Command]
    void CmdClearSelection() {
        RpcClearSelection();
    }


    public void SetNextPiece() {
        CmdPieceActiveNow();

        MainController.control.objSelected.Clear();
        CmdClearSelection();

        if (dataSync.pieceActiveNow == halfObjects - 1) {
            if(isServer)
                GameObject.Find("MainHandler").gameObject.GetComponent<HandleLog>().log.close();

            CmdChangeScene();
        }
    }
}

