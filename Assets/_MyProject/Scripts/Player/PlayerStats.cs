using UnityEngine;
using GameCreator.Runtime.Characters;
using System;
using System.Collections.Generic;

[Serializable]
public class SkillLevel
{
    public bool isUnlocked;
    public int level;
    public int maxLevel = 4;
    public float baseValue;
    public float currentValue;
    public List<string> activeEnhancements = new List<string>();

    public SkillLevel(bool unlocked = false, int startLevel = 0, float baseVal = 0, int maxLvl = 4)
    {
        isUnlocked = unlocked;
        level = startLevel;
        baseValue = baseVal;
        currentValue = baseVal;
        maxLevel = maxLvl;
    }
}

public class PlayerStats : MonoBehaviour
{
    #region Basic Stats
    [Header("Level and XP")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private float currentXP = 0;
    [SerializeField] private float xpToNextLevel = 100;
    [SerializeField] private float xpGrowthRate = 1.5f;

    private PlayerHealth playerHealth;
    #endregion

    #region Character Attributes
    //[Header("Attributes")]
    //[SerializeField] private SkillLevel stinkLevel = new SkillLevel(false, 0, 1f);
    #endregion

    #region Skills and Abilities
    [Header("Combat Skills")]
    public SkillLevel spitSkill = new SkillLevel(true, 1, 10f);     // Livello 1 (Default)
    public SkillLevel gasSkill = new SkillLevel(false, 0, 10f);     // Livello 2
    public SkillLevel waterSkill = new SkillLevel(false, 0, 10f);   // Livello 3
    public SkillLevel stinkSkill = new SkillLevel(false, 0, 10f);   // Livello 4
    public SkillLevel foodSkill = new SkillLevel(false, 0, 10f);    // Livello 5
    public SkillLevel alcoholSkill = new SkillLevel(false, 0, 10f); // Livello 6
    public SkillLevel diarrheaSkill = new SkillLevel(false, 0, 10f);// Livello 7
    public SkillLevel beerSkill = new SkillLevel(false, 0, 1f, 3);  // Livello 8, max 3 birre

    [Header("Utility Skills")]
    public SkillLevel gemAttractionSkill = new SkillLevel(true, 1, 1f, 4); // Inizia al livello 1, max 4
    #endregion

    private Character character;
    private ResourceManager resourceManager;

    // Eventi Unity
    public static event Action<SkillLevel> OnSkillUnlocked;

    private void Awake()
    {
        character = GetComponent<Character>();
        resourceManager = GetComponent<ResourceManager>();
        playerHealth = GetComponent<PlayerHealth>();

        if (character == null || resourceManager == null || playerHealth == null)
        {
            Debug.LogError("Required components missing on PlayerStats!");
        }
    }

    #region Level Management
    public void AddExperience(float xpAmount)
    {
        currentXP += xpAmount;
        while (currentXP >= xpToNextLevel && currentLevel < 99)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentLevel++;
        currentXP -= xpToNextLevel;
        xpToNextLevel *= xpGrowthRate;

        // Sblocco abilità basato sul livello
        UnlockAbilitiesByLevel();

        if (currentLevel >= 9)
        {
            OfferUpgradeChoices();
        }
    }

    private void UnlockAbilitiesByLevel()
    {
        Debug.Log($"Checking abilities unlock for level {currentLevel}");

        switch (currentLevel)
        {
            case 2:
                gasSkill.isUnlocked = true;
                gasSkill.level = 1;
                OnSkillUnlocked?.Invoke(gasSkill);
                break;
            case 3:
                waterSkill.isUnlocked = true;
                waterSkill.level = 1;
                OnSkillUnlocked?.Invoke(waterSkill);
                break;
            case 4:
                stinkSkill.isUnlocked = true;
                stinkSkill.level = 1;
                OnSkillUnlocked?.Invoke(stinkSkill);
                break;
            case 5:
                foodSkill.isUnlocked = true;
                foodSkill.level = 1;
                OnSkillUnlocked?.Invoke(foodSkill);
                break;
            case 6:
                alcoholSkill.isUnlocked = true;
                alcoholSkill.level = 1;
                OnSkillUnlocked?.Invoke(alcoholSkill);
                break;
            case 7:
                diarrheaSkill.isUnlocked = true;
                diarrheaSkill.level = 1;
                OnSkillUnlocked?.Invoke(diarrheaSkill);
                break;
            case 8:
                beerSkill.isUnlocked = true;
                beerSkill.level = 1;
                OnSkillUnlocked?.Invoke(beerSkill);
                break;
        }
    }

    private void OfferUpgradeChoices()
    {
        Debug.Log("Level up! Choose your upgrade!");
    }
    #endregion

    #region Skill Management
    public void UpgradeSkill(SkillLevel skill, string enhancementName = "")
    {
        if (skill.isUnlocked && skill.level < skill.maxLevel)
        {
            skill.level++;
            skill.currentValue = skill.baseValue * (1 + (skill.level * 0.2f));
            if (!string.IsNullOrEmpty(enhancementName))
            {
                skill.activeEnhancements.Add($"{enhancementName} Lv.{skill.level}");
            }

            // Se stiamo aggiornando l'abilità di attrazione delle gemme, aggiorna il GemCollector
            if (skill == gemAttractionSkill)
            {
                GemCollector gemCollector = GetComponent<GemCollector>();
                if (gemCollector != null)
                {
                    gemCollector.UpdateCollectionStats();
                }
            }
        }
    }

    public int GetGemAttractionLevel()
    {
        return gemAttractionSkill.level;
    }
    #endregion

    #region Getters
    public int GetLevel() => currentLevel;
    public float GetCurrentXP() => currentXP;
    public float GetXPToNextLevel() => xpToNextLevel;
    public float GetXPProgress() => currentXP / xpToNextLevel;
    public float GetCurrentHealth() => playerHealth ? playerHealth.GetCurrentHealth() : 0f;
    public float GetMaxHealth() => playerHealth ? playerHealth.GetMaxHealth() : 0f;
    public float GetHealthPercentage() => playerHealth ? playerHealth.GetHealthPercentage() : 0f;
    //public SkillLevel GetStinkLevel() => stinkLevel;

    public bool IsSkillUnlocked(SkillLevel skill) => skill.isUnlocked;
    public int GetSkillLevel(SkillLevel skill) => skill.level;
    public List<string> GetSkillEnhancements(SkillLevel skill) => skill.activeEnhancements;
    #endregion
}