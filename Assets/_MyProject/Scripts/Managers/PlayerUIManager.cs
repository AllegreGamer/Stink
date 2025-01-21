using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerUIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private UnlockNotification unlockNotification;

    [Header("Experience UI")]
    [SerializeField] private Image experienceBar;
    [SerializeField] private TextMeshProUGUI levelText;

    [Header("Health UI")]
    [SerializeField] private Image healthBar;
    [SerializeField] private Image healthStall;
    [SerializeField] private float stallDelay = 0.5f;
    [SerializeField] private float stallSpeed = 2f;

    [Header("Resource Bars")]
    [SerializeField] private Image fartBar;
    [SerializeField] private Image liquidBar;
    [SerializeField] private Image foodBar;
    [SerializeField] private Image burpBar;
    [SerializeField] private Image alcoholBar;

    [Header("Resource Bar Parents")]
    [SerializeField] private GameObject fartParent;    // Riferimento al GameObject "Fart"
    [SerializeField] private GameObject liquidParent;  // Riferimento al GameObject "Liquid"
    [SerializeField] private GameObject foodParent;    // Riferimento al GameObject "Food"
    [SerializeField] private GameObject burpParent;    // Riferimento al GameObject "Burp"
    [SerializeField] private GameObject alcoholParent; // Riferimento al GameObject "Alcohol"


    [Header("Colors")]
    [SerializeField] private Color healthBarColor = Color.red;
    [SerializeField] private Color healthStallColor = Color.white;
    [SerializeField] private Color fartBarColor;
    [SerializeField] private Color liquidBarColor;
    [SerializeField] private Color foodBarColor;
    [SerializeField] private Color burpBarColor;
    [SerializeField] private Color alcoholBarColor;

    private float targetStallAmount = 1f;
    private float stallDelayTimer = 0f;
    private float currentStallAmount = 1f;
    private float lastHealthPercentage = 1f;

    private void Start()
    {
        InitializeReferences();
        SetupUIComponents();
        InitializeResourceBarsVisibility(); // Aggiungi questa chiamata

        // Sottoscrivi all'evento di sblocco delle skill
        PlayerStats.OnSkillUnlocked += HandleSkillUnlock;
    }

    private void OnEnable()
    {
        StartCoroutine(InitializeHealthBarsNextFrame());
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHealthUI;
        }

        if (playerStats != null)
        {
            PlayerStats.OnSkillUnlocked -= HandleSkillUnlock;
        }
    }

    private void InitializeReferences()
    {
        if (playerStats == null)
            playerStats = FindObjectOfType<PlayerStats>();

        if (playerHealth == null)
            playerHealth = FindObjectOfType<PlayerHealth>();

        if (resourceManager == null)
            resourceManager = FindObjectOfType<ResourceManager>();

        // Cerca riferimenti UI se non assegnati
        if (experienceBar == null)
            experienceBar = transform.Find("Progress/Experience/ExperienceBar")?.GetComponent<Image>();

        if (levelText == null)
            levelText = transform.Find("Progress/Level/Value")?.GetComponent<TextMeshProUGUI>();

        if (healthBar == null)
            healthBar = transform.Find("Health/Health_Bar")?.GetComponent<Image>();

        if (healthStall == null)
            healthStall = transform.Find("Health/Health_Stall")?.GetComponent<Image>();

        // Cerca riferimenti per le barre delle risorse
        if (fartBar == null)
            fartBar = transform.Find("Fart/Fart_Bar")?.GetComponent<Image>();

        if (liquidBar == null)
            liquidBar = transform.Find("Liquid/Liquid_Bar")?.GetComponent<Image>();

        if (foodBar == null)
            foodBar = transform.Find("Food/Food_Bar")?.GetComponent<Image>();

        if (burpBar == null)
            burpBar = transform.Find("Burp/Burp_Bar")?.GetComponent<Image>();

        if (alcoholBar == null)
            alcoholBar = transform.Find("Alcohol/Alcohol_Bar")?.GetComponent<Image>();

        // Aggiungi questi controlli per i parent
        if (fartParent == null)
            fartParent = transform.Find("Fart")?.gameObject;
        if (liquidParent == null)
            liquidParent = transform.Find("Liquid")?.gameObject;
        if (foodParent == null)
            foodParent = transform.Find("Food")?.gameObject;
        if (burpParent == null)
            burpParent = transform.Find("Burp")?.gameObject;
        if (alcoholParent == null)
            alcoholParent = transform.Find("Alcohol")?.gameObject;
    }

    private void HandleSkillUnlock(SkillLevel skill)
    {
        Debug.Log($"HandleSkillUnlock called for {GetSkillName(skill)}");

        // Verifica se unlockNotification è assegnato
        if (unlockNotification == null)
        {
            Debug.LogError("UnlockNotification reference is missing in PlayerUIManager!");
            return;
        }

        if (playerStats == null)
        {
            Debug.LogError("PlayerStats reference is missing in PlayerUIManager!");
            return;
        }

        // Determina quale potere è stato sbloccato e aggiorna l'UI di conseguenza
        if (skill == playerStats.gasSkill && fartParent != null)
        {
            Debug.Log("Activating Fart UI");
            fartParent.SetActive(true);
        }
        else if (skill == playerStats.waterSkill && liquidParent != null)
        {
            Debug.Log("Activating Liquid UI");
            liquidParent.SetActive(true);
        }
        else if (skill == playerStats.foodSkill && foodParent != null)
        {
            Debug.Log("Activating Food UI");
            foodParent.SetActive(true);
        }
        else if (skill == playerStats.beerSkill && burpParent != null)
        {
            Debug.Log("Activating Burp UI");
            burpParent.SetActive(true);
        }
        else if (skill == playerStats.alcoholSkill && alcoholParent != null)
        {
            Debug.Log("Activating Alcohol UI");
            alcoholParent.SetActive(true);
        }

        // Mostra la notifica di sblocco
        string skillName = GetSkillName(skill);
        Debug.Log($"Showing unlock notification for {skillName} at level {playerStats.GetLevel()}");
        unlockNotification.ShowUnlock(playerStats.GetLevel(), skillName);

        // Aggiorna la visibilità di tutte le barre
        UpdateResourceBarsVisibility();
    }
    private string GetSkillName(SkillLevel skill)
    {
        if (skill == playerStats.spitSkill) return "Spit Attack";
        if (skill == playerStats.gasSkill) return "Fart Attack";
        if (skill == playerStats.waterSkill) return "Urine Attack";
        if (skill == playerStats.stinkSkill) return "Stink Attack";
        if (skill == playerStats.foodSkill) return "Shit Bomb Attack";
        if (skill == playerStats.alcoholSkill) return "Vomit Attack";
        if (skill == playerStats.diarrheaSkill) return "Diarrhea Attack";
        if (skill == playerStats.beerSkill) return "Burp Attack";
        return "Unknown Skill";
    }

    private void SetupUIComponents()
    {
        // Setup barra esperienza
        SetupBar(experienceBar, Image.FillMethod.Vertical, (int)Image.OriginVertical.Bottom);

        // Setup barra salute
        SetupBar(healthBar, Image.FillMethod.Horizontal, (int)Image.OriginHorizontal.Left, healthBarColor);
        SetupBar(healthStall, Image.FillMethod.Horizontal, (int)Image.OriginHorizontal.Left, healthStallColor);

        // Setup barre risorse
        SetupBar(fartBar, Image.FillMethod.Horizontal, (int)Image.OriginHorizontal.Left, fartBarColor);
        SetupBar(liquidBar, Image.FillMethod.Horizontal, (int)Image.OriginHorizontal.Left, liquidBarColor);
        SetupBar(foodBar, Image.FillMethod.Horizontal, (int)Image.OriginHorizontal.Left, foodBarColor);
        SetupBar(burpBar, Image.FillMethod.Horizontal, (int)Image.OriginHorizontal.Left, burpBarColor);
        SetupBar(alcoholBar, Image.FillMethod.Horizontal, (int)Image.OriginHorizontal.Left, alcoholBarColor);

        UpdateExperienceUI();
    }

    private void InitializeResourceBarsVisibility()
    {
        if (resourceManager == null) return;

        // Inizialmente nascondi tutti i parent delle barre non sbloccate
        if (fartParent != null)
            fartParent.SetActive(resourceManager.IsFartUnlocked());

        if (liquidParent != null)
            liquidParent.SetActive(resourceManager.IsUrineUnlocked());

        if (foodParent != null)
            foodParent.SetActive(resourceManager.IsFoodUnlocked());

        if (burpParent != null)
            burpParent.SetActive(resourceManager.IsBurpUnlocked());

        if (alcoholParent != null)
            alcoholParent.SetActive(resourceManager.IsAlcoholUnlocked());
    }

    private void UpdateResourceBarsVisibility()
    {
        InitializeResourceBarsVisibility(); // Riutilizza lo stesso metodo
    }

    private void SetupBar(Image bar, Image.FillMethod fillMethod, int fillOrigin, Color? color = null)
    {
        if (bar != null)
        {
            bar.type = Image.Type.Filled;
            bar.fillMethod = fillMethod;
            bar.fillOrigin = fillOrigin;
            if (color.HasValue)
            {
                bar.color = color.Value;
            }
            bar.fillAmount = 1f;
        }
    }

    private void Update()
    {
        UpdateExperienceUI();
        UpdateHealthStall();
        UpdateResourceBars();
    }

    private void UpdateResourceBars()
    {
        if (resourceManager == null) return;

        // Aggiorna solo le barre che sono attualmente visibili
        if (fartBar != null && fartParent != null && fartParent.activeSelf)
        {
            fartBar.fillAmount = resourceManager.GetFartPercentage();
        }

        if (liquidBar != null && liquidParent != null && liquidParent.activeSelf)
        {
            liquidBar.fillAmount = resourceManager.GetUrinePercentage();
        }

        if (foodBar != null && foodParent != null && foodParent.activeSelf)
        {
            foodBar.fillAmount = resourceManager.GetFoodPercentage();
        }

        if (burpBar != null && burpParent != null && burpParent.activeSelf)
        {
            burpBar.fillAmount = resourceManager.GetBurpPercentage();
        }

        if (alcoholBar != null && alcoholParent != null && alcoholParent.activeSelf)
        {
            alcoholBar.fillAmount = resourceManager.GetAlcoholPercentage();
        }
    }

    private IEnumerator InitializeHealthBarsNextFrame()
    {
        yield return null;

        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += UpdateHealthUI;

            float initialHealth = playerHealth.GetCurrentHealth();
            float maxHealth = playerHealth.GetMaxHealth();
            float initialHealthPercentage = 1f;

            if (maxHealth > 0)
            {
                initialHealthPercentage = initialHealth / maxHealth;
            }

            if (initialHealthPercentage <= 0)
            {
                initialHealthPercentage = 1f;
            }

            if (healthBar != null)
            {
                healthBar.fillAmount = initialHealthPercentage;
                healthBar.color = healthBarColor;
            }

            if (healthStall != null)
            {
                healthStall.fillAmount = initialHealthPercentage;
                healthStall.color = healthStallColor;
            }

            lastHealthPercentage = initialHealthPercentage;
            currentStallAmount = initialHealthPercentage;
            targetStallAmount = initialHealthPercentage;
        }
    }

    private void UpdateExperienceUI()
    {
        if (playerStats == null) return;

        if (experienceBar != null)
        {
            experienceBar.fillAmount = playerStats.GetXPProgress();
        }

        if (levelText != null)
        {
            levelText.text = $"{playerStats.GetLevel()}";
        }
    }

    private void UpdateHealthUI(float currentHealth, float maxHealth)
    {
        if (healthBar == null) return;

        float healthPercentage = maxHealth > 0 ? currentHealth / maxHealth : 0;

        if (healthPercentage < lastHealthPercentage)
        {
            stallDelayTimer = stallDelay;
            targetStallAmount = healthPercentage;
        }
        else if (healthPercentage > lastHealthPercentage)
        {
            currentStallAmount = healthPercentage;
            targetStallAmount = healthPercentage;
            stallDelayTimer = 0;
        }

        lastHealthPercentage = healthPercentage;
        healthBar.fillAmount = healthPercentage;
    }

    private void UpdateHealthStall()
    {
        if (healthStall == null) return;

        if (stallDelayTimer > 0)
        {
            stallDelayTimer -= Time.deltaTime;
            return;
        }

        if (currentStallAmount != targetStallAmount)
        {
            currentStallAmount = Mathf.Lerp(currentStallAmount, targetStallAmount, stallSpeed * Time.deltaTime);

            if (Mathf.Abs(currentStallAmount - targetStallAmount) < 0.001f)
            {
                currentStallAmount = targetStallAmount;
            }

            healthStall.fillAmount = currentStallAmount;
        }
    }

    private void OnValidate()
    {
        // Aggiorna immediatamente i colori se cambiano nell'inspector
        if (healthBar != null) healthBar.color = healthBarColor;
        if (healthStall != null) healthStall.color = healthStallColor;
        if (fartBar != null) fartBar.color = fartBarColor;
        if (liquidBar != null) liquidBar.color = liquidBarColor;
        if (foodBar != null) foodBar.color = foodBarColor;
        if (burpBar != null) burpBar.color = burpBarColor;
        if (alcoholBar != null) alcoholBar.color = alcoholBarColor;
    }
}