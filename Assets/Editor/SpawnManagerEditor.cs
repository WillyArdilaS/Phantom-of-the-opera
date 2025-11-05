using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpawnManager))]
public class SpawnManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SpawnManager manager = (SpawnManager)target;
        serializedObject.Update();

        DrawDefaultInspector();
        EditorGUILayout.Space(10);

        if (manager.ChannelGroups.Count == 0)
        {
            if (GUILayout.Button("Generate Ghost Spawn Data"))
            {
                if (manager.MIDIPrefab == null)
                {
                    Debug.LogError("[SpawnManager] No se ha asignado un MIDIDataPrefab. Asigna uno antes de generar");
                    return;
                }
                else
                {
                    manager.InitializeDictionaries();
                    GenerateFromMIDI(manager);
                }
            }
        }
        else
        {
            if (GUILayout.Button("Reset Ghost Spawn Data"))
            {
                ClearGhostSpawnData(manager);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    // === Ghost spawn data methods ===
    private void GenerateFromMIDI(SpawnManager manager)
    {
        SerializedProperty channelGroupsProperty = serializedObject.FindProperty("channelGroups");
        channelGroupsProperty.ClearArray();

        foreach (var channel in manager.MIDIPrefab.Channels)
        {
            // Add the MIDIData channel to the list
            int groupIndex = channelGroupsProperty.arraySize;
            channelGroupsProperty.InsertArrayElementAtIndex(groupIndex);

            SerializedProperty channelGroup = channelGroupsProperty.GetArrayElementAtIndex(groupIndex);
            SerializedProperty channelNameProperty = channelGroup.FindPropertyRelative("channelName");
            SerializedProperty ghostDataListProperty = channelGroup.FindPropertyRelative("ghostSpawnDataList");

            channelNameProperty.stringValue = channel.ChannelName;
            ghostDataListProperty.ClearArray();

            // Scan each group of notes in the channel.
            foreach (var noteGroup in channel.NoteGroups)
            {
                if (!noteGroup.Notes.Any(note => note.IsActive)) continue;

                // Initialize ghostSpawnData and add it to the list
                int ghostDataIndex = ghostDataListProperty.arraySize;
                ghostDataListProperty.InsertArrayElementAtIndex(ghostDataIndex);
                SerializedProperty ghostSpawnData = ghostDataListProperty.GetArrayElementAtIndex(ghostDataIndex);
                SetPatternTimes(ghostSpawnData, noteGroup);

                // Assign note pattern based on the number of active notes  
                PatternData ghostPattern = GetPatternForNoteGroup(manager, channel, noteGroup);
                ghostSpawnData.FindPropertyRelative("patternData").objectReferenceValue = ghostPattern;

                // Assign speed and spawn time 
                float ghostSpeed = CalculateSpeed(ghostSpawnData, ghostPattern);
                ghostSpawnData.FindPropertyRelative("speedMovement").floatValue = ghostSpeed;

                float ghostSpawnTime = CalculateSpawnTime(manager, ghostSpawnData, ghostSpeed);
                ghostSpawnData.FindPropertyRelative("spawnTime").floatValue = ghostSpawnTime;

                // Show the notes that are part of GhostSpawnData 
                SerializedProperty activeNotesProperty = ghostSpawnData.FindPropertyRelative("activeNotes");

                if (activeNotesProperty != null)
                {
                    activeNotesProperty.ClearArray();

                    foreach (var note in noteGroup.Notes.Where(note => note.IsActive))
                    {
                        int noteIndex = activeNotesProperty.arraySize;
                        activeNotesProperty.InsertArrayElementAtIndex(noteIndex);
                        SerializedProperty currentNote = activeNotesProperty.GetArrayElementAtIndex(noteIndex);

                        currentNote.FindPropertyRelative("name").stringValue = note.Name;
                        currentNote.FindPropertyRelative("channel").intValue = note.Channel;
                        currentNote.FindPropertyRelative("startTime").doubleValue = note.StartTime;
                        currentNote.FindPropertyRelative("duration").doubleValue = note.Duration;
                        currentNote.FindPropertyRelative("isActive").boolValue = note.IsActive;
                    }
                }
            }
        }
    }

    private void ClearGhostSpawnData(SpawnManager manager)
    {
        SerializedProperty channelGroupsProperty = serializedObject.FindProperty("channelGroups");
        channelGroupsProperty.ClearArray();
    }

    // === Initialization methods ===
    private void SetPatternTimes(SerializedProperty ghost, NoteGroup noteGroup)
    {
        var activeNotes = noteGroup.Notes.Where(note => note.IsActive).ToList();

        if (activeNotes.Count > 0)
        {
            float newStartTime = activeNotes.Min(note => (float)note.StartTime);
            float newEndTime = activeNotes.Max(note => (float)(note.StartTime + note.Duration));

            ghost.FindPropertyRelative("patternStartTime").floatValue = newStartTime;
            ghost.FindPropertyRelative("patternEndTime").floatValue = newEndTime;
        }
    }

    private PatternData GetPatternForNoteGroup(SpawnManager manager, MIDIChannelData channel, NoteGroup noteGroup)
    {
        PatternData pattern = null;
        int activeCount = noteGroup.Notes.Count(note => note.IsActive);

        // Search for a pattern that matches the number of active notes
        if (!manager.PatternDataDictionary.TryGetValue(activeCount, out var foundPattern) || foundPattern == null)
        {
            pattern = manager.PatternDataDictionary.ContainsKey(1) ? manager.PatternDataDictionary[1] : null;

            Debug.LogWarning($"[GhostSpawnData] No se encontró un PatternData válido para el grupo #{noteGroup.GroupIndex} (Canal: {channel.ChannelName}, " +
            $"Notas activas: {activeCount}). Se asignará el patrón de 1 nota por defecto");
        }
        else
        {
            pattern = foundPattern;
        }

        return pattern;
    }

    // * MUST CHECK THIS FORMULA BECAUSE GHOSTS ARE VERY FAST, BUT I DON'T KNOW IF IT'S BECAUSE OF THE FORMULA OR THE GROUPING OF NOTES *
    private float CalculateSpeed(SerializedProperty ghost, PatternData patternData)
    {
        float patternStartTime = ghost.FindPropertyRelative("patternStartTime").floatValue;
        float patternEndTime = ghost.FindPropertyRelative("patternEndTime").floatValue;

        if (patternData == null)
        {
            Debug.LogError("[GhostSpawnData] patternData es null al intentar calcular velocidad");
            return 0.5f;
        }

        /* 
            Formula for the speed of the pattern within the interaction zone:
            patternDuration = patternEndTime - patternStartTime
            patternWidth = (notesQuantity − 1) ∗ horizontalSpacing
            speedMovement = patternWidth / totalTime
        */

        float patternDuration = Mathf.Max(patternEndTime - patternStartTime, 0.1f);
        float patternWidth = Mathf.Max((patternData.NotesQuantity - 1) * patternData.HorizontalSpacing, patternData.HorizontalSpacing / 2);
        float speedMovement = patternWidth / patternDuration;

        return speedMovement;
    }

    private float CalculateSpawnTime(SpawnManager manager, SerializedProperty ghost, float speedMovement)
    {
        /* 
            Formula for the spawn time:
            travelTime = distanceToCenter / speedMovement
            spawnTime = patternStartTime - travelTime
        */

        GhostMovement.GhostDirection ghostDirection = (GhostMovement.GhostDirection)ghost.FindPropertyRelative("ghostDirection").enumValueIndex;
        GameObject interactionZone = manager.InteractionZonesDictionary[ghostDirection];
        GameObject spawner = manager.SpawnersDictionary[ghostDirection];

        if (interactionZone == null || spawner == null)
        {
            Debug.LogError($"[SpawnManagerEditor] Faltan referencias de zona o spawner para {ghostDirection}");
            return 0f;
        }

        // Midpoint of the interaction zone
        if (!interactionZone.TryGetComponent(out Collider2D interactionCollider))
        {
            Debug.LogError($"[SpawnManagerEditor] La InteractionZone {interactionZone.name} no tiene Collider2D");
            return 0f;
        }

        float interactionCenterX = interactionCollider.bounds.center.x;

        // Distance and timing
        float distanceToCenter = Mathf.Abs(spawner.transform.position.x - interactionCenterX);

        // Calculate when to spawn
        float patternStartTime = ghost.FindPropertyRelative("patternStartTime").floatValue;

        float travelTime = distanceToCenter / speedMovement;
        float spawnTime = patternStartTime - travelTime;
        spawnTime = Mathf.Round(spawnTime * 1000f) / 1000f; // Rounded to three decimal places for greater precision in ms

        return spawnTime;
    }
}