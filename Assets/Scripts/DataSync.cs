using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class DataSync : NetworkBehaviour {

    [SyncVar]
    public int pieceActiveNow = 0;
    
    public int pieceTraining = 0;

    public SyncListInt Groups = new SyncListInt();
    public SyncListInt piecesList = new SyncListInt();
	public SyncListInt distancesList = new SyncListInt();
    public SyncListInt rotationsList = new SyncListInt();
    public List<int> piecesListRedo = new List<int>();

    public int GroupCount = 0;
    public GameObject playerObject;

    public float errorTranslation;
    public float errorRotation;
	public float errorRotationAngle;

    public int[] vecRotIndex = { 0, 0, 0, 1, 1, 1, 2, 2, 2, 3, 3, 3 };

    public override void OnStartServer() {
        GameObject trackedObjects = GameObject.Find("TrackedObjects");
        for (int i = 0; i< trackedObjects.transform.childCount; i++) {
            Groups.Add(-1);
        }

        GameObject trainingObjects = GameObject.Find("TrainingObjects");
        for (int i = 0; i < trainingObjects.transform.childCount; i++) {
            Groups.Add(-1);
        }

        List<int> randomizedList = Utils.randomizeVector(trackedObjects.transform.childCount/2); // Randomize the blocks order. Store it in an array.
        listToSyncList(ref randomizedList, ref piecesList);

		randomizedList.Clear ();
		randomizedList = Utils.randomizeVector(trackedObjects.transform.childCount/2); // Randomize distances. Store it in an array.
		listToSyncList(ref randomizedList, ref distancesList);

        randomizedList.Clear();
        randomizedList = Utils.randomizeVector(vecRotIndex); // Randomize rotations. Store it in an array.
        listToSyncList(ref randomizedList, ref rotationsList);



    }

    void listToSyncList(ref List<int> list, ref SyncListInt syncList) {
        syncList.Clear();
        for (int i = 0; i < list.Count; i++) {
            syncList.Add(list[i]);
        }
    }




}
