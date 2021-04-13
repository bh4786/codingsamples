using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScrapMachine : MonoBehaviour, IInteractable
{
    [SerializeField] private AudioClip purchase = null;

    [SerializeField] private PlayerMovement playerMovement = null;
    [SerializeField] private GameObject mainCanvas = null;

    [SerializeField] private InputManager inputManager = null;
    [SerializeField] private Camera mainCam = null;
    [SerializeField] private Transform focusPos = null;
    [SerializeField] private GameObject zoomCam = null;

    [SerializeField] private int keyPrice = 10000;
    [SerializeField] private int upgradePrice = 5000;
    [SerializeField] private Text scrapBalanceText = null;
    [SerializeField] private Text keyPriceText = null;
    [SerializeField] private Text upgradePriceText = null;
    [SerializeField] private float upgradeMultiplier = 1.5f;

    public Inventory inventory { get; set; }
    public Gun gun { get; set; }

    private bool isFocused = false;
    private AudioSource aud;

    private void Start()
    {
        aud = GetComponent<AudioSource>();

        scrapBalanceText.text = 0.ToString();
        keyPriceText.text = keyPrice.ToString();
        upgradePriceText.text = upgradePrice.ToString();
    }
    public void Interact(Inventory i)
    {
        if (!isFocused)
        {
            isFocused = true;

            inputManager.SetInputMode(ScreenInputMode.Virtual);
            playerMovement.ToggleMovement(false);

            mainCanvas.SetActive(false);
            mainCam.enabled = false;
            zoomCam.GetComponent<Camera>().enabled = true;
            zoomCam.GetComponent<ScreenZoomCam>().Focus(mainCam.transform, focusPos);

            UpdateUI();
        }
    }
    private void UpdateUI()
    {
        scrapBalanceText.DOText(inventory.GetItemCount("Scrap").ToString(), 0.5f, false, ScrambleMode.Numerals, "0123456789");
        keyPriceText.text = keyPrice.ToString();
        upgradePriceText.text = upgradePrice.ToString();
    }
    public void BuyKey()
    {
        if (inventory.GetItemCount("Scrap") >= keyPrice)
        {
            Key keyItem = new Key("Key", 1);
            inventory.AddItem(keyItem);
            inventory.RemoveItem("Scrap", keyPrice);
            aud.PlayOneShot(purchase);
            UpdateUI();
        }
    }
    public void UpgradeGun()
    {
        if (inventory.GetItemCount("Scrap") >= upgradePrice)
        {
            inventory.RemoveItem("Scrap", upgradePrice);
            aud.PlayOneShot(purchase);
            gun.UpgradeWeapon(upgradeMultiplier);
            UpdateUI();
        }       
    }
    public void ZoomOut()
    {
        if (isFocused)
        {
            zoomCam.GetComponent<ScreenZoomCam>().Unfocus(zoomCam.transform, mainCam.transform);
            inputManager.SetInputMode(ScreenInputMode.Standalone);

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked; //lock cursor and set it to invisible    

            StartCoroutine(ExitScreen());
        }
    }
    private IEnumerator ExitScreen()
    {
        yield return new WaitForSeconds(zoomCam.GetComponent<ScreenZoomCam>().zoomDuration);    
        
        playerMovement.ToggleMovement(true);
        mainCanvas.SetActive(true);

        zoomCam.GetComponent<Camera>().enabled = false;
        mainCam.enabled = true;

        isFocused = false;
    }
}
