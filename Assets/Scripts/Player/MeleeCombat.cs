using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using Xft;

public class MeleeCombat : MonoBehaviour
{
	#region Public and Serialized Variables
	public GameObject opponent = null;		           // Get the opponent variable from the MeleeEnemy.cs script if you're being chased
	public Animator animator;
	public Image chargedRing;

	public XWeaponTrail SimpleTrail;

    public float fieldOfViewAngle = 110f;              // Number of degrees, centred on forward, for the enemy see.
	public float range; 					           // Weapons attack range
	public int damage = 40; 				           // Weapons damage per attack
	public double impactTime; 				           // The time in the attack animation that the player actually makes contact with the enemy
	private bool _impacted;

	private WeaponManager wpnManager;
	private PlayerHealth playerHealth;
	private PlayerMovement playerMovement;

	private float chargeTime = 0;
	private float chargeAmmount = 0.7f;
	public float chargeRate = 1;
	private bool chargeing = false;
	private float combatCoolDownTimer;
	private float combatCoolDownTime = 8;
	public bool heavyCombo;
	#endregion


/* 		Ability Cooldown Psudo Code */
//		if (mouse click & Character.Cooldown = False)
//		Character.cTimer = 0
//		Character.cEndTime = txtEndTimer.text
//		Character.Cooldown = True
//		CooldownBar.Width = 0
//		CooldownBar.AnimationFrame = 1
//
//		If Character.Cooldown = True
//		Character.cTimer = Character.cTimer + dt
//		CooldownBar.Width = (CooldownBar.MaxWidth / Character.cEndTime) * Character.cTimer
//
//		If Character.cTimer >= Character.cEndTime
//		Character.cTimer = -1
//		Character.Cooldown = False
//		CooldownBar.Width = CooldownBar.MaxWidth
//		CooldownBar.AnimationFrame = 0


	void Start() {

		//initalize the needed scripts script
		wpnManager = GetComponent <WeaponManager>();
		playerHealth = GetComponent <PlayerHealth>();
		playerMovement = GetComponent <PlayerMovement>();

		SimpleTrail.Init();
		//SimpleTrail.Deactivate ();

		chargedRing.fillAmount = 0f;
	}
				


	void Update() {	

		if (CrossPlatformInputManager.GetButton("Attack")) {
			chargeing = true;

			if (chargeing == true) {
				chargeTime = (chargeTime + (chargeRate * Time.deltaTime));
				if (chargeTime >= 0.1f) {
					chargedRing.fillAmount += Time.deltaTime / chargeAmmount; 
				}
			}
 		}
			
		if(CrossPlatformInputManager.GetButtonUp("Attack")) {

			chargedRing.fillAmount = 0f;

			if(opponent != null) { // player auto-LookAt if in range
				Vector3 direction = opponent.transform.position - transform.position;
				float angle = Vector3.Angle(direction, transform.forward);

				if((angle < fieldOfViewAngle * 0.5f)) {
					RaycastHit hit;

					// ... and if a raycast towards the opponent hits something...
					if(Physics.Raycast(transform.position + transform.up, direction.normalized, out hit, range)) {
						transform.LookAt (opponent.transform.position);
					}
				}            
			}

			if (playerHealth.currentStamina >= 5) {

				if (chargeTime >= 0.8) {
					animator.SetTrigger("ChargedAttack");
					playerHealth.StaminaCost(10);
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
					playerHealth.StaminaCost(5);
				} else if (chargeTime < 0.8 && animator.GetCurrentAnimatorStateInfo (0).IsName ("1H_Light_Combo2")) {
					animator.SetTrigger ("Attack3");
					playerHealth.StaminaCost(5);
				} else if (chargeTime < 0.8 && animator.GetCurrentAnimatorStateInfo (0).IsName ("1H_Medium_Combo2")) {
					animator.SetTrigger ("Attack3");
					playerHealth.StaminaCost(5);
				}

				CombatCooldownTimer();
				animator.SetBool("InCombat", true);
				wpnManager.WeaponSheath();
			} else {
				playerHealth.OutOfStaminaFlash();
			}

			// reset charge info
			chargeTime = 0;
			chargeing = false;
		}

		if (CrossPlatformInputManager.GetButtonDown("Block")) {
			animator.SetBool("InCombat", true);
			animator.SetLayerWeight(1, 1.0f);
			animator.SetBool("Blocking", true);
			playerMovement.runSpeed = playerMovement.runSpeed / 2;
			wpnManager.WeaponSheath();
		}

		if (CrossPlatformInputManager.GetButtonUp("Block")) {
			animator.SetBool("Blocking", false);
			animator.SetLayerWeight(1, 0.0f);
			playerMovement.runSpeed = playerMovement.runSpeed * 2;
			CombatCooldownTimer();
			wpnManager.WeaponSheath();
		}
						
		// combat cool down check
		if (Time.time >= combatCoolDownTimer) {
			animator.SetBool("InCombat", false);
			animator.SetTrigger("OutOfCombat");
			wpnManager.WeaponSheath();
		}
	}
		
	void AttackOver() {
		animator.SetBool("DisableTransitions", false);
		_impacted = false;
	}

	void TrailActivate(){
		SimpleTrail.Activate ();
	}

	void TrailDeactivate(){
		SimpleTrail.Deactivate ();
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

}
