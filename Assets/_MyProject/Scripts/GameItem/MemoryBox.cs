using UnityEngine;
using System.Collections.Generic;
public class MemoryBox : MonoBehaviour
{
    [SerializeField] private BoxCollider mainCollider; // Collider principale solido
    [SerializeField] private BoxCollider damageCollider; // Trigger collider per danni

    private int fragmentCount;

    private void Awake()
    {
        if (mainCollider == null)
            mainCollider = GetComponent<BoxCollider>();

        // Crea il damage collider se non esiste
        if (damageCollider == null)
        {
            damageCollider = gameObject.AddComponent<BoxCollider>();
            damageCollider.isTrigger = true;
            damageCollider.size = mainCollider.size * 1.1f; // Leggermente più grande
        }
    }

    public void Initialize(int fragments)
    {
        fragmentCount = fragments;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Controlla se il collider appartiene a un proiettile
        if (other.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            DropFragments();
            Destroy(gameObject);
        }
    }

    [Header("Fragment Settings")]
    [SerializeField] private GameObject fragmentPrefab;
    [SerializeField] private float fragmentSpreadRadius = 2f;

    private void DropFragments()
    {
        for (int i = 0; i < fragmentCount; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * fragmentSpreadRadius;
            Vector3 spawnPos = transform.position + new Vector3(randomCircle.x, 0.5f, randomCircle.y);

            Instantiate(fragmentPrefab, spawnPos, Quaternion.identity);
        }
    }
}