using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Lean.Touch {

    public class HandleTransformations : MonoBehaviour {


        public GameObject trackedObjetecs;
        public GameObject lockedObjects;

        public int Mode = 0;

        private float rotSpeed = 1.0f;

        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {

            if (MainController.control.lockTransform) {
                foreach (GameObject g in MainController.control.objSelectedNow) {
                    g.transform.parent = lockedObjects.transform;
                }
            } else {
                foreach (GameObject g in MainController.control.objSelectedNow) {
                    g.transform.parent = trackedObjetecs.transform;
                }
            }
            
            switch (MainController.control.transformationNow) {
                case (int)Utils.Transformations.Translation:
                    Mode = 0;
                    break;
                case (int)Utils.Transformations.Rotation:
                    Mode = 1;
                    break;
                case (int)Utils.Transformations.Scale:
                    Mode = 2;
                    break;
                default:
                    break;
            }
            
            var fingers = LeanTouch.GetFingers(true, 2);
            if (fingers != null) OnGesture(fingers);

        }
        protected virtual void OnEnable() {
            // Hook into the events we need
            LeanTouch.OnFingerDown += OnFingerDown;
            LeanTouch.OnFingerSet += OnFingerSet;
            LeanTouch.OnFingerUp += OnFingerUp;
            LeanTouch.OnFingerTap += OnFingerTap;
            LeanTouch.OnFingerSwipe += OnFingerSwipe;
            LeanTouch.OnFingerHeldDown += OnFingerHeldDown;
            LeanTouch.OnFingerHeldSet += OnFingerHeld;
            LeanTouch.OnFingerHeldUp += OnFingerHeldUp;
           // LeanTouch.OnGesture += OnGesture;
        }

        protected virtual void OnDisable() {
            // Unhook the events
            LeanTouch.OnFingerDown -= OnFingerDown;
            LeanTouch.OnFingerSet -= OnFingerSet;
            LeanTouch.OnFingerUp -= OnFingerUp;
            LeanTouch.OnFingerTap -= OnFingerTap;
            LeanTouch.OnFingerSwipe -= OnFingerSwipe;
            LeanTouch.OnFingerHeldDown -= OnFingerHeldDown;
            LeanTouch.OnFingerHeldSet -= OnFingerHeld;
            LeanTouch.OnFingerHeldUp -= OnFingerHeldUp;
           // LeanTouch.OnGesture -= OnGesture;
        }

        public void OnFingerDown(LeanFinger finger) {
            Debug.Log("Finger " + finger.Index + " began touching the screen");
        }

        public void OnFingerSet(LeanFinger finger) {
            if (LeanTouch.Fingers.Count != 1) return;
            if (Mode == 0) {
                foreach (GameObject g in MainController.control.objSelectedNow) {

                    g.transform.position += Camera.main.transform.right * finger.ScreenDelta.x * 0.005f;
                    g.transform.position += Camera.main.transform.up * finger.ScreenDelta.y * 0.005f;


                }
            } else if (Mode == 1) {
                Vector3 avg = Vector3.zero;
                foreach (GameObject g in MainController.control.objSelectedNow) {
                    avg += g.transform.position;
                }
                avg /= MainController.control.objSelectedNow.Count;
                
                Vector3 axis = Camera.main.transform.right * finger.ScreenDelta.y + Camera.main.transform.up * -finger.ScreenDelta.x;

                foreach (GameObject g in MainController.control.objSelectedNow) {
                    g.transform.RotateAround(avg, axis, finger.ScreenDelta.magnitude*0.1f);

                }



                }
            Debug.Log("Finger " + finger.Index + " is still touching the screen");
        }

        public void OnFingerUp(LeanFinger finger) {
            Debug.Log("Finger " + finger.Index + " finished touching the screen");
        }

        public void OnFingerTap(LeanFinger finger) {
            Debug.Log("Finger " + finger.Index + " tapped the screen");
        }

        public void OnFingerSwipe(LeanFinger finger) {
            Debug.Log("Finger " + finger.Index + " swiped the screen");
        }

        public void OnFingerHeldDown(LeanFinger finger) {
            Debug.Log("Finger " + finger.Index + " began touching the screen for a long time");
        }

        public void OnFingerHeld(LeanFinger finger) {
            Debug.Log("Finger " + finger.Index + " is still touching the screen for a long time");
        }

        public void OnFingerHeldUp(LeanFinger finger) {
            Debug.Log("Finger " + finger.Index + " stopped touching the screen for a long time");
        }

        public void OnGesture(List<LeanFinger> fingers) {
            if (Mode == 0) {

                Vector3 avg = Vector3.zero;
                foreach (GameObject g in MainController.control.objSelectedNow) {
                    avg += g.transform.position;
                }
                avg /= MainController.control.objSelectedNow.Count;

                Vector3 dir = avg - Camera.main.transform.position;

                foreach (GameObject g in MainController.control.objSelectedNow) {
                    g.transform.position += dir * LeanGesture.GetScreenDelta(fingers).y * 0.01f;
                }
            } else if (Mode == 1) {

                Vector3 avg = Vector3.zero;
                foreach (GameObject g in MainController.control.objSelectedNow) {
                    avg += g.transform.position;
                }
                avg /= MainController.control.objSelectedNow.Count;

                float angle = LeanGesture.GetTwistDegrees(fingers);
                //Vector3 delta = LeanGesture.GetScreenDelta(fingers);
                Vector3 axis = Camera.main.transform.forward;

                foreach (GameObject g in MainController.control.objSelectedNow) {
                    g.transform.RotateAround(avg, axis, angle);
                }

            } else if (Mode == 2) {

                Vector3 avg = Vector3.zero;
                foreach (GameObject g in MainController.control.objSelectedNow) {
                    avg += g.transform.position;
                }
                avg /= MainController.control.objSelectedNow.Count;

                float scale = LeanGesture.GetPinchScale(fingers);

                foreach (GameObject g in MainController.control.objSelectedNow) {
                    Scale(g, scale, avg);

                }

            }
        }
        void handleTranslation() {


        }


        void handleRotation() {


        }

        void handleScale() {

        }

        private void Scale(GameObject obj, float scale, Vector3 center) {

            Vector3 dir = obj.transform.position - center;
            obj.transform.position += dir *(-1 + scale);
            obj.transform.localScale *= scale;
            
        }

    }
}