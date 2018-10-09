using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;

public class DataController : MonoBehaviour 
{
	private RoundData[] allRoundData;
	private UserData userData;
	private userIDPlay idplay;
	public MedalData medalData;
	string dataAsJson = "";
	string savepath= "";
	//string saves = "";
	private string userDataFileName = "user.txt";
	private string indicator = "indicator.txt";
	public Text texto;
	public Data leaderData;
	private int currentChallenge;
	public Sprite [] avatares;

	public Dictionary<string, string> headerInfo = new Dictionary<string, string>();

	public const string requestsUrlUser = "http://200.9.176.59:80/FundamentosPlay/servicios/usuarios/";
	public const string requestUrlQuestions = "http://200.9.176.59:80/FundamentosPlay/servicios/preguntas/";
	public const string requestMedallas = "http://200.9.176.59:80/FundamentosPlay/servicios/fichacompletacion/";
	public const string requestNiveles = "http://200.9.176.59:80/FundamentosPlay/servicios/niveles/";
	public const string requestLideres = "http://200.9.176.59:80/FundamentosPlay/servicios/leaderboard/";

	public static int level = 0;
	public static int maxLevel = 0;
	public static string nextScene;

	void Start ()  
	{
		Invoke ("gameStart", 2f);
			
	}


	
	public RoundData GetCurrentRoundData()
	{
		return allRoundData [0];

	}

	public void SetCurrentRoundData (RoundData roundData){
		allRoundData [0] = roundData;
	}
		

	public bool LoadGameData(int dificultad, int nivel){
		//Aqui debo llamar al webservice que me cargue la ronda de preguntas
		string servicio_id = requestUrlQuestions + "getRonda/" + nivel + "/" + dificultad;
		WWW www = new WWW (servicio_id,null,headerInfo);
		while (!www.isDone) {
			//se espera a que la descarga se salga
		}
		if (!string.IsNullOrEmpty (www.error)) {
			Debug.LogError ("Cannot load game data!");
			return false;
			//Aquí sería regresar a la página de persistent, o algo mas
		} else {
			Debug.Log ("DATOS RONDA CARGADOS EXITOSAMENTE");
			dataAsJson = www.text;
			GameData loadedData = JsonUtility.FromJson<GameData> (dataAsJson);
			allRoundData = loadedData.allRoundData;
			for (int i = 0; i < allRoundData [0].questions.Length; i++) {
				QuestionData q = allRoundData [0].questions [i];
				Debug.Log (q.imagen);
				if (q.imagen != "") {
					Debug.Log (q.imagen);
					WWW wwwImage = new WWW (q.imagen);
					while (!wwwImage.isDone) {

					}
					if (!string.IsNullOrEmpty (wwwImage.error)) {
						Debug.LogError (wwwImage.error);
						return false;
					} else {
						q.textura = new Texture2D (wwwImage.texture.width, wwwImage.texture.height, TextureFormat.DXT1, false);
						wwwImage.LoadImageIntoTexture (q.textura);
						wwwImage.Dispose();
						wwwImage = null;
					}
				} else {
					q.textura = null;
				}
			}
			return true;
		}
	}

	public void LoadMedalData(int nivel){
		//Aqui debo llamar el webservice de las medallas del ultimo nivel
		string servicio_id = requestMedallas + "getMedallas/" + userData.userHash + "/" + nivel;
		WWW www = new WWW (servicio_id,null,headerInfo);
		while (!www.isDone) {
			//se espera a que la descarga se salga
		}
		if (!string.IsNullOrEmpty (www.error)) {
			//Aquí sería regresar a la página de persistent, o algo mas
			Debug.LogError ("Cannot load game data!");
			Application.Quit ();
		} else {
			Debug.Log ("DATOS MEDALLAS CARGADOS EXITOSAMENTE");
			dataAsJson = www.text;
			MedalData loadedData = JsonUtility.FromJson <MedalData> (dataAsJson);
			medalData = loadedData;
		}

	}

	public UserData GetUserData(){
		return userData;
	}

	public void SetUserData (UserData user){
		userData = user;
	}

	public MedalData GetMedalData(){
		return medalData;
	}

	public void UpdateUser (int preguntas_acertadas, int preguntas_erroneas, int preguntas_totales) {
		string servicio_id = requestsUrlUser + "updateUserData/" + userData.userHash + "/" + preguntas_totales + "/" + preguntas_erroneas + "/" + preguntas_acertadas;
		WWW www = new WWW (servicio_id,null,headerInfo);
		while (!www.isDone) {
			//se espera a que la descarga se salga
		}
		if (!string.IsNullOrEmpty (www.error)) {
			//Aquí sería regresar a la página de persistent, o algo mas
			Debug.LogError ("Cannot load game data!");
			Application.Quit ();
		} else {
			Debug.Log ("DATOS USUARIO ACTUALIZADO EXITOSAMENTE");
		}
	}

	public void SaveUserJSON(){
		string servicio_id = requestsUrlUser + "updateVisitedLevel/" + userData.userHash + "/" + userData.ultimo_nivel_visitado;
		WWW www = new WWW (servicio_id,null,headerInfo);
		while (!www.isDone) {
			//se espera a que la descarga se salga
		}
		if (!string.IsNullOrEmpty (www.error)) {
			//Aquí sería regresar a la página de persistent, o algo mas
			Debug.LogError ("Cannot load game data!");
			Application.Quit ();
		} else {
			Debug.Log ("DATOS ULTIMO NIVEL ACTUALIZADO EXITOSAMENTE");
		}
	}

	public void SaveMedalJSON(int dificultad){
		//Aqui debo llamar al servicio para actualizar las medallas de un nivel dado
		string servicio_id = requestMedallas + "updateMedallas/" + userData.userHash + "/" + userData.ultimo_nivel_visitado + "/" + dificultad;
		WWW www = new WWW (servicio_id,null,headerInfo);
		while (!www.isDone) {
			//se espera a que la descarga se salga
		}
		if (!string.IsNullOrEmpty (www.error)) {
			//Aquí sería regresar a la página de persistent, o algo mas
			Debug.LogError ("Cannot load game data!");
			Application.Quit ();
		} else {
			Debug.Log ("DATOS MEDALLAS ACTUALIZADO EXITOSAMENTE");
		}
	}

	public void loadJson(){
		TextAsset jsonD = Resources.Load("leader") as TextAsset;//usando el folder Resources para guardar un Json predefinido
		dataAsJson = jsonD.text;
		string filepath = Path.Combine (savepath, "leader.json");
		File.WriteAllText (filepath, dataAsJson);
		leaderData = JsonUtility.FromJson<Data> (dataAsJson);
	}

	public Data getLeaderData(){
		return leaderData;
	}

	/*public void InitializeDataRL(){
		info = new DataRL ();
	}*/

	public void saveLeaderJson(){
		//llamar al json para sobreescribir los datos del leader
		/*
		string dataAsJson = JsonUtility.ToJson (leaderData);
		string filepath = Path.Combine (savepath, leaderDataFileName);
		File.WriteAllText (filepath, dataAsJson); //creates or overwrite*/
	}

	public int getCurrentChallenge (){
		return currentChallenge;
	}

	public void SetCurrentChallenge (int chapter){
		currentChallenge = chapter;
	}

	public void getLeaders () {
		string servicio_id = requestLideres + "getLeaders/" + userData.userHash;
		WWW www = new WWW (servicio_id,null,headerInfo);
		while (!www.isDone) {
			//se espera a que la descarga se salga
		}
		if (!string.IsNullOrEmpty (www.error)) {
			//Aquí sería regresar a la página de persistent, o algo mas
			Debug.LogError ("Cannot load game data!");
			Application.Quit ();
		} else {
			Debug.Log ("DATOS LIDERES CARGADOS EXITOSAMENTE");
			dataAsJson = www.text;
			Data loadedData = JsonUtility.FromJson <Data> (dataAsJson);
			leaderData = loadedData;
		}
	}

	public UserData buildUser (string jsonDatos) {
		UserData temporal;
		temporal = JsonUtility.FromJson<UserData> (dataAsJson);
		temporal.medal_data = new MedalData[9];
		for (int i = 0; i < temporal.medal_data.Length; i++) {
			string servicio_id = requestMedallas + "getMedallas/" + temporal.userHash + "/" + (i + 1);
			WWW www = new WWW (servicio_id,null,headerInfo);
			while (!www.isDone) {
				//se espera a que la descarga se salga
			}
			if (!string.IsNullOrEmpty (www.error)) {
				//Aquí sería regresar a la página de persistent, o algo mas
				Debug.LogError ("Cannot load game data!");
				Application.Quit ();
			} else {
				Debug.Log ("DATOS MEDALLAS CARGADOS EXITOSAMENTE");
				dataAsJson = www.text;
				MedalData loadedData = JsonUtility.FromJson <MedalData> (dataAsJson);
				temporal.medal_data [i] = loadedData;
			}
		}
		return temporal;
	}

	public bool[] getUserMedalUnit(int i){
		return userData.getMedalDataUnit (i);
	}

	public MedalData getUserMedalForUnit(int i){
		return userData.getMedalDataForUnit (i);
	}

	public void UpdateMaxLevel () {
		string servicio_id = requestsUrlUser + "updateLevel/" + userData.userHash;
		WWW www = new WWW (servicio_id,null,headerInfo);
		while (!www.isDone) {
			//se espera a que la descarga se salga
		}
		if (!string.IsNullOrEmpty (www.error)) {
			//Aquí sería regresar a la página de persistent, o algo mas
			Debug.LogError ("Cannot load game data!");
			Application.Quit ();
		} else {
			Debug.Log ("DATOS MAXIMO NIVEL ACTUALIZADO EXITOSAMENTE");
		}
	}

	public void UpdateUserAvatar (int avatarPos) {
		string servicio_id = requestsUrlUser + "updateAvatar/" + userData.userHash + "/" + avatarPos;
		WWW www = new WWW (servicio_id,null,headerInfo);
		while (!www.isDone) {
			//se espera a que la descarga se salga
		}
		if (!string.IsNullOrEmpty (www.error)) {
			//Aquí sería regresar a la página de persistent, o algo mas
			Debug.LogError ("Cannot load game data!");
			Application.Quit ();
		} else {
			Debug.Log ("DATOS AVATAR ACTUALIZADO EXITOSAMENTE");
		}
	}

	public void UpdateLeader (float time){
		string servicio_id = requestLideres + "updatePosition/" + userData.userHash + "/" + (currentChallenge + 1) + "/" + time;
		WWW www = new WWW (servicio_id,null,headerInfo);
		while (!www.isDone) {
			//se espera a que la descarga se salga
		}
		if (!string.IsNullOrEmpty (www.error)) {
			//Aquí sería regresar a la página de persistent, o algo mas
			Debug.LogError ("Cannot load game data!");
			Application.Quit ();
		} else {
			Debug.Log ("DATOS LEADERBOARD ACTUALIZADO EXITOSAMENTE");
		}
	}

	public bool CheckNewUser(){
		string filepath = "";
		filepath = Path.Combine (savepath, indicator);
		if (!File.Exists (filepath)) {
			return false;
		} else {
			string contenido = File.ReadAllText (filepath);
			if (contenido.Equals ("pase")) {
				return true;
			} else {
				return false;
			}
		}
	}

	public void WriteNewUser(){
		string filepath = Path.Combine (savepath, indicator);
		File.WriteAllText (filepath,"pase");
	}

	public void gameStart(){
		headerInfo.Add ("Authorization", "Basic " + System.Convert.ToBase64String (System.Text.Encoding.ASCII.GetBytes ("admingamma:admin")));
		//AQUI INVOCAR EL SERVICIO QUE LLAMA AL LEADERBOARD
		savepath= Application.persistentDataPath;
		DontDestroyOnLoad (gameObject);
		//InitializeDataRL ();
		//LoadGameData ();
		string filepath = "";
		filepath = Path.Combine (savepath, userDataFileName);
		if (!File.Exists (filepath)) {
			//aqui mandar a pantalla de login
			SceneManager.LoadScene ("LoginScene");
		} else {
			dataAsJson = File.ReadAllText (filepath);
			string user_hash = dataAsJson;
			string servicio_id = requestsUrlUser + "find/" + user_hash;
			WWW www = new WWW (servicio_id,null,headerInfo);
			while (!www.isDone) {
				//se espera a que la descarga se salga
			}
			if (!string.IsNullOrEmpty (www.error)) {
				//Aquí sería regresar a la página de persistent, o algo mas
				Debug.Log(user_hash);
				Debug.Log(www.error);
				Application.Quit ();
			} else {
				dataAsJson = www.text;
				userData = buildUser (dataAsJson);


				SceneManager.LoadScene ("MenuScreen");
			}
		}
	}

	public void updateQuestionData(int id_pregunta, int correctas, int fallidas){
		string servicio_id = requestUrlQuestions + "updateQuestionData/" + id_pregunta + "/" + correctas + "/" + fallidas;
		WWW www = new WWW (servicio_id,null,headerInfo);
		Debug.Log (servicio_id);
		while (!www.isDone) {
			//se espera a que la descarga se salga
		}
		if (!string.IsNullOrEmpty (www.error)) {
			Debug.Log (id_pregunta);
			Debug.Log (correctas);
			Debug.Log (fallidas);
			//Aquí sería regresar a la página de persistent, o algo mas
			Debug.LogError ("Cannot update question data!");
			Application.Quit ();
		} else {
			Debug.Log ("DATOS PREGUNTA ACTUALIZADO EXITOSAMENTE");
		}
	}

	public void updateFichaComp(string userHash, int nivel, int pacierto, int perror, int iacierto, int ierror, int eacierto, int eerror){
		string servicio_id = requestMedallas + "updateFichaData/" + userHash + "/" + nivel + "/" + pacierto + "/" + perror + "/" + iacierto + "/" + ierror + "/" + eacierto + "/" + eerror;
		WWW www = new WWW (servicio_id,null,headerInfo);
		while (!www.isDone) {
			//se espera a que la descarga se salga
		}
		if (!string.IsNullOrEmpty (www.error)) {
			//Aquí sería regresar a la página de persistent, o algo mas
			Debug.LogError ("Cannot update ficha!");
			Application.Quit ();
		} else {
			Debug.Log ("DATOS FICHA ACTUALIZADO EXITOSAMENTE");
		}
	}
}