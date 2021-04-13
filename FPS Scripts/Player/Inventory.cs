using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Inventory : MonoBehaviour
{
    [SerializeField] private Text scrapText = null; //ui for scrap amount
    [SerializeField] private Text keyText = null; //ui for key amount
    [SerializeField] private List<ItemDisplay> itemDisplays = new List<ItemDisplay>(); //list for item uis

    private List<Item> items = new List<Item>(); //list for items

    void Start()
    {
        //set balances to zero
        scrapText.text = 0.ToString();
        keyText.text = 0.ToString();
    }
    private void DisplayItemCount(Item item, int itemCount)
    {
        foreach (ItemDisplay id in itemDisplays) //for each item display, update amounts
        {
            if (id.type.Equals(item.type))
            {
                id.DisplayCount(itemCount);
            }
        }
    }
    public void AddItem(Item item)
    {
        bool isNew = true;
        if (items != null)
        {
            foreach (Item i in items)
            {
                if (i.type.Equals(item.type)) //if item exists, just add to its amount
                {
                    i.amount += item.amount;
                    DisplayItemCount(i, i.amount);
                    isNew = false;
                }
            }
            if (isNew) //if new item, make new item in item list and set its amount to amount added
            {
                items.Add(item);
                DisplayItemCount(item, item.amount);
            }
        }
        else
        {
            items.Add(item);
            DisplayItemCount(item, item.amount);
        }
    }
    public void RemoveItem(string itemName, int itemCount)
    {
        foreach(Item i in items) //check for specific item
        {
            if (i.type.Equals(itemName))
            {
                i.amount -= itemCount; //remove set amount of items
                DisplayItemCount(i, i.amount);
            }
        }
    }
    public int GetItemCount(string itemName)
    {
        foreach(Item i in items)
        {
            if (i.type.Equals(itemName))
            {
                return i.amount; //return specific item count
            }
        }
        return 0;
    }
}
