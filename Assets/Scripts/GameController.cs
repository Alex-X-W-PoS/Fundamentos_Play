using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class GameController : MonoBehaviour
{
	public AudioSource gameMusic;
	public AudioSource bomb;
	public AudioClip normalSong;
	public AudioClip timeSong;
	public AudioClip loseSong;
	public AudioClip winSong;
	public SimpleObjectPool answerButtonObjectPool;
	public TextMeshProUGUI questionUText;
	public Text scoreDisplay;
	public Text timeRemainingDisplay;
	public Transform answerButtonParent;
	public Image TimeBar;
	public Sprite[] TimeBarImages;
	private bool songController;

	public GameObject questionDisplay;
	public GameObject roundEndDisplay;
	public Scrollbar scroll;
	private DataController dataController;
	private RoundData currentRoundData;
	private QuestionData[] questionPool;

	private bool isRoundActive = false;
	private float timeRemaining;
	private int playerScore;
	private int questionIndex;
	private float maxTime;
	private List<GameObject> answerButtonGameObjects = new List<GameObject>();
	public Text medalText;
	public Image medalImage;
	public Sprite[] medals;
	private int maxScore;
	private int dificultad;
	private int total_respondidas;
	public ponerTexto Retroalimentador;
	public GameObject loadingPanelPrefab;
	private GameObject loadingPanelInstance;
	public Canvas parentCanvas;
	public Button entrarImagen;
	public Button salirImagen;
	public GameObject imagenPanel;
	public Image imageToDisplay;
	public int [] id_array;
	public int[] answer_array;
	public int[] wrong_array;
	public int starting_count;

	void Start()
	{
		id_array = new int[10];
		answer_array = new int[10];
		wrong_array = new int[10];
		for (int i = 0; i < 10; i++) {
			id_array [i] = -1;
			answer_array [i] = 0;
			wrong_array [i] = 0;

		}
		starting_count = 0;
		dataController = FindObjectOfType<DataController>();								// Store a reference to the DataController so we can request the data we need for this round
		currentRoundData = dataController.GetCurrentRoundData();							// Ask the DataController for the data for the current round. At the moment, we only have one round - but we could extend this
		questionPool = currentRoundData.questions;											// Take a copy of the questions so we could shuffle the pool or drop questions from it without affecting the original RoundData object
		//timeRemaining = currentRoundData.timeLimitInSeconds;								// Set the time limit for this round based on the RoundData object
		maxScore = (questionPool.Length); //* (currentRoundData.pointsAddedForCorrectAnswer);
		maxTime = 0;
		for (int i = 0; i < questionPool.Length; i++) {
			maxTime = maxTime + questionPool [i].tiempo_en_segundos;
		}
		dificultad = questionPool [0].dificultad;
		timeRemaining = maxTime;
		UpdateTimeRemainingDisplay();
		playerScore = 0;
		total_respondidas = 0;
		questionIndex = 0;
		scoreDisplay.text = "0/" + maxScore.ToString ();
		ShowQuestion();
		isRoundActive = true;
		gameMusic.clip = normalSong;
		gameMusic.Play ();
	}

	void Update()
	{
		if (isRoundActive)
		{
			timeRemaining -= Time.deltaTime;												// If the round is active, subtract the time since Update() was last called from timeRemaining
			TimeBar.fillAmount -= 1.0f/maxTime * Time.deltaTime;
			UpdateTimeRemainingDisplay();

			if (timeRemaining <= 0f)														// If timeRemaining is 0 or less, the round ends
			{
				gameMusic.Pause ();
				bomb.Play ();
				gameMusic.clip = loseSong;
				gameMusic.PlayDelayed (1f);
				gameMusic.loop = false;
				EndRound();
			}
		}
	}

	void ShowQuestion()
	{
		RemoveAnswerButtons();
		scroll.value = 1;
		QuestionData questionData = questionPool[questionIndex];							// Get the QuestionData for the current question
		string questionSF = questionData.contenido.ToString();
		questionSF = questionSF.Replace ("\\n", "\n");
		questionSF = questionSF.Replace ("\\t", "\t");
		if (questionData.textura != null) {
			Rect rec = new Rect (0, 0, questionData.textura.width, questionData.textura.height);
			Sprite spriteToUse = Sprite.Create (questionData.textura, rec, new Vector2 (0.5f, 0.5f), 100);
			imageToDisplay.sprite = spriteToUse;
			imageToDisplay.rectTransform.sizeDelta = new Vector2 (1080f, questionData.textura.height*2);
			entrarImagen.interactable = true;
		} else {
			entrarImagen.interactable = false;
		}
		questionUText.text = questionSF;
								// Update questionText with the correct text

		for (int i = 0; i < questionData.answers.Length; i ++)								// For every AnswerData in the current QuestionData...
		{
			GameObject answerButtonGameObject = answerButtonObjectPool.GetObject();			// Spawn an AnswerButton from the object pool
			answerButtonGameObjects.Add(answerButtonGameObject);
			answerButtonGameObject.transform.SetParent(answerButtonParent);
			answerButtonGameObject.transform.localScale = Vector3.one;

			AnswerButton answerButton = answerButtonGameObject.GetComponent<AnswerButton>();
			answerButton.SetUp(questionData.answers[i]);									// Pass the AnswerData to the AnswerButton so the AnswerButton knows what text to display and whether it is the correct answer
		}
	}

	void RemoveAnswerButtons()
	{
		while (answerButtonGameObjects.Count > 0)											// Return all spawned AnswerButtons to the object pool
		{
			answerButtonObjectPool.ReturnObject(answerButtonGameObjects[0]);
			answerButtonGameObjects.RemoveAt(0);
		}
	}

	public void AnswerButtonClicked(bool isCorrect,string retro)
	{
		if (isCorrect) {
			playerScore += 1;					// If the AnswerButton that was clicked was the correct answer, add points
			total_respondidas +=1;
			scoreDisplay.text = playerScore.ToString () +"/"+ maxScore.ToString(); 
			dataController.GetUserData ().preguntas_correctas++;
			dataController.GetUserData ().preguntas_totales++;
			id_array [starting_count] = questionPool [questionIndex].idPregunta;
			answer_array [starting_count] = 1;
			starting_count++;

		} else {
			dataController.GetUserData ().preguntas_erroneas++;
			dataController.GetUserData ().preguntas_totales++;
			id_array [starting_count] = questionPool [questionIndex].idPregunta;
			wrong_array [starting_count] = 1;
			total_respondidas +=1;
			gameMusic.Pause ();
			gameMusic.clip = loseSong;
			gameMusic.Play ();
			gameMusic.loop = false;
			isRoundActive = false;
			Retroalimentador.activar (retro);
			EndRound();
		}
		if(questionPool.Length > questionIndex + 1)											// If there are more questions, show the next question
		{
			questionIndex++;
			ShowQuestion();
		}
		else																				// If there are no more questions, the round ends
		{
			gameMusic.Pause ();
			gameMusic.clip = winSong;
			gameMusic.Play ();
			EndRound();
		}
	}

	private void UpdateTimeRemainingDisplay()
	{
		timeRemainingDisplay.text = Mathf.Round(timeRemaining).ToString();
		if (timeRemaining <= maxTime / 2.0f && timeRemaining >= maxTime / 5.0f) {
			TimeBar.sprite = TimeBarImages [1];
		} else if (timeRemaining <= maxTime / 5.0f) {
			TimeBar.sprite = TimeBarImages [2];
			if (songController == false) {
				gameMusic.Pause ();
				gameMusic.clip = timeSong;
				gameMusic.Play ();
				songController = true;
			}
		}
	}

	public void EndRound()
	{
		//showLoadingPanel ();
		isRoundActive = false;
		questionDisplay.SetActive(false);
		roundEndDisplay.SetActive(true);
		loadingPanelPrefab.SetActive (true);
		Invoke ("startEndingResults", 2f);

	}

	public void startEndingResults(){
		if (playerScore == maxScore) {
			showLoadingPanel ();
			if (dificultad == 0 && dataController.getUserMedalForUnit (dataController.GetUserData().ultimo_nivel_visitado-1).medalla_principiante == false) {
				medalImage.sprite = medals [1];
				medalText.text = "Medalla Obtenida!";
				dataController.getUserMedalForUnit (dataController.GetUserData().ultimo_nivel_visitado-1).medalla_principiante = true;
			} else if (dificultad == 1 && dataController.getUserMedalForUnit (dataController.GetUserData().ultimo_nivel_visitado-1).medalla_intermedio == false) {
				medalImage.sprite = medals [2];
				medalText.text = "Medalla Obtenida!";
				dataController.getUserMedalForUnit (dataController.GetUserData().ultimo_nivel_visitado-1).medalla_intermedio = true;
				if (dataController.GetUserData ().maximo_nivel < 3) {
					dataController.GetUserData ().maximo_nivel++;
					dataController.UpdateMaxLevel ();
				}
			} else if (dificultad == 2 && dataController.getUserMedalForUnit (dataController.GetUserData().ultimo_nivel_visitado-1).medalla_experto == false) {
				medalImage.sprite = medals [3];
				medalText.text = "Medalla Obtenida!";
				dataController.getUserMedalForUnit (dataController.GetUserData().ultimo_nivel_visitado-1).medalla_experto = true;
				dataController.getLeaderData ().chall [dataController.GetUserData ().ultimo_nivel_visitado - 1] = true;
				//dataController.getInfo ().saveDataToDisk (dataController.getLeaderData ());
			}
			medalImage.rectTransform.localScale = new Vector3 (2f, 2f, 2f);

			dataController.SaveMedalJSON (dificultad);
			Destroy (loadingPanelInstance);
		} else {
			medalImage.sprite = medals [0];
			medalText.text = "";
		}
		dataController.UpdateUser(playerScore,(total_respondidas-playerScore),total_respondidas);
		if (dificultad == 0) {
			dataController.updateFichaComp (dataController.GetUserData ().userHash, dataController.GetUserData ().ultimo_nivel_visitado, playerScore, (total_respondidas - playerScore), 0, 0, 0, 0);
		} else if (dificultad == 1) {
			dataController.updateFichaComp (dataController.GetUserData ().userHash, dataController.GetUserData ().ultimo_nivel_visitado, 0, 0, playerScore, (total_respondidas - playerScore), 0, 0);
		} else {
			dataController.updateFichaComp (dataController.GetUserData ().userHash, dataController.GetUserData ().ultimo_nivel_visitado, 0, 0, 0, 0, playerScore, (total_respondidas - playerScore));
		}
		int strt = 0;
		while (strt<10  && id_array [strt] > -1) {
			dataController.updateQuestionData (id_array [strt], answer_array [strt], wrong_array [strt]);
			strt++;
		}
		loadingPanelPrefab.SetActive (false);
	}

	public void ReturnToMenu()
	{
		//SceneManager.LoadScene("Level Test");
		DataController.nextScene="Level Test";
		UnityEngine.SceneManagement.SceneManager.LoadScene("SceneLoader");
	}

	public void showLoadingPanel(){
		loadingPanelInstance = (GameObject)Instantiate (loadingPanelPrefab, new Vector3 (0,0,0), Quaternion.identity);
		loadingPanelInstance.transform.SetParent (parentCanvas.transform,false);
	}

	public void abrirImagen(){
		imagenPanel.SetActive (true);
	}

	public void cerrarImagen(){
		imagenPanel.SetActive (false);
	}
}