using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [HideInInspector] public int damage;
    [HideInInspector] public float speed;

    [SerializeField] private float lifeTime = 5f;

    void Update()
    {
        float distance = speed * Time.deltaTime;
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
    
        if (Physics.Raycast(ray, out hit, distance, enemyLayer | obstacleLayer))
            OnTriggerEnter(hit.collider);
    
        transform.Translate(Vector3.forward * distance);
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
            Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {

        if (IsInLayerMask(other.gameObject.layer, enemyLayer))
        {
            var enemyStats = other.gameObject.GetComponent<StatsSystem>();
            if (enemyStats != null)
                enemyStats.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (IsInLayerMask(other.gameObject.layer, obstacleLayer))
            Destroy(gameObject);
    }

    private bool IsInLayerMask(int layer, LayerMask layerMask)
    {
        return (layerMask.value & (1 << layer)) > 0;
    }
}
