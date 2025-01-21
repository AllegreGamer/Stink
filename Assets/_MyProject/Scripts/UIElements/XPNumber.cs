using UnityEngine;
using TMPro;

public class XPNumber : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float lifetime = 1f;
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private Vector3 offset = new Vector3(0, 1f, 0);

    [Header("Text Settings")]
    [SerializeField] private Color xpColor = Color.yellow;
    [SerializeField] private float textSize = 3f;

    private TextMeshPro textMesh;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        if (textMesh != null)
        {
            textMesh.color = xpColor;
            textMesh.fontSize = textSize;
            textMesh.alignment = TextAlignmentOptions.Center;
        }
        else
        {
            Debug.LogError("TextMeshPro component not found!");
        }
    }

    public void Setup(float xpAmount)
    {
        if (textMesh != null)
        {
            textMesh.text = $"+{Mathf.RoundToInt(xpAmount)}XP";
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