using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

//[NetworkSettings(channel = 0, sendInterval = 0.0f)]

public class StackController : NetworkBehaviour {

    GameObject trackedObjects;
    GameObject howToUseObjects;

    GameObject ghosts;

    //int[] objectsOrder;

    //public int halfObjects;
    //public int halfTrainingObjects;
    public List<Transform> childMoving = new List<Transform>();
    public List<Transform> childStatic = new List<Transform>();

    List<Matrix4x4> movingObjMatrixTrans = new List<Matrix4x4>() { Matrix4x4.identity, Matrix4x4.identity};
    List<Matrix4x4> movingObjMatrixRot = new List<Matrix4x4>() { Matrix4x4.identity, Matrix4x4.identity };

    List<Matrix4x4> staticObjMatrixTrans = new List<Matrix4x4>() { Matrix4x4.identity, Matrix4x4.identity };
    List<Matrix4x4> staticObjMatrixRot = new List<Matrix4x4>() { Matrix4x4.identity, Matrix4x4.identity };



    DataSync dataSync;
	bool redoList = false;

    bool showErrorTraining2 = false;

    public IEnumerator showError() {

        showErrorTraining2 = true;

        
        yield return new WaitForSeconds(10.0f);
        CmdPieceTrainingActiveNow();
        showErrorTraining2 = false;

    }
    GUIStyle titleStyle = new GUIStyle();
    GUIStyle titleStyle2 = new GUIStyle();

    void OnGUI() {

        //GameObject handler = GameObject.Find("MainHandler");
        //if (handler == null) return;

        //titleStyle.fontSize = 50;
        //titleStyle.fontStyle = FontStyle.Bold;
        //titleStyle.alignment = TextAnchor.MiddleCenter;


        
        //titleStyle2.fontSize = 50;
        //titleStyle2.fontStyle = FontStyle.Bold;
        //titleStyle2.alignment = TextAnchor.MiddleCenter;

        //if (dataSync.errorRotationAngle < 3.0f)
        //    titleStyle.normal.textColor = Color.green;
        //else
        //    titleStyle.normal.textColor = Color.white;

        //if (dataSync.errorTranslation < 0.13f)
        //    titleStyle2.normal.textColor = Color.green;
        //else
        //    titleStyle2.normal.textColor = Color.white;


        //if (dataSync.pieceTraining == 0 && TestController.tcontrol.sceneIndex != 0) {
        //    GUI.Label(new Rect(Screen.width / 2, 40, 50, 50), "Diff Angle: " + dataSync.errorRotationAngle.ToString("F2") , titleStyle);
        //    GUI.Label(new Rect(Screen.width / 2, 80, 50, 50), "Diff Pos:   " + dataSync.errorTranslation.ToString("F2"), titleStyle2);
        //}

        //if(dataSync.pieceTraining == 1) {
        //    if (showErrorTraining2) {
        //        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");
        //        GUI.Label(new Rect(Screen.width / 2, 40, 50, 50), "Diff Angle: " + dataSync.errorRotationAngle.ToString("F2"), titleStyle);
        //        GUI.Label(new Rect(Screen.width / 2, 80, 50, 50), "Diff Pos:   " + dataSync.errorTranslation.ToString("F2"), titleStyle2);
        //    }
        //}
    }


    //void setSpawnPos(int id){
    //	float x = TestController.tcontrol.spawnDistances[dataSync.posList[id] * 3];
    //	float y = TestController.tcontrol.spawnDistances[dataSync.posList[id] * 3 + 1];
    //	float z = TestController.tcontrol.spawnDistances[dataSync.posList[id] * 3 + 2];
    //       trackedObjects.transform.GetChild(id).transform.position = new Vector3(x, y, z);
    //}

    //void setSpawnRot(int id){
    //	float rx = TestController.tcontrol.spawnRotations[dataSync.rotationsList[id] * 4];
    //	float ry = TestController.tcontrol.spawnRotations[dataSync.rotationsList[id] * 4 + 1];
    //	float rz = TestController.tcontrol.spawnRotations[dataSync.rotationsList[id] * 4 + 2];
    //	float rw = TestController.tcontrol.spawnRotations[dataSync.rotationsList[id] * 4 + 3];
    //	trackedObjects.transform.GetChild(id).transform.rotation = new Quaternion(rx, ry, rz, rw);
    //}


    void Start() {
        if (!isLocalPlayer) return;

        GameObject handler = GameObject.Find("MainHandler");
        trackedObjects = GameObject.Find("TrackedObjects");

        if (TestController.tcontrol.sceneIndex == 0) return;

        ghosts = GameObject.Find("Ghosts");
        
        if (ghosts == null) return;

        if (handler == null) return;
        dataSync = handler.GetComponent<DataSync>();


        for (int i = 0; i < dataSync.pieceActiveNow.Count; i++) {
            //trackedObjects.transform.GetChild(dataSync.piecesList[dataSync.pieceActiveNow[i]]).gameObject.SetActive(true); //activate the first piece
            int id = dataSync.piecesList[dataSync.pieceActiveNow[i]];
            ghosts.transform.GetChild(id).gameObject.SetActive(true); //activate the destination (static) piece.
            childMoving.Add(trackedObjects.transform.GetChild(id));
            childStatic.Add(ghosts.transform.GetChild(id).transform);
            
        }
        if (isServer)
            dataSync.pieceCounter++;
        for (int i = 0; i < dataSync.piecesList.Count; i++) {
            trackedObjects.transform.GetChild(dataSync.piecesList[i]).gameObject.SetActive(dataSync.activeState[i]); // sync active state of the tracked objects, in case of reconnect...
        }


        //        for (int i = trackedObjects.transform.childCount; i < trainingObjects.transform.childCount + trackedObjects.transform.childCount; i++) {

        //        GameObject obj = trainingObjects.transform.GetChild(count).transform.gameObject;
        //        obj.AddComponent<ObjectGroupId>();
        //        //obj.GetComponent<ObjectGroupId>().material = obj.GetComponent<Renderer>().material;
        //        obj.GetComponent<ObjectGroupId>().index = i;
        //        count++;
        //    }

        //    halfObjects = trackedObjects.transform.childCount / 2; // The objs/2 values are the moving objects. -2 to discard the training pieces in the end
        //    if (TestController.tcontrol.sceneIndex != 0) { // if it is not the howtouse scene
        //        for (int i = 0; i < halfObjects; i++) {
        //setSpawnPos (i);
        //setSpawnRot (i);
        //        }

        //        int index = trainingObjects.transform.childCount;
        //        for (int i = 0; i < index; i++) {
        //            trainingObjects.transform.GetChild(0).transform.parent = trackedObjects.transform;
        //        }
        //    }


        //foreach (Transform child in trackedObjects.transform) // Disable all objects.
        //    child.gameObject.SetActive(false);

        //if (TestController.tcontrol.sceneIndex == 0) // if it is the howtouse scene, activate only one piece without their ghost
        //    trackedObjects.transform.GetChild(0).gameObject.SetActive(true);

        //if (TestController.tcontrol.sceneIndex != 0 && dataSync.pieceTraining == 0) { // if is not the howtouse scene and is the first piece, activate the first tranning piece.
        //    trackedObjects.transform.GetChild(trackedObjects.transform.childCount - 4).gameObject.SetActive(true); // activate the moving object 
        //    trackedObjects.transform.GetChild(trackedObjects.transform.childCount - 3).gameObject.SetActive(true); // and its ghost, here the ghost is the next piece
        //    childMoving = trackedObjects.transform.GetChild(trackedObjects.transform.childCount - 4); // take the moving object 
        //    childStatic = trackedObjects.transform.GetChild(trackedObjects.transform.childCount - 3); // and its ghost
        //}
    }

    void checkIfUsersFinished() {
        if (dataSync.usersDone.Count == 0) return;
        if (!isServer) return;
        //if (dataSync.usersConnected - 1 == dataSync.usersDone.Count) { // -1 because the server counts as a connected player
        if (dataSync.usersDone.Count == 2) { //hard coded for 2 players.. it will not work for players != 2 
            CmdChangeScene();
        }
    }


    void Update() {
        if (!isLocalPlayer) return;
        // if (dataSync.pieceActiveNow == halfObjects) return;
        if (TestController.tcontrol.sceneIndex == 0) { // if it is the how to use scene
            checkIfUsersFinished();
            return;
        }




        //if (dataSync.pieceTraining == 1) {
        //    trackedObjects.transform.GetChild(trackedObjects.transform.childCount - 4).gameObject.SetActive(false);
        //    trackedObjects.transform.GetChild(trackedObjects.transform.childCount - 3).gameObject.SetActive(false);

        //    trackedObjects.transform.GetChild(trackedObjects.transform.childCount - 2).gameObject.SetActive(true);
        //    trackedObjects.transform.GetChild(trackedObjects.transform.childCount - 1).gameObject.SetActive(true);
        //    childMoving = trackedObjects.transform.GetChild(trackedObjects.transform.childCount - 2); // take the moving object 
        //    childStatic = trackedObjects.transform.GetChild(trackedObjects.transform.childCount - 1); // and its ghost
        //} else if (dataSync.pieceTraining == 0) { //if user is in the training mode


        //    trackedObjects.transform.GetChild(trackedObjects.transform.childCount - 4).gameObject.SetActive(true);
        //    trackedObjects.transform.GetChild(trackedObjects.transform.childCount - 3).gameObject.SetActive(true);


        //    childMoving = trackedObjects.transform.GetChild(trackedObjects.transform.childCount - 4); // take the moving object 
        //    childStatic = trackedObjects.transform.GetChild(trackedObjects.transform.childCount - 3); // and its ghost

        //} else { // if user is not in the training mode

        //    trackedObjects.transform.GetChild(trackedObjects.transform.childCount - 2).gameObject.SetActive(false);
        //    trackedObjects.transform.GetChild(trackedObjects.transform.childCount - 1).gameObject.SetActive(false);

        //        childMoving = trackedObjects.transform.GetChild(dataSync.piecesList[dataSync.pieceActiveNow]); // take the moving object 
        //        childStatic = ghosts.transform.GetChild(dataSync.piecesList[dataSync.pieceActiveNow]); // and its ghost

        //for (int i = 0; i < ghosts.transform.childCount; i++) {
        //    bool activeState = ghosts.transform.GetChild(i).gameObject.activeSelf;
        //    if (i == dataSync.pieceActiveNow && activeState == false) {

        //        //trackedObjects.transform.GetChild(dataSync.piecesList[i]).gameObject.SetActive(true);
        //        ghosts.transform.GetChild(dataSync.piecesList[i]).gameObject.SetActive(true);

        //    } else if (i != dataSync.pieceActiveNow && activeState == true) {

        //        trackedObjects.transform.GetChild(dataSync.piecesList[i]).gameObject.SetActive(false); // disable the previous object
        //        ghosts.transform.GetChild(dataSync.piecesList[i]).gameObject.SetActive(false); //and its ghost

        //        // NEED TO DESELECT THE PIECE FOR ALL USERS

        //        //setSpawnPos(i);
        //        //setSpawnRot(i);

        //    }
        //}

        for (int i = 0; i < dataSync.piecesList.Count; i++) {
            int pieceID = dataSync.piecesList[i];
            bool active = ghosts.transform.GetChild(pieceID).gameObject.activeSelf;
            if(active && !dataSync.pieceActiveNow.Contains(i)) {
                ghosts.transform.GetChild(pieceID).gameObject.SetActive(false);
                trackedObjects.transform.GetChild(pieceID).gameObject.SetActive(false);
                if (isServer)
                    ChangeActiveState(pieceID, false);
                CmdClearSelection(pieceID);
            } else if(!active && dataSync.pieceActiveNow.Contains(i)){
                ghosts.transform.GetChild(pieceID).gameObject.SetActive(true);
            }

        }

        if (!isServer) return;

        for (int i = 0; i < dataSync.pieceActiveNow.Count; i++) {
            childMoving[i] = trackedObjects.transform.GetChild(dataSync.piecesList[dataSync.pieceActiveNow[i]]); // take the moving object 
            childStatic[i] = ghosts.transform.GetChild(dataSync.piecesList[dataSync.pieceActiveNow[i]]); // and its ghost

            movingObjMatrixTrans[i] = Matrix4x4.TRS(childMoving[i].transform.position, Quaternion.identity, new Vector3(1.0f, 1.0f, 1.0f));
            movingObjMatrixRot[i] = Matrix4x4.TRS(new Vector3(0, 0, 0), childMoving[i].transform.rotation, new Vector3(1.0f, 1.0f, 1.0f));


            staticObjMatrixTrans[i] = Matrix4x4.TRS(childStatic[i].transform.position, Quaternion.identity, new Vector3(1.0f, 1.0f, 1.0f));
            staticObjMatrixRot[i] = Matrix4x4.TRS(new Vector3(0, 0, 0), childStatic[i].transform.rotation, new Vector3(1.0f, 1.0f, 1.0f));

            dataSync.errorTranslation[i] = Utils.distMatrices(movingObjMatrixTrans[i], staticObjMatrixTrans[i]);
            dataSync.errorRotation[i] = Utils.distMatrices(movingObjMatrixRot[i], staticObjMatrixRot[i]);
            dataSync.errorRotationAngle[i] = Quaternion.Angle(childMoving[i].transform.rotation, childStatic[i].transform.rotation);
            dataSync.errorScale[i] = Mathf.Abs(childMoving[i].localScale.x - childStatic[i].localScale.x);

            //if(dataSync.errorTranslation < 0.15f && dataSync.errorRotationAngle < 5.0f && dataSync.errorScale < 0.01f) {
            if (dataSync.errorTranslation[i] < 0.65f && dataSync.errorRotationAngle[i] < 15.0f && dataSync.errorScale[i] < 0.1f) { //relaxed values
                SetNextPiece(i);
            }
        }

    }

    
    public void SetNextPiece(int index) {

        


        //childMoving = null;
        //childStatic = null;

        //trackedObjects.transform.GetChild(dataSync.piecesList[dataSync.pieceActiveNow]).gameObject.SetActive(false); // disable the previous object
        //ghosts.transform.GetChild(dataSync.piecesList[dataSync.pieceActiveNow]).gameObject.SetActive(false); //and its ghost

        CmdIncrementPieceCounter();

        //dataSync.pieceActiveNow[index] = dataSync.pieceCounter;

        if (dataSync.pieceCounter == dataSync.piecesList.Count)
            CmdChangeScene();
        Debug.Log(dataSync.piecesList[dataSync.pieceCounter]);

        CmdSetEnabledObject(index, dataSync.pieceCounter);



        //childMoving = trackedObjects.transform.GetChild(dataSync.piecesList[dataSync.pieceActiveNow]); // take the moving object 
        //childStatic = ghosts.transform.GetChild(dataSync.piecesList[dataSync.pieceActiveNow]); // and its ghost

        //ghosts.transform.GetChild(dataSync.piecesList[dataSync.pieceActiveNow]).gameObject.SetActive(true); //and its ghost

        //if (TestController.tcontrol.sceneIndex == 0) // if it is howtouse scene, after the first piece the setup scene is loaded.
        //    CmdChangeScene();

        //gameObject.GetComponent<Lean.Touch.NetHandleSelectionTouch>().objSelected.Clear();
        //CmdClearSelection();

        //CmdSaveResumed();

        //if (dataSync.pieceTraining < 2) { // if user in the training, go to next training piece.

        //    if (dataSync.pieceTraining == 1) {
        //        StartCoroutine(showError());
        //    } else {
        //        CmdPieceTrainingActiveNow();
        //    }
        //    CmdSetEnabledObject(0);
        //} else {

        //    if ((dataSync.errorTranslation > 0.15f || dataSync.errorRotationAngle > 15.0f) && !redoList) // if the piece is coarse docked.. add it to redo list.
        //        CmdAddToRedo();

        //    if (dataSync.pieceActiveNow == halfObjects - 1 && !redoList) {
        //        redoList = true;
        //    }

        //    if(redoList && dataSync.piecesListRedo.Count <= 0) {
        //        CmdChangeScene();
        //        return;
        //    }

        //    if (!redoList) {
        //        CmdSetEnabledObject(dataSync.pieceActiveNow+1);
        //    }else {
        //        CmdSetEnabledObject(dataSync.piecesListRedo[0]);
        //        CmdRemoveFromRedo(0);

        //    }
        //}
    }

    [Command]
    void CmdIncrementPieceCounter() {
        dataSync.pieceCounter++;
    }

    //[Command]
    void ChangeActiveState(int index,bool state) {
        dataSync.activeState[index] = state;
    }

    [Command]
    void CmdSetEnabledObject(int id, int value) {
        dataSync.pieceActiveNow[id] = value;
    }

    [ClientRpc]
    void RpcIncrementSceneID() {
        TestController.tcontrol.sceneIndex++;
    }

    [Command]
    void CmdChangeScene() {
        //RpcIncrementSceneID();
        TestController.tcontrol.sceneIndex++;
        MyNetworkManager.singleton.ServerChangeScene("SetupTest");
    }

    [ClientRpc]
    void RpcPieceTrainingActiveNow() {
        //        dataSync.pieceTraining++;
    }

    [Command]
    void CmdPieceTrainingActiveNow() {
        RpcPieceTrainingActiveNow();
    }

    [ClientRpc]
    void RpcClearSelection(int id) {
        foreach (var player in GameObject.FindGameObjectsWithTag("player")) {
            if (player.GetComponent<Lean.Touch.NetHandleSelectionTouch>().objSelected.Contains(id)) {
                player.GetComponent<Lean.Touch.NetHandleSelectionTouch>().objSelected.Clear();
                player.GetComponent<Lean.Touch.NetHandleSelectionTouch>().objSelectedShared.Clear();
            }
        }

    }

    [Command]
    void CmdClearSelection(int id) {
        foreach (var player in GameObject.FindGameObjectsWithTag("player")) {
            if (player.GetComponent<Lean.Touch.NetHandleSelectionTouch>().objSelected.Contains(id)) {
                player.GetComponent<Lean.Touch.NetHandleSelectionTouch>().objSelected.Clear();
                player.GetComponent<Lean.Touch.NetHandleSelectionTouch>().objSelectedShared.Clear();
            }
        }
        RpcClearSelection(id);
    }

    //void ClearSelection(int id) {
    //    this.gameObject.GetComponent<Lean.Touch.NetHandleSelectionTouch>().objSelected.Clear();
    //}

    [Command]
    void CmdAddToRedo() {
        //        dataSync.piecesListRedo.Add(dataSync.pieceActiveNow);
    }

    [Command]
    void CmdRemoveFromRedo(int id) {
        //        dataSync.piecesListRedo.RemoveAt(id);
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
        //        setSpawnPos(id);
        //        setSpawnRot(id);
    }

    [Command]
    void CmdSpawnPos(int id) {
        RpcSpawnPos(id);
    }

    [Command]
    void CmdSaveResumed() {
        GameObject.Find("MainHandler").gameObject.transform.GetComponent<HandleLog>().SaveResumed();
    }

}

