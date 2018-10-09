using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using System.IO;
using UnityEngine.SceneManagement;
public class Controller : MonoBehaviour{
	
	public GameObject numbersPanel;
	public GameObject chaptersPrefab;
	public Transform chaptersPanel;
	public GameObject leaderboardPanel;//Ventana de leaderboards
	public Button openButton;
	public GameObject confirmationPanel; // panel para el mensaje de confirmacion del desafio
	public Button okButton; //botones de aceptar o rechazar
	public Button cancelButton;



	private GameObject[] chapters;// array de gameobject para el prefab de capitulos
	private Text[] textComponent;// array para los textos del nombre capitulo y tiempo
	private string time;//string para el tiempo
	private TimeSpan t;//objecto para llevar el tiempo de milisegundo a formato minuto:segundo
	private string dataAsJson;//para lectura del archivo Json
	private DataController dataController;
	private WorldMaster master;
	private Image avat;
	private int cap;


	void Awake()
	{
		dataController = FindObjectOfType<DataController> ();
		chapters = new GameObject[9];
		textComponent = new Text[2];
		confirmationPanel.SetActive (false);
		cap = -1;
		setupLeads ();//organiza el panel de leaderboards segun gameData
		//La forma en que esta organizada la lectura de datos y su ingreso al juego la hice de esta manera para verificar su funcionamiento, lo que se carga que si se pueda leer y asi.
	}
	
	void Start(){
		master = GameObject.FindObjectOfType<WorldMaster> ();
	}
	
	void setupLeads ()
	{
		dataController.getLeaders ();
		leaderboardPanel.SetActive(true);
		Button Bcomponent;
		for (int i = 0; i < 9; i++) {
			Debug.Log (i);
			chapters [i] = (GameObject)GameObject.Instantiate (chaptersPrefab);
			chapters [i].SetActive (true);
			chapters [i].name = i.ToString ();
			chapters [i].transform.SetParent (chaptersPanel, false);//se colocan en el panel correspondiente, el false era por algo de su posicion, no recuerdo la explicacion exacta ):
			Bcomponent = chapters [i].GetComponentInChildren<Button> ();//se asigna el componente de boton para su modificacion
			Bcomponent.interactable = dataController.getLeaderData().chall[i];//segun los datos leidos se pondra activo o no
			string captured = chapters[i].name;// string capturado por ciclo, una forma de guardar el nombre de cada capitulo para su llamada al ser presionado
			Bcomponent.onClick.AddListener(() => Desafio(captured));//se coloca un listener al boton que llama a esa funcion con el nombre del capitulo como argumento
			textComponent = chapters [i].GetComponentsInChildren<Text> ();//se asigna para cambiar los textos
			textComponent[0].text = "Unidad "+(i+1).ToString() +": "+ dataController.getLeaderData().names [i];//se colocan datos segun la informacion obtenida
			textComponent [1].text = "Tiempo: "+getTime(dataController.getLeaderData().times [i]);
			textComponent [2].text = "Días: "+dataController.getLeaderData().dias[i] + " días.";
			avat = chapters [i].GetComponentInChildren<Image> ();
			avat.sprite = dataController.avatares [dataController.getLeaderData ().avatares[i]];
		}
		leaderboardPanel.SetActive(false);//se desactiva el panel de leaderboards
	}
	public void ShowLead()
	{
		openButton.interactable =false;
		dataController.getLeaders ();
		leaderboardPanel.SetActive(true);
	}

	public void HideLead()
	{
		openButton.interactable = true;
		leaderboardPanel.SetActive (false);
	}

	private string getTime(float f)
	{
		t = TimeSpan.FromMilliseconds (f);
		time = string.Format ("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
		return time;
	}

	private void Desafio(string Ncap){
		int chapterNumber = Int32.Parse (Ncap);
		Debug.Log (chapterNumber.ToString ());
		confirmationPanel.SetActive (true);//cuando se presione el boton de desafiar se activa el panel de confirmacion y se agregan listeners a sus botones
		okButton.onClick.AddListener(() => changeScene(chapterNumber));//si es presionado se hace cambio de escena al desafio con el nombre del capitulo para su identificacion
		cancelButton.onClick.AddListener(() => cancel());
	}
	private void changeScene(int Ncap){
		cap = Ncap;
		master.showLoadingPanel ();
		okButton.onClick.RemoveAllListeners ();//si se realiza el cambio de escena se quitan los listeneres(lei que era de buena practica y optimizacion) 
		cancelButton.onClick.RemoveAllListeners ();
		confirmationPanel.SetActive (false);
		leaderboardPanel.SetActive (false);
		openButton.interactable =true;//desactivando paneles
		Invoke ("ToChallenge", 2f);





	}

	private void cancel(){
		okButton.onClick.RemoveAllListeners ();
		cancelButton.onClick.RemoveAllListeners ();
		confirmationPanel.SetActive (false);

	}

	public void levelPanel(){
		numbersPanel.SetActive (!numbersPanel.activeSelf);
		Button [] hijos = numbersPanel.GetComponentsInChildren<Button> ();
		for (int i = 0; i < 9; i++) {
			if (i <= dataController.GetUserData ().maximo_nivel - 1) {
				hijos [i].interactable =  true;
			} else {
				hijos [i].interactable = false;
			}
		}
	}
	
	public void ToChallenge(){
		dataController.SetCurrentChallenge(cap);
		dataController.LoadGameData (0,cap+1);
		//SceneManager.LoadScene ("LeaderGame");
		UnityEngine.SceneManagement.SceneManager.LoadScene("LeaderGame");
	}


}