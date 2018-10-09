using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;


public class FirstTimeDataEditor : EditorWindow {

	public FirstTimeChecker firstTime;
	private string firstTimeDataFileName = "first.json";

	[MenuItem ("Window/First Time Data Editor")]
	static void Init (){
		FirstTimeDataEditor window = (FirstTimeDataEditor)EditorWindow.GetWindow (typeof(FirstTimeDataEditor));
		window.Show ();
	}

	void OnGUI(){
		if (firstTime != null) {
			SerializedObject serializedObject = new SerializedObject (this);
			SerializedProperty serializedProperty = serializedObject.FindProperty ("firstTime");
			EditorGUILayout.PropertyField (serializedProperty, true);
			serializedObject.ApplyModifiedProperties ();
			if (GUILayout.Button ("Save Data")) {
				SaveFirstData ();
			}
		}

		if (GUILayout.Button ("Load Data")) {
			LoadFirstData ();
		}
	}

	private void LoadFirstData(){
		string filepath = Path.Combine (Application.streamingAssetsPath, firstTimeDataFileName);
		if (File.Exists (filepath)) {
			string dataAsJson = File.ReadAllText (filepath);
			firstTime = JsonUtility.FromJson<FirstTimeChecker> (dataAsJson);
		} else {
			firstTime = new FirstTimeChecker ();
		}
	}

	private void SaveFirstData(){
		string dataAsJson = JsonUtility.ToJson (firstTime);
		string filepath = Path.Combine (Application.streamingAssetsPath, firstTimeDataFileName);
		File.WriteAllText (filepath, dataAsJson); //creates or overwrite
	}
}
