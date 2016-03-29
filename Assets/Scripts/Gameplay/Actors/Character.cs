using UnityEngine;

public enum CharacterType {NPC, Vendor, Enemy, Player}

public abstract class Character : MonoBehaviour {


	public CharacterType characterType;

	public int baseHealth = 10;
	public int baseStamina = 10;

	public int baseStrength = 10;
	public int baseEndurance = 10;
	public int baseDexterity = 10;
	public int baseIntellect = 10;
	public int baseWillpower = 10;
	public int baseCharisma = 10;
	public int baseLuck = 10;

	public float moveSpeed = 10f;

	void OnEnable(){
		CharacterManager.Register(this);
	}

	void OnDisable(){
		CharacterManager.Unregister(this);
	}

}