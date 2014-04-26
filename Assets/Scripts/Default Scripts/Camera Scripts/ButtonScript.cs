using UnityEngine;
using System.Collections;

public class ButtonScript : MonoBehaviour {

	// Store all camera points as Vectors
	public Vector3 posTitle;
	public Vector3 posMain;
	public Vector3 posSP;
	public Vector3 posMP;
	public Vector3 posOpts;
	public Vector3 posNow;

	// These are shared between all buttons
	public Material buttonOn;
	public Material buttonOff;
	public float moveSpeed;

	// Detect all possible Main Menu buttons
	GameObject startButton;
	GameObject singleplayerButton;
	GameObject multiplayerButton;
	GameObject optionsButton;
	GameObject quitButton;
	GameObject backButton1;
	GameObject backButton2;	
	GameObject backButton3;

	void Awake() {
		// Find all button gameObjects and define them
		startButton = GameObject.Find("ButtonStart");
		singleplayerButton = GameObject.Find("ButtonSP");
		multiplayerButton = GameObject.Find("ButtonMP");
		optionsButton = GameObject.Find("ButtonOpts");
		quitButton = GameObject.Find("ButtonQuit");
		backButton1 = GameObject.Find("ButtonBack1");
		backButton2 = GameObject.Find("ButtonBack2");
		backButton3 = GameObject.Find("ButtonBack3");

		// Find all empty CamLocation positions and define them
		posTitle = GameObject.Find("TitleScreen").transform.position;
		posMain = GameObject.Find("MainMenu").transform.position;
		posSP = GameObject.Find("SinglePlayer").transform.position;
		posMP = GameObject.Find("Multiplayer").transform.position;
		posOpts = GameObject.Find("Options").transform.position;
		posNow = posTitle;

		Camera.main.transform.position = posNow;
	}
	
	void Update() {
	}
}