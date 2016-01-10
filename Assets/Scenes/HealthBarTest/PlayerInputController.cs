using UnityEngine; 
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

/* 																											*/
/* This class handles the inputs from the Attack, Block and Evade buttons on the right side of the screen.	*/
/* 																											*/ 

public class PlayerInputController : MonoBehaviour
{		
	
	#region Fields

	public Animator animator;
	private CharacterController controller;
	private PlayerHealth playerHealth;
	private PlayerMovementController movementController;

	public int rollCost = 10; 										// Stamina cost of rolling
	private float rollDodgeCooldownTimer = 0;
	private float rollDodgeCooldownTime = 0.75f;
	private bool rolldodging = false;

	#endregion

	void Start () {

		// Initalize required components
		controller = GetComponent<CharacterController>();
		animator = GetComponent<Animator>();
		movementController = GetComponent<PlayerMovementController>();
	}

	void Update () {

		// Rolldodge cool down check  *** IS THIS RIGHT? WHY NOT JUST USE A WAITFORSECONDS IN THE COROUTINE?
		if ((rollDodgeCooldownTimer != 0) && (Time.time >= rollDodgeCooldownTimer))
		{
			rolldodging = false;
		}

		//if (CrossPlatformInputManager.GetButton("Evade")) 
		if (Input.GetButton("Evade")) 
		{
			if ((animator.GetFloat("Speed") >= 0.1) && !rolldodging) // If moving the directional stick while holding Evade ...
			{

				if (playerHealth.currentStamina >= rollCost) // ... and you have enough stamina
				{
					Vector3 forward = transform.forward;
					float angle = Vector3.Dot(movementController.inputVec, forward);
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
	}

	// *** IS THIS RIGHT? WHY NOT JUST USE A WAITFORSECONDS IN THE COROUTINE?
	void RollDodgeCooldownTimer()
	{
		rollDodgeCooldownTimer = Time.time + rollDodgeCooldownTime;
	}

}
