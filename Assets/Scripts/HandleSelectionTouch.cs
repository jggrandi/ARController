using UnityEngine;
using System.Collections;
using System;

namespace Lean.Touch {
    public class HandleSelectionTouch : MonoBehaviour {

        public GameObject trackedObjects;

        [Tooltip("Ignore fingers with StartedOverGui?")]
        public bool IgnoreGuiFingers = true;

        [Tooltip("How many times must this finger tap before OnFingerTap gets called? (0 = every time)")]
        public int RequiredTapCount = 0;

        [Tooltip("How many times repeating must this finger tap before OnFingerTap gets called? (e.g. 2 = 2, 4, 6, 8, etc) (0 = every time)")]
        public int RequiredTapInterval;

        [Tooltip("This stores the layers we want the raycast to hit (make sure this GameObject's layer is included!)")]
        public LayerMask LayerMask = Physics.DefaultRaycastLayers;

        GameObject objToRemove;

        bool isMultipleSelection = false;

        protected virtual void OnEnable() {
            // Hook into the events we need
            LeanTouch.OnFingerTap += OnFingerTap;
            LeanTouch.OnFingerHeldDown += OnFingerHeldDown;
        }

        protected virtual void OnDisable() {
            // Unhook the events
            LeanTouch.OnFingerTap -= OnFingerTap;
            LeanTouch.OnFingerHeldDown -= OnFingerHeldDown;
        }

        private void OnFingerTap(LeanFinger finger) {
            // Ignore this tap?
            if (IgnoreGuiFingers == true && finger.StartedOverGui == true) return;
            if (RequiredTapCount > 0 && finger.TapCount != RequiredTapCount) return;
            if (RequiredTapInterval > 0 && (finger.TapCount % RequiredTapInterval) != 0) return;

            Select(finger);

        }

        private void OnFingerHeldDown(LeanFinger finger) {
            if (IgnoreGuiFingers == true && finger.StartedOverGui == true) return;
            isMultipleSelection = true;
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
             
                if (isMultipleSelection) {
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
                } else {
                    if (MainController.control.objSelectedNow != null) {

                        foreach (GameObject g in MainController.control.objSelectedNow) {
                            g.transform.GetComponent<Renderer>().material.color = Color.white;
                        }

                        MainController.control.objSelectedNow.Clear();
                        if (component.transform.gameObject.GetComponent<ObjectGroupId>().id != -1) {
                            int idToSelect = component.transform.gameObject.GetComponent<ObjectGroupId>().id;

                            for (int i = 0; i < trackedObjects.transform.childCount; i++) {
                                if (trackedObjects.transform.GetChild(i).transform.gameObject.GetComponent<ObjectGroupId>().id == idToSelect) {
                                    trackedObjects.transform.GetChild(i).transform.gameObject.GetComponent<Renderer>().material.color = Color.yellow;
                                    MainController.control.objSelectedNow.Add(trackedObjects.transform.GetChild(i).transform.gameObject);
                                }
                            }

                        }else {
                            component.transform.GetComponent<Renderer>().material.color = Color.yellow;
                            MainController.control.objSelectedNow.Add(component.transform.gameObject);

                        }
                    }
                }
            } else {
                isMultipleSelection = false;
                foreach (GameObject g in MainController.control.objSelectedNow) {
                    g.transform.GetComponent<Renderer>().material.color = Color.white;
                }
                MainController.control.objSelectedNow.Clear();
                
            }
        }
    }
}

