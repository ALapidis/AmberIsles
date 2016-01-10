using UnityEngine; 
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

/* 																											*/
/* This class handles the input from the virtual joystick on the left side of the screen for movement only. */
/* 																											*/ 

public class PlayerMovementController : MonoBehaviour
{		
	#region Fields

	private Animator animator;
	private CharacterController controller;

	public float runSpeed = 8.5f; 				// Current velocity
	public float rotationSpeed = 50f;			// How fast the player rotates to a new vector
	public int gravity = 20; 					// Gravity of the world 

	private Vector3 currentVector;
	[HideInInspector]
	public Vector3 inputVec;
	private Vector3 targetDirection;

	#endregion

	void Start () {
		
		controller = GetComponent<CharacterController>();
		animator = GetComponent<Animator>();
	}

	void Update () {

		// Get input vector from joystick controls
		//inputVec = new Vector3(CrossPlatformInputManager.GetAxisRaw("Horizontal"), 0, CrossPlatformInputManager.GetAxisRaw("Vertical"));
		inputVec = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

		if (inputVec != Vector3.zero) {

			UpdateMovement(); 				
		} else {

			animator.SetFloat("Speed", 0f);
		}
	}

	void UpdateMovement() {

		Vector3 motion = inputVec;

		// Reduce input for diagonal movement
		motion *= (Mathf.Abs(inputVec.x) == 1 && Mathf.Abs(inputVec.z) == 1) ? 0.7f : 1;

		// Multiply the directional vector by the players run speed to establish velocity.
		motion *= runSpeed;

		// Do I need these? Are they used to normalize the value for magniture int he next block for passing into the speed variable? I can't recall ...
		//inputVec = new Vector3 (Mathf.Abs(CrossPlatformInputManager.GetAxisRaw("Horizontal")), 0, Mathf.Abs(CrossPlatformInputManager.GetAxisRaw("Vertical")));
		//inputVec = new Vector3 (Mathf.Abs(Input.GetAxisRaw("Horizontal")), 0, Mathf.Abs(Input.GetAxisRaw("Vertical")));

		// If the character is holding block, reduce his movement speed by 1/3
		if (animator.GetBool("Blocking")) {

			//speed = inputVec.magnitude / 3;
			animator.SetFloat("Speed", inputVec.sqrMagnitude / 3);
		} else {

			//speed = inputVec.magnitude;
			animator.SetFloat("Speed", inputVec.sqrMagnitude);
		}

		// Move the character controller taking into account gravity to keep the player on the ground once per second rather than per frame
		controller.Move ((motion + Vector3.up * -gravity) * Time.deltaTime);

		RotateTowardMovementDirection();  
		GetCameraRelativeMovement();  
	}

	void GetCameraRelativeMovement() {  

		Transform cameraTransform = Camera.main.transform;

		// Forward vector relative to the camera along the x-z plane   
		Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
		forward.y = 0;
		forward = forward.normalized;

		// Always orthogonal to the forward vector, Right vector relative to the camera
		Vector3 right= new Vector3(forward.z, 0, -forward.x);

		// Directional inputs
		//float v = CrossPlatformInputManager.GetAxisRaw("Vertical");
		//float h = CrossPlatformInputManager.GetAxisRaw("Horizontal");
		float v = Input.GetAxisRaw("Vertical");
		float h = Input.GetAxisRaw("Horizontal");

		// Target direction relative to the camera
		targetDirection = h * right + v * forward;
	}

	void RotateTowardMovementDirection() {

		if (inputVec != Vector3.zero) {
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDirection), Time.deltaTime * rotationSpeed);
		}
	}
}
