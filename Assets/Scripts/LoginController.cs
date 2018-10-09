using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Security.Cryptography;
using System.Text;
public class LoginController : MonoBehaviour
{

	private DataController dataController;
	public InputField userInputField;
	public InputField passwordInputField;
	public Button loginButton;
	public GameObject loginScreen;
	public GameObject avatarScreen;
	private UserData user;
	public GameObject errorDialog;
	private string hashToBeSaved;
	private string savepath;
	private string userDataFileName = "user.txt";
	public bool genero = true; //true = hombre, false = mujer.
	public int pelo = 0; //para los hombres es del 0 al 8, para las mujeres es del 9 al 17
	public int tono = 0; //para los hombres es del 0 al 2, para las mujeres es del 3 al 5
	public Sprite [] pelos;
	public Sprite [] tonos;
	public Image tonoPiel;
	public Image elPelo;
	public Text gender;
	public Text tone;
	public Text hair;
	public Text color;
	public GameObject loadingPanelPrefab;
	private GameObject loadingPanelInstance;
	public Canvas parentCanvas;

	void Start ()
	{
		savepath= Application.persistentDataPath;
		dataController = FindObjectOfType<DataController> ();
	}

	void Update ()
	{

	}

	public void LoginButtonClicked ()
	{
		loginButton.interactable = false;
		showLoadingPanel ();
		Invoke ("startingLogin", 2f);

	}

	public void startingLogin(){
		loginRequest (userInputField.text, passwordInputField.text);	
	}

	public void loginRequest (string username, string password)
	{
		
		bool foundError = false;
		string dataAsJson;
		string hashP = hashFunc (password);
		WWW www = new WWW (DataController.requestsUrlUser + "login/" + username + "/" + hashP,null,dataController.headerInfo);
		while (!www.isDone) {
			//se espera a que la descarga se salga
		}
		if (!string.IsNullOrEmpty (www.error)|| string.IsNullOrEmpty(www.text)) {
			stopLoading ();
			Debug.Log (hashP);
			Debug.LogError (www.error);
			errorDialog.SetActive (true);

		} else {
			string hashId = www.text;
			hashToBeSaved = www.text;
			www = new WWW (DataController.requestsUrlUser + "find/" + hashId,null,dataController.headerInfo);
			while (!www.isDone) {
				//se espera a que la descarga se salga
			}
			if (!string.IsNullOrEmpty (www.error) || string.IsNullOrEmpty(www.text)) {
				stopLoading ();
				errorDialog.SetActive (true);
				foundError = true;
				Destroy (loadingPanelInstance);
			} else {
				dataAsJson = www.text;
				user = JsonUtility.FromJson<UserData> (dataAsJson);
				user.medal_data = new MedalData[9];
				for (int i = 0; i < user.medal_data.Length; i++) {
					string servicio_id = DataController.requestMedallas + "getMedallas/" + user.userHash + "/" + (i+1);
					Debug.Log (DataController.requestMedallas + "getMedallas/" + user.userHash + "/" + (i+1));
					www = new WWW (servicio_id,null,dataController.headerInfo);
					while (!www.isDone) {
						//se espera a que la descarga se salga
					}
					if (!string.IsNullOrEmpty (www.error) || string.IsNullOrEmpty (www.text)) {
						stopLoading ();
						errorDialog.SetActive (true);
						foundError = true;
						Destroy (loadingPanelInstance);
					} else {
						Debug.Log ("DATOS MEDALLAS CARGADOS EXITOSAMENTE");
						dataAsJson = www.text;
						MedalData loadedData = JsonUtility.FromJson <MedalData> (dataAsJson);
						user.medal_data [i] = loadedData;
					}
				}
				if (!foundError) {
					stopLoading ();
					if (user.avatar > -1) {
						dataController.SetUserData (user);
						string filepath = Path.Combine (savepath, userDataFileName);
						File.WriteAllText (filepath, hashToBeSaved); //creates or overwrite
						//DataRL.saveDataToDisk (user);
						SceneManager.LoadScene ("MenuScreen");
					} else {
						toAvatarScreen ();
					}

				}
			}
		}
	}

	public void toAvatarScreen ()
	{
		loginScreen.SetActive (false);
		Destroy (loginScreen);
		avatarScreen.SetActive (true);

	}

	public void cambioGenero () {
		genero = !genero;
		if (genero == true) {
			pelo = 0;
			tono = 0;
			tonoPiel.sprite = tonos [tono];
			elPelo.sprite = pelos [pelo];
			gender.text = "HOMBRE";
			tone.text = "TONO 1";
			hair.text = "TIPO 1";
			color.text = "COLOR 1";
		} else {
			pelo = 9;
			tono = 3;
			tonoPiel.sprite = tonos [tono];
			elPelo.sprite = pelos [pelo];
			gender.text = "MUJER";
			tone.text = "TONO 1";
			hair.text = "TIPO 1";
			color.text = "COLOR 1";
		}
	}

	public void cambioTonoDer () {
		string mensaje = "TONO ";
		if (genero && tono == 2) {
			tono = 0;
			tonoPiel.sprite = tonos [tono];
			tone.text = mensaje + "1";
		} else if (!genero && tono == 5) {
			tono = 3;
			tonoPiel.sprite = tonos [tono];
			tone.text = mensaje + "1";
		} else {
			tono++;
			tonoPiel.sprite = tonos [tono];
			if (genero == true) {
				tone.text = mensaje + (tono + 1).ToString ();
			} else {
				tone.text = mensaje + (tono - 2).ToString ();
			}
		}
	}

	public void cambioTonoIzq () {
		string mensaje = "TONO ";
		if (genero && tono == 0) {
			tono = 2;
			tonoPiel.sprite = tonos [tono];
			tone.text = mensaje + "3";
		} else if (genero == false && tono == 3) {
			tono = 5;
			tonoPiel.sprite = tonos [tono];
			tone.text = mensaje + "3";
		} else {
			tono--;
			tonoPiel.sprite = tonos [tono];
			if (genero) {
				tone.text = mensaje + (tono + 1).ToString ();
			} else {
				tone.text = mensaje + (tono - 2).ToString ();
			}
		}
	}

	public void cambioTipoDer () {
		if (genero) {
			if (pelo == 0) {
				pelo = 3;
				elPelo.sprite = pelos [pelo];
				hair.text = "TIPO 2";
			} else if (pelo == 1) {
				pelo = 4;
				elPelo.sprite = pelos [pelo];
				hair.text = "TIPO 2";
			} else if (pelo == 2) {
				pelo = 5;
				elPelo.sprite = pelos [pelo];
				hair.text = "TIPO 2";
			} else if (pelo == 3) {
				pelo = 6;
				elPelo.sprite = pelos [pelo];
				hair.text = "TIPO 3";
			} else if (pelo == 4) {
				pelo = 7;
				elPelo.sprite = pelos [pelo];
				hair.text = "TIPO 3";
			} else if (pelo == 5) {
				pelo = 8;
				elPelo.sprite = pelos [pelo];
				hair.text = "TIPO 3";
			} else if (pelo == 6) {
				pelo = 0;
				elPelo.sprite = pelos [pelo];
				hair.text = "TIPO 1";
			} else if (pelo == 7) {
				pelo = 1;
				elPelo.sprite = pelos [pelo];
				hair.text = "TIPO 1";
			} else if (pelo == 8) {
				pelo = 2;
				elPelo.sprite = pelos [pelo];
				hair.text = "TIPO 1";
			}
		} else {
			if (pelo == 9) {
				pelo = 12;
				elPelo.sprite = pelos [pelo];
				hair.text = "TIPO 2";
			} else if (pelo == 10) {
				pelo = 13;
				elPelo.sprite = pelos [pelo];
				hair.text = "TIPO 2";
			} else if (pelo == 11) {
				pelo = 14;
				elPelo.sprite = pelos [pelo];
				hair.text = "TIPO 2";
			} else if (pelo == 12) {
				pelo = 15;
				elPelo.sprite = pelos [pelo];
				hair.text = "TIPO 3";
			} else if (pelo == 13) {
				pelo = 16;
				elPelo.sprite = pelos [pelo];
				hair.text = "TIPO 3";
			} else if (pelo == 14) {
				pelo = 17;
				elPelo.sprite = pelos [pelo];
				hair.text = "TIPO 3";
			} else if (pelo == 15) {
				pelo = 9;
				elPelo.sprite = pelos [pelo];
				hair.text = "TIPO 1";
			} else if (pelo == 16) {
				pelo = 10;
				elPelo.sprite = pelos [pelo];
				hair.text = "TIPO 1";
			} else if (pelo == 17) {
				pelo = 11;
				elPelo.sprite = pelos [pelo];
				hair.text = "TIPO 1";
			}
		}

	}

	public void cambioTipoIzq () {
		if (genero) {
			if (pelo == 0) {
				pelo = 6;
				elPelo.sprite = pelos [pelo];
				hair.text = "TIPO 3";
			} else if (pelo == 1) {
				pelo = 7;
				elPelo.sprite = pelos [pelo];
				hair.text = "TIPO 3";
			} else if (pelo == 2) {
				pelo = 8;
				elPelo.sprite = pelos [pelo];
				hair.text = "TIPO 3";
			} else if (pelo == 3) {
				pelo = 0;
				elPelo.sprite = pelos [pelo];
				hair.text = "TIPO 1";
			} else if (pelo == 4) {
				pelo = 1;
				elPelo.sprite = pelos [pelo];
				hair.text = "TIPO 1";
			} else if (pelo == 5) {
				pelo = 2;
				elPelo.sprite = pelos [pelo];
				hair.text = "TIPO 1";
			} else if (pelo == 6) {
				pelo = 3;
				elPelo.sprite = pelos [pelo];
				hair.text = "TIPO 2";
			} else if (pelo == 7) {
				pelo = 4;
				elPelo.sprite = pelos [pelo];
				hair.text = "TIPO 2";
			} else if (pelo == 8) {
				pelo = 5;
				elPelo.sprite = pelos [pelo];
				hair.text = "TIPO 2";
			}
		} else {
			if (pelo == 9) {
				pelo = 15;
				elPelo.sprite = pelos [pelo];
				hair.text = "TIPO 3";
			} else if (pelo == 10) {
				pelo = 16;
				elPelo.sprite = pelos [pelo];
				hair.text = "TIPO 3";
			} else if (pelo == 11) {
				pelo = 17;
				elPelo.sprite = pelos [pelo];
				hair.text = "TIPO 3";
			} else if (pelo == 12) {
				pelo = 9;
				elPelo.sprite = pelos [pelo];
				hair.text = "TIPO 1";
			} else if (pelo == 13) {
				pelo = 10;
				elPelo.sprite = pelos [pelo];
				hair.text = "TIPO 1";
			} else if (pelo == 14) {
				pelo = 11;
				elPelo.sprite = pelos [pelo];
				hair.text = "TIPO 1";
			} else if (pelo == 15) {
				pelo = 12;
				elPelo.sprite = pelos [pelo];
				hair.text = "TIPO 2";
			} else if (pelo == 16) {
				pelo = 13;
				elPelo.sprite = pelos [pelo];
				hair.text = "TIPO 2";
			} else if (pelo == 17) {
				pelo = 14;
				elPelo.sprite = pelos [pelo];
				hair.text = "TIPO 2";
			}
		}

	}

	public void cambioColorDer () {
		if (genero == true) {
			if (pelo == 0) {
				pelo = 1;
				elPelo.sprite = pelos [pelo];
				color.text = "COLOR 2";
			} else if (pelo == 1) {
				pelo = 2;
				elPelo.sprite = pelos [pelo];
				color.text = "COLOR 3";
			} else if (pelo == 2) {
				pelo = 0;
				elPelo.sprite = pelos [pelo];
				color.text = "COLOR 1";
			} else if (pelo == 3) {
				pelo = 4;
				elPelo.sprite = pelos [pelo];
				color.text = "COLOR 2";
			} else if (pelo == 4) {
				pelo = 5;
				elPelo.sprite = pelos [pelo];
				color.text = "COLOR 3";
			} else if (pelo == 5) {
				pelo = 3;
				elPelo.sprite = pelos [pelo];
				color.text = "COLOR 1";
			} else if (pelo == 6) {
				pelo = 7;
				elPelo.sprite = pelos [pelo];
				color.text = "COLOR 2";
			} else if (pelo == 7) {
				pelo = 8;
				elPelo.sprite = pelos [pelo];
				color.text = "COLOR 3";
			} else if (pelo == 8) {
				pelo = 6;
				elPelo.sprite = pelos [pelo];
				color.text = "COLOR 1";
			}
		} else {
			if (pelo == 9) {
				pelo = 10;
				elPelo.sprite = pelos [pelo];
				color.text = "COLOR 2";
			} else if (pelo == 10) {
				pelo = 11;
				elPelo.sprite = pelos [pelo];
				color.text = "COLOR 3";
			} else if (pelo == 11) {
				pelo = 9;
				elPelo.sprite = pelos [pelo];
				color.text = "COLOR 1";
			} else if (pelo == 12) {
				pelo = 13;
				elPelo.sprite = pelos [pelo];
				color.text = "COLOR 2";
			} else if (pelo == 13) {
				pelo = 14;
				elPelo.sprite = pelos [pelo];
				color.text = "COLOR 3";
			} else if (pelo == 14) {
				pelo = 12;
				elPelo.sprite = pelos [pelo];
				color.text = "COLOR 1";
			} else if (pelo == 15) {
				pelo = 16;
				elPelo.sprite = pelos [pelo];
				color.text = "COLOR 2";
			} else if (pelo == 16) {
				pelo = 17;
				elPelo.sprite = pelos [pelo];
				color.text = "COLOR 3";
			} else if (pelo == 17) {
				pelo = 15;
				elPelo.sprite = pelos [pelo];
				color.text = "COLOR 1";
			}
		}

	}

	public void cambioColorIzq () {
		if (genero == true) {
			if (pelo == 0) {
				pelo = 2;
				elPelo.sprite = pelos [pelo];
				color.text = "COLOR 3";
			} else if (pelo == 1) {
				pelo = 0;
				elPelo.sprite = pelos [pelo];
				color.text = "COLOR 1";
			} else if (pelo == 2) {
				pelo = 1;
				elPelo.sprite = pelos [pelo];
				color.text = "COLOR 2";
			} else if (pelo == 3) {
				pelo = 5;
				elPelo.sprite = pelos [pelo];
				color.text = "COLOR 3";
			} else if (pelo == 4) {
				pelo = 3;
				elPelo.sprite = pelos [pelo];
				color.text = "COLOR 1";
			} else if (pelo == 5) {
				pelo = 4;
				elPelo.sprite = pelos [pelo];
				color.text = "COLOR 2";
			} else if (pelo == 6) {
				pelo = 8;
				elPelo.sprite = pelos [pelo];
				color.text = "COLOR 3";
			} else if (pelo == 7) {
				pelo = 6;
				elPelo.sprite = pelos [pelo];
				color.text = "COLOR 1";
			} else if (pelo == 8) {
				pelo = 7;
				elPelo.sprite = pelos [pelo];
				color.text = "COLOR 2";
			}
		} else {
			if (pelo == 9) {
				pelo = 11;
				elPelo.sprite = pelos [pelo];
				color.text = "COLOR 3";
			} else if (pelo == 10) {
				pelo = 9;
				elPelo.sprite = pelos [pelo];
				color.text = "COLOR 1";
			} else if (pelo == 11) {
				pelo = 10;
				elPelo.sprite = pelos [pelo];
				color.text = "COLOR 2";
			} else if (pelo == 12) {
				pelo = 14;
				elPelo.sprite = pelos [pelo];
				color.text = "COLOR 3";
			} else if (pelo == 13) {
				pelo = 12;
				elPelo.sprite = pelos [pelo];
				color.text = "COLOR 1";
			} else if (pelo == 14) {
				pelo = 13;
				elPelo.sprite = pelos [pelo];
				color.text = "COLOR 2";
			} else if (pelo == 15) {
				pelo = 17;
				elPelo.sprite = pelos [pelo];
				color.text = "COLOR 3";
			} else if (pelo == 16) {
				pelo = 15;
				elPelo.sprite = pelos [pelo];
				color.text = "COLOR 1";
			} else if (pelo == 17) {
				pelo = 16;
				elPelo.sprite = pelos [pelo];
				color.text = "COLOR 2";
			}
		}

	}

	public void selectAvatar ()
	{
		showLoadingPanel ();
		user.avatar = pelo;
		if (genero) {
			user.avatar+=9*tono;
		} else {
			user.avatar+=9*(3+tono);
		}

		Invoke ("toMenu", 2f);
	}

	public void toMenu(){

		dataController.SetUserData (user);
		dataController.UpdateUserAvatar (user.avatar);
		string filepath = Path.Combine (savepath, userDataFileName);
		File.WriteAllText (filepath, hashToBeSaved); //creates or overwrite
		//DataRL.saveDataToDisk (user);
		SceneManager.LoadScene ("MenuScreen");
	}


	public void closeErrorDialog ()
	{
		errorDialog.SetActive (false);
		loginButton.interactable = true;
	}

	private string hashFunc(string s){
		SHA256 mySHA256 = SHA256Managed.Create();
		byte[] hashValue = mySHA256.ComputeHash(Encoding.UTF8.GetBytes(s));
		int i;
		string result="";
		for (i = 0; i < hashValue.Length; i++)
		{
			result = string.Concat(result,string.Format("{0:X2}", hashValue[i]));
		}
		return result.ToLower();
	}

	public void showLoadingPanel(){
		loadingPanelPrefab.SetActive (true);
	}

	public void stopLoading(){
		loadingPanelPrefab.SetActive (false);
	}


}
