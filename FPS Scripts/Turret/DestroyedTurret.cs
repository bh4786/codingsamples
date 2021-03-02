using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DestroyedTurret : MonoBehaviour
{
    [Header("Destroyed Turret References")]
    [SerializeField] private Material turretMat = null; //transparent turret material
    [SerializeField] private GameObject matObj = null; //object with material attached

    [Header("Destroyed Turret Settings")]
    [SerializeField] private float explosionForce = 30000f;
    [SerializeField] private float explosionSpread = 3f; //distance explosion will throw turret head on x and z axises'

    public GameObject turretHead;

    private void Start()
    {
        turretHead.transform.parent = null; //separate turret head and apply force to it
        turretHead.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, new Vector3(transform.position.x + Random.Range(-explosionSpread, explosionSpread), transform.position.y + 
            Random.Range(-explosionSpread, explosionSpread), transform.position.z + Random.Range(-explosionSpread, explosionSpread)), 10f, 2f);

        Invoke(nameof(DestroyTurret), 3f); //start timer to destroy turret
    }
    private void DestroyTurret()
    {
        matObj.GetComponent<Renderer>().material = new Material(turretMat); //set turret to transparent material and tween to completely transparent
        matObj.GetComponent<Renderer>().material.DOColor(new Color(1, 1, 1, 0), 1f);

        Destroy(turretHead, 1f); //destroy turret head and body
        Destroy(this.gameObject, 1f);
    }
}
