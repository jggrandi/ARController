﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class StackController : NetworkBehaviour {


    GameObject trackedObjects;
    //int[] objectsOrder;

    public int halfObjects;
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

		for (int i = 0; i < halfObjects; i++) {
			float x = TestController.tcontrol.spawnDistances [dataSync.distancesList [i] * 3];
			float y = TestController.tcontrol.spawnDistances [dataSync.distancesList [i] * 3 + 1] ;
			float z = TestController.tcontrol.spawnDistances [dataSync.distancesList [i] * 3 + 2] ;

            float rx = TestController.tcontrol.spawnRotations[dataSync.rotationsList[i] * 4];
            float ry = TestController.tcontrol.spawnRotations[dataSync.rotationsList[i] * 4 + 1];
            float rz = TestController.tcontrol.spawnRotations[dataSync.rotationsList[i] * 4 + 2];
            float rw = TestController.tcontrol.spawnRotations[dataSync.rotationsList[i] * 4 + 3];

            trackedObjects.transform.GetChild (i).transform.position = new Vector3 (x, y, z);
            trackedObjects.transform.GetChild(i).transform.rotation = new Quaternion(rx, ry, rz, rw);
        }

		foreach (Transform child in trackedObjects.transform) // Disable all objects.
            child.gameObject.SetActive(false);



    }

    void Update() {
        if (!isLocalPlayer) return;
        if (dataSync.pieceActiveNow == halfObjects) return;

        childMoving = trackedObjects.transform.GetChild(dataSync.piecesList[dataSync.pieceActiveNow]); // take the moving object 
        childStatic = trackedObjects.transform.GetChild(dataSync.piecesList[dataSync.pieceActiveNow] + halfObjects); // and its ghost

        if (dataSync.pieceActiveNow > 0) {
            trackedObjects.transform.GetChild(dataSync.piecesList[dataSync.pieceActiveNow - 1]).gameObject.SetActive(false); // disable the previous object
            trackedObjects.transform.GetChild(dataSync.piecesList[dataSync.pieceActiveNow - 1] + halfObjects).gameObject.SetActive(false); //and its ghost
        }

        childMoving.gameObject.SetActive(true);
        childStatic.gameObject.SetActive(true);

        movingObjMatrixTrans = Matrix4x4.TRS(childMoving.transform.position, Quaternion.identity, new Vector3(1.0f, 1.0f, 1.0f));
        movingObjMatrixRot = Matrix4x4.TRS(new Vector3(0, 0, 0), childMoving.transform.rotation, new Vector3(1.0f, 1.0f, 1.0f));

        staticObjMatrixTrans = Matrix4x4.TRS(childStatic.transform.position, Quaternion.identity, new Vector3(1.0f, 1.0f, 1.0f));
        staticObjMatrixRot = Matrix4x4.TRS(new Vector3(0, 0, 0), childStatic.transform.rotation, new Vector3(1.0f, 1.0f, 1.0f));

        dataSync.errorTranslation = Utils.distMatrices(movingObjMatrixTrans, staticObjMatrixTrans);
        dataSync.errorRotation = Utils.distMatrices(movingObjMatrixRot, staticObjMatrixRot);

    }


    [ClientRpc]
    void RpcIncrementSceneID() {
        TestController.tcontrol.sceneIndex++;
    }

    [Command]
    void CmdChangeScene() {
        //RpcIncrementSceneID();
        TestController.tcontrol.sceneIndex++;
        MyNetworkManager.singleton.ServerChangeScene("SetupScene");
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
            StartCoroutine(Wait());
        }
    }

    IEnumerator Wait() { // wait until saving the resumed log.

        yield return new WaitForSeconds(2);
        CmdChangeScene();
    }


}

