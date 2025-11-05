using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GhostSpawnData
{
    // === Settings ===
    [Header("Time Settings")]
    [SerializeField] private float patternStartTime;
    [SerializeField] private float patternEndTime;
    [SerializeField, Tooltip("Relative to the audio start time. Negative values mean the ghost should appear before the music starts")] private float spawnTime;

    [Header("Ghost Settings")]
    [SerializeField] private SpawnManager.GhostSize ghostSize;
    [SerializeField] private SpawnerData.SpawnHeight spawnHeight;
    [SerializeField] private GhostMovement.GhostDirection ghostDirection;
    [SerializeField] private PatternData patternData;
    
    [SerializeField] private float speedMovement;

    // === Ghost Notes ===
    [SerializeField] private List<MIDINoteData> activeNotes = new();

    // === Properties ===
    public SpawnerData.SpawnHeight SpawnHeight => spawnHeight;
    public SpawnManager.GhostSize Size => ghostSize;
    public GhostMovement.GhostDirection Direction => ghostDirection;
    public PatternData Pattern => patternData;
    public float SpeedMovement => speedMovement;
    public float SpawnTime => spawnTime;
}