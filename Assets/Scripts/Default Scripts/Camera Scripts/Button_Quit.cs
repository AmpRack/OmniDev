using UnityEngine;
using System.Collections;

public class Button_Quit : MonoBehaviour {

	ButtonScript buttons;		// Call the main script for variables
	
	void Start(){
		buttons = GameObject.Find("ButtonHolder").GetComponent<ButtonScript>();
	}
	
	void Update(){
	}
	
	void OnMouseEnter(){
		renderer.material = buttons.buttonOn;
	}
	
	void OnMouseExit(){
		renderer.material = buttons.buttonOff;
	}
	
	void OnMouseUp(){
		// Need to throw up a gui confirmation first, and then quit.
		Debug.Log ("Quit Game button pressed.");
		Application.Quit();
	}
}