using UnityEngine;

public class DealDamageZone : MonoBehaviour
{
    private BaseEnemy parentEnemy;

    private void Start()
    {
        // Prende il riferimento al BaseEnemy dal parent
        parentEnemy = GetComponentInParent<BaseEnemy>();
        if (parentEnemy == null)
        {
            Debug.LogError("No BaseEnemy found in parent hierarchy!");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && parentEnemy != null)
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                Debug.Log("Player entered damage zone"); // Debug
                parentEnemy.DealDamageToPlayer(other);
            }
        }
    }
}