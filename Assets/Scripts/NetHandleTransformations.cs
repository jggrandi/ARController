﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Lean.Touch {

    public class NetHandleTransformations : NetworkBehaviour {

        public GameObject trackedObjects;
        public GameObject lockedObjects;

        [Tooltip("Ignore fingers with StartedOverGui?")]
        public bool IgnoreGuiFingers = true;
        Utils.Transformations mode = Utils.Transformations.Translation;

        Matrix4x4 prevMatrix;
        void Start() {
            trackedObjects = GameObject.Find("TrackedObjects");
            lockedObjects = GameObject.Find("LockedObjects");
            
        }

        void Update() {
            if (!isLocalPlayer) return;
            
            Matrix4x4 camMatrix = Camera.main.worldToCameraMatrix; 

            if (MainController.control.lockTransform) {

                Matrix4x4 step = prevMatrix * camMatrix.inverse;

                foreach (GameObject g in MainController.control.objSelectedNow) {
                    Matrix4x4 modelMatrix = Matrix4x4.TRS(g.transform.position, g.transform.rotation, new Vector3(1,1,1)); // get the object matrix
                    modelMatrix = prevMatrix * modelMatrix; // transform the model matrix to the camera space matrix
                    modelMatrix = step * modelMatrix; // transform the object's position and orientation
                    modelMatrix = prevMatrix.inverse * modelMatrix; // put the object in the world coordinates
                    
                    g.transform.position = Utils.GetPosition(modelMatrix);
                    g.transform.rotation = Utils.GetRotation(modelMatrix);

                    this.gameObject.transform.GetComponent<HandleNetworkFunctions>().CmdLockTransform(GetIndex(g), Utils.GetPosition(modelMatrix), Utils.GetRotation(modelMatrix));
                }
            }

            prevMatrix = camMatrix;
            mode = MainController.control.transformationNow;
            this.gameObject.transform.GetComponent<HandleNetworkFunctions>().SyncCamPosition(Camera.main.transform.position);
            
        }

       protected virtual void OnEnable() {
            LeanTouch.OnFingerSet += OnFingerSet; // Hook into the events we need
            LeanTouch.OnGesture += OnGesture;
        }

        protected virtual void OnDisable() {
            LeanTouch.OnFingerSet -= OnFingerSet;    // Unhook the events
            LeanTouch.OnGesture -= OnGesture;
        }

        public int GetIndex(GameObject g) {
            return g.GetComponent<ObjectGroupId>().index;
        }

        public void OnFingerSet(LeanFinger finger) {  // one finger on the screen
            if (!isLocalPlayer) return;
            if (IgnoreGuiFingers == true && finger.StartedOverGui == true) return;
            if (LeanTouch.Fingers.Count != 1) return;

            if (mode == Utils.Transformations.Translation) {  // translate in x and y axis
                foreach (GameObject g in MainController.control.objSelectedNow) {
                    
                    Vector3 right = Camera.main.transform.right * finger.ScreenDelta.x * 0.005f;
                    Vector3 up = Camera.main.transform.up * finger.ScreenDelta.y * 0.005f;
                    this.gameObject.transform.GetComponent<HandleNetworkFunctions>().Translate(GetIndex(g), Utils.PowVec3(right+up, 1.2f));
                    
                }
            } else if (mode == Utils.Transformations.Rotation) { // rotate in the x and y axis
                Vector3 avg = avgCenterOfObjects(MainController.control.objSelectedNow);
                Vector3 axis = Camera.main.transform.right * finger.ScreenDelta.y + Camera.main.transform.up * -finger.ScreenDelta.x;
                float magnitude = finger.ScreenDelta.magnitude*0.3f;
                foreach (GameObject g in MainController.control.objSelectedNow)
                    this.gameObject.GetComponent<HandleNetworkFunctions>().Rotate(GetIndex(g), avg, axis, magnitude);
            }
        }
        
        public void OnGesture(List<LeanFinger> fingers) {  // two fingers on screen
            if (!isLocalPlayer) return;
            if (LeanTouch.Fingers.Count != 2) return;

            Vector3 avg = avgCenterOfObjects(MainController.control.objSelectedNow);
            if (mode == Utils.Transformations.Translation) { // translate the object near or far away from the camera position
                Vector3 dir = (avg - Camera.main.transform.position) * LeanGesture.GetScreenDelta(fingers).y * 0.005f;
                foreach (GameObject g in MainController.control.objSelectedNow)
                    this.gameObject.GetComponent<HandleNetworkFunctions>().CmdTranslate(GetIndex(g), dir);
            } else if (mode == Utils.Transformations.Rotation) { // rotate the object around the 3rd axis
                float angle = LeanGesture.GetTwistDegrees(fingers)*0.3f;
                Vector3 axis = Camera.main.transform.forward;
                foreach (GameObject g in MainController.control.objSelectedNow)
                    this.gameObject.GetComponent<HandleNetworkFunctions>().Rotate(GetIndex(g), avg, axis, angle);
            } else if (mode == Utils.Transformations.Scale) { // pinch to scale up and down
                float scale = LeanGesture.GetPinchScale(fingers);
                foreach (GameObject g in MainController.control.objSelectedNow) {
                    Vector3 dir = g.transform.position - avg;
                    this.gameObject.GetComponent<HandleNetworkFunctions>().CmdScale(GetIndex(g), scale, dir);
                }
            }
        }

        private Vector3 avgCenterOfObjects(List<GameObject> objects) {
            Vector3 avg = Vector3.zero;
            foreach (GameObject g in objects)
                avg += g.transform.position;
            return avg /= objects.Count;
        }

        private void Scale(GameObject obj, float scale, Vector3 dir) {
            obj.transform.position += dir * (-1 + scale);
            obj.transform.localScale *= scale;
        }
    }
}