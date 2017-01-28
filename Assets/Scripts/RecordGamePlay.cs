using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;



public class Log{

	StreamWriter f;
	String filename = "";
	

	public Log(string user)
	{

        Debug.Log(Application.persistentDataPath);
		f = File.CreateText(Application.persistentDataPath + "/User-" + user + "---" + System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".csv");
		//f.WriteLine(n + ";" + c + ";" + team);

		string header = "Time;PieceID;DistanceToTarget;Translation X;Translation Y;Translation Z;Rotation X;Rotation Y;Rotation Z;Rotation W;Camera X;Camera Y;Camera Z;Error Trans;Error Rot";
		f.WriteLine(header);
	}

	public void close()
	{
		f.Close();
	}

	public void setFilename(String name){
		filename = name;
	}

	public void save(int pieceID, GameObject piece, Vector3 cameraPosition, float errorTrans, float errorRot) //, float distToTarget, GameObject t, Quaternion cameraRotation, float errorTrans, float errorRot)
	{

	//	//if (clients.Count < numberOfClients) return;

	    String line = "";

    	line += Time.realtimeSinceStartup + "";
        line += ";" + pieceID + ";" /*Add the distance here*/;
        line += ";" + piece.transform.localPosition.x + ";" + piece.transform.localPosition.y + ";" + piece.transform.localPosition.z;
        line += ";" + piece.transform.localRotation.x + ";" + piece.transform.localRotation.y + ";" + piece.transform.localRotation.z + ";" + piece.transform.localRotation.w;
        line += ";" + cameraPosition.x + ";" + cameraPosition.y + ";" + cameraPosition.z;
        line += ";" + errorTrans + ";" + errorRot;
        //	line += ";" + Convert.ToInt32(isInCollision) + ";" + collisionForce.x + ";" + collisionForce.y + ";" + collisionForce.z;

        //	for (int j = 0; j < numberOfCheckpoints; j++) {
        //		line += ";" + stackDist [j];
        //	}

        //	for (int j = 0; j < numberOfClients; j++) {

        //		bool connected = false;

        //		foreach (Client i in clients) {
        //			if (i.id != j) continue;
        //			line += ";" + i.id + ",1";
        //			line += ";" + i.totalTranslation.x + ";" + i.totalTranslation.y + ";" + i.totalTranslation.z;
        //			i.totalTranslation = Vector3.zero;
        //			line += ";" + i.totalRotation.x + ";" + i.totalRotation.y + ";" + i.totalRotation.z + ";" + i.totalRotation.w;
        //			i.totalRotation = Quaternion.identity;
        //			line += ";" + i.totalScaling;
        //			i.totalScaling = 1;
        //			line += ";" + i.totalRotationCamera.x + ";" + i.totalRotationCamera.y + ";" + i.totalRotationCamera.z + ";" + i.totalRotationCamera.w;
        //			i.totalRotationCamera = Quaternion.identity;
        //			connected = true;
        //			break;

        //		}

        //		if (!connected) {

        //			line += ";"+j+",0,0,0,0,0,0,0,1,1,0,0,0,1";

        //		}

        //	}
        //Debug.Log(line);
        f.WriteLine(line);
        f.Flush();
	}
}

