using System;
using System.Data.Common;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class PlayerMotor : MonoBehaviour
{
    #region - Player Settings
    public enum PlayerStance {
        Stand,
        Crouch,
        Crawl
    }
    private CharacterController controller;
    public Transform cameraHolder;
    public Transform feetTransform;
    public LayerMask playerMask;
    public LayerMask groundMask;
    public Animator animator;


    [Header("Player Movement")]
    public float walkForwardSpeed;
    public float walkStrafeSpeed;
    public float walkBackwardSpeed;
    public float sprintForwardSpeed;
    public float sprintStrafeSpeed;
    public float MovementSmoothing;
    public bool holdToSprint;

    [Header("Speed Effectors")]
    public float speedEffector = 1;
    public float crouchSpeedEffector;
    public float crawlSpeedEffector;
    public float fallSpeedEffector;
    public float aimingSpeedEffector;

    [Header("Jumping")]
    public float jumpingHeight;
    private Vector3 jumpingForce;
    public float jumpingFalloff;
    private Vector3 jumpingForceVelocity;
    public float fallingSmoothing;

    [Header("Stance")]
    public PlayerStance playerStance;
    public float playerStanceSmoothing;
    public float cameraStandHeight;
    public CapsuleCollider standCollider;
    public float cameraCrouchHeight;    
    public CapsuleCollider crouchCollider;
    public float cameraCrawlHeight;
    public CapsuleCollider crawlCollider;
    private float stanceCheckErrorMargin = 0.05f;
    private float cameraHeight;
    private float cameraHeightVelocity;
    private Vector3 stanceCapsuleCenterVelocity;
    private float stanceCapsuleHeightVelocity;
    [HideInInspector] public static bool isSprinting;
    private Vector3 newMovementSpeed;
    private Vector3 newMovementSpeedVelocity;
    
    [Header("Weapon")]
    public WeaponController weapon1;
    public WeaponController weapon2;
    public static float weaponAnimationSpeed;
    public static int selectedWeaponInt;
    private int currentWeaponInt;
    private WeaponController currentWeapon;

    [Header("Gravity")]
    public float gravityAmount;
    private float playerGravity;
    public float gravityMin;
    
    [Header("Grounded / Falling")]
    [HideInInspector] public static bool grounded;
    [HideInInspector] public static bool falling;
    public float isGroundedRadius;
    public float isFallingSpeed;

    [Header("Leaning")]
    public Transform leanPivot;
    private float currentLean;
    private float targetLean;
    public float leanAngle;
    public float leanSmoothing;
    private float leanVelocity;
    public bool isLeaningLeft;
    public bool isLeaningRight;

    [HideInInspector] public static bool isAiming;
    #endregion

    void Start()
    {
        // if (WeaponSwitch.selectedWeapon == 0) {
        //     currentWeapon = weapon1;
        // }
        // else {
        //     currentWeapon = weapon2;
        // }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentWeaponInt = selectedWeaponInt = 0;
        currentWeapon = weapon1;
        controller = GetComponent<CharacterController>();
        if (currentWeapon) {
            currentWeapon.Initalize(controller);
        }
        cameraHeight = cameraHolder.localPosition.y;
    }

    void Update()
    {
        if (animator.GetBool("IsDead")) {
            return;
        }
        SetIsGrounded();
        SetIsFalling();
        CalculateJump();
        if (selectedWeaponInt != currentWeaponInt) {
            if (currentWeapon == weapon1) {
                currentWeapon = weapon2;
            }
            else {
                currentWeapon = weapon1;
            }
            currentWeapon.Initalize(controller);
            currentWeaponInt = selectedWeaponInt;
        }
        CalculateStance();
        CalculateLeaning();
        CalculateAiming();
        
    }


    #region - Movement
    private void SetIsGrounded() {
        grounded = Physics.CheckSphere(feetTransform.position, isGroundedRadius, groundMask);
    } 

    private void SetIsFalling() {
        falling = !grounded && controller.velocity.magnitude >= isFallingSpeed;
    } 

    public void ProcessMove2(Vector2 input) {
        if (animator.GetBool("IsDead")) {
            return;
        }
        if (input.y < 0.2f) {
            isSprinting = false;
        }

        var verticalSpeed = walkForwardSpeed;
        var horizontalSpeed = walkStrafeSpeed;

        if (isSprinting) {
            verticalSpeed = sprintForwardSpeed;
            horizontalSpeed = sprintStrafeSpeed;
        }

        if (input.y < 0) {
            verticalSpeed = walkBackwardSpeed;
        }

        if (!grounded) {
            speedEffector = fallSpeedEffector;
        }
        else if (isAiming) {
            speedEffector = aimingSpeedEffector;
        }
        else if (playerStance == PlayerStance.Crouch) {
            speedEffector = crouchSpeedEffector;
        }
        else if (playerStance == PlayerStance.Crawl) {
            speedEffector = crawlSpeedEffector;
        }
        else {
            speedEffector = 1;
        }
        weaponAnimationSpeed = controller.velocity.magnitude / (walkForwardSpeed * speedEffector);
    
        if (weaponAnimationSpeed > 1) {
            weaponAnimationSpeed = 1;
        }
        

        verticalSpeed *= speedEffector;
        horizontalSpeed *= speedEffector;


        newMovementSpeed = Vector3.SmoothDamp(newMovementSpeed, new Vector3(horizontalSpeed * input.x * Time.deltaTime, 0, verticalSpeed * input.y * Time.deltaTime), ref newMovementSpeedVelocity, grounded ? MovementSmoothing : fallingSmoothing);
        var movementSpeed = transform.TransformDirection(newMovementSpeed);

        if (playerGravity > gravityMin) {
            playerGravity -= gravityAmount * Time.deltaTime;
        }

        if (playerGravity < -0.1f && grounded) {
            playerGravity = -0.1f;
        }


        movementSpeed.y += playerGravity;
        movementSpeed += jumpingForce * Time.deltaTime;
        controller.Move(movementSpeed);
    }

    private void CalculateJump() {
        jumpingForce = Vector3.SmoothDamp(jumpingForce, Vector3.zero, ref jumpingForceVelocity, jumpingFalloff);
    }   

    private void CalculateStance() {
        float stanceHeight = cameraStandHeight;
        var stanceCollider = standCollider;
        if (playerStance == PlayerStance.Crouch) {
            stanceHeight = cameraCrouchHeight;
            stanceCollider = crouchCollider;
        }   
        else if (playerStance == PlayerStance.Crawl) {
            stanceHeight = cameraCrawlHeight;
            stanceCollider = crawlCollider;
        }

        cameraHeight = Mathf.SmoothDamp(cameraHolder.localPosition.y, stanceHeight, ref cameraHeightVelocity, playerStanceSmoothing);
        cameraHolder.localPosition = new Vector3(cameraHolder.localPosition.x, cameraHeight, cameraHolder.localPosition.z);
        controller.height = Mathf.SmoothDamp(controller.height, stanceCollider.height, ref stanceCapsuleHeightVelocity, playerStanceSmoothing);
        controller.center = Vector3.SmoothDamp(controller.center, stanceCollider.center, ref stanceCapsuleCenterVelocity, playerStanceSmoothing);
    }
    public void Jump() {
        if (!grounded || playerStance == PlayerStance.Crawl) {
            return; // can't jump while crawling
        }
        if (playerStance == PlayerStance.Crouch) {
            if (StanceCheck(standCollider.height)) {
                return;
            }
            playerStance = PlayerStance.Stand;
            return;
        }
        jumpingForce = Vector3.up * jumpingHeight;
        playerGravity = 0;
        currentWeapon.TriggerJump();
    }
    
    public void Crouch() {
        if (playerStance == PlayerStance.Crouch) {
            if (StanceCheck(standCollider.height)) {
                return; // can't stand up
            }
            playerStance = PlayerStance.Stand;
            return;
        }
        if (StanceCheck(crouchCollider.height)) {
            return; // can't crouch from crawl
        }
        playerStance = PlayerStance.Crouch;
    }

    public void Crawl() {
        if (playerStance == PlayerStance.Crawl) {
            if (StanceCheck(standCollider.height)) {
                return;
            }
            playerStance = PlayerStance.Stand;
            return;
        }
        playerStance = PlayerStance.Crawl;
    }

    private bool StanceCheck(float stanceCheckHeight) {
        Vector3 start = new Vector3(feetTransform.position.x, feetTransform.position.y + stanceCheckErrorMargin + controller.radius, feetTransform.position.z);
        Vector3 end = new Vector3(feetTransform.position.x, feetTransform.position.y - stanceCheckErrorMargin - controller.radius + stanceCheckHeight, feetTransform.position.z);


        return Physics.CheckCapsule(start, end, controller.radius, playerMask);
    }
    
    public void ToggleSprint(Vector2 input) {
        if (input.y < 0.2f) {
            isSprinting = false;
            return;
        }
        isSprinting = !isSprinting;
    }

    public void StopSprint() {
        if (holdToSprint) {
            isSprinting = false;
        }
    }
    #endregion

    #region - Leaning


    private void CalculateLeaning() {
        if (isLeaningLeft) {
            targetLean = leanAngle;
        }
        else if (isLeaningRight) {
            targetLean = -leanAngle;
        }
        else {
            targetLean = 0;
        }

        currentLean = Mathf.SmoothDamp(currentLean, targetLean, ref leanVelocity, leanSmoothing);

        leanPivot.localRotation = Quaternion.Euler(new Vector3(0, 0, currentLean));
    }

    #endregion

    #region - Aiming
    public void AimingPressed() {
        isAiming = true;
    }

    public void AimingReleased() {
        isAiming = false;

    }
    public void CalculateAiming() {
        if (!currentWeapon) {
            return;
        }
        currentWeapon.isAiming = isAiming;
    }
    #endregion

    #region - Shooting

    
    
    public void ShootingPressed() {
        if (currentWeapon) {
            currentWeapon.isShooting = true;
        }
    }

    public void ShootingReleased() {
        if (currentWeapon) {
            currentWeapon.isShooting = false;
        }
    } 
    

    #endregion
}
