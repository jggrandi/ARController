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

        void Start() {
            trackedObjects = GameObject.Find("TrackedObjects");
            lockedObjects = GameObject.Find("LockedObjects");
            
        }

    void Update() {
            if (!isLocalPlayer) return;
           
            if (MainController.control.lockTransform) {
                foreach (GameObject g in MainController.control.objSelectedNow) {
                    g.transform.parent = lockedObjects.transform;
                }
            } else {
                foreach (GameObject g in MainController.control.objSelectedNow) {
                    g.transform.parent = trackedObjects.transform;
                }
            }

			mode = MainController.control.transformationNow;
            var fingers = LeanTouch.GetFingers(true, 2);
            if (fingers != null) OnGesture(fingers);
        }

       protected virtual void OnEnable() {
            LeanTouch.OnFingerSet += OnFingerSet; // Hook into the events we need
        }

        protected virtual void OnDisable() {
            LeanTouch.OnFingerSet -= OnFingerSet;    // Unhook the events
        }

        public void OnFingerSet(LeanFinger finger) {  // one finger on the screen
            if (!isLocalPlayer) return;
            if (IgnoreGuiFingers == true && finger.StartedOverGui == true) return;

            if (LeanTouch.Fingers.Count < 1) return;
            if (mode == Utils.Transformations.Translation) {  // translate in x and y axis
                foreach (GameObject g in MainController.control.objSelectedNow) {
                    Vector3 right = Camera.main.transform.right * finger.ScreenDelta.x * 0.005f;
                    Vector3 up = Camera.main.transform.up * finger.ScreenDelta.y * 0.005f;
                    this.gameObject.transform.GetComponent<HandleNetworkFunctions>().CmdTranslate(g, right);
                    this.gameObject.transform.GetComponent<HandleNetworkFunctions>().CmdTranslate(g, up);
                }
            } else if (mode == Utils.Transformations.Rotation) { // rotate in the x and y axis
                Vector3 avg = avgCenterOfObjects(MainController.control.objSelectedNow);
                Vector3 axis = Camera.main.transform.right * finger.ScreenDelta.y + Camera.main.transform.up * -finger.ScreenDelta.x;
                float magnitude = finger.ScreenDelta.magnitude;
                foreach (GameObject g in MainController.control.objSelectedNow)
                    this.gameObject.GetComponent<HandleNetworkFunctions>().CmdRotate(g, avg, axis, magnitude);
            }
        }
        
        public void OnGesture(List<LeanFinger> fingers) {  // two fingers on screen
            if (!isLocalPlayer) return;

            //bool fingerOverGUI = false;
            //foreach (LeanFinger f in fingers)
            //    if (f.StartedOverGui == true)
            //        fingerOverGUI = true;
            //Debug.Log(fingerOverGUI);
            //if (IgnoreGuiFingers == true && fingerOverGUI == true) { Debug.Log("A"); return; }

            Vector3 avg = avgCenterOfObjects(MainController.control.objSelectedNow);
            if (mode == Utils.Transformations.Translation) { // translate the object near or far away from the camera position
                Vector3 dir = avg - Camera.main.transform.position * LeanGesture.GetScreenDelta(fingers).y * 0.005f;
                foreach (GameObject g in MainController.control.objSelectedNow)
                    this.gameObject.GetComponent<HandleNetworkFunctions>().CmdTranslate(g, dir);
            } else if (mode == Utils.Transformations.Rotation) { // rotate the object around the 3rd axis
                float angle = LeanGesture.GetTwistDegrees(fingers);
                Vector3 axis = Camera.main.transform.forward;
                foreach (GameObject g in MainController.control.objSelectedNow)
                    this.gameObject.GetComponent<HandleNetworkFunctions>().CmdRotate(g, avg, axis, angle);
            } else if (mode == Utils.Transformations.Scale) { // pinch to scale up and down
                float scale = LeanGesture.GetPinchScale(fingers);
                foreach (GameObject g in MainController.control.objSelectedNow) {
                    Vector3 dir = g.transform.position - avg;
                    this.gameObject.GetComponent<HandleNetworkFunctions>().CmdScale(g, scale, dir);
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