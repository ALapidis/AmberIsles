using UnityEngine;
using System.Collections;

public class ItemClass : MonoBehaviour {

	// Inventory icons for items
	static public Texture2D swordIcon;
	static public Texture2D arrowIcon;
	static public Texture2D breadIcon;
	static public Texture2D shieldIcon;
	static public Texture2D potionIcon;
	static public Texture2D bowIcon;
	static public Texture2D waterIcon;
	static public Texture2D hatchetIcon;
	static public Texture2D pickaxeIcon;
	static public Texture2D glovesIcon;


	//Items
	public ItemCreatorClass swordItem = new ItemCreatorClass (0, "Longsword", swordIcon, "A standard one-handed blade.");
	public ItemCreatorClass arrowItem = new ItemCreatorClass (1, "Arrow", arrowIcon, "A single broadhead arrow.");
	public ItemCreatorClass breadItem = new ItemCreatorClass (2, "Bread", breadIcon, "A loaf of country bread.");
	public ItemCreatorClass shieldItem = new ItemCreatorClass (3, "Shield", shieldIcon, "A basic heater shield.");
	public ItemCreatorClass potionItem = new ItemCreatorClass (4, "Potion", potionIcon, "A health potion.");
	public ItemCreatorClass bowItem = new ItemCreatorClass (5, "Bow", bowIcon, "A flimsy bow.");
	public ItemCreatorClass waterItem = new ItemCreatorClass (6, "Water", waterIcon, "A dram of water.");
	public ItemCreatorClass hatchetItem = new ItemCreatorClass (7, "Hatchet", hatchetIcon, "A well worn hatchet.");
	public ItemCreatorClass pickaxeItem = new ItemCreatorClass (8, "Pickaxe", pickaxeIcon, "A rusted old pickaxe.");
	public ItemCreatorClass glovesItem = new ItemCreatorClass (9, "Gloves", glovesIcon, "A pair of tattered gloves.");


	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	// Base class for all game items
	public class ItemCreatorClass
	{
		public int id;
		public string name;
		public Texture2D icon;
		public string description;

		public ItemCreatorClass (int ide, string nam, Texture2D ico, string des)
		{
			id = ide;
			name = nam;
			icon = ico;
			description = des;
		}
	}

}
