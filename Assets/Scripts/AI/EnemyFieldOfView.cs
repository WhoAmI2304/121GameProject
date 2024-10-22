using System;
using System.Collections;
using UnityEngine;

public class EnemyFieldOfView : MonoBehaviour
{
    [SerializeField] public float radius;
    [SerializeField] [Range(0,360)] private float angle;

    public GameObject playerRef;

    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstructionMask;

    public bool canSeePlayer;

    private void Start()
    {
        StartCoroutine(FOVRoutine());
    }

    private void Update(){
        playerRef = GameObject.FindGameObjectWithTag("Player");
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                    canSeePlayer = true;
                else
                    canSeePlayer = false;
            }
            else
                canSeePlayer = false;
        }
        else if (canSeePlayer)
            canSeePlayer = false;
    }
}
