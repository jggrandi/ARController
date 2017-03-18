using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MyNetworkManager : NetworkManager {

    //public override void OnClientConnect(NetworkConnection conn) {
    //    base.OnClientConnect(conn);
    //    GameObject.Find("MainHandler").GetComponent<NetworkIdentity>().AssignClientAuthority(conn);
    //}

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
        base.OnServerAddPlayer(conn,playerControllerId);
        GameObject player = null;
        foreach(PlayerController p in conn.playerControllers) {
            if(p.playerControllerId == playerControllerId) {
                player = p.gameObject;
                break;
            }
        }
    }

    public void StartMyHost() {
        MainController.Landscape = GameObject.Find("Panel").transform.GetChild(4).GetComponent<Toggle>().isOn;
        SetPort();
        NetworkManager.singleton.StartHost();
        ShowLoading();
    }

    public void SetPort() {
        NetworkManager.singleton.networkPort = 7777;
    }

    public void JoinGame() {
        MainController.Landscape = GameObject.Find("Panel").transform.GetChild(4).GetComponent<Toggle>().isOn;
        SetIpAddress();
        SetPort();
        NetworkManager.singleton.StartClient();
        ShowLoading();
    }


    public void ShowLoading() {
        GameObject.Find("Panel").transform.GetChild(0).gameObject.SetActive(true);
        GameObject.Find("Panel").transform.GetChild(1).gameObject.SetActive(false);
        GameObject.Find("Panel").transform.GetChild(2).gameObject.SetActive(false);
        GameObject.Find("Panel").transform.GetChild(3).gameObject.SetActive(false);
        GameObject.Find("Panel").transform.GetChild(4).gameObject.SetActive(false);
        GameObject.Find("Panel").transform.GetChild(5).gameObject.SetActive(false);

    }


    void SetIpAddress() {
        string ipAddress = GameObject.Find("InputFieldIPAddress").transform.FindChild("Text").GetComponent<Text>().text;
        NetworkManager.singleton.networkAddress = ipAddress;
    }

}
