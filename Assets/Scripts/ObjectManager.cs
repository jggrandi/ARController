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

        trackedObjects = GameObject.Find("TrackedObjects");
        GameObject pai = GameObject.Find("Objects");
        if (TestController.tcontrol.sceneIndex == 0)
            parent = pai.transform.Find("HowToUseObjects").transform;
        else
            parent = pai.transform.Find("TaskObjects").transform;

        if (TestController.tcontrol.sceneIndex == 2)
            GameObject.Find("Objects").transform.Find("WallTask2").gameObject.SetActive(true);
        else if (TestController.tcontrol.sceneIndex == 3)
            GameObject.Find("Objects").transform.Find("WallTask3").gameObject.SetActive(true);


        int count = parent.childCount;
        for (int i = 0; i < count; i++) {
            GameObject obj = parent.GetChild(0).transform.gameObject;
            obj.AddComponent<ObjectGroupId>();
            obj.GetComponent<ObjectGroupId>().index = i;
            list.Add(parent.GetChild(0).gameObject);
            parent.GetChild(0).transform.parent = trackedObjects.transform;

        }

        //GameObject aleatoryObjects = pai.transform.Find("AleatoryObjects").gameObject;

        //for(int i = 0; i< aleatoryObjects.transform.childCount; i++) {
        //    list.Add(aleatoryObjects.transform.GetChild(0).gameObject);
        //    aleatoryObjects.transform.GetChild(0).transform.parent = trackedObjects.transform;
        //}


        manager = this;
    }

    // Update is called once per frame
    void Update () {
	
	}
}
