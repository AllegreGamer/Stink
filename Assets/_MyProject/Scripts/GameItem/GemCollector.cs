using UnityEngine;

public class GemCollector : MonoBehaviour
{
    [Header("Collection Settings")]
    [SerializeField] private float baseCollectionRadius = 3f;
    [SerializeField] private float maxCollectionRadius = 8f;
    [SerializeField] private LayerMask gemLayer;

    private float currentCollectionRadius;
    private PlayerStats playerStats;

    private void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats not found on GemCollector!");
        }

        UpdateCollectionStats();
    }

    private void Update()
    {
        // Trova tutte le gemme nel raggio
        Collider[] nearbyGems = Physics.OverlapSphere(transform.position, currentCollectionRadius, gemLayer);

        foreach (Collider gemCollider in nearbyGems)
        {
            XPGem gem = gemCollider.GetComponent<XPGem>();
            if (gem != null)
            {
                // Calcola il moltiplicatore di velocità basato sulla distanza
                float distanceToGem = Vector3.Distance(transform.position, gem.transform.position);
                float speedMultiplier = 1f + ((currentCollectionRadius - distanceToGem) / currentCollectionRadius);

                // Attiva l'attrazione della gemma
                gem.StartAttraction(transform, speedMultiplier);
            }
        }
    }

    public void UpdateCollectionStats()
    {
        int gemAttractionLevel = playerStats.GetGemAttractionLevel();
        float radiusIncrease = (maxCollectionRadius - baseCollectionRadius) * (gemAttractionLevel / 4f);
        currentCollectionRadius = baseCollectionRadius + radiusIncrease;
    }

    private void OnDrawGizmosSelected()
    {
        // Visualizza il raggio di raccolta nell'editor
        Gizmos.color = new Color(1f, 1f, 0f, 0.2f);
        Gizmos.DrawWireSphere(transform.position, currentCollectionRadius);
    }
}