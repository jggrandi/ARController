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
        if (isServer) {
            GameObject.Find("PanelClient").gameObject.SetActive(false);
            GameObject.Find("PanelServer").gameObject.SetActive(true);

            GameObject.Find("InputFieldUserID").GetComponent<InputField>().text = TestController.tcontrol.userID.ToString();
            TestController.tcontrol.taskOrder = Utils.selectUserTaskSequence(TestController.tcontrol.userID, TestController.tcontrol.tasksToPermute);
            GameObject.Find("InputFieldSceneID").GetComponent<InputField>().text = TestController.tcontrol.taskOrder[TestController.tcontrol.sceneID].ToString();

        } else {
            GameObject.Find("PanelClient").gameObject.SetActive(true);
            GameObject.Find("PanelServer").gameObject.SetActive(false);

        }


    }

    void Update() {
        if (!isServer) return;
        if (int.Parse(GameObject.Find("InputFieldSceneID").GetComponent<InputField>().text) != TestController.tcontrol.taskOrder[TestController.tcontrol.idNow])    
            GameObject.Find("InputFieldSceneID").GetComponent<InputField>().text = TestController.tcontrol.taskOrder[TestController.tcontrol.idNow].ToString();

        if (int.Parse(GameObject.Find("InputFieldUserID").GetComponent<InputField>().text) != TestController.tcontrol.userID) {
            GameObject.Find("InputFieldUserID").GetComponent<InputField>().text = TestController.tcontrol.userID.ToString();
        }

    }

    public void StartScene() {
        CmdStartScene();

    }

    public void ManuallyChangeScene() {
        TestController.tcontrol.sceneID = int.Parse(GameObject.Find("InputFieldSceneID").GetComponent<InputField>().text);
    }


    public void UpdataUserId() {
        if (GameObject.Find("InputFieldUserID").GetComponent<InputField>().text == "") return;
        
        CmdUpdateUser();
        UpdateScene();

    }

    void UpdateScene() {
        TestController.tcontrol.taskOrder = Utils.selectUserTaskSequence(TestController.tcontrol.userID, TestController.tcontrol.tasksToPermute);

        CmdUpdateScene();
        GameObject.Find("InputFieldSceneID").GetComponent<InputField>().text = TestController.tcontrol.sceneID.ToString();
    }

    [Command]
    void CmdUpdateUser() {
        TestController.tcontrol.userID = int.Parse(GameObject.Find("InputFieldUserID").GetComponent<InputField>().text);
    }

    [Command]
    void CmdUpdateScene() {
        TestController.tcontrol.sceneID = TestController.tcontrol.taskOrder[TestController.tcontrol.idNow];
    }

    [Command]
    void CmdStartScene() {
        if(TestController.tcontrol.sceneID == 0)
            MyNetworkManager.singleton.ServerChangeScene("workingWithNetNew");
        else if (TestController.tcontrol.sceneID == 1)
            MyNetworkManager.singleton.ServerChangeScene("workingWithNetNew2");
    }

    public void SetSceneID() {
        TestController.tcontrol.sceneID = int.Parse(GameObject.Find("InputFieldSceneID").GetComponent<InputField>().text);

    }

    public void SetUSerID() {
        TestController.tcontrol.userID = int.Parse(GameObject.Find("InputFieldUserID").GetComponent<InputField>().text);
    }

}
