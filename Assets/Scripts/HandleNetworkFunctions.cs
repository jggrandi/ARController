using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class HandleNetworkFunctions : NetworkBehaviour {

    public GameObject TrackedObjects = null;

    public Transform GetLocalTransform() {
        return TrackedObjects.transform.parent;
    }

    public GameObject GetByIndex(int index) {
        return TrackedObjects.transform.GetChild(index).gameObject;
    }

    
    public void SyncObj(int index, bool pos = true, bool rot = true, bool scale = true) {
        Vector3 p = Vector3.zero;
        Quaternion r = new Quaternion(0, 0, 0, 0);
        Vector3 s = Vector3.zero;
        var g = GetByIndex(index);
        if (pos) p = g.transform.localPosition;
        if (rot) r = g.transform.localRotation;
        if (scale) s = g.transform.localScale;
        RpcSyncObj(index, p, r, s);
    }

    [Command]
    public void CmdSyncAll() {
        if(TrackedObjects == null) TrackedObjects = GameObject.Find("TrackedObjects");
        for (int i = 0; i < TrackedObjects.transform.childCount; i++) {
            SyncObj(i);
        }
    }

    [ClientRpc]
    public void RpcSyncObj(int index, Vector3 pos, Quaternion rot, Vector3 scale) {
        var g = GetByIndex(index);
        if (pos != Vector3.zero) g.transform.localPosition = pos;
        if (rot != new Quaternion(0,0,0,0)) g.transform.localRotation = rot;
        if (scale != Vector3.zero) g.transform.localScale = scale;
    }

    public void Start() {
        TrackedObjects = GameObject.Find("TrackedObjects");
    }

    [ClientRpc]
    public void RpcLockTransform(int index, Vector3 position, Quaternion rotation) {
        if (isLocalPlayer) return;
        position = GetLocalTransform().TransformPoint(position);
        rotation = rotation * GetLocalTransform().rotation;
        GetByIndex(index).transform.position = position;
        GetByIndex(index).transform.rotation = rotation;
    }

    [Command]
    public void CmdLockTransform(int index, Vector3 position, Quaternion rotation) {
        RpcLockTransform(index, position, rotation);
    }
    public void LockTransform(int index, Vector3 position, Quaternion rotation) {
        position = GetLocalTransform().InverseTransformPoint(position);
        rotation = Quaternion.Inverse(GetLocalTransform().rotation) * rotation;
        CmdLockTransform(index, position, rotation);
    }

    public void Translate(int index, Vector3 vec) {
        var g = GetByIndex(index);
        Vector3 prevLocalPos = g.transform.localPosition;
        g.transform.position += vec;
        Vector3 localPos = g.transform.localPosition;
        g.transform.position -= vec;
        CmdTranslate(index, localPos - prevLocalPos);

    }

    [Command]
    public void CmdTranslate(int index, Vector3 vec) {
        var g = GetByIndex(index);
        g.transform.localPosition += vec;
        Debug.Log(vec.x);
        RpcTranslate(index, g.transform.localPosition);
    }
    [ClientRpc]
    public void RpcTranslate(int index, Vector3 pos) {
        var g = GetByIndex(index);
        g.transform.localPosition = pos;
    }
    

    [Command]
    public void CmdRotate(int index, Vector3 avg, Vector3 axis, float mag) {
        var g = GetByIndex(index);
        avg = GetLocalTransform().TransformPoint(avg);
        axis = GetLocalTransform().TransformVector(axis);
        g.transform.RotateAround(avg, axis, mag);
        SyncObj(index);
    }
    public void Rotate(int index, Vector3 avg, Vector3 axis, float mag) {
        avg = GetLocalTransform().InverseTransformPoint(avg);
        axis = GetLocalTransform().InverseTransformVector(axis);
        CmdRotate(index, avg, axis, mag);
    }


    [ClientRpc]
    public void RpcScale(int index, float scale, Vector3 dir) {
        var g = GetByIndex(index);
        g.transform.position += dir * (-1 + scale);

        g.transform.localScale *= scale;

        var s = g.transform.localScale.x;
        s = Mathf.Min(Mathf.Max(s, 0.1f), 4.0f);

        g.transform.localScale = new Vector3(s,s,s);
    }

    [Command]
    public void CmdScale(int index, float scale, Vector3 dir) {
        RpcScale(index, scale, dir);
    }


    public override void OnStartLocalPlayer() {
    //public override void OnStartClient() {
        CmdSyncAll();
    }

}
