using UnityEngine;
using System.Collections;

public class ButtonScript : MonoBehaviour {

	// Store all camera points as Vectors
	Vector3 posTitle;
	Vector3 posMain;
	Vector3 posSP;
	Vector3 posMP;
	Vector3 posOptions;
	public Vector3 posDest;			// This is a placeholder vector for "where are we going next"
	public Vector3 posNow; 			// Only holds transform.position

	// Define which materials to use when the button is hovered over
	public Material buttonOn;
	public Material buttonOff;

	// Detect all possible Main Menu buttons
	GameObject startButton;
	GameObject singleplayerButton;
	GameObject multiplayerButton;
	GameObject optionsButton;
	GameObject quitgameButton;
	GameObject backButton1;
	GameObject backButton2;	
	GameObject backButton3;
	GameObject mainCamera;

	enum MenuState {
		Title = 0,
		Main = 1,
		Singleplayer = 2,
		Multiplayer = 3,
		Options = 4,
	}
	private MenuState _activeMenu;
	private MenuState _destMenu;

	public static bool isMoving;
//	public float moveSpeed = 20.0f;;
//	private Vector3 buttonPosOff;
//	private Vector3 buttonPosOn;

	void Start () {
		// Find all button objects and define them
		startButton = GameObject.Find("ButtonStart");
		singleplayerButton = GameObject.Find("ButtonSP");
		multiplayerButton = GameObject.Find("ButtonMP");
		optionsButton = GameObject.Find("ButtonOpts");
		quitgameButton = GameObject.Find("ButtonQuit");
		backButton1 = GameObject.Find("ButtonBack1");
		backButton2 = GameObject.Find("ButtonBack2");
		backButton3 = GameObject.Find("ButtonBack3");
		mainCamera = GameObject.Find("Main Camera");
		// Find all empty CamLocation positions and define them
		posTitle = GameObject.Find("TitleScreen").transform.position;
		posMain = GameObject.Find("MainMenu").transform.position;
		posSP = GameObject.Find("SinglePlayer").transform.position;
		posMP = GameObject.Find("Multiplayer").transform.position;
		posOptions = GameObject.Find("Options").transform.position;
		posDest = GameObject.Find("TitleScreen").transform.position;
//		buttonPosOff = transform.position;
//		buttonPosOn = buttonPosOff;
//		buttonPosOn.z = (buttonPosOff.z - 10);

		_activeMenu = MenuState.Title;
		_destMenu = MenuState.Title;

		isMoving = false;

		}
	
	void Update () {
		DoCameraMoving ();
	}

	void OnMouseEnter() {
		renderer.material = buttonOn;
	}

	void OnMouseExit() {
		renderer.material = buttonOff;
	}

	void OnMouseDown() {
		// transform.position = Vector3.MoveTowards(buttonPosOff, buttonPosOn, moveSpeed * Time.deltaTime);
	}


	void OnMouseUp() {
		if (gameObject == startButton) {
			Debug.Log("Clicked Start Button");
			_destMenu = MenuState.Main;
			posDest = posMain;
		}
		if (gameObject == singleplayerButton) {
			Debug.Log("Clicked Single Player Button");
			_destMenu = MenuState.Singleplayer;
			posDest = posSP;
		}
		if (gameObject == multiplayerButton) {
			Debug.Log("Clicked Multiplayer Button");
			_destMenu = MenuState.Multiplayer;
			posDest = posMP;
		}
		if (gameObject == optionsButton) {
			Debug.Log("Clicked Options Button");
			_destMenu = MenuState.Options;
			posDest = posOptions;
		}
		if ((gameObject == backButton1)) {
			Debug.Log("Clicked Back Button");
			_destMenu = MenuState.Main;
			posDest = posMain;
		}
		if ((gameObject == backButton2)) {
			Debug.Log("Clicked Back Button");
			_destMenu = MenuState.Main;
			posDest = posMain;
		}
		if ((gameObject == backButton3)) {
			Debug.Log("Clicked Back Button");
			_destMenu = MenuState.Main;
			posDest = posMain;
		}
		if (gameObject == quitgameButton) {
			Debug.Log("Clicked QuitGame Button");
			Application.Quit();

		// transform.position = Vector3.MoveTowards(buttonPosOn, buttonPosOff, moveSpeed * Time.deltaTime);
		}
	}

	// See if we should be changing pages, and move to it.
	void DoCameraMoving(){
		posNow = mainCamera.transform.position;
		
		if (_activeMenu != _destMenu) {
			isMoving = true;
//			mainCamera.transform.position = Vector3.MoveTowards(posNow, posDest, moveSpeed * Time.deltaTime);
		}
		if (posNow == posDest) {
			isMoving = false;
			_activeMenu = _destMenu;
		}
	}
}