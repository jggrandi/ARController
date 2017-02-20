using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class StackController : NetworkBehaviour {


    GameObject trackedObjects;
    GameObject trainingObjects;
    //int[] objectsOrder;

    public int halfObjects;
    public int halfTrainingObjects;
    Transform childMoving;
    Transform childStatic;

    Matrix4x4 movingObjMatrixTrans;
    Matrix4x4 movingObjMatrixRot;

    Matrix4x4 staticObjMatrixTrans;
    Matrix4x4 staticObjMatrixRot;


    DataSync dataSync;

    void Start() {

        dataSync = GameObject.Find("MainHandler").GetComponent<DataSync>();

        if (!isLocalPlayer) return;



        trackedObjects = GameObject.Find("TrackedObjects");
        trainingObjects = GameObject.Find("TrainingObjects");

        int count = 0;
        for (int i = trackedObjects.transform.childCount; i < trainingObjects.transform.childCount + trackedObjects.transform.childCount; i++) {

            GameObject obj = trainingObjects.transform.GetChild(count).transform.gameObject;
            obj.AddComponent<ObjectGroupId>();
            //obj.GetComponent<ObjectGroupId>().material = obj.GetComponent<Renderer>().material;
            obj.GetComponent<ObjectGroupId>().index = i;
            count++;
        }

        halfObjects = trackedObjects.transform.childCount / 2; // The objs/2 values are the moving objects. -2 to discard the training pieces in the end
        if (TestController.tcontrol.sceneIndex != 0) { // if it is not the howtouse scene
            for (int i = 0; i < halfObjects; i++) {
                float x = TestController.tcontrol.spawnDistances[dataSync.distancesList[i] * 3];
                float y = TestController.tcontrol.spawnDistances[dataSync.distancesList[i] * 3 + 1];
                float z = TestController.tcontrol.spawnDistances[dataSync.distancesList[i] * 3 + 2];

                float rx = TestController.tcontrol.spawnRotations[dataSync.rotationsList[i] * 4];
                float ry = TestController.tcontrol.spawnRotations[dataSync.rotationsList[i] * 4 + 1];
                float rz = TestController.tcontrol.spawnRotations[dataSync.rotationsList[i] * 4 + 2];
                float rw = TestController.tcontrol.spawnRotations[dataSync.rotationsList[i] * 4 + 3];

                trackedObjects.transform.GetChild(i).transform.position = new Vector3(x, y, z);
                trackedObjects.transform.GetChild(i).transform.rotation = new Quaternion(rx, ry, rz, rw);
            }

            int index = trainingObjects.transform.childCount;
            for (int i = 0; i < index; i++) {
                trainingObjects.transform.GetChild(0).transform.parent = trackedObjects.transform;
            }
        }


        foreach (Transform child in trackedObjects.transform) // Disable all objects.
            child.gameObject.SetActive(false);

        if (TestController.tcontrol.sceneIndex == 0) // if it is the howtouse scene, activate only one piece without their ghost
            trackedObjects.transform.GetChild(0).gameObject.SetActive(true);

        if (TestController.tcontrol.sceneIndex != 0 && dataSync.pieceTraining == 0) { // if is not the howtouse scene and is the first piece, activate the first tranning piece.
            trackedObjects.transform.GetChild(trackedObjects.transform.childCount - 4).gameObject.SetActive(true); // activate the moving object 
            trackedObjects.transform.GetChild(trackedObjects.transform.childCount - 3).gameObject.SetActive(true); // and its ghost, here the ghost is the next piece
            childMoving = trackedObjects.transform.GetChild(trackedObjects.transform.childCount - 4); // take the moving object 
            childStatic = trackedObjects.transform.GetChild(trackedObjects.transform.childCount - 3); // and its ghost
        }
    }


    void Update() {
        if (!isLocalPlayer) return;
        if (dataSync.pieceActiveNow == halfObjects) return;
        if (TestController.tcontrol.sceneIndex == 0) return;


        if (dataSync.pieceTraining == 1) {
            trackedObjects.transform.GetChild(trackedObjects.transform.childCount - 4).gameObject.SetActive(false);
            trackedObjects.transform.GetChild(trackedObjects.transform.childCount - 3).gameObject.SetActive(false);

            trackedObjects.transform.GetChild(trackedObjects.transform.childCount - 2).gameObject.SetActive(true);
            trackedObjects.transform.GetChild(trackedObjects.transform.childCount - 1).gameObject.SetActive(true);
            childMoving = trackedObjects.transform.GetChild(trackedObjects.transform.childCount - 2); // take the moving object 
            childStatic = trackedObjects.transform.GetChild(trackedObjects.transform.childCount - 1); // and its ghost
        } else if (dataSync.pieceTraining == 0) { //if user is in the training mode


            trackedObjects.transform.GetChild(trackedObjects.transform.childCount - 4).gameObject.SetActive(true);
            trackedObjects.transform.GetChild(trackedObjects.transform.childCount - 3).gameObject.SetActive(true);


            childMoving = trackedObjects.transform.GetChild(trackedObjects.transform.childCount - 4); // take the moving object 
            childStatic = trackedObjects.transform.GetChild(trackedObjects.transform.childCount - 3); // and its ghost

        } else { // if user is not in the training mode

            trackedObjects.transform.GetChild(trackedObjects.transform.childCount - 2).gameObject.SetActive(false);
            trackedObjects.transform.GetChild(trackedObjects.transform.childCount - 1).gameObject.SetActive(false);

            childMoving = trackedObjects.transform.GetChild(dataSync.piecesList[dataSync.pieceActiveNow]); // take the moving object 
            childStatic = trackedObjects.transform.GetChild(dataSync.piecesList[dataSync.pieceActiveNow] + halfObjects); // and its ghost

            if (dataSync.pieceActiveNow > 0) {
                trackedObjects.transform.GetChild(dataSync.piecesList[dataSync.pieceActiveNow - 1]).gameObject.SetActive(false); // disable the previous object
                trackedObjects.transform.GetChild(dataSync.piecesList[dataSync.pieceActiveNow - 1] + halfObjects).gameObject.SetActive(false); //and its ghost
            }
            childMoving.gameObject.SetActive(true);
            childStatic.gameObject.SetActive(true);
        }




        movingObjMatrixTrans = Matrix4x4.TRS(childMoving.transform.position, Quaternion.identity, new Vector3(1.0f, 1.0f, 1.0f));
        movingObjMatrixRot = Matrix4x4.TRS(new Vector3(0, 0, 0), childMoving.transform.rotation, new Vector3(1.0f, 1.0f, 1.0f));

        staticObjMatrixTrans = Matrix4x4.TRS(childStatic.transform.position, Quaternion.identity, new Vector3(1.0f, 1.0f, 1.0f));
        staticObjMatrixRot = Matrix4x4.TRS(new Vector3(0, 0, 0), childStatic.transform.rotation, new Vector3(1.0f, 1.0f, 1.0f));

        dataSync.errorTranslation = Utils.distMatrices(movingObjMatrixTrans, staticObjMatrixTrans);
        dataSync.errorRotation = Utils.distMatrices(movingObjMatrixRot, staticObjMatrixRot);
        dataSync.errorRotationAngle = Quaternion.Angle(childMoving.transform.rotation, childStatic.transform.rotation);
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
    void RpcPieceTrainingActiveNow() {
        dataSync.pieceTraining++;
    }

    [Command]
    void CmdPieceTrainingActiveNow() {
        RpcPieceTrainingActiveNow();
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
        if (TestController.tcontrol.sceneIndex == 0) // if it is howtouse scene, after the first piece the setup scene is loaded.
            StartCoroutine(Wait());

        MainController.control.objSelected.Clear();
        CmdClearSelection();

        if (dataSync.pieceTraining < 2) // if user in the training, go to next training piece.
            CmdPieceTrainingActiveNow();
        else
            CmdPieceActiveNow();

        //MainController.control.objSelected.Clear();
        //CmdClearSelection();

        if (dataSync.pieceActiveNow == halfObjects - 1) {
            StartCoroutine(Wait());
        }
    }

    IEnumerator Wait() { // wait until saving the resumed log.

        yield return new WaitForSeconds(2);
        CmdChangeScene();
    }


}

