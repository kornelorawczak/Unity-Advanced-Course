using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [SerializeField] private GameObject weapon;
    [SerializeField] private GameObject weapon2;
    [HideInInspector] public static int selectedWeaponInt;
    private int currentWeaponInt;
    private PlayerInput playerInput;
    public PlayerInput.OnFootActions onFoot;
    private PlayerMotor motor;
    private PlayerLook look;
    private WeaponController wController;
    [SerializeField] private GameObject weaponSwitchObject;
    private WeaponSwitch weaponSwitch;
    public static bool isPaused = false;
    [Header("PauseMenu")]
    public PauseScript pauseMenuManager;

    void Awake() {
        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;
        weaponSwitch = weaponSwitchObject.GetComponent<WeaponSwitch>();
        motor = GetComponent<PlayerMotor>();
        onFoot.Jump.performed += ctx => motor.Jump(); // callback context with a pointer to function
        onFoot.Crouch.performed += ctx => motor.Crouch();
        onFoot.Crawl.performed += ctx => motor.Crawl();
        onFoot.Sprint.performed += ctx => motor.ToggleSprint(onFoot.Movement.ReadValue<Vector2>());
        onFoot.SprintReleased.performed += ctx => motor.StopSprint();
        onFoot.ZoomPressed.performed += ctx => motor.AimingPressed();
        onFoot.ZoomReleased.performed += ctx => motor.AimingReleased();

        onFoot.LeanLeftPressed.performed += ctx => motor.isLeaningLeft = true;
        onFoot.LeanLeftReleased.performed += ctx => motor.isLeaningLeft = false;

        onFoot.LeanRightPressed.performed += ctx => motor.isLeaningRight = true;
        onFoot.LeanRightReleased.performed += ctx => motor.isLeaningRight = false;

        onFoot.FirePressed.performed += ctx => motor.ShootingPressed();
        onFoot.FireReleased.performed += ctx => motor.ShootingReleased();

        onFoot.SwitchWeapon.performed += ctx => weaponSwitch.SwitchWeapon();

        look = GetComponent<PlayerLook>();
        currentWeaponInt = selectedWeaponInt = 0;
        wController = weapon.GetComponent<WeaponController>();

        onFoot.Reload.performed += ctx => wController.ReloadInput();
        onFoot.Pause.performed += ctx => pauseMenuManager.PauseMenuController();
    }

    void FixedUpdate() {
        motor.ProcessMove2(onFoot.Movement.ReadValue<Vector2>());

    }

    private void LateUpdate() {
        look.ProcessLook(onFoot.Look.ReadValue<Vector2>());
        if (selectedWeaponInt != currentWeaponInt) {
            if (selectedWeaponInt == 1) {
                wController = weapon2.GetComponent<WeaponController>();
            }
            else {
                wController = weapon.GetComponent<WeaponController>();
            }
            currentWeaponInt = selectedWeaponInt;
        }
        wController.ProcessSway(onFoot.Look.ReadValue<Vector2>(), onFoot.Movement.ReadValue<Vector2>());
    }

    private void OnEnable() {
        onFoot.Enable();
    }

    private void OnDisable() {
        onFoot.Disable();
    }
}
