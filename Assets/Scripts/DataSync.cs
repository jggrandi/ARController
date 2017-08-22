using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
//[NetworkSettings(channel = 0, sendInterval = 0.0f)]
public class DataSync : NetworkBehaviour {




    [SyncVar]
    public int pieceCounter = 0;

    [SyncVar]
    public bool changeScene = false;

    [SyncVar]
    public bool saveResumed = false;

    public SyncListInt Groups = new SyncListInt();
    public SyncListInt piecesList = new SyncListInt();
    public SyncListBool activeState = new SyncListBool(); // sync the active state of the moving pieces (trackedObjects). it is necessary in case of client reconnections 
    public SyncListFloat piecesTimer = new SyncListFloat();

    public List<float> piecesErrorTrans = new List<float>();
    public List<float> piecesErrorRot = new List<float>();
    public List<float> piecesErrorScale = new List<float>();

    //	public SyncListInt posList = new SyncListInt();
    public SyncListInt rotationsList = new SyncListInt();
    public SyncListInt scaleList = new SyncListInt();
    //    public SyncListInt piecesListRedo = new SyncListInt();

    public int GroupCount = 0;
    public GameObject playerObject;

    [SyncVar]
    public int usersConnected = 0;

    public SyncListInt pieceActiveNow = new SyncListInt();
    public SyncListFloat errorTranslation = new SyncListFloat() { 0.0f, 0.0f };
    public SyncListFloat errorRotation = new SyncListFloat() { 0.0f, 0.0f };
    public SyncListFloat errorRotationAngle = new SyncListFloat() { 0.0f, 0.0f };
    public SyncListFloat errorScale = new SyncListFloat() { 0.0f, 0.0f };

    //public int[] vecTransIndex = { 0, 0, 0, 1, 1, 1, 2, 2, 2, 3, 3, 3 };
    //public int[] vecRotAngle = { 45, 45, 45, 90, 90, 90, 45, 45, 45, 90, 90, 90 }; //Crianças, não façam isso em casa.

    public override void OnStartServer() {

        if(TestController.tcontrol.sceneIndex == 0) {
            GameObject trainningObjects = GameObject.Find("Objects").transform.FindChild("TrainningObjects").gameObject;
            Debug.Log(trainningObjects.transform.childCount);
            if (trainningObjects == null) return;
            GameObject trainningGhosts = GameObject.Find("Objects").transform.FindChild("TrainningObjectsGhost").gameObject;
            for (int i = 0; i < trainningObjects.transform.childCount; i++) {
                Groups.Add(-1);
                activeState.Add(true); // true because all trackedObjects start active;
                piecesTimer.Add(0.0f); // add seconds for each piece.
                piecesErrorTrans.Add(0.0f);
                piecesErrorRot.Add(0.0f);
                piecesErrorScale.Add(0.0f);
            }
            List<int> randomizedList = Utils.randomizeVector(trainningGhosts.transform.childCount); // Randomize the blocks order. Store it in an array. 
            listToSyncList(ref randomizedList, ref piecesList); //send to sync list

            randomizedList.Clear();
            randomizedList = Utils.randomizeVector(trainningGhosts.transform.childCount); // Randomize rotations. First half
            listToSyncList(ref randomizedList, ref rotationsList);

            randomizedList.Clear();
            randomizedList = Utils.randomizeVector(trainningGhosts.transform.childCount); // Randomize rotations. First half
            listToSyncList(ref randomizedList, ref scaleList);

        }

        if (TestController.tcontrol.sceneIndex != 0) {

            GameObject taskObjects = GameObject.Find("Objects").transform.FindChild("TaskObjects").gameObject;
            Debug.Log(taskObjects.transform.childCount);
            if (taskObjects == null) return;
            GameObject ghosts = GameObject.Find("Objects").transform.FindChild("TaskObjectsGhost").gameObject;
            Debug.Log(ghosts.transform.childCount);

            for (int i = 0; i < taskObjects.transform.childCount; i++) {
                Groups.Add(-1);
                activeState.Add(true); // true because all trackedObjects start active;
                piecesTimer.Add(0.0f); // add seconds for each piece.
                piecesErrorTrans.Add(0.0f);
                piecesErrorRot.Add(0.0f);
                piecesErrorScale.Add(0.0f);
            }
            List<int> randomizedList = Utils.randomizeVector(ghosts.transform.childCount); // Randomize the blocks order. Store it in an array. 
            listToSyncList(ref randomizedList, ref piecesList); //send to sync list

            randomizedList.Clear();
            randomizedList = Utils.randomizeVector(ghosts.transform.childCount); // Randomize rotations. First half
            listToSyncList(ref randomizedList, ref rotationsList);

            randomizedList.Clear();
            randomizedList = Utils.randomizeVector(ghosts.transform.childCount); // Randomize rotations. First half
            listToSyncList(ref randomizedList, ref scaleList);

        }



    }

    public void listToSyncList(ref List<int> list, ref SyncListInt syncList) {
        syncList.Clear();
        for (int i = 0; i < list.Count; i++) {
            syncList.Add(list[i]);
        }
    }

    public void syncListToList(ref SyncListInt syncList, ref List<int> list) {
        list.Clear();
        for (int i = 0; i < syncList.Count; i++) {
            list.Add(syncList[i]);
        }
    }




}
