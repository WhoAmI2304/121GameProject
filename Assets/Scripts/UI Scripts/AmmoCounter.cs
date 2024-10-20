using UnityEngine;
using UnityEngine.UI;

public class AmmoCounter : MonoBehaviour
{
    [SerializeField] private Text pocketAmmoText; // UI Text for pocket ammo
    [SerializeField] private Text magazineAmmoText; // UI Text for magazine ammo

    [SerializeField] private Text reloadText;
    [SerializeField] private LayerMask weaponLayerMask; // Layer Mask to identify active weapon

    // Update is called once per frame
    void Update()
    {
        Weapons activeWeapon = FindActiveWeapon();
        if (activeWeapon != null)
        {
            reloadText.text = activeWeapon.isReloading ? "Reloading..." : "";
            pocketAmmoText.text = "Pocket Ammo: " + activeWeapon.ammoInPocket;
            magazineAmmoText.text = "Magazine Ammo: " + activeWeapon.currentAmmo;
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
}
