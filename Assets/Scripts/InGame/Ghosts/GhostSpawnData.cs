using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class GhostSpawnData
{
    [Header("Pattern Time Settings")]
    [SerializeField, Min(0)] private int patternStartMin;
    [SerializeField, Range(0, 59)] private float patternStartSec;
    [SerializeField, Min(0), Space(10)] private int patternEndMin;
    [SerializeField, Range(0, 59)] private float patternEndSec;

    [Header("Spawner & Interaction Zone Settings")]
    [SerializeField] private SpawnerData.SpawnHeight spawnHeight;
    [SerializeField] private GameObject[] interactionZones;
    private Dictionary<GhostMovement.GhostDirection, GameObject> interactionZonesDictionary;

    public enum GhostSize { Small, Medium, Big }
    [Header("Ghost Settings")]
    [SerializeField] private GhostSize ghostSize;
    [SerializeField] private GhostMovement.GhostDirection ghostDirection;
    [SerializeField] private PatternData patternData;
    [SerializeField] private GameObject[] ghostPools;
    private Dictionary<GhostSize, ObjectPool> ghostPoolDictionary;
    private float speedMovement;
    [SerializeField] private float spawnTime;

    // === Extra validation ===
    private bool isInitialized = false;

    // === Properties ===
    public SpawnerData.SpawnHeight SpawnHeight => spawnHeight;
    public GhostSize Size => ghostSize;
    public GhostMovement.GhostDirection Direction => ghostDirection;
    public PatternData Pattern => patternData;
    public Dictionary<GhostSize, ObjectPool> GhostPoolDictionary => ghostPoolDictionary;
    public float SpeedMovement => speedMovement;
    public float SpawnTime => spawnTime;

    // === Initialization methods ===
    public void Initialize()
    {
        if (isInitialized) return; // Avoid double initialization
        isInitialized = true;

        InitializeDictionaries();
        CalculateSpeed();
        CalculateSpawnTime();
    }

    private void InitializeDictionaries()
    {
        ghostPoolDictionary = new()
        {
            {GhostSize.Small, FindPool("SmallGhostPool")},
            {GhostSize.Medium, FindPool("MediumGhostPool")},
            {GhostSize.Big, FindPool("BigGhostPool")}
        };

        interactionZonesDictionary = new()
        {
            {GhostMovement.GhostDirection.Left, FindInteractionZone("Right")},
            {GhostMovement.GhostDirection.Right, FindInteractionZone("Left") }
        };
    }

    private GameObject FindInteractionZone(string interactionZoneName)
    {
        GameObject poolObject = interactionZones.FirstOrDefault(zone => zone.name == interactionZoneName);

        if (poolObject == null)
        {
            Debug.LogWarning($"La zona de interacción '{interactionZoneName}' no se econtró para el fantasma {ghostSize}");
            return null;
        }
        return poolObject;
    }

    private ObjectPool FindPool(string poolName)
    {
        GameObject poolObject = ghostPools.FirstOrDefault(pool => pool.name == poolName);

        if (poolObject == null)
        {
            Debug.LogWarning($"La pool '{poolName}' no se econtró para el fantasma {ghostSize}");
            return null;
        }
        return poolObject.GetComponent<ObjectPool>();
    }

    private void CalculateSpeed()
    {
        /* 
            Formula for the speed of the pattern within the interaction zone:
            patternDuration = patternEndTime - patternStartTime
            speedMovement = patternWidth / totalTime
        */

        float patternStartTime = (patternStartMin * 60f) + patternStartSec;
        float patternEndTime = (patternEndMin * 60f) + patternEndSec;
        float patternDuration = patternEndTime - patternStartTime;

        float patternWidth = (patternData.NotesQuantity - 1) * patternData.HorizontalSpacing;

        speedMovement = patternWidth / patternDuration;
    }

    private void CalculateSpawnTime()
    {
        /* 
            Formula for the spawn time:
            travelTime = distanceToCenter / speedMovement
            spawnTime = patternStartTime - travelTime
        */

        GameObject interactionZone = interactionZonesDictionary[ghostDirection];
        GameObject spawner = GameManager.instance.SpawnManager.GetComponent<SpawnManager>().SpawnersDictionary[ghostDirection];

        // Midpoint of the interaction zone
        Collider2D interactionCollider = interactionZone.GetComponent<Collider2D>();
        float interactionCenterX = interactionCollider.bounds.center.x;

        // Distance and timing
        float distanceToCenter = Mathf.Abs(spawner.transform.position.x - interactionCenterX);
        float patternStartTime = (patternStartMin * 60f) + patternStartSec;

        // Calculate when to spawn
        spawnTime = patternStartTime - (distanceToCenter / speedMovement);
        spawnTime = Mathf.Round(spawnTime * 1000f) / 1000f; // Rounded to three decimal places for greater precision in ms
    }
}