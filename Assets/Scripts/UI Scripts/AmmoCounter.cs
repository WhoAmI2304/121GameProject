using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class AmmoCounter : MonoBehaviour
{
    [SerializeField] private Text pocketAmmoText; // UI Text for pocket ammo
    [SerializeField] private Text magazineAmmoText; // UI Text for magazine ammo
    [SerializeField] private Text HPText; // UI Text for HP
    [SerializeField] private Text reloadText;
    [SerializeField] private LayerMask weaponLayerMask; // Layer Mask to identify active weapon

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
        if (Input.GetKeyDown(KeyCode.BackQuote)) SceneManager.LoadScene(0);

        StatsSystem playerStats = FindPlayerStats();
        Weapons activeWeapon = FindActiveWeapon();
        
        if (playerStats != null) HPText.text = "HP: " + playerStats.currentHP;
        else HPText.text = "";

        if (activeWeapon != null)
        {
            reloadText.text = activeWeapon.isReloading ? "Reloading..." : "";
            pocketAmmoText.text = "Pocket Ammo: " + activeWeapon.ammoInPocket / activeWeapon.bulletsPerShot;
            magazineAmmoText.text = "Magazine Ammo: " + activeWeapon.currentAmmo / activeWeapon.bulletsPerShot;
        } else
        {
            reloadText.text = "";
            pocketAmmoText.text = "";
            magazineAmmoText.text = "";
        }
    }

    Weapons FindActiveWeapon()
    {
        Weapons[] weapons = FindObjectsOfType<Weapons>();
        foreach (Weapons weapon in weapons)
        {
            if (weapon.gameObject.activeInHierarchy && ((1 << weapon.gameObject.layer) & weaponLayerMask) != 0)
                return weapon;
        }
        return null;
    }

    StatsSystem FindPlayerStats()
    {
        StatsSystem[] playerStats = FindObjectsOfType<StatsSystem>();
        foreach (StatsSystem playerStat in playerStats)
        {
            if (playerStat.gameObject.activeInHierarchy && ((1 << playerStat.gameObject.layer) & weaponLayerMask) != 0)
                return playerStat;
        }
        return null;
    }
}
