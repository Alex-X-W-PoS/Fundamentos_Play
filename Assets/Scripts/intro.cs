using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.IO;

public class intro : MonoBehaviour {
	public GameObject medallas;
	public GameObject cRobot;
	public GameObject camara;
	public GameObject nNiveles;
	public GameObject bNiveles;
	public GameObject bNiveles2;
	public GameObject bLideres;
	public GameObject bDir;
	public GameObject lideres;
	public GameObject logo;
	public GameObject cajaTexto;
	public GameObject cajaTexto2;
	public GameObject cajaTexto3;
	public GameObject cajaTexto4;
	public GameObject dificultad;
	public GameObject nivel1;
	public GameObject nivel2;
	public GameObject nivel3;
	public GameObject reto;

	private Animation animRobot;
	private Animation animCam;
	private Animation animTexto;
	private DataController dataController;

	private int count = 0;

	// Use this for initialization
	void Start () {
		dataController = FindObjectOfType<DataController> ();
		animRobot = cRobot.gameObject.GetComponent<Animation> ();
		animCam = camara.gameObject.GetComponent<Animation> ();
		animTexto = cajaTexto.gameObject.GetComponent<Animation> ();
	}

	void Update(){
	}

	public void avancetuto(){
		switch (count){
		case 0:
			animRobot.Play ();
			animCam.Play ();
			medallas.SetActive (true);
			count++;
			break;
		case 1:
			medallas.SetActive (false);
			bNiveles.SetActive (true);
			count++;
			break;
		case 2:
			bNiveles.SetActive (false);
			bNiveles2.SetActive (true);
			nNiveles.SetActive (true);
			count++;
			break;
		case 3:
			nNiveles.SetActive (false);
			bLideres.SetActive (true);
			count++;
			break;
		case 4:
			bNiveles2.SetActive (false);
			bLideres.SetActive (false);
			lideres.SetActive (true);
			count++;
			break;
		case 5:
			lideres.SetActive (false);
			logo.SetActive (true);
			count++;
			break;
		case 6:
			animRobot.Play ("movcr2");
			cajaTexto.SetActive (false);
			cajaTexto2.SetActive (true);
			bDir.SetActive (true);
			count++;
			break;
		case 7:
			logo.SetActive (false);
			bDir.SetActive (false);
			cajaTexto2.SetActive (false);
			cajaTexto3.SetActive (true);
			animRobot.Play ("movcr3");
			dificultad.SetActive (true);
			count++;
			break;
		case 8:
			dificultad.SetActive (false);
			nivel1.SetActive (true);
			count++;
			break;
		case 9:
			nivel1.SetActive (false);
			nivel2.SetActive (true);
			count++;
			break;
		case 10:
			nivel2.SetActive (false);
			nivel3.SetActive (true);
			count++;
			break;
		case 11:
			nivel3.SetActive (false);
			reto.SetActive (true);
			count++;
			break;
		case 12:
			reto.SetActive (false);
			cajaTexto3.SetActive (false);
			cajaTexto4.SetActive (true);
			count++;
			break;
		case 13:
			DataController.nextScene = "map";
			dataController.WriteNewUser ();
			UnityEngine.SceneManagement.SceneManager.LoadScene("SceneLoader");
			count++;
			break;
		}
	}

	/*IEnumerator animacion(){
		medallas.SetActive (true);
	}*/
		
}
