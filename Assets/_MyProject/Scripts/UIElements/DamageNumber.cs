using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float lifetime = 1f;
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private Vector3 offset = new Vector3(0, 1f, 0);

    [Header("Text Settings")]
    [SerializeField] private Color textColor = Color.red;
    [SerializeField] private float minTextSize = 3f;
    [SerializeField] private float maxTextSize = 5f;
    [SerializeField] private float sizeDamageThreshold = 50f; // Danno oltre il quale il testo sarà più grande

    private TextMeshPro textMesh;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        if (textMesh != null)
        {
            textMesh.color = textColor;
            textMesh.alignment = TextAlignmentOptions.Center;
        }
        else
        {
            Debug.LogError("TextMeshPro component not found!");
        }
    }

    public void Setup(float damageAmount)
    {
        if (textMesh != null)
        {
            // Arrotonda il danno a numeri interi
            int roundedDamage = Mathf.RoundToInt(damageAmount);
            textMesh.text = roundedDamage.ToString();

            // Scala il testo in base al danno
            float normalizedDamage = Mathf.Min(damageAmount / sizeDamageThreshold, 1f);
            float fontSize = Mathf.Lerp(minTextSize, maxTextSize, normalizedDamage);
            textMesh.fontSize = fontSize;

            transform.position += offset;
            transform.rotation = Camera.main.transform.rotation;
        }

        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
        transform.rotation = Camera.main.transform.rotation;
    }
}