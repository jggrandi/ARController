using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;

public class Log{

	StreamWriter fUsersActions;
	//StreamWriter fResume;

	public Log(string group, int task)
	{

        Debug.Log(Application.persistentDataPath);
		fUsersActions = File.CreateText(Application.persistentDataPath + "/Group-" + group + "-Task-" + task + "---" + System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "-UserActions.csv");
        string header = "Time";
        for (int i = 0; i < 2; i++) // hard coded for two clients (temporary)
            header += ";UserID;Camera X;Camera Y;Camera Z;PieceID;Modality;TargetsTracked;Translation X;Translation Y;Translation Z;Rotation X;Rotation Y;Rotation Z;Rotation W;Scale";
        fUsersActions.WriteLine(header);

        //		fResume = File.CreateText(Application.persistentDataPath + "/User-" + user + "-Task-" + task + "---" + System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "-Resume.csv");
        //string header = "Time;PieceID;IsSelected;DistanceID;RotationID;RotAngle;Modality;Translation X;Translation Y;Translation Z;Rotation X;Rotation Y;Rotation Z;Rotation W;Camera X;Camera Y;Camera Z;Error Trans Mat; Error Rot Mat;Error Rot Angle;Tracked Targets";
        //fUsersActions.WriteLine(header);
        //		header = "PieceID;DistanceID;RotationID;RotAngle;Time;Error Trans;Error Rot;Error Rot Angle";
        //		fResume.WriteLine(header);

    }

    public void close()
	{
		fUsersActions.Close();
//		fResume.Close();
	}

    public void flush() {
        fUsersActions.Flush();
//        fResume.Flush();
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

	//public void saveVerbose(bool isTraining, int pieceID, bool isSelected, int distanceID, int rotationID, int rotAngle, int modality, GameObject piece, Vector3 cameraPosition, float errorTrans, float errorRot, float errorRotAngle, int trackedTargets)
	//{

	//    String line = "";

 //   	line += Time.realtimeSinceStartup + "";
 //       line += ";";
 //       if (isTraining) line += "T";
 //       line += pieceID + ";" + isSelected + ";" + distanceID + ";" + rotationID + ";" + rotAngle + ";" + modality;
 //       line += ";" + piece.transform.localPosition.x + ";" + piece.transform.localPosition.y + ";" + piece.transform.localPosition.z;
 //       line += ";" + piece.transform.localRotation.x + ";" + piece.transform.localRotation.y + ";" + piece.transform.localRotation.z + ";" + piece.transform.localRotation.w;
 //       line += ";" + cameraPosition.x + ";" + cameraPosition.y + ";" + cameraPosition.z;
	//	line += ";" + errorTrans + ";" + errorRot + ";" + errorRotAngle;
	//	line += ";" + trackedTargets;
 //       fVerbose.WriteLine(line);
 //       fVerbose.Flush();
	//}


	//public void saveResume(bool isTraining, int pieceID, int distanceID, int rotationID, int rotAngle,  float time, float errorTrans, float errorRot, float errorRotAngle)
	//{
	//	String line = "";
 //       if (isTraining) line += "T";
	//	line += pieceID + ";" + distanceID + ";" + rotationID + ";" + rotAngle;
	//	line += ";" + time;
	//	line += ";" + errorTrans + ";" + errorRot + ";" + errorRotAngle;
	//	fResume.WriteLine(line);
	//	fResume.Flush();
	//}

}

