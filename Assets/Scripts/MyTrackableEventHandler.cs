/*==============================================================================
Copyright (c) 2010-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Protected under copyright and other laws.
==============================================================================*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Vuforia {

    /// <summary>
    /// A custom handler that implements the ITrackableEventHandler interface.
    /// </summary>
    public class MyTrackableEventHandler : MonoBehaviour, ITrackableEventHandler {
        #region PRIVATE_MEMBER_VARIABLES

        private TrackableBehaviour mTrackableBehaviour;

        #endregion // PRIVATE_MEMBER_VARIABLES

        public GameObject TrackedObjects;
        
        #region UNTIY_MONOBEHAVIOUR_METHODS

        void Awake() {

            mTrackableBehaviour = GetComponent<TrackableBehaviour>();
            if (mTrackableBehaviour) {
                mTrackableBehaviour.RegisterTrackableEventHandler(this);
            }

        }

        #endregion // UNTIY_MONOBEHAVIOUR_METHODS



        #region PUBLIC_METHODS

        /// <summary>
        /// Implementation of the ITrackableEventHandler function called when the
        /// tracking state changes.
        /// </summary>
        public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus,TrackableBehaviour.Status newStatus) {
            if (newStatus == TrackableBehaviour.Status.DETECTED ||
                newStatus == TrackableBehaviour.Status.TRACKED ||
                newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED) {
                OnTrackingFound();
            } else {
                OnTrackingLost();
            }
        }

        #endregion // PUBLIC_METHODS



        #region PRIVATE_METHODS


        private bool findTrackable(int i) {
            foreach (int value in MainController.control.trackedTargets)
                if (value == i)
                    return true;
            return false;
        } 

        private void OnTrackingFound() {
            Renderer[] rendererComponents = TrackedObjects.GetComponentsInChildren<Renderer>(true);
            Collider[] colliderComponents = TrackedObjects.GetComponentsInChildren<Collider>(true);
            Rigidbody[] rigidbodyComponents = TrackedObjects.GetComponentsInChildren<Rigidbody>(true);

            foreach (Renderer component in rendererComponents)
                component.enabled = true;
            
            foreach (Collider component in colliderComponents)
                component.enabled = true;

            foreach (Rigidbody component in rigidbodyComponents) {
                component.detectCollisions = true;
                component.isKinematic = false;
            }

            
            if(!findTrackable(mTrackableBehaviour.Trackable.ID))
                MainController.control.trackedTargets.Add(mTrackableBehaviour.Trackable.ID);

            //Debug.Log("Qnt Targets " + trackedTargets.Count );
            //Debug.Log("ID " + mTrackableBehaviour.Trackable.ID);
        }


        private void OnTrackingLost() {
            Renderer[] rendererComponents = TrackedObjects.GetComponentsInChildren<Renderer>(true);
            Collider[] colliderComponents = TrackedObjects.GetComponentsInChildren<Collider>(true);
            Rigidbody[] rigidbodyComponents = TrackedObjects.GetComponentsInChildren<Rigidbody>(true);

            if (findTrackable(mTrackableBehaviour.Trackable.ID))
                MainController.control.trackedTargets.Remove(mTrackableBehaviour.Trackable.ID);
            //Debug.Log("Qnt Targets " + trackedTargets.Count);
            if (MainController.control.trackedTargets.Count <= 0) { 

            //if (MainController.control.targetsTrackedNow <= 0) {
            //    MainController.control.targetsTrackedNow = 0;
            foreach (Renderer component in rendererComponents)
                    component.enabled = false;
                
                foreach (Collider component in colliderComponents)
                    component.enabled = false;

                foreach (Rigidbody component in rigidbodyComponents) {
                    component.detectCollisions = false;
                    component.isKinematic = true;
                }

            }

            
        }

        #endregion // PRIVATE_METHODS
    }
}