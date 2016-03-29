using UnityEngine;
using System.Collections;

public class PlayerStatistics : MonoBehaviour {
	
	public int currentHealth = 10;
	public int maxHealth = 10;
	public int currentStamina = 10;
	public int maxStamina = 10;

	public double stamRegen = 10;
	public double healthRegen = 10;

	public int strength;			// Melee Damage & Carrying Capacity
	public int endurance;			// Health & Stamina
	public int intelligence;		// Stamina & Spell Power
	public int dexterity;			// Physical Evasion
	public int agility;				// Chance to Hit
	public int wisdom;				// Spell Resistance
	
}
