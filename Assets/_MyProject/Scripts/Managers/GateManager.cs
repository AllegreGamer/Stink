using UnityEngine;
using System.Collections;

public class GateManager : MonoBehaviour
{
    private static GateManager instance;
    public static GateManager Instance => instance;

    [Header("Level Management")]
    [SerializeField] private GameObject[] levelObjects;
    [SerializeField] private float extraTimePerLevel = 5f * 60f; // 5 minuti in secondi

    private int currentLevelIndex = 0;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // Attiva solo il primo livello
        for (int i = 0; i < levelObjects.Length; i++)
        {
            levelObjects[i].SetActive(i == 0);
        }
    }

    public void OnPlayerFall(Collider player)
    {
        // Verifica che sia effettivamente il player
        if (!player.CompareTag("Player")) return;

        Debug.Log("Player fell through the gate!");

        // Aggiungi tempo al timer
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.AddTime(extraTimePerLevel);
        }

        // Passa al livello successivo
        StartCoroutine(TransitionToNextLevel());
    }

    private IEnumerator TransitionToNextLevel()
    {
        // Fade out opzionale qui
        yield return new WaitForSeconds(1f);

        // Disattiva il livello corrente
        if (currentLevelIndex < levelObjects.Length)
            levelObjects[currentLevelIndex].SetActive(false);

        currentLevelIndex++;

        // Attiva il nuovo livello se esiste
        if (currentLevelIndex < levelObjects.Length)
        {
            levelObjects[currentLevelIndex].SetActive(true);
        }
        else
        {
            // Gestione fine gioco o boss level
            Debug.Log("No more levels! Game Complete or Boss Time!");
        }
    }
}