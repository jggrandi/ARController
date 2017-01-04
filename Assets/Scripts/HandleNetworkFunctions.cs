﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class HandleNetworkFunctions : NetworkBehaviour {

    public GameObject TrackedObjects;

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
        GetByIndex(index).transform.position = position;
        GetByIndex(index).transform.rotation = rotation;
    }

    [Command]
    public void CmdLockTransform(int index, Vector3 position, Quaternion rotation) {
        RpcLockTransform(index, position, rotation);
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
    }

    [Command]
    public void CmdScale(int index, float scale, Vector3 dir) {
        RpcScale(index, scale, dir);
    }

    /*[Command]
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
    }*/

    [ClientRpc]
    public void RpcSyncCamPosition(Vector3 pos) {
        gameObject.transform.GetChild(0).transform.position = pos;
    }
    [Command]
    public void CmdSyncCamPosition(Vector3 pos) {
        RpcSyncCamPosition(pos);
    }
    public void SyncCamPosition(Vector3 pos) {
        gameObject.transform.GetChild(0).transform.position = pos;
        CmdSyncCamPosition(pos);
    }

}
