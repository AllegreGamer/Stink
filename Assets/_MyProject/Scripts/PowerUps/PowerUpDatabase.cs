using UnityEngine;
using System.Collections.Generic; // Necessario per List<>

[System.Serializable]
public class PowerUpConfig
{
    public PowerUpId id;

    [Header("Shop Data")]
    public string name;
    public string description;
    public int cost;
    public Sprite icon;

    [Header("Gameplay Data")]
    public GameObject prefab;
    public float spawnProbability;
    public float spawnFrequency;
}

[CreateAssetMenu(fileName = "PowerUpDatabase", menuName = "Game/Power Up Database")]
public class PowerUpDatabase : ScriptableObject
{
    public List<PowerUpConfig> powerUps;

    // Metodo helper completo (non più con ...)
    public PowerUpConfig GetPowerUpConfig(PowerUpId id)
    {
        return powerUps.Find(p => p.id == id);
    }
}