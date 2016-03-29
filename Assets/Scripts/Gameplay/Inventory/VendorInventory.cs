using UnityEngine;
using System.Collections;

public class VendorInventory : ChestInventory
{
    private static VendorInventory instance;

    public static VendorInventory Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<VendorInventory>();
            }
            return instance;
        }
    }

    protected override void Start()
    {
        EmptySlots = slots;

        base.Start();

        GiveItem("Health potion");
        GiveItem("Awesome Sword");
        GiveItem("Strong Helmet");
    }


    protected void GiveItem(string itemName)
    {
        GameObject tmp = Instantiate(InventoryManager.Instance.itemObject);

        tmp.AddComponent<ItemScript>();

        ItemScript newItem = tmp.GetComponent<ItemScript>();

        if (InventoryManager.Instance.ItemContainer.Consumeables.Exists(x => x.ItemName == itemName))
        {
            newItem.Item = InventoryManager.Instance.ItemContainer.Consumeables.Find(x => x.ItemName == itemName);

        }
        else if (InventoryManager.Instance.ItemContainer.Weapons.Exists(x => x.ItemName == itemName))
        {
            newItem.Item = InventoryManager.Instance.ItemContainer.Weapons.Find(x => x.ItemName == itemName);
        }
        else if (InventoryManager.Instance.ItemContainer.Equipment.Exists(x => x.ItemName == itemName))
        {
            newItem.Item = InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == itemName);
        }

        if (newItem != null)
        {
            AddItem(newItem);
        }

        Destroy(tmp);
    }

    public override void MoveItem(GameObject clicked)
    {
        
    }



}
