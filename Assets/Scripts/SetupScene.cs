using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SetupScene : NetworkBehaviour {



    public GameObject playerObject;

    void Start() {
        foreach (var player in GameObject.FindGameObjectsWithTag("player")) {
            player.gameObject.SetActive(false);

        }
    }

    public void StartScene() {
        SetSceneID();
        SetUSerID();
        CmdStartScene();

        //SceneManager.LoadScene("workingWithNetNew");

    }
    
    [Command]
    void CmdStartScene() {
        if(int.Parse(TestController.tcontrol.sceneID) == 0)
            MyNetworkManager.singleton.ServerChangeScene("workingWithNetNew");
        else if (int.Parse(TestController.tcontrol.sceneID) == 1)
            MyNetworkManager.singleton.ServerChangeScene("workingWithNetNew2");
    }

    public void SetSceneID() {
        TestController.tcontrol.sceneID = GameObject.Find("InputFieldSceneID").GetComponent<InputField>().text;

    }

    public void SetUSerID() {
        TestController.tcontrol.userID = GameObject.Find("InputFieldUserID").GetComponent<InputField>().text;
    }

}
