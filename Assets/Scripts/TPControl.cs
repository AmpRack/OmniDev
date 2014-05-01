using UnityEngine;
using System.Collections;

// For now, this is the main controller for the non-rigidbody scene.
public class TPControl : MonoBehaviour {

// Animation
	public AnimationClip idleAnimation;
	public AnimationClip walkAnimation;
	public AnimationClip boostAnimation;
	public AnimationClip jumpPoseAnimation;
	public AnimationClip deathAnimation;
	public AnimationClip victoryAnimation;
	public AnimationClip failureAnimation;
	public float walkMaxAnimationSpeed = 0.75f;
	public float runMaxAnimationSpeed = 1.0f;
	public float boostMaxAnimationSpeed = 1.0f;
	public float jumpAnimationSpeed = 1.15f;
	public float landAnimationSpeed = 1.0f;
	public float deathAnimationSpeed = 1.0f;
	public float victoryAnimationSpeed = 1.0f;
	public float failureAnimationSpeed = 1.0f;
	private Animation _animation;

// Movement
	public float weight = 20.0f;	 			// How effective ApplyGravity() is; heavier means more gravity
	public float terminalVelocity = 20.0f;		// Maybe instead, just refer to weight again?
	public float jumpHeight = 6.0f;				// Actually, it's vertical velocity!
	public float moveSpeed = 10.0f;				// The normal, unboosted, maximum speed.
	public float deadZone = 0.1f;				// Controller deadzone; minimum input needed to register
	public Vector3 moveDirection {get; set;}	// Determines overall direction of movement
	public float verticalVelocity {get; set;}	// Current vertical movement
	public int maxBoostFactor = 200;			// Maximum boost range, percentage-wise
	private int boostFactor = 100;				// Current percentage of boost.
	private int boostTimer = 100;				// Length of time we can be boosting
	private bool isBoosting = false;			// Using a boost, or in a speed-up area; boost is more than 100%
	private bool isSlowing = false;				// Stuck in a slow-down area; boost is less than 100%

// Miscellaneous
	class vehicleParts {						// Eventually, this will be fed into KillUni. Hopefully.
		GameObject pedalLeft = GameObject.Find("PedalLeft");			
		GameObject pedalRight = GameObject.Find("PedalRight");			
		GameObject seat = GameObject.Find("Seat");
		GameObject frame = GameObject.Find("Frame");
		GameObject pedalAxle = GameObject.Find("PedalAxle");
		GameObject spokes = GameObject.Find("Spokes");
		GameObject spokeRings = GameObject.Find("SpokeRings");
		GameObject tireTube = GameObject.Find("TireTube");
		GameObject reflector = GameObject.Find("Reflector");
		GameObject reflectorBack = GameObject.Find("ReflectorBack");
		GameObject metalRim = GameObject.Find("MetalRim");
		GameObject axleRod = GameObject.Find("AxleRod");
	}
	vehicleParts _unicycleParts;

	enum CharacterState {
		Idle = 0,
		Walking = 1,
		Running = 2,
		Boosting = 3,
		Jumping = 4,
		Death = 5,
		Victory = 6,
		Failure = 7,
	}
	private CharacterState _characterState;
	private bool isControllable = true;		// This doesn't do much right now


	CharacterController controller;
	
	void Awake() {
		controller = gameObject.GetComponent("CharacterController") as CharacterController;
		CheckAnimations();
		_characterState = CharacterState.Idle;
	}
	
	void Update() {
		if (_characterState == CharacterState.Death) {
			KillUni();
		}
		MovementListener();					// Listen for horizontal input
		InputListener();					// Listen for jumping or boosting
		ApplyMovement();					// Convert input to actual movement
		Realignment();						// Move back to Z=0 to keep aligned on the track
		DebugMessenger();					// Throws variable reports
		isBoosting = false;
		if (boostTimer < 0) {
			boostTimer = 0;
		}
	}

	void MovementListener() {
		verticalVelocity = moveDirection.y;
		moveDirection = Vector3.zero;
		if ((Input.GetAxis("Horizontal") > deadZone) || (Input.GetAxis("Horizontal") < -deadZone)) {
			moveDirection += new Vector3(0, 0, Input.GetAxis("Horizontal"));
		}
	}

	void InputListener() {
		if (Input.GetButton ("Jump")) {
			Jump();
		}
		if (Input.GetButton ("Boost")) {
			Boost();
		}
	}

	void Jump(){	// Should also figure out how to do down-jumping through a platform...
		if (controller.isGrounded) {
			verticalVelocity = jumpHeight;
		}
	}
	
	void Boost() {
		if ((boostTimer > 0) && (boostFactor <= maxBoostFactor)) {
			isBoosting = true;
			boostFactor += 3;
			if (boostFactor > maxBoostFactor) { 
				boostFactor = maxBoostFactor;
			}
			boostTimer--;
		}
	}

	void ApplyMovement(){
		moveDirection = transform.TransformDirection(moveDirection);
		if (moveDirection.magnitude > 1) {
			moveDirection = Vector3.Normalize(moveDirection);
		}
		CheckBoost();		// This is to make sure the boostFactor is in the proper bounds, before it gets applied
		moveDirection *= (moveSpeed * (boostFactor / 100));
		moveDirection = new Vector3(moveDirection.x, verticalVelocity, moveDirection.z);
		ApplyWeight();
		controller.Move(moveDirection * Time.deltaTime); 
	}

	void CheckBoost() {
		// If you're going faster than your normal speed, slow down a bit
		if ((!isSlowing) & (!isBoosting) & (boostFactor > 100)) {
			boostFactor -= 4;
		}
		// If you're going slower for any normal reason, speed back up.
		if ((!isSlowing) & (boostFactor < 100)) {
			boostFactor = 100;
		}
		// If you hit your speed limit while boosting, reset to maxBoostFactor
		if ((isBoosting) & (boostFactor > maxBoostFactor)) {	
			boostFactor = maxBoostFactor;
		}
		// If you go too slow while slowing, reset to 1. Slowing should NEVER stop you. 
		if ((isSlowing) & (boostFactor < 1)) {			
			boostFactor = 1;
		}
		// Make sure boostTimer cant be negative
		if (boostTimer < 0) {
			boostTimer = 0;
		}
	}

	void ApplyWeight(){
		if (moveDirection.y > -terminalVelocity) {
			moveDirection = new Vector3(moveDirection.x, (moveDirection.y - (weight * Time.deltaTime)), moveDirection.z);
		}
		if (controller.isGrounded && moveDirection.y < -1) {
			moveDirection = new Vector3(moveDirection.x, (-1), moveDirection.z);
		}
	}

// Realigns the player to the track
	void Realignment() {			
		Vector3 temp = transform.position;
		temp.z = 0;
		transform.position = temp;
	}

// The death sequence
// Give each gameObject a random moveDirection, apply gravity, pause, respawn
	void KillUni() {
		isControllable = false;
		Input.ResetInputAxes();
	}

// Make sure that all vehicle animations load correctly, and disable it otherwise
	void CheckAnimations() {
		_animation = GetComponent<Animation>();

		if (_animation == false)
			Debug.Log ("No animations found!");
		
		if (idleAnimation == false) {
			_animation = null;
			Debug.Log ("No idle animation found. Turning off animations.");
		}
		if (walkAnimation == false) {
			_animation = null;
			Debug.Log ("No walk animation found. Turning off animations.");
		}
		if (boostAnimation == false) {
			_animation = null;
			Debug.Log ("No boost animation found. Turning off animations.");
		}
		if (jumpPoseAnimation == false) {
			_animation = null;
			Debug.Log ("No jump animation found. Turning off animations.");
		}
		if (deathAnimation == false) {
			_animation = null;
			Debug.Log ("No death animation found. Turning off animations.");
		}
		
		if (victoryAnimation == false) {
			_animation = null;
			Debug.Log ("No victory animation found. Turning off animations.");
		}
		
		if (failureAnimation == false) {
			_animation = null;
			Debug.Log ("No failure animation found. Turning off animations.");
		}
	}

	void DebugMessenger() {
		Debug.Log ("Current boostFactor is " + boostFactor + ", and boostTimer is " + boostTimer); 
	}

// In case of emergency, this is the super stripped down version for testing.
//	public float moveSpeed = 10.0F;
//	public float weight = 20.0F;
//	private Vector3 moveDirection = Vector3.zero;
//	void Update() {
//		CharacterController controller = GetComponent<CharacterController>();
//		if (controller.isGrounded) {
//			moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
//			moveDirection = transform.TransformDirection(moveDirection);
//			moveDirection *= speed;
//		moveDirection.y -= weight * Time.deltaTime;
//		controller.Move(moveDirection * Time.deltaTime);


}