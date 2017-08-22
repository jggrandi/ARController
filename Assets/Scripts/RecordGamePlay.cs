using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;

public class Log{

	StreamWriter fUsersActions;
	StreamWriter fPiecesState;

	public Log(string group, int task,List<int> piecesList)
	{

        Debug.Log(Application.persistentDataPath);
		fUsersActions = File.CreateText(Application.persistentDataPath + "/Group-" + group + "-Task-" + task + "---" + System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "-UserActions.csv");
        fPiecesState = File.CreateText(Application.persistentDataPath + "/Group-" + group + "-Task-" + task + "---" + System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "-PiecesState.csv");
        string header = "Time";
        for (int i = 0; i < 2; i++) // hard coded for two clients (temporary)
            header += ";UserID;Camera X;Camera Y;Camera Z;PieceID;Modality;TargetsTracked;Translation X;Translation Y;Translation Z;Rotation X;Rotation Y;Rotation Z;Rotation W;Scale";
        fUsersActions.WriteLine(header);

        header = "Time";
        foreach (int piece in piecesList)
            header += ";P" + piece + "Time" + ";P" + piece + "ErrorTrans" + ";P" + piece + "ErrorRot" + ";P" + piece + "ErrorScale";
        fPiecesState.WriteLine(header);

    }

    public void close()
	{
		fUsersActions.Close();
		fPiecesState.Close();
	}

    public void flush() {
        fUsersActions.Flush();
        fPiecesState.Flush();
    }

    public void saveUserActions(GameObject[] gs) {
        String line = "";
        line += Time.realtimeSinceStartup + "";
        
        foreach(GameObject player in gs) {
            if (player.GetComponent<NetworkIdentity>().isLocalPlayer) continue;
            line += ";" + player.GetComponent<PlayerStuff>().userID;
            Vector3 cam = player.GetComponent<Lean.Touch.NetHandleSelectionTouch>().CameraPosition;
            line += ";" + cam.x + ";" + cam.y + ";" + cam.z;
            if (player.GetComponent<Lean.Touch.NetHandleSelectionTouch>().objSelectedShared.Count <= 0) line += ";;;;;;;;;;;";
            else {
                line += ";" + player.GetComponent<Lean.Touch.NetHandleSelectionTouch>().objSelectedShared[0];
                line += ";" + player.GetComponent<Lean.Touch.NetHandleTransformations>().modality;
                line += ";" + player.GetComponent<PlayerStuff>().targetsTracked;
                Vector3 trans = player.GetComponent<HandleNetworkFunctions>().objTranslateStep;
                Quaternion rot = player.GetComponent<HandleNetworkFunctions>().objRotStep;
                line += ";" + trans.x + ";" + trans.y + ";" + trans.z;
                line += ";" + rot.x + ";" + rot.y + ";" + rot.z + ";" + rot.w;
                line += ";" + player.GetComponent<HandleNetworkFunctions>().objScaleStep;
            }
        }
        fUsersActions.WriteLine(line);
        fUsersActions.Flush();
    }


    public void savePiecesState(List<int> pieces,List<float> timers, List<float> errorTrans, List<float> errorRot, List<float> errorScale) {
        String line = "";
        line += Time.realtimeSinceStartup + "";

        foreach (int piece in pieces)
            line += ";" + timers[piece] + ";" + errorTrans[piece] + ";" + errorRot[piece] + ";" + errorScale[piece];
        
        fPiecesState.WriteLine(line);
        fPiecesState.Flush();

    }


}

