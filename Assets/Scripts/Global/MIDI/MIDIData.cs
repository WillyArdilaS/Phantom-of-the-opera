using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New MIDI Data", menuName = "ScriptableObjects/MIDIData")]
public class MIDIData : ScriptableObject
{
    // === MIDI analysis ===
    public enum MidiDataState { None, InvalidFile, ReadyToAnalyze, Analyzed }
    [HideInInspector] private MidiDataState midiDataState = MidiDataState.None;
    [SerializeField] private DefaultAsset midiFile;

    // === Channels ===
    [SerializeField] private List<MIDIChannelData> channels = new();

    // === Grouping parameters ===
    [SerializeField, Range(0f, 1f)] private float timeWindow;
    [SerializeField, Range(0f, 1f)] private float pauseThreshold;

    // === Properties ===
    public MidiDataState MidiState { get => midiDataState; set => midiDataState = value; }
    public DefaultAsset MidiFile { get => midiFile; set => midiFile = value; }
    public List<MIDIChannelData> Channels => channels;
    public float TimeWindow => timeWindow;
    public float PauseThreshold => pauseThreshold;

    public void ClearChannels() => channels.Clear();
}