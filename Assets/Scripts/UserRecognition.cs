using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserRecognition : MonoBehaviour {

	private DataController dataController;
	private UserData userData;
	private Text welcoming;

	// Use this for initialization
	void Start () {
		dataController = FindObjectOfType<DataController>();
		welcoming = this.gameObject.GetComponent<Text> ();
		userData = dataController.GetUserData ();
		welcoming.text = "Bienvenido " + userData.nombre;
	}

}
