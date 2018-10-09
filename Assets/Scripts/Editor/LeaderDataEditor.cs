using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LeaderDataEditor :  EditorWindow
{

	public Data leaderData;

	private string leaderDataProjectFilePath = "/leader.json";

	[MenuItem ("Window/Leader Data Editor")]
	static void Init()
	{
		EditorWindow.GetWindow (typeof(LeaderDataEditor)).Show ();
	}

	void OnGUI()
	{
		if (leaderData != null) 
		{
			SerializedObject serializedObject = new SerializedObject (this);
			SerializedProperty serializedProperty = serializedObject.FindProperty ("leaderData");
			EditorGUILayout.PropertyField (serializedProperty, true);

			serializedObject.ApplyModifiedProperties ();

			if (GUILayout.Button ("Save data"))
			{
				SaveGameData();
			}
		}

		if (GUILayout.Button ("Load data"))
		{
			LoadGameData();
		}
	}

	private void LoadGameData()
	{
		string filePath =  Application.persistentDataPath + leaderDataProjectFilePath;

		if (File.Exists (filePath)) {
			string dataAsJson = File.ReadAllText (filePath);
			leaderData = JsonUtility.FromJson<Data> (dataAsJson);
		} else 
		{
			leaderData = new Data();
		}
	}

	private void SaveGameData()
	{

		string dataAsJson = JsonUtility.ToJson (leaderData);

		string filePath =  Application.persistentDataPath + leaderDataProjectFilePath;

		File.WriteAllText (filePath, dataAsJson);

	}
}