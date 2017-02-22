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

        int translationZ = 0;
        int countFrames = 0;

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
            //mode = MainController.control.transformationNow;
            //this.gameObject.transform.GetComponent<HandleNetworkFunctions>().SyncCamPosition(Camera.main.transform.position);

            if (countFrames % 35 == 0) {
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
            MainController.control.isTapForTransform = false;
        }

        public int GetIndex(GameObject g) {
            return g.GetComponent<ObjectGroupId>().index;
        }

        private void OnFingerTap(LeanFinger finger) {
            translationZ = 1;
            if (MainController.control.objSelected.Count != 0)
                MainController.control.isTapForTransform = true;

        }

        float transFactor = 0.005f;

        public void OnFingerSet(LeanFinger finger) {  // one finger on the screen
            if (!isLocalPlayer) return;
            if (IgnoreGuiFingers == true && finger.StartedOverGui == true) return;
            if (LeanTouch.Fingers.Count != 1) return;

            if (translationZ == 1)
                translationZ = 2;

            if (translationZ == 2)
                
                MainController.control.isTapForTransform = true;

            this.gameObject.GetComponent<NetHandleSelectionTouch>().unselectAllCount = 0;

            foreach (var index in MainController.control.objSelected) {
                if (translationZ < 2) {
                    Vector3 right = Camera.main.transform.right.normalized * finger.ScreenDelta.x * transFactor;
                    Vector3 up = Camera.main.transform.up.normalized * finger.ScreenDelta.y * transFactor;
                    this.gameObject.transform.GetComponent<HandleNetworkFunctions>().Translate(index, Utils.PowVec3(right + up, 1.2f));
                } else if (translationZ == 2) {
                    Vector3 avg = avgCenterOfObjects(MainController.control.objSelected);
                    Vector3 translate = (avg - Camera.main.transform.position).normalized * finger.ScreenDelta.y * transFactor; // obj pos - cam pos

                    this.gameObject.GetComponent<HandleNetworkFunctions>().Translate(index, Utils.PowVec3(translate, 1.2f));
                }
            }
        }

        public void OnGesture(List<LeanFinger> fingers) {  // two fingers on screen
            if (!isLocalPlayer) return;
            if (LeanTouch.Fingers.Count != 2) return;

            Vector3 avg = avgCenterOfObjects(MainController.control.objSelected);
            Vector3 axis, axisTwist;

            float angleTwist = LeanGesture.GetTwistDegrees(fingers) * 0.8f;
            axisTwist = Camera.main.transform.forward.normalized;
            axis = Camera.main.transform.right.normalized * LeanGesture.GetScreenDelta(fingers).y + Camera.main.transform.up.normalized * -LeanGesture.GetScreenDelta(fingers).x;
            float pos = LeanGesture.GetScreenDelta(fingers).magnitude * 0.3f;
            float scale = LeanGesture.GetPinchScale(fingers);
            

            foreach (int index in MainController.control.objSelected) {
                var g = Utils.GetByIndex(index);

                if (g.GetComponent<ParticleSystem>() != null) {

                    float lifeTime = 1 + pos ;
                    float rate = 1 + angleTwist ;

                    this.gameObject.GetComponent<HandleNetworkFunctions>().CmdSetParticle(index, lifeTime, rate);

                    if (g.transform.GetComponent<ParameterBars>() != null) {

                        ParticleSystem particle = g.GetComponent<ParticleSystem>();
                        var r = particle.emission.rate;

                        g.transform.GetComponent<ParameterBars>().active();
                        g.transform.GetComponent<ParameterBars>().values[0] = g.transform.localScale.x * scale / 4.0f;
                        g.transform.GetComponent<ParameterBars>().values[1] = particle.startLifetime * lifeTime / 5.0f;
                        g.transform.GetComponent<ParameterBars>().values[2] = r.constant * rate / 12.0f;
                    }


                }

                this.gameObject.GetComponent<HandleNetworkFunctions>().Rotate(index, avg, axisTwist, angleTwist);
                this.gameObject.GetComponent<HandleNetworkFunctions>().Rotate(index, avg, axis, pos);
                Vector3 dir = g.transform.position - avg;
                this.gameObject.GetComponent<HandleNetworkFunctions>().CmdScale(index, scale, dir);

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