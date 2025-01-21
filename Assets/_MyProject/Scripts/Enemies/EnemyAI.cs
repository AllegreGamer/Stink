using UnityEngine;

[RequireComponent(typeof(BaseEnemy))]
public class EnemyAI : MonoBehaviour
{
    private Transform player;
    private BaseEnemy enemyStats;
    private float startY;

    [Header("Movement Settings")]
    [SerializeField] private float avoidanceRadius = 1.5f;    // Raggio per evitare altri nemici
    [SerializeField] private float minPlayerDistance = 0.5f;  // Distanza minima dal player
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float rotationSpeed = 5f;

    private Vector3 currentDirection;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemyStats = GetComponent<BaseEnemy>();
        currentDirection = transform.forward;
        startY = transform.position.y;
    }

    private void Update()
    {
        if (player == null) return;

        Vector3 desiredDirection = CalculateMovementDirection();

        // Applica smooth solo alla rotazione, non alla velocità
        currentDirection = Vector3.Lerp(currentDirection, desiredDirection, Time.deltaTime * rotationSpeed);

        // Movimento a velocità costante con Y bloccata
        Vector3 movement = currentDirection * enemyStats.GetMoveSpeed() * Time.deltaTime;
        movement.y = 0; // Forza il movimento solo su X e Z
        Vector3 newPosition = transform.position + movement;
        newPosition.y = startY; // Mantiene l'altezza originale
        transform.position = newPosition;

        // Rotazione fluida
        if (currentDirection.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(currentDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private Vector3 CalculateMovementDirection()
    {
        Vector3 targetPosition = player.position;
        targetPosition.y = transform.position.y;
        Vector3 directionToPlayer = (targetPosition - transform.position);
        float distanceToPlayer = directionToPlayer.magnitude;

        // Normalizza la direzione verso il player
        directionToPlayer = directionToPlayer.normalized;

        // Se siamo troppo vicini al player, manteniamo la distanza ma continuiamo a seguirlo
        if (distanceToPlayer < minPlayerDistance)
        {
            directionToPlayer = -directionToPlayer;
        }

        // Calcola la forza di evitamento degli altri nemici
        Vector3 avoidanceForce = CalculateAvoidanceForce();

        // Combina le forze
        return (directionToPlayer + avoidanceForce * 0.5f).normalized;
    }

    private Vector3 CalculateAvoidanceForce()
    {
        Vector3 avoidanceForce = Vector3.zero;
        Collider[] nearbyEnemies = Physics.OverlapSphere(transform.position, avoidanceRadius, enemyLayer);

        foreach (Collider enemy in nearbyEnemies)
        {
            if (enemy.gameObject != gameObject)
            {
                Vector3 awayFromEnemy = transform.position - enemy.transform.position;
                float distance = awayFromEnemy.magnitude;

                if (distance < 0.1f) continue;

                float strength = 1f - (distance / avoidanceRadius);
                strength = strength * strength;

                avoidanceForce += awayFromEnemy.normalized * strength;
            }
        }

        return avoidanceForce.normalized;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, avoidanceRadius);

        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, minPlayerDistance);
        }
    }
}