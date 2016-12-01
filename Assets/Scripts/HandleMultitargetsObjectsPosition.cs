using UnityEngine;
using System.Collections;

public class HandleMultitargetsObjectsPosition : MonoBehaviour
{
    public GameObject trackedObjects;
    public GameObject imageTargets;
    // Use this for initialization

    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        
        Vector3 p = Vector3.zero;
        Quaternion r = Quaternion.identity;

        for (int i = 0; i < trackedObjects.transform.childCount; i++) { 
            if (trackedObjects.transform.GetChild(i).transform.gameObject.GetComponent<MeshRenderer>().enabled) { 
               // trackedObjects.transform.position = Vector3.Lerp(trackedObjects.transform.position,)
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