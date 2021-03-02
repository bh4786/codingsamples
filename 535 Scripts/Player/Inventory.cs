using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] private Animator invAnim = null;
    [SerializeField] private float scrollDelay = 0.5f; //minimum time in between each item switch
    [SerializeField] private bool[] hasItem = { true, false, false }; //list of bool for each item depending on if player has it or not
    [SerializeField] private GameObject flashlightIcon = null;
    [SerializeField] private GameObject keyIcon = null;
    [SerializeField] private ItemText itemText = null;

    public GameObject[] items = null;

    private int currentItem = 0; //current item in items list
    private float scrollTimer = 0f; 
    void Start()
    {
        flashlightIcon.SetActive(false);
        keyIcon.SetActive(false);
    }
    void Update()
    {
        scrollTimer += Time.deltaTime;
        if (Input.GetAxis("Mouse ScrollWheel") > 0f && scrollTimer > scrollDelay) //if scrolling up and scroll delay is done
        {
            scrollTimer = 0f; //reset timer
            while (true) //check through list until there is an item that the player has
            {
                if (currentItem == 0) //if at first item, go to last item
                {
                    currentItem = 2;
                    if (hasItem[currentItem]) //check if has item
                        break;
                }
                else //...or else go up 1 item
                {
                    currentItem--;
                    if (hasItem[currentItem]) //check if has item
                        break;
                }
            }
            invAnim.SetInteger("InvPos", currentItem); //update inventory UI
            foreach (GameObject g in items) 
                g.SetActive(false); //disable all inventory items
            items[currentItem].SetActive(true); //enable current item
            itemText.ShowText();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f && scrollTimer > scrollDelay) //if scrolling down and scroll delay is done
        {
            scrollTimer = 0f; //reset timer
            while (true) //check through list until there is an item that the player has
            {
                if (currentItem == 2) //if at last item, go to first item...
                {
                    currentItem = 0;
                    if (hasItem[currentItem]) //check if has item
                        break;
                }
                else //...or else go down 1 item
                {
                    currentItem++;
                    if (hasItem[currentItem]) //check if has item
                        break;
                }
            }
            invAnim.SetInteger("InvPos", currentItem); //update inventory UI
            foreach (GameObject g in items) 
                g.SetActive(false); //disable all inventory items
            items[currentItem].SetActive(true); //enable current item
            itemText.ShowText();
        }
    }
    public GameObject GetCurrentItem()
    {
        return items[currentItem];
    }
    public void PickupFlashlight() //adds flashlight to inventory and updates UI accordingly
    {
        hasItem[1] = true;
        flashlightIcon.SetActive(true);
    }
    public void PickupKey() //adds key to inventory and updates UI accordingly
    {
        hasItem[2] = true;
        keyIcon.SetActive(true);
    }
}
