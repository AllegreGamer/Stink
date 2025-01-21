using UnityEngine;
using System.Collections;

public class LevelTransitionTrigger : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float timeToAdd = 300f; // 5 minuti in secondi
    [SerializeField] private GameObject nextLevel;
    [SerializeField] private GameObject currentLevel;

    [SerializeField] private EnemySpawner enemySpawner; // Aggiungi questo riferimento

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;

            // Pulisci tutti i nemici prima della transizione
            if (enemySpawner != null)
            {
                enemySpawner.ClearAllEnemies();
            }

            // Resetta il contatore delle lacrime
            if (TearCollector.Instance != null)
            {
                TearCollector.Instance.ResetTears();
            }

            // Aggiunge tempo al timer
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddTime(timeToAdd);

                if (nextLevel != null)
                {
                    nextLevel.SetActive(true);
                    GameManager.Instance.AdvanceToNextLevel();
                    if (currentLevel != null)
                    {
                        StartCoroutine(DisableLevelDelayed(currentLevel));
                    }
                }
                else
                {
                    GameManager.Instance.GameComplete();
                }
            }
        }
    }

    private System.Collections.IEnumerator DisableLevelDelayed(GameObject level)
    {
        yield return new WaitForSeconds(2f);
        level.SetActive(false);
    }
}