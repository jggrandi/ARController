using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Lean.Touch {

    public class HandleTransformations : NetworkBehaviour {


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
            LeanTouch.OnFingerSet += OnFingerSet;
        }

        protected virtual void OnDisable() {
            // Unhook the events
            LeanTouch.OnFingerSet -= OnFingerSet;

        }

        [Command]
        public void CmdTranslate() {
            Debug.Log("asdasfasf amerda");
           // g.transform.position += v;
        }



        public void OnFingerSet(LeanFinger finger) {  // one finger on the screen
            if (LeanTouch.Fingers.Count < 1) return;
            if (Mode == (int)Utils.Transformations.Translation) {  // translate in x and y axis
                foreach (GameObject g in MainController.control.objSelectedNow) {
                    CmdTranslate();
                }
            } else if (Mode == (int)Utils.Transformations.Rotation) { // rotate in the x and y axis
                Vector3 avg = avgCenterOfObjects(MainController.control.objSelectedNow);
                Vector3 axis = Camera.main.transform.right * finger.ScreenDelta.y + Camera.main.transform.up * -finger.ScreenDelta.x;

                foreach (GameObject g in MainController.control.objSelectedNow) {
                    g.transform.RotateAround(avg, axis, finger.ScreenDelta.magnitude * 0.1f);

                }
            }
        }
        

        public void OnGesture(List<LeanFinger> fingers) {  // two fingers on screen
            if (Mode == (int)Utils.Transformations.Translation) { // translate the object near or far away from the camera position

                Vector3 avg = avgCenterOfObjects(MainController.control.objSelectedNow);
                Vector3 dir = avg - Camera.main.transform.position;

                foreach (GameObject g in MainController.control.objSelectedNow) {
                    g.transform.position += dir * LeanGesture.GetScreenDelta(fingers).y * 0.01f;

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