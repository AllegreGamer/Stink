using UnityEngine;

public class TearDropper : MonoBehaviour
{
    [Header("Tear Settings")]
    [SerializeField] private GameObject tearPrefab;
    [SerializeField] private float dropOffset = 0.5f;
    [SerializeField] private bool randomizePosition = true;

    [Header("Drop Settings")]
    [SerializeField] private float dropHeight = 0.5f;

    public void DropTear()
    {
        if (tearPrefab != null)
        {
            Vector3 dropPosition = transform.position;

            if (randomizePosition)
            {
                dropPosition += new Vector3(
                    Random.Range(-dropOffset, dropOffset),
                    dropHeight,
                    Random.Range(-dropOffset, dropOffset)
                );
            }
            else
            {
                dropPosition.y += dropHeight;
            }

            Instantiate(tearPrefab, dropPosition, Quaternion.identity);
        }
    }
}