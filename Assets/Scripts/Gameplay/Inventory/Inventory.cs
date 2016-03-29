using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class Inventory : MonoBehaviour
{

    #region Variables

	public enum PackTypes {
		Beginner = 20,
		Adept = 30,
		Adventurer = 40,
		Journeyman = 50,
		Quester = 60,
		Trader = 70,
		AncientRucksack = 80,
		EnscrolledPouch = 90,
		LordsSachel = 100,
	}

	public int rows;										//The number of rows

	[SerializeField]
	private PackTypes _inventorySize;

    [SerializeField]
	private int emptySlots;									//The number of empty slots in the inventory

	protected float hoverYOffset;							//Offset used to move the hovering object away from the mouse 

	protected float inventoryWidth, inventoryHight;			//The width and height of the inventory

	public float slotPaddingLeft, slotPaddingTop;			//The left and top slots padding

	public float slotSize;									//The size of each slot

	protected RectTransform inventoryRect;					//A reference to the inventorys RectTransform

	public CanvasGroup canvasGroup;							//The inventory's canvas group, this is used for hiding the inventory

	protected static GameObject playerRef;					//A reference to the player

	private bool isOpen;									//This indicates if the inventory is open or closed
    
	public static bool mouseInside = false;

    #endregion

    #region Collections

	protected List<GameObject> allSlots;					//A list of all the slots in the inventory

    #endregion

    #region Properties

	//Cast the read as int and write as packTypes enum
	public int slots 
	{ 	
		get { return (int)_inventorySize; } 
		set { _inventorySize = (PackTypes)value; } 
	}

	//Indicates if the inventory is open
	public bool IsOpen										
    {
        get { return isOpen; }
        set { isOpen = value; }
    }
		
	//Property for accessing the amount of empty slots
	public int EmptySlots									
    {
        get { return emptySlots; }
        set { emptySlots = value; }
    }

    #endregion

    // Use this for initialization
    protected virtual void Start()
    {
        IsOpen = false;
			
        playerRef = GameObject.Find("Player");

        //Creates the inventory layout
        CreateLayout();

        InventoryManager.Instance.MovingSlot = GameObject.Find("MovingSlot").GetComponent<Slot>();

    }

    // Update is called once per frame
	void Update() {
		
		if (Input.GetMouseButtonUp(0)) { //Checks if the user lifted the first mousebutton
        
            //Removes the selected item from the inventory
			if (!mouseInside && InventoryManager.Instance.From != null) { //If we click outside the inventory and the have picked up an item
            
                InventoryManager.Instance.From.GetComponent<Image>().color = Color.white; //Rests the slots color 

				foreach (ItemScript item in InventoryManager.Instance.From.Items) {
                
                    float angle = UnityEngine.Random.Range(0.0f, Mathf.PI * 2);

                    Vector3 v = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));

                    v *= 25;

                    GameObject tmpDrp = (GameObject)GameObject.Instantiate(InventoryManager.Instance.dropItem, playerRef.transform.position - v, Quaternion.identity);

                    tmpDrp.AddComponent<ItemScript>();
                    tmpDrp.GetComponent<ItemScript>().Item = item.Item;
                }

                InventoryManager.Instance.From.ClearSlot(); //Removes the item from the slot

				if (InventoryManager.Instance.From.transform.parent == CharacterPanel.Instance.transform) {
                
                    CharacterPanel.Instance.CalcStats();
                }

                Destroy(GameObject.Find("Hover")); //Removes the hover icon

                //Resets the objects
                InventoryManager.Instance.To = null;
                InventoryManager.Instance.From = null;
        
			} else if (!InventoryManager.Instance.eventSystem.IsPointerOverGameObject(-1) && !InventoryManager.Instance.MovingSlot.IsEmpty) {
           
				foreach (ItemScript item in InventoryManager.Instance.MovingSlot.Items) {
                
                    float angle = UnityEngine.Random.Range(0.0f, Mathf.PI * 2);

                    Vector3 v = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));

                    v *= 25;

                    GameObject tmpDrp = (GameObject)GameObject.Instantiate(InventoryManager.Instance.dropItem, playerRef.transform.position - v, Quaternion.identity);


                    tmpDrp.AddComponent<ItemScript>();
                    tmpDrp.GetComponent<ItemScript>().Item = item.Item;
                }

                InventoryManager.Instance.MovingSlot.ClearSlot();
                Destroy(GameObject.Find("Hover"));
            }
        }

		if (InventoryManager.Instance.HoverObject != null) { //Checks if the hoverobject exists
        
            //The hoverObject's position
            Vector2 position;

            //Translates the mouse screen position into a local position and stores it in the position
            RectTransformUtility.ScreenPointToLocalPointInRectangle(InventoryManager.Instance.canvas.transform as RectTransform, Input.mousePosition, InventoryManager.Instance.canvas.worldCamera, out position);
  
            //Adds the offset to the position
            position.Set(position.x, position.y - hoverYOffset);

            //Sets the hoverObject's position
            InventoryManager.Instance.HoverObject.transform.position = InventoryManager.Instance.canvas.transform.TransformPoint(position);
        }        
    }
		

    public void PointerExit() {
        
		mouseInside = false;
    }

    public void PointerEnter() {
        
		if (canvasGroup.alpha > 0) {

            mouseInside = true;
        }
    }

	//Toggles the alpha value of the canvasGroup and Book fields
    public virtual void Open() {
		
		if (canvasGroup.alpha > 0) { //If our inventory is visible, then we know that it is open
	         
			//Put all items we have in our hand back in the inventory
			PutItemBack(); 
            HideToolTip();

			canvasGroup.blocksRaycasts = false;
			canvasGroup.alpha = 0;
			isOpen = false;

        } else { //If it isn't open then it's closed and we neeed to fade in

			canvasGroup.blocksRaycasts = true;
			canvasGroup.alpha = 1;
			isOpen = true;
        }
	}


    //Shows the tooltip
    //The slot we just hovered
    public virtual void ShowToolTip(GameObject slot) {
        
		//Saves a reference to the slot we just moused over
        Slot tmpSlot = slot.GetComponent<Slot>();

        //If the slot contains an item and we arent splitting or moving any items then we can show the tooltip
		if (slot.GetComponentInParent<Inventory>().isOpen && !tmpSlot.IsEmpty && InventoryManager.Instance.HoverObject == null && !InventoryManager.Instance.selectStackSize.activeSelf) {
        
            //Gets the information from the item on the slot we just moved our mouse over
            InventoryManager.Instance.visualTextObject.text = tmpSlot.CurrentItem.GetTooltip(this);

            //Makes sure that the tooltip has the correct size.
            InventoryManager.Instance.sizeTextObject.text = InventoryManager.Instance.visualTextObject.text;

            //Shows the tool tip
            InventoryManager.Instance.tooltipObject.SetActive(true);

            //Calculates the position while taking the padding into account
            float xPos = slot.transform.position.x + slotPaddingLeft;
            float yPos = slot.transform.position.y - slot.GetComponent<RectTransform>().sizeDelta.y - slotPaddingTop;

            //Sets the position
            InventoryManager.Instance.tooltipObject.transform.position = new Vector2(xPos, yPos);
        }
    }
		
    //Hide the tooltip
    public void HideToolTip() {
       
		InventoryManager.Instance.tooltipObject.SetActive(false);
    }


    //Saves the inventory and its content
    public virtual void SaveInventory() {
        
		string content = string.Empty; //Creates a string for containing infor about the items inside the inventory

		for (int i = 0; i < allSlots.Count; i++) { //Runs through all slots in the inventory
        
            Slot tmp = allSlots[i].GetComponent<Slot>(); //Careates a reference to the slot at the current index

			if (!tmp.IsEmpty) { //We only want to save the info if the slot contains an item
            
                //Creates a string with this format: SlotIndex-ItemType-AmountOfItems; this string can be read so that we can rebuild the inventory
                content += i + "-" + tmp.CurrentItem.Item.ItemName.ToString() + "-" + tmp.Items.Count.ToString() + ";";
            }
        }

        //Stores all the info in the PlayerPrefs
        PlayerPrefs.SetString(gameObject.name + "content", content);
        PlayerPrefs.SetInt(gameObject.name + "slots", slots);
        PlayerPrefs.SetInt(gameObject.name + "rows", rows);
        PlayerPrefs.SetFloat(gameObject.name + "slotPaddingLeft", slotPaddingLeft);
        PlayerPrefs.SetFloat(gameObject.name + "slotPaddingTop", slotPaddingTop);
        PlayerPrefs.SetFloat(gameObject.name + "slotSize", slotSize);
        PlayerPrefs.SetFloat(gameObject.name + "xPos", inventoryRect.position.x);
        PlayerPrefs.SetFloat(gameObject.name + "yPos", inventoryRect.position.y);
        PlayerPrefs.Save();
    }


    //Loads the inventory
    public virtual void LoadInventory() {
        
		foreach (GameObject slot in allSlots) {
            
			slot.GetComponent<Slot>().ClearSlot();
        }

        //Loads all the inventory's data from the playerprefs
        string content = PlayerPrefs.GetString(gameObject.name + "content");

        if (content != string.Empty) {
            
			slots = PlayerPrefs.GetInt(gameObject.name + "slots");
            rows = PlayerPrefs.GetInt(gameObject.name + "rows");
            slotPaddingLeft = PlayerPrefs.GetFloat(gameObject.name + "slotPaddingLeft");
            slotPaddingTop = PlayerPrefs.GetFloat(gameObject.name + "slotPaddingTop");
            slotSize = PlayerPrefs.GetFloat(gameObject.name + "slotSize");

            //Sets the inventorys position
            inventoryRect.position = new Vector3(PlayerPrefs.GetFloat(gameObject.name + "xPos"), PlayerPrefs.GetFloat(gameObject.name + "yPos"), inventoryRect.position.z);

            //Recreates the inventory's layout
            CreateLayout();

            //Splits the loaded content string into segments, so that each index inthe splitContent array contains information about a single slot
            //e.g[0]0-MANA-3
            string[] splitContent = content.Split(';');

            //Runs through every single slot we have infor about -1 is to avoid an empty string error
            for (int x = 0; x < splitContent.Length - 1; x++) {
                
				//Splits the slot's information into single values, so that each index in the splitValues array contains info about a value
                //E.g[0]InventorIndex [1]ITEMTYPE [2]Amount of items
                string[] splitValues = splitContent[x].Split('-');

                int index = Int32.Parse(splitValues[0]); //InventorIndex 

                string itemName = splitValues[1]; //ITEMTYPE

                int amount = Int32.Parse(splitValues[2]); //Amount of items

                Item tmp = null;

				for (int i = 0; i < amount; i++) { //Adds the correct amount of items to the inventory
                
                    GameObject loadedItem = Instantiate(InventoryManager.Instance.itemObject);

					if (tmp == null) {
                        tmp = InventoryManager.Instance.ItemContainer.Consumeables.Find(item => item.ItemName == itemName);
                    }
					if (tmp == null) {
                        tmp = InventoryManager.Instance.ItemContainer.Equipment.Find(item => item.ItemName == itemName);
                    }
					if (tmp == null) {
                        tmp = InventoryManager.Instance.ItemContainer.Weapons.Find(item => item.ItemName == itemName);
                    }
					if (tmp == null) {
                           tmp = InventoryManager.Instance.ItemContainer.Materials.Find(item => item.ItemName == itemName);
                    }

                    loadedItem.AddComponent<ItemScript>();
                    loadedItem.GetComponent<ItemScript>().Item = tmp;
                    allSlots[index].GetComponent<Slot>().AddItem(loadedItem.GetComponent<ItemScript>());
                    Destroy(loadedItem);
                }
            }
        }
    }


    //Creates the inventory's layout
    public virtual void CreateLayout() {
        if (allSlots != null) {
            
			foreach (GameObject go in allSlots) {
                
				Destroy(go);
            }
        }

        //Instantiates the allSlot's list
        allSlots = new List<GameObject>();

        //Calculates the hoverYOffset by taking 1% of the slot size
        hoverYOffset = slotSize * 0.1f;

        //Stores the number of empty slots
        emptySlots = slots;

        //Calculates the width of the inventory
        inventoryWidth = (slots / rows) * (slotSize + slotPaddingLeft);

        //Calculates the highs of the inventory
        inventoryHight = rows * (slotSize + slotPaddingTop);

        //Creates a reference to the inventory's RectTransform
        inventoryRect = GetComponent<RectTransform>();

        //Sets the with and height of the inventory.
        inventoryRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, inventoryWidth + slotPaddingLeft);
        inventoryRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, inventoryHight + slotPaddingTop);

        //Calculates the amount of columns
        int columns = slots / rows;

		for (int y = 0; y < rows; y++) { //Runs through the rows

			for (int x = 0; x < columns; x++) { //Runs through the columns
            
                //Instantiates the slot and creates a reference to it
                GameObject newSlot = (GameObject)Instantiate(InventoryManager.Instance.slotPrefab);

                //Makes a reference to the rect transform
                RectTransform slotRect = newSlot.GetComponent<RectTransform>();

                //Sets the slots name
                newSlot.name = "Slot";

                //Sets the canvas as the parent of the slots, so that it will be visible on the screen
                newSlot.transform.SetParent(this.transform.parent);

                //Sets the slots position
                slotRect.localPosition = inventoryRect.localPosition + new Vector3(slotPaddingLeft * (x + 1) + (slotSize * x), -slotPaddingTop * (y + 1) - (slotSize * y));

                //Sets the size of the slot
                slotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, slotSize * InventoryManager.Instance.canvas.scaleFactor);
                slotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, slotSize * InventoryManager.Instance.canvas.scaleFactor);
                newSlot.transform.SetParent(this.transform);

                //Adds the new slots to the slot list
                allSlots.Add(newSlot);

                newSlot.GetComponent<Button>().onClick.AddListener
                    (
                        delegate{MoveItem(newSlot);  }
                       
                    );
            }
        }
    }
		
    //Adds an item to the inventory
    public bool AddItem(ItemScript item) {
        
		if (item.Item.MaxSize == 1) { //If the item isn't stackable
        
            //Places the item at an empty slot
            return PlaceEmpty(item);
		} else { //If the item is stackable 
			
			foreach (GameObject slot in allSlots) { //Runs through all slots in the inventory
            
                Slot tmp = slot.GetComponent<Slot>(); //Creates a reference to the slot

				if (!tmp.IsEmpty) { //If the item isn't empty
                
                    //Checks if the om the slot is the same type as the item we want to pick up
					if (tmp.CurrentItem.Item.ItemName == item.Item.ItemName && tmp.IsAvailable) {
                    
						if (!InventoryManager.Instance.MovingSlot.IsEmpty && InventoryManager.Instance.Clicked.GetComponent<Slot>() == tmp.GetComponent<Slot>()) {
                        
                            continue;
                        } else {
                            
							tmp.AddItem(item); //Adds the item to the inventory
                            return true;
                        }
                    }
                }
            }
            
			if (emptySlots > 0) //Places the item on an empty slots
            {
                return PlaceEmpty(item);
            }
        }

        return false;
    }
 
    //Places an item on an empty slot
	private bool PlaceEmpty(ItemScript item) {
    
		if (emptySlots > 0) { //If we have atleast 1 empty slot
        
			foreach (GameObject slot in allSlots) { //Runs through all slots
            
                Slot tmp = slot.GetComponent<Slot>(); //Creates a reference to the slot 

				if (tmp.IsEmpty) { //If the slot is empty
                
                    tmp.AddItem(item); //Adds the item

                    return true;
                }
            }
        }

        return false;
    }
		
    //Moves an item to another slot in the inventory
	public virtual void MoveItem(GameObject clicked) {
    
		if (isOpen) {
        
            CanvasGroup cg = clicked.transform.parent.GetComponent<CanvasGroup>();

			if (cg != null && cg.alpha > 0 || clicked.transform.parent.parent.GetComponent<CanvasGroup>().alpha > 0) {
            
                //Careates a reference to the object that we just clicked
                InventoryManager.Instance.Clicked = clicked;

				if (!InventoryManager.Instance.MovingSlot.IsEmpty) { //Checks if we are splitting an item 
                
                    Slot tmp = clicked.GetComponent<Slot>(); //Get's a reference to the slot we just clicked

					if (tmp.IsEmpty) { //If the clicked slot is empty, then we can simply put all items down
                    
                        tmp.AddItems(InventoryManager.Instance.MovingSlot.Items); //Puts all the items down in the slot that we clicked
                        InventoryManager.Instance.MovingSlot.Items.Clear(); //Clears the moving slot
                        Destroy(GameObject.Find("Hover")); //Removes the hover object
                    } else if (!tmp.IsEmpty && InventoryManager.Instance.MovingSlot.CurrentItem.Item.ItemName == tmp.CurrentItem.Item.ItemName && tmp.IsAvailable) { //If the slot we clicked isn't empty, then we need to merge the stacks
                    
                        //Merges two stacks of the same type
                        MergeStacks(InventoryManager.Instance.MovingSlot, tmp);
                    }

				} else if (InventoryManager.Instance.From == null && clicked.transform.parent.GetComponent<Inventory>().isOpen && !Input.GetKey(KeyCode.LeftShift)) { //If we haven't picked up an item
                
					if (!clicked.GetComponent<Slot>().IsEmpty && !GameObject.Find("Hover")) { //If the slot we clicked sin't empty
                    
                        InventoryManager.Instance.From = clicked.GetComponent<Slot>(); //The slot we ar emoving from

                        InventoryManager.Instance.From.GetComponent<Image>().color = Color.gray; //Sets the from slots color to gray, to visually indicate that its the slot we are moving from

                        CreateHoverIcon();
                    }

				} else if (InventoryManager.Instance.To == null && !Input.GetKey(KeyCode.LeftShift)) { //Selects the slot we are moving to

                    InventoryManager.Instance.To = clicked.GetComponent<Slot>(); //Sets the to object
                    Destroy(GameObject.Find("Hover")); //Destroys the hover object
                }

				if (InventoryManager.Instance.To != null && InventoryManager.Instance.From != null) { //If both to and from are null then we are done moving. 
                
					if (!InventoryManager.Instance.To.IsEmpty && InventoryManager.Instance.From.CurrentItem.Item.ItemName == InventoryManager.Instance.To.CurrentItem.Item.ItemName && InventoryManager.Instance.To.IsAvailable) {
                    
                        MergeStacks(InventoryManager.Instance.From, InventoryManager.Instance.To);
					} else {
						
                        Slot.SwapItems(InventoryManager.Instance.From, InventoryManager.Instance.To);
                    }

                    //Resets all values
                    InventoryManager.Instance.From.GetComponent<Image>().color = Color.white;
                    InventoryManager.Instance.To = null;
                    InventoryManager.Instance.From = null;
                    Destroy(GameObject.Find("Hover"));
                }
            }

			if (CraftingBench.Instance.isOpen) {
            
                CraftingBench.Instance.UpdatePreview();
            }
        }
    }
		
    //Creates a hover icon next to the mouse
	private void CreateHoverIcon() {
    
        InventoryManager.Instance.HoverObject = (GameObject)Instantiate(InventoryManager.Instance.iconPrefab); //Instantiates the hover object 

        InventoryManager.Instance.HoverObject.GetComponent<Image>().sprite = InventoryManager.Instance.Clicked.GetComponent<Image>().sprite; //Sets the sprite on the hover object so that it reflects the object we are moing

        InventoryManager.Instance.HoverObject.name = "Hover"; //Sets the name of the hover object

        //Creates references to the transforms
        RectTransform hoverTransform = InventoryManager.Instance.HoverObject.GetComponent<RectTransform>();
        RectTransform clickedTransform = InventoryManager.Instance.Clicked.GetComponent<RectTransform>();

        ///Sets the size of the hoverobject so that it has the same size as the clicked object
        hoverTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, clickedTransform.sizeDelta.x);
        hoverTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, clickedTransform.sizeDelta.y);

        //Sets the hoverobject's parent as the canvas, so that it is visible in the game
        InventoryManager.Instance.HoverObject.transform.SetParent(GameObject.Find("Canvas").transform, true);

        //Sets the local scale to make usre that it has the correct size
        InventoryManager.Instance.HoverObject.transform.localScale = InventoryManager.Instance.Clicked.gameObject.transform.localScale;

        InventoryManager.Instance.HoverObject.transform.GetChild(0).GetComponent<Text>().text = InventoryManager.Instance.MovingSlot.Items.Count > 1 ? InventoryManager.Instance.MovingSlot.Items.Count.ToString() : string.Empty;
    }
		
    //Puts the items back in the inventory
	private void PutItemBack() {
    
		if (InventoryManager.Instance.From != null) { //If we are carrying a whole stack of items
        
            //put the items back and remove the hover icon
            Destroy(GameObject.Find("Hover"));
            InventoryManager.Instance.From.GetComponent<Image>().color = Color.white;
            InventoryManager.Instance.From = null;
		} else if (!InventoryManager.Instance.MovingSlot.IsEmpty) { //If we are carrying  split stack
			
            //Removes the hover icon
            Destroy(GameObject.Find("Hover"));

            //Puts the items back one by one
			foreach (ItemScript item in InventoryManager.Instance.MovingSlot.Items) {
            
                InventoryManager.Instance.Clicked.GetComponent<Slot>().AddItem(item);
            }

            InventoryManager.Instance.MovingSlot.ClearSlot(); //Makes sure that the moving slot is empty
        }

        //Hides the UI for splitting a stack
        InventoryManager.Instance.selectStackSize.SetActive(false);
    }

    //Splits a stack of items
	public void SplitStack() {
    
        //Hids the UI for splitting a stack
        InventoryManager.Instance.selectStackSize.SetActive(false);

		if (InventoryManager.Instance.SplitAmount == InventoryManager.Instance.MaxStackCount) { //If we picked up all the items then we dont need to handle it as as split stack 
        
            MoveItem(InventoryManager.Instance.Clicked);
		} else if (InventoryManager.Instance.SplitAmount > 0) { //If the split amount is larger than 0 then we need to pick up x amount of items
        
            InventoryManager.Instance.MovingSlot.Items = InventoryManager.Instance.Clicked.GetComponent<Slot>().RemoveItems(InventoryManager.Instance.SplitAmount); //Picks up the items 

            CreateHoverIcon(); //Careates the hover icon
        }
    }
		
    //Updates the text on the split UI elemt so that it reflects the users selection
	public void ChangeStackText(int i) {
    
        InventoryManager.Instance.SplitAmount += i;

		if (InventoryManager.Instance.SplitAmount < 0) { //Makes sure we dont go below 
        
            InventoryManager.Instance.SplitAmount = 0;
        }

		if (InventoryManager.Instance.SplitAmount > InventoryManager.Instance.MaxStackCount) { //Makes sure that we dont go above max
        
            InventoryManager.Instance.SplitAmount = InventoryManager.Instance.MaxStackCount;
        }

        //Writes the text on the UI element
        InventoryManager.Instance.stackText.text = InventoryManager.Instance.SplitAmount.ToString();
    }
		
    //Merges the items on two slots
	//The slot to merge the items from, The slot to merge the items into
	public void MergeStacks(Slot source, Slot destination) {
    
        //Calculates the max amount of items we are allowed to merge onto the stack
        int max = destination.CurrentItem.Item.MaxSize - destination.Items.Count;

        //Sets the correct amount so that we don't put too many items down
        int count = source.Items.Count < max ? source.Items.Count : max;

		for (int i = 0; i < count; i++) { //Merges the items into the other stack
        
            destination.AddItem(source.RemoveItem()); //Removes the items from the source and adds them to the destination
            InventoryManager.Instance.HoverObject.transform.GetChild(0).GetComponent<Text>().text = InventoryManager.Instance.MovingSlot.Items.Count.ToString(); //Updates the text on the stack that
        }
        
		if (source.Items.Count == 0) { //We don't have more items to merge with
       
            Destroy(GameObject.Find("Hover"));
        }
    }

}
