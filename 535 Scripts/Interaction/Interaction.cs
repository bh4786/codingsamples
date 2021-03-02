using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interaction : MonoBehaviour
{
    [Header("Player Camera")]
    [SerializeField] private Camera cam = null;

    [Header("Interaction Parameters")]
    [SerializeField] private LayerMask interactionMask = new LayerMask(); //layer with interaction objects
    [SerializeField] private float interactionDistance = 2f; //max distance player can interact with things
    [SerializeField] private float interactionOutlineWidth = 2f;
    [SerializeField] private Color interactionOutlineColor = Color.white;

    [Header("Interaction Text")]
    [SerializeField] private Text interactPromptText = null; //text that shows when you hover over interactable object
    [SerializeField] private GameObject interactPrompt = null; //all UI associated with hover text

    [Header("Player Sounds")]
    [SerializeField] private PlayerSounds playerSounds = null;

    [Header("Player Inventory")]
    [SerializeField] private Inventory playerInventory = null;

    //private PlayerProgression playerProg;
    private GameObject lastInteracted; //last object player hovered on or interacted with

    private void Start()
    {
        //playerProg = GetComponent<PlayerProgression>();
    }
    void Update()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance, interactionMask)) //if player pointing at object and in range
        {
            lastInteracted = hit.collider.gameObject; //declare last interacted object
            lastInteracted.GetComponent<Outline>().OutlineWidth = interactionOutlineWidth; //update object outline width
            lastInteracted.GetComponent<Outline>().OutlineColor = interactionOutlineColor; //update object outline color

            interactPrompt.SetActive(true); //show hover text
            interactPromptText.text = lastInteracted.GetComponent<Interactable>().identifier; //set hover text to object identifier

            if (Input.GetKeyDown(KeyCode.E))
            {
                IInteractable<GameObject> interactable = lastInteracted.GetComponent<Interactable>().interactableObj.GetComponent<IInteractable<GameObject>>(); //declare interactable-type object
                if (interactable != null) //if it exists
                {
                    interactable.Interact(playerInventory.GetCurrentItem()); //call method on object to interact with
                    if (lastInteracted.CompareTag("Key"))
                    {
                        playerSounds.PickupItem();
                        playerInventory.PickupKey();
                    }
                    else if (lastInteracted.CompareTag("Flashlight"))
                    {
                        playerSounds.PickupItem();
                        playerInventory.PickupFlashlight();
                    }
                }
            }
        }
        else //if player not pointing at object or not in range
        {
            if (lastInteracted != null) //if there was an object the player has looked at before
                lastInteracted.GetComponent<Outline>().OutlineWidth = 0f; //set outline width to zero
            interactPrompt.SetActive(false); //disable hover text
            interactPromptText.text = ""; //clear hover text
        }
    }
}
