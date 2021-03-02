using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject holderPrefab = null; //item that will hold physics object when picked up
    [SerializeField] private float breakForce = 0f;

    private FixedJoint holdJoint = null;
    private GameObject holder = null; //object that will be assigned the holder prefab

    private Vector3 lastPosition;
    private Vector3 velocity;

    private bool isHeld = false;
    private void Start()
    {
        lastPosition = transform.position;
    }
    private void Update()
    {
        velocity = (transform.position - lastPosition) / Time.deltaTime; //finds velocity of object
        lastPosition = transform.position;

        if (holdJoint == null && isHeld) //if other joint is destroyed, drop object
        {
            Drop();
        }
    }
    public void Interact(Transform cam, Vector3 hitPoint)
    {
        if (isHeld)
        {
            Drop();
        }
        else
        {
            isHeld = true;

            holder = Instantiate(holderPrefab, hitPoint, Quaternion.identity); //create object that will hold this object and child it to player camera
            holder.transform.parent = cam;

            holdJoint = holder.AddComponent<FixedJoint>(); //create joint component, set break force, and set connected body to this object's rigidbody
            holdJoint.breakForce = breakForce;
            holdJoint.connectedBody = GetComponent<Rigidbody>();
        }
    }
    private void Drop()
    {
        isHeld = false;

        Destroy(holdJoint); //destroy holder and its joint
        Destroy(holder);

        holder = null;

        GetComponent<Rigidbody>().velocity = velocity / 5f; //set velocity to the clamped velocity when object was held
    }
}
