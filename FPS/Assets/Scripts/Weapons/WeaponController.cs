using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;
using System.Runtime.CompilerServices;
using TMPro;
public class WeaponController : MonoBehaviour
{
    public enum WeaponFireType
    {
        SemiAuto,
        FullyAuto
    }
    private CharacterController controller;
    [SerializeField] private Transform cam;
    [SerializeField] private ParticleSystem flash;
    [SerializeField] private GameObject hitEffect;
    private bool isInitialized = false;
    private Vector3 newWeaponRotation;
    private Vector3 newWeaponRotationVelocity;
    private Vector3 targetWeaponRotation;
    private Vector3 targetWeaponRotationVelocity;
    
    private Vector3 newWeaponMovementRotation;
    private Vector3 newWeaponMovementRotationVelocity;
    private Vector3 targetWeaponMovementRotation;
    private Vector3 targetWeaponMovementRotationVelocity;
    public Animator animator;

    [Header("References")]
    public Animator weaponAnimator;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public TextMeshProUGUI ammoText;

    
    [Header("Sway - Settings")]
    public float swayAmount;
    public float swaySmoothing;
    public float swayResetSmoothing;
    public float swayClampX;
    public float swayClampY;

    [Header("Movement Sway - Settings")]
    public float movementSwayX;
    public float movementSwayY; 
    public float movementSwaySmoothing;

    private bool isGroundedTrigger;
    private bool isFallingTrigger;
    private float fallingDelay;

    [Header("Weapon Sway - Breathing")]
    public Transform weaponSwayObject;
    public float swayAmountA = 1;
    public float swayAmountB = 2;
    public float swayScale = 150; // scaling breathing depending on weapon holding in hand
    public float swayLerpSpeed = 14;
    private float swayTime;
    private Vector3 swayPosition;

    [Header("Aim")]
    public Transform sightTarget;
    [HideInInspector] public bool isAiming;
    public float sightOffset;
    public float aimingTime;
    private Vector3 weaponSwayPosition;
    private Vector3 weaponSwayPositionVelocity;

    [Header("Shooting")] 
    public float fireRate;
    public List<WeaponFireType> allowedFireTypes;
    public WeaponFireType currentFireType;
    [HideInInspector] public bool isShooting;
    public float damage = 10f;
    public float range = 100f;
    private float nextTimeToFire = 0f;

    [Header("Reload")]
    public int maxAmmo;
    private int currentAmmo;
    public float reloadTime = 1f;
    public float hidingTime = 1f;
    public float drawingTime = 1f;
    private bool isReloading = false;
    private bool isChanging = false;

    #region - Initialize / Start / Update
    public void Initalize(CharacterController cont) {
        controller = cont;
        isInitialized = true;
    }
    
    private void Start() {
        newWeaponRotation = transform.localRotation.eulerAngles;
        currentFireType = allowedFireTypes.First();
        currentAmmo = maxAmmo;
    }

    private void Update() {
        if (animator.GetBool("IsDead")) {
            return;
        }
        ammoText.text = currentAmmo.ToString() + "/" + maxAmmo.ToString();
        if (isReloading || isChanging) {
            return;
        }
        if (!isInitialized) {
            return;
        }
        SetWeaponAnimations();
        CalculateWeaponSway();
        CalculateAiming();
        if (currentAmmo > 0) {
            CalculateShooting();   
        }
    }

    void OnEnable() {
        isReloading = false;
        weaponAnimator.SetBool("Reloading", false);
    }
    #endregion
    
    #region - Weapon Behaviour
    public void TriggerJump() {
        isGroundedTrigger = false;
        // Jump
        weaponAnimator.SetTrigger("Jump");
    }

    public void ProcessSway(Vector2 input, Vector2 input_movement) {
        if (!isInitialized) {
            return;
        }

        //weaponAnimator.speed = PlayerMotor.weaponAnimationSpeed;
        targetWeaponRotation.y += (isAiming ? swayAmount / 2 : swayAmount) * input.x * Time.deltaTime;
        targetWeaponRotation.x += (isAiming ? swayAmount / 2 : swayAmount) * input.y * Time.deltaTime;

        targetWeaponRotation.x = Mathf.Clamp(targetWeaponRotation.x, -swayClampX, swayClampX);
        targetWeaponRotation.y = Mathf.Clamp(targetWeaponRotation.y, -swayClampY, swayClampY);
        targetWeaponRotation.z = isAiming ? 0 : targetWeaponRotation.y * -2;

        targetWeaponRotation = Vector3.SmoothDamp(targetWeaponRotation, Vector3.zero, ref targetWeaponRotationVelocity, swayResetSmoothing);
        newWeaponRotation = Vector3.SmoothDamp(newWeaponRotation, targetWeaponRotation, ref newWeaponRotationVelocity, swaySmoothing); 

        targetWeaponMovementRotation.z = movementSwayX * input_movement.x;
        targetWeaponMovementRotation.x = movementSwayY * -input_movement.y;

        targetWeaponMovementRotation = Vector3.SmoothDamp(targetWeaponMovementRotation, Vector3.zero, ref targetWeaponMovementRotationVelocity, movementSwaySmoothing);
        newWeaponMovementRotation = Vector3.SmoothDamp(newWeaponMovementRotation, targetWeaponMovementRotation, ref newWeaponMovementRotationVelocity, movementSwaySmoothing);



        transform.localRotation = Quaternion.Euler(newWeaponRotation + newWeaponMovementRotation);
    }

    private void SetWeaponAnimations() {
        if (isGroundedTrigger) {
            fallingDelay = 0;
        }
        else {
            fallingDelay += Time.deltaTime;
        }
        if (PlayerMotor.grounded && !isGroundedTrigger && fallingDelay > 0.01f) {
            // Land
            weaponAnimator.SetTrigger("Land");
            SoundManager.PlaySound(SoundType.LAND, 0.5f);
            isGroundedTrigger = true;
        }
        else if (!PlayerMotor.grounded && isGroundedTrigger) {
            // Falling
            weaponAnimator.SetTrigger("Falling");
            isGroundedTrigger = false;
        }
        weaponAnimator.SetBool("isSprinting", PlayerMotor.isSprinting);
        weaponAnimator.SetFloat("WeaponAnimationSpeed", PlayerMotor.weaponAnimationSpeed);
    }

    private void CalculateWeaponSway() {
        // breathing sim
        var targetPosition = LissajousCurve(swayTime, swayAmountA, swayAmountB) / (isAiming ? swayScale * 3 : swayScale);
        swayPosition = Vector3.Lerp(swayPosition, targetPosition, Time.smoothDeltaTime * swayLerpSpeed);
        swayTime += Time.deltaTime;
        if (swayTime > 6.3f) {
            swayTime = 0;
        }
    }   
    private Vector3 LissajousCurve(float Time, float A, float B) {
        // curve simulating breathing
        return new Vector3(Mathf.Sin(Time), A * Mathf.Sin(B * Time * Mathf.PI));
    }
    #endregion
    
    #region - Aiming, Shooting and Reloading
    
    private void CalculateAiming() {
        var targetPosition = transform.position;
        if (isAiming) {
            targetPosition = cam.position + (weaponSwayObject.transform.position - sightTarget.position) + (cam.transform.forward * sightOffset);
        }

        weaponSwayPosition = weaponSwayObject.transform.position;
        weaponSwayPosition = Vector3.SmoothDamp(weaponSwayPosition, targetPosition, ref weaponSwayPositionVelocity, aimingTime);
        weaponSwayObject.transform.position = weaponSwayPosition + swayPosition;
    }

    private void CalculateShooting() {
        if (isShooting && Time.time >= nextTimeToFire && !PlayerMotor.isSprinting) {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
            if (currentFireType == WeaponFireType.SemiAuto) {
                isShooting = false;
            }
        }
    }

    public void Shoot() {
        var bullet = Instantiate(bulletPrefab, bulletSpawn);
        weaponAnimator.SetTrigger("Shoot");
        flash.Play();
        currentAmmo--;

        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range)) {
            Target target = hit.transform.GetComponent<Target>();
            if (target != null) {
                target.TakeDamage(damage);
            }
            Debug.Log(hit.transform.tag);
            if (hit.transform.tag == "Enemy") {
                hit.transform.GetComponent<Enemy>().TakeDamage(damage);
            }

            Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
        }


    }
    public void ReloadInput() {
        StartCoroutine(Reload());
    }
    private IEnumerator Reload(){
        isReloading = true;
        weaponAnimator.SetBool("Reloading", true);
        yield return new WaitForSeconds(reloadTime);
        weaponAnimator.SetBool("Reloading", false);
        yield return new WaitForSeconds(.3f);
        currentAmmo = maxAmmo;
        isReloading = false;
    }

    public IEnumerator HideWeapon() {
        isChanging = true;
        weaponAnimator.SetBool("Hiding", true);
        yield return new WaitForSeconds(hidingTime);
        weaponAnimator.SetBool("Hiding", false);
        yield return new WaitForSeconds(1f);
        isChanging = false;
    }
    
    public IEnumerator DrawWeapon() {
        isChanging = true;
        weaponAnimator.SetBool("Drawing", true);
        yield return new WaitForSeconds(drawingTime);
        weaponAnimator.SetBool("Drawing", false);
        yield return new WaitForSeconds(.3f);
        isChanging = false;
    }
    #endregion
}
