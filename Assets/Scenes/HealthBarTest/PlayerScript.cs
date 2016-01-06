using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
	#region Fields

	public float speed; 						// The players movement speed
	private int currentHealth; 					// The players current health
	public int maxHealth; 						// The players max health
	public float healthLerpSpeed; 				// The time to multiply by Time.deltaTime to accelerate the lerp
	public int healthRegen;						// How much health is recovered per cycle
	public float healthRegenSpeed;				// How long between health recovery cycles 

	private int currentStamina; 				// The players current stamina
	public int maxStamina; 						// The players max stamina
	public float staminaLerpSpeed; 				// The time to multiply by Time.deltaTime to accelerate the lerp
	public int staminaRegen;					// How much stamina is recovered per cycle
	public float staminaRegenSpeed;				// How long between stamina recovery cycles 

	public RectTransform healthTransform; 		// The health's transform, this is used for moving the object
	public Text healthText; 					// The health text
	public Text healthMaxText; 					// The health max text
	public Image visualHealth; 					// The health's image, this is used for color changing

	public RectTransform staminaTransform; 		// The stamina's transform, this is used for moving the object
	public Text staminaText; 					// The stamina text
	public Text staminaMaxText; 				// The stamina max text
	public Image visualStamina; 				// The stamina's image, this is used for color changing

	public Canvas canvas; 						// The Canvas, this is used for it's scale mode

	private Animator animator; 					// Reference to the Animator component.

	public Image damageImage;					// Reference to the texture used for the damage flash
	public Color flashColour;  					// The colour the damageImage is set to, to flash
	public float flashSpeed = 5f;               // The speed the damageImage will fade at

	private float currentHPValue; 				// Current value of the health in 0 to 1
	private float currentSPValue; 				// Current value of the stamina in 0 to 1
	private bool damaged;

	// For purposes of demo scene
	public float coolDown;						// The time in seconds to cooldown between taking damage from the damage trigger
	private bool onCD;							// Flag to disable damage/enable from the damage trigger

	// To be moved as properties of a player class (singleton?)
	public float blockValue;					// The percent of damage mititgated by the block
	public float dodgeChance = 1f;				// Dodge chance

	#endregion

	#region Properties

	public int Health {
		get { return currentHealth; }
		set { currentHealth = value;
		}
	}

	public int Stamina {
		get { return currentStamina; }
		set { currentStamina = value;
		}
	}

	#endregion

	void Awake () {

		animator = GetComponent <Animator>();
	}


	// Use this for initialization
	void Start () {

		// Initalize current health and regeneration coroutines
		currentHealth = maxHealth; 
		currentStamina = maxStamina;
		StartCoroutine(HealthRegen(healthRegen, healthRegenSpeed));
		StartCoroutine(StaminaRegen(staminaRegen, staminaRegenSpeed));
	}
	
	// Update is called once per frame
	void Update () {

		// Handle player health and movement
		HandleHealthbar ();
		HandleMovement ();

		// Debug damage to player for testing healthbar
		if (Input.GetKeyDown (KeyCode.Space)) {

			damaged = true;
			Health -= 10;

		}

		if (Input.GetKeyDown (KeyCode.LeftShift)) {

			Stamina -= 5;
		}

		if (damaged) { // damage screen vingette flash
			damageImage.color = flashColour;
		} else {
			damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
		}

		damaged = false;
	}
		
	IEnumerator HealthRegen(int amount, float time){

		while (true)
		{
			yield return new WaitForSeconds(time);
			Health += amount;
		}
	}

	IEnumerator StaminaRegen(int amount, float time){

		while (true)
		{
			yield return new WaitForSeconds(time);
			Stamina += amount;
		}
	}


	// Handles the health and stamina meters on the health bar UI
	private void HandleHealthbar () {

		// Ensure the health dosn't go over max, or below min
		if (currentHealth > maxHealth) {
			currentHealth = maxHealth;
		} else if (currentHealth < 0) {
			currentHealth = 0;
		}

		// Ensure the stamina dosn't go over max, or below min
		if (currentStamina > maxStamina) {
			currentStamina = maxStamina;
		} else if (currentStamina < 0) {
			currentStamina = 0;
		}
			
		// Writes the current health/stamina in the text field
		healthText.text = "" + currentHealth;
		healthMaxText.text = "" + maxHealth;
		staminaText.text = "" + currentStamina;
		staminaMaxText.text = "" + maxStamina;

		// Maps the min and max position to the range between 0 and max health/stamina
		currentHPValue = Map (currentHealth, 0, maxHealth, 0, 1);
		currentSPValue = Map (currentStamina, 0, maxStamina, 0, 1);

		// Sets the position of the health/stamina to simulate the reductuon of health
		visualHealth.fillAmount = Mathf.Lerp(visualHealth.fillAmount, currentHPValue, Time.deltaTime * healthLerpSpeed);
		visualStamina.fillAmount = Mathf.Lerp(visualStamina.fillAmount, currentSPValue, Time.deltaTime * staminaLerpSpeed);

		// If health drops below 25% of maximum, strobe the alpha
		if (currentHealth < maxHealth / 4) { 
			visualHealth.color = new Color (255.0f, 0.0f, 0.0f, Mathf.Lerp (1.0f, 0.25f, Mathf.PingPong (2 * Time.time, 1)));
		} else if (visualHealth.color.a != 1f) {
			// Reset healthbar back to default alpha intensity
			visualHealth.color = new Color (255.0f, 0.0f, 0.0f, 1.0f);
		}
	}

	// Handles player movement
	private void HandleMovement () {

		float translation = speed * Time.deltaTime;

		transform.Translate (new Vector3 (Input.GetAxis ("Horizontal") * translation, 0, Input.GetAxis ("Vertical") * translation));
	}

	// Handle trigger events
	void OnTriggerStay (Collider other) {

		if (other.name == "Damage") {
			if (!onCD && currentHealth > 0) {
				StartCoroutine (CoolDownDmg ());
				Health -= 1;
			}
		}

		if (other.name == "Health") {
			if (!onCD && currentHealth < maxHealth) {
				StartCoroutine (CoolDownDmg ());
				Health += 1;
			}
		}
	}

	// Damage cooldown timer
	IEnumerator CoolDownDmg () {

		// Set the cooldown timer flag to true to disable damage
		onCD = true;
		yield return new WaitForSeconds (coolDown);
		onCD = false;
	}

	// Formula to map the input values to a range of 0 to 1
	private float Map (float x, float inMin, float inMax, float outMin, float outMax) {
		return (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
	}

	// To be moved to the player rather than in this script
	public void TakeDamage (int amount) {

		float dmgAmount;

		// set the damaged flag so the screen will flash.
		damaged = true;

		// if blocking, take into account mitigation
		if (animator.GetBool("Blocking")) {
			if (currentStamina < 3) {
				Debug.Log("Out of Stamina");
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

		Health -= amount;

		if(currentHealth <= 0 && !animator.GetCurrentAnimatorStateInfo(0).IsName("Death")) {
			Death ();
		}
	}

	// To be moved into a combat manager script
	public void StaminaCost (int amount) {

		currentStamina -= amount;

		if (currentStamina <= 0) { // don't let stamina drop below 0
			currentStamina = 0;
		}
	}

	// To be moved to the player rather than in this script
	void Death() {

		// trigger dath anim and flip bool
		animator.SetBool("isDead", true);
		animator.SetTrigger ("Die");

		// disable input scripts
		//playerMovement.enabled = false;
		//playerMeleeCombat.enabled = false;
	}

	// if dead, popup a window
	void OnGUI() {
		if (animator.GetCurrentAnimatorStateInfo(0).IsName("Death")) {
			GUI.Box(new Rect(Screen.width / 2f, Screen.height / 2f, 100, 20), "You have died!");
		}
	}

}
