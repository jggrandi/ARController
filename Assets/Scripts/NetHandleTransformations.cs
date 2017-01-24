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
        Utils.Transformations mode = Utils.Transformations.Translation;

        int translationZ = 0;
        
        float countFrames = 0;

        Matrix4x4 prevMatrix;
        void Start() {
            trackedObjects = GameObject.Find("TrackedObjects");
        }

        void Update() {
            if (!isLocalPlayer) return;
            
            Matrix4x4 camMatrix = Camera.main.worldToCameraMatrix; 

            if (MainController.control.lockTransform) {

                Matrix4x4 step = prevMatrix * camMatrix.inverse;

                foreach (int index in MainController.control.objSelected) {
                    var g = Utils.GetByIndex(index);
                    Matrix4x4 modelMatrix = Matrix4x4.TRS(g.transform.position, g.transform.rotation, new Vector3(1,1,1)); // get the object matrix
                    modelMatrix = prevMatrix * modelMatrix; // transform the model matrix to the camera space matrix
                    modelMatrix = step * modelMatrix; // transform the object's position and orientation
                    modelMatrix = prevMatrix.inverse * modelMatrix; // put the object in the world coordinates
                    
                    g.transform.position = Utils.GetPosition(modelMatrix);
                    g.transform.rotation = Utils.GetRotation(modelMatrix);

                    this.gameObject.transform.GetComponent<HandleNetworkFunctions>().LockTransform(GetIndex(g), Utils.GetPosition(modelMatrix), Utils.GetRotation(modelMatrix));
                }
            }

            prevMatrix = camMatrix;
            mode = MainController.control.transformationNow;
            //this.gameObject.transform.GetComponent<HandleNetworkFunctions>().SyncCamPosition(Camera.main.transform.position);

            if (countFrames % 50 == 0 ) {
                if (LeanTouch.Fingers.Count <= 0) {
                    translationZ = 0;
                    MainController.control.isTapForTransform = false;
                }
            }
            countFrames++;
        }

       protected virtual void OnEnable() {
            LeanTouch.OnFingerSet += OnFingerSet; // Hook into the events we need
            LeanTouch.OnGesture += OnGesture;
            LeanTouch.OnFingerTap += OnFingerTap;

        }

        protected virtual void OnDisable() {
            LeanTouch.OnFingerSet -= OnFingerSet;    // Unhook the events
            LeanTouch.OnGesture -= OnGesture;
            LeanTouch.OnFingerTap -= OnFingerTap;
        }

        public int GetIndex(GameObject g) {
            return g.GetComponent<ObjectGroupId>().index;
        }

        private void OnFingerTap(LeanFinger finger) {
            translationZ = 1;
            if (MainController.control.objSelected.Count != 0)
                MainController.control.isTapForTransform = true;

        }

        public void OnFingerSet(LeanFinger finger) {  // one finger on the screen
            if (!isLocalPlayer) return;
            if (IgnoreGuiFingers == true && finger.StartedOverGui == true) return;
            if (LeanTouch.Fingers.Count != 1) return;

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

                    Vector3 translate = Camera.main.transform.position * finger.ScreenDelta.y * 0.005f; ;
                    this.gameObject.GetComponent<HandleNetworkFunctions>().Translate(index, translate);
                }
            }

        }
        
        public void OnGesture(List<LeanFinger> fingers) {  // two fingers on screen
            if (!isLocalPlayer) return;
            if (LeanTouch.Fingers.Count != 2) return;
            //Debug.Log(LeanGesture.GetTwistDegrees(fingers));

            Vector3 avg = avgCenterOfObjects(MainController.control.objSelected);
            if (mode == Utils.Transformations.Translation) { // translate the object near or far away from the camera position
                float scale = LeanGesture.GetPinchScale(fingers);
                //Vector3 translate = (avg - Camera.main.transform.position) * LeanGesture.GetScreenDelta(fingers).y * 0.005f;
                //translate += (avg - Camera.main.transform.position) * (1-scale) * 0.8f;

                Vector3 translate = (avg - Camera.main.transform.position) * (1 - scale) * 0.8f;
                
                foreach (var index in MainController.control.objSelected)
                    this.gameObject.GetComponent<HandleNetworkFunctions>().CmdTranslate(index, translate);
            } else if (mode == Utils.Transformations.Rotation) { // rotate the object around the 3rd axis
                float angle = LeanGesture.GetTwistDegrees(fingers)*0.8f;
                Vector3 axis;
                if( Mathf.Abs(angle) > 0.5f)
                    axis = Camera.main.transform.forward;
                else {
                    axis = Camera.main.transform.right * fingers[0].ScreenDelta.y + Camera.main.transform.up * -fingers[0].ScreenDelta.x;
                    angle = fingers[0].ScreenDelta.magnitude * 0.3f;
                }
                foreach (int index in MainController.control.objSelected)
                    this.gameObject.GetComponent<HandleNetworkFunctions>().Rotate(index, avg, axis, angle);
            } else if (mode == Utils.Transformations.Scale) { // pinch to scale up and down
                float scale = LeanGesture.GetPinchScale(fingers);
                float translate = LeanGesture.GetScreenDelta(fingers).y;
                float angle = LeanGesture.GetTwistDegrees(fingers);


                foreach (var index in MainController.control.objSelected) {
                    var g = Utils.GetByIndex(index);
                    Vector3 dir = g.transform.position - avg;
                    this.gameObject.GetComponent<HandleNetworkFunctions>().CmdScale(index, scale, dir);
                    
               }

            }
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
    }
}