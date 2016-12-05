using UnityEngine;
using System.Collections;

public class MainController : MonoBehaviour {
    public static MainController control;

    public int targetsTrackedNow = 0;
    public int totalTargets = 0;

    void Awake() {

        if (control == null) {
            DontDestroyOnLoad(gameObject);
            control = this;

        } else if (control != this) {
            Destroy(gameObject);
        }

    }

    // Update is called once per frame
    void Update () {
        //Debug.Log("tracked now: " + targetsTrackedNow);
	}
}
