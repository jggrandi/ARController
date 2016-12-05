using UnityEngine;
using System.Collections;

public class HandleMultitargetsObjectsPosition : MonoBehaviour
{
    
    public GameObject trackedObjects;
    public GameObject imageTargets;
    
    void Start(){
        MainController.control.totalTargets = imageTargets.transform.childCount;
        //Debug.Log(imageTargets.transform.childCount);
    }

    // Update is called once per frame
    void Update(){

        //Debug.Log(trackedObjects.transform.childCount);

        for (int i = 0; i < imageTargets.transform.childCount; i++){
            if (trackedObjects.activeInHierarchy){
                if (imageTargets.transform.GetChild(i).GetChild(0).gameObject.activeInHierarchy) {
                    trackedObjects.transform.position = Vector3.Lerp(trackedObjects.transform.position, imageTargets.transform.GetChild(i).GetChild(0).transform.position, 0.5f);
                    trackedObjects.transform.rotation = Quaternion.Slerp(trackedObjects.transform.rotation, imageTargets.transform.GetChild(i).GetChild(0).transform.rotation, 0.5f);
                }
            }
        }
    }
}