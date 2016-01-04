using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
	#region Public and Serialized Variables
	// UI and Audio resources
	public Slider healthSlider;                                 // Reference to the UI's health bar.
	public Image healthFill;  // assign in the editor the "Fill"
	public Slider staminaSlider;                                // Reference to the UI's health bar.
	public Image damageImage;                                   // Reference to an image to flash on the screen on being hurt.
	public Image staminaFlash;

	public Color healthStrobeColor1 = new Color(255f, 0, 0, 1f);
	public Color healthStrobeColor2 = new Color(110f, 0, 0, 1f);

	public Color flashColour = new Color(1f, 0f, 0f, 0.1f);     // The colour the damageImage is set to, to flash.
	public float flashSpeed = 5f;                               // The speed the damageImage will fade at.

	public AudioClip deathClip;                               	// The audio clip to play when the player dies.

	public IEnumerator stamCoroutine;
	public IEnumerator healthCoroutine;

	private PlayerMovement playerMovement;
	private MeleeCombat playerMeleeCombat;

	public Animator animator;                                   // Reference to the Animator component.
	public Animator flashAnim;
	public Animator shakeAnim;
	private AudioSource playerAudio;                            // Reference to the AudioSource component.

	// health and stamina
	public int maxHealth;                           			// The amount of health the player starts the game with.
	public int currentHealth;                                   // The current health the player has.
	public int hpRegen;											// Ammount of health regenrated every 2 seconds
	public int maxStamina;                           			// The amount of stamina the player starts the game with.
	public int currentStamina;                                  // The current stamina the player has.
	public int spRegen;											// Ammount of stamina regenrated every 2 seconds

	// evasion & mitigation Values
	public float blockValue;									// % of damage mititgated by the shield
	public float dodgeChance = 1f;								// Dodge chance
	private float regenStamina;
	private float regenHealth;

	// flags
	bool damaged = false;                                               // True when the player gets damaged.
	bool regenerating = true;
	bool healing = true;
	#endregion

	void Awake ()
	{
		// setting up the references
		animator = GetComponent <Animator>();
		playerAudio = GetComponent <AudioSource>();
		playerMovement = GetComponent <PlayerMovement>();
		playerMeleeCombat = GetComponent <MeleeCombat>();
		stamCoroutine = StaminaRegen ();
		healthCoroutine = HealthRegen ();

		// initalize the health and stamina bar values
		currentHealth = maxHealth;
		currentStamina = maxStamina;
		healthSlider.maxValue = maxHealth;
		healthSlider.value = currentHealth;
		staminaSlider.maxValue = maxStamina;
		staminaSlider.value = currentStamina;
	}

	void Start () {
		
		StartCoroutine(stamCoroutine);
		StartCoroutine(healthCoroutine);
	}

	void Update () {

		staminaSlider.value = currentStamina;
		healthSlider.value = currentHealth;

		if(damaged) { // damage screen vingette flash
			damageImage.color = flashColour;
		} else {
			damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
		}
			
		if (healthSlider.value <= (healthSlider.maxValue * 0.25f)) { // if health drops below 25% of maximum
			healthFill.color = Color.Lerp (healthStrobeColor1, healthStrobeColor2, Mathf.PingPong (2 * Time.time, 1));
		} else {
			
			if (healthFill.color != healthStrobeColor1) { // reset healthbar back to default color
				healthFill.color = healthStrobeColor1;
			}
		}
		 
		damaged = false;
	}
		
	public void StaminaCost (int amount) {

		StartCoroutine (StaminaRegenCoolDown (2));
		currentStamina -= amount;

		if (currentStamina <= 0) { // don't let stamina drop below 0
			currentStamina = 0;
			OutOfStaminaFlash ();
		}
	}

	public void OutOfStaminaFlash() {

		shakeAnim.SetTrigger("StamShake");
		flashAnim.SetTrigger("StamFlash");
	}
		
	public void TakeDamage (int amount) {

		float dmgAmount;

		// set the damaged flag so the screen will flash.
		damaged = true;

		// if blocking, take into account mitigation
		if (animator.GetBool("Blocking")) {
			if (currentStamina < 3) {
				OutOfStaminaFlash();
			} else {
				StaminaCost(3);
				dmgAmount = (float)amount;
				dmgAmount = (blockValue * dmgAmount);
				amount = (int) Mathf.Round(dmgAmount);
			} 
		}

		// if incoming damage is greater than 5% of maximum health ...
		if (amount >= (.15 * maxHealth)) {
			animator.SetTrigger("CriticalHit");
		} else {
			animator.SetTrigger("Hit");
		}
	
		currentHealth -= amount;
		//healthSlider.value = currentHealth;
		StartCoroutine (HealthRegenCoolDown (1));

		if(currentHealth <= 0 && !animator.GetCurrentAnimatorStateInfo(0).IsName("Death")) {
			Death ();
		}
	}
		
	private IEnumerator StaminaRegenCoolDown(float time) {

		regenerating = false;
		yield return new WaitForSeconds(time);
		regenerating = true;
	}

	private IEnumerator HealthRegenCoolDown(float time) {

		healing = false;
		yield return new WaitForSeconds(time);
		healing = true;
	}

	private IEnumerator HealthRegen() { // Health Regeneration - Loops indefinitly
		float hpTime = 1;
		float hpValue;

		while (true) { // loop forever ... 

			yield return new WaitForSeconds (2); // seconds between regen 'ticks'
			float hpElapsedTime = 0;

			if (currentHealth < maxHealth && animator.GetBool ("InCombat") && healing) { 

				regenHealth = Mathf.Clamp (currentHealth + (int)Mathf.Round (hpRegen / 3), 0, maxHealth);	
				hpValue = healthSlider.value;

				while (hpElapsedTime < hpTime && healing) {
					hpValue = Mathf.Lerp (hpValue, regenHealth, (hpElapsedTime / hpTime));
					currentHealth = (int)Mathf.Round (hpValue);
					hpElapsedTime += Time.deltaTime;

					yield return new WaitForEndOfFrame(); // loop and update the bar each frame
				}
			} else if (currentHealth < maxHealth && !animator.GetBool("InCombat") && healing) {
				
				regenHealth = Mathf.Clamp (currentHealth + hpRegen, 0, maxHealth);	
				hpValue = healthSlider.value;

				while (hpElapsedTime < hpTime && healing) {
					hpValue = Mathf.Lerp (hpValue, regenHealth, (hpElapsedTime / hpTime));
					currentHealth = (int)Mathf.Round (hpValue);
					hpElapsedTime += Time.deltaTime;

					yield return new WaitForEndOfFrame();
				}
			}
		}
	}

	private IEnumerator StaminaRegen() { // Stamina Regeneration - Loops indeffinitly
		float spTime = 1;
		float stamValue;
	
		while (true) { // loop forever ... 
			yield return new WaitForSeconds (2); // seconds between regen 'ticks'
			float spElapsedTime = 0;

			if (currentStamina < maxStamina && animator.GetBool ("InCombat") && regenerating) { // reduce regen by 1/3
				
				regenStamina = Mathf.Clamp (currentStamina + (int)Mathf.Round (spRegen / 3), 0, maxStamina);
				stamValue = staminaSlider.value;

				while (spElapsedTime < spTime && regenerating) {
					stamValue = Mathf.Lerp (stamValue, regenStamina, (spElapsedTime / spTime));
					currentStamina = (int)Mathf.Round (stamValue);
					spElapsedTime += Time.deltaTime;

					yield return new WaitForEndOfFrame();
				}
			} else if (currentStamina < maxStamina && !animator.GetBool ("InCombat") && regenerating) {

				regenStamina = Mathf.Clamp (currentStamina + spRegen, 0, maxStamina);
				stamValue = staminaSlider.value;

				while (spElapsedTime < spTime && regenerating) {
					stamValue = Mathf.Lerp (stamValue, regenStamina, (spElapsedTime / spTime));
					currentStamina = (int)Mathf.Round (stamValue);
					spElapsedTime += Time.deltaTime;

					yield return new WaitForEndOfFrame();
				}
			}
		}
	}

	void Death() {
		
		playerAudio.clip = deathClip;
		playerAudio.Play ();

		// trigger dath anim and flip bool
		animator.SetBool("isDead", true);
		animator.SetTrigger ("Die");
	
		// disable input scripts
		playerMovement.enabled = false;
		playerMeleeCombat.enabled = false;
	}

	// if dead, popup a window
	void OnGUI() {
		if (animator.GetCurrentAnimatorStateInfo(0).IsName("Death")) {
			GUI.Box(new Rect(Screen.width / 2f, Screen.height / 2f, 100, 20), "You have died!");
		}
	}
}