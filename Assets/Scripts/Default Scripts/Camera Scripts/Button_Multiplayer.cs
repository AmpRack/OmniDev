using UnityEngine;
using System.Collections;

public class Button_Multiplayer : MonoBehaviour {

	bool letsMove;				// Detects whether or not we're going between menus
	ButtonScript buttons;		// Call the main script for variables
	
	void Start(){
		buttons = GameObject.Find("ButtonHolder").GetComponent<ButtonScript>();
		letsMove = false;
	}
	
	void Update(){
		DoMoving ();
	}
	
	void OnMouseEnter(){
		renderer.material = buttons.buttonOn;
	}
	
	void OnMouseExit(){
		renderer.material = buttons.buttonOff;
	}
	
	void OnMouseUp(){
		letsMove = true;
	}
	
	void DoMoving(){
		// If the assigned button was clicked, lock-out the other buttons and slide over to the next screen.
		// Once it's done, unlock the buttons.
		Camera.main.transform.position = buttons.posNow;
		if ((letsMove == true) && (buttons.posNow != buttons.posMP)) {
			buttons.posNow = Vector3.MoveTowards(buttons.posNow, buttons.posMP, (buttons.moveSpeed * Time.deltaTime));
		}
		
		if ((letsMove) && (buttons.posNow == buttons.posMP)) {
			letsMove = false;
		}
	}
}