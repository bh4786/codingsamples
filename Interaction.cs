using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    [SerializeField] private float interactionDistance = 4f; //maximum distance player can interact with interactable objects
    [SerializeField] private Camera cam = null;
    [SerializeField] private LayerMask interactionLayer = new LayerMask(); //layer of interactable objects
    void Update()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); //shoot ray from center of camera
        RaycastHit hit;

        if (Input.GetButtonDown("Interact")) //if player presses interact button, raycast from cam and call interact function if object has interactable script
        {
            if (Physics.Raycast(ray, out hit, interactionDistance, interactionLayer))
            {
                hit.transform.gameObject.GetComponent<IInteractable>().Interact(cam.transform, hit.point);
            }
        }
    }
}
