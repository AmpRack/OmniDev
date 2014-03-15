using UnityEngine;
using System.Collections;

public class Button_PressStart : MonoBehaviour {

	GameObject buttons;
	ButtonScript buttonScript;

	void Start() {
		buttons = GameObject.Find("Buttons");
		buttons.GetComponent<ButtonScript>();
	}
	
	void Update() {
		// Broadcast button state to another script
	}

	void OnMouseEnter() {
		// Get material from another script, change button material
	}
	
	void OnMouseExit() {
		// Get material from another script, change button material
	}

	void OnMouseUp() {
		// Send signal that the button has been clicked.
	}
}