using UnityEngine;
using System.Collections;

public class HandleMultitargetsObjectsPosition : MonoBehaviour
{
    
    public GameObject trackedObjects;
    public GameObject imageTargets;
    
    void Start(){
        MainController.control.totalTargets = imageTargets.transform.childCount;
        Debug.Log(imageTargets.transform.childCount);
    }

    // Update is called once per frame
    void Update(){
        
        Vector3 p = Vector3.zero;
        Quaternion r = Quaternion.identity;
        
        for (int i = 0; i < imageTargets.transform.childCount; i++)
        {
            if (trackedObjects.activeInHierarchy)
            {
                trackedObjects.transform.position = Vector3.Lerp(trackedObjects.transform.position, imageTargets.transform.GetChild(i).transform.position, 0.5f);
                trackedObjects.transform.rotation = Quaternion.Slerp(trackedObjects.transform.rotation, imageTargets.transform.GetChild(i).transform.rotation, 0.5f);
            }
            //if (cubo2.gameObject.GetComponent<MeshRenderer>().enabled)
            //{
            //    cubo1.gameObject.transform.position = Vector3.Lerp(cubo1.gameObject.transform.position, cubo2.gameObject.transform.position, 0.5f);
            //    cubo1.gameObject.transform.rotation = Quaternion.Slerp(cubo1.gameObject.transform.rotation, cubo2.gameObject.transform.rotation, 0.5f);
            //}
            //if (cubo3.gameObject.GetComponent<MeshRenderer>().enabled)
            //{
            //    cubo1.gameObject.transform.position = Vector3.Lerp(cubo1.gameObject.transform.position, cubo3.gameObject.transform.position, 0.5f);
            //    cubo1.gameObject.transform.rotation = Quaternion.Slerp(cubo1.gameObject.transform.rotation, cubo3.gameObject.transform.rotation, 0.5f);
            //}


        }
    }
}