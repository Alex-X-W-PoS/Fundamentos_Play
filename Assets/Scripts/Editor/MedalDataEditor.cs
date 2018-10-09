using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;


public class MedalDataEditor : EditorWindow {

	public MedalData medalData;
	private string medalDataFileName = "medal.json";

	[MenuItem ("Window/Medal Data Editor")]
	static void Init (){
		MedalDataEditor window = (MedalDataEditor)EditorWindow.GetWindow (typeof(MedalDataEditor));
		window.Show ();
	}

	void OnGUI(){
		if (medalData != null) {
			SerializedObject serializedObject = new SerializedObject (this);
			SerializedProperty serializedProperty = serializedObject.FindProperty ("medalData");
			EditorGUILayout.PropertyField (serializedProperty, true);
			serializedObject.ApplyModifiedProperties ();
			if (GUILayout.Button ("Save Data")) {
				SaveMedalData ();
			}
		}

		if (GUILayout.Button ("Load Data")) {
			LoadMedalData ();
		}
	}

	private void LoadMedalData(){
		string filepath = Path.Combine (Application.streamingAssetsPath, medalDataFileName);
		if (File.Exists (filepath)) {
			string dataAsJson = File.ReadAllText (filepath);
			medalData = JsonUtility.FromJson<MedalData> (dataAsJson);
		} else {
			medalData = new MedalData ();
		}
	}

	private void SaveMedalData(){
		string dataAsJson = JsonUtility.ToJson (medalData);
		string filepath = Path.Combine (Application.streamingAssetsPath, medalDataFileName);
		File.WriteAllText (filepath, dataAsJson); //creates or overwrite
	}

}
