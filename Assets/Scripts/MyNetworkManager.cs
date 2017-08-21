using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class MyNetworkManager : NetworkManager {

    public string userID;

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
        if (TestController.tcontrol.sceneIndex == TestController.tcontrol.tasksToPermute + 1) return; //The +1 is because of the howtouse initial task
        base.OnServerAddPlayer(conn, playerControllerId);
        
    }


    //public override void OnClientConnect(NetworkConnection conn) {
    //    connID = conn.connectionId;
    //    base.OnClientConnect(conn);
    //}

    public override void OnClientSceneChanged(NetworkConnection conn) {
        StartCoroutine(Wait(conn));
    }


    IEnumerator Wait(NetworkConnection conn) { // wait until saving the resumed log.
        yield return new WaitForSeconds(1);
        base.OnClientSceneChanged(conn);
    }

    public override void ServerChangeScene(string newSceneName) {
        base.ServerChangeScene(newSceneName);
    }

    public void StartMyHost() {
        SetPort();
        userID = "0";
        NetworkManager.singleton.StartHost();
    }

    public void SetPort() {
        NetworkManager.singleton.networkPort = 7777;
    }

    public void JoinGame() {
        if (!SetUserID()) return;
        SetPort();
        SetIpAddress();
        NetworkManager.singleton.StartClient();
    }

    void SetIpAddress() {
        string ipAddress = GameObject.Find("InputFieldIPAddress").transform.FindChild("Text").GetComponent<Text>().text;
        NetworkManager.singleton.networkAddress = ipAddress;
    }

    bool SetUserID() {
        string uID = GameObject.Find("UserID").transform.FindChild("Text").GetComponent<Text>().text;
        if (uID.Length != 0){
            userID = uID;
            return true;
        }
        return false;
    }



}