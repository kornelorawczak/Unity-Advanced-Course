using UnityEngine;
using System.Collections;

public class WeaponSwitch : MonoBehaviour
{
    public static int selectedWeapon = 0; // 0 - rifle; 1 - shotgun
    
    
    public void SwitchWeapon() {
        if (selectedWeapon == 0) {
            StartCoroutine(transform.GetChild(0).GetComponent<WeaponController>().HideWeapon());
            transform.GetChild(0).gameObject.SetActive(false);
            PlayerMotor.selectedWeaponInt = 1; 
            InputManager.selectedWeaponInt = 1;
            selectedWeapon = 1;
            transform.GetChild(1).gameObject.SetActive(true);
            StartCoroutine(transform.GetChild(1).GetComponent<WeaponController>().DrawWeapon());
        }
        else {            
            StartCoroutine(transform.GetChild(1).GetComponent<WeaponController>().HideWeapon());
            transform.GetChild(1).gameObject.SetActive(false);
            PlayerMotor.selectedWeaponInt = 0;
            InputManager.selectedWeaponInt = 0;
            selectedWeapon = 0;
            transform.GetChild(0).gameObject.SetActive(true);
            StartCoroutine(transform.GetChild(0).GetComponent<WeaponController>().DrawWeapon());
        }
        
    }

    
}
