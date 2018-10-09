using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AnswerButton : MonoBehaviour 
{
	public Text answerText;

	private GameController gameController;
	private AnswerData answerData;
	private DataController dataController;
	private LeaderGameController leaderGameController;

	void Start()
	{
		dataController = FindObjectOfType<DataController> ();
		gameController = FindObjectOfType<GameController>();
		leaderGameController = FindObjectOfType<LeaderGameController> ();
	}

	public void SetUp(AnswerData data)
	{
		answerData = data;
		answerText.text = answerData.contenido;
	}

	public void HandleClick()
	{
		if (dataController.getCurrentChallenge () == -1) {
			gameController.AnswerButtonClicked(answerData.esCorrecta,answerData.retroalimentacion);
		} else {
			leaderGameController.AnswerButtonClicked(answerData.esCorrecta,answerData.retroalimentacion);
		}

	}
}
