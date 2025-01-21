using UnityEngine;

[System.Serializable]
public class GameEvent
{
    public string eventName;
    public int levelNumber;        // In quale livello si attiva l'evento
    public float timeToTrigger;    // Quando si attiva l'evento nel livello (in minuti)
    public GameEventType eventType;
    [Range(0, 100)]
    public int enemyCountIncrease;
    public GameObject enemyPrefabToSpawn;
    public bool hasTriggered;

    // Parametri aggiuntivi per ogni tipo di evento
    public float enemyHealthMultiplier = 1f;
    public float enemyDamageMultiplier = 1f;
    public float enemySpeedMultiplier = 1f;
}

public enum GameEventType
{
    IncreaseEnemyCount,
    SpawnNewEnemyType,
    BossEvent,
    CustomEvent
}