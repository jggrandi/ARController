using UnityEngine;
using System.Collections;
using System.IO;

public class CalculateDistancesForTest : MonoBehaviour {

    GameObject sphereRef;
    GameObject sphereMoving;
    StreamWriter fPositions;
    float dist;

    
    void Start () {
        sphereRef = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphereMoving = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        GameObject g = GameObject.Find("TrackedObjects");
        sphereRef.transform.parent = g.transform;
        sphereMoving.transform.parent = g.transform;

        sphereRef.transform.position = new Vector3(0.0f, 0.5f, 0.0f);
        sphereRef.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
        //        sphereRef.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);
        //        sphereRef.transform.localScale = new Vector3(4.5f, 4.5f, 4.5f);

        sphereMoving.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        dist = sphereRef.transform.localScale.x / 2;

        fPositions = File.CreateText(Application.persistentDataPath + "/Positions-Dist" + dist + ".csv");

    }
	
	// Update is called once per frame
	void Update () {
        dist = sphereRef.transform.localScale.x / 2;
        Vector3 diff = sphereMoving.transform.position - sphereRef.transform.position;
        sphereMoving.transform.position = sphereRef.transform.position + diff.normalized * dist;



        if (Input.GetKeyDown(KeyCode.A)) {
            Debug.Log("Saving...");
            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            g.transform.position = sphereMoving.transform.position;
            g.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            string line = "";
            line = sphereMoving.transform.position.x + ";" + sphereMoving.transform.position.y + ";" + sphereMoving.transform.position.z + ";";
            fPositions.WriteLine(line);
            fPositions.Flush();
        }
    }

    void OnApplicationQuit() {
        fPositions.Close();

    }
}
