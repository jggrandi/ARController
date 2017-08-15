using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectManager : MonoBehaviour {

    public List<GameObject> list;
    public static ObjectManager manager;

    public GameObject trackedObjects;
    Transform parent;

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

        
        trackedObjects = GameObject.Find("TrackedObjects");
        if (TestController.tcontrol.sceneIndex == 0)
             parent = GameObject.Find("HowToUseObjects").transform;

        int count = parent.childCount;
        for (int i = 0; i < count; i++) {
            GameObject obj = parent.GetChild(0).transform.gameObject;
            obj.AddComponent<ObjectGroupId>();
            obj.GetComponent<ObjectGroupId>().index = i;
            list.Add(parent.GetChild(0).gameObject);
            parent.GetChild(0).transform.parent = trackedObjects.transform;

        }

        manager = this;
    }

    // Update is called once per frame
    void Update () {
	
	}
}
