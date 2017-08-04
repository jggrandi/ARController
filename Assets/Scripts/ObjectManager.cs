using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectManager : MonoBehaviour {

    public List<GameObject> list;
    public static ObjectManager manager;

    public static GameObject Get(int i) {
        return manager.list[i];
    }
    public static void Set(int i, GameObject obj) {
        manager.list[i] = obj;
    }

    // Use this for initialization
    void Start () {
        
    }

    void Awake() {
        var parent = GameObject.Find("TrackedObjects").transform;

        for (int i = 0; i < parent.childCount; i++) {
            list.Add(parent.GetChild(i).gameObject);
        }
        parent = GameObject.Find("TrainingObjects").transform;
        for (int i = 0; i < parent.childCount; i++) {
            list.Add(parent.GetChild(i).gameObject);
        }
        manager = this;
    }

    // Update is called once per frame
    void Update () {
	
	}
}
