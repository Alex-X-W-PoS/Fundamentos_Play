using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using TMPro;

public class LeaderGameController : MonoBehaviour
{
	public AudioSource gameMusic;
	public AudioClip leaderSong;
	public AudioClip winSong;
	public AudioClip loseSong;
	public SimpleObjectPool answerButtonObjectPool;
	public TextMeshProUGUI questionText;
	public Text scoreDisplay;
	public Text timeRemainingDisplay;
	public Transform answerButtonParent;
	//public Image TimeBar;
	//public Sprite[] TimeBarImages;

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
	private int total_respondidas;
	public ponerTexto Retroalimentador;
	public GameObject loadingPanelPrefab;
	private GameObject loadingPanelInstance;
	public Canvas parentCanvas;
	public Button entrarImagen;
	public Button salirImagen;
	public GameObject imagenPanel;
	public Image imageToDisplay;

	void Start()
	{
		dataController = FindObjectOfType<DataController>();								// Store a reference to the DataController so we can request the data we need for this round
		currentRoundData = dataController.GetCurrentRoundData();							// Ask the DataController for the data for the current round. At the moment, we only have one round - but we could extend this
		questionPool = currentRoundData.questions;											// Take a copy of the questions so we could shuffle the pool or drop questions from it without affecting the original RoundData object
		timeRemaining = 0;								// Set the time limit for this round based on the RoundData object
		maxScore = (questionPool.Length);// * (currentRoundData.pointsAddedForCorrectAnswer);
		//maxTime = currentRoundData.timeLimitInSeconds;
		UpdateTimeRemainingDisplay();
		playerScore = 0;
		questionIndex = 0;
		scoreDisplay.text = "0/" + maxScore.ToString ();
		ShowQuestion();
		isRoundActive = true;
		gameMusic.clip = leaderSong;
		gameMusic.Play ();
	}

	void Update()
	{
		if (isRoundActive)
		{
			timeRemaining += Time.deltaTime;												// If the round is active, subtract the time since Update() was last called from timeRemaining
			UpdateTimeRemainingDisplay();

			if (timeRemaining <= 0f)														// If timeRemaining is 0 or less, the round ends
			{
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
		questionText.text = questionSF;										// Update questionText with the correct text

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

	public void AnswerButtonClicked(bool isCorrect, string retro)
	{
		if (isCorrect) {
			playerScore += 1;					// If the AnswerButton that was clicked was the correct answer, add points
			total_respondidas +=1;
			scoreDisplay.text = playerScore.ToString () +"/"+ maxScore.ToString(); 
			dataController.GetUserData ().preguntas_correctas++;
			dataController.GetUserData ().preguntas_totales++;
		} else {
			dataController.GetUserData ().preguntas_erroneas++;
			dataController.GetUserData ().preguntas_totales++;
			total_respondidas +=1;
			gameMusic.Pause ();
			gameMusic.clip = loseSong;
			gameMusic.Play ();
			gameMusic.loop = false;
			isRoundActive = false;
			Retroalimentador.activar (retro);
			EndRound();
		}
		dataController.GetUserData ().preguntas_totales++;
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
		TimeSpan t = TimeSpan.FromSeconds (timeRemaining);
		timeRemainingDisplay.text = string.Format ("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
	}

	public void EndRound()
	{
		Debug.Log (timeRemaining.ToString ());
		isRoundActive = false;
		questionDisplay.SetActive(false);
		roundEndDisplay.SetActive(true);
		if (playerScore == maxScore) {
			Debug.Log (timeRemaining.ToString ());
			float timeForCompare = timeRemaining * 1000;
			if (timeForCompare < dataController.getLeaderData ().times [dataController.getCurrentChallenge ()]) {
				gameMusic.Pause ();
				gameMusic.clip = winSong;
				gameMusic.Play ();
				dataController.getLeaderData ().names [dataController.getCurrentChallenge ()] = dataController.GetUserData ().nombre;
				dataController.getLeaderData ().times [dataController.getCurrentChallenge ()] = timeForCompare;
				dataController.UpdateLeader (timeForCompare);
				//dataController.getInfo ().saveDataToDisk (dataController.getLeaderData ());
			} else {
				gameMusic.clip = loseSong;
				gameMusic.Play ();
				gameMusic.loop = false;
			}
		} else {
			gameMusic.clip = loseSong;
			gameMusic.Play ();
			gameMusic.loop = false;
			//medalImage.sprite = medals [0];
			//medalText.text = "";
		}
		dataController.SaveUserJSON ();
	}

	public void ReturnToMenu()
	{
		
		SceneManager.LoadScene("map");
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