using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;



public class Log{

	StreamWriter fVerbose;
	StreamWriter fResume;

	public Log(string user, int task)
	{

        Debug.Log(Application.persistentDataPath);
		fVerbose = File.CreateText(Application.persistentDataPath + "/User-" + user + "-Task-" + task + "---" + System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "-Verbose.csv");
		fResume = File.CreateText(Application.persistentDataPath + "/User-" + user + "-Task-" + task + "---" + System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "-Resume.csv");
		string header = "Time;PieceID;IsSelected;DistanceID;RotationID;RotAngle;Modality;Translation X;Translation Y;Translation Z;Rotation X;Rotation Y;Rotation Z;Rotation W;Camera X;Camera Y;Camera Z;Error Trans Mat; Error Rot Mat;Error Rot Angle;Tracked Targets";
		fVerbose.WriteLine(header);
		header = "PieceID;DistanceID;RotationID;RotAngle;Time;Error Trans;Error Rot;Error Rot Angle";
		fResume.WriteLine(header);

	}

	public void close()
	{
		fVerbose.Close();
		fResume.Close();
	}

    public void flush() {
        fVerbose.Flush();
        fResume.Flush();
    }


	public void saveVerbose(bool isTraining, int pieceID, bool isSelected, int distanceID, int rotationID, int rotAngle, int modality, GameObject piece, Vector3 cameraPosition, float errorTrans, float errorRot, float errorRotAngle, int trackedTargets)
	{

	    String line = "";

    	line += Time.realtimeSinceStartup + "";
        line += ";";
        if (isTraining) line += "T";
        line += pieceID + ";" + isSelected + ";" + distanceID + ";" + rotationID + ";" + rotAngle + ";" + modality;
        line += ";" + piece.transform.localPosition.x + ";" + piece.transform.localPosition.y + ";" + piece.transform.localPosition.z;
        line += ";" + piece.transform.localRotation.x + ";" + piece.transform.localRotation.y + ";" + piece.transform.localRotation.z + ";" + piece.transform.localRotation.w;
        line += ";" + cameraPosition.x + ";" + cameraPosition.y + ";" + cameraPosition.z;
		line += ";" + errorTrans + ";" + errorRot + ";" + errorRotAngle;
		line += ";" + trackedTargets;
        fVerbose.WriteLine(line);
        fVerbose.Flush();
	}


	public void saveResume(bool isTraining, int pieceID, int distanceID, int rotationID, int rotAngle,  float time, float errorTrans, float errorRot, float errorRotAngle)
	{
		String line = "";
        if (isTraining) line += "T";
		line += pieceID + ";" + distanceID + ";" + rotationID + ";" + rotAngle;
		line += ";" + time;
		line += ";" + errorTrans + ";" + errorRot + ";" + errorRotAngle;
		fResume.WriteLine(line);
		fResume.Flush();
	}

}

