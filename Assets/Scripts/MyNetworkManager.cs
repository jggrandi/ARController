using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MyNetworkManager : NetworkManager {

    //public override void OnClientConnect(NetworkConnection conn) {
    //    base.OnClientConnect(conn);
    //    GameObject.Find("MainHandler").GetComponent<NetworkIdentity>().AssignClientAuthority(conn);
    //}

    public void StartHost() {
        SetPort();
        NetworkManager.singleton.StartHost();
    }

    public void SetPort() {
        NetworkManager.singleton.networkPort = 7777;
    }

    public void JoinGame() {
        SetPort();
        NetworkManager.singleton.StartClient();
    }

    void SetIpAddress() {
        string ipAddress = GameObject.Find("InputFieldIPAddress").transform.FindChild("Text").GetComponent<Text>().text;
        NetworkManager.singleton.networkAddress = ipAddress;
    }

    void OnLevelWasLoaded(int level) {
        if (level == 0) {
            GameObject.Find("ButtonStartHost").GetComponent<Button>().onClick.RemoveAllListeners();
            GameObject.Find("ButtonStartHost").GetComponent<Button>().onClick.AddListener(StartHost);

            GameObject.Find("ButtonJoin").GetComponent<Button>().onClick.RemoveAllListeners();
            GameObject.Find("ButtonJoin").GetComponent<Button>().onClick.AddListener(JoinGame);
        }

    }
}
