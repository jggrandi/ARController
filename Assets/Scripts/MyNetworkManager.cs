using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MyNetworkManager : NetworkManager {

    //public override void OnClientConnect(NetworkConnection conn) {
    //    base.OnClientConnect(conn);
    //    GameObject.Find("MainHandler").GetComponent<NetworkIdentity>().AssignClientAuthority(conn);
    //}

   
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
        base.OnServerAddPlayer(conn,playerControllerId);
        //GameObject player = (GameObject)Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        //NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        GameObject player = null;
        foreach (PlayerController p in conn.playerControllers) {
            if (p.playerControllerId == playerControllerId) {
                player = p.gameObject;
                break;
            }
        }
    }

    public override void ServerChangeScene(string newSceneName) {
        //SceneManager.LoadScene(newSceneName);
        base.ServerChangeScene(newSceneName);
    }

    public void StartMyHost() {
        SetPort();
        NetworkManager.singleton.StartHost();
        
    }

    public void SetPort() {
        NetworkManager.singleton.networkPort = 7777;
    }

    public void JoinGame() {
        SetPort();
        SetIpAddress();
        NetworkManager.singleton.StartClient();
    }

    void SetIpAddress() {
        string ipAddress = GameObject.Find("InputFieldIPAddress").transform.FindChild("Text").GetComponent<Text>().text;
        NetworkManager.singleton.networkAddress = ipAddress;
    }

}
