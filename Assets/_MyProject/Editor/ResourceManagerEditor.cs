using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ResourceManager))]
public class ResourceManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ResourceManager resourceManager = (ResourceManager)target;
        // Disegna le proprietà predefinite
        DrawDefaultInspector();
        // Aggiungi spazio
        EditorGUILayout.Space(10);
        // Disegna le barre delle risorse
        EditorGUILayout.LabelField("Current Resources", EditorStyles.boldLabel);

        // Barra Urina
        EditorGUILayout.LabelField($"Urine: {resourceManager.GetCurrentUrine():F1}/{resourceManager.GetMaxUrine():F1}");
        EditorGUI.ProgressBar(
            EditorGUILayout.GetControlRect(false, 20),
            resourceManager.GetUrinePercentage(),
            "Urine Level"
        );

        // Barra Stamina
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField($"Fart: {resourceManager.GetCurrentFart():F1}/{resourceManager.GetMaxFart():F1}");
        EditorGUI.ProgressBar(
            EditorGUILayout.GetControlRect(false, 20),
            resourceManager.GetFartPercentage(),
            "Fart Level"
        );

        // Barra Food
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField($"Food: {resourceManager.GetCurrentFood():F1}/{resourceManager.GetMaxFood():F1}");
        EditorGUI.ProgressBar(
            EditorGUILayout.GetControlRect(false, 20),
            resourceManager.GetFoodPercentage(),
            "Food Level"
        );

        // Barra Alcohol
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField($"Alcohol: {resourceManager.GetCurrentAlcohol():F1}/{resourceManager.GetMaxAlcohol():F1}");
        EditorGUI.ProgressBar(
            EditorGUILayout.GetControlRect(false, 20),
            resourceManager.GetAlcoholPercentage(),
            "Alcohol Level"
        );

        // Barra Burp
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField($"Burp: {resourceManager.GetCurrentBurp():F1}/{resourceManager.GetMaxBurp():F1}");
        EditorGUI.ProgressBar(
            EditorGUILayout.GetControlRect(false, 20),
            resourceManager.GetBurpPercentage(),
            "Burp Level"
        );

        // Aggiorna l'inspector in play mode
        if (EditorApplication.isPlaying)
        {
            Repaint();
        }
    }
}