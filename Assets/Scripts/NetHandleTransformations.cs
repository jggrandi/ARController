using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Lean.Touch {

    public class NetHandleTransformations : NetworkBehaviour {


        public GameObject trackedObjetcs;
        public GameObject lockedObjects;

        public int Mode = 0;

        private float rotSpeed = 1.0f;

        // Use this for initialization
        void Start() {
            trackedObjetcs = GameObject.Find("TrackedObjects");
            lockedObjects = GameObject.Find("LockedObjects");
            
        }

    // Update is called once per frame
    void Update() {

            //var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
            //var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;
            //CmdTranslate(x, z);

            if (MainController.control.lockTransform) {
                foreach (GameObject g in MainController.control.objSelectedNow) {
                    g.transform.parent = lockedObjects.transform;
                }
            } else {
                foreach (GameObject g in MainController.control.objSelectedNow) {
                    g.transform.parent = trackedObjetcs.transform;
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
            LeanTouch.OnFingerSet += OnFingerSet;
        }

        protected virtual void OnDisable() {
            // Unhook the events
            LeanTouch.OnFingerSet -= OnFingerSet;

        }

        [Command]
        void CmdTranslate(GameObject g, Vector3 vec) {
            g.transform.position += vec;
        }

        [Command]
        void CmdRotate(GameObject g, Vector3 avg, Vector3 axis, float mag) {
            g.transform.RotateAround(avg, axis, mag * 0.1f);
        }

        [Command]
        void CmdScale(GameObject g, Vector3 right, Vector3 up) {

        }


        public void OnFingerSet(LeanFinger finger) {  // one finger on the screen
            if (LeanTouch.Fingers.Count < 1) return;
            if (Mode == (int)Utils.Transformations.Translation) {  // translate in x and y axis
                foreach (GameObject g in MainController.control.objSelectedNow) {
                    Vector3 right = Camera.main.transform.right * finger.ScreenDelta.x * 0.005f;
                    Vector3 up = Camera.main.transform.up * finger.ScreenDelta.y * 0.005f;
                    CmdTranslate(g, right);
                    CmdTranslate(g, up);
                }
            } else if (Mode == (int)Utils.Transformations.Rotation) { // rotate in the x and y axis
                Vector3 avg = avgCenterOfObjects(MainController.control.objSelectedNow);
                Vector3 axis = Camera.main.transform.right * finger.ScreenDelta.y + Camera.main.transform.up * -finger.ScreenDelta.x;
                float magnitude = finger.ScreenDelta.magnitude;
                foreach (GameObject g in MainController.control.objSelectedNow) {
                    CmdRotate(g, avg, axis, magnitude);

                }
            }
        }
        

        public void OnGesture(List<LeanFinger> fingers) {  // two fingers on screen
            if (Mode == (int)Utils.Transformations.Translation) { // translate the object near or far away from the camera position

                Vector3 avg = avgCenterOfObjects(MainController.control.objSelectedNow);
                Vector3 dir = avg - Camera.main.transform.position * LeanGesture.GetScreenDelta(fingers).y * 0.005f;

                foreach (GameObject g in MainController.control.objSelectedNow) {
                    CmdTranslate(g, dir);
                }
            } else if (Mode == (int)Utils.Transformations.Rotation) { // rotate the object around the 3rd axis

                Vector3 avg = avgCenterOfObjects(MainController.control.objSelectedNow);

                float angle = LeanGesture.GetTwistDegrees(fingers);
                Vector3 axis = Camera.main.transform.forward;

                foreach (GameObject g in MainController.control.objSelectedNow) {
                    g.transform.RotateAround(avg, axis, angle);
                }

            } else if (Mode == (int)Utils.Transformations.Scale) { // pinch to scale up and down

                Vector3 avg = avgCenterOfObjects(MainController.control.objSelectedNow);

                float scale = LeanGesture.GetPinchScale(fingers);

                foreach (GameObject g in MainController.control.objSelectedNow) {
                    Scale(g, scale, avg);

                }

            }
        }

        private Vector3 avgCenterOfObjects(List<GameObject> objects) {

            Vector3 avg = Vector3.zero;
            foreach (GameObject g in objects) {
                avg += g.transform.position;
            }

            return avg /= objects.Count;
        } 

        private void Scale(GameObject obj, float scale, Vector3 center) {

            Vector3 dir = obj.transform.position - center;
            obj.transform.position += dir *(-1 + scale);
            obj.transform.localScale *= scale;
            
        }

    }
}