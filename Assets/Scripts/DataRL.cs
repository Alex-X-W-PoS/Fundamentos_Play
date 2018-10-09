using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
//para serializar datos
public class DataRL {
	public Data caps;
	protected string savePath;

	public DataRL()
	{
		this.savePath = Application.persistentDataPath + "/data.dat";
		this.loadDataFromDisk();
	}

	public void saveDataToDisk(Data datos)
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(savePath);
		bf.Serialize(file, datos);
		file.Close();
	}
		
	public void loadDataFromDisk()
	{
		if (File.Exists (savePath)) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (savePath, FileMode.Open);
			this.caps = (Data)bf.Deserialize (file);
			file.Close ();
		} 
		else 
		{
			caps = new Data ();
		}
	}
}



	
[System.Serializable]

public class Data
{
	public bool[] chall;
	public float[] times;
	public string[] names;
	public int[] avatares;
	public int[] dias;
	public Data(){
		chall = new bool[9];
		times = new float[9];
		names = new string[9];
		avatares = new int[9];
		dias = new int[9];
	}
}