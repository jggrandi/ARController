using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Lean.Touch {

    public class NetHandleTransformations : NetworkBehaviour {
        
        public GameObject trackedObjects;
        public LayerMask LayerMask = Physics.DefaultRaycastLayers;
        [Tooltip("Ignore fingers with StartedOverGui?")]
        public bool IgnoreGuiFingers = true;
        //Utils.Transformations mode = Utils.Transformations.Translation;

        int translationZ = 0;
        
        float countFrames = 0;
        Matrix4x4 prevMatrix;

        public int modality = -1;

        void Start() {
            trackedObjects = GameObject.Find("TrackedObjects");
            
        }

        void Update() {
            if (!isLocalPlayer) return;

            Matrix4x4 camMatrix = Camera.main.worldToCameraMatrix; 

            if (MainController.control.lockTransform) {
                CmdUpdateModality(1);
                Matrix4x4 step = prevMatrix * camMatrix.inverse;

                foreach (int index in MainController.control.objSelected) {
                    var g = Utils.GetByIndex(index);
                    var gSharp = g;
                    Matrix4x4 modelMatrix = Matrix4x4.TRS(gSharp.transform.position, gSharp.transform.rotation, new Vector3(1,1,1)); // get the object matrix
                    modelMatrix = prevMatrix * modelMatrix; // transform the model matrix to the camera space matrix
                    modelMatrix = step * modelMatrix; // transform the object's position and orientation
                    modelMatrix = prevMatrix.inverse * modelMatrix; // put the object in the world coordinates
                    
                    gSharp.transform.position = Utils.GetPosition(modelMatrix);
                    gSharp.transform.rotation = Utils.GetRotation(modelMatrix);

                    g.transform.position = Vector3.Lerp(g.transform.position, gSharp.transform.position, 0.5f);
                    g.transform.rotation = Quaternion.Lerp(g.transform.rotation, gSharp.transform.rotation, 0.5f);

                    this.gameObject.transform.GetComponent<HandleNetworkFunctions>().LockTransform(GetIndex(g), Utils.GetPosition(modelMatrix), Utils.GetRotation(modelMatrix));
                }
            }

            prevMatrix = camMatrix;
            //mode = MainController.control.transformationNow;
            //this.gameObject.transform.GetComponent<HandleNetworkFunctions>().SyncCamPosition(Camera.main.transform.position);

            

            if (countFrames % 35 == 0 ) {
                if (LeanTouch.Fingers.Count <= 0) {
                    translationZ = 0;
                    MainController.control.isTapForTransform = false;
                    if (!MainController.control.lockTransform)
                        CmdUpdateModality(-1);
                }
            }
            countFrames++;
        }

       protected virtual void OnEnable() {
            LeanTouch.OnFingerSet += OnFingerSet; // Hook into the events we need
            LeanTouch.OnGesture += OnGesture;
            LeanTouch.OnFingerTap += OnFingerTap;
            LeanTouch.OnFingerUp += OnFingerUp;


        }

        protected virtual void OnDisable() {
            LeanTouch.OnFingerSet -= OnFingerSet;    // Unhook the events
            LeanTouch.OnGesture -= OnGesture;
            LeanTouch.OnFingerTap -= OnFingerTap;
            LeanTouch.OnFingerUp -= OnFingerUp;
        }

        public int GetIndex(GameObject g) {
            return g.GetComponent<ObjectGroupId>().index;
        }

        private void OnFingerUp(LeanFinger finger) {
            translationZ = 0;
            MainController.control.isTapForTransform = false;

        }

        private void OnFingerTap(LeanFinger finger) {
            if (TestController.tcontrol.taskOrder[TestController.tcontrol.sceneIndex] == 1) return;
            translationZ = 1;
            if (MainController.control.objSelected.Count != 0)
                MainController.control.isTapForTransform = true;

        }

        public void OnFingerSet(LeanFinger finger) {  // one finger on the screen
            if (!isLocalPlayer) return;
            if (IgnoreGuiFingers == true && finger.StartedOverGui == true) return;
            if (LeanTouch.Fingers.Count != 1) return;
            if (TestController.tcontrol.taskOrder[TestController.tcontrol.sceneIndex] == 1) return;

            if(MainController.control.lockTransform)
                CmdUpdateModality(2);
            else
                CmdUpdateModality(0);

            if (translationZ == 1)
                translationZ = 2;
            
            if(translationZ == 2)
                MainController.control.isTapForTransform = true;

            foreach (var index in MainController.control.objSelected) {
                if (translationZ < 2) {
                    Vector3 right = Camera.main.transform.right * finger.ScreenDelta.x * 0.005f;
                    Vector3 up = Camera.main.transform.up * finger.ScreenDelta.y * 0.005f;
                    this.gameObject.transform.GetComponent<HandleNetworkFunctions>().Translate(index, Utils.PowVec3(right + up, 1.2f));
                } else if (translationZ == 2) {
                    Vector3 avg = avgCenterOfObjects(MainController.control.objSelected);
                    Vector3 translate =  (avg - Camera.main.transform.position).normalized * finger.ScreenDelta.y * 0.005f; // obj pos - cam pos
                    this.gameObject.GetComponent<HandleNetworkFunctions>().Translate(index, translate);
                }
            }

        }
        
        public void OnGesture(List<LeanFinger> fingers) {  // two fingers on screen
            if (!isLocalPlayer) return;
            if (LeanTouch.Fingers.Count != 2) return;
            if (TestController.tcontrol.taskOrder[TestController.tcontrol.sceneIndex] == 1) return;

            if (MainController.control.lockTransform)
                CmdUpdateModality(2);
            else
                CmdUpdateModality(0);

            Vector3 avg = avgCenterOfObjects(MainController.control.objSelected);
            float angle = LeanGesture.GetTwistDegrees(fingers) * 0.8f;
            Vector3 axis;
            if (Mathf.Abs(angle) > 0.5f)
                axis = Camera.main.transform.forward;
            else {
                axis = Camera.main.transform.right * fingers[0].ScreenDelta.y + Camera.main.transform.up * -fingers[0].ScreenDelta.x;
                angle = fingers[0].ScreenDelta.magnitude * 0.3f;
            }
            foreach (int index in MainController.control.objSelected)
                this.gameObject.GetComponent<HandleNetworkFunctions>().Rotate(index, avg, axis, angle);

        }

        private Vector3 avgCenterOfObjects(List<int> objects) {
            Vector3 avg = Vector3.zero;
            foreach (var index in objects) {
                var g = Utils.GetByIndex(index);
                avg += g.transform.position;
            }
            return avg /= objects.Count;
        }

        private void Scale(GameObject obj, float scale, Vector3 dir) {
            obj.transform.position += dir * (-1 + scale);
            obj.transform.localScale *= scale;
        }

        [ClientRpc]
        void RpcUpdateModality(int m) {
            modality = m;
        }

        [Command]
        void CmdUpdateModality(int m) {
            RpcUpdateModality(m);
        }
            
    }
}