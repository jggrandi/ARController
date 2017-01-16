using UnityEngine;
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
                foreach (var index in MainController.control.objSelected) {
                    
                    Vector3 right = Camera.main.transform.right * finger.ScreenDelta.x * 0.005f;
                    Vector3 up = Camera.main.transform.up * finger.ScreenDelta.y * 0.005f;
                    this.gameObject.transform.GetComponent<HandleNetworkFunctions>().Translate(index, Utils.PowVec3(right+up, 1.2f));
                    
                }
            } else if (mode == Utils.Transformations.Rotation) { // rotate in the x and y axis
                Vector3 avg = avgCenterOfObjects(MainController.control.objSelected);
                Vector3 axis = Camera.main.transform.right * finger.ScreenDelta.y + Camera.main.transform.up * -finger.ScreenDelta.x;
                float magnitude = finger.ScreenDelta.magnitude*0.3f;
                foreach (int index in MainController.control.objSelected)
                    this.gameObject.GetComponent<HandleNetworkFunctions>().Rotate(index, avg, axis, magnitude);
            }
        }
        
        public void OnGesture(List<LeanFinger> fingers) {  // two fingers on screen
            if (!isLocalPlayer) return;
            if (LeanTouch.Fingers.Count != 2) return;


                    Vector3 avg = avgCenterOfObjects(MainController.control.objSelected);
            if (mode == Utils.Transformations.Translation) { // translate the object near or far away from the camera position
                float scale = LeanGesture.GetPinchScale(fingers);
                Vector3 translate = (avg - Camera.main.transform.position) * LeanGesture.GetScreenDelta(fingers).y * 0.005f;
                translate += (avg - Camera.main.transform.position) * (1-scale) * 0.8f;

                foreach (var index in MainController.control.objSelected)
                    this.gameObject.GetComponent<HandleNetworkFunctions>().CmdTranslate(index, translate);
            } else if (mode == Utils.Transformations.Rotation) { // rotate the object around the 3rd axis
                float angle = LeanGesture.GetTwistDegrees(fingers)*0.8f;
                Vector3 axis = Camera.main.transform.forward;
                foreach (int index in MainController.control.objSelected)
                    this.gameObject.GetComponent<HandleNetworkFunctions>().Rotate(index, avg, axis, angle);
            } else if (mode == Utils.Transformations.Scale) { // pinch to scale up and down
                float scale = LeanGesture.GetPinchScale(fingers);
                float translate = LeanGesture.GetScreenDelta(fingers).y;
                float angle = LeanGesture.GetTwistDegrees(fingers);


                foreach (var index in MainController.control.objSelected) {
                    var g = Utils.GetByIndex(index);

                    if (g.GetComponent<ParticleSystem>() != null) {

                        float lifeTime = 1+translate * 0.01f;
                        float rate = 1+angle * 0.05f;

                        this.gameObject.GetComponent<HandleNetworkFunctions>().CmdSetParticle(index, lifeTime, rate);

                        if (g.transform.GetComponent<ParameterBars>() != null) {

                            ParticleSystem particle = g.GetComponent<ParticleSystem>();
                            var r = particle.emission.rate;

                            g.transform.GetComponent<ParameterBars>().active();
                            g.transform.GetComponent<ParameterBars>().values[0] = g.transform.localScale.x * scale / 4.0f ;
                            g.transform.GetComponent<ParameterBars>().values[1] = particle.startLifetime * lifeTime / 5.0f;
                            g.transform.GetComponent<ParameterBars>().values[2] = r.constant * rate / 12.0f;
                        }


                    }
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