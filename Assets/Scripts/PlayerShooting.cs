using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerShooting : MonoBehaviour
{
    public WeaponType weaponType;
    [SerializeField] Missile bulletPrefabRef;
    [SerializeField] TargetingSystem cmp_targetingSystem;
    [SerializeField] Transform t_PistolSpawnPoint;
    [SerializeField] float f_spreadOffset;
    [SerializeField] float f_ReloadTime;
    [SerializeField] bool b_Reloading;

    [SerializeField] Image img_Shoot;
    [SerializeField] Button btn_Shoot;
    public float rotationSpeed = 5f;  // Speed of the turret rotation

    public void OnClick_Switch()
    {
        if(weaponType == WeaponType.Pistol)
            weaponType = WeaponType.Shotgun;
        else 
            weaponType=WeaponType.Pistol;
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            OnClick_Switch();
        }

        if (Input.GetKeyDown(KeyCode.Space) && !b_Reloading)
        {
            OnClick_Shoot();
        }
    }


    public void OnClick_Shoot()
    {
        if (!b_Reloading)
        {
            switch (weaponType)
            {
                case WeaponType.Pistol:
                    {
                        ShootSingleMissile();
                    }
                    break;
                case WeaponType.Shotgun:
                    {
                        ShootMultipleMissile();
                    }
                    break;
                default:
                    break;
            }
        }
    }

    private void ShootSingleMissile()
    {
        Missile missile = Instantiate(bulletPrefabRef, t_PistolSpawnPoint.position, t_PistolSpawnPoint.rotation);
        missile.target = cmp_targetingSystem.currentClosestTarget;
        StartFillAnimation(f_ReloadTime);
    }

    void ShootMultipleMissile()
    {
        for (int i = 0; i < 3; i++)
        {
            // Calculate a random offset for each missile
            Vector3 offset = UnityEngine.Random.insideUnitCircle * f_spreadOffset;
            Vector3 spawnPosition = t_PistolSpawnPoint.position + offset;

            // Instantiate and configure the missile
            Missile missile = Instantiate(bulletPrefabRef, spawnPosition, t_PistolSpawnPoint.rotation);
            missile.target = cmp_targetingSystem.currentClosestTarget;
            missile.SetAsSpreadMissile();
        }
        StartFillAnimation(f_ReloadTime);
    }

    public IEnumerator AnimateFill(float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // Calculate the current fill amount based on the elapsed time
            float fill = Mathf.Clamp01(elapsedTime / duration);

            // Update the image fill amount
            img_Shoot.fillAmount = fill;

            // Wait until the next frame
            yield return null;
        }

        // Ensure the fill amount is set to 1 when the animation is complete
        img_Shoot.fillAmount = 1f;
        b_Reloading = false;
        btn_Shoot.interactable = true;
    }

    // Method to start the fill animation
    public void StartFillAnimation(float time)
    {
        img_Shoot.fillAmount = 0f;
        b_Reloading = true;
        btn_Shoot.interactable = false;
        StartCoroutine(AnimateFill(time));
    }

}
