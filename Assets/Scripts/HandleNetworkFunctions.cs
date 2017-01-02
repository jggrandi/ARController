using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class HandleNetworkFunctions : NetworkBehaviour {

    [Command]
    public void CmdLockTransform(GameObject g, Vector3 position, Quaternion rotation) {
        g.transform.position = position;
        g.transform.rotation = rotation;
    }

    [Command]
    public void CmdTranslate(GameObject g, Vector3 vec) {
        g.transform.position += vec;
    }

    [Command]
    public void CmdRotate(GameObject g, Vector3 avg, Vector3 axis, float mag) {
        g.transform.RotateAround(avg, axis, mag * 0.1f);
    }

    [ClientRpc]
    public void RpcScale(GameObject g, float scale, Vector3 dir) {
        g.transform.position += dir * (-1 + scale);
        g.transform.localScale *= scale;
    }

    [Command]
    public void CmdScale(GameObject g, float scale, Vector3 dir) {
        RpcScale(g, scale, dir);
    }

    [Command]
    public void CmdSetGroup(GameObject obj) {
        obj.transform.gameObject.GetComponent<ObjectGroupId>().id = MainController.control.idAvaiableNow;
    }

    [Command]
    public void CmdSetGroup2(GameObject obj, int id) {
        obj.transform.gameObject.GetComponent<ObjectGroupId>().id = id;
    }

    [Command]
    public void CmdIncrementCount() {
        MainController.control.idAvaiableNow++;
    }


}
