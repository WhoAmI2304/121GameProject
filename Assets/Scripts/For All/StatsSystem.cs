using UnityEngine;

public class StatsSystem : MonoBehaviour
{
    [SerializeField] private int maxHP = 100;
    [SerializeField] public int currentHP;

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
