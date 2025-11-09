using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InventoryController : MonoBehaviour
{
    [SerializeField]
    int maxSlotsNr = int.MaxValue;
    [SerializeField]
    int maxBigSlotsNr = 1;
    [SerializeReference]
    public List<ItemWeaponWrapper> allItems = new List<ItemWeaponWrapper>();
    public List<ItemWeaponWrapper> bigSlots = new List<ItemWeaponWrapper>();
    public ToolsController toolsController;

    public int currentSlot = 0;
    private void Awake()
    {
        toolsController = GetComponent<ToolsController>();
    }
    private void FixedUpdate()
    {
        
    }
    public bool ChangeLocation(ItemWeaponWrapper item, ItemLocation _location, bool priority = false)
    {
        if (_location == ItemLocation.Back)
            if (item.Big && allItems.Count(i => i.Big && i.location == ItemLocation.Back) >= maxBigSlotsNr)
            {
                var bigItem = allItems.FirstOrDefault(z => z.Big && z.location == ItemLocation.Back);
                if (priority)
                {
                    RemoveFromInventory(bigItem);
                }
                else
                {
                    RemoveFromInventory(item);
                    return false;
                }   
            }

        item.location = _location;
        return true;
    }
    public bool AddToInventory(ItemWeaponWrapper item)
    {
        if (item == null) return false;
        if (item.emptyhanded) return false;
        // Don't allow adding big item to inventory if limit is hit
        

        // Stack if possible
        var matching = allItems.FirstOrDefault(i => i.ID == item.ID && i.Stackable && i.location == ItemLocation.Inventory);
        if (matching != null)
        {
            matching.stack += item.stack;
            item.DestroyPhysicalPresence();
            return true;
        }

        // Otherwise, add normally
      //  if (item.Big) 
     //       item.location = ItemLocation.Back;
      //  else
      //      item.location = ItemLocation.Inventory;
        allItems.Add(item);
        currentSlot = allItems.Count - 1;
       // item.DestroyPhysicalPresence();
        return true;
    }
    public ItemWeaponWrapper GetWeaponOnTheBack()
    {
        return allItems.FirstOrDefault(i => i.location == ItemLocation.Back);
    }
    public bool IsAlreadyInInventory(ItemWeaponWrapper item)
    {
        if (allItems.Any(n => n.ID == item.ID))
            return true;
        
        return false;
    }
    public ItemWeaponWrapper RemoveFromInventory(int index = 0)
    {
        if (allItems.Count <= index) return null;
        ItemWeaponWrapper itemToReturn = allItems[index];
        RemoveFromInventory(itemToReturn);
       return itemToReturn;
    }
    public ItemWeaponWrapper RemoveFromInventory(ItemWeaponWrapper _item)
    {
        allItems.Remove(_item);
        _item.MakePickable();
        return _item;
    }
    public List<ItemWeaponWrapper> GetProperList(ItemWeaponWrapper item)
    {
            return allItems;
    }
    public void EquipFromInventory(ItemWeaponWrapper item)
    {
        if (item == null) return;
        if (toolsController == null) return;
        if (allItems.Contains(item)) //allItems.Remove(item);

        toolsController.EquipItem(item);
    }
    public void ChangeSlot(int slotIncrease)
    {
        if (currentSlot > allItems.Count) currentSlot = 0;
        if (allItems.Count <= 0) return;
        int _previousSlot = currentSlot;
        currentSlot += slotIncrease;
        while (currentSlot >= allItems.Count) currentSlot -= allItems.Count;
        while (currentSlot < 0) currentSlot += allItems.Count;
    }

    public ItemWeaponWrapper GetItemWithMatchingTag(ItemWeaponWrapper item)
    {
        foreach (ItemWeaponWrapper _item in allItems)
            if (_item.ID == item.ID) return _item;
        return null;
    }
    public ItemWeaponWrapper GetNextWeapon()
    {
        if (allItems.Count <= 0) return null;
        ChangeSlot(1);
        ItemWeaponWrapper itemToReturn = allItems[currentSlot];
        //Debug.Log("nextweapon: " + itemToReturn.name);
        return itemToReturn;
    }
    public ItemWeaponWrapper GetPreviousWeapon()
    {
        if (allItems.Count <= 0) return null;
        ChangeSlot(-1);
        ItemWeaponWrapper itemToReturn = allItems[currentSlot];
        //Debug.Log("prev weapon: " + itemToReturn.name);
        return itemToReturn;
    }
    public void DebugListInventory()
    {
        string currentInventory = "CURRENT INVENTORY: ";
        
        foreach (var item in allItems)
        {
            currentInventory+=item.name+", ";
        }
        Debug.Log(currentInventory);
    }
}

