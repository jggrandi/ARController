using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace Lean.Touch {
    public class NetHandleSelectionTouch : NetworkBehaviour {

        public GameObject trackedObjects;

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
        Color blueColor = new Color(0 / 255.0f, 118 / 255.0f, 255 / 255.0f);

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


        public List<GameObject> objSelected = new List<GameObject>();

        [ClientRpc]
        void RpcAddSelected(GameObject g) {
            objSelected.Add(g);
        }
        [Command]
        void CmdAddSelected(GameObject g) {
            RpcAddSelected(g);
        }
        [ClientRpc]
        void RpcClearSelected() {
            objSelected.Clear();
        }
        [Command]
        void CmdClearSelected() {
            RpcClearSelected();
        }
        public void CmdSyncSelected() {
            CmdClearSelected();
            foreach (GameObject g in MainController.control.objSelectedNow) {
                CmdAddSelected(g);
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

            // if (selectedMaterial == null)
            selectedMaterial = (Material)Resources.Load("Light Blue");

            if (!isLocalPlayer) return;

            trackedObjects = GameObject.Find("TrackedObjects");
            lines = GameObject.Find("Lines");
            var obj = GameObject.Find("VolumetricLinePrefab");
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


        void Update() {

            if (!isLocalPlayer) return;

            linesUsed = 0;

            foreach (var player in GameObject.FindGameObjectsWithTag("player")) {
                //if (player.GetComponent<NetworkIdentity>().isLocalPlayer) continue;
                var selected = new List<GameObject>(player.GetComponent<NetHandleSelectionTouch>().objSelected);

                if (selected.Count == 0) continue;

                float minDist = float.MaxValue;
                GameObject minObj = null;
                var camera = player.transform.GetChild(0).gameObject.transform.position;


                Color color = greyColor;
                if (player.GetComponent<NetworkIdentity>().isLocalPlayer) {
                    camera -= new Vector3(0, 0.3f, 0);
                    color = new Color(0 / 255.0f, 118 / 255.0f, 255 / 255.0f);
                }

                    foreach (var g in selected) {
                    var dist = Vector3.Magnitude(g.transform.position - camera);
                    if (dist < minDist) {
                        minDist = dist;
                        minObj = g;
                    }
                }

                AddLine(camera, minObj.transform.position, color);
                
                List<GameObject> visited = new List<GameObject>();
                visited.Add(minObj);
                selected.Remove(minObj);


                while (selected.Count > 0) {

                    minDist = float.MaxValue;
                    GameObject minObjA = null;
                    GameObject minObjB = null;

                    foreach (var a in visited) {
                        foreach (var b in selected) {
                            var dist = Vector3.Magnitude(a.transform.position - b.transform.position);
                            if (dist < minDist) {
                                minDist = dist;
                                minObjA = a;
                                minObjB = b;
                            }
                        }
                    }
                    AddLine(minObjA.transform.position, minObjB.transform.position, color);

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
                if (MainController.control.objSelectedNow.Count > 0) {
                    UnselectAll();
                    CmdSyncSelected();
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
                if (MainController.control.objSelectedNow.Count > 0) { // only multiple selection when there is at least one object in the selectednow list
                    MainController.control.isMultipleSelection = true;
                    Select(finger, component);
                    sync = true;
                }
            }
            if(sync) CmdSyncSelected();

        }

        public void UnselectAll() {
            MainController.control.isMultipleSelection = false;
            foreach (GameObject g in MainController.control.objSelectedNow)
                g.transform.GetComponent<Renderer>().material = g.transform.GetComponent<ObjectGroupId>().material;

            MainController.control.objSelectedNow.Clear();
        }

        public void Select(GameObject obj) {
            obj.GetComponent<Renderer>().material = selectedMaterial;
            
           
            
            MainController.control.objSelectedNow.Add(obj);
        }

        public void Select(LeanFinger finger, Component obj) {

            if (!MainController.control.isMultipleSelection) {
                UnselectAll();
            }

            GameObject objToRemove = null;
            bool objIsSelected = false;
            foreach (GameObject g in MainController.control.objSelectedNow) {
                if (g == obj.transform.gameObject) {
                    objIsSelected = true;
                    objToRemove = g;
                    break;
                }
            }

            if (objIsSelected) {
                MainController.control.objSelectedNow.Remove(objToRemove);
                obj.transform.GetComponent<Renderer>().material = obj.transform.GetComponent<ObjectGroupId>().material;
                if (MainController.control.objSelectedNow.Count == 0)
                    MainController.control.isMultipleSelection = false;
                return;
            }

            if (DataSyncRef.Groups[Utils.GetIndex(obj.transform.gameObject)] != -1) { // if the object is in a group
                int idToSelect = DataSyncRef.Groups[Utils.GetIndex(obj.transform.gameObject)]; // take the obj id
                if (MainController.control.objSelectedNow.Count > 0 && DataSyncRef.Groups[Utils.GetIndex(MainController.control.objSelectedNow[0])] == idToSelect)
                    Select(obj.transform.gameObject);
                else {
                    for (int i = 0; i < trackedObjects.transform.childCount; i++) { // and find the other objects in the same group
                        if (DataSyncRef.Groups[i] == idToSelect) {
                            Debug.Log(trackedObjects.transform.GetChild(i).transform.gameObject.name);
                            Select(trackedObjects.transform.GetChild(i).transform.gameObject); // select them
                        }
                    }
                }

            } else {
                Select(obj.transform.gameObject);
            }

        }
    }
}

