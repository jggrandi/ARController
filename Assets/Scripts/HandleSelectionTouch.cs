using UnityEngine;
using System.Collections;
using System;

namespace Lean.Touch {
    public class HandleSelectionTouch : MonoBehaviour {

        [Tooltip("Ignore fingers with StartedOverGui?")]
        public bool IgnoreGuiFingers = true;

        [Tooltip("How many times must this finger tap before OnFingerTap gets called? (0 = every time)")]
        public int RequiredTapCount = 0;

        [Tooltip("How many times repeating must this finger tap before OnFingerTap gets called? (e.g. 2 = 2, 4, 6, 8, etc) (0 = every time)")]
        public int RequiredTapInterval;

        [Tooltip("This stores the layers we want the raycast to hit (make sure this GameObject's layer is included!)")]
        public LayerMask LayerMask = Physics.DefaultRaycastLayers;

        GameObject objToRemove;

        protected virtual void OnEnable() {
            // Hook into the events we need
            LeanTouch.OnFingerTap += OnFingerTap;
        }

        protected virtual void OnDisable() {
            // Unhook the events
            LeanTouch.OnFingerTap -= OnFingerTap;
        }

        private void OnFingerTap(LeanFinger finger) {
            // Ignore this tap?
            if (IgnoreGuiFingers == true && finger.StartedOverGui == true) return;
            if (RequiredTapCount > 0 && finger.TapCount != RequiredTapCount) return;
            if (RequiredTapInterval > 0 && (finger.TapCount % RequiredTapInterval) != 0) return;

            Select(finger);

        }


        public void Select(LeanFinger finger) {
            // Get ray for finger
            var ray = finger.GetRay();

            // Stores the raycast hit info
            var hit = default(RaycastHit);

            // Stores the component we hit (Collider)
            var component = default(Component);

            // Was this finger pressed down on a collider?
            bool objIsSelected = false;

            if (Physics.Raycast(ray, out hit, float.PositiveInfinity, LayerMask) == true) {
                component = hit.collider;
                foreach (GameObject g in MainController.control.objSelectedNow) {
                    if (g.name == component.transform.gameObject.name) {
                        objIsSelected = true;
                        objToRemove = g;
                        break;
                    }
                }

                if (objIsSelected) {
                    MainController.control.objSelectedNow.Remove(objToRemove);
                    component.transform.GetComponent<Renderer>().material.color = Color.white;

                } else {
                    component.transform.GetComponent<Renderer>().material.color = Color.yellow;
                    MainController.control.objSelectedNow.Add(component.transform.gameObject);
                }
            }

        }


            // Select the component

        }

        //Ray ray;
        //RaycastHit hit;
        //GameObject objToRemove;

        //// Use this for initialization
        //void Start() {
        //}

        //// Update is called once per frame
        //void Update() {

        //    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {
        //        ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        //        Debug.DrawRay(ray.origin, ray.direction * 20, Color.red);


        //        bool objIsSelected = false;

        //        if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
        //            foreach (GameObject g in MainController.control.objSelectedNow) {
        //                if (g.name == hit.transform.gameObject.name) {
        //                    objIsSelected = true;
        //                    objToRemove = g;
        //                    break;
        //                }
        //            }

        //            if (objIsSelected) {
        //                MainController.control.objSelectedNow.Remove(objToRemove);
        //                hit.transform.GetComponent<Renderer>().material.color = Color.white;

        //            } else {
        //                hit.transform.GetComponent<Renderer>().material.color = Color.yellow;
        //                MainController.control.objSelectedNow.Add(hit.transform.gameObject);
        //            }
        //        }
        //    }
        //}

    //}

}