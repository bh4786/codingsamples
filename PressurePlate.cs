using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private GameObject activatable = null; //object that the pressure plate activates

    [Header("Plate References")]
    [SerializeField] private Material plateMat = null; //material of pressure plate
    [SerializeField] private Transform plate = null;

    [Header("Plate Settings")]
    [SerializeField] private float plateBottomTolerance = 0.1f; //distance plate must be pushed to activate

    [Header("Audio Clips")]
    [SerializeField] private AudioClip click = null;

    [Header("Lights")]
    [SerializeField] private Light[] buttonLights = null; //lights emitted from plate

    private AudioSource aud;

    private bool isPressed = false;
    private void Start()
    {
        aud = plate.gameObject.GetComponent<AudioSource>();

        plateMat.DisableKeyword("_EMISSION"); //turn off emission and disable lights
        foreach (Light l in buttonLights)
        {
            l.enabled = false;
        }
    }
    private void FixedUpdate()
    {
        if (plate.localPosition.y > 2f) //keep plate from going too high
        {
            plate.localPosition = new Vector3(plate.localPosition.x, 2f, plate.localPosition.z);
        }
        if (plate.localPosition.y < 0.75f) //keep plate from going too low
        {
            plate.localPosition = new Vector3(plate.localPosition.x, 0.75f, plate.localPosition.z);
        }

        if (Mathf.Abs(plate.localPosition.y - 0.75f) < plateBottomTolerance && !isPressed) //if plate pressed
        {
            aud.Stop(); //play click sound at random pitch
            aud.pitch = Random.Range(0.975f, 1.025f);
            aud.PlayOneShot(click);

            isPressed = true; //activate object
            activatable.GetComponent<IActivatable>().Activate(true);

            plateMat.EnableKeyword("_EMISSION"); //enable emission and turn on lights
            foreach (Light l in buttonLights)
            {
                l.enabled = true;
            }
        }
        else if (Mathf.Abs(plate.localPosition.y - 0.75f) > plateBottomTolerance && isPressed) //if plate unpressed
        {
            aud.Stop(); //play click sound at random pitch
            aud.pitch = Random.Range(0.975f, 1.025f);
            aud.PlayOneShot(click);

            isPressed = false; //deactivate object
            activatable.GetComponent<IActivatable>().Activate(false);

            plateMat.DisableKeyword("_EMISSION"); //disable emission and turn off lights
            foreach (Light l in buttonLights)
            {
                l.enabled = false;
            }
        }
    }
}
