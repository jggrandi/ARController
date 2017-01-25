using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SetupScene : NetworkBehaviour {

    string userID;
    string sceneID;

    public GameObject playerObject;

    public void StartScene() {
        SetSceneID();

        CmdStartScene();

        //SceneManager.LoadScene("workingWithNetNew");

    }
    
    [Command]
    void CmdStartScene() {
        if(int.Parse(sceneID) == 0)
            MyNetworkManager.singleton.ServerChangeScene("workingWithNetNew");
        else if (int.Parse(sceneID) == 1)
            MyNetworkManager.singleton.ServerChangeScene("workingWithNetNew2");
    }

    public void SetSceneID() {
        sceneID = GameObject.Find("InputFieldSceneID").transform.FindChild("Text").GetComponent<Text>().text;
    }

}
