using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class UserDataEditor : EditorWindow {

	public UserData userData;
	private string userDataFileName = "user.json";

	[MenuItem ("Window/User Data Editor")]
	static void Init (){
		UserDataEditor window = (UserDataEditor)EditorWindow.GetWindow (typeof(UserDataEditor));
		window.Show ();
	}

	void OnGUI(){
		if (userData != null) {
			SerializedObject serializedObject = new SerializedObject (this);
			SerializedProperty serializedProperty = serializedObject.FindProperty ("userData");
			EditorGUILayout.PropertyField (serializedProperty, true);
			serializedObject.ApplyModifiedProperties ();
			if (GUILayout.Button ("Save Data")) {
				SaveUserData ();
			}
		}

		if (GUILayout.Button ("Load Data")) {
			LoadUserData ();
		}
	}

	private void LoadUserData(){
		string filepath = Path.Combine (Application.streamingAssetsPath, userDataFileName);
		if (File.Exists (filepath)) {
			string dataAsJson = File.ReadAllText (filepath);
			userData = JsonUtility.FromJson<UserData> (dataAsJson);
		} else {
			userData = new UserData ();
		}
	}

	private void SaveUserData(){
		string dataAsJson = JsonUtility.ToJson (userData);
		string filepath = Path.Combine (Application.streamingAssetsPath, userDataFileName);
		File.WriteAllText (filepath, dataAsJson); //creates or overwrite
	}
}
