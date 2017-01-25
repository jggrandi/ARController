using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class DataSync : NetworkBehaviour {

    [SyncVar]
    public int pieceActiveNow = 0;

    public SyncListInt Groups = new SyncListInt();
    public SyncListInt piecesList = new SyncListInt();
    public int GroupCount = 0;
    public GameObject playerObject;

 
    
    public override void OnStartServer() {
        GameObject trackedObjects = GameObject.Find("TrackedObjects");
        for (int i = 0; i< trackedObjects.transform.childCount; i++) {
            Groups.Add(-1);
        }
        randomizeVector(trackedObjects.transform.childCount/2); // Randomize the blocks order. Store it in an array. The randomizer have to only choose even values
    }

    void randomizeVector(int size) {

        //var randomNumbers = new int[size];
        var numbers = new List<int>(size);


        for (int i = 0; i < size; i++)
            numbers.Add(i);

        for (int i = 0; i < size; i++) {
            var thisNumber = Random.Range(0, numbers.Count);
            piecesList.Add(numbers[thisNumber]);
            numbers.RemoveAt(thisNumber);
        }
        //return randomNumbers;
    }


}
