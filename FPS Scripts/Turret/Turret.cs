using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Turret : MonoBehaviour, IDamageable, IActivatable
{
    [Header("Turret Transform References")]
    [SerializeField] private Transform player = null; //player body
    [SerializeField] private Transform turretXObj = null; //turret part that rotates to follow player on x and z axises
    [SerializeField] private Transform barrelTip = null; //tip of turret, where missile will be created

    [Header("Turret Visuals/Audio")]
    [SerializeField] private ParticleSystem shootSmoke = null; //smoke when missile is fired
    [SerializeField] private GameObject missilePrefab = null;
    [SerializeField] private AudioClip rocketLaunch = null; //rocket firing sound effect
    [SerializeField] private ParticleSystem damageSmoke = null; //smoke created when turret is damaged
    [SerializeField] private LineRenderer laser = null;
    [SerializeField] private GameObject laserLight = null;

    [Header("Turret Settings")]
    [SerializeField] private float fireRate = 3f;
    [SerializeField] private float startHealth = 100f;
    [SerializeField] private float targetingMinDistance = 5f; //minimum distance player has to be to be able to fire, keeps turret from damaging self
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float distanceMult = 5f; //how much the players distance from turret affects its rotation speed

    [Header("Destroyed Turret References")]
    [SerializeField] private GameObject destroyedTurret = null; //separated turret
    [SerializeField] private GameObject explosion = null;

    [SerializeField] private bool activated = true;

    private bool alive = true;
    private bool canSeeTarget = false;

    private float shootTimer = 0f;
    private float currentHealth;

    private AudioSource aud;
    private GameObject laserLightObj; //light object at laser's hit point
    private ParticleSystem.MainModule main;
    private void Start()
    {
        aud = GetComponent<AudioSource>();
        currentHealth = startHealth;

        fireRate += Random.Range(-0.15f, 0.15f); //Randomizes fire rate so turrets don't fire at same time

        main = damageSmoke.main;
        main.startColor = new Color(main.startColor.color.r, main.startColor.color.g, main.startColor.color.b, 0f); //set damage smoke color alpha to 0
    }
    private void Update()
    {
        if (activated && alive)
        {
            
            shootTimer += Time.deltaTime;
            Vector3 direction = new Vector3(player.position.x, transform.position.y, player.position.z) - transform.position;
            Quaternion toRotation = Quaternion.LookRotation(direction);
            GetComponent<Rigidbody>().MoveRotation(Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed / (Vector3.Distance(player.position, transform.position) * distanceMult) * Time.deltaTime));

            float clampedY = 0f;
            if (player.position.y < transform.position.y) //keeps turret from aiming below its own height and colliding with ground
            {
                clampedY = transform.position.y;
            }
            else
            {
                clampedY = player.position.y;
            }
            Vector3 direction1 = new Vector3(player.position.x, clampedY, player.position.z) - turretXObj.position;
            Quaternion toRotation1 = Quaternion.LookRotation(direction1);

            //rotate head towards target using target's distance to affect speed (farther target = slower rotation)
            turretXObj.gameObject.GetComponent<Rigidbody>().MoveRotation(Quaternion.RotateTowards(turretXObj.rotation, toRotation1, rotationSpeed / (Vector3.Distance(player.position, transform.position) * distanceMult) * Time.deltaTime));

            RaycastHit hit;
            if (Physics.Raycast(laser.transform.position, laser.transform.forward, out hit) && hit.transform != null) //if ray hits something, check if player, otherwise create light object where ray hits and set line renderer position to hit point
            {
                if (hit.transform == player && !canSeeTarget)
                {
                    canSeeTarget = true;
                }
                else if (hit.transform != player && canSeeTarget)
                {
                    canSeeTarget = false;
                }
                if (laserLightObj == null)
                {
                    laserLightObj = Instantiate(laserLight, hit.point, Quaternion.identity);
                }
                else
                {
                    laserLightObj.transform.position = hit.point;
                    laserLightObj.transform.LookAt(laser.transform.position);
                }
                laser.SetPosition(0, laser.transform.position);
                laser.SetPosition(1, laserLightObj.transform.position);
            }
            else //if ray doesn't hit anything, set line renderer position and laser light to 100 units from laser origin
            {
                if (canSeeTarget)
                {
                    canSeeTarget = false;
                }
                if (laserLightObj == null)
                {
                    laserLightObj = Instantiate(laserLight, laser.transform.position + laser.transform.forward * 100f, Quaternion.identity);
                }
                else
                {
                    laserLightObj.transform.position = laser.transform.position + laser.transform.forward * 100f;
                    laserLightObj.transform.rotation = Quaternion.Euler(-(laser.transform.position + laser.transform.forward * 100f - laser.transform.position));
                }
                laser.SetPosition(0, laser.transform.position);
                laser.SetPosition(1, laserLightObj.transform.position);
            }
            
            if (shootTimer >= fireRate && Vector3.Distance(transform.position, player.position) > targetingMinDistance && canSeeTarget) //if turret can fire and player far away enough and can see player
            {
                shootTimer = 0f;

                shootSmoke.Play();

                GameObject tempMissile = Instantiate(missilePrefab, barrelTip.position, Quaternion.identity); //create missile, set target to player, and adjust initial rotation
                tempMissile.GetComponent<Missile>().target = player;
                tempMissile.transform.rotation = Quaternion.LookRotation(barrelTip.transform.up);// Quaternion.LookRotation(player.position - transform.position);
                tempMissile.transform.parent = null;

                aud.PlayOneShot(rocketLaunch);
            }
            else if (Vector3.Distance(transform.position, player.position) <= targetingMinDistance && shootTimer > 0f) //reset shoot timer until player gets outside range (keeps turret from shooting immediately if player gets outside safe range)
            {
                shootTimer = 0f;
            }
        }
        //main = damageSmoke.main;
        main.startColor = new Color(main.startColor.color.r, main.startColor.color.g, main.startColor.color.b, 1f - currentHealth / startHealth); //set damage smoke alpha to correspond to turret health
    }
    public void TakeDamage(float damage)
    {
        currentHealth -= damage; //damage turret using amount given from damaging object
        if (currentHealth <= 0)
        {
            Die();
        }
        if (!activated)
        {
            activated = true;
        }
    }
    public void Activate(bool activate)
    {
        activated = !activated; //toggle activation of turret
    }
    private void Die()
    {
        if (alive)
        {
            activated = false;
            alive = false;

            GameObject deadTurret = Instantiate(destroyedTurret, transform.position, transform.rotation); //instatiate separated turret at original turret position and instatiate explosion
            Instantiate(explosion, turretXObj.transform.position, transform.rotation);
            deadTurret.GetComponent<DestroyedTurret>().turretHead.transform.position = turretXObj.transform.position;
            deadTurret.GetComponent<DestroyedTurret>().turretHead.transform.rotation = turretXObj.transform.rotation;

            Destroy(laserLightObj);

            damageSmoke.transform.parent = deadTurret.transform; //set parent of damage smoke to new turret object (keeps smoke from disappearing when old turret is destroyed)
            damageSmoke.Stop();

            Destroy(this.gameObject);
        }
    }
}
