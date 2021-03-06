﻿using UnityEngine; 
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;

/* 																											*/
/* This class handles the inputs from the Attack, Block and Evade buttons on the right side of the screen.	*/
/* 																											*/ 

public class PlayerInputController : MonoBehaviour
{		
	
	#region Fields

	private Animator animator;
	private PlayerHealthbarController playerHealth;
	private PlayerMovementController movementController;
	private WeaponManager wpnManager;
	public Image chargedRing;										// Charge ring image for charged attack counter

	public int rollCost = 10; 										// Stamina cost of rolling
	private float rollDodgeCooldownTimer = 0;
	private float rollDodgeCooldownTime = 0.75f;
	private bool rolldodging = false;


	public GameObject opponent = null;		           // Get the opponent variable from the MeleeEnemy.cs script if you're being chased
	public float fieldOfViewAngle = 110f;              // Number of degrees, centred on forward, for the enemy see.
	public float range; 					           // Weapons attack range
	public int damage = 40; 				           // Weapons damage per attack

	public float chargeRate = 1;
	private float chargeTime = 0;
	private float chargeAmmount = 0.7f;

	private bool _impacted;
	private bool chargeing = false;
	private float combatCoolDownTimer;
	private float combatCoolDownTime = 8;
	private bool heavyCombo;

	#endregion

	void Start () {
		// Initalize required components
		animator = GetComponent<Animator>();
		movementController = GetComponent<PlayerMovementController>();

		//initalize the needed scripts script
		wpnManager = GetComponent <WeaponManager>();
		playerHealth = GetComponent <PlayerHealthbarController>();

		chargedRing.fillAmount = 0f;
	}

	void Update () {

		// Softlock if in tripple the range of the weapon's attack AND you have a target AND you're in combat
		// This is only true for melee, some other system will be needed for magic and ranged
		SoftLock(3);

		// Charge'd Attack
		if (CrossPlatformInputManager.GetButton("Attack")) {
			chargeing = true;

			if (chargeing == true) {
				chargeTime = (chargeTime + (chargeRate * Time.deltaTime));
				if (chargeTime >= 0.1f) {
					chargedRing.fillAmount += Time.deltaTime / chargeAmmount; 
				}
			}
		}

		// Basic Attack Chain Logic
		if(CrossPlatformInputManager.GetButtonUp("Attack")) {

			SoftLock(1)	;			

			// If you have enough stamina
			if (playerHealth.Stamina >= 5) {

				if (chargeTime >= 0.8) {

					animator.SetTrigger("ChargedAttack");
				
				// If no potential attacks are playing, play the default first attack 
				} else if (chargeTime < 0.8					
					&& !animator.GetCurrentAnimatorStateInfo (0).IsName ("1H_Light_Combo1")
					&& !animator.GetCurrentAnimatorStateInfo (0).IsName ("1H_Light_Combo2")
					&& !animator.GetCurrentAnimatorStateInfo (0).IsName ("1H_Light_Combo3")
					&& !animator.GetCurrentAnimatorStateInfo (0).IsName ("1H_Medium_Combo2")
					&& !animator.GetCurrentAnimatorStateInfo (0).IsName ("1H_Medium_Combo3")) {

					animator.SetTrigger("Attack1");
					playerHealth.StaminaCost(5);

				} else if (chargeTime < 0.8 && animator.GetCurrentAnimatorStateInfo (0).IsName ("1H_Light_Combo1")) {

					animator.SetTrigger ("Attack2");
				} else if (chargeTime < 0.8 && animator.GetCurrentAnimatorStateInfo (0).IsName ("1H_Light_Combo2")) {

					animator.SetTrigger ("Attack3");
				} else if (chargeTime < 0.8 && animator.GetCurrentAnimatorStateInfo (0).IsName ("1H_Medium_Combo2")) {

					animator.SetTrigger ("Attack3");
				}

				CombatCooldownTimer();
				animator.SetBool("InCombat", true);
				wpnManager.WeaponSheath();
			} else {

				Debug.Log("Out of Stamina");
				//playerHealth.OutOfStaminaFlash();
			}

			// Reset charge info
			chargeTime = 0;
			chargedRing.fillAmount = 0f;
			chargeing = false;
		}

		if (CrossPlatformInputManager.GetButtonDown("Block")) {

			animator.SetBool("InCombat", true);
			animator.SetLayerWeight(1, 1.0f);
			animator.SetBool("Blocking", true);
			movementController.runSpeed = movementController.runSpeed / 2;
			wpnManager.WeaponSheath();
		}

		if (CrossPlatformInputManager.GetButtonUp("Block")) {

			animator.SetBool("Blocking", false);
			animator.SetLayerWeight(1, 0.0f);
			movementController.runSpeed = movementController.runSpeed * 2;
			CombatCooldownTimer();
			wpnManager.WeaponSheath();
		}

		// Combat cool down check
		if (Time.time >= combatCoolDownTimer) {

			animator.SetBool("InCombat", false);
			animator.SetTrigger("OutOfCombat");
			wpnManager.WeaponSheath();
		}

		// Rolldodge cool down check  *** IS THIS RIGHT? WHY NOT JUST USE A WAITFORSECONDS IN THE COROUTINE?
		if ((rollDodgeCooldownTimer != 0) && (Time.time >= rollDodgeCooldownTimer)) {

			rolldodging = false;
		}

		//if (CrossPlatformInputManager.GetButton("Evade")) 
		if (CrossPlatformInputManager.GetButton("Evade")) {
			
			// If moving the directional stick while holding Evade ...
			if ((animator.GetFloat("Speed") >= 0.1) && !rolldodging) {

				// ... and you have enough stamina
				if (playerHealth.Stamina >= rollCost) {

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
				} else {

					Debug.Log("Out of Stamina");
					//playerHealth.OutOfStaminaFlash();
				}

				RollDodgeCooldownTimer();
			}
		}
	}

	// *** IS THIS RIGHT? WHY NOT JUST USE A WAITFORSECONDS IN THE COROUTINE?
	void RollDodgeCooldownTimer() {

		rollDodgeCooldownTimer = Time.time + rollDodgeCooldownTime;
	}

	void AttackOver() {

		animator.SetBool("DisableTransitions", false);
		_impacted = false;
	}

	void TrailActivate() {

//		SimpleTrail.Activate ();
	}

	void TrailDeactivate() {

//		SimpleTrail.Deactivate ();
	}

	void HeavyComboTrue() {

		animator.SetBool("HeavyCombo", true);
	}

	// TODO: depricate this and use a standardize damage() function
	void impact() {

		//if we do have an opponent, and we're playing the attack animation, and we haven't yet hit the enemy... 
		if (opponent != null && !_impacted) {
			//send damage to the target
			opponent.GetComponent<MeleeEnemy>().getHit(damage); 
			_impacted = true;
		}
	}

	bool inRange() {
		
		return (Vector3.Distance (opponent.transform.position, transform.position) <= range);
	}

	void CombatCooldownTimer() {
		
		combatCoolDownTimer = Time.time + combatCoolDownTime;
	}

	// Autoface the target if you're in combat, have a target, and are within weapon range * dist
	void SoftLock(int dist) {

		if(opponent != null && animator.GetBool("InCombat")) { 

			Vector3 direction = opponent.transform.position - transform.position;
			float angle = Vector3.Angle(direction, transform.forward);

			if((angle < fieldOfViewAngle * 0.5f)) {

				RaycastHit hit;

				// ... and if a raycast towards the opponent hits something...
				if(Physics.Raycast(transform.position + transform.up, direction.normalized, out hit, (range * dist))) {

					transform.LookAt (opponent.transform.position);
				}
			}            
		}
	}
}
