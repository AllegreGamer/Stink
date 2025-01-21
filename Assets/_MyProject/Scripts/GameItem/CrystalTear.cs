using UnityEngine;
using System.Collections;

public class CrystalTear : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float floatAmplitude = 0.5f;
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private float attractionSpeed = 5f;

    [Header("Visual Settings")]
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private GameObject collectEffectPrefab;

    private Vector3 startPosition;
    private float timeOffset;
    private bool isAttracting = false;
    private Transform targetPlayer;

    private void Start()
    {
        startPosition = transform.position;
        timeOffset = Random.Range(0f, 2f * Mathf.PI);

        if (trailRenderer != null)
        {
            trailRenderer.enabled = false;
        }
    }

    private void Update()
    {
        if (isAttracting && targetPlayer != null)
        {
            // Movimento verso il player quando attratto
            Vector3 directionToPlayer = (targetPlayer.position - transform.position).normalized;
            transform.position += directionToPlayer * attractionSpeed * Time.deltaTime;
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
            TearCollector tearCollector = other.GetComponent<TearCollector>();
            if (tearCollector != null)
            {
                tearCollector.CollectTear();
                if (collectEffectPrefab != null)
                {
                    Instantiate(collectEffectPrefab, transform.position, Quaternion.identity);
                }
                Destroy(gameObject);
            }
        }
    }

    public void StartAttraction(Transform player)
    {
        if (!isAttracting)
        {
            isAttracting = true;
            targetPlayer = player;

            if (trailRenderer != null)
            {
                trailRenderer.enabled = true;
            }
        }
    }
}