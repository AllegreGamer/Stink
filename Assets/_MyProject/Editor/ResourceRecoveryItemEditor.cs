using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ResourceRecoveryItem))]
public class ResourceRecoveryItemEditor : Editor
{
    private bool showRecoveryAmounts = true;
    private bool showSpawnSettings = true;

    public override void OnInspectorGUI()
    {
        ResourceRecoveryItem item = (ResourceRecoveryItem)target;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Resource Recovery Item Settings", EditorStyles.boldLabel);

        serializedObject.Update();

        // Item Type and Unlock Level
        EditorGUILayout.PropertyField(serializedObject.FindProperty("itemType"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("unlockLevel"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("itemPrefab"));

        // Recovery Amounts Section
        EditorGUILayout.Space(10);
        showRecoveryAmounts = EditorGUILayout.Foldout(showRecoveryAmounts, "Recovery Amounts", true);
        if (showRecoveryAmounts)
        {
            EditorGUI.indentLevel++;
            SerializedProperty recoveryAmounts = serializedObject.FindProperty("recoveryAmounts");

            EditorGUILayout.PropertyField(recoveryAmounts.FindPropertyRelative("healthRecovery"));
            EditorGUILayout.PropertyField(recoveryAmounts.FindPropertyRelative("urineRecovery"));
            EditorGUILayout.PropertyField(recoveryAmounts.FindPropertyRelative("foodRecovery"));
            EditorGUILayout.PropertyField(recoveryAmounts.FindPropertyRelative("alcoholRecovery"));
            EditorGUILayout.PropertyField(recoveryAmounts.FindPropertyRelative("burpRecovery"));

            EditorGUI.indentLevel--;
        }

        // Spawn Settings Section
        EditorGUILayout.Space(10);
        showSpawnSettings = EditorGUILayout.Foldout(showSpawnSettings, "Spawn Settings", true);
        if (showSpawnSettings)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("spawnProbability"));
            EditorGUILayout.Slider(serializedObject.FindProperty("spawnProbability"), 0f, 1f, new GUIContent("Spawn Probability"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("spawnFrequency"));
            EditorGUILayout.HelpBox("Spawn Frequency is in seconds", MessageType.Info);

            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
    }
}

[CustomEditor(typeof(ResourceItemSpawner))]
public class ResourceItemSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ResourceItemSpawner spawner = (ResourceItemSpawner)target;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Resource Item Spawner Settings", EditorStyles.boldLabel);

        serializedObject.Update();

        // Spawn Settings
        EditorGUILayout.PropertyField(serializedObject.FindProperty("spawnRadius"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("minDistanceFromPlayer"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("player"));

        // Items List
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Spawnable Items", EditorStyles.boldLabel);

        SerializedProperty itemsData = serializedObject.FindProperty("itemsData");
        EditorGUI.indentLevel++;

        for (int i = 0; i < itemsData.arraySize; i++)
        {
            SerializedProperty itemData = itemsData.GetArrayElementAtIndex(i);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.PropertyField(itemData.FindPropertyRelative("itemPrefab"));

            if (Application.isPlaying)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.Toggle("Is Unlocked", itemData.FindPropertyRelative("isUnlocked").boolValue);
                EditorGUILayout.FloatField("Next Spawn Time", itemData.FindPropertyRelative("nextSpawnTime").floatValue);
                EditorGUI.EndDisabledGroup();
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }

        EditorGUI.indentLevel--;

        // Add/Remove buttons
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Item"))
        {
            itemsData.arraySize++;
        }
        if (GUILayout.Button("Remove Last"))
        {
            if (itemsData.arraySize > 0)
                itemsData.arraySize--;
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();

        if (Application.isPlaying)
        {
            Repaint();
        }
    }
}