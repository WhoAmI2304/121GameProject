using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [HideInInspector] public int damage;
    [SerializeField] private float speed = 20f;

    [SerializeField] private float lifeTime = 5f;

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        lifeTime -= Time.deltaTime;
        if(lifeTime <= 0)
            Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Bullet collided with: {other.gameObject.name}");

        if (IsInLayerMask(other.gameObject.layer, enemyLayer))
        {
            Debug.Log($"Hit enemy: {other.gameObject.name}");
            var enemyStats = other.gameObject.GetComponent<EnemyStatsSystem>();
            if (enemyStats != null)
            {
                enemyStats.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (IsInLayerMask(other.gameObject.layer, obstacleLayer))
        {
            Debug.Log($"Hit obstacle: {other.gameObject.name}");
            Destroy(gameObject);
        }
    }

    private bool IsInLayerMask(int layer, LayerMask layerMask)
    {
        return (layerMask.value & (1 << layer)) > 0;
    }
}
