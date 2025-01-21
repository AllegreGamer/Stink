using UnityEngine;

public class TearCollector : MonoBehaviour
{
    private static TearCollector instance;
    public static TearCollector Instance => instance;

    [Header("Collection Settings")]
    [SerializeField] private float collectionRadius = 5f;
    private int currentTears = 0;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        // Trova tutte le lacrime nel raggio
        Collider[] nearbyTears = Physics.OverlapSphere(transform.position, collectionRadius, LayerMask.GetMask("Tear"));

        foreach (Collider tearCollider in nearbyTears)
        {
            CrystalTear tear = tearCollider.GetComponent<CrystalTear>();
            if (tear != null)
            {
                tear.StartAttraction(transform);
            }
        }
    }

    public void CollectTear()
    {
        currentTears++;
    }

    public void RemoveTears(int amount)
    {
        currentTears = Mathf.Max(0, currentTears - amount);
    }

    public int GetCurrentTears() => currentTears;

    public void ResetTears()
    {
        currentTears = 0;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.5f, 0.8f, 1f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, collectionRadius);
    }
}