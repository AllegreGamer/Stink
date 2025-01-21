using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [Header("Urine Settings")]
    [SerializeField] private float maxUrine = 10000f;
    [SerializeField] private float urineRegenRate = 0f;
    [SerializeField] private float currentUrine = 0f;

    [Header("Fart Settings")]  // Rinominato da Stamina a Gas/Fart
    [SerializeField] private float maxFart = 100f;
    [SerializeField] private float fartRegenRate = 10f;
    [SerializeField] private float currentFart = 0f;

    [Header("Food Settings")]
    [SerializeField] private float maxFood = 100f;
    [SerializeField] private float foodRegenRate = 0f;
    [SerializeField] private float currentFood = 0f;

    [Header("Alcohol Settings")]
    [SerializeField] private float maxAlcohol = 100f;
    [SerializeField] private float alcoholRegenRate = 0f;
    [SerializeField] private float currentAlcohol = 0f;

    [Header("Burp Settings")]
    [SerializeField] private float maxBurp = 100f;
    [SerializeField] private float burpRegenRate = 0f;
    [SerializeField] private float currentBurp = 0f;

    private PlayerStats playerStats;
    private bool urineUnlocked = false;
    private bool gasUnlocked = false;    // Rinominato da stamina a gas
    private bool foodUnlocked = false;
    private bool alcoholUnlocked = false;
    private bool burpUnlocked = false;

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats not found on ResourceManager!");
        }
    }

    private void Start()
    {
        if (!Application.isEditor)
        {
            // In build, inizializza tutto a 0
            currentUrine = 0f;
            currentFart = 0f;
            currentFood = 0f;
            currentAlcohol = 0f;
            currentBurp = 0f;
        }

        // Controlla quali abilità sono già sbloccate
        CheckInitialUnlocks();
    }

    private void OnEnable()
    {
        if (playerStats != null)
        {
            PlayerStats.OnSkillUnlocked += HandleSkillUnlock;
        }
    }

    private void OnDisable()
    {
        if (playerStats != null)
        {
            PlayerStats.OnSkillUnlocked -= HandleSkillUnlock;
        }
    }

    private void CheckInitialUnlocks()
    {
        if (playerStats != null)
        {
            urineUnlocked = playerStats.waterSkill.isUnlocked;
            gasUnlocked = playerStats.gasSkill.isUnlocked;    // Controllo per il gas skill
            foodUnlocked = playerStats.foodSkill.isUnlocked;
            alcoholUnlocked = playerStats.alcoholSkill.isUnlocked;
            burpUnlocked = playerStats.beerSkill.isUnlocked;
        }
    }

    private void HandleSkillUnlock(SkillLevel skill)
    {
        if (skill == playerStats.waterSkill)
        {
            urineUnlocked = true;
            if (!Application.isEditor) currentUrine = 0f;
        }
        else if (skill == playerStats.gasSkill)    // Gestisce lo sblocco del gas
        {
            gasUnlocked = true;
            if (!Application.isEditor) currentFart = 0f;
        }
        else if (skill == playerStats.foodSkill)
        {
            foodUnlocked = true;
            if (!Application.isEditor) currentFood = 0f;
        }
        else if (skill == playerStats.alcoholSkill)
        {
            alcoholUnlocked = true;
            if (!Application.isEditor) currentAlcohol = 0f;
        }
        else if (skill == playerStats.beerSkill)
        {
            burpUnlocked = true;
            if (!Application.isEditor) currentBurp = 0f;
        }
    }

    private void Update()
    {
        // Applica rigenerazione solo alle risorse sbloccate
        if (urineUnlocked) RegenerateResource(ref currentUrine, maxUrine, urineRegenRate);
        if (gasUnlocked) RegenerateResource(ref currentFart, maxFart, fartRegenRate);
        if (foodUnlocked) RegenerateResource(ref currentFood, maxFood, foodRegenRate);
        if (alcoholUnlocked) RegenerateResource(ref currentAlcohol, maxAlcohol, alcoholRegenRate);
        if (burpUnlocked) RegenerateResource(ref currentBurp, maxBurp, burpRegenRate);
    }

    private void RegenerateResource(ref float currentValue, float maxValue, float regenRate)
    {
        if (regenRate > 0)
        {
            currentValue = Mathf.Min(maxValue, currentValue + (regenRate * Time.deltaTime));
        }
    }

    #region Resource Consumption
    public bool ConsumeUrine(float amount)
    {
        if (!urineUnlocked) return false;
        if (currentUrine >= amount)
        {
            currentUrine -= amount;
            return true;
        }
        return false;
    }

    public bool ConsumeFart(float amount)    // Rinominato da ConsumeStamina a ConsumeGas
    {
        if (!gasUnlocked) return false;
        if (currentFart >= amount)
        {
            currentFart -= amount;
            return true;
        }
        return false;
    }

    public bool ConsumeFood(float amount)
    {
        if (!foodUnlocked) return false;
        if (currentFood >= amount)
        {
            currentFood -= amount;
            return true;
        }
        return false;
    }

    public bool ConsumeAlcohol(float amount)
    {
        if (!alcoholUnlocked) return false;
        if (currentAlcohol >= amount)
        {
            currentAlcohol -= amount;
            return true;
        }
        return false;
    }

    public bool ConsumeBurp(float amount)
    {
        if (!burpUnlocked) return false;
        if (currentBurp >= amount)
        {
            currentBurp -= amount;
            return true;
        }
        return false;
    }
    #endregion

    #region Resource Addition
    public void AddUrine(float amount)
    {
        if (!urineUnlocked) return;
        currentUrine = Mathf.Min(maxUrine, currentUrine + amount);
    }

    public void AddFart(float amount)    // Rinominato da AddStamina a AddGas
    {
        if (!gasUnlocked) return;
        currentFart = Mathf.Min(maxFart, currentFart + amount);
    }

    public void AddFood(float amount)
    {
        if (!foodUnlocked) return;
        currentFood = Mathf.Min(maxFood, currentFood + amount);
    }

    public void AddAlcohol(float amount)
    {
        if (!alcoholUnlocked) return;
        currentAlcohol = Mathf.Min(maxAlcohol, currentAlcohol + amount);
    }

    public void AddBurp(float amount)
    {
        if (!burpUnlocked) return;
        currentBurp = Mathf.Min(maxBurp, currentBurp + amount);
    }
    #endregion

    #region Getters
    public float GetCurrentUrine() => currentUrine;
    public float GetMaxUrine() => maxUrine;
    public float GetCurrentFart() => currentFart;        // Rinominato
    public float GetMaxFart() => maxFart;               // Rinominato
    public float GetCurrentFood() => currentFood;
    public float GetMaxFood() => maxFood;
    public float GetCurrentAlcohol() => currentAlcohol;
    public float GetMaxAlcohol() => maxAlcohol;
    public float GetCurrentBurp() => currentBurp;
    public float GetMaxBurp() => maxBurp;

    public float GetUrinePercentage() => currentUrine / maxUrine;
    public float GetFartPercentage() => currentFart / maxFart;    // Rinominato
    public float GetFoodPercentage() => currentFood / maxFood;
    public float GetAlcoholPercentage() => currentAlcohol / maxAlcohol;
    public float GetBurpPercentage() => currentBurp / maxBurp;

    public bool IsUrineUnlocked() => urineUnlocked;
    public bool IsFartUnlocked() => gasUnlocked;    // Rinominato
    public bool IsFoodUnlocked() => foodUnlocked;
    public bool IsAlcoholUnlocked() => alcoholUnlocked;
    public bool IsBurpUnlocked() => burpUnlocked;
    #endregion
}