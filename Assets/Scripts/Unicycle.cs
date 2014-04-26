using UnityEngine;
using System.Collections;

// This is the first draft of the controller for the rigidbody unicycle.
// Given the nature of rigidbodies, this script may be tossed entirely.

public class Unicycle : MonoBehaviour {

// All gameobjects
// Feel free to remove some of these later, if you don' t use them.
	public WheelCollider uniWheelCol;		// The actual collider
	private GameObject uniWheel;			
	private GameObject pedalLeft;			
	private GameObject pedalRight;			
	private GameObject seat;
	private GameObject frame;
	private GameObject pedalAxle;
	private GameObject spokes;
	private GameObject spokeRings;
	private GameObject tireTube;
	private GameObject reflector;
	private GameObject reflectorBack;
	private GameObject metalRim;
	private GameObject axleRod;

// Rigidbody Specs
	public float offsetX = 0.0f;			// Center of balance, X
	public float offsetY = 0.0f;			// Center of balance, Y
	public float offsetZ = 0.0f;			// Center of balance, Z
	public float motor_max = 10.0f;			// 
	public float brake_max = 100.0f;		// 
	private float motor = 0.0f;				// 
	private float brake = 0.0f;				// 

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
	public float maxSpeed;					// Without boosting, the fastest the vehicle can go
	public float boostSpeed;				// Added speed, beyond maxSpeed
	public bool canJump;					// Can this vehicle jump? NOTE THIS MAY BE REDUNDANT
	public float jumpHeight;				// Unmodified jump height restriction
	private bool jumpingReachedApex;		// Signals if we've hit jumpHeight or not
	private float currentSpeed;				// How fast the vehicle is actually going
	private bool isMoving;					// Tracks if any button is being pressed, NOT the actual vehicle position
	private bool isJumping;					// Tracks if we're in the middle of a jump
	private bool isBoosting;				// Tracks whether or not we're boosting
	private bool isControllable;			// Can disable button-input
	private bool isGrounded;				// Are we actually touching the ground?
	private Vector3 moveDirection;			// Which direction are we moving?
	private CollisionFlags collisionFlags;	// Helps us tell if the player is touching anything

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

	// Initial settings
	void Awake() {
			moveDirection = transform.TransformDirection (Vector3.forward);
	
			_animation = GetComponent<Animation> ();
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
			if ((jumpPoseAnimation && canJump) == false) {
					_animation = null;
					Debug.Log ("No jump animation found and the character has canJump enabled. Turning off animations.");
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

		uniWheel = GameObject.Find("WheelAssembly");
		pedalLeft = GameObject.Find("PedalLeft");
		pedalRight = GameObject.Find("PedalRight");
		seat = GameObject.Find("Seat");

		}

	void Start() {
		rigidbody.centerOfMass = new Vector3(offsetX, offsetY, offsetZ);
	}

	// The main loop
	void FixedUpdate() {
		ApplyAnimation();
//		WheelMovement();
		ApplyJumping();
		Realignment();
	}
	
//	// Actual Movement of the WheelCollider
//	void WheelMovement() {
//		motor = -1 * Mathf.Clamp(Input.GetAxis("Horizontal"), 0, 1);
//		brake = -1 * Mathf.Clamp(Input.GetAxis("Horizontal"), -1, 0);
//		uniWheelCol.motorTorque = motor_max * motor;
//		uniWheelCol.brakeTorque = brake_max * brake;
//		
//		if (Input.GetKey("right")) {
//			uniWheel.transform.Rotate (-400 * Time.deltaTime, 0, 0);
//			pedalLeft.transform.Rotate (400 * Time.deltaTime, 0, 0);
//			pedalRight.transform.Rotate (-400 * Time.deltaTime, 0, 0);
//		}
//		
//		if (Input.GetKey("left")) {
//			uniWheel.transform.Rotate (400 * Time.deltaTime, 0, 0);
//			pedalLeft.transform.Rotate (-400 * Time.deltaTime, 0, 0);
//			pedalRight.transform.Rotate (400 * Time.deltaTime, 0, 0);
//		}
//	}

	// Move the frame back to Z=1, level the seat
	void Realignment() {
		Vector3 temp = transform.position;
		temp.z = 0;
		transform.position = temp;
		Quaternion rotation = Quaternion.LookRotation(Vector3.up , Vector3.right);
		seat.transform.rotation = rotation;
	}

	// The jumping module. 
	void ApplyJumping() {
		if (canJump && Input.GetKey("space")) {
			isJumping = true;
			_characterState = CharacterState.Jumping;
		}
		if (isJumping) {
			transform.position += transform.up * jumpHeight * Time.deltaTime;
			isGrounded = false;
		}
	}

	void ApplyAnimation() {
		if(_animation == true) {
			if(_characterState == CharacterState.Jumping) 
			{
				if(jumpingReachedApex == false) {
					_animation[jumpPoseAnimation.name].speed = jumpAnimationSpeed;
					_animation[jumpPoseAnimation.name].wrapMode = WrapMode.ClampForever;
					_animation.CrossFade(jumpPoseAnimation.name);
				} else {
					_animation[jumpPoseAnimation.name].speed = -landAnimationSpeed;
					_animation[jumpPoseAnimation.name].wrapMode = WrapMode.ClampForever;
					_animation.CrossFade(jumpPoseAnimation.name);				
				}
			} 
//			else 
//			{
//				if(controller.velocity.sqrMagnitude < 0.1f) {
//					_animation.CrossFade(idleAnimation.name);
//				}
//				else 
//				{
//					if(_characterState == CharacterState.Boosting) {
//						_animation[boostAnimation.name].speed = Mathf.Clamp(controller.velocity.magnitude, 0.0f, boostMaxAnimationSpeed);
//						_animation.CrossFade(boostAnimation.name);	
//					}
//					else if(_characterState == CharacterState.Running) {
//						_animation[walkAnimation.name].speed = Mathf.Clamp(controller.velocity.magnitude, 0.0f, walkMaxAnimationSpeed);
//						_animation.CrossFade(walkAnimation.name);	
//					}
//					else if(_characterState == CharacterState.Walking) {
//						_animation[walkAnimation.name].speed = Mathf.Clamp(controller.velocity.magnitude, 0.0f, walkMaxAnimationSpeed);
//						_animation.CrossFade(walkAnimation.name);	
//					}
//				}
//			}
		}
	}
}


// New Jumping module idea
// Save current Y position to temp space
// Lerp to (current Y + jumpHeight)
// When reached, lerp back down to the ground

// Balancing while idle idea
// If (notMoving) 

