using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class GameDataEditor : EditorWindow {


	public GameData gameData;
	private string gameDataFileName = "data.json";

	[MenuItem ("Window/Game Data Editor")]
	static void Init (){
		GameDataEditor window = (GameDataEditor)EditorWindow.GetWindow (typeof(GameDataEditor));
		window.Show ();
	}

	void OnGUI(){
		if (gameData != null) {
			SerializedObject serializedObject = new SerializedObject (this);
			SerializedProperty serializedProperty = serializedObject.FindProperty ("gameData");
			EditorGUILayout.PropertyField (serializedProperty, true);
			serializedObject.ApplyModifiedProperties ();
			if (GUILayout.Button ("Save Data")) {
				SaveGameData ();
			}
		}

		if (GUILayout.Button ("Load Data")) {
			LoadGameData ();
		}
	}

	private void LoadGameData(){
		string filepath = Path.Combine (Application.streamingAssetsPath, gameDataFileName);
		if (File.Exists (filepath)) {
			string dataAsJson = File.ReadAllText (filepath);
			gameData = JsonUtility.FromJson<GameData> (dataAsJson);
		} else {
			gameData = new GameData ();
		}
	}

	private void SaveGameData(){
		string dataAsJson = JsonUtility.ToJson (gameData);
		string filepath = Path.Combine (Application.streamingAssetsPath, gameDataFileName);
		File.WriteAllText (filepath, dataAsJson); //creates or overwrite
	}
}
