using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScrapMachine : MonoBehaviour, IInteractable
{
    [SerializeField] private AudioClip purchase = null; //sound played when item is bought

    [SerializeField] private PlayerMovement playerMovement = null;
    [SerializeField] private GameObject mainCanvas = null; //main canvas for ui when not using machine

    [SerializeField] private InputManager inputManager = null;
    [SerializeField] private Camera mainCam = null;
    [SerializeField] private Transform focusPos = null; //position camera will tween to
    [SerializeField] private GameObject zoomCam = null; //camera for scrap machine

    [SerializeField] private int keyPrice = 10000;
    [SerializeField] private int upgradePrice = 5000;
    [SerializeField] private Text scrapBalanceText = null;
    [SerializeField] private Text keyPriceText = null;
    [SerializeField] private Text upgradePriceText = null;
    [SerializeField] private float upgradeMultiplier = 1.5f; //multiplier for upgrading gun stats

    public Inventory inventory { get; set; }
    public Gun gun { get; set; }

    private bool isFocused = false; //if player is focused on machine screen
    private AudioSource aud;

    private void Start()
    {
        aud = GetComponent<AudioSource>();

        //set ui on screen to starting amounts
        scrapBalanceText.text = 0.ToString();
        keyPriceText.text = keyPrice.ToString();
        upgradePriceText.text = upgradePrice.ToString();
    }
    public void Interact(Inventory i)
    {
        if (!isFocused)
        {
            isFocused = true;

            inputManager.SetInputMode(ScreenInputMode.Virtual); //set input mode to virtual screen
            playerMovement.ToggleMovement(false); //stop accepting input for movement

            mainCanvas.SetActive(false); //disable ui
            mainCam.enabled = false; //disable camera
            zoomCam.GetComponent<Camera>().enabled = true; //enable machine screen camera
            zoomCam.GetComponent<ScreenZoomCam>().Focus(mainCam.transform, focusPos); //start camera tween to focus position

            UpdateUI();
        }
    }
    private void UpdateUI()
    {
        scrapBalanceText.DOText(inventory.GetItemCount("Scrap").ToString(), 0.5f, false, ScrambleMode.Numerals, "0123456789"); //tween ui balance value to new value
        keyPriceText.text = keyPrice.ToString(); //update key price
        upgradePriceText.text = upgradePrice.ToString(); //update upgrade price
    }
    public void BuyKey()
    {
        if (inventory.GetItemCount("Scrap") >= keyPrice) //if player has enough scrap
        {
            Key keyItem = new Key("Key", 1); //create new key item
            inventory.AddItem(keyItem); //add key to inventory
            inventory.RemoveItem("Scrap", keyPrice); //remove scrap from inventory
            aud.PlayOneShot(purchase);
            UpdateUI();
        }
    }
    public void UpgradeGun()
    {
        if (inventory.GetItemCount("Scrap") >= upgradePrice) //if player has enough scrap
        {
            inventory.RemoveItem("Scrap", upgradePrice); //remove scrap from inventory
            aud.PlayOneShot(purchase);
            gun.UpgradeWeapon(upgradeMultiplier); //upgrade gun by multiplier
            UpdateUI();
        }       
    }
    public void ZoomOut()
    {
        if (isFocused)
        {
            zoomCam.GetComponent<ScreenZoomCam>().Unfocus(zoomCam.transform, mainCam.transform); //tween camera to original player camera position
            inputManager.SetInputMode(ScreenInputMode.Standalone); //set input mode to normal mode

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked; //lock cursor and set it to invisible    

            StartCoroutine(ExitScreen());
        }
    }
    private IEnumerator ExitScreen()
    {
        yield return new WaitForSeconds(zoomCam.GetComponent<ScreenZoomCam>().zoomDuration); //wait
        
        playerMovement.ToggleMovement(true); //enable movement
        mainCanvas.SetActive(true); //enable main ui

        zoomCam.GetComponent<Camera>().enabled = false; //disable zoom camera
        mainCam.enabled = true; //enable main camera

        isFocused = false;
    }
}
