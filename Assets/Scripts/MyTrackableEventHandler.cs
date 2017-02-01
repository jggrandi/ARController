/*==============================================================================
Copyright (c) 2010-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Protected under copyright and other laws.
==============================================================================*/

using UnityEngine;
using System.Collections;


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
            TrackedObjects.SetActive(true);

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
        public void OnTrackableStateChanged(
                                        TrackableBehaviour.Status previousStatus,
                                        TrackableBehaviour.Status newStatus) {
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


        private void OnTrackingFound() {
            Renderer[] rendererComponents = TrackedObjects.GetComponentsInChildren<Renderer>(true);
            Collider[] colliderComponents = TrackedObjects.GetComponentsInChildren<Collider>(true);
                       
            foreach (Renderer component in rendererComponents) {
                component.enabled = true;
            }

            foreach (Collider component in colliderComponents)
                component.enabled = true;
            
            MainController.control.targetsTrackedNow++;

        }


        private void OnTrackingLost() {
            Renderer[] rendererComponents = TrackedObjects.GetComponentsInChildren<Renderer>(true);
            Collider[] colliderComponents = TrackedObjects.GetComponentsInChildren<Collider>(true);

            MainController.control.targetsTrackedNow--;

            if (MainController.control.targetsTrackedNow <= 0) {
                MainController.control.targetsTrackedNow = 0;
                foreach (Renderer component in rendererComponents)
                    component.enabled = false;
                
                foreach (Collider component in colliderComponents)
                    component.enabled = false;
                
            }

            //Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
        }

        #endregion // PRIVATE_METHODS
    }
}