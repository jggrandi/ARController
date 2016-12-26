/*==============================================================================
Copyright (c) 2010-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Protected under copyright and other laws.
==============================================================================*/

using UnityEngine;
using System.Collections;


namespace Vuforia
{
    
    /// <summary>
    /// A custom handler that implements the ITrackableEventHandler interface.
    /// </summary>
    public class MyTrackableEventHandler : MonoBehaviour,ITrackableEventHandler
    {
        #region PRIVATE_MEMBER_VARIABLES
        
        private TrackableBehaviour mTrackableBehaviour;

        #endregion // PRIVATE_MEMBER_VARIABLES

        public GameObject TrackedObjects;
        public bool tracking = false;
        #region UNTIY_MONOBEHAVIOUR_METHODS
    
        void Awake()
        {
            TrackedObjects.SetActive(true);
            Debug.Log("AQUI");
            mTrackableBehaviour = GetComponent<TrackableBehaviour>();
            if (mTrackableBehaviour)
            {
                mTrackableBehaviour.RegisterTrackableEventHandler(this);
            }
        }

        #endregion // UNTIY_MONOBEHAVIOUR_METHODS

        int stayActive = 0;
        
        void Update() {
            stayActive++;
            if (stayActive <= 50)
                TrackedObjects.SetActive(true);
        }


        #region PUBLIC_METHODS

        /// <summary>
        /// Implementation of the ITrackableEventHandler function called when the
        /// tracking state changes.
        /// </summary>
        public void OnTrackableStateChanged(
                                        TrackableBehaviour.Status previousStatus,
                                        TrackableBehaviour.Status newStatus)
        {
            if (newStatus == TrackableBehaviour.Status.DETECTED ||
                newStatus == TrackableBehaviour.Status.TRACKED ||
                newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
            {
                OnTrackingFound();
            }
            else
            {
                OnTrackingLost();
            }
        }

        #endregion // PUBLIC_METHODS



        #region PRIVATE_METHODS


        private void OnTrackingFound()
        {
            //Renderer[] rendererComponents = TrackedObjects.GetComponentsInChildren<Renderer>(true);
            //Collider[] colliderComponents = TrackedObjects.GetComponentsInChildren<Collider>(true);

            //// Enable rendering:
            //foreach (Renderer component in rendererComponents)
            //{
            //    component.enabled = true;
            //}

            //// Enable colliders:
            //foreach (Collider component in colliderComponents)
            //{
            //    component.enabled = true;
            //}
            this.transform.GetChild(0).gameObject.SetActive(true);
            MainController.control.targetsTrackedNow++;
            TrackedObjects.SetActive(true);
            for(int i = 0; i < TrackedObjects.transform.childCount; i++) {
                TrackedObjects.transform.GetChild(i).gameObject.SetActive(true);
            }
            //Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");
        }


        private void OnTrackingLost()
        {
            //Renderer[] rendererComponents = TrackedObjects.GetComponentsInChildren<Renderer>(true);
            //Collider[] colliderComponents = TrackedObjects.GetComponentsInChildren<Collider>(true);

            //// Disable rendering:
            //foreach (Renderer component in rendererComponents)
            //{
            //    component.enabled = false;
            //}

            //// Disable colliders:
            //foreach (Collider component in colliderComponents)
            //{
            //    component.enabled = false;
            //}

            
            MainController.control.targetsTrackedNow--;
            if(this.transform.childCount > 0)
                this.transform.GetChild(0).gameObject.SetActive(false);
            if (MainController.control.targetsTrackedNow <= 0) {
                MainController.control.targetsTrackedNow = 0;
                TrackedObjects.SetActive(false);
            }
                
            //Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
        }

        #endregion // PRIVATE_METHODS
    }
}
