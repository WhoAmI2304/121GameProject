using System.Collections;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private int maxAmmo = 30;
    public int currentAmmo;
    [SerializeField] float coldown = 0.1f;
    public int ammoInPocket = 90;
    [SerializeField] private float reloadTime = 2f;
    [SerializeField] [Min(1)] public int bulletsPerShot = 1;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private Texture2D spreadTexture;
    [HideInInspector] public bool isReloading = false;
    [SerializeField] public Transform target;
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] [Range(0.001f, 5)] private float dotOffset = 0.5f;
    [SerializeField] private float angleMinX = -10;
    [SerializeField] private float angleMaxX = -90;
    private LayerMask weaponLayerMask;
    void Start()
    {
        currentAmmo = maxAmmo;
        weaponLayerMask = 1 << gameObject.layer;
    }

    private bool isCoolingDown = false;

    private float maxDistance;
    void RotateTowardsTarget()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > maxDistance)
            maxDistance = distance;
        
        distance = Mathf.Clamp(distance, 0, maxDistance);

        float t = Mathf.SmoothStep(0, 1, distance / (maxDistance * dotOffset));
        float angleX = Mathf.Lerp(angleMinX, angleMaxX, t);

        Vector3 currentRotation = transform.eulerAngles;

        transform.rotation = Quaternion.Euler(angleX, currentRotation.y, currentRotation.z);
    }
    void Update()
    {   

        RotateTowardsTarget();
        if (isReloading || isCoolingDown) return;

        if (weaponLayerMask == playerLayerMask)
        {
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
    }


    public void EnemyInput()
    {
        if (isReloading || isCoolingDown) return;
        
        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        Shoot();
        StartCoroutine(Cooldown());
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
            Vector3 spreadVector = GetRandomSpreadDirection();
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.LookRotation(spreadVector));
            bullet.GetComponent<Bullet>().damage = damage;
        }
    }

    Vector3 GetRandomSpreadDirection()
    {
        if (spreadTexture == null) return bulletSpawnPoint.forward;

        Vector3 spreadDirection = bulletSpawnPoint.forward;
        bool validPointFound = false;

        while (!validPointFound)
        {
            int x = Random.Range(0, spreadTexture.width);
            int y = Random.Range(0, spreadTexture.height);
            Color pixelColor = spreadTexture.GetPixel(x, y);

            if (pixelColor == Color.white)
            {
                float spreadX = (x / (float)spreadTexture.width) - 0.5f;
                float spreadY = (y / (float)spreadTexture.height) - 0.5f;
                spreadDirection = bulletSpawnPoint.forward + new Vector3(spreadX, spreadY, 0);
                validPointFound = true;
            }
        }

        return spreadDirection;
    }
}