using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;



public class Log{

	StreamWriter fVerbose;
	StreamWriter fResume;

	float previousTime = 0.0f;

	public Log(string user, int task)
	{

        Debug.Log(Application.persistentDataPath);
		fVerbose = File.CreateText(Application.persistentDataPath + "/User-" + user + "-Task-" + task + "---" + System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "-Verbose.csv");
		fResume = File.CreateText(Application.persistentDataPath + "/User-" + user + "-Task-" + task + "---" + System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "-Resume.csv");
		string header = "Time;PieceID;DistanceToTarget;Translation X;Translation Y;Translation Z;Rotation X;Rotation Y;Rotation Z;Rotation W;Camera X;Camera Y;Camera Z;Error Trans;Error Rot";
		fVerbose.WriteLine(header);
		header = "PieceID;DistanceToTarget;Time;Error Trans;Error Rot";
		fResume.WriteLine(header);

	}

	public void close()
	{
		fVerbose.Close();
		fResume.Close();
	}



	public void saveVerbose(int pieceID, GameObject piece, Vector3 cameraPosition, float errorTrans, float errorRot) //, float distToTarget, GameObject t, Quaternion cameraRotation, float errorTrans, float errorRot)
	{

	//	//if (clients.Count < numberOfClients) return;

	    String line = "";

    	line += Time.realtimeSinceStartup + "";
        line += ";" + pieceID + ";" /*Add the distance here*/;
        line += ";" + piece.transform.localPosition.x + ";" + piece.transform.localPosition.y + ";" + piece.transform.localPosition.z;
        line += ";" + piece.transform.localRotation.x + ";" + piece.transform.localRotation.y + ";" + piece.transform.localRotation.z + ";" + piece.transform.localRotation.w;
        line += ";" + cameraPosition.x + ";" + cameraPosition.y + ";" + cameraPosition.z;
        line += ";" + errorTrans + ";" + errorRot;

        fVerbose.WriteLine(line);
        fVerbose.Flush();
	}


	public void saveResume(int pieceID, float time, float errorTrans, float errorRot) //, float distToTarget, GameObject t, Quaternion cameraRotation, float errorTrans, float errorRot)
	{
		String line = "";
		//line += Time.realtimeSinceStartup + "";
		line += pieceID + ";" /*Add the distance here*/;
		line += ";" + time;
		line += ";" + errorTrans + ";" + errorRot;
		fResume.WriteLine(line);
		fResume.Flush();
		previousTime = time;
	}

}

