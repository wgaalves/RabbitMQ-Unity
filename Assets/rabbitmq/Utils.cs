using UnityEngine;
using System.Collections;
using System.IO;
using System;
using UnityEngine.UI;

public class Utils {


	public static bool SaveFileToDisk(string fullFileName, byte[] rawFile){

		try{
			File.WriteAllBytes(GetFullPathFileName(fullFileName) ,rawFile);
		}catch(IOException e){
			Debug.LogError( "ERROR Write File " + GetFullPathFileName(fullFileName) + " Error: " + e.GetBaseException().ToString() );
			return false;
		}

		return true;

	}

	public static byte[] GetFileAsBytesOrNull(string fullFileName){

		if (File.Exists(GetFullPathFileName(fullFileName))){
			return File.ReadAllBytes(GetFullPathFileName(fullFileName));
		}else{
			Debug.LogError("File Not Found: " + GetFullPathFileName(fullFileName));
			return null;
		}

	}

	public static string GetFullPathFileName(string fullFileName){
		if (Application.platform != RuntimePlatform.Android) 
			return fullFileName;
		else {
			return Application.persistentDataPath + @"/" + fullFileName;
		}
	}






}
