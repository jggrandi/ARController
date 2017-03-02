using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace Lean.Touch {
    public class NetHandleSelectionTouch : NetworkBehaviour {

        public GameObject trackedObjects = null;

        [Tooltip("Ignore fingers with StartedOverGui?")]
        public bool IgnoreGuiFingers = true;

        [Tooltip("How many times must this finger tap before OnFingerTap gets called? (0 = every time)")]
        public int RequiredTapCount = 0;

        [Tooltip("How many times repeating must this finger tap before OnFingerTap gets called? (e.g. 2 = 2, 4, 6, 8, etc) (0 = every time)")]
        public int RequiredTapInterval;

        [Tooltip("This stores the layers we want the raycast to hit (make sure this GameObject's layer is included!)")]
        public LayerMask LayerMask = Physics.DefaultRaycastLayers;

        bool isFingerMoving = false;

        Color greyColor = new Color(150 / 255.0f, 150 / 255.0f, 150 / 255.0f);
        //Color blueColor = new Color(0 / 255.0f, 118 / 255.0f, 255 / 255.0f);

        /*public class SyncGameObject : SyncList<GameObject> {
            protected override GameObject DeserializeItem(NetworkReader reader) {
                return reader.ReadGameObject();
            }

            protected override void SerializeItem(NetworkWriter writer, GameObject item) {
                writer.Write(item);
            }
        }

        public SyncGameObject objSelected = new SyncGameObject();
        
        //[Command]
        public void CmdSyncSelected(List<GameObject> objs) {
            objSelected.Clear();
            foreach(GameObject g in objs) {
                objSelected.Add(g);
            }
        }*/


        public SyncListInt objSelectedShared = new SyncListInt();


        [Command]
        void CmdAddSelected(int index) {
            objSelectedShared.Add(index);
        }

        [Command]
        void CmdClearSelected() {
            objSelectedShared.Clear();
        }

        public void CmdSyncSelected() {
            CmdClearSelected();
            foreach (var i in MainController.control.objSelected) {
                CmdAddSelected(i);
            }
        }

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

        public GameObject lines;
        public Material selectedMaterial = null;

        DataSync DataSyncRef;
        public void Start() {
            DataSyncRef = GameObject.Find("MainHandler").GetComponent<DataSync>();

            if (selectedMaterial == null)
                selectedMaterial = (Material)Resources.Load("Light Blue");

            if (!isLocalPlayer) return;

            trackedObjects = GameObject.Find("TrackedObjects");
            lines = GameObject.Find("Lines");
            //var obj = GameObject.Find("VolumetricLinePrefab");
            /*for (int i = 0; i < 100; i++) {
                var g = (Instantiate(obj) as GameObject);
                g.transform.parent = lines.transform;
            }*/
            ClearLines();

        }


        int linesUsed = 0;
        void AddLine(Vector3 a, Vector3 b, Color c) {
            if (linesUsed >= lines.transform.childCount) return;
            var g = lines.transform.GetChild(linesUsed++).gameObject;
            var line = g.GetComponent<VolumetricLines.VolumetricLineBehavior>();
            //line.SetStartAndEndPoints(a, b);
            line.transform.position = a;
            line.transform.rotation = Quaternion.FromToRotation(new Vector3(0, 0, 1), (b - a).normalized);
            line.transform.localScale = new Vector3(1, 1, (b - a).magnitude);
            line.LineColor = c;
        }
        void ClearLines() {
            for (int i = linesUsed; i < lines.transform.childCount; i++) {
                var g = lines.transform.GetChild(i).gameObject;
                var line = g.GetComponent<VolumetricLines.VolumetricLineBehavior>();
                //line.SetStartAndEndPoints(new Vector3(5000.0f, 5000.0f, 5000.0f), new Vector3(5000.0f, 5000.0f, 5000.0f));
                line.transform.position = new Vector3(500, 500, 500);
            }
            linesUsed = 0;
        }

        public Vector3 CameraPosition;
        public int targetsTracked;

        [ClientRpc]
        public void RpcSetCameraPosition(Vector3 p) {

            if(trackedObjects == null) trackedObjects = GameObject.Find("TrackedObjects");
            p = trackedObjects.transform.TransformPoint(p);
            CameraPosition = p;
        }
        [Command]
        public void CmdSetCameraPosition(Vector3 p) {
            RpcSetCameraPosition(p);
        }

        [ClientRpc]
        public void RpcTragetsTracked(int t) {
            targetsTracked = t;
        }

        [Command]
        public void CmdTargetsTracked(int t) {
            RpcTragetsTracked(t);

        }

        [Command]
        public void CmdClearSelectedShared() {
            objSelectedShared.Clear();
        }


        public int unselectAllCount = -1;

        void Update() {

            if (!isLocalPlayer) return;

            if(unselectAllCount > 0 && --unselectAllCount == 0) {
                UnselectAll();
                CmdSyncSelected();
            }

            CmdSetCameraPosition(trackedObjects.transform.InverseTransformPoint(Camera.main.transform.position));
            CmdTargetsTracked(MainController.control.targetsTrackedNow);
            linesUsed = 0;

            foreach (var player in GameObject.FindGameObjectsWithTag("player")) {
                //if (player.GetComponent<NetworkIdentity>().isLocalPlayer) continue;
                var selected = new List<int>(player.GetComponent<NetHandleSelectionTouch>().objSelectedShared);

                if (selected.Count == 0) continue;

                float minDist = float.MaxValue;
                int minObj = -1;
                var camera = player.GetComponent<NetHandleSelectionTouch>().CameraPosition;
                
                Color color = greyColor;
                if (player.GetComponent<NetworkIdentity>().isLocalPlayer) {
                    camera = Camera.main.transform.position - Camera.main.transform.up.normalized * 0.4f + new Vector3(0.031f, 0.021f, 0.01f);
                    color = new Color(0 / 255.0f, 118 / 255.0f, 255 / 255.0f);
                }

                foreach (var index in selected) {
                    var g = ObjectManager.Get(index);
                    var dist = Vector3.Magnitude(g.transform.position - camera);
                    if (dist < minDist) {
                        minDist = dist;
                        minObj = index;
                    }
                }

                AddLine(camera, ObjectManager.Get(minObj).transform.position, color);
                
                List<int> visited = new List<int>();
                visited.Add(minObj);
                selected.Remove(minObj);


                while (selected.Count > 0) {

                    minDist = float.MaxValue;
                    int minObjA = -1;
                    int minObjB = -1;

                    foreach (var a in visited) {
                        foreach (var b in selected) {
                            var dist = Vector3.Magnitude(ObjectManager.Get(a).transform.position - ObjectManager.Get(b).transform.position);
                            if (dist < minDist) {
                                minDist = dist;
                                minObjA = a;
                                minObjB = b;
                            }
                        }
                    }
                    AddLine(ObjectManager.Get(minObjA).transform.position, ObjectManager.Get(minObjB).transform.position, color);

                    selected.Remove(minObjB);
                    visited.Add(minObjB);
                }

            }
            ClearLines();

            if (LeanTouch.Fingers.Count == 1) {
                if (LeanTouch.Fingers[0].ScreenDelta.magnitude > 0.001f)
                    isFingerMoving = true;
            } else {
                isFingerMoving = false;
            }
            
        }

        private void OnFingerTap(LeanFinger finger) {
            // Ignore this tap?
            if (!isLocalPlayer) return;
            //Debug.Log(MainController.control.isTapForTransform);
            //if (MainController.control.isTapForTransform) return;

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
                CmdSyncSelected();
            } else {
                
                //if (MainController.control.isTapForTransform ) return;
                if (MainController.control.objSelected.Count > 0) {
                    unselectAllCount = 10;
                    
                }
            }
        }

        private void OnFingerHeldDown(LeanFinger finger) {
            if (!isLocalPlayer) return;
            if (LeanTouch.Fingers.Count != 1) return;
            if (IgnoreGuiFingers == true && finger.StartedOverGui == true) return;
            if (isFingerMoving) return;

            var ray = finger.GetRay();// Get ray for finger
            var hit = default(RaycastHit);// Stores the raycast hit info
            var component = default(Component);// Stores the component we hit (Collider)


            bool sync = false;
            if (Physics.Raycast(ray, out hit, float.PositiveInfinity, LayerMask) == true) { // se tocou em um objeto
                component = hit.collider;
                if (MainController.control.objSelected.Count > 0) { // only multiple selection when there is at least one object in the selectednow list
                    MainController.control.isMultipleSelection = true;
                    Select(finger, component);
                    sync = true;
                }
            }
            if(sync) CmdSyncSelected();

        }

        public void UnselectAll() {
            MainController.control.isMultipleSelection = false;
            foreach (int i in MainController.control.objSelected) {
                GameObject g = ObjectManager.Get(i);
                g.transform.GetComponent<Renderer>().material = g.transform.GetComponent<ObjectGroupId>().material;
            }
            MainController.control.objSelected.Clear();
            CmdClearSelectedShared();
            MainController.control.isMultipleSelection = false;
        }

        public void Select(int index) {
            if (ObjectManager.Get(index).GetComponent<ParticleSystem>() == null) {
                ObjectManager.Get(index).GetComponent<Renderer>().material = selectedMaterial;
            }
            MainController.control.objSelected.Add(index);
        }

        public void Select(LeanFinger finger, Component obj) {

            int index = Utils.GetIndex(obj.transform.gameObject);
            if (!MainController.control.isMultipleSelection) {
                UnselectAll();
            }

            int objToRemove = -1;
            bool objIsSelected = false;
            foreach (var i in MainController.control.objSelected) {
                if (i == index) {
                    objIsSelected = true;
                    objToRemove = i;
                    break;
                }
            }

            if (objIsSelected) {
                MainController.control.objSelected.Remove(objToRemove);
                //obj.transform.GetComponent<Renderer>().material = obj.transform.GetComponent<ObjectGroupId>().material;
            
                if (MainController.control.objSelected.Count == 0)
                    MainController.control.isMultipleSelection = false;
                return;
            }

            if (DataSyncRef.Groups[index] != -1) { // if the object is in a group
                int idToSelect = DataSyncRef.Groups[index]; // take the obj id
                if (MainController.control.objSelected.Count > 0 && DataSyncRef.Groups[MainController.control.objSelected[0]] == idToSelect)
                    Select(index);
                else {
                    for (int i = 0; i < trackedObjects.transform.childCount; i++) { // and find the other objects in the same group
                        if (DataSyncRef.Groups[i] == idToSelect) {
                            Select(i); // select them
                        }
                    }
                }

            } else {
                Select(index);
            }

        }
    }
}

