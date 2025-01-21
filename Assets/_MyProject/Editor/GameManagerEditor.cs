using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    private bool showEvents = true;

    public override void OnInspectorGUI()
    {
        GameManager gameManager = (GameManager)target;
        serializedObject.Update();  // Solo una volta all'inizio

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Game Time Settings", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("initialTime"));

        if (Application.isPlaying)
        {
            EditorGUILayout.LabelField($"Current Game Time: {gameManager.GetGameTime():F2} seconds");
            EditorGUILayout.LabelField($"Game Status: {(gameManager.IsGameStarted() ? "Running" : "Paused")}");
        }

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("References", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("timerText"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("enemySpawner"));

        EditorGUILayout.Space(10);
        showEvents = EditorGUILayout.Foldout(showEvents, "Game Events", true);

        if (showEvents)
        {
            SerializedProperty eventsList = serializedObject.FindProperty("gameEvents");
            EditorGUI.indentLevel++;

            if (GUILayout.Button("Add New Event", GUILayout.Height(30)))
            {
                eventsList.arraySize++;
                var newEvent = eventsList.GetArrayElementAtIndex(eventsList.arraySize - 1);
                newEvent.FindPropertyRelative("eventName").stringValue = "New Event";
                newEvent.FindPropertyRelative("levelNumber").intValue = 1;
                newEvent.FindPropertyRelative("timeToTrigger").floatValue = 0f;
            }

            for (int i = 0; i < eventsList.arraySize; i++)
            {
                SerializedProperty eventProp = eventsList.GetArrayElementAtIndex(i);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                // Header con nome evento e pulsante rimuovi
                EditorGUILayout.BeginHorizontal();
                string eventName = eventProp.FindPropertyRelative("eventName").stringValue;
                EditorGUILayout.LabelField($"Event {i + 1}: {eventName}", EditorStyles.boldLabel);
                if (GUILayout.Button("Remove", GUILayout.Width(60)))
                {
                    eventsList.DeleteArrayElementAtIndex(i);
                    break;
                }
                EditorGUILayout.EndHorizontal();

                // Proprietà base dell'evento
                EditorGUILayout.PropertyField(eventProp.FindPropertyRelative("eventName"));
                EditorGUILayout.PropertyField(eventProp.FindPropertyRelative("levelNumber"));
                EditorGUILayout.PropertyField(eventProp.FindPropertyRelative("timeToTrigger"));
                EditorGUILayout.PropertyField(eventProp.FindPropertyRelative("eventType"));

                // Proprietà specifiche per tipo di evento
                var eventType = (GameEventType)eventProp.FindPropertyRelative("eventType").enumValueIndex;
                switch (eventType)
                {
                    case GameEventType.IncreaseEnemyCount:
                        EditorGUILayout.PropertyField(eventProp.FindPropertyRelative("enemyCountIncrease"));
                        break;

                    case GameEventType.SpawnNewEnemyType:
                    case GameEventType.BossEvent:
                        EditorGUILayout.PropertyField(eventProp.FindPropertyRelative("enemyPrefabToSpawn"));
                        EditorGUILayout.PropertyField(eventProp.FindPropertyRelative("enemyHealthMultiplier"));
                        EditorGUILayout.PropertyField(eventProp.FindPropertyRelative("enemyDamageMultiplier"));
                        EditorGUILayout.PropertyField(eventProp.FindPropertyRelative("enemySpeedMultiplier"));
                        break;
                }

                if (Application.isPlaying)
                {
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.Toggle("Has Triggered", eventProp.FindPropertyRelative("hasTriggered").boolValue);
                    EditorGUI.EndDisabledGroup();
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(5);
            }

            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();

        if (Application.isPlaying)
        {
            Repaint();
        }
    }
}