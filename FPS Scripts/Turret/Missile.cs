using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Missile : MonoBehaviour, IDamageable
{
    [Header("Missile Settings")]
    [SerializeField] private float speed = 15;
    [SerializeField] private float startRotSpeed = 1000; //starting rotation speed of missile
    [SerializeField] private float missileLifetime = 3f;
    [SerializeField] private float speedDistanceMult = 0.3f; //multiplier applied to missile rotation speed (how much the player's distance from missile affects it's speed)
    [SerializeField] private float startFollowDelay = 0.1f; //delay before missile starts to follow player

    [Header("Missile Particles")]
    [SerializeField] private ParticleSystem flames = null;
    [SerializeField] private GameObject explosion = null;
    [SerializeField] private Light[] lights = null; //lights attached to missile

    private float currentRotSpeed = 0f;
    private bool slowStarted = false; //if delay has ended
    public Transform target { get; set; }
    private void Awake()
    {
        currentRotSpeed = startRotSpeed;
        Invoke("DestroyMissile", missileLifetime); //destroy missile in x seconds (depends on missile lifetime variable)
        StartCoroutine(StartDelay());
    }
    private void FixedUpdate()
    {
        if (slowStarted) //if starting delay has happened, rotate missile towards player
        {
            currentRotSpeed = Vector3.Distance(transform.position, target.position) * speedDistanceMult;
        }

        GetComponent<Rigidbody>().velocity = transform.forward * speed;

        Quaternion rocketTargetRot = Quaternion.LookRotation(target.position - transform.position);

        GetComponent<Rigidbody>().MoveRotation(Quaternion.RotateTowards(transform.rotation, rocketTargetRot, currentRotSpeed));
    }
    public void TakeDamage(float damage)
    {
        DestroyMissile();
    }
    private void DestroyMissile()
    {
        flames.transform.parent = null; //set parent of missile particles to null so they don't disappear abruptly when missile destroyed
        flames.Stop();

        foreach (Light l in lights)
        {
            if (l != null) // for each light attached to missile, unchild them and slowly dim them to 0 intensity, then destroy
            {
                l.transform.parent = null;
                l.DOIntensity(0, 0.5f);
                Destroy(l.gameObject, 0.55f);
            }
        }

        Instantiate(explosion, transform.position, Quaternion.identity); //create explosion and destroy missile
        Destroy(this.gameObject);
    }
    private IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(startFollowDelay);
        slowStarted = true;
    }
    private void OnDisable()
    {
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        DestroyMissile();
    }
}
