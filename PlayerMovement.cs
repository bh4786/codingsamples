using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerCam = null;
    [SerializeField] private Transform orientation = null; //where player is looking (y axis)
    [SerializeField] private Animator camAnim = null; //animator attached to camera
    [SerializeField] private PlayerSounds playerSounds = null; //object controlling the player sound effects
    [SerializeField] private Animator gunAnim = null; //animator attached to the gun
    [SerializeField] private HeadCheck headCheck = null; //checks if player can stand
    [SerializeField] private Rigidbody rb = null;

    [Header("Movement Settings")]
    [SerializeField] private float baseSpeed = 12f;
    [SerializeField] private float runSpeed = 3.45f;
    [SerializeField] private float crouchSpeed = 1f;
    [SerializeField] private float forceMult = 100f; //force muliplier added to increase acceleration of player 
    [SerializeField] private float maxGroundedVelocityChange = 100f; //max amount of force that can be applied to change player velocity
    [SerializeField] private float maxAirborneVelocityChange = 25f; //max amount of force that can be applied to change player velocity while airborne
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float airborneSpeedMult = 3f; //speed player can move at while airborne
    [SerializeField] private float clampedAirborneMagnitude = 4f; //maximum additional velocity applied while airborne

    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheck = null; //origin of raycast to floor
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask = new LayerMask(); //what layer to check for when doing ground check
    [SerializeField] private float landVelThresh = 6f; //how fast the player has to fall to trigger the landing sound

    [Header("Dash Settings")]
    [SerializeField] private float dashMult = 10f; //strength of dash
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashDragMult = 3f; //how much player is slowed down after dashing
    [SerializeField] private float dashDelay = 2f; //how often player can dash
    [SerializeField] private Slider dashBar = null;

    [Header("Crouch Settings")]
    [SerializeField] private float crouchDuration = 0.25f; //time it takes to crouch and uncrouch
    [SerializeField] private float crouchBobSpeed = 0.5f; //multiplier applied to walking animation
    [SerializeField] private float crouchHeightDivisor = 1.5f; //number that the player's height is divided by when crouching

    private float currentSpeed = 1.5f;
    private float startingYScale = 0f; //starting height of player
    private float maxVelocityChange = 100f;

    private float xRotation; //player's camera x rotation target
    private float sensitivity = 50f;
    private float sensMultiplier = 1f;
    private float desiredY; //target of player y orientation

    private Vector3 movement; //movement vector

    public enum PlayerState { Default, Sprinting, Crouching, Airborne }
    private PlayerState playerState = PlayerState.Default;

    private bool canMove;
    private bool canDash;

    void Start()
    {
        canMove = true;

        currentSpeed = baseSpeed;
        maxVelocityChange = maxGroundedVelocityChange;
        startingYScale = transform.localScale.y;
        dashBar.value = 1f;      
    }
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (playerState == PlayerState.Crouching)
            {
                ToggleCrouch();
            }
            else if (playerState == PlayerState.Default || playerState == PlayerState.Sprinting)
            {
                ToggleSprint(false);
                Jump();
            }
            else if (playerState == PlayerState.Airborne && (Input.GetAxis("Horizontal") != 0f || Input.GetAxis("Vertical") != 0f) && canDash && dashBar.value == 1f) //if airborne and moving, and can dash, then dash
                Dash();
        }
        if (Input.GetButton("Sprint") && playerState == PlayerState.Default && movement != Vector3.zero)
        {
            ToggleSprint(true);
        }
        if (Input.GetButtonDown("Sprint") && playerState == PlayerState.Crouching)
        {
            ToggleCrouch();
        }
        else if (playerState == PlayerState.Sprinting && movement == Vector3.zero)
        {
            ToggleSprint(false);
        }
        else if (playerState == PlayerState.Airborne && gunAnim.GetBool("Sprinting"))
        {
            gunAnim.SetBool("Sprinting", false); //change gun and camera animations to normal moving animations
            camAnim.SetBool("Sprint", false);
        }
        else if (playerState == PlayerState.Sprinting && !gunAnim.GetBool("Sprinting"))
        {
            gunAnim.SetBool("Sprinting", true); //change gun and camera animations to sprinting animations
            camAnim.SetBool("Sprint", true);
        }

        if (Input.GetButtonUp("Sprint") && playerState != PlayerState.Crouching)
        {
            ToggleSprint(false);
        }
        if (Input.GetButtonDown("Crouch"))
        {
            ToggleCrouch();
        }
    }
    private void FixedUpdate()
    {
        Movement();
        if (Physics.CheckSphere(groundCheck.position, groundDistance, groundMask) && playerState == PlayerState.Airborne) //check if player is grounded
        {
            playerState = PlayerState.Default;
            canMove = true;
            maxVelocityChange = maxGroundedVelocityChange;
            if (Mathf.Abs(rb.velocity.y) >= landVelThresh) //if player hits ground hard enough
            {
                camAnim.SetTrigger("Land"); //trigger landing camera animation
                playerSounds.Land();
            }
        }
        else if (!Physics.CheckSphere(groundCheck.position, groundDistance, groundMask) && playerState != PlayerState.Airborne) //check if player is not grounded
        {
            playerState = PlayerState.Airborne;
        }
    }
    private void LateUpdate()
    {
        Look();
    }
    private void ToggleSprint(bool sprint)
    {
        if (sprint)
        {
            playerState = PlayerState.Sprinting;
            currentSpeed = runSpeed;

            gunAnim.SetBool("Sprinting", true); //change camera and gun animations
            gunAnim.SetBool("Moving", true);
            camAnim.SetBool("Sprint", true);
        }
        else
        {
            playerState = PlayerState.Default;
            currentSpeed = baseSpeed;

            gunAnim.SetBool("Sprinting", false); //change camera and gun animations
            gunAnim.SetBool("Moving", true);
            camAnim.SetBool("Sprint", false);
        }
    }
    private void ToggleCrouch()
    {
        if (playerState == PlayerState.Default)
        {
            ToggleSprint(false);
            playerState = PlayerState.Crouching;
            currentSpeed = crouchSpeed;
            gunAnim.SetFloat("BobSpeed", crouchBobSpeed); //slow down walking animation
            transform.DOScaleY(startingYScale / crouchHeightDivisor, crouchDuration); //compress player scale on y axis            
        }
        else if (playerState == PlayerState.Crouching && headCheck.canStand)
        {
            playerState = PlayerState.Default;
            currentSpeed = baseSpeed;
            gunAnim.SetFloat("BobSpeed", 1f); //speed up walking animation
            transform.DOScaleY(startingYScale, crouchDuration); //expand player scale on y axis            
        }        
    }
    private void Jump()
    {
        canDash = true;
        maxVelocityChange = maxAirborneVelocityChange;
        camAnim.SetTrigger("Jump");
        rb.AddForce(transform.up * jumpHeight, ForceMode.Acceleration); //use player velocity to apply force upwards
    }
    private void Dash()
    {
        canDash = false;
        canMove = false;

        dashBar.value = 0f;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        rb.velocity = Vector3.zero; //disable gravity, stop player movement and apply impulse force to player in desired direction
        rb.useGravity = false;
        rb.AddForce((orientation.right * x + orientation.forward * z).normalized * dashMult, ForceMode.Impulse); 

        playerSounds.Dash(); //play sound effect, animations, and start filling dash bar
        camAnim.SetTrigger("Dash");
        dashBar.DOValue(1f, dashDelay);

        Invoke("ReenableGravity", dashDuration); //reenable gravity after set amount of time
    }
    private void ReenableGravity()
    {
        rb.useGravity = true; //reenable gravity and slow player down so they don't retain dash speed
        rb.velocity /= dashDragMult;
        canMove = true;
    }
    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier;

        Vector3 rot = playerCam.transform.localRotation.eulerAngles; //find current look rotation
        desiredY = rot.y + mouseX;

        xRotation -= mouseY; //rotate player without rotating to much or too little
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredY, 0); //apply rotations
        orientation.transform.localRotation = Quaternion.Euler(0, desiredY, 0);
    }
    private void Movement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        movement = orientation.right * x + orientation.forward * z; //set movement vector to desired direction and clamp so player can't move faster when going diagonally
        movement = Vector3.ClampMagnitude(movement, 1f);

        Vector3 targetVelocity = movement * currentSpeed * 100f * Time.deltaTime; //set force relative to current player velocity
        Vector3 force = (targetVelocity - rb.velocity) * forceMult;

        force.x = Mathf.Clamp(force.x, -maxVelocityChange, maxVelocityChange); //clamp force so it doesn't overcorrect
        force.z = Mathf.Clamp(force.z, -maxVelocityChange, maxVelocityChange);
        force.y = 0;

        if (canMove && playerState != PlayerState.Airborne) //apply normal acceleration force to move player
        {
            rb.AddForce(new Vector3(force.x, 0f, force.z), ForceMode.Acceleration);
        }
        else if (canMove && playerState == PlayerState.Airborne) //apply dampened acceleration when player is airborne
        {
            Vector3 airborneForce = movement * currentSpeed * airborneSpeedMult;
            airborneForce = Vector3.ClampMagnitude(airborneForce, clampedAirborneMagnitude);
            rb.AddForce(airborneForce, ForceMode.Acceleration);
        }
        if (movement == Vector3.zero && gunAnim.GetBool("Moving"))
        {
            gunAnim.SetBool("Moving", false);
        }
        else if (movement != Vector3.zero && !gunAnim.GetBool("Moving"))
        {
            gunAnim.SetBool("Moving", true);
        }
    }
}
