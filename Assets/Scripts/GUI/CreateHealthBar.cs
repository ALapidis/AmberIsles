using UnityEngine;
using System.Collections;

public class CreateHealthBar : MonoBehaviour {

	//
	// Passes the gameObject to the script is attached to for the CreateHealthBar function in  UI_Manager.cs to trigger an instance of the healthbar UI object 
	//

	private MeleeEnemy targetMob;
	public GameObject mob;
	private float value;
	private float maxValue;
	private bool hurt = false;


	void Start () 
	{
		// Get current and max health from parent game object script.
		mob = transform.parent.gameObject;
		targetMob = mob.GetComponent<MeleeEnemy>();
	}

	void LateUpdate ()
	{
		// Check if the current health is less than the max and if mob is unhurt.
		if (targetMob.health < targetMob.maxHealth && hurt == false) 
		{
			// Mob is hurt.
			hurt = true;
			// Instanciate the healthbar.
			UI_Manager.Instance.CreateHealthBar(gameObject);
		}


	}
}
