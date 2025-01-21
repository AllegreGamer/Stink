using UnityEngine;

public class XPGemDropper : MonoBehaviour
{
    [System.Serializable]
    public class GemData
    {
        public GameObject gemPrefab;
        public float xpValue = 10f;
    }

    [Header("Gem Prefabs")]
    [SerializeField] private GemData smallGem;  // Gemma blu
    [SerializeField] private GemData mediumGem; // Gemma verde
    [SerializeField] private GemData largeGem;  // Gemma rossa

    public void DropGem(XPGem.GemType type)
    {
        GemData gemData = GetGemData(type);
        if (gemData?.gemPrefab != null)
        {
            GameObject gem = Instantiate(gemData.gemPrefab, transform.position, Quaternion.identity);
            XPGem xpGem = gem.GetComponent<XPGem>();
            if (xpGem != null)
            {
                xpGem.SetXPValue(gemData.xpValue);
            }
        }
    }

    private GemData GetGemData(XPGem.GemType type)
    {
        switch (type)
        {
            case XPGem.GemType.Small: return smallGem;
            case XPGem.GemType.Medium: return mediumGem;
            case XPGem.GemType.Large: return largeGem;
            default: return null;
        }
    }

    // Metodo di test per dropppare gemme random
    public void DropRandomGem()
    {
        XPGem.GemType randomType = (XPGem.GemType)Random.Range(0, 3);
        DropGem(randomType);
    }
}