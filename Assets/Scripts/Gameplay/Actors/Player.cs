using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : Character
{

    private static Player instance;

    //The player's movement speed
    //public float speed;

    //A reference to the inventory
    public Inventory inventory;

	//A reference to the character panel
    public Inventory charPanel;
    
    //A reference to the chest
    private Inventory chest;

    //Shows the player what he needs to do
    public Text helperText;

	//Displays the players stats
    public Text statsText;


    [SerializeField]
    private Text goldText;

    //public ItemScript[] items = new ItemScript[10];

//	public int baseHealth = 400;
//	public int baseStamina = 200;
//
//    public int baseStrength = 10;
//    public int baseEndurance = 10;
//	public int baseDexterity = 10;
//	public int baseIntellect = 10;
//	public int baseWillpower = 10;
//	public int baseCharisma = 10;
//	public int baseLuck = 10;

	private int strength;
	private int endurance;
	private int dexterity;
	private int intellect;
	private int willpower;
	private int charisma;
	private int luck;
    
    private int gold;
	private int health;

	public int Strength {
		get { return strength; }
		set { strength = value; }
	}
		
	public int Endurance {
		get { return endurance; }
		set { endurance = value; }
	}

	public int Dexterity {
		get { return dexterity; }
		set { dexterity = value; }
	}

	public int Intellect {
		get { return intellect; }
		set { intellect = value; }
	}

	public int Willpower {
		get { return willpower; }
		set { willpower = value; }
	}

	public int Charisma {
		get { return charisma; }
		set { charisma = value; }
	}

	public int Luck {
		get { return luck; }
		set { luck = value; }
	}


    public int Gold {
        get{ return gold; }

        set {
            goldText.text = "Gold: " + value;
            gold = value;
        }
    }

	//Initalize Singleton
    public static Player Instance {
        get {
            
			if (instance == null) {
                instance = GameObject.FindObjectOfType<Player>();
            }
            return instance;
        }
    }
		
    // Use this for initialization
    void Awake() {
       
		Gold = 0;
        SetStats(0, 0, 0, 0, 0, 0, 0);
    }

    // Update is called once per frame
    void Update() {
       
		        
		if (Input.GetKeyDown(KeyCode.E)) {
            
			if (chest != null) {
                
				if (chest.canvasGroup.alpha == 0 ||chest.canvasGroup.alpha == 1) {
                    chest.Open();
                }
            }
        }
    }
		
		
    //Handles the player's collision
    private void OnTriggerEnter(Collider other) {

		if (other.tag == "Item") { //If we collide with an item that we can pick up
            ///Picks a random category, consumeable, equipment, weapons
            int randomType = UnityEngine.Random.Range(0, 3);

            //instantiates for adding to the inventory
            GameObject tmp = Instantiate(InventoryManager.Instance.itemObject);

            //Adds the item script to the object
            tmp.AddComponent<ItemScript>();

            //variable for selecting an item inside the category
            int randomItem;

            tmp.AddComponent<ItemScript>();

            ItemScript newEquipment = tmp.GetComponent<ItemScript>();

            switch (randomType) //Selects an item
            {
                case 0:
                    //Find selects an item
                    randomItem = UnityEngine.Random.Range(0, InventoryManager.Instance.ItemContainer.Consumeables.Count);

                    //Ginds the item in the list
                    newEquipment.Item = InventoryManager.Instance.ItemContainer.Consumeables[randomItem];
                    break;

                case 1:
                    randomItem = UnityEngine.Random.Range(0, InventoryManager.Instance.ItemContainer.Weapons.Count);
                    newEquipment.Item = InventoryManager.Instance.ItemContainer.Weapons[randomItem];
                    break;
                case 2:

                    randomItem = UnityEngine.Random.Range(0, InventoryManager.Instance.ItemContainer.Equipment.Count);
                    newEquipment.Item = InventoryManager.Instance.ItemContainer.Equipment[randomItem];

                    break;
            }

            inventory.AddItem(newEquipment);
            Destroy(tmp);
	
        }

		if (other.tag == "Chest" || other.tag == "Vendor") { //If we enter a chest we need to be able to open it
            helperText.gameObject.SetActive(true);
            chest = other.GetComponent<InventoryLink>().linkedInventory;
        }
        
		if (other.tag == "CraftingBench") {
            helperText.gameObject.SetActive(true);
            chest = other.GetComponent<CraftingBenchScript>().craftingBench;
        }

		if (other.tag == "Material") { //Creates some test materials
            
			for (int i = 0; i < 5; i++) {
                
				for (int x = 0; x < 3; x++) {
                    GameObject tmp = Instantiate(InventoryManager.Instance.itemObject);

                    tmp.AddComponent<ItemScript>();

                    ItemScript newMaterial = tmp.GetComponent<ItemScript>();

                    newMaterial.Item = InventoryManager.Instance.ItemContainer.Materials[x];

                    inventory.AddItem(newMaterial);

                    Destroy(tmp);
                }
            }
        }
    }

    //Handles the player's trigger collision
    private void OnTriggerExit(Collider other) {

		if (other.gameObject.tag == "Chest" || other.gameObject.tag == "CraftingBench" ||other.gameObject.tag == "Vendor") { //If we collide with a chest
            helperText.gameObject.SetActive(false);
         
            if (chest.IsOpen) {
                chest.Open(); //This will close the chest if the player runs away from the chest
            }
     
			chest = null;
        }
    }

    private void OnCollisionEnter(Collision collision) {

		if (collision.gameObject.tag == "Item") { //If we collide with an item that we can pick up

			if (inventory.AddItem(collision.gameObject.GetComponent<ItemScript>())) { //Adds the item to the inventory.
                Destroy(collision.gameObject);
            }
        }
    }

    
    //Sets the player's stats
	public void SetStats(int strength, int endurance, int dexterity, int intellect, int willpower, int charisma, int luck) {
		
        this.strength = strength + baseStrength;
		this.endurance = endurance + baseEndurance;
		this.dexterity = dexterity + baseDexterity;
		this.intellect = intellect + baseIntellect;
		this.willpower = willpower + baseWillpower;
		this.charisma = charisma + baseCharisma;
		this.luck = luck + baseLuck;

		//Calculates maximum health and stamina
		SetMaxHealthAndStamina();

        //Writes the stats text
		statsText.text = string.Format("Strength: {0}\nEndurance: {1}\nDexterity: {2}\nIntellect: {3}\nWillpower: {4}\nCharisma: {5}\nLuck: {6}\n", this.strength, this.endurance, this.dexterity, this.intellect, this.willpower, this.charisma, this.luck);
    }

	//Calculates maximum health and stamina
	public void SetMaxHealthAndStamina() {

		//Create a pointer to the healthbarController script
		PlayerHealthbarController hbController = this.gameObject.GetComponent<PlayerHealthbarController>();

		//Sets max HP and SP based on formula
		hbController.maxHealth = (Endurance * 15) + baseHealth;
		hbController.maxStamina = (Endurance * 10) + baseStamina;
	}

}
