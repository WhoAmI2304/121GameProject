using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private int maxAmmo = 30;
    public int currentAmmo;
    [SerializeField] float coldown = 0.1f;
    public int ammoInPocket = 90;
    [SerializeField] private float reloadTime = 2f;
    [SerializeField] private float spread = 0.1f;
    [SerializeField] private int bulletsPerShot = 1;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;

    [HideInInspector] public bool isReloading = false;

    void Start()
    {
        currentAmmo = maxAmmo;
    }

    private bool isCoolingDown = false;

    void Update()
    {
        if (isReloading || isCoolingDown)
            return;

        if (currentAmmo <= 0 || Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetMouseButton(0))
        {
            Shoot();
            StartCoroutine(Cooldown());
        }
    }

    IEnumerator Cooldown()
    {
        isCoolingDown = true;
        yield return new WaitForSeconds(coldown);
        isCoolingDown = false;
    }

    IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        // Calculate the amount of ammo needed to fill the magazine
        int ammoNeeded = maxAmmo - currentAmmo;
        // Determine the actual amount to reload based on available ammo in pocket
        int ammoToReload = Mathf.Min(ammoNeeded, ammoInPocket);
        // Update the current ammo and ammo in pocket
        currentAmmo += ammoToReload;
        ammoInPocket -= ammoToReload;

        isReloading = false;
    }

    void Shoot()
    {
        for (int i = 0; i < bulletsPerShot; i++)
        {
            currentAmmo--;
            Vector3 spreadVector = new Vector3(
                Random.Range(-spread, spread),
                Random.Range(-spread, spread),
                0
            ).normalized * Random.Range(-spread, spread);
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            bullet.GetComponent<Bullet>().damage = damage;
            bullet.transform.forward += spreadVector;
        }
    }
}
