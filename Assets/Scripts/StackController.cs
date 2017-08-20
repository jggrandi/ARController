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

    List<Transform> childMoving = new List<Transform>();
    List<Transform> childStatic = new List<Transform>();

    List<Matrix4x4> movingObjMatrixTrans = new List<Matrix4x4>() { Matrix4x4.identity, Matrix4x4.identity};
    List<Matrix4x4> movingObjMatrixRot = new List<Matrix4x4>() { Matrix4x4.identity, Matrix4x4.identity };

    List<Matrix4x4> staticObjMatrixTrans = new List<Matrix4x4>() { Matrix4x4.identity, Matrix4x4.identity };
    List<Matrix4x4> staticObjMatrixRot = new List<Matrix4x4>() { Matrix4x4.identity, Matrix4x4.identity };

    DataSync dataSync;


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

    void Start() {
        if (!isLocalPlayer) return;

        GameObject handler = GameObject.Find("MainHandler");
        if (handler == null) return;
        dataSync = handler.GetComponent<DataSync>();

        trackedObjects = GameObject.Find("TrackedObjects");
        if (TestController.tcontrol.sceneIndex == 0) return;

        ghosts = GameObject.Find("Ghosts");
        if (ghosts == null) return;

        for (int i = 0; i < dataSync.pieceActiveNow.Count; i++) {
            //trackedObjects.transform.GetChild(dataSync.piecesList[dataSync.pieceActiveNow[i]]).gameObject.SetActive(true); //activate the first piece
            int id = dataSync.piecesList[dataSync.pieceActiveNow[i]];
            ghosts.transform.GetChild(id).gameObject.SetActive(true); //activate the destination (static) piece.
            childMoving.Add(trackedObjects.transform.GetChild(id));
            childStatic.Add(ghosts.transform.GetChild(id).transform);
            
        }
        if (isServer) {
            dataSync.pieceCounter++;
        }
        for (int i = 0; i < dataSync.piecesList.Count; i++) {
            trackedObjects.transform.GetChild(i).gameObject.SetActive(dataSync.activeState[i]); // sync active state of the tracked objects, in case of reconnect...
        }

    }


    void Update() {
        if (!isLocalPlayer) return;
        // if (dataSync.pieceActiveNow == halfObjects) return;
        
        if (isServer && dataSync.changeScene)
            ChangeScene();

        if (TestController.tcontrol.sceneIndex == 0) return;

        for (int i = 0; i < dataSync.piecesList.Count; i++) { //this toggle on or off the visibility of the pieces based on their state.
            int pieceID = dataSync.piecesList[i]; // get the pieceif
            bool active = ghosts.transform.GetChild(pieceID).gameObject.activeSelf; //get the ghost of the piece state
            if (active && !dataSync.pieceActiveNow.Contains(i)) { // if the ghost is active and their moving piece is not in the list of pieces to dock now
                ghosts.transform.GetChild(pieceID).gameObject.SetActive(false); //deactivate their ghost
                trackedObjects.transform.GetChild(pieceID).gameObject.SetActive(false); //and the moving piece
                if (isServer)
                    ChangeActiveState(pieceID, false); //the server keeps the active state of the pieces in case of clients reconnect.
                CmdClearSelection(pieceID); // clear the selection to that piece
            } else if(!active && dataSync.pieceActiveNow.Contains(i)){ // if the ghost is not active and its moving piece is in the list to dock 
                ghosts.transform.GetChild(pieceID).gameObject.SetActive(true); //activate the ghost.
            }
        }

        
        if (!isServer) return; // the server calculate the docking stuff.

        UpdatePiecesTime();


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

            dataSync.piecesErrorTrans[dataSync.piecesList[dataSync.pieceActiveNow[i]]] = dataSync.errorTranslation[i];
            dataSync.piecesErrorRot[dataSync.piecesList[dataSync.pieceActiveNow[i]]] = dataSync.errorRotationAngle[i];
            dataSync.piecesErrorScale[dataSync.piecesList[dataSync.pieceActiveNow[i]]] = dataSync.errorScale[i];

            //if(dataSync.errorTranslation < 0.15f && dataSync.errorRotationAngle < 5.0f && dataSync.errorScale < 0.01f) {
            if (dataSync.errorTranslation[i] < 1.65f && dataSync.errorRotationAngle[i] < 150.0f && dataSync.errorScale[i] < 1.1f) { //relaxed values
                SetNextPiece(i);
            }
        }

    }

    
    public void SetNextPiece(int index) {

        CmdIncrementPieceCounter();

        if (dataSync.pieceCounter == dataSync.piecesList.Count)
            ChangeScene();
        Debug.Log(dataSync.piecesList[dataSync.pieceCounter]);

        CmdSetEnabledObject(index, dataSync.pieceCounter);

    }




    void UpdatePiecesTime() {
        List<int> tempPiecesToUpdate = new List<int>();
        foreach (var player in GameObject.FindGameObjectsWithTag("player")) {
            foreach(int pieceSelected in player.GetComponent<Lean.Touch.NetHandleSelectionTouch>().objSelectedShared) {
                if (!tempPiecesToUpdate.Contains(pieceSelected))
                    tempPiecesToUpdate.Add(pieceSelected);
            }
        }
        //foreach (var piece in tempPiecesToUpdate)
        //    Debug.Log(piece);
        for (int i = 0; i < tempPiecesToUpdate.Count; i++)
            dataSync.piecesTimer[tempPiecesToUpdate[i]] += Time.deltaTime;
        //    tempPiecesToUpdate[i] += Time.deltaTime;
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


    
    void ChangeScene() {
        //RpcIncrementSceneID();
        TestController.tcontrol.sceneIndex++;
        MyNetworkManager.singleton.ServerChangeScene("SetupTest");
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


    [Command]
    void CmdSaveResumed() {
        GameObject.Find("MainHandler").gameObject.transform.GetComponent<HandleLog>().SaveResumed();
    }

}

