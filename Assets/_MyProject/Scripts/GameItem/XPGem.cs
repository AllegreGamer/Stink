using UnityEngine;

public class XPGem : MonoBehaviour
{
    public enum GemType
    {
        Small,  // Blu
        Medium, // Verde
        Large   // Rossa
    }

    [Header("Gem Settings")]
    [SerializeField] private GemType gemType;
    [SerializeField] private float xpValue = 10f;

    [Header("Movement Settings")]
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private float floatAmplitude = 0.5f;
    [SerializeField] private float moveSpeed = 5f; // Velocità di movimento verso il player

    [Header("Trail Settings")]
    [SerializeField] private TrailRenderer trailRenderer;

    [SerializeField] private GameObject xpNumberPrefab;

    private Vector3 startPosition;
    private float timeOffset;
    private bool isAttracted = false;
    private Transform targetPlayer;

    private void Start()
    {
        startPosition = transform.position;
        timeOffset = Random.Range(0f, 2f * Mathf.PI);

        if (trailRenderer == null)
        {
            trailRenderer = GetComponentInChildren<TrailRenderer>();
        }

        if (trailRenderer != null)
        {
            trailRenderer.enabled = false;
        }
    }

    private void Update()
    {
        if (isAttracted && targetPlayer != null)
        {
            // Movimento verso il player quando attratto
            Vector3 directionToPlayer = (targetPlayer.position - transform.position).normalized;
            transform.position += directionToPlayer * moveSpeed * Time.deltaTime;
        }
        else
        {
            // Movimento fluttuante normale
            float newY = startPosition.y + Mathf.Sin((Time.time + timeOffset) * floatSpeed) * floatAmplitude;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }

        // Rotazione costante
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                if (xpNumberPrefab != null)
                {
                    GameObject xpNumberObj = Instantiate(xpNumberPrefab, transform.position, Quaternion.identity);
                    XPNumber xpNumber = xpNumberObj.GetComponent<XPNumber>();
                    if (xpNumber != null)
                    {
                        xpNumber.Setup(xpValue);
                    }
                }

                playerStats.AddExperience(xpValue);
                Destroy(gameObject);
            }
        }
    }

    public void StartAttraction(Transform player, float speedMultiplier = 1f)
    {
        if (!isAttracted)
        {
            isAttracted = true;
            targetPlayer = player;
            moveSpeed *= speedMultiplier;

            if (trailRenderer != null)
            {
                trailRenderer.enabled = true;
            }
        }
    }

    public void SetXPValue(float value)
    {
        xpValue = value;
    }
}