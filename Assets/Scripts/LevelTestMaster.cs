using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelTestMaster : MonoBehaviour {
	
	private DataController dataController;
	public GameObject[] challengeButtons;
	public GameObject challengeButton;
	private GameObject challengeButtonInstance;
	public Canvas parentCanvas;
	public GameObject loadingPanelPrefab;
	public SpriteRenderer background;
	public Sprite[] bg_images;
	private GameObject loadingPanelInstance;
	Vector3 level1 = new Vector3 (218f, 455f, 0f);
	Vector3 level2 = new Vector3 (309f, 16f, 0f);
	Vector3 level3 = new Vector3 (218f, -415f, 0f);
	Vector3 level;
	string texto;
	bool medalla;

	// Use this for initialization
	void Awake () {
		dataController = FindObjectOfType<DataController>();
		dataController.SaveUserJSON ();
		background.sprite = bg_images [dataController.GetUserData ().ultimo_nivel_visitado - 1];
		//Aqui debo consultar servicio WEB para obtener el numero del nivel, y de acuerdo a ese numero escojo los botones
		for (int i = ((dataController.GetUserData().ultimo_nivel_visitado-1)*3); i < (((dataController.GetUserData().ultimo_nivel_visitado-1)*3)+3); i++) {
			getLevel (i%3);
			challengeButtonInstance = (GameObject)Instantiate (challengeButtons[i],level,Quaternion.identity);
			challengeButtonInstance.transform.SetParent (parentCanvas.transform,false);
			//aqui debo usar el numero del nivel para poner el id con solo numeros del 0 al 2.
			challengeButtonInstance.GetComponent<ChallengeButton> ().ID = (i % 3);
			challengeButtonInstance.GetComponent<ChallengeButton> ().master = this;
			getMedalBool (i % 3);
			if (medalla == true) {
				challengeButtonInstance.GetComponent<Button> ().interactable = true;
			} else {
				challengeButtonInstance.GetComponent<Button> ().interactable = false;
			}
		}
		//loadingPanelInstance = (GameObject)Instantiate (loadingPanelPrefab, new Vector3 (0,0,0), Quaternion.identity);
		//loadingPanelInstance.transform.SetParent (parentCanvas.transform,false);
		//Invoke ("stopLoading", 3f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void getMedalBool(int i){
		switch (i) {
		case 0:
			medalla = true;
			break;
		case 1:
			medalla = dataController.getUserMedalForUnit (dataController.GetUserData().ultimo_nivel_visitado-1).medalla_principiante;
			break;
		case 2:
			medalla = dataController.getUserMedalForUnit (dataController.GetUserData().ultimo_nivel_visitado-1).medalla_intermedio;
			break;
		}
	}

	void getLevel(int i){
		switch (i) {
		case 0:
			level = level1;
			break;
		case 1:
			level = level2;
			break;
		case 2:
			level = level3;
			break;
		}
	}

	public void stopLoading(){
		loadingPanelPrefab.SetActive (false);
		//loadData.SetActive (false);
	}

	public void ShowLoadingPanel(){
		loadingPanelPrefab.SetActive (true);
	}
}
