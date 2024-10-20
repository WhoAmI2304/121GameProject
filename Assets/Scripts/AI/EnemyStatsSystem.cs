using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatsSystem : MonoBehaviour
{
    public int maxHP = 100;
    private int currentHP;

    // Start is called before the first frame update
    void Start()
    {
        currentHP = maxHP;
    }


    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
            Die();
    }

    private void Die()
    {
        Debug.Log("Enemy died");
        Destroy(gameObject);
    }

    public int GetCurrentHP()
    {
        return currentHP;
    }
}
