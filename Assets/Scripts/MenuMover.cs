using UnityEngine;
using System.Collections;

public class MenuMover : MonoBehaviour {

	// Store all camera points as Vectors
	Vector3 posTitle;
	Vector3 posMain;
	Vector3 posSP;
	Vector3 posMP;
	Vector3 posOptions;
	Vector3 posDest;			// This is a placeholder vector for "where are we going"

	public float moveSpeed = 0.0f;
	private bool isMoving = false;

	enum activeMenu {
		Title = 0,
		Main = 1,
		Singleplayer = 2,
		Multiplayer = 3,
		Options = 4,
	}
	private activeMenu _activeMenu;
	private activeMenu _destMenu;

	// Use this for initialization
	void Start () {
		posTitle = GameObject.Find("TitleScreen").transform.position;
		posMain = GameObject.Find("MainMenu").transform.position;
		posSP = GameObject.Find("SinglePlayer").transform.position;
		posMP = GameObject.Find("Multiplayer").transform.position;
		posOptions = GameObject.Find("Options").transform.position;
		posDest = GameObject.Find("TitleScreen").transform.position;
		transform.position = posTitle;
		_activeMenu = activeMenu.Title;
		_destMenu = activeMenu.Title;
	}
	
	// Update is called once per frame
	void Update () {
		MenuListener();
		DoMoving();
	}

	// Listen for key-presses, then determine/assign the next menu page to load up.
	void MenuListener() {
		if ((_activeMenu == activeMenu.Title) && (Input.GetKey("down")) && (isMoving == false)) {
			Debug.Log("Moving Title to Main");
			_destMenu = activeMenu.Main;
			posDest = posMain;
		}
		if ((_activeMenu == activeMenu.Main) && (Input.GetKey("left")) && (isMoving == false)) {
			Debug.Log("Moving Main to MP");
			_destMenu = activeMenu.Multiplayer;
			posDest = posMP;
		}
		if ((_activeMenu == activeMenu.Main) && (Input.GetKey("right")) && (isMoving == false)) {
			Debug.Log("Moving Main to SP");
			_destMenu = activeMenu.Singleplayer;
			posDest = posSP;
		}
		if ((_activeMenu == activeMenu.Main) && (Input.GetKey("down")) && (isMoving == false)) {
			Debug.Log("Moving Main to Options");
			_destMenu = activeMenu.Options;
			posDest = posOptions;
		}
		if ((_activeMenu == activeMenu.Singleplayer) && (Input.GetKey("left")) && (isMoving == false)) {
			Debug.Log("Moving SP to Main");
			_destMenu = activeMenu.Main;
			posDest = posMain;
		}
		if ((_activeMenu == activeMenu.Multiplayer) && (Input.GetKey("right")) && (isMoving == false)) {
			Debug.Log("Moving MP to Main");
			_destMenu = activeMenu.Main;
			posDest = posMain;
		}

		if ((_activeMenu == activeMenu.Options) && (Input.GetKey("up")) && (isMoving == false)) {
			Debug.Log("Moving Options to Main");
			_destMenu = activeMenu.Main;
			posDest = posMain;
		}
	}

	// Read destination, then lerp to it
	void DoMoving(){
		Debug.Log("Made it to MenuMover");
		Vector3 posNow = transform.position;

		if (_activeMenu != _destMenu) {
			isMoving = true;
			transform.position = Vector3.MoveTowards(posNow, posDest, moveSpeed * Time.deltaTime);
			Debug.Log("Inside if-loop of doMoving");
		}
		if (posNow == posDest) {
			isMoving = false;
			_activeMenu = _destMenu;
			Debug.Log("isMoving now set to false");
		}
	}
}