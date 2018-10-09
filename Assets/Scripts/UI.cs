using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class UI : MonoBehaviour{
	
	public GameObject numbersPanel;
	public GameObject chaptersPrefab;
	public Transform chaptersPanel;
	public GameObject leaderboardPanel;//Ventana de leaderboards
	public Data gameData; //Clase para leer los datos
	public Button openButton;
	public GameObject confirmationPanel; // panel para el mensaje de confirmacion del desafio
	public Button okButton; //botones de aceptar o rechazar
	public Button cancelButton;



	private GameObject[] chapters;// array de gameobject para el prefab de capitulos
	private DataRL info;// controlador para el objecto de datos
	private Text[] textComponent;// array para los textos del nombre capitulo y tiempo
	private string time;//string para el tiempo
	private TimeSpan t;//objecto para llevar el tiempo de milisegundo a formato minuto:segundo
	private string dataAsJson;//para lectura del archivo Json


	void Awake()
	{
		chapters = new GameObject[9];
		textComponent = new Text[2];
		info = new DataRL();
		confirmationPanel.SetActive (false);
		loadJson ();//lectura del archivo Json, por ahora no valide su existencia. Lo estoy incluyendo en el proyecto, asigna valores a gameData
		info.saveDataToDisk (gameData);//guarda gameData serializado
		setupLeads ();//organiza el panel de leaderboards segun gameData
		//La forma en que esta organizada la lectura de datos y su ingreso al juego la hice de esta manera para verificar su funcionamiento, lo que se carga que si se pueda leer y asi.
	}
	void setupLeads ()
	{
		leaderboardPanel.SetActive(true);
		info.loadDataFromDisk ();//lee el archivo serializado y lo carga
		Button Bcomponent;
		for (int i = 0; i < 9; i++) {
			chapters [i] = (GameObject)GameObject.Instantiate (chaptersPrefab);
			chapters [i].SetActive (true);
			chapters [i].name = "chapter" + i.ToString ();//se cambia el nombre del objecto a chapter0 -->chapter8 para su identificacion
			chapters [i].transform.SetParent (chaptersPanel, false);//se colocan en el panel correspondiente, el false era por algo de su posicion, no recuerdo la explicacion exacta ):
			Bcomponent = chapters [i].GetComponentInChildren<Button> ();//se asigna el componente de boton para su modificacion
			Bcomponent.interactable = info.caps.chall[i];//segun los datos leidos se pondra activo o no
			string captured = chapters[i].name;// string capturado por ciclo, una forma de guardar el nombre de cada capitulo para su llamada al ser presionado
			Bcomponent.onClick.AddListener(() => Desafio(captured));//se coloca un listener al boton que llama a esa funcion con el nombre del capitulo como argumento
			textComponent = chapters [i].GetComponentsInChildren<Text> ();//se asigna para cambiar los textos
			textComponent[0].text = "Capítulo "+(i+1).ToString() +": "+ info.caps.names [i];//se colocan datos segun la informacion obtenida
			textComponent [1].text = "Tiempo: "+getTime(info.caps.times [i]);
		}
		leaderboardPanel.SetActive(false);//se desactiva el panel de leaderboards
	}
	public void ShowLead()
	{
		openButton.interactable =false;
		leaderboardPanel.SetActive(true);
	}

	public void HideLead()
	{
		openButton.interactable = true;
		leaderboardPanel.SetActive (false);
	}

	public void loadJson(){
		TextAsset jsonD = Resources.Load("data") as TextAsset;
		dataAsJson = jsonD.text;
		gameData = JsonUtility.FromJson<Data> (dataAsJson);
	}

	private string getTime(float f)
	{
		t = TimeSpan.FromMilliseconds (f);
		time = string.Format ("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
		return time;
	}

	private void Desafio(string Ncap){
		confirmationPanel.SetActive (true);//cuando se presione el boton de desafiar se activa el panel de confirmacion y se agregan listeners a sus botones
		okButton.onClick.AddListener(() => changeScene(Ncap));//si es presionado se hace cambio de escena al desafio con el nombre del capitulo para su identificacion
		cancelButton.onClick.AddListener(() => cancel());
	}
	private void changeScene(string Ncap){
		okButton.onClick.RemoveAllListeners ();//si se realiza el cambio de escena se quitan los listeneres(lei que era de buena practica y optimizacion) 
		cancelButton.onClick.RemoveAllListeners ();
		confirmationPanel.SetActive (false);
		leaderboardPanel.SetActive (false);
		openButton.interactable =true;//desactivando paneles

	}

	private void cancel(){
		okButton.onClick.RemoveAllListeners ();
		cancelButton.onClick.RemoveAllListeners ();
		confirmationPanel.SetActive (false);

	}

	public void levelPanel(){
		numbersPanel.SetActive (!numbersPanel.activeSelf);
	}


}