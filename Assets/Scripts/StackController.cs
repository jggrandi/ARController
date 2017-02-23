using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class StackController : NetworkBehaviour {

    public GameObject playerObject;

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
	bool redoList = false;

    int prevPiece = -1;


    bool showErrorTraining2 = false;

    public IEnumerator showError() {

        showErrorTraining2 = true;

        
        yield return new WaitForSeconds(1.0f);
        CmdPieceTrainingActiveNow();
        showErrorTraining2 = false;

    }
    GUIStyle titleStyle = new GUIStyle();
    GUIStyle titleStyle2 = new GUIStyle();

    void OnGUI() {
    
        titleStyle.fontSize = 50;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.alignment = TextAnchor.MiddleCenter;


        
        titleStyle2.fontSize = 50;
        titleStyle2.fontStyle = FontStyle.Bold;
        titleStyle2.alignment = TextAnchor.MiddleCenter;

        if (dataSync.errorRotationAngle < 3.0f)
            titleStyle.normal.textColor = Color.green;
        else
            titleStyle.normal.textColor = Color.white;

        if (dataSync.errorTranslation < 0.13f)
            titleStyle2.normal.textColor = Color.green;
        else
            titleStyle2.normal.textColor = Color.white;


        if (dataSync.pieceTraining == 0 && TestController.tcontrol.sceneIndex != 0) {
            GUI.Label(new Rect(Screen.width / 2, 40, 50, 50), "Diff Angle: " + dataSync.errorRotationAngle.ToString("F2") , titleStyle);
            GUI.Label(new Rect(Screen.width / 2, 80, 50, 50), "Diff Pos:   " + dataSync.errorTranslation.ToString("F2"), titleStyle2);
        }

        if(dataSync.pieceTraining == 1) {
            if (showErrorTraining2) {
                GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");
                GUI.Label(new Rect(Screen.width / 2, 40, 50, 50), "Diff Angle: " + dataSync.errorRotationAngle.ToString("F2"), titleStyle);
                GUI.Label(new Rect(Screen.width / 2, 80, 50, 50), "Diff Pos:   " + dataSync.errorTranslation.ToString("F2"), titleStyle2);
            }
        }
    }


	void setSpawnPos(int id){
        Debug.Log(id);
		float x = TestController.tcontrol.spawnDistances[dataSync.posList[id] * 3];
		float y = TestController.tcontrol.spawnDistances[dataSync.posList[id] * 3 + 1];
		float z = TestController.tcontrol.spawnDistances[dataSync.posList[id] * 3 + 2];
		trackedObjects.transform.GetChild(id).transform.position = new Vector3(x*0.1f, y, z * 0.1f);
	}

	void setSpawnRot(int id){
		float rx = TestController.tcontrol.spawnRotations[dataSync.rotationsList[id] * 4];
		float ry = TestController.tcontrol.spawnRotations[dataSync.rotationsList[id] * 4 + 1];
		float rz = TestController.tcontrol.spawnRotations[dataSync.rotationsList[id] * 4 + 2];
		float rw = TestController.tcontrol.spawnRotations[dataSync.rotationsList[id] * 4 + 3];
		trackedObjects.transform.GetChild(id).transform.rotation = new Quaternion(rx, ry, rz, rw);
	}


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
				setSpawnPos (i);
				setSpawnRot (i);
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


    public int disableObject = -1;
    public int enableObject = -1;

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

            if(disableObject >= 0) {
                Debug.Log("disable: "+disableObject);
                trackedObjects.transform.GetChild(dataSync.piecesList[disableObject]).gameObject.SetActive(false); // disable the previous object
                trackedObjects.transform.GetChild(dataSync.piecesList[disableObject] + halfObjects).gameObject.SetActive(false); //and its ghost
                disableObject = -1;
            }
            if (enableObject >= 0) {
                Debug.Log("enable: " + enableObject);
                trackedObjects.transform.GetChild(dataSync.piecesList[enableObject]).gameObject.SetActive(true);
                trackedObjects.transform.GetChild(dataSync.piecesList[enableObject] + halfObjects).gameObject.SetActive(true);
                enableObject = -1;
            }
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
	void RpcChangePieceActiveNow(int id) {
		dataSync.pieceActiveNow = id;
	}

	[Command]
	void CmdChangePieceActiveNow(int id) {
		RpcChangePieceActiveNow(id);
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

    [ClientRpc]
    void RpcAddToRedo() {
        dataSync.piecesListRedo.Add(dataSync.pieceActiveNow);
    }

    [Command]
    void CmdAddToRedo() {
        RpcAddToRedo();
    }

    [ClientRpc]
    void RpcARemoveFromRedo(int id) {
		dataSync.piecesListRedo.RemoveAt (id);
    }

    [Command]
    void CmdRemoveFromRedo(int id) {
        RpcARemoveFromRedo(id);
    }
    
    [ClientRpc]
    void RpcDeactivatePiece(int id) {
        trackedObjects.transform.GetChild(id).gameObject.SetActive(false); // disable the previous object
    }

    [Command]
    void CmdDeactivatePiece(int id) {
        RpcDeactivatePiece(id);
    }

    [ClientRpc]
    void RpcSpawnPos(int id) {
        setSpawnPos(id);
        setSpawnRot(id);
    }

    [Command]
    void CmdSpawnPos(int id) {
        RpcSpawnPos(id);
    }

    public void SetNextPiece() {

        if (TestController.tcontrol.sceneIndex == 0) // if it is howtouse scene, after the first piece the setup scene is loaded.
            StartCoroutine(Wait());

        MainController.control.objSelected.Clear();
        CmdClearSelection();

        if (dataSync.pieceTraining < 2) { // if user in the training, go to next training piece.
            
            if (dataSync.pieceTraining == 1) {
                StartCoroutine(showError());
            } else {
                CmdPieceTrainingActiveNow();
            }
            enableObject = 0;
        } else {


            if ((dataSync.errorTranslation > 0.15f || dataSync.errorRotationAngle > 15.0f) && !redoList) // if the piece is coarse docked.. add it to redo list.
                CmdAddToRedo();

            Debug.Log(halfObjects);

            if (dataSync.pieceActiveNow == halfObjects - 1 && !redoList) {
                Debug.Log("Active redoList");
                redoList = true;
            }

            if(redoList && dataSync.piecesListRedo.Count <= 0) {
                Debug.Log("End");
                StartCoroutine(Wait());
                return;
            }

            disableObject = dataSync.pieceActiveNow;
            if (!redoList) {
                Debug.Log("Increment enableObject");
                enableObject = dataSync.pieceActiveNow+1;
            }else {
                Debug.Log("Restore piecesListRedo[0]");
                enableObject = dataSync.piecesListRedo[0];
                CmdRemoveFromRedo(0);

                //CmdSpawnPos(dataSync.piecesList[dataSync.piecesListRedo[0]]); //reset the initial piece position
            }

            CmdChangePieceActiveNow(enableObject);
            
        }
    }

    IEnumerator Wait() { // wait until saving the resumed log.

        yield return new WaitForSeconds(2);
        CmdChangeScene();
    }


}

