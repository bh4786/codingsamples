using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PushButton : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject activatable = null;

    [Header("Button Settings")]
    [SerializeField] private float pressDuration = 0.3f;
    [SerializeField] private float pressDistanceMult = 0.125f; //how far button is pushed

    [Header("Button References")]
    [SerializeField] private Transform plate = null;
    [SerializeField] private Material plateMat = null; //plate material
    [SerializeField] private Light redLight = null; //light emitted from button
    [SerializeField] private AudioClip click = null;

    private float startY = 0f;
    private bool isPressed = false;
    private AudioSource aud = null;
    private void Start()
    {
        aud = GetComponent<AudioSource>();

        startY = plate.localPosition.y;

        plate.gameObject.GetComponent<Renderer>().material = new Material(plateMat); //disable emission and light
        plate.gameObject.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
        redLight.enabled = false;
    }
    public void Interact(Transform itemHolder, Vector3 hitPoint)
    {
        if (!isPressed)
        {
            aud.pitch = Random.Range(0.95f, 1.05f); //play click sound at random pitch
            aud.PlayOneShot(click);

            isPressed = true; //press plate down and activate emission and light
            plate.DOLocalMoveY(startY * pressDistanceMult, pressDuration);
            plate.gameObject.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
            redLight.enabled = true;

            if (activatable != null)
                activatable.GetComponent<IActivatable>().Activate(true); //activate object
        }
        else
        {
            aud.pitch = Random.Range(0.95f, 1.05f); //play click sound at random pitch
            aud.PlayOneShot(click);

            isPressed = false; //press plate up and deactivate emission and light
            plate.DOLocalMoveY(startY, pressDuration);
            plate.gameObject.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
            redLight.enabled = false;

            if (activatable != null)
                activatable.GetComponent<IActivatable>().Activate(false); //deactivate object
        }
    }
}
