using UnityEngine;

public class PlayerChase : MonoBehaviour
{
    private EnemyFieldOfView fov;
    [SerializeField] private float speed = 5f;
    private Weapons weapon;
    void Start()
    {
        fov = GetComponent<EnemyFieldOfView>();
        weapon = GetComponentInChildren<Weapons>();
    }

    // Update is called once per frame
    void Update()
    {
        if (fov.canSeePlayer)
        { 
            if (fov.playerRef == null) return;
            
            Vector3 directionToPlayer = (fov.playerRef.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(directionToPlayer.x, directionToPlayer.z) * Mathf.Rad2Deg;
            float distanceToPlayer = Vector3.Distance(transform.position, fov.playerRef.transform.position);
            float attackRange = fov.radius / 2; 
            float visibleRange = fov.radius; 

            if (weapon == null)
            {
                transform.position += (speed * directionToPlayer) * Time.deltaTime;
                transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
            }
            else 
            {
                if (distanceToPlayer < attackRange)
                {
                    transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
                    weapon.target = fov.playerRef.transform;
                    weapon.EnemyInput();
                } 
                else if (distanceToPlayer < visibleRange)
                {
                    transform.position += (speed * directionToPlayer) * Time.deltaTime;
                    transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
                }
            }
        } 
    }
}
