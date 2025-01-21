using UnityEngine;
using System.Collections.Generic;

public enum SkillType
{
    None,
    Spit,
    Gas,
    Water,
    Stink,
    Food,
    Alcohol,
    Diarrhea,
    Beer
}

[System.Serializable]
public class AttackSetup
{
    public GameObject attackObject;    // Il GameObject contenente lo script dell'attacco
    public GameObject triggerObject;   // L'oggetto trigger associato (se presente)
    public int unlockLevel;           // Livello necessario per sbloccare
    public SkillType skillType;       // Tipo di skill associata
}

public class AttackManager : MonoBehaviour
{
    [Header("Attack Setups")]
    [SerializeField] private List<AttackSetup> attacks = new List<AttackSetup>();

    [Header("References")]
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private ResourceManager resourceManager;

    private Dictionary<SkillType, SkillLevel> skillMap;

    private void Awake()
    {
        // Se i riferimenti non sono assegnati, prova a trovarli
        if (playerStats == null)
            playerStats = GetComponentInParent<PlayerStats>();

        if (resourceManager == null)
            resourceManager = GetComponentInParent<ResourceManager>();

        // Verifica i riferimenti necessari
        if (playerStats == null || resourceManager == null)
        {
            Debug.LogError("Required references missing in AttackManager!");
            return;
        }

        // Inizializza la mappa delle skill
        InitializeSkillMap();

        // Inizialmente disattiva tutti gli attacchi
        DisableAllAttacks();

        // Sottoscrivi all'evento di sblocco delle skill
        PlayerStats.OnSkillUnlocked += HandleSkillUnlock;
    }

    private void InitializeSkillMap()
    {
        skillMap = new Dictionary<SkillType, SkillLevel>
        {
            { SkillType.Spit, playerStats.spitSkill },
            { SkillType.Gas, playerStats.gasSkill },
            { SkillType.Water, playerStats.waterSkill },
            { SkillType.Stink, playerStats.stinkSkill },
            { SkillType.Food, playerStats.foodSkill },
            { SkillType.Alcohol, playerStats.alcoholSkill },
            { SkillType.Diarrhea, playerStats.diarrheaSkill },
            { SkillType.Beer, playerStats.beerSkill }
        };
    }

    private void OnDestroy()
    {
        PlayerStats.OnSkillUnlocked -= HandleSkillUnlock;
    }

    private void Start()
    {
        // Verifica lo stato iniziale degli attacchi
        foreach (var attack in attacks)
        {
            UpdateAttackState(attack);
        }
    }

    private void DisableAllAttacks()
    {
        foreach (var attack in attacks)
        {
            if (attack.triggerObject != null)
                attack.triggerObject.SetActive(false);
        }
    }

    private void HandleSkillUnlock(SkillLevel unlockedSkill)
    {
        Debug.Log($"Skill unlock detected in AttackManager");

        // Trova e aggiorna tutti gli attacchi
        foreach (var attack in attacks)
        {
            if (skillMap[attack.skillType] == unlockedSkill)
            {
                Debug.Log($"Found matching attack for skill type: {attack.skillType}");
                UpdateAttackState(attack);
            }
        }
    }

    private void UpdateAttackState(AttackSetup attack)
    {
        if (!skillMap.ContainsKey(attack.skillType))
        {
            Debug.LogError($"Skill type {attack.skillType} not found in skill map!");
            return;
        }

        bool shouldBeActive = skillMap[attack.skillType].isUnlocked;
        Debug.Log($"Updating attack state for {attack.skillType}. Should be active: {shouldBeActive}");

        if (attack.attackObject != null)
        {
            attack.attackObject.SetActive(shouldBeActive);
            Debug.Log($"Attack object {attack.attackObject.name} set to {shouldBeActive}");
        }

        if (attack.triggerObject != null)
        {
            attack.triggerObject.SetActive(shouldBeActive);
            Debug.Log($"Trigger object {attack.triggerObject.name} set to {shouldBeActive}");
        }
    }

    public bool IsAttackUnlocked(GameObject attackObject)
    {
        var attack = attacks.Find(a => a.attackObject == attackObject);
        if (attack == null) return false;

        return skillMap.ContainsKey(attack.skillType) && skillMap[attack.skillType].isUnlocked;
    }

    // Metodi per l'upgrade system
    public void UpgradeAttack(GameObject attackObject, string upgradeType, float value)
    {
        var attack = attacks.Find(a => a.attackObject == attackObject);
        if (attack != null && attack.attackObject != null)
        {
            FartAttack fartAttack = attack.attackObject.GetComponent<FartAttack>();
            if (fartAttack != null)
            {
                switch (upgradeType)
                {
                    case "damage":
                        fartAttack.UpgradeDamage(value);
                        break;
                    case "radius":
                        fartAttack.UpgradeRadius(value);
                        break;
                    case "duration":
                        fartAttack.UpgradeDuration(value);
                        break;
                }
            }
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Verifica che tutti gli attacchi abbiano i riferimenti necessari
        foreach (var attack in attacks)
        {
            if (attack.attackObject == null)
                Debug.LogWarning("Attack object reference missing in AttackManager!");
            if (attack.skillType == SkillType.None)
                Debug.LogWarning("Skill type not set in AttackManager!");
        }
    }
#endif
}