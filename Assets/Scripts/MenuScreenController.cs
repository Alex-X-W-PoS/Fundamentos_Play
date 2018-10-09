using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class MenuScreenController : MonoBehaviour
{
	private DataController dataController;
	public AudioSource source;

	void Start ()
	{
		dataController = FindObjectOfType<DataController> ();
	}
	public void StartGame()
	{
		source.Play ();
		if (dataController.CheckNewUser () == false) {
			DataController.nextScene = "intro";
			UnityEngine.SceneManagement.SceneManager.LoadScene ("SceneLoader");
		} else {
			DataController.nextScene="map";
			UnityEngine.SceneManagement.SceneManager.LoadScene("SceneLoader");
		}
		//UnityEngine.SceneManagement.SceneManager.LoadScene("map");

	}
}