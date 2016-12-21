using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Lean.Touch {

    public class NetHandleTransformations : NetworkBehaviour {


        public GameObject trackedObjetcs;
        public GameObject lockedObjects;

        //int Mode = 0;
		Utils.Transformations mode = Utils.Transformations.Translation;

        private float rotSpeed = 1.0f;

        // Use this for initialization
        void Start() {
            trackedObjetcs = GameObject.Find("TrackedObjects");
            lockedObjects = GameObject.Find("LockedObjects");
            
        }

    // Update is called once per frame
    void Update() {
            if (!isLocalPlayer) return;
           
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
            
			mode = MainController.control.transformationNow;
            /*switch (MainController.control.transformationNow) {
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
            }*/
            
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




        public void OnFingerSet(LeanFinger finger) {  // one finger on the screen
            if (!isLocalPlayer) return;

            if (LeanTouch.Fingers.Count < 1) return;
            if (mode == Utils.Transformations.Translation) {  // translate in x and y axis
                foreach (GameObject g in MainController.control.objSelectedNow) {
                    Vector3 right = Camera.main.transform.right * finger.ScreenDelta.x * 0.005f;
                    Vector3 up = Camera.main.transform.up * finger.ScreenDelta.y * 0.005f;
                    this.gameObject.GetComponent<HandleNetworkFunctions>().CmdTranslate(g, right);
                    this.gameObject.GetComponent<HandleNetworkFunctions>().CmdTranslate(g, up);
                }
            } else if (mode == Utils.Transformations.Rotation) { // rotate in the x and y axis
                Vector3 avg = avgCenterOfObjects(MainController.control.objSelectedNow);
                Vector3 axis = Camera.main.transform.right * finger.ScreenDelta.y + Camera.main.transform.up * -finger.ScreenDelta.x;
                float magnitude = finger.ScreenDelta.magnitude;
                foreach (GameObject g in MainController.control.objSelectedNow) {
                    this.gameObject.GetComponent<HandleNetworkFunctions>().CmdRotate(g, avg, axis, magnitude);

                }
            }
        }
        

        public void OnGesture(List<LeanFinger> fingers) {  // two fingers on screen
            if (!isLocalPlayer) return;

            if (mode == Utils.Transformations.Translation) { // translate the object near or far away from the camera position

                Vector3 avg = avgCenterOfObjects(MainController.control.objSelectedNow);
                Vector3 dir = avg - Camera.main.transform.position * LeanGesture.GetScreenDelta(fingers).y * 0.005f;

                foreach (GameObject g in MainController.control.objSelectedNow) {
                    this.gameObject.GetComponent<HandleNetworkFunctions>().CmdTranslate(g, dir);
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