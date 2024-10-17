using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlayerFieldOfView : MonoBehaviour
{
    [Header("Основний огляд")]
    public float viewRadius = 10f;
    [Range(0, 360)]
    public float viewAngle = 90f;

    [Header("Огляд зі спини")]
    public bool enableBackView = false;
    public float backViewRadius = 5f;
    [Range(0, 360)]
    public float backViewAngle = 90f;

    [Header("Налаштування променів")]
    [Range(10, 256)]
    public int rayCount = 100;  // Кількість променів

    [Header("Шари для перевірки зіткнень")]
    public LayerMask obstacleMask;
    public LayerMask enemyMask;

    [Header("Налаштування дебагу")]
    public bool showDebugRays = true;

    private void OnDrawGizmos()
    {
        if (showDebugRays)
        {
            // Основний огляд
            DrawViewGizmos(viewRadius, viewAngle, Color.green);
            
            // Огляд зі спини, якщо включений
            if (enableBackView)
                DrawViewGizmos(backViewRadius, backViewAngle, Color.blue, true);
        }
    }

    void DrawViewGizmos(float radius, float angle, Color color, bool isBackView = false)
    {
        Vector3 viewDirection = DirFromAngle(-angle / 2, isBackView);
        Gizmos.color = color;
        Gizmos.DrawLine(transform.position, transform.position + viewDirection * radius);

        viewDirection = DirFromAngle(angle / 2, isBackView);
        Gizmos.DrawLine(transform.position, transform.position + viewDirection * radius);

        // Промінь по центру
        Gizmos.DrawLine(transform.position, transform.position + DirFromAngle(0, isBackView) * radius);

        // Кут огляду променів
        CastRaysInFov(radius, angle, isBackView);
    }

    void CastRaysInFov(float radius, float angle, bool isBackView)
    {
        float stepAngleSize = angle / rayCount;  // Крок між променями

        for (int i = 0; i <= rayCount; i++)
        {
            float rayAngle = -angle / 2 + stepAngleSize * i;
            Vector3 rayDirection = DirFromAngle(rayAngle, isBackView);
            RaycastHit hit;

            // Випускаємо промінь
            if (Physics.Raycast(transform.position, rayDirection, out hit, radius, obstacleMask))
                Debug.DrawRay(transform.position, rayDirection * hit.distance, Color.blue);
            else if (Physics.Raycast(transform.position, rayDirection, out hit, radius, enemyMask))
                Debug.DrawRay(transform.position, rayDirection * hit.distance, Color.red);
            else
                Debug.DrawRay(transform.position, rayDirection * radius, Color.green);
        }
    }

    Vector3 DirFromAngle(float angleInDegrees, bool isBackView)
    {
        if (!isBackView)
            angleInDegrees += transform.eulerAngles.y;
        else
            angleInDegrees += transform.eulerAngles.y + 180f; // Додаємо 180 градусів для огляду зі спини
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}