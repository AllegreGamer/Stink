using UnityEngine;

public class GrudgeFragment : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float floatHeight = 0.5f;
    [SerializeField] private float rotationSpeed = 90f;

    private Vector3 startPosition;
    private float timeOffset;

    private void Start()
    {
        startPosition = transform.position;
        timeOffset = Random.Range(0f, 360f);
    }

    private void Update()
    {
        // Movimento fluttuante
        float newY = startPosition.y + Mathf.Sin((Time.time + timeOffset) * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // Rotazione
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FragmentManager.Instance.AddFragments(1);
            Destroy(gameObject);
        }
    }
}