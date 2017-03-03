using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
//[NetworkSettings(channel = 0, sendInterval = 0.0f)]
public class DataSync : NetworkBehaviour {

    [SyncVar]
    public int pieceActiveNow = 0;
    [SyncVar]
    public int pieceTraining = 0;
    [SyncVar]
    public bool saveResumed = false;

    public SyncListInt Groups = new SyncListInt();
    public SyncListInt piecesList = new SyncListInt();
	public SyncListInt posList = new SyncListInt();
    public SyncListInt rotationsList = new SyncListInt();
    public SyncListInt piecesListRedo = new SyncListInt();

    public int GroupCount = 0;
    public GameObject playerObject;

    public float errorTranslation;
    public float errorRotation;
	public float errorRotationAngle;

    public int[] vecTransIndex = { 0, 0, 0, 1, 1, 1, 2, 2, 2, 3, 3, 3 };

    public override void OnStartServer() {
        GameObject trackedObjects = GameObject.Find("TrackedObjects");
        for (int i = 0; i< trackedObjects.transform.childCount; i++) {
            Groups.Add(-1);
        }

        GameObject trainingObjects = GameObject.Find("TrainingObjects");
        for (int i = 0; i < trainingObjects.transform.childCount; i++) {
            Groups.Add(-1);
        }

        List<int> randomizedList = Utils.randomizeVector(trackedObjects.transform.childCount/4); // Randomize the blocks order. Store it in an array. /4 because we are dividing the moving pieces in 2 blocks 
        List<int> randomizeSecondPart = Utils.randomizeVector(trackedObjects.transform.childCount / 4); // This second sort is for the second half of pieces

        for (int i = 0; i < randomizeSecondPart.Count; i++)
            randomizeSecondPart[i] = randomizeSecondPart[i] + (trackedObjects.transform.childCount / 4); // to assing correct values of the second half of pieces.

        randomizedList.AddRange(randomizeSecondPart); // concat the first 6 trials with the last 6 trials
        listToSyncList(ref randomizedList, ref piecesList);

        randomizedList.Clear ();
        randomizeSecondPart.Clear();

		randomizedList = Utils.randomizeVector(trackedObjects.transform.childCount/4); // Randomize rotations. First half
        randomizeSecondPart = Utils.randomizeVector(trackedObjects.transform.childCount / 4); // This second sort is for the second half of pieces

        for (int i = 0; i < randomizeSecondPart.Count; i++)
            randomizeSecondPart[i] = randomizeSecondPart[i] + (trackedObjects.transform.childCount / 4); // to assing correct values of the second half of pieces.
        randomizedList.AddRange(randomizeSecondPart); // concat the first 6 rotations index with the last 6 rotations index
        listToSyncList(ref randomizedList, ref rotationsList);

        randomizedList.Clear();
        randomizedList = Utils.randomizeVector(vecTransIndex); // Randomize rotations. Store it in an array.
        listToSyncList(ref randomizedList, ref posList);

    }

    void listToSyncList(ref List<int> list, ref SyncListInt syncList) {
        syncList.Clear();
        for (int i = 0; i < list.Count; i++) {
            syncList.Add(list[i]);
        }
    }




}
