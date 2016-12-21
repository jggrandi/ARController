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


        bool isMultipleSelection = false;
        bool isFingerMoving = false;

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

        void Start() {
            trackedObjects = GameObject.Find("TrackedObjects");
        }

        void Update() {
            if (LeanTouch.Fingers.Count == 1) {
                if(LeanTouch.Fingers[0].ScreenDelta.magnitude > 0.001f)
                isFingerMoving = true;
            }else {
                isFingerMoving = false;
            }
        }
        
        private void OnFingerTap(LeanFinger finger) {
            // Ignore this tap?
            if (LeanTouch.Fingers.Count > 1) return;
            if (IgnoreGuiFingers == true && finger.StartedOverGui == true) return;
            if (RequiredTapCount > 0 && finger.TapCount != RequiredTapCount) return;
            if (RequiredTapInterval > 0 && (finger.TapCount % RequiredTapInterval) != 0) return;

            var ray = finger.GetRay();// Get ray for finger
            var hit = default(RaycastHit);// Stores the raycast hit info
            var component = default(Component);// Stores the component we hit (Collider)

            if (Physics.Raycast(ray, out hit, float.PositiveInfinity, LayerMask) == true) { // if the finger touched an object
                component = hit.collider;
                Select(finger, component);
            } else {
                UnselectAll();
            }
        }

        private void OnFingerHeldDown(LeanFinger finger) {

            if (IgnoreGuiFingers == true && finger.StartedOverGui == true) return;
            if (isFingerMoving) return;

            var ray = finger.GetRay();// Get ray for finger
            var hit = default(RaycastHit);// Stores the raycast hit info
            var component = default(Component);// Stores the component we hit (Collider)

            if (Physics.Raycast(ray, out hit, float.PositiveInfinity, LayerMask) == true) { // se tocou em um objeto
                component = hit.collider;
                if (MainController.control.objSelectedNow.Count > 0) { // only multiple selection when there is at least one object in the selectednow list
                    isMultipleSelection = true;
                    Select(finger, component);
                }
            }

        }

        public void UnselectAll() {
            isMultipleSelection = false;
            foreach (GameObject g in MainController.control.objSelectedNow)
                g.transform.GetComponent<Renderer>().material.color = Color.white;

            MainController.control.objSelectedNow.Clear();
        }
        
        public void Select(GameObject obj) {
            obj.GetComponent<Renderer>().material.color = Color.yellow;
            MainController.control.objSelectedNow.Add(obj);
        }

        public void Select(LeanFinger finger, Component obj) {
            
            if (!isMultipleSelection) {
                UnselectAll();
            }

            GameObject objToRemove = null;
            bool objIsSelected = false;
            foreach (GameObject g in MainController.control.objSelectedNow) {
                if (g.name == obj.transform.gameObject.name) {
                    objIsSelected = true;
                    objToRemove = g;
                    break;
                }
            }

            if (objIsSelected) {
                MainController.control.objSelectedNow.Remove(objToRemove);
                obj.transform.GetComponent<Renderer>().material.color = Color.white;
                return;
            }

            if (obj.transform.gameObject.GetComponent<ObjectGroupId>().id != -1) {
                int idToSelect = obj.transform.gameObject.GetComponent<ObjectGroupId>().id;

                for (int i = 0; i < trackedObjects.transform.childCount; i++) {
                    if (trackedObjects.transform.GetChild(i).transform.gameObject.GetComponent<ObjectGroupId>().id == idToSelect) {
                        Select(trackedObjects.transform.GetChild(i).transform.gameObject);
                    }
                }

            } else {
                Select(obj.transform.gameObject);
            }

        }
    }
}

