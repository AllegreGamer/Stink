using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DiarrheaAreaManager : MonoBehaviour
{
    [Header("Area Detection")]
    [SerializeField] private float connectionDistance = 1f;     // Distanza massima per considerare due macchie connesse
    [SerializeField] private float checkInterval = 0.5f;        // Quanto spesso controlliamo per forme chiuse
    [SerializeField] private float areaDamageMultiplier = 2f;   // Moltiplicatore danno per l'area chiusa
    [SerializeField] private GameObject diarrheaAreaEffectPrefab; // Prefab per l'effetto visivo dell'area chiusa
    [SerializeField] private float minRequiredArea = 10f;  // Area minima richiesta, modificabile in inspector

    private List<DiarrheaSplash> activeSpots = new List<DiarrheaSplash>();  // Mantenuto in ordine di creazione
    private float nextCheckTime;

    // Singleton pattern
    private static DiarrheaAreaManager instance;
    public static DiarrheaAreaManager Instance => instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void RegisterSpot(DiarrheaSplash splash)
    {
        if (!activeSpots.Contains(splash))
        {
            // Se ci sono già macchie, controlla la connessione con l'ultima
            if (activeSpots.Count > 0)
            {
                DiarrheaSplash lastSpot = activeSpots[activeSpots.Count - 1];
                if (AreConnected(lastSpot, splash))
                {
                    // Continua la catena esistente
                    activeSpots.Add(splash);
                    Debug.Log($"Macchia connessa alla precedente. Total spots: {activeSpots.Count}");
                }
                else
                {
                    // Se è troppo lontana, inizia una nuova catena
                    Debug.Log("Inizia nuova catena di macchie");
                    activeSpots.Clear(); // Pulisce la vecchia catena
                    activeSpots.Add(splash); // Inizia una nuova catena
                }
            }
            else
            {
                // Prima macchia della sequenza
                activeSpots.Add(splash);
            }
        }
    }

    public void UnregisterSpot(DiarrheaSplash splash)
    {
        activeSpots.Remove(splash);
        Debug.Log($"Unregistered splash. Remaining spots: {activeSpots.Count}");
    }

    private void Update()
    {
        if (Time.time >= nextCheckTime)
        {
            CheckForClosedAreas();
            nextCheckTime = Time.time + checkInterval;
        }
    }

    private void CheckForClosedAreas()
    {
        // Rimuovi eventuali macchie nulle dalla lista
        activeSpots.RemoveAll(spot => spot == null);

        // Servono almeno 2 macchie per formare una connessione
        if (activeSpots.Count < 2) return;

        DiarrheaSplash lastSpot = activeSpots[activeSpots.Count - 1];

        for (int i = 0; i < activeSpots.Count - 1; i++)
        {
            if (AreConnected(lastSpot, activeSpots[i]))
            {
                var spotsInArea = activeSpots.GetRange(i, activeSpots.Count - i);
                var spotsSet = new HashSet<DiarrheaSplash>(spotsInArea);

                float area = CalculateFormArea(spotsSet);
                Debug.Log($"Forma chiusa trovata con area: {area}");

                if (area >= minRequiredArea)
                {
                    CreateClosedArea(spotsSet);
                    break;
                }
                else
                {
                    Debug.Log($"Area troppo piccola ({area}) - Minimo richiesto: {minRequiredArea}");
                }
            }
        }
    }

    private float CalculateFormArea(HashSet<DiarrheaSplash> spots)
    {
        // Trova i punti più distanti
        float maxDistance = 0f;
        foreach (var spotA in spots)
        {
            foreach (var spotB in spots)
            {
                float distance = Vector3.Distance(spotA.GetPosition(), spotB.GetPosition());
                maxDistance = Mathf.Max(maxDistance, distance);
            }
        }

        // Calcola l'area approssimativa
        float area = Mathf.PI * maxDistance * maxDistance / 4f;

        return area;
    }
    private bool AreConnected(DiarrheaSplash a, DiarrheaSplash b)
    {
        float distance = Vector3.Distance(a.GetPosition(), b.GetPosition());
        float combinedRadius = a.GetRadius() + b.GetRadius();

        Debug.Log($"Checking connection: {a.GetPosition()} to {b.GetPosition()} - Distance: {distance}, Combined Radius: {combinedRadius}");

        return distance <= combinedRadius + connectionDistance;
    }

    private bool FindCycle(DiarrheaSplash current, DiarrheaSplash parent,
        Dictionary<DiarrheaSplash, List<DiarrheaSplash>> connections,
        HashSet<DiarrheaSplash> visited,
        HashSet<DiarrheaSplash> currentPath)
    {
        visited.Add(current);
        currentPath.Add(current);

        foreach (var neighbor in connections[current])
        {
            if (neighbor == parent) continue;

            if (currentPath.Contains(neighbor))
            {
                if (currentPath.Count >= 20)
                {
                    return true;
                }
                else
                {
                    continue;
                }
            }

            if (!visited.Contains(neighbor))
            {
                if (FindCycle(neighbor, current, connections, visited, currentPath))
                {
                    return true;
                }
            }
        }

        currentPath.Remove(current);
        return false;
    }

    private void CreateClosedArea(HashSet<DiarrheaSplash> spotsInArea)
    {
        Vector3 center = CalculateCentroid(spotsInArea);

        float maxRadius = 0f;
        foreach (var spot in spotsInArea)
        {
            float distance = Vector3.Distance(center, spot.GetPosition());
            maxRadius = Mathf.Max(maxRadius, distance + spot.GetRadius());
        }

        if (diarrheaAreaEffectPrefab != null)
        {
            GameObject effectObject = Instantiate(diarrheaAreaEffectPrefab, center, Quaternion.identity);
            DiarrheaAreaEffect effectComponent = effectObject.GetComponent<DiarrheaAreaEffect>();
            if (effectComponent != null)
            {
                float totalDamage = spotsInArea.Sum(spot => spot.GetDamage());
                totalDamage *= areaDamageMultiplier;
                effectComponent.Initialize(totalDamage, maxRadius);
            }
        }

        foreach (var spot in spotsInArea)
        {
            if (spot != null)
            {
                Destroy(spot.gameObject);
            }
        }
    }

    private Vector3 CalculateCentroid(HashSet<DiarrheaSplash> spotsInArea)
    {
        Vector3 weightedCenter = Vector3.zero;
        float totalWeight = 0f;

        foreach (var spot in spotsInArea)
        {
            float weight = Mathf.PI * Mathf.Pow(spot.GetRadius(), 2);
            weightedCenter += spot.GetPosition() * weight;
            totalWeight += weight;
        }

        return totalWeight > 0 ? weightedCenter / totalWeight : Vector3.zero;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (activeSpots != null)
        {
            Gizmos.color = Color.yellow;
            // Disegna solo le linee tra macchie consecutive
            for (int i = 0; i < activeSpots.Count - 1; i++)
            {
                DiarrheaSplash spotA = activeSpots[i];
                DiarrheaSplash spotB = activeSpots[i + 1];
                if (spotA != null && spotB != null)
                {
                    Gizmos.DrawLine(spotA.GetPosition(), spotB.GetPosition());
                }
            }
        }
    }
#endif
}
