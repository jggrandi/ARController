using UnityEngine;
using System.Collections;

public class StackController : MonoBehaviour {

    GameObject trackedObjects;

	void Start () {
	
        // Randomize the blocks order. Store it in an array. The randomizer have to only choose even values
        // The odd values are the ghost object where the user have to match when docking.
        // Disable all objects.
        

	}
	
	void Update () {
        // Loop into the blockorder array
        // Set active the object with the id stored
        // Set active the object with the id+1 stored;

        // Take the translation and rotation matrices of the MOVING object
        // Take the translation and rotation matrices of the STATIC object

        // Calculate the matrices distances between MOVING and STATIC for tranlation and rotation
    }
}
