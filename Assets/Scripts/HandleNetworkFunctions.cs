using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class HandleNetworkFunctions : NetworkBehaviour {

    [Command]
    public void CmdTranslate(GameObject g, Vector3 vec) {
        Debug.Log("Translate");
        g.transform.position += vec;
    }

    [Command]
    public void CmdRotate(GameObject g, Vector3 avg, Vector3 axis, float mag) {
        g.transform.RotateAround(avg, axis, mag * 0.1f);
    }

    [Command]
    public void CmdScale(GameObject g, Vector3 right, Vector3 up) {

    }


    //[Command]
    //public int CmdGetGroup(GameObject obj) {
    //    return obj.transform.gameObject.GetComponent<ObjectGroupId>().id;
    //}
    [Command]
    public void CmdSetGroup(GameObject obj) {
        Debug.Log("SetGroup");
        obj.transform.gameObject.GetComponent<ObjectGroupId>().id = MainController.control.idAvaiableNow;
    }
    [Command]
    public void CmdSetGroup2(GameObject obj, int id) {
        Debug.Log("SetGroup2");
        obj.transform.gameObject.GetComponent<ObjectGroupId>().id = id;
    }
    public void CmdIncrementCount() {
        Debug.Log("INC");
        MainController.control.idAvaiableNow++;
    }

}
