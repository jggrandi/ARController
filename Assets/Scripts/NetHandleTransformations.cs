using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Lean.Touch {

    public class NetHandleTransformations : NetworkBehaviour {
        
        public GameObject lockedObjects;
        public GameObject trackedObjects;
        //public LayerMask LayerMask = Physics.DefaultRaycastLayers;
        [Tooltip("Ignore fingers with StartedOverGui?")]
        public bool IgnoreGuiFingers = true;
        //Utils.Transformations mode = Utils.Transformations.Translation;

        int translationZ = 0;
        float countFrames = 0;
        Matrix4x4 prevMatrix;
        public int modality = -1;
        //int currentOperation = 0; /* move rotate resize move_cel */

        void Start() {
            trackedObjects = GameObject.Find("TrackedObjects");            
        }


        string log ="";

        void OnGUI() {
            var centeredStyle = GUI.skin.GetStyle("Label");
            centeredStyle.alignment = TextAnchor.UpperCenter;
            centeredStyle.fontSize = 30;
            GUI.Label(new Rect(Screen.width / 2 - 500, Screen.height / 2 - 25, 500, 50), log, centeredStyle);
        }


        void Update() {
            if (!isLocalPlayer) return;

            Matrix4x4 camMatrix = Camera.main.worldToCameraMatrix;

            if (MainController.control.lockTransform) {
                CmdUpdateModality(1);
                Matrix4x4 step = prevMatrix * camMatrix.inverse;

                foreach (int index in MainController.control.objSelected) {
                    var g = ObjectManager.Get(index);
                    //var gSharp = new GameObject();  // The comments are for smoothed movements. Buggy: it is creating empty objects. 
                    //gSharp.transform.position = g.transform.position;
                    //gSharp.transform.rotation = g.transform.rotation;
                    //Matrix4x4 modelMatrix = Matrix4x4.TRS(gSharp.transform.position, gSharp.transform.rotation, new Vector3(1, 1, 1)); // get the object matrix
                    Matrix4x4 modelMatrix = Matrix4x4.TRS(g.transform.position, g.transform.rotation, new Vector3(1, 1, 1)); // get the object matrix
                    modelMatrix = prevMatrix * modelMatrix; // transform the model matrix to the camera space matrix
                    modelMatrix = step * modelMatrix; // transform the object's position and orientation
                    modelMatrix = prevMatrix.inverse * modelMatrix; // put the object in the world coordinates
                    //gSharp.transform.position = Utils.GetPosition(modelMatrix);
                    //gSharp.transform.rotation = Utils.GetRotation(modelMatrix);
                    //g.transform.position = Vector3.Lerp(g.transform.position, gSharp.transform.position, 0.95f);
                    //g.transform.rotation = Quaternion.Lerp(g.transform.rotation, gSharp.transform.rotation, 0.95f);
                    g.transform.position = Utils.GetPosition(modelMatrix);
                    g.transform.rotation = Utils.GetRotation(modelMatrix);
                    this.gameObject.transform.GetComponent<HandleNetworkFunctions>().LockTransform(GetIndex(g), Utils.GetPosition(modelMatrix), Utils.GetRotation(modelMatrix));
                }
                setCurrentOperation(OPERATION_LOCK);
            }

            prevMatrix = camMatrix;
            //mode = MainController.control.transformationNow;
            //this.gameObject.transform.GetComponent<HandleNetworkFunctions>().SyncCamPosition(Camera.main.transform.position);



            if (countFrames % 35 == 0) {
                if (LeanTouch.Fingers.Count <= 0) {
                    translationZ = 0;
                    MainController.control.isTapForTransform = false;
                    if (!MainController.control.lockTransform)
                        CmdUpdateModality(-1);
                }
            }
            countFrames++;
        }

        const int OPERATION_NONE = 0;
        const int OPERATION_MOVE = 1;
        const int OPERATION_ROTATE = 2;
        const int OPERATION_RESIZE = 3;
        const int OPERATION_LOCK = 4;
        void setCurrentOperation(int op) {
            GetComponent<NetHandleSelectionTouch>().CmdSetCurrentOperation(op);
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

        private void OnFingerUp(LeanFinger finger) {
            translationZ = 0;
            gestureOperation = 0;
            MainController.control.isTapForTransform = false;
            setCurrentOperation(OPERATION_NONE);
        }

        public int GetIndex(GameObject g) {
            return g.GetComponent<ObjectGroupId>().index;
        }


        float transFactor = 0.005f;
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

            if (translationZ == 1)
                translationZ = 2;
            else
                CmdUpdateModality(0);

            if (translationZ == 2)
                
                MainController.control.isTapForTransform = true;

            this.gameObject.GetComponent<NetHandleSelectionTouch>().unselectAllCount = 0;

            foreach (var index in MainController.control.objSelected) {
                if (translationZ < 2) {
                    Vector3 right = Camera.main.transform.right.normalized * finger.ScreenDelta.x * transFactor * Utils.ToutchSensibility;
                    Vector3 up = Camera.main.transform.up.normalized * finger.ScreenDelta.y * transFactor * Utils.ToutchSensibility;
                    this.gameObject.transform.GetComponent<HandleNetworkFunctions>().Translate(index, Utils.PowVec3(right + up, 1.2f));
                } else if (translationZ == 2) {
                    Vector3 avg = avgCenterOfObjects(MainController.control.objSelected);
                    Vector3 translate = (avg - Camera.main.transform.position).normalized * finger.ScreenDelta.y * transFactor * Utils.ToutchSensibility; // obj pos - cam pos

                    this.gameObject.GetComponent<HandleNetworkFunctions>().Translate(index, Utils.PowVec3(translate, 1.2f));

                    this.gameObject.GetComponent<HandleNetworkFunctions>().Translate(index, translate);
                }
            }
            setCurrentOperation(OPERATION_MOVE);
        }

        public int gestureOperation = 0;
       
        public void OnGesture(List<LeanFinger> fingers) {  // two fingers on screen
            if (!isLocalPlayer) return;
            if (LeanTouch.Fingers.Count != 2) return;
            if (TestController.tcontrol.taskOrder[TestController.tcontrol.sceneIndex] == 1) return;

            if (MainController.control.lockTransform)
                CmdUpdateModality(2);
            else
                CmdUpdateModality(0);
            Vector3 avg = avgCenterOfObjects(MainController.control.objSelected);
            Vector3 axis, axisTwist;
			float angle = LeanGesture.GetScreenDelta(fingers).magnitude * 0.3f;
            
            float angleTwist = LeanGesture.GetTwistDegrees(fingers) * 0.8f;
            axisTwist = Camera.main.transform.forward.normalized;
            axis = Camera.main.transform.right.normalized * LeanGesture.GetScreenDelta(fingers).y + Camera.main.transform.up.normalized * -LeanGesture.GetScreenDelta(fingers).x;
            float pos = LeanGesture.GetScreenDelta(fingers).magnitude * 0.3f;
            float scale = LeanGesture.GetPinchScale(fingers);
            
            foreach (int index in MainController.control.objSelected) {
                var g = ObjectManager.Get(index);
                if (g.GetComponent<ParticleSystem>() != null) {
                    var dir = LeanGesture.GetScreenDelta(fingers);
                    float lifeTime = Vector2.Dot(dir, new Vector2(1, 0)) * 0.01f;
                    float rate = Vector2.Dot(dir, new Vector2(0, 1)) * 0.01f;
                    
                    log = lifeTime + " | " + rate;

                    this.gameObject.GetComponent<HandleNetworkFunctions>().CmdSetParticle(index, lifeTime, rate);
                    if (g.transform.GetComponent<ParameterBars>() != null) {
                        ParticleSystem particle = g.GetComponent<ParticleSystem>();
                        var r = particle.emission.rate;
                        g.transform.GetComponent<ParameterBars>().active();
                        g.transform.GetComponent<ParameterBars>().values[0] = g.transform.localScale.x * scale / 4.0f;
                        g.transform.GetComponent<ParameterBars>().values[1] = (particle.startLifetime + lifeTime) / 5.0f;
                        g.transform.GetComponent<ParameterBars>().values[2] = (r.constant + rate) / 12.0f;
                    }
                }

                float rotationMagnitude = Mathf.Max(Mathf.Abs(angleTwist) * 2, pos); ;
                float scallingMagnitude = Mathf.Abs(scale-1)*100;
                


                if (gestureOperation == 0 && (rotationMagnitude > 2 || scallingMagnitude > 2)) {
                    //log = angleTwist + "|" + pos + "|" + scallingMagnitude;
                    if (rotationMagnitude > scallingMagnitude) {
                        gestureOperation = 1;
                    } else {
                        gestureOperation = 2;
                    }
                }
                if (gestureOperation == 0) setCurrentOperation(OPERATION_NONE);
                else if (gestureOperation == 1) setCurrentOperation(OPERATION_ROTATE);
                else if (gestureOperation == 2) setCurrentOperation(OPERATION_RESIZE);

                if (gestureOperation != 2) {
                    this.gameObject.GetComponent<HandleNetworkFunctions>().Rotate(index, avg, axisTwist, angleTwist);
                    this.gameObject.GetComponent<HandleNetworkFunctions>().Rotate(index, avg, axis, pos);
                }
                if (gestureOperation != 1) {
                    Vector3 dir = g.transform.position - avg;
                    this.gameObject.GetComponent<HandleNetworkFunctions>().CmdScale(index, scale, dir);
                }
            }
        }

        private Vector3 avgCenterOfObjects(List<int> objects) {
            Vector3 avg = Vector3.zero;
            foreach (var index in objects) {
                var g = ObjectManager.Get(index);
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