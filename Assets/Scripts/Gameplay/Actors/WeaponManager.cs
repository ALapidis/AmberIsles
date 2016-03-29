using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour {

	#region Public and Serialized Variables
	public Animator animator;

	public GameObject rHand;
	public GameObject lHand;
	public GameObject lShield;
	public GameObject bHolster;
	public GameObject lHipHolster;
	public GameObject rHipHolster;

	public GameObject[] hands = new GameObject[3];
	public GameObject[] holsters = new GameObject[3];
	#endregion


	// Use this for initialization
	void Start () 
	{	
		GameObject weapon;
		GameObject item;

		// Populate the hands array
		hands[0] = rHand;
		hands[1] = lHand;
		hands[2] = lShield;

		// Populate the holsters array
		holsters[0] = bHolster;
		holsters[1] = lHipHolster;
		holsters[2] = rHipHolster;

		// Equip the player manually till we read this data from stored data or something...
		item = Resources.Load("items/Swords/Longsword/LongSword01") as GameObject;
		weapon = Instantiate(item, rHand.transform.position, rHand.transform.rotation) as GameObject; 
		weapon.transform.parent = rHand.transform;

		item = Resources.Load("items/Shields/Shield01") as GameObject;
		weapon = Instantiate(item, lShield.transform.position, lShield.transform.rotation) as GameObject;
		weapon.transform.parent = lShield.transform;

		// Sheath our weapons from the start
		WeaponSheath();
	}

		
	public void WeaponSheath()
	{
		Transform childTrans;
		GameObject swap;

		if (!animator.GetBool("InCombat")) // If not dead, and NOT in combat...
		{
			foreach(GameObject item in hands)
			{
				if (item.transform.childCount > 0)
				{
					// Grab the transform of the first child and then grab that transforms GameObject
					childTrans = item.transform.GetChild(0);
					swap = childTrans.transform.gameObject;

					Sheath(swap);						
				}
			}
		}

		if (animator.GetBool("InCombat")) // If not dead, and in-combat...
		{
			foreach(GameObject item in holsters)
			{
				if (item.transform.childCount > 0)
				{
					childTrans = item.transform.GetChild(0);		
					swap = childTrans.transform.gameObject;			

					Sheath(swap);									
				}			
			}
		}
	}
		

	// Moves the weapon/shield to it's proper holster or hands depending on tag.
	void Sheath( GameObject swap)
	{
		GameObject parent;
		parent = swap.transform.parent.gameObject;

		if ((swap.tag == "1H_Weapon") || (swap.tag == "1H_Wand")) // If one handed ... 
		{
			switch (parent.name)
			{
			case "RightHand":
				swap.transform.parent = lHipHolster.transform;
				swap.transform.position = lHipHolster.transform.position;
				swap.transform.rotation = lHipHolster.transform.rotation;
				break;
			
			case "LeftHand":
				swap.transform.parent = rHipHolster.transform;
				swap.transform.position = rHipHolster.transform.position;
				swap.transform.rotation = rHipHolster.transform.rotation;
				break;

			case "LeftHipHolster":
				swap.transform.parent = rHand.transform;
				swap.transform.position = rHand.transform.position;
				swap.transform.rotation = rHand.transform.rotation;
				break;

			case "RightHipHolster":
				swap.transform.parent = lHand.transform;
				swap.transform.position = lHand.transform.position;
				swap.transform.rotation = lHand.transform.rotation;
				break;
			}
		}

		if ((swap.tag == "2H_Weapon") || (swap.tag == "Staff")) // If two-handed ...
		{
			switch (parent.name)
			{
			case "RightHand":
				swap.transform.parent = bHolster.transform;
				swap.transform.position = bHolster.transform.position;
				swap.transform.rotation = bHolster.transform.rotation;
				break;
			
			case "BackHolster":
				swap.transform.parent = rHand.transform;
				swap.transform.position = rHand.transform.position;
				swap.transform.rotation = rHand.transform.rotation;
				break;
			}
		}

		if (swap.tag == "Shield") // If shield ...
		{
			switch (parent.name)
			{
			case "LeftShield":
				swap.transform.parent = bHolster.transform;
				swap.transform.position = bHolster.transform.position;
				swap.transform.rotation = bHolster.transform.rotation;
				break;
			
			case "BackHolster":
				swap.transform.parent = lShield.transform;
				swap.transform.position = lShield.transform.position;
				swap.transform.rotation = lShield.transform.rotation;
				break;
			}
		}
	}
}
