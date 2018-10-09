using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour {

	// Use this for initialization
	public void Clicked() {
		//UnityEngine.SceneManagement.SceneManager.LoadScene("map");
		DataController.nextScene="map";
		UnityEngine.SceneManagement.SceneManager.LoadScene("SceneLoader");
	}
}
