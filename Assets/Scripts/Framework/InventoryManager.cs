using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

public class InventoryManager : MonoBehaviour
{

    #region fields

	private static InventoryManager instance;						//This is the InventoryManager's singleton instance

	private ItemContainer itemContainer = new ItemContainer();		//This item container contains all the items in the game

	public GameObject slotPrefab;									//The slots prefab
	public GameObject iconPrefab;									//A prefab used for instantiating the hoverObject
	public GameObject itemObject;									//This object is used for instantiating items
	public GameObject dropItem;									    //A prototype of the item to drop
	public GameObject tooltipObject;								//The tool tip to show at the screen
	public Text sizeTextObject;										//This object is used for scaling the tooltip
	public Text visualTextObject;								    //This is the visual text on the tooltip
	public Text stackText;									  		//The amount of items to pickup (this is the text on the UI element we use for splitting)
	public GameObject selectStackSize;								//The UI element that we are using when we need to split a stack

	private GameObject hoverObject;									//A reference to the object that hovers next to the mouse

	public Canvas canvas;										    //A reference to the inventorys canvas

	private Slot from;												//The slots that we are moving an item from
	private Slot to;												//The slots that we are moving and item to

	private Slot movingSlot;									    //This is sed to store our items when moving them from one slot to another

	private GameObject clicked;										//The clicked object

	private int splitAmount;										//The amount of items we have in our "hand"

	private int maxStackCount;										//The maximum amount of items we are allowed to remove from the stack

	public EventSystem eventSystem;									//A reference to the EventSystem

    #endregion

    #region properties

    //This is the property for the singleton instance
    public static InventoryManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<InventoryManager>();
            }

            return instance;

        }
    }

    //Property for accssing the item's container
    public ItemContainer ItemContainer
    {
        get { return itemContainer; }
        set { itemContainer = value; }
    }

    //The slot we are moving from
    public Slot From
    {
        get { return from; }
        set { from = value; }
    }

    //The slot we are moving to
    public Slot To
    {
        get { return to; }
        set { to = value; }
    }

    //The clicked item
    public GameObject Clicked
    {
        get { return clicked; }
        set { clicked = value; }
    }
		
    //The acmount if items we are splitting
    public int SplitAmount
    {
        get { return splitAmount; }
        set { splitAmount = value; }
    }
		
    //The max amount of times an item can stack
    public int MaxStackCount
    {
        get { return maxStackCount; }
        set { maxStackCount = value; }
    }
		
    //The slot that contains the items  that we are moing
    public Slot MovingSlot
    {
        get { return movingSlot; }
        set { movingSlot = value; }
    }
		
    //The hover object, that shows the object we have in her hand
    public GameObject HoverObject
    {
        get { return hoverObject; }
        set { hoverObject = value; }
    }

    #endregion

    public void Awake()
    {
        //Creates an XML document
        XmlDocument doc = new XmlDocument();

        //Loads the item xml
        TextAsset myXmlAsset = Resources.Load<TextAsset>("Items");

        //Loads it into the xml document
        doc.LoadXml(myXmlAsset.text);
       
        //Defines all the item types that we can serialize
        Type[] itemTypes = { typeof(Equipment), typeof(Weapon), typeof(Consumeable), typeof(CraftingMaterial) };
       
        //Creates a serializer so that we can serialize the itemContainer
        XmlSerializer serializer = new XmlSerializer(typeof(ItemContainer), itemTypes);
   
        //Instantiates a stringreader, so that we can read the document
        TextReader textReader = new StringReader(doc.InnerXml);

        //Deserializes the itemcontainer, so that we can access the items
        itemContainer = (ItemContainer)serializer.Deserialize(textReader);

        //Closes to textReader, to clear up
        textReader.Close();

        //Creates the blueprints so that we can craft some items
        CraftingBench.Instance.CreateBlueprints();
    }
		
    //Sets the stacks info, so that we know how many items we can remove
    public void SetStackInfo(int maxStackCount)
    {
        //Shows the UI for splitting a stack
        selectStackSize.SetActive(true);

        //Hides the tooltip so that it doesn't overlap the splitstack ui
        tooltipObject.SetActive(false);
        

        //Resets the amount of split items
        splitAmount = 0;

        //Stores the maxcount
        this.maxStackCount = maxStackCount;

        //Writes writes the selected amount of itesm in the UI
        stackText.text = splitAmount.ToString();
    }
		
    //Saves every single inventory in the scene
    public void Save()
    {   
        //Finds all inventories
        GameObject[] inventories = GameObject.FindGameObjectsWithTag("Inventory");
        GameObject[] chests = GameObject.FindGameObjectsWithTag("Chest");

        //Loads all inventories
        foreach (GameObject inventory in inventories)
        {
            inventory.GetComponent<Inventory>().SaveInventory();
        }

        foreach (GameObject chest in chests)
        {
            chest.GetComponent<InventoryLink>().SaveInventory();
        }
    }
		
    //Loads every single inventory in the scene
    public void Load()
    {
        //Finds all inventorys
        GameObject[] inventories = GameObject.FindGameObjectsWithTag("Inventory");
        GameObject[] chests = GameObject.FindGameObjectsWithTag("Chest");

        //Loads all inventories
        foreach (GameObject inventory in inventories)
        {
            inventory.GetComponent<Inventory>().LoadInventory();
        }

        foreach (GameObject chest in chests)
        {
            chest.GetComponent<InventoryLink>().LoadInventory();
        }
    }
}
