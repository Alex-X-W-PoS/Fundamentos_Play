using UnityEngine;

[System.Serializable]
public class QuestionData
{
	public int idPregunta;
	public string contenido;
	public int tiempo_en_segundos;
	public int dificultad;
	public AnswerData[] answers;
	public string imagen;
	public Texture2D textura;
}