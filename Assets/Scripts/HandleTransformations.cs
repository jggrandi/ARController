using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Lean.Touch {

    public class HandleTransformations : MonoBehaviour {


        public GameObject trackedObjetecs;
        public GameObject lockedObjects;

        //public int Mode = 0;
		Utils.Transformations mode = 0;

        // Use this for initialization
        void Start() {
        trackedObjetecs = GameObject.Find("TrackedObjects");
        lockedObjects = GameObject.Find("LockedObjects");
            
        }

    // Update is called once per frame
    void Update() {

            var fingers = LeanTouch.GetFingers(true, 2);

            if (MainController.control.lockTransform) {
                foreach (GameObject g in MainController.control.objSelectedNow) {
                    g.transform.parent = lockedObjects.transform;
                }
            } else {
                foreach (GameObject g in MainController.control.objSelectedNow) {
                    g.transform.parent = trackedObjetecs.transform;
                }
            }
            
			mode = MainController.control.transformationNow;
            
            
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


        public void OnFingerSet(LeanFinger finger) {  // one finger on the screen
            if (LeanTouch.Fingers.Count < 1) return;
            if (finger.IsOverGui) return;

            if (mode == Utils.Transformations.Translation) {  // translate in x and y axis
                foreach (GameObject g in MainController.control.objSelectedNow) {
                    g.transform.position += Camera.main.transform.right * finger.ScreenDelta.x * 0.005f;
                    g.transform.position += Camera.main.transform.up * finger.ScreenDelta.y * 0.005f;
                }
            } else if (mode == Utils.Transformations.Rotation) { // rotate in the x and y axis
                Vector3 avg = avgCenterOfObjects(MainController.control.objSelectedNow);
                Vector3 axis = Camera.main.transform.right * finger.ScreenDelta.y + Camera.main.transform.up * -finger.ScreenDelta.x;

                foreach (GameObject g in MainController.control.objSelectedNow) {
                    g.transform.RotateAround(avg, axis, finger.ScreenDelta.magnitude * 0.1f);

                }
            }
        }
        

        public void OnGesture(List<LeanFinger> fingers) {  // two fingers on screen
            if (mode == Utils.Transformations.Translation) { // translate the object near or far away from the camera position

                Vector3 avg = avgCenterOfObjects(MainController.control.objSelectedNow);
                Vector3 dir = avg - Camera.main.transform.position;

                foreach (GameObject g in MainController.control.objSelectedNow) {
                    g.transform.position += dir * LeanGesture.GetScreenDelta(fingers).y * 0.01f;

                }
            } else if (mode == Utils.Transformations.Rotation) { // rotate the object around the 3rd axis

                Vector3 avg = avgCenterOfObjects(MainController.control.objSelectedNow);

                float angle = LeanGesture.GetTwistDegrees(fingers);
                Vector3 axis = Camera.main.transform.forward;

                foreach (GameObject g in MainController.control.objSelectedNow) {
                    g.transform.RotateAround(avg, axis, angle);
                }

            } else if (mode == Utils.Transformations.Scale) { // pinch to scale up and down

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