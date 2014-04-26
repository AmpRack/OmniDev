using UnityEngine;
using System.Collections;

public class ThirdPersonController : MonoBehaviour 
{

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
	private Animation _animation;

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

// Movement
	public float walkSpeed = 2.0f;					// 'Walking' is the capped speed between 0 and maxSpeed
	public float runSpeed = 4.0f;					// 'Running' is a normal maximum speed. Vehicles cannot move faster than this without special means
	public float boostSpeed = 6.0f;					// 'Boosting' is above max speed, and only happens with special items or boost pads. 
	public float inAirControlAcceleration = 3.0f;	// Amount of control player has while in air.
	public float jumpHeight = 0.5f;					// How high do we jump when pressing jump and letting go immediately
	public float gravity = 20.0f;					// The gravity for the character
	public float speedSmoothing = 10.0f;			// The gravity in controlled descent mode
	public float rotateSpeed = 500.0f;				// How fast to turn around when changing horizontal directions while running
	public float runAfterSeconds = 3.0f;			// Time it takes to go from walking to running
	private float currentSpeed = 0.0f;				// Currently unimplemented, may be unnecessary with moveSpeed, below
	private float speedRatio = 0.0f;				// Ratio of total / maximum speed
	private bool isMoving = false;					// Is the user pressing any keys?
	private float walkTimeStart = 0.0f;				// When did the user start walking (Used for going into trot after a while)
	private bool isControllable = true;				// Controls whether input is passed or not
	private Vector3 moveDirection = Vector3.zero;	// The current move direction in x-z
	private float verticalSpeed = 0.0f;				// The current vertical speed
	private float moveSpeed= 0.0f;					// The current x-z move speed
	private CollisionFlags collisionFlags; 			// The last collision flags returned from controller.Move

// Jumping
	public bool canJump= true;						
	private float jumpRepeatTime= 0.05f;			// Control how quickly one can jump after landing another jump
	private float jumpTimeout= 0.15f;
	private float groundedTimeout= 0.25f;
	private bool jumping= false;					// Are we jumping? (Initiated with jump button and not grounded yet)
	private bool jumpingReachedApex= false;
	private float lastJumpButtonTime= -10.0f;		// Last time the jump button was clicked down
	private float lastJumpTime= -1.0f;				// Last time we performed a jump
	private float lastJumpStartHeight= 0.0f;		// the height we jumped from (Used to determine for how long to apply extra jump power after jumping.)
	private Vector3 inAirVelocity= Vector3.zero;
	private float lastGroundedTime= 0.0f;

// Camera Stuff
	private float lockCameraTimer= 0.0f;			// The camera doesnt start following the target immediately but waits for a split second to avoid too much waving around.
	public float cameraFocalPointDistance = 0.0f;	// Set the maximum distance from vehicle the camera will track ahead
	private GameObject cameraFocalPoint; 			// Initialized here, declared during Awake
	

	void Awake ()
	{
		moveDirection = transform.TransformDirection(Vector3.forward);
		
		_animation = GetComponent<Animation>();
		if(_animation == false)
			Debug.Log("The character you would like to control doesn't have animations. Is this intentional?");
		
		if(idleAnimation == false) {
			_animation = null;
			Debug.Log("No idle animation found. Turning off animations.");
		}
		if(walkAnimation == false) {
			_animation = null;
			Debug.Log("No walk animation found. Turning off animations.");
		}
		if(boostAnimation == false) {
			_animation = null;
			Debug.Log("No boost animation found. Turning off animations.");
		}
		if((jumpPoseAnimation && canJump) == false) {
			_animation = null;
			Debug.Log("No jump animation found and the character has canJump enabled. Turning off animations.");
		}
		if(deathAnimation == false) {
			_animation = null;
			Debug.Log("No death animation found. Turning off animations.");
		}

		if(victoryAnimation == false) {
			_animation = null;
			Debug.Log("No victory animation found. Turning off animations.");
		}

		if(failureAnimation == false) {
			_animation = null;
			Debug.Log("No failure animation found. Turning off animations.");
		}

		cameraFocalPoint = GameObject.Find("CameraFocalPoint");	
		if (cameraFocalPoint == false) {
			Debug.LogError ("GameObject cameraFocalPoint does not exist, or cannot be found.");
		}
	}
	
	
	void  UpdateSmoothedMovementDirection() {
		Transform cameraTransform = Camera.main.transform;
		bool grounded = IsGrounded();
		
		// Forward vector relative to the camera along the x-z plane	
		Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
		forward.y = 0;
		forward = forward.normalized;
		
		// Right vector relative to the camera
		// Always orthogonal to the forward vector
		Vector3 right = new Vector3(forward.z, 0, -forward.x);
		
		float v = Input.GetAxisRaw("Vertical");
		float h = Input.GetAxisRaw("Horizontal");
		
		bool wasMoving = isMoving;
		isMoving = ((Mathf.Abs (h) > 0.1f) || (Mathf.Abs (v) > 0.1f));
		
		// Target direction relative to the camera
		Vector3 targetDirection = ((h * right) + (v * forward));
		
		// Grounded controls
		if (grounded) {
			// Lock camera for short period when transitioning moving & standing still
			lockCameraTimer += Time.deltaTime;
			if (isMoving != wasMoving)
				lockCameraTimer = 0.0f;
			
			// We store speed and direction seperately,
			// so that when the character stands still we still have a valid forward direction
			// moveDirection is always normalized, and we only update it if there is user input.
			if (targetDirection != Vector3.zero)
			{
				// If we are really slow, just snap to the target direction
				if (moveSpeed < walkSpeed * 0.9f && grounded)
				{
					moveDirection = targetDirection.normalized;
				}
				// Otherwise smoothly turn towards it
				else
				{
					moveDirection = Vector3.RotateTowards(moveDirection, targetDirection, rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);
					
					moveDirection = moveDirection.normalized;
				}
			}
			
			// Smooth the speed based on the current target direction
			float curSmooth= speedSmoothing * Time.deltaTime;
			
			// Choose target speed
			//* We want to support analog input but make sure you cant walk faster diagonally than just forward or sideways
			float targetSpeed= Mathf.Min(targetDirection.magnitude, 1.0f);
			
			_characterState = CharacterState.Idle;
			
			// Pick speed modifier
			if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift))
			{
				targetSpeed *= boostSpeed;
				_characterState = CharacterState.Running;
			}
			else if (Time.time - runAfterSeconds > walkTimeStart)
			{
				targetSpeed *= runSpeed;
				_characterState = CharacterState.Running;
			}
			else
			{
				targetSpeed *= walkSpeed;
				_characterState = CharacterState.Walking;
			}
			
			moveSpeed = Mathf.Lerp(moveSpeed, targetSpeed, curSmooth);
			
			// Reset walk time start when we slow down
			if (moveSpeed < walkSpeed * 0.3f)
				walkTimeStart = Time.time;
		}
		// In air controls
		else {
			// Lock camera while in air
			if (jumping == true)
				lockCameraTimer = 0.0f;
			
			if (isMoving == true)
				inAirVelocity += targetDirection.normalized * Time.deltaTime * inAirControlAcceleration;
		}
	}
	
	
	void  ApplyJumping (){
		// Prevent jumping too fast after each other
		if (lastJumpTime + jumpRepeatTime > Time.time)
			return;
		
		if (IsGrounded() == true) {
			// Jump
			// - Only when pressing the button down
			// - With a timeout so you can press the button slightly before landing		
			if (canJump && Time.time < lastJumpButtonTime + jumpTimeout) {
				verticalSpeed = CalculateJumpVerticalSpeed (jumpHeight);
				SendMessage("DidJump", SendMessageOptions.DontRequireReceiver);
			}
		}
	}
	
	
	void  ApplyGravity ()
	{
		if (isControllable == true)	{// don't move player at all if not controllable.
		
			bool jumpButton = Input.GetButton("Jump");
			// When we reach the apex of the jump we send out a message
			if (jumping && !jumpingReachedApex && verticalSpeed <= 0.0f) {
				jumpingReachedApex = true;
				SendMessage("DidJumpReachApex", SendMessageOptions.DontRequireReceiver);
			}
			
			if (IsGrounded() == true)
				verticalSpeed = 0.0f;
			else
				verticalSpeed -= gravity * Time.deltaTime;
		}
	}
	
	public float CalculateJumpVerticalSpeed (float targetJumpHeight)
	{
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt(2 * targetJumpHeight * gravity);
	}
	
	public void DidJump ()
	{
		jumping = true;
		jumpingReachedApex = false;
		lastJumpTime = Time.time;
		lastJumpStartHeight = transform.position.y;
		lastJumpButtonTime = -10;
		
		_characterState = CharacterState.Jumping;
	}
	
	void Update ()
	{
		// kill all inputs if not controllable.		
		if (isControllable == false)
		{
			Input.ResetInputAxes();
		}
		
		if (Input.GetButtonDown("Jump") == true)
		{
			lastJumpButtonTime = Time.time;
		}
		
		UpdateSmoothedMovementDirection();
		
		// Apply gravity
		// - extra power jump modifies gravity
		// - controlledDescent mode modifies gravity
		ApplyGravity ();
		
		// Apply jumping logic
		ApplyJumping ();
		
		// Calculate actual motion
		Vector3 movement= moveDirection * moveSpeed + new Vector3 (0, verticalSpeed, 0) + inAirVelocity;
		movement *= Time.deltaTime;
		
		// Move the controller
		CharacterController controller = GetComponent<CharacterController>();
		collisionFlags = controller.Move(movement);
		
		// ANIMATION sector
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
			else 
			{
				if(controller.velocity.sqrMagnitude < 0.1f) {
					_animation.CrossFade(idleAnimation.name);
				}
				else 
				{
					if(_characterState == CharacterState.Boosting) {
						_animation[boostAnimation.name].speed = Mathf.Clamp(controller.velocity.magnitude, 0.0f, boostMaxAnimationSpeed);
						_animation.CrossFade(boostAnimation.name);	
					}
					else if(_characterState == CharacterState.Running) {
						_animation[walkAnimation.name].speed = Mathf.Clamp(controller.velocity.magnitude, 0.0f, walkMaxAnimationSpeed);
						_animation.CrossFade(walkAnimation.name);	
					}
					else if(_characterState == CharacterState.Walking) {
						_animation[walkAnimation.name].speed = Mathf.Clamp(controller.velocity.magnitude, 0.0f, walkMaxAnimationSpeed);
						_animation.CrossFade(walkAnimation.name);	
					}
					
				}
			}
		}
		// ANIMATION sector
		
		// Set rotation to the move direction
		if (IsGrounded() == true)
		{
			
			transform.rotation = Quaternion.LookRotation(moveDirection);
			
		}	
		else
		{
			Vector3 xzMove= movement;
			xzMove.y = 0;
			if (xzMove.sqrMagnitude > 0.001f)
			{
				transform.rotation = Quaternion.LookRotation(xzMove);
			}
		}	
		
		// We are in jump mode but just became grounded
		if (IsGrounded() == true)
		{
			lastGroundedTime = Time.time;
			inAirVelocity = Vector3.zero;
			if (jumping == true)
			{
				jumping = false;
				SendMessage("DidLand", SendMessageOptions.DontRequireReceiver);
			}
		}
		RealignToTrack ();
	}
	
	void OnControllerColliderHit ( ControllerColliderHit hit   )
	{
		//	Debug.DrawRay(hit.point, hit.normal);
		if (hit.moveDirection.y > 0.01f) 
			return;
	}
	
	public float GetSpeed ()
	{
		return moveSpeed;
	}
	
	public bool IsJumping ()
	{
		return jumping;
	}
	
	public bool IsGrounded ()
	{
		return (collisionFlags & CollisionFlags.CollidedBelow) != 0;
	}
	
	public Vector3 GetDirection ()
	{
		return moveDirection;
	}
	
	public float GetLockCameraTimer ()
	{
		return lockCameraTimer;
	}
	
	public bool IsMoving ()
	{
		return Mathf.Abs(Input.GetAxisRaw("Vertical")) + Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.5f;
	}
	
	public bool HasJumpReachedApex ()
	{
		return jumpingReachedApex;
	}
	
	public bool IsGroundedWithTimeout ()
	{
		return lastGroundedTime + groundedTimeout > Time.time;
	}
	
	public void Reset ()
	{
		gameObject.tag = "Player";
	}

	public void RealignToTrack ()
	{
		Vector3 temp = transform.position;
		temp.z = 0;
		transform.position = temp;
	}

	
}