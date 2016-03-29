using UnityEngine;
using System.Collections;

public abstract class Item
{

    public ItemType ItemType { get; set; }

    public Quality Quality { get; set; }

    //File path to the neutral sprite
    public string SpriteNeutral { get; set; }

    //File path to the highlighted sprite
    public string SpriteHighlighted { get; set; }

    //The maximum amount of stacks
    public int MaxSize { get; set; }

    public string ItemName { get; set; }

    public string Description { get; set; }

    public int BuyPrice { get; set; }

    public int SellPrice { get; set; }

    //Empty constructor for serilization
    public Item()
    { 
        
    }
    
    //The items's constructor
    public Item(string itemName, string description, ItemType itemType, Quality quality, string spriteNeutral, string spriteHighlighted, int maxSize) {
 
		//Sets all the stats
        this.ItemName = itemName;
        this.Description = description;
        this.ItemType = itemType;
        this.Quality = quality;
        this.SpriteNeutral = spriteNeutral;
        this.SpriteHighlighted = spriteHighlighted;
        this.MaxSize = maxSize;

    }

    
    //This function uses the item
    public abstract void Use(Slot slot, ItemScript item);

    
    //Creates a tooltip
    public virtual string GetTooltip(Inventory inv) {

        string color = string.Empty;  //Resets the color info
        string newLine = string.Empty; //Resets the new line

		//Creates a newline if the item has a description, this is done to makes sure that the headline and the describion isn't on the same line
		if (Description != string.Empty) { 
            newLine = "\n";
        }

		//Sets the color accodring to the quality of the item
        switch (Quality) 
        {
            case Quality.COMMON:
                color = "white";
                break;
            case Quality.UNCOMMON:
                color = "lime";
                break;
            case Quality.RARE:
                color = "navy";
                break;
            case Quality.EPIC:
                color = "magenta";
                break;
            case Quality.LEGENDARY:
                color = "orange";
                break;
            case Quality.ARTIFACT:
                color = "red";
                break;
        }

        //Returns the item info so that we can use it in the tooltip
        return string.Format("<color=" + color + "><size=16>{0}</size></color><size=14><i><color=lime>" + newLine + "{1}</color></i>\n{2}</size>", ItemName, Description,ItemType.ToString().ToLower());
    }

}
