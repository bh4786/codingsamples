using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable<GameObject>
{
    [Header("Sound Effects")]
    [SerializeField] private AudioClip openDoor = null;
    [SerializeField] private AudioClip closeDoor = null;
    [SerializeField] private AudioClip lockedDoorAttempt = null;
    [SerializeField] private AudioClip unlockDoor = null;

    [Header("Door Modifiers")]
    public bool needsKey = false;
    public GameObject keyRequired = null;
    [SerializeField] private float audioVolume = 0.5f;
    [SerializeField] private string startingID = null; //what the hover text starts as

    [Header("Interaction Text")]
    [SerializeField] private Interactable itemID = null;

    [SerializeField] private DoorState startingDoorState = DoorState.Closed;

    private Animator anim = null;
    private AudioSource aud;
    private bool usable; //determines when door is ready to be interacted with again

    private enum DoorState
    {
        Open,
        Closed,
        Locked,
    }
    private DoorState doorState;
    // Start is called before the first frame update
    void Start()
    {
        doorState = startingDoorState;
        anim = GetComponent<Animator>();
        aud = GetComponent<AudioSource>();
        itemID.identifier = startingID;
    }
    public void Interact(GameObject g)
    {
        aud.volume = audioVolume; //set volume to starting amount
        switch (doorState)
        {
            case DoorState.Closed:
                if (usable) //open door
                {
                    anim.SetTrigger("Open");
                    doorState = DoorState.Open;
                    itemID.identifier = "Close";
                }
                break;
            case DoorState.Open:
                if (usable) //close door
                {
                    anim.SetTrigger("Close");
                    doorState = DoorState.Closed;
                    itemID.identifier = "Open";
                }
                break;
            case DoorState.Locked:
                if (GameObject.ReferenceEquals(g, keyRequired)) //if player has key required, unlock door
                {
                    usable = true;
                    aud.PlayOneShot(unlockDoor);
                    doorState = DoorState.Closed;
                    itemID.identifier = "Unlocked";                    
                }
                else //play locked sound and show door is locked on hover text
                {
                    if (!aud.isPlaying)
                        aud.PlayOneShot(lockedDoorAttempt);
                    itemID.identifier = "Locked";
                }
                break;
        }
    }
    public IEnumerator DoorEvents()
    {
        usable = false;
        if (doorState == DoorState.Open)
        {
            aud.pitch = Random.Range(0.95f, 1.05f);
            aud.PlayOneShot(openDoor);
            yield return new WaitForSeconds(anim.GetAnimatorTransitionInfo(0).duration); //wait for door to finish opening
            itemID.identifier = "Close";
            usable = true;
        }
        else if (doorState == DoorState.Closed)
        {
            yield return new WaitForSeconds(anim.GetAnimatorTransitionInfo(0).duration); //wait for door to finish closing
            itemID.identifier = "Open";
            usable = true;
            aud.pitch = Random.Range(0.95f, 1.05f);
            aud.PlayOneShot(closeDoor);
        }
    }
}
