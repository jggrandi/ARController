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
            int[] order = Utils.selectUserTaskSequence(TestController.tcontrol.userID, TestController.tcontrol.tasksToPermute);
            TestController.tcontrol.taskOrder.Clear();
            for (int i = 0; i < order.Length; i++)
                TestController.tcontrol.taskOrder.Add(order[i]);

            if (TestController.tcontrol.sceneIndex > TestController.tcontrol.taskOrder.Count - 1)
                MyNetworkManager.singleton.ServerChangeScene("EndTest");

            GameObject.Find("InputFieldSceneID").GetComponent<InputField>().text = TestController.tcontrol.taskOrder[TestController.tcontrol.sceneIndex].ToString();
            GameObject.Find("InputFieldSceneNow").GetComponent<InputField>().text = TestController.tcontrol.sceneIndex.ToString();

        } else {
            GameObject.Find("PanelClient").gameObject.SetActive(true);
            GameObject.Find("PanelServer").gameObject.SetActive(false);
        }
    }


    public void StartScene() {
        if(isServer)
            CmdStartScene();
    }

    public void UpdataUserId() {
        if (GameObject.Find("InputFieldUserID").GetComponent<InputField>().text == "") return;

        CmdUpdateUser();
        UpdateScene();
    }

    void UpdateScene() {
        int[] order = Utils.selectUserTaskSequence(TestController.tcontrol.userID, TestController.tcontrol.tasksToPermute);
        TestController.tcontrol.taskOrder.Clear();
        for (int i = 0; i < order.Length; i++)
            TestController.tcontrol.taskOrder.Add(order[i]);
        

        GameObject.Find("InputFieldSceneID").GetComponent<InputField>().text = TestController.tcontrol.taskOrder[TestController.tcontrol.sceneIndex].ToString();
        GameObject.Find("InputFieldSceneNow").GetComponent<InputField>().text = TestController.tcontrol.sceneIndex.ToString();
        CmdUpdateScene();
    }

    [Command]
    void CmdUpdateUser() {
        TestController.tcontrol.userID = int.Parse(GameObject.Find("InputFieldUserID").GetComponent<InputField>().text);
    }

    [Command]
    void CmdUpdateScene() {
        TestController.tcontrol.sceneIndex = int.Parse(GameObject.Find("InputFieldSceneNow").GetComponent<InputField>().text);
    }

    [Command]
    void CmdStartScene() {
        MyNetworkManager.singleton.ServerChangeScene("workingWithNetNew");
    }

    [Command]
    void CmdIncrementSceneID() {
        TestController.tcontrol.sceneIndex++;
    }

    [Command]
    void CmdDecrementSceneID() {
        TestController.tcontrol.sceneIndex--;
    }

    public void ButtonNextScene() {
        if (TestController.tcontrol.sceneIndex < 2) {
            CmdIncrementSceneID();
            GameObject.Find("InputFieldSceneNow").GetComponent<InputField>().text = TestController.tcontrol.sceneIndex.ToString();
            GameObject.Find("InputFieldSceneID").GetComponent<InputField>().text = TestController.tcontrol.taskOrder[TestController.tcontrol.sceneIndex].ToString();
            CmdUpdateScene();
        }
    }

    public void ButtonPreviousScene() {
        if (TestController.tcontrol.sceneIndex > 0) {
            CmdDecrementSceneID();
            GameObject.Find("InputFieldSceneNow").GetComponent<InputField>().text = TestController.tcontrol.sceneIndex.ToString();
            GameObject.Find("InputFieldSceneID").GetComponent<InputField>().text = TestController.tcontrol.taskOrder[TestController.tcontrol.sceneIndex].ToString();
            CmdUpdateScene();
        }
    }
}
