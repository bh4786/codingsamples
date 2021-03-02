using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Player Health References")]
    [SerializeField] private AudioSource aud = null;
    [SerializeField] private Volume ppVolume = null; //post processing volume
    [SerializeField] private Slider healthBar = null;
    [SerializeField] private Animator camAnim = null;

    [Header("Player Health Settings")]
    [SerializeField] private float startHealth = 100f;
    [SerializeField] private float healDelay = 3f; //time before player starts healing after taking damage
    [SerializeField] private float healDuration = 0.5f; //how long health bar takes to return to normal
    [SerializeField] private float damageDuration = 0.5f; //how long health bar takes to go down to new level

    private Vignette vignette; //vignette effect
    private ChromaticAberration chrAb; //chromatic aberration effect

    private bool isAlive = true;
    private int damageTier = 0; //number showing player health level in tiers
    private float currentHealth = 0f;
    private float healTimer = 0f;

    void Start()
    {
        currentHealth = startHealth;
        healthBar.value = currentHealth;

        ppVolume.profile.TryGet(out vignette); //set vignette variable
        ppVolume.profile.TryGet(out chrAb); //set chromatic aberation variable
        vignette.intensity.value = 0f;
        chrAb.intensity.value = 0f;
    }
    private void Update()
    {
        healTimer += Time.deltaTime;
        if (healTimer > healDelay && currentHealth < startHealth) //if player hasn't been damaged in set amnt of time, heal
        {
            aud.Stop(); //stop heartbeat sounds

            healTimer = 0f; //reset timer, damage tier, and health
            damageTier = 0;
            currentHealth = startHealth;

            DOTween.To(() => vignette.intensity.value, x => vignette.intensity.value = x, 0f, currentHealth / 100f * healDuration); //tween post processing effects back to 0
            DOTween.To(() => chrAb.intensity.value, x => chrAb.intensity.value = x, 0f, currentHealth / 100f * healDuration);

            healthBar.DOValue(startHealth, currentHealth / 100f * healDuration); //tween health bar back to full
        }
    }
    void FixedUpdate()
    {
        if (currentHealth > 50f && damageTier >= 1)
        {
            damageTier = 0;

            DOTween.To(() => vignette.intensity.value, x => vignette.intensity.value = x, 0f, damageDuration); //remove vignette and chrom aberration
            DOTween.To(() => chrAb.intensity.value, x => chrAb.intensity.value = x, 0f, damageDuration);
        }
        else if (currentHealth <= 50f && damageTier < 1)
        {
            damageTier = 1;

            DOTween.To(() => vignette.intensity.value, x => vignette.intensity.value = x, 0.3f, damageDuration); //add slight amount of vignette and chrom aberration
            DOTween.To(() => chrAb.intensity.value, x => chrAb.intensity.value = x, 0.2f, damageDuration);
        }
        else if (currentHealth <= 25f && damageTier < 2)
        {
            damageTier = 2;

            DOTween.To(() => vignette.intensity.value, x => vignette.intensity.value = x, 0.375f, damageDuration); //add more vignette and chrom aberration
            DOTween.To(() => chrAb.intensity.value, x => chrAb.intensity.value = x, 0.3f, damageDuration);

            aud.Play(); //start playing heartbeat sounds
        }
        else if (currentHealth <= 10f && damageTier < 3)
        {
            damageTier = 3;

            DOTween.To(() => vignette.intensity.value, x => vignette.intensity.value = x, 0.45f, damageDuration); //add max amount of vignette and chrom aberation
            DOTween.To(() => chrAb.intensity.value, x => chrAb.intensity.value = x, 0.4f, damageDuration);
        }
        if (currentHealth <= 0 && isAlive)
        {
            Die();
        }
    }
    public void CamShake(float intensity)
    {
        camAnim.SetTrigger("Explosion");
    }
    public void TakeDamage(float damage)
    {
        healTimer = 0f;
        currentHealth -= damage;
        healthBar.DOValue(currentHealth, damageDuration); //tween health to new health level
    }

    private void Die()
    {
        isAlive = false;
    }
}
