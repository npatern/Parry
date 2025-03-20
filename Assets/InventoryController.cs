using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField]
    int maxSlotsNr = int.MaxValue;
    [SerializeField]
    int maxBigSlotsNr = 1;
    [SerializeReference]
    public List<ItemWeaponWrapper> slots = new List<ItemWeaponWrapper>();
    public List<ItemWeaponWrapper> bigSlots = new List<ItemWeaponWrapper>();
    public ToolsController toolsController;

   
    private void Awake()
    {
        toolsController = GetComponent<ToolsController>();
    }
    private void FixedUpdate()
    {
        
    }
    //public ItemWeaponWrapper handSlot;
    public bool AddToInventory(ItemWeaponWrapper item)
    {
        Debug.Log("start Add to inventory");
        ListInventory();
        if (item == null) return false;
        List<ItemWeaponWrapper> list;
        int maxNr = maxSlotsNr;
        list = slots;
        /*
        if (item.Big)
        {
            maxNr = maxBigSlotsNr;
            list = bigSlots;
        }
        else
        {
            list = slots;
        }
         
        if (list.Contains(item))
        {
            if (item.Stackable)
            {
                int index = list.IndexOf(item);
                list[index].stack += item.stack;
                return true;
            }
            else
            {
                return false;
            }       
        }  
        if (list.Count >= maxSlotsNr) return false;
        */
        list.Add(item);
        item.DestroyPhysicalPresence();
        Debug.Log("End Add to inventory");
        ListInventory();
        return true;
    }
    public ItemWeaponWrapper RemoveFromInventory(int index = 0)
    {
        if (slots.Count <= index) return null;
        ItemWeaponWrapper itemToReturn = slots[index];
        slots.RemoveAt(index);
        itemToReturn.MakePickable();
        return itemToReturn;
    }
    public List<ItemWeaponWrapper> GetProperList(ItemWeaponWrapper item)
    {
        if (item.Big)
            return bigSlots;
        else
            return slots;
    }
    public void Equip(ItemWeaponWrapper item)
    {
        if (item == null) return;
        if (toolsController == null) return;
        if (toolsController.EquipWeapon(item))
            if (slots.Contains(item)) slots.Remove(item);
    }
    void RemoveFromList(List<ItemWeaponWrapper> list, ItemWeaponWrapper item)
    {
        list.Remove(item);
    }
    public ItemWeaponWrapper GetNextWeapon()
    {
        if (slots.Count <= 0) return null;
        ItemWeaponWrapper itemToReturn = slots[0];
        Debug.Log("nextweapon: " + itemToReturn.name);
        return itemToReturn;
    }
    public ItemWeaponWrapper GetPreviousWeapon()
    {
        if (slots.Count <= 0) return null;
        int slotNr = slots.Count-1;
        ItemWeaponWrapper itemToReturn = slots[slotNr];
        Debug.Log("prev weapon: " + itemToReturn.name);
        return itemToReturn;
    }
    public void ListInventory()
    {
        string currentInventory = "CURRENT INVENTORY: ";
        
        foreach (var item in slots)
        {
            currentInventory+=item.name+", ";
        }
        Debug.Log(currentInventory);
    }
}

