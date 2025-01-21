using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerHealth))]
public class PlayerHealthEditor : Editor
{
    private bool showHealthSettings = true;
    private bool showDefenseSettings = true;
    private bool showRegenSettings = true;
    private bool showInvulnerabilitySettings = true;
    private bool showStatusEffects = true;

    public override void OnInspectorGUI()
    {
        PlayerHealth playerHealth = (PlayerHealth)target;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Player Health System", EditorStyles.boldLabel);

        serializedObject.Update();

        // Health Bar
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField($"Health: {playerHealth.GetCurrentHealth():F1}/{playerHealth.GetMaxHealth():F1}");
        Rect r = EditorGUILayout.GetControlRect(false, 20);
        EditorGUI.ProgressBar(r, playerHealth.GetHealthPercentage(), "Current Health");

        // Health Settings Section
        EditorGUILayout.Space(10);
        showHealthSettings = EditorGUILayout.Foldout(showHealthSettings, "Health Settings", true);
        if (showHealthSettings)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxHealth"));
            EditorGUI.indentLevel--;
        }

        // Defense Settings Section
        EditorGUILayout.Space(5);
        showDefenseSettings = EditorGUILayout.Foldout(showDefenseSettings, "Defense Settings", true);
        if (showDefenseSettings)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("baseArmor"));
            EditorGUILayout.LabelField($"Current Armor: {playerHealth.GetCurrentArmor():F1}");
            EditorGUI.indentLevel--;
        }

        // Regeneration Settings
        EditorGUILayout.Space(5);
        showRegenSettings = EditorGUILayout.Foldout(showRegenSettings, "Regeneration Settings", true);
        if (showRegenSettings)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("regenRate"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("regenTickInterval"));
            EditorGUI.indentLevel--;
        }

        // Invulnerability Settings
        EditorGUILayout.Space(5);
        showInvulnerabilitySettings = EditorGUILayout.Foldout(showInvulnerabilitySettings, "Invulnerability Settings", true);
        if (showInvulnerabilitySettings)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("invulnerabilityDuration"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("enableInvulnerabilityOnDamage"));
            EditorGUI.indentLevel--;
        }

        // Status Effects Section
        EditorGUILayout.Space(5);
        showStatusEffects = EditorGUILayout.Foldout(showStatusEffects, "Active Status Effects", true);
        if (showStatusEffects && Application.isPlaying)
        {
            EditorGUI.indentLevel++;
            // Mostra gli effetti di stato attivi
            var activeEffects = playerHealth.GetActiveEffects();
            if (activeEffects != null && activeEffects.Count > 0)
            {
                foreach (var effect in activeEffects)
                {
                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                    EditorGUILayout.LabelField($"{effect.Key}:", GUILayout.Width(100));
                    EditorGUILayout.LabelField($"Duration: {effect.Value.duration:F1}s", GUILayout.Width(120));
                    EditorGUILayout.LabelField($"Power: {effect.Value.power:F1}");
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                EditorGUILayout.LabelField("No active effects");
            }
            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();

        // Aggiorna l'inspector in play mode
        if (Application.isPlaying)
        {
            Repaint();
        }
    }
}