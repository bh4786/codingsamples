using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Gun : MonoBehaviour
{
    [Header("Gun Particle References")]
    [SerializeField] private ParticleSystem flash = null; //muzzle flash
    [SerializeField] private GameObject impactFx = null; //bullet impact particles

    [Header("Camera References")]
    [SerializeField] private Camera cam = null;
    [SerializeField] private Animator camAnim = null;
    [SerializeField] private Recoil recoil = null;

    [Header("Audio References")]
    [SerializeField] private AudioSource aud = null;
    [SerializeField] private PlayerSounds playerSounds = null;
    [SerializeField] private AudioClip gunShot = null;

    [Header("Gun Settings")]
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float shotForce = 50f; //force applied to physics objects
    [SerializeField] private float gunDamage = 50f;
    [SerializeField] private float recoilIntensity = 5f;
    [SerializeField] private LayerMask hitLayers = new LayerMask();
    [SerializeField] private PlayerMovement playerMovement = null;

    private GameObject tempImpact = null; //temporary bullet impact fx
    private Animator anim = null;
    private float shootTimer = 0f;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        shootTimer += Time.deltaTime;

        if (Input.GetButtonDown("Shoot") && shootTimer > fireRate && !anim.GetBool("Sprinting") && !playerMovement.movementLocked)
        {
            aud.pitch = Random.Range(0.9f, 1f); //randomize audio pitch
            aud.PlayOneShot(gunShot);
            StartCoroutine(Shoot());
        }
    }
    private IEnumerator Shoot()
    {
        shootTimer = 0f;

        anim.speed = 1f; //trigger animations
        anim.Play("GunIdle", -1, 0f);
        anim.SetTrigger("Shoot");
        anim.SetInteger("ShootIndex", Random.Range(0, 3));
        camAnim.SetTrigger("GunShake");

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0)); //create ray from center of camera
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, hitLayers))
        {
            if (tempImpact == null) //if impact particle doesn't exist, create it
            {
                tempImpact = Instantiate(impactFx, hit.point, Quaternion.LookRotation(Vector3.Reflect(ray.direction, hit.normal)));
            }

            tempImpact.transform.position = hit.point; //set particle position and rotation according to ray direction and reflection off surface
            tempImpact.transform.rotation = Quaternion.LookRotation(Vector3.Reflect(ray.direction, hit.normal));
            tempImpact.GetComponent<ParticleSystem>().Play();

            if (hit.transform.gameObject.GetComponent<IDamageable>() != null) //if ray hits damageable object, call its take damage function
            {
                hit.transform.gameObject.GetComponent<IDamageable>().TakeDamage(gunDamage);
            }
            else if (hit.transform.gameObject.GetComponent<Rigidbody>() != null && hit.transform.gameObject.layer != 16) //if player hits physics object, apply force based on shot direction
            {
                hit.transform.gameObject.GetComponent<Rigidbody>().AddForce(ray.direction * shotForce, ForceMode.Impulse);
            }
        }

        recoil.DoRecoil(recoilIntensity);

        yield return new WaitForSeconds(0.03f);

        flash.Play(); //play muzzle flash particle system
    }
    public void UpgradeWeapon(float multiplier)
    {
        gunDamage *= multiplier; //increase damage of gun by set multiplier
    }
    public void FootstepSound(float volume)
    {
        playerSounds.Footstep(volume);
    }
}
