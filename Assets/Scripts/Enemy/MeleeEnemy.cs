using UnityEngine;
using System.Collections;

public class MeleeEnemy : MonoBehaviour
{
	#region Public and Serialized Variables
	// Mob Combat Stats
	public float speed;
	public float range;
	public float agroRange;
	public int maxHealth;
	public int health;
	public int damage;
	public CharacterController controller;
	public Transform player;
	public AnimationClip run, idle, die, attackClip;
	public double impactTime = 0.36;
	private bool _impacted;
	private PlayerHealthbarController opponent;
	private FloatingHealthbar healthbar;
	public float sinkSpeed = 2.5f;              // The speed at which the enemy sinks through the floor when dead.
	bool isSinking;                             // Whether the enemy has started sinking through the floor.
	#endregion
	
	void Start()
	{
		healthbar = GetComponent<FloatingHealthbar>();
		health = maxHealth;
		player = GameObject.Find ("Player").transform;
		opponent = player.GetComponent<PlayerHealthbarController>();
	}

	void Update()
	{
		// If the enemy should be sinking...
		if(isSinking)
		{
			// ... move the enemy down by the sinkSpeed per second.
			transform.Translate (-Vector3.up * sinkSpeed * Time.deltaTime);
		}

		if (!isDead()) 
		{
			if (!inAgroRange())
			{
				GetComponent<Animation>().CrossFade (idle.name);
			} else {
				chase();
				player.GetComponent<PlayerInputController>().opponent = gameObject;

				if (inRange()) 
				{
					GetComponent<Animation>().Play (attackClip.name);
					attack();
					
					if (GetComponent<Animation>()[attackClip.name].time > 0.9 * GetComponent<Animation>()[attackClip.name].length)
					{
						_impacted = false;
					}
				}
			}
		} else {
			dieMethod();
		}
	}

	void attack()
	{
		if (GetComponent<Animation>()[attackClip.name].time > GetComponent<Animation>()[attackClip.name].length * impactTime 
		    && !_impacted
		    && GetComponent<Animation>()[attackClip.name].time < 0.9 * GetComponent<Animation>()[attackClip.name].length)
		{
			bool dodged = false;
			float chanceToHit;

			chanceToHit = Random.Range(0.0f, 100.00f);

			if (chanceToHit > opponent.dodgeChance && !dodged)
			{
				opponent.TakeDamage(damage);	
				_impacted = true;
				dodged = true;
				return;
			} else {
				opponent.animator.SetTrigger("Dodge");
				Debug.Log("You dodged!");
				_impacted = true;
				dodged = true;
				return;
			}
		}
	}

	bool inAgroRange()
	{
		return (Vector3.Distance (transform.position, player.position) < agroRange);
	}

	bool inRange()
	{
		return (Vector3.Distance (transform.position, player.position) < range);
	}
	
	// Face the player and chase after him
	void chase()
	{
		Vector3 targetPostition = new Vector3( player.position.x, this.transform.position.y, player.position.z ) ;

		this.transform.LookAt( targetPostition ) ;

		//controller.SimpleMove (transform.forward * speed);
		//GetComponent<Animation>().CrossFade (run.name);
	}

	// Recieve damage 
	public void getHit (int damage)
	{
		health = health - damage;
		if (health < 0) 
			health = 0;
	}

	// Play the death animation and destroy yourself
	void dieMethod()
	{
		GetComponent<Animation>().Play (die.name);
		if (GetComponent<Animation>()[die.name].time > GetComponent<Animation>()[die.name].length * 0.9)
		{
			//Destroy (gameObject);
			GetComponent<Animation>()["die"].speed = 0;
			StartCoroutine(Wait(2));
		}
	}

	IEnumerator Wait(float duration)
	{
		yield return new WaitForSeconds(duration);   //Wait
		StartSinking();
	}

	// Am I dead?
	bool isDead()
	{
		return (health <= 0);
	}

	public void StartSinking ()
	{
		// The enemy should no sink.
		isSinking = true;

		Destroy(healthbar.enemyHealthSlider.gameObject);

		// After 2 seconds destory the enemy.
		Destroy (gameObject, 2f);
	}

}
