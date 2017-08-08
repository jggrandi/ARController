using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SetupScene : NetworkBehaviour {

    public GameObject playerObject;


    void OnGUI() {
        GameObject NetMan = GameObject.Find("NetworkManager");
        //string connID = NetMan.GetComponent<MyNetworkManager>().connID.ToString();
        var centeredStyle = GUI.skin.GetStyle("Label");
        Color myColor = new Color();
        //centeredStyle.alignment = TextAnchor.UpperCenter;
        centeredStyle.fontSize = 30;
        centeredStyle.fontStyle = FontStyle.Normal;
        ColorUtility.TryParseHtmlString("#323232FF", out myColor);
        centeredStyle.normal.textColor = myColor;
        //if(!isServer)
        //    GUI.Label(new Rect(Screen.width / 2 - 250, Screen.height / 2-40, 500, 100), "Get Ready\nPlayer ID: "+connID, centeredStyle);
    }


    void Start() {
        foreach (var player in GameObject.FindGameObjectsWithTag("player")) {
            player.gameObject.SetActive(false);
        }
        
        if (isServer) {
            GameObject.Find("PanelClient").gameObject.SetActive(false);
            GameObject.Find("PanelServer").gameObject.SetActive(true);

            GameObject.Find("InputFieldGroupID").GetComponent<InputField>().text = TestController.tcontrol.groupID.ToString();
            int[] order = Utils.selectTaskSequence(TestController.tcontrol.groupID, TestController.tcontrol.tasksToPermute);
            TestController.tcontrol.taskOrder.Clear();
			TestController.tcontrol.taskOrder.Add (-1);
            for (int i = 0; i < order.Length; i++)
                TestController.tcontrol.taskOrder.Add(order[i]);

            if (TestController.tcontrol.sceneIndex > TestController.tcontrol.taskOrder.Count - 1)
                MyNetworkManager.singleton.ServerChangeScene("EndTest");

            GameObject.Find("InputFieldSceneID").GetComponent<InputField>().text = TestController.tcontrol.taskOrder[TestController.tcontrol.sceneIndex].ToString();
            GameObject.Find("InputFieldSceneNow").GetComponent<InputField>().text = TestController.tcontrol.sceneIndex.ToString();            
        } else {
            GameObject.Find("PanelClient").gameObject.SetActive(true);
            GameObject.Find("PanelServer").gameObject.SetActive(false);
            if(TestController.tcontrol.userID != 0)
                GameObject.Find("InputFieldUserID").GetComponent<InputField>().text = TestController.tcontrol.userID.ToString();
        }
    }

    void Update() {
        foreach (var player in GameObject.FindGameObjectsWithTag("player")) {
            player.gameObject.SetActive(false);
        }
    }


    public void StartScene() {
        if(isServer)
            CmdStartScene();
    }

    public void UpdateGroupId() {
        if (GameObject.Find("InputFieldGroupID").GetComponent<InputField>().text == "") return;

        CmdUpdateGroup();
        UpdateScene();
    }

    public void UpdataUserId() {
        if (GameObject.Find("InputFieldUserID").GetComponent<InputField>().text == "") return;
        TestController.tcontrol.userID = int.Parse(GameObject.Find("InputFieldUserID").GetComponent<InputField>().text);
    }

    void UpdateScene() {
        Debug.Log(TestController.tcontrol.groupID);
        int[] order = Utils.selectTaskSequence(TestController.tcontrol.groupID, TestController.tcontrol.tasksToPermute);
        TestController.tcontrol.taskOrder.Clear();

		TestController.tcontrol.taskOrder.Add (-1);
		for (int i = 0; i < order.Length; i++)
            TestController.tcontrol.taskOrder.Add(order[i]);
        

        GameObject.Find("InputFieldSceneID").GetComponent<InputField>().text = TestController.tcontrol.taskOrder[TestController.tcontrol.sceneIndex].ToString();
        GameObject.Find("InputFieldSceneNow").GetComponent<InputField>().text = TestController.tcontrol.sceneIndex.ToString();
        Debug.Log(TestController.tcontrol.sceneIndex);
        CmdUpdateScene();
    }

    [Command]
    void CmdUpdateGroup() {
        TestController.tcontrol.groupID = int.Parse(GameObject.Find("InputFieldGroupID").GetComponent<InputField>().text);
    }

    [Command]
    void CmdUpdateScene() {
        TestController.tcontrol.sceneIndex = int.Parse(GameObject.Find("InputFieldSceneNow").GetComponent<InputField>().text);
    }

    [Command]
    void CmdStartScene() {
        if (TestController.tcontrol.sceneIndex == 0)
            MyNetworkManager.singleton.ServerChangeScene("Trainning");
        else if (TestController.tcontrol.sceneIndex == 1)
            MyNetworkManager.singleton.ServerChangeScene("Task1");
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
        if (TestController.tcontrol.sceneIndex < 3) {
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
