using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChallengeButton : MonoBehaviour {

	private DataController dataController;
	public int ID;
	public LevelTestMaster master;
	private bool noError;

	void Start () {
		dataController = FindObjectOfType<DataController>();
	}

	public void StartGame()
	{
		master.ShowLoadingPanel ();
		Invoke ("startLoadingData", 2f);

	}

	void startLoadingData(){
		dataController.SetCurrentChallenge (-1);

		noError = dataController.LoadGameData (ID,dataController.GetUserData().ultimo_nivel_visitado);
		if (noError == true) {
			Invoke ("startRound", 0.2f);
		} else {
			master.stopLoading ();
		}
	}

	void startRound(){
		//master.stopLoading ();
		//DataController.nextScene="Game";
		UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
	}
}
