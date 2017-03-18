using UnityEngine;
using System.Collections;

public class HandleMultitargetsObjectsPosition : MonoBehaviour
{
    
    public GameObject trackedObjects;
    public GameObject imageTargets;
    
    void Start(){
        
        //Debug.Log(imageTargets.transform.childCount);
        for (int i = 0; i < imageTargets.transform.childCount; i++) {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.parent = imageTargets.transform.GetChild(i).transform;
            cube.transform.position = Vector3.zero;
            cube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            
            cube.transform.GetComponent<BoxCollider>().enabled = false;
            cube.transform.GetComponent<MeshRenderer>().enabled = false;
            //imageTargets.transform.GetChild(i).GetChild(0).transform.position = Vector3.zero;
            //imageTargets.transform.GetChild(i).GetChild(0).transform.rotation = Quaternion.identity;
        }
    }

    // Update is called once per frame
    void Update(){

        //Debug.Log(trackedObjects.transform.childCount);


        Vector3 avg = Vector3.zero;
        int n = 0;

        for (int i = 0; i < imageTargets.transform.childCount; i++) {
            if (trackedObjects.activeInHierarchy) {
                if (imageTargets.transform.GetChild(i).GetChild(0).gameObject.activeInHierarchy) {

                    avg += imageTargets.transform.GetChild(i).GetChild(0).position;
                    n++;
                }
            }
        }

        float distAvg = 0;
        avg /= n;

        for (int i = 0; i < imageTargets.transform.childCount; i++) {
            if (trackedObjects.activeInHierarchy) {
                if (imageTargets.transform.GetChild(i).GetChild(0).gameObject.activeInHierarchy) {
                    distAvg += Vector3.Distance(imageTargets.transform.GetChild(i).GetChild(0).position, avg);
                }
            }
        }
        distAvg = Mathf.Max(distAvg / n, 0.05f);

        //Debug.Log("distAvg: " + distAvg);
        Vector3 avg2 = Vector3.zero;
        n = 0;

        for (int i = 0; i < imageTargets.transform.childCount; i++) {
            if (trackedObjects.activeInHierarchy) {
                if (imageTargets.transform.GetChild(i).GetChild(0).gameObject.activeInHierarchy) {
                    //if (Vector3.Distance(imageTargets.transform.GetChild(i).GetChild(0).transform.position, avg) > distAvg*1.5) {
                        //Debug.Log("Descartado: " + i);
                      //  continue;
                    //}
                    
                    trackedObjects.transform.position = Vector3.Lerp(trackedObjects.transform.position, imageTargets.transform.GetChild(i).GetChild(0).transform.position, 0.10f);
                    trackedObjects.transform.rotation = Quaternion.Slerp(trackedObjects.transform.rotation, imageTargets.transform.GetChild(i).GetChild(0).transform.rotation, 0.10f);

                    avg2 += imageTargets.transform.GetChild(i).GetChild(0).transform.position;
                    n++;
                }
            }
        }
        
        //avg2 /= n;
        //if(n > 0)
        //for (int i = 0; i < imageTargets.transform.childCount; i++) {
        //    if (trackedObjects.activeInHierarchy) {
        //        if (imageTargets.transform.GetChild(i).GetChild(0).gameObject.activeInHierarchy) {
        //            if (Vector3.Distance(imageTargets.transform.GetChild(i).GetChild(0).transform.position, avg2) > 1.5f) continue;

        //            imageTargets.transform.GetChild(i).GetChild(0).transform.position = Vector3.Lerp(avg2, imageTargets.transform.GetChild(i).GetChild(0).transform.position, 0.999f);
        //            imageTargets.transform.GetChild(i).GetChild(0).transform.rotation = Quaternion.Slerp(trackedObjects.transform.rotation, imageTargets.transform.GetChild(i).GetChild(0).transform.rotation, 0.999f);
        //        }
        //    }
        //}
    }
}