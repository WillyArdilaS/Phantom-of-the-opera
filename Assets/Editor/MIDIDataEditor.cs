using UnityEditor;
using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.IO;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(MIDIData))]
public class MIDIDataEditor : Editor
{
    private readonly Dictionary<int, bool> channelFoldouts = new();

    public override void OnInspectorGUI()
    {
        MIDIData data = (MIDIData)target;
        serializedObject.Update();

        // MIDI file section
        SerializedProperty midiFileProperty = serializedObject.FindProperty("midiFile");

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(midiFileProperty, new GUIContent("MIDI File"));
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        // Validate file path
        string path = AssetDatabase.GetAssetPath(data.MidiFile);

        if (string.IsNullOrEmpty(path))
        {
            data.MidiState = MIDIData.MidiDataState.None;
        }
        else if (!path.EndsWith(".mid"))
        {
            data.MidiState = MIDIData.MidiDataState.InvalidFile;
        }
        else if (data.Channels == null || data.Channels.Count == 0)
        {
            data.MidiState = MIDIData.MidiDataState.ReadyToAnalyze;
        }
        else
        {
            data.MidiState = MIDIData.MidiDataState.Analyzed;
        }

        // If the MIDI file has changed, clear the inspector window
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();

            string newPath = AssetDatabase.GetAssetPath(data.MidiFile);
            if (path != newPath)
            {
                ClearAnalysis(data);
                path = newPath;
                Repaint();

                return;
            }
        }

        // Show info based on MIDIData state 
        switch (data.MidiState)
        {
            case MIDIData.MidiDataState.None:
                EditorGUILayout.HelpBox("Selecciona un archivo MIDI para comenzar", MessageType.Info);
                break;

            case MIDIData.MidiDataState.InvalidFile:
                EditorGUILayout.HelpBox("El archivo seleccionado no es un .mid válido.", MessageType.Error);
                break;

            case MIDIData.MidiDataState.ReadyToAnalyze:
                DrawGroupingParameters();
                EditorGUILayout.Space(10);

                if (GUILayout.Button("Analyze MIDI file"))
                {
                    try
                    {
                        AnalyzeMIDI(data, path);
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError($"Error al analizar el MIDI: {ex.Message}");
                        return;
                    }
                }
                break;

            case MIDIData.MidiDataState.Analyzed:
                DrawGroupingParameters();
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                DrawAnalysisResults(data);
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                // Button to clear channels
                if (GUILayout.Button("Reset channels list"))
                {
                    data.ClearChannels();
                }
                break;
        }

        // Apply changes made from the inspector
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(data);
    }

    // === Drawing methods ===
    private void DrawGroupingParameters()
    {
        EditorGUILayout.LabelField("Grouping Parameters", EditorStyles.boldLabel);

        SerializedProperty timeWindowProperty = serializedObject.FindProperty("timeWindow");
        SerializedProperty pauseThresholdProperty = serializedObject.FindProperty("pauseThreshold");

        EditorGUILayout.PropertyField(timeWindowProperty, new GUIContent("Default Time Window"));
        EditorGUILayout.PropertyField(pauseThresholdProperty, new GUIContent("Default Pause Threshold"));
    }

    private void DrawAnalysisResults(MIDIData data)
    {
        SerializedProperty channelsProperty = serializedObject.FindProperty("channels");
        EditorGUILayout.PropertyField(channelsProperty, true);

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        // Show the first 10 notes of each channel in the inspector
        EditorGUILayout.LabelField("Notes Preview", EditorStyles.boldLabel);
        foreach (var channel in data.Channels)
        {
            if (channel.RawNotes.Count == 0) continue;

            // Create the foldout if it doesn't already exist in the dictionary
            if (!channelFoldouts.ContainsKey(channel.ChannelNumber)) channelFoldouts[channel.ChannelNumber] = false;
            channelFoldouts[channel.ChannelNumber] = EditorGUILayout.Foldout(channelFoldouts[channel.ChannelNumber], channel.ChannelName, false);

            if (channelFoldouts[channel.ChannelNumber])
            {
                EditorGUI.indentLevel++;

                int previewCount = Mathf.Min(10, channel.RawNotes.Count);
                for (int i = 0; i < previewCount; i++)
                {
                    var note = channel.RawNotes[i];
                    EditorGUILayout.LabelField($"[{i + 1}] {note.name}{note.octave} - Start:{note.startTime:F3}s - Duration:{note.duration:F3}s");
                }

                EditorGUI.indentLevel--;
                EditorGUILayout.Space(5);
            }
        }
    }

    // === Analysis methods ===
    private void AnalyzeMIDI(MIDIData data, string assetPath)
    {
        // Read MIDI file
        string fullPath = Path.Combine(Application.dataPath, assetPath["Assets/".Length..]); // Convert the relative path to absolute
        MidiFile midiFile = MidiFile.Read(fullPath);
        TempoMap tempoMap = midiFile.GetTempoMap();
        ICollection<Note> notes = midiFile.GetNotes();

        // Clear previous channels
        data.ClearChannels();

        // Group notes by channel
        Dictionary<int, MIDIChannelData> channelMap = new();

        // Scan and convert each note into readable information
        foreach (var note in notes)
        {
            int channel = note.Channel;

            // Create the channel if it doesn't already exist in the dictionary
            if (!channelMap.ContainsKey(channel)) channelMap[channel] = new MIDIChannelData(channel);

            // Convert position (ticks) to real time (seconds)
            var startMetric = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, tempoMap);
            var lengthMetric = TimeConverter.ConvertTo<MetricTimeSpan>(note.Length, tempoMap);

            double startSeconds = startMetric.Minutes * 60 + startMetric.Seconds + startMetric.Milliseconds / 1000.0;
            double durationSeconds = lengthMetric.Minutes * 60 + lengthMetric.Seconds + lengthMetric.Milliseconds / 1000.0;

            // Add the note to the corresponding channel
            channelMap[channel].RawNotes.Add(new MIDINoteData
            {
                name = note.NoteName.ToString(),
                octave = note.Octave,
                startTime = startSeconds,
                duration = durationSeconds,
                channel = note.Channel
            });
        }

        // Save channels in ScriptableObject
        data.Channels.AddRange(channelMap.Values.OrderBy(channel => channel.ChannelNumber));

        // Group notes for each channel
        foreach (var channelData in data.Channels)
        {
            channelData.GroupNotes(data.TimeWindow, data.PauseThreshold);
        }

        // Mark and save changes to the asset
        EditorUtility.SetDirty(data);
        AssetDatabase.SaveAssets();
    }

    private void ClearAnalysis(MIDIData data)
    {
        if (data == null) return;

        data.ClearChannels();
        data.MidiState = MIDIData.MidiDataState.None;
    }
}