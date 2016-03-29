using UnityEngine;
using System.Collections;
using System;

public class CharacterPanel : Inventory 
{

    //Contains all the character panel's slots
    public Slot[] equipmentSlots;

    //The singleton's instance
    private static CharacterPanel instance;

    //This property is used to access the singleton instance
    public static CharacterPanel Instance
    {
        get
        {
            if (instance == null) //Makes sure that we only have 1 instance
            {
                instance = GameObject.FindObjectOfType<CharacterPanel>();
            }

            return CharacterPanel.instance; 
        }
    }
		
    //Returns the weapon slot
	public Slot WeaponSlot {
    
        get { return equipmentSlots[9]; }
    }
		
    //Returns the offhand slot
	public Slot OffHandSlot {
    
        get { return equipmentSlots[10]; }
    }
		
    void Awake()
    {   
        //Creates a reference to all the equipment slots, and adds them to an array
        equipmentSlots = transform.GetComponentsInChildren<Slot>();
    }

	public override void CreateLayout() {
       
    }
		
    //Equipts an item
	//The slot the item comes from, The item to equip
    public void EquipItem(Slot slot, ItemScript item)
    {
        //If we are equipping something in our hands, then we need to make sure, that we cant equip a twohand and an offhand at the same time
		if (item.Item.ItemType == ItemType.MAINHAND || item.Item.ItemType == ItemType.TWOHAND && OffHandSlot.IsEmpty) {
        
            Slot.SwapItems(slot, WeaponSlot); //Equips the item
		} else { //If it isn't a weapon
			
            Slot.SwapItems(slot, Array.Find(equipmentSlots, x => x.canContain == item.Item.ItemType)); //Equips the item
        }
    }
		
    //Calculates the stats based on the equipped items
	public void CalcStats() {
    
		int strength = 0;
		int endurance = 0;
		int dexterity = 0;
        int intellect = 0;
		int willpower = 0;
		int charisma = 0;
		int luck = 0;


		foreach (Slot slot in equipmentSlots) { //Runs through all the equipment, and calculates the items.
        
            if (!slot.IsEmpty) //If the slot isn't empty then we need to add the stats
            {
                Equipment e = (Equipment)slot.CurrentItem.Item;
				strength += e.Strength;
				endurance += e.Endurance;
				dexterity += e.Dexterity;
                intellect += e.Intellect;
				willpower += e.Willpower;
				charisma += e.Charisma;
				luck += e.Luck;
            }
        }

		Player.Instance.SetStats(strength, endurance, dexterity, intellect, willpower, charisma, luck); //Sets the player stats
    }
		
    //Saves the char panel
    public override void SaveInventory()
    {
        string content = string.Empty;

        for (int i = 0; i < equipmentSlots.Length; i++) //Runs through all the slots and stores the content in a string
        {
            if (!equipmentSlots[i].IsEmpty)
            {
                content += i + "-" + equipmentSlots[i].Items.Peek().Item.ItemName + ";";
            }
        }

        //Saves the string
        PlayerPrefs.SetString("CharPanel", content); 
        PlayerPrefs.Save();
    }
		
    //Loads the inventory
    public override void LoadInventory()
    {
        //Runs through all the slots and clears them, so that we only have new loaded items in the inventory
        foreach (Slot slot in equipmentSlots) 
        {
            slot.ClearSlot();
        }

        //Loads the saved content into a string
        string content = PlayerPrefs.GetString("CharPanel");

        //Splits the string, so that we can load the correct items
        string[] splitContent = content.Split(';');

        //Runs through the content
        for (int i = 0; i < splitContent.Length-1; i++)
        {   
            //Splits the values
            string[] splitValues = splitContent[i].Split('-');

            //Stores item index in the char panel
            int index = Int32.Parse(splitValues[0]);

            //Gets the itemname
            string itemName = splitValues[1];

            //Creates an object for the loaded item
            GameObject loadedItem = Instantiate(InventoryManager.Instance.itemObject);

            //Adds a script to the item, so that we can add it to the charpanel
            loadedItem.AddComponent<ItemScript>();

            //IF we have a weapon
            if (index == 9 || index == 10)
            {
                loadedItem.GetComponent<ItemScript>().Item = InventoryManager.Instance.ItemContainer.Weapons.Find(x => x.ItemName == itemName);
            }
            else //If we equip anything else
	        {
                loadedItem.GetComponent<ItemScript>().Item = InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == itemName);
	        }

            //Adds the item to the charpanel
            equipmentSlots[index].AddItem(loadedItem.GetComponent<ItemScript>());

            Destroy(loadedItem); //Destroys the extra tiem

            //Recalculates the stats after loading
            CalcStats();
        }
    }
}
