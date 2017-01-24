using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SetupScene : MonoBehaviour {

    string userID;
    string sceneID;

    public void StartScene() {
        SetSceneID();
        Debug.Log(sceneID);
        MyNetworkManager.singleton.ServerChangeScene("workingWithNetNew");
        //SceneManager.LoadScene("workingWithNetNew");

    }


    public void SetSceneID() {
        sceneID = GameObject.Find("InputFieldSceneID").transform.FindChild("Text").GetComponent<Text>().text;
    }

}
