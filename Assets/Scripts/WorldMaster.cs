using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class WorldMaster : MonoBehaviour {

	private DataController dataController;
	public GameObject playerParent;
	public GameObject unidadPrefab;
	public GameObject player;
	public float speed = 2.5f;
	public GameObject drone;
	public AudioSource source;
	public AudioClip teleport;

	public float rotationSpeed = 50f;
	public int level;
	public int minLevel=1;
	public int maxLevel;
	public Image blackScreen;
	public float fadeSpeed=0.5f;
	public Animator anim;
	public GameObject levelsPanel;
	public Canvas parentCanvas;
	public Image medalGot;
	public Sprite [] medalImages;
	public GameObject loadingPanelPrefab;
	private GameObject loadingPanelInstance;

	private ParticleSystem particles;

	private TextMesh[] logoText;
	private GameObject unidadPlace;
	private bool done;
	private int direction;
	private bool blackFade;
	private bool blackFadeOut;
	private bool triggerPortal;
	private bool jump;
	private GameObject portal;
	private bool anyAnim;
	private bool playAnim;
	private Vector3 target;
	private Vector3 halfWay;
	public Button leftB;
	public Button rightB;

	void Awake () {
		dataController = FindObjectOfType<DataController>();
		maxLevel = dataController.GetUserData ().maximo_nivel;
		level = dataController.GetUserData ().ultimo_nivel_visitado;
	}

	void Start () {
	

		unidadPlace= (GameObject)GameObject.Instantiate (unidadPrefab);
		unidadPrefab = null;
		unidadPlace.SetActive (true);
		unidadPlace.transform.position= new Vector3 ((level-1)*2f*1.65f, 0.02f,0f);

		logoText = unidadPlace.transform.Find ("Logo").gameObject.GetComponentsInChildren<TextMesh> ();
		logoText[0].text = level.ToString();
		logoText[1].text = level.ToString();
		portal = unidadPlace.transform.Find ("blackHole").gameObject;
		particles = unidadPlace.transform.Find ("spiral").gameObject.GetComponent<ParticleSystem>();

		playerParent.transform.position=new Vector3 ((level-1)*2f*1.65f, 0, 0);

		anyAnim = false;
		blackFade = false;
		blackFadeOut = false;
		triggerPortal = false;
		playAnim = false;
		jump = false;
		done = true;

		showNewMedal ();
		processButton ();
		//loadingPanelInstance = (GameObject)Instantiate (loadingPanelPrefab, new Vector3 (0,0,0), Quaternion.identity);
		//loadingPanelInstance.transform.SetParent (parentCanvas.transform,false);
		//Invoke ("stopLoading", 3f);

	}	

	void Update () {



		if (!done && anyAnim && !playAnim) {
			
			playerParent.transform.position= Vector3.MoveTowards(playerParent.transform.position,halfWay, speed*Time.deltaTime);

			anim.SetBool ("isWalking", true);
			done = newLocation();
			if (done) {
				unidadPlace.transform.position= new Vector3 (unidadPlace.transform.position.x + (direction*-1.65f*2), 0.02f, 0f);
				level += -1*direction;
				dataController.GetUserData ().ultimo_nivel_visitado = level;
				logoText[0].text = level.ToString();
				logoText[1].text = level.ToString();
				showNewMedal ();
				processButton ();
			}
		}
		else if (done && anyAnim&&!blackFade&&!blackFadeOut && !playAnim) {
			if (!(checkLocation())) {
				playerParent.transform.position= Vector3.MoveTowards(playerParent.transform.position, target, speed*Time.deltaTime);
			} 
			else {

				anim.SetBool ("isWalking", false);
				anyAnim = false;
			}

		}

		if (blackFade) {
			if (blackScreen.color [3] <= 1) {
				blackScreen.color = blackScreen.color + new Color (0f, 0f, 0f, fadeSpeed*Time.deltaTime);
			} else  {
				logoText [0].text = level.ToString ();
				logoText [1].text = level.ToString ();
				blackFade=!blackFade;
				blackFadeOut = true;
				playerParent.transform.position = target;
				unidadPlace.transform.position = target;


			}

		} else if (blackFadeOut) {
			if (blackScreen.color [3] >= 0) {
				blackScreen.color = blackScreen.color - new Color (0f, 0f, 0f, fadeSpeed*Time.deltaTime);

			} else {
				
				blackFadeOut = false;
				anyAnim = false;

			}
		}

		if (jump && (playerParent.transform.position.y > -0.5)) {
			playerParent.transform.Translate (0, -0.25f * Time.deltaTime, 0);
		} else if (jump&&triggerPortal) {
			
			triggerPortal = false;
			jump = false;
			anim.SetBool ("Jumping", false);
			portal.SetActive (false);
			playerParent.GetComponentInChildren<BoxCollider> ().enabled = true;
			playerParent.transform.position = new Vector3(playerParent.transform.position.x,0f,0f);
			player.transform.position=target;
			anyAnim = false;
			playAnim = false;
		}
		



	}
	public void movement(int direction){
		if (!anyAnim && done && (level-direction >= minLevel) && (level-direction<= maxLevel)) {
			this.direction = direction;
			if (direction > 0) {
				player.transform.eulerAngles = new Vector3 (0, 270, 0);
				drone.transform.localScale= new Vector3 (0.05f, 0.05f, 1f);


			} else {
				player.transform.eulerAngles = new Vector3 (0, 90, 0);
				drone.transform.localScale= new Vector3 (-0.05f, 0.05f, 1f);
			}
			target = new Vector3 (playerParent.transform.position.x+(direction*-1.65f*2f), 0f, 0f);
			halfWay= new Vector3 (playerParent.transform.position.x+(direction*-1.65f), 0f, 0f);
			done = false;
			anyAnim = true;
		}


	}

	bool newLocation(){
		return (halfWay[0]) == (playerParent.transform.position.x);
	}

	bool checkLocation(){
		return  target[0] ==playerParent.transform.position.x;
	}


	public void play(){
		if (!anyAnim) {
			anyAnim = true;
			playAnim = true;
			triggerPortal = true;
			portal.gameObject.SetActive (triggerPortal);
			target = player.transform.position;
			StartCoroutine (playAnimation ());
		}
	}


	public void changeLevel(int newLevel){
		if (newLevel > maxLevel) {

		}
		else if (((level < newLevel) || (level > newLevel))&& !anyAnim ) {
			target = new Vector3 ((newLevel-1)*2f*1.65f, 0, 0);
			blackFade = !blackFade;
			level = newLevel;
			anyAnim = true;
			dataController.GetUserData ().ultimo_nivel_visitado = level;
			dataController.SaveUserJSON ();
			StartCoroutine (fadeAnimation (level));
			levelsPanel.SetActive (false);

		}


	}



	public IEnumerator playAnimation(){
		source.PlayOneShot (teleport);
		yield return new WaitForSeconds (0.75f);
		anim.SetBool ("Jumping", true);
		yield return new WaitForSeconds (0.75f);
		jump = true;
		player.GetComponent<BoxCollider> ().enabled = false;
		Invoke ("toLevel", 1f);
	}

	public IEnumerator fadeAnimation(int nivel){
		source.PlayOneShot (teleport);
		particles.Play ();
		anim.SetBool ("Jumping", true);
		yield return new WaitForSeconds (1.5f);
		anim.SetBool ("Jumping", false);
		particles.Stop ();
		showNewMedal ();
		processButton ();
	}
	
	public void showNewMedal(){
		bool[] temp = dataController.getUserMedalUnit (level - 1);
		if (temp[0]) {
			if (temp[1]) {
				if (temp[2]) {
					medalGot.sprite = medalImages [3];
				} else {
					medalGot.sprite = medalImages [2];
				}
			} else {
				medalGot.sprite = medalImages [1];
			}
		} else {
			medalGot.sprite = medalImages [0];
		}
		//Destroy (loadingPanelInstance);
		//loadData.SetActive (false);
	}

	public void processButton(){
		if (level == 1 && dataController.GetUserData ().maximo_nivel == 1) {
			leftB.interactable = false;
			rightB.interactable = false;
		}
		else if (level == 1) {
			leftB.interactable = false;
			rightB.interactable = true;
		} else if (level == dataController.GetUserData ().maximo_nivel) {
			leftB.interactable = true;
			rightB.interactable = false;
		}
	}


	public void showLoadingPanel(){
		loadingPanelPrefab.SetActive (true);
	}

	public void stopLoading(){
		loadingPanelPrefab.SetActive (false);
	}

	public void toLevel (){
		//UnityEngine.SceneManagement.SceneManager.LoadScene("Level Test");
		DataController.nextScene="Level Test";
		UnityEngine.SceneManagement.SceneManager.LoadScene("SceneLoader");
	}

}
