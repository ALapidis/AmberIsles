using UnityEngine; 
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerMovement : MonoBehaviour
{		
	#region Public and Serialized Variables
	public Animator animator;
	private CharacterController controller;
	private PlayerHealth playerHealth;

	private float speed;						// Current movement speed based on the magnitude of the inputVector from the joystick
	public float runSpeed = 8.5f; 				// Current velocity

	public float rotationSpeed = 50f;			// How fast the player rotates to a new vector
	public int gravity = 20; 					// Gravity of the world 
	public float rollSpeed = 0.45f; 			// Speed at which you roll
	public float rollDistance = 5f; 			// Distance you roll
	public int rollCost = 10; 					// Stamina cost of rolling

	private float rollDodgeCooldownTimer = 0;
	private float rollDodgeCooldownTime = 0.75f;

	bool rolldodging = false;

	Vector3 currentVector;
	Vector3 inputVec;
	Vector3 targetDirection;
	#endregion

	void Start ()
	{
		controller = (CharacterController)GetComponent (typeof(CharacterController));
		playerHealth = GetComponent <PlayerHealth>();
	}
		
	void Update ()
	{
		// Get input vector from joystick controls
		inputVec = new Vector3(CrossPlatformInputManager.GetAxisRaw("Horizontal"), 0, CrossPlatformInputManager.GetAxisRaw("Vertical"));

		// rolldodge cool down check
		if ((rollDodgeCooldownTimer != 0) && (Time.time >= rollDodgeCooldownTimer))
		{
			rolldodging = false;
		}

		if (CrossPlatformInputManager.GetButton("Evade")) 
		{
			if ((animator.GetFloat("Speed") >= 0.1) && !rolldodging) // If moving the directional stick while holding Evade ...
			{
				
				if (playerHealth.currentStamina >= rollCost) // ... and you have enough stamina
				{
					Vector3 forward = transform.forward;
					float angle = Vector3.Dot(inputVec, forward);
					rolldodging = true;

					if(angle > 0.5f) {
						animator.SetTrigger("RollForward");
						playerHealth.StaminaCost(10);
					} else if(angle < 0.5f && angle < -0.5) {
						animator.SetTrigger("RollBack");
						playerHealth.StaminaCost(10);
					} else if(angle < 0.5f && angle > 0) {
						animator.SetTrigger("RollLeft");
						playerHealth.StaminaCost(10);
					} else if(angle > -0.5f && angle < 0) {
						animator.SetTrigger("RollRight");
						playerHealth.StaminaCost(10);
					}
				} 
				else 
				{
					playerHealth.OutOfStaminaFlash();
				}

				RollDodgeCooldownTimer();
			}
		}

		if (inputVec != Vector3.zero)
		{
			UpdateMovement(); 				
		}
		else
		{
			animator.SetFloat("Speed", 0f);
		}
	}
		
	void UpdateMovement()
	{
		Vector3 motion = inputVec;

		// reduce input for diagonal movement
		motion *= (Mathf.Abs(inputVec.x) == 1 && Mathf.Abs(inputVec.z) == 1) ? 0.7f : 1;

		// multiply the directional vector by the players run speed to establish velocity.
		motion *= runSpeed;

		inputVec = new Vector3 (Mathf.Abs(CrossPlatformInputManager.GetAxisRaw("Horizontal")), 0, Mathf.Abs(CrossPlatformInputManager.GetAxisRaw("Vertical")));
			
		if (animator.GetBool("Blocking"))
		{
			speed = inputVec.magnitude / 3;
			animator.SetFloat("Speed", speed);
		}
		else 
		{
			speed = inputVec.magnitude;
			animator.SetFloat("Speed", speed);
		}

		// move the character controller taking into account gravity to keep the player on the ground once per second rather than per frame
		controller.Move ((motion + Vector3.up * -gravity) * Time.deltaTime);
		
		RotateTowardMovementDirection();  
		GetCameraRelativeMovement();  
	}
		
	void GetCameraRelativeMovement()
	{  
		Transform cameraTransform = Camera.main.transform;

		// forward vector relative to the camera along the x-z plane   
		Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
		forward.y = 0;
		forward = forward.normalized;

		// right vector relative to the camera
		// always orthogonal to the forward vector
		Vector3 right= new Vector3(forward.z, 0, -forward.x);

		//directional inputs
		float v = CrossPlatformInputManager.GetAxisRaw("Vertical");
		float h = CrossPlatformInputManager.GetAxisRaw("Horizontal");

		// Target direction relative to the camera
		targetDirection = h * right + v * forward;
	}
		
	void RotateTowardMovementDirection()  
	{
		if (inputVec != Vector3.zero)
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDirection), Time.deltaTime * rotationSpeed);
		}
	}
		
	void RollDodgeCooldownTimer()
	{
		rollDodgeCooldownTimer = Time.time + rollDodgeCooldownTime;
	}

}
