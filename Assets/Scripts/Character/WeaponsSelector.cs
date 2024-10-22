using System.Collections.Generic;
using UnityEngine;

public class WeaponsSelector : MonoBehaviour
{
    [SerializeField] private List<GameObject> weapons;
    [SerializeField] public List<KeyCode> keys;

    private Dictionary<KeyCode, GameObject> keyBindings = new Dictionary<KeyCode, GameObject>();

    private void Awake()
    {
        keyBindings.Clear();
        for (int i = 0; i < weapons.Count; i++)
        {
            if (i < keys.Count)
                keyBindings[keys[i]] = weapons[i];
        }
    }

    void Update()
    {
        foreach (var binding in keyBindings)
        {
            if (Input.GetKeyDown(binding.Key))
                SelectWeapon(binding.Value);
        }
    }

    private void SelectWeapon(GameObject selectedWeapon)
    {
        foreach (var weapon in weapons)
        {
            weapon.gameObject.SetActive(weapon == selectedWeapon ? true : false);
            Weapons _weapon = weapon.gameObject.GetComponent<Weapons>();
            if (weapon != selectedWeapon)
                _weapon.SwitchWeapon(_weapon);
        }
    }
}
