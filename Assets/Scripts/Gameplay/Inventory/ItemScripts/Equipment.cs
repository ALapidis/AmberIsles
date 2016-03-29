using UnityEngine;
using System.Collections;

public class Equipment : Item
{
	public int Strength { get; set; }
	public int Endurance { get; set; }
	public int Dexterity { get; set; }
	public int Intellect { get; set; }
	public int Willpower { get; set; }
	public int Charisma { get; set; }
	public int Luck { get; set; }

    public Equipment()
    { }

    /// <summary>
    /// The Equipment's constructor
    /// </summary>
    /// <param name="itemName">The name of the item</param>
    /// <param name="description">The item's description</param>
    /// <param name="itemType">The type of time</param>
    /// <param name="quality">The item's quality</param>
    /// <param name="spriteNeutral">Path to the item's sprite</param>
    /// <param name="spriteHighlighted">Path to the items highlighted sprite</param>
    /// <param name="maxSize">The item's max size</param>
    /// <param name="intellect">The item's intellect</param>
    /// <param name="agility">The item's agility</param>
    /// <param name="stamina">The Item's stamina</param>
    /// <param name="strength">The item's strength</param>
	public Equipment(string itemName, string description, ItemType itemType, Quality quality, string spriteNeutral, string spriteHighlighted, int maxSize, int strength, int endurance, int dexterity, int intellect, int willpower, int charisma, int luck)
        : base(itemName, description, itemType, quality, spriteNeutral, spriteHighlighted, maxSize)
    {
		this.Strength = strength;
		this.Endurance = endurance;
		this.Dexterity = dexterity;
		this.Intellect = intellect;
		this.Willpower = willpower;
		this.Charisma = charisma;
		this.Luck = luck;
    }

    /// <summary>
    /// Uses the item
    /// </summary>
    public override void Use(Slot slot, ItemScript item)
    {
        CharacterPanel.Instance.EquipItem(slot, item);
    }

    /// <summary>
    /// Creates a tooltip
    /// </summary>
    /// <returns>A complete tooltip</returns>
    public override string GetTooltip(Inventory inv)
    {
        string stats = string.Empty;

        if (Strength > 0) //Adds Strength to the tooltip if it is larger than 0
        {
            stats += "\n+" + Strength.ToString() + " Strength";
        }
		if (Endurance > 0)//Adds Endurance to the tooltip if it is larger than 0
		{
			stats += "\n+" + Endurance.ToString() + " Endurance";
		}
		if (Dexterity > 0)//Adds Dexterity to the tooltip if it is larger than 0
		{
			stats += "\n+" + Dexterity.ToString() + " Dexterity";
		}
		if (Intellect > 0) //Adds Intellect to the tooltip if it is larger than 0
        {
            stats += "\n+" + Intellect.ToString() + " Intellect";
        }
		if (Willpower > 0) //Adds Willpower to the tooltip if it is larger than 0
		{
			stats += "\n+" + Willpower.ToString() + " Willpower";
		}
		if (Charisma > 0) //Adds Charisma to the tooltip if it is larger than 0
		{
			stats += "\n+" + Charisma.ToString() + " Charisma";
		}
		if (Luck > 0) //Adds Luck to the tooltip if it is larger than 0
		{
			stats += "\n+" + Luck.ToString() + " Luck";
		}
    
		//Gets the tooltip from the base class
        string itemTip = base.GetTooltip(inv);

        if (inv is VendorInventory && !(this is Weapon))
        {
            return string.Format("{0}" + "<size=14>{1}</size>\n<color=yellow>Price: {2} </color>", itemTip, stats, BuyPrice);
        }
        else if (VendorInventory.Instance.isActiveAndEnabled && !(this is Weapon))
        {
            return string.Format("{0}" + "<size=14>{1}</size>\n<color=yellow>Price: {2} </color>", itemTip, stats, SellPrice);
        }
        else
        {
            //Returns the complete tooltip
            return string.Format("{0}" + "<size=14>{1}</size>", itemTip, stats);
        }
        

    }
}
