using UnityEngine;
using System.Collections;

public class NetHandleGroup : MonoBehaviour {

    public void CreateGroup() {
        foreach (GameObject g in MainController.control.objSelectedNow) {
            this.gameObject.transform.GetComponent<HandleNetworkFunctions>().CmdSetGroup(g);
        }
        this.gameObject.transform.GetComponent<HandleNetworkFunctions>().CmdIncrementCount();
    }


    public void UnGroup() {
        
        foreach (GameObject g in MainController.control.objSelectedNow) {
            this.gameObject.transform.GetComponent<HandleNetworkFunctions>().CmdSetGroup2(g, -1);
            g.GetComponent<Renderer>().material.color = Color.white;
        }
        
        MainController.control.objSelectedNow.Clear();
    }
}
