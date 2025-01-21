using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerStats))]
public class PlayerStatsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Riferimento al componente PlayerStats
        PlayerStats playerStats = (PlayerStats)target;

        // Disegna le proprietà predefinite
        DrawDefaultInspector();

        // Aggiungi spazio
        EditorGUILayout.Space(10);

        // BASIC STATS SECTION
        EditorGUILayout.LabelField("Basic Stats", EditorStyles.boldLabel);

        // Level and XP
        EditorGUILayout.LabelField($"Level: {playerStats.GetLevel()}");
        EditorGUILayout.LabelField($"XP: {playerStats.GetCurrentXP():F1}/{playerStats.GetXPToNextLevel():F1}");

        // Progress bar per XP
        EditorGUI.ProgressBar(
            EditorGUILayout.GetControlRect(false, 20),
            playerStats.GetXPProgress(),
            "XP Progress"
        );

        // HP
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField($"HP: {playerStats.GetCurrentHealth():F1}/{playerStats.GetMaxHealth():F1}");
        EditorGUI.ProgressBar(
            EditorGUILayout.GetControlRect(false, 20),
            playerStats.GetHealthPercentage(),
            "Health"
        );

        // COMBAT SKILLS SECTION
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Combat Skills", EditorStyles.boldLabel);

        DrawSkill("Spit", playerStats.spitSkill);
        DrawSkill("Gas", playerStats.gasSkill);
        DrawSkill("Water", playerStats.waterSkill);
        DrawSkill("Stink", playerStats.stinkSkill);
        DrawSkill("Food", playerStats.foodSkill);
        DrawSkill("Alcohol", playerStats.alcoholSkill);
        DrawSkill("Diarrhea", playerStats.diarrheaSkill);
        DrawSkill("Beer", playerStats.beerSkill);

        // Aggiorna l'inspector in play mode
        if (EditorApplication.isPlaying)
        {
            Repaint();
        }
    }

    private void DrawSkill(string skillName, SkillLevel skill)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        // Header con nome skill e stato
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(skillName, EditorStyles.boldLabel);
        GUI.color = skill.isUnlocked ? Color.green : Color.red;
        EditorGUILayout.LabelField(skill.isUnlocked ? "Unlocked" : "Locked", GUILayout.Width(60));
        GUI.color = Color.white;
        EditorGUILayout.EndHorizontal();

        if (skill.isUnlocked)
        {
            // Livello e potenziamenti
            EditorGUILayout.LabelField($"Level: {skill.level}/{skill.maxLevel}");

            // Mostra potenziamenti attivi
            if (skill.activeEnhancements.Count > 0)
            {
                EditorGUILayout.LabelField("Active Enhancements:", EditorStyles.boldLabel);
                foreach (string enhancement in skill.activeEnhancements)
                {
                    EditorGUILayout.LabelField($"- {enhancement}");
                }
            }
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(5);
    }
}