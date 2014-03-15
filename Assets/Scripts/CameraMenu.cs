using UnityEngine;
using System.Collections;

public class CameraMenu : MonoBehaviour {

		// Store all camera points as Vectors
	Vector3 posTitle;
	Vector3 posMain;
	Vector3 posSP;
	Vector3 posMP;
	Vector3 posOptions;
	Vector3 posDest;			// This is a placeholder vector for "where are we going next"

	public float moveSpeed = 0.0f;
	private bool isMoving = false;

	enum Menu {
		Title = 0,
		Main = 1,
		Singleplayer = 2,
		Multiplayer = 3,
		Options = 4,
		QuitGame = 5,
	}
	private Menu _activeMenu;
	private Menu _destMenu;
	private Menu _activeButton;
	private Menu _destButton;

	// Use this for initialization
	void Start () {
		// Find locations for all CamLocation.Empties, and assign values
		// Also, default everything else to the Title Menu
		posTitle = GameObject.Find("TitleScreen").transform.position;
		posMain = GameObject.Find("MainMenu").transform.position;
		posSP = GameObject.Find("SinglePlayer").transform.position;
		posMP = GameObject.Find("Multiplayer").transform.position;
		posOptions = GameObject.Find("Options").transform.position;
		posDest = GameObject.Find("TitleScreen").transform.position;
		transform.position = posTitle;
		_activeMenu = Menu.Title;
		_destMenu = Menu.Title;
		_activeButton = Menu.Title;
		_destButton = Menu.Title;
		isMoving = false;
	}
	
	// Update is called once per frame
	void Update () {
		MenuListener();
		DoMenuMoving();
	}

	// Listen for key-presses, then determine/assign the next menu page to load up.
	void MenuListener() {
		if ((_activeMenu == Menu.Title) && ((Input.GetKey("return")) || (Input.GetKey("enter"))) && (isMoving == false)) {
			Debug.Log("Moving Title to Main");
			_destMenu = Menu.Main;
			posDest = posMain;
		}
		if ((_activeMenu == Menu.Main) && (Input.GetKey("left")) && (isMoving == false)) {
			Debug.Log("Moving Main to MP");
			_destMenu = Menu.Multiplayer;
			posDest = posMP;
		}
		if ((_activeMenu == Menu.Main) && (Input.GetKey("right")) && (isMoving == false)) {
			Debug.Log("Moving Main to SP");
			_destMenu = Menu.Singleplayer;
			posDest = posSP;
		}
		if ((_activeMenu == Menu.Main) && (Input.GetKey("down")) && (isMoving == false)) {
			Debug.Log("Moving Main to Options");
			_destMenu = Menu.Options;
			posDest = posOptions;
		}
		if ((_activeMenu == Menu.Singleplayer) && (Input.GetKey("left")) && (isMoving == false)) {
			Debug.Log("Moving SP to Main");
			_destMenu = Menu.Main;
			posDest = posMain;
		}
		if ((_activeMenu == Menu.Multiplayer) && (Input.GetKey("right")) && (isMoving == false)) {
			Debug.Log("Moving MP to Main");
			_destMenu = Menu.Main;
			posDest = posMain;
		}

		if ((_activeMenu == Menu.Options) && (Input.GetKey("up")) && (isMoving == false)) {
			Debug.Log("Moving Options to Main");
			_destMenu = Menu.Main;
			posDest = posMain;
		}
	}

	// Read destination, then lerp to it
	void DoMenuMoving(){
		Debug.Log("Made it to DoMenuMoving");
		Vector3 posNow = transform.position;

		if (_activeMenu != _destMenu) {
			isMoving = true;
			transform.position = Vector3.MoveTowards(posNow, posDest, moveSpeed * Time.deltaTime);
			Debug.Log("Inside if-loop of DoMoving");
		}
		if (posNow == posDest) {
			isMoving = false;
			_activeMenu = _destMenu;
			Debug.Log("isMoving now set to false");
		}
	}

}