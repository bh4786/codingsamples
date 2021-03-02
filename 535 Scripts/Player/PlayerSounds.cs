using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    [Header("Player Sound Clips")]
    //[SerializeField] private AudioClip[] footsteps = null;
    [SerializeField] private AudioClip landing = null;
    [SerializeField] private AudioClip jump = null;

    [SerializeField] private AudioClip[] carpetFootstepSounds = null;
    [SerializeField] private AudioClip[] tileFootstepSounds = null;
    [SerializeField] private AudioClip[] hardwoodFootstepSounds = null;

    [Header("Pickup Sound Clips")]
    [SerializeField] private AudioClip pickupItem = null;

    [SerializeField] private GroundMaterialCheck groundMatCheck = null;

    private GameObject player = null;

    private float stepCoolDown; //current time between steps
    private float stepRate = 0.58f; //time set to wait between steps
    private AudioSource aud;
    private PlayerMovement playerMovement;
    private CharacterController playerController;

    private AudioClip[] currentStepSounds;
    void Start()
    {
        aud = GetComponent<AudioSource>();
        player = transform.root.gameObject;
        playerMovement = player.GetComponent<PlayerMovement>(); //set player to the root of this object
        playerController = player.GetComponent<CharacterController>();
    }
    void Update()
    {
        stepCoolDown -= Time.deltaTime; //step timer countdown
        //check if player moving, grounded, and timer at/below zero
        if ((Mathf.Abs(Input.GetAxis("Horizontal")) > 0f || Mathf.Abs(Input.GetAxis("Vertical")) > 0f) && stepCoolDown < 0f && playerMovement.GetGrounded() && playerController.velocity.magnitude > 0.05f)
        {
            stepRate = 1 - playerController.velocity.magnitude / 6f;
            aud.pitch = 1f + Random.Range(-0.15f, 0.15f); //randomize pitch of footsteps
            switch (groundMatCheck.GetGroundMaterial())
            {
                case "Carpet":
                    currentStepSounds = carpetFootstepSounds;
                    break;
                case "Tile":
                    currentStepSounds = tileFootstepSounds;
                    break;
                case "Hardwood":
                    currentStepSounds = hardwoodFootstepSounds;
                    break;
            }
            aud.PlayOneShot(currentStepSounds[Random.Range(0, currentStepSounds.Length)], 0.07f); //play random footstep sound effect from array
            stepCoolDown = stepRate; //reset step timer
        }
    }
    public void Jump()
    {
        aud.PlayOneShot(jump, 0.5f);
    }
    public void Land()
    {
        aud.PlayOneShot(landing, 0.7f);
    }
    public void PickupItem()
    {
        aud.PlayOneShot(pickupItem);
    }
}
