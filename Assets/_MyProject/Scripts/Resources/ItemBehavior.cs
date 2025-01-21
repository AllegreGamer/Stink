using UnityEngine;

public class ItemBehavior : MonoBehaviour
{
    [Header("Float Settings")]
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float floatAmplitude = 0.5f;
    [SerializeField] private float rotationSpeed = 45f;

    private Vector3 startPosition;
    private float timeOffset;

    private void Start()
    {
        startPosition = transform.position;
        // Aggiungi un offset casuale per evitare che tutti gli item si muovano sincronizzati
        timeOffset = Random.Range(0f, 2f * Mathf.PI);
    }

    private void Update()
    {
        // Movimento fluttuante sull'asse Y
        float newY = startPosition.y + Mathf.Sin((Time.time + timeOffset) * floatSpeed) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // Rotazione continua sull'asse Y
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    // Opzionale: puoi aggiungere un effetto visivo quando l'oggetto viene raccolto
    private void OnDestroy()
    {
        // Qui puoi aggiungere effetti particellari o audio quando l'oggetto viene raccolto
        // Per ora lo lasciamo vuoto
    }
}