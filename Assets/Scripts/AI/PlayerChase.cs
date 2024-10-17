using UnityEngine;

public class PlayerChase : MonoBehaviour
{
    private EnemyFieldOfView fov;
    void Start()
    {
        fov = GetComponent<EnemyFieldOfView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (fov.canSeePlayer)
        {
            // Move towards player
            Vector3 directionToPlayer = (fov.playerRef.transform.position - transform.position).normalized;
            transform.position += (5f * directionToPlayer) * Time.deltaTime;

            // Rotate towards player
            float angle = Mathf.Atan2(directionToPlayer.x, directionToPlayer.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
        } 
    }
}
