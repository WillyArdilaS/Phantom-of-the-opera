using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    // === Spawners ===
    [SerializeField] private GameObject[] spawners;
    private Dictionary<GhostMovement.GhostDirection, GameObject> spawnersDictionary;

    //=== Interaction zones ===
    [SerializeField] private GameObject[] interactionZones;
    private Dictionary<GhostMovement.GhostDirection, GameObject> interactionZonesDictionary;

    // === Ghost pools ===
    public enum GhostSize { Small, Medium, Big }
    [SerializeField] private GameObject[] ghostPools;
    private Dictionary<GhostSize, GameObject> ghostPoolDictionary;

    // === Patterns data ===
    [SerializeField] private PatternData[] patternDataArray;
    private Dictionary<int, PatternData> patternDataDictionary;

    // === MIDI Data ===
    [SerializeField] private MIDIData MIDIDataPrefab;

    // === Spawn Data ===
    [SerializeField] private List<ChannelSpawnGroup> channelGroups = new();
    [SerializeField, HideInInspector] private List<GhostSpawnData> ghostSpawnEvents;
    private int ghostSpawnIndex = 0;

    // === Properties ===
    public List<ChannelSpawnGroup> ChannelGroups => channelGroups;
    public Dictionary<GhostMovement.GhostDirection, GameObject> SpawnersDictionary => spawnersDictionary;
    public Dictionary<GhostMovement.GhostDirection, GameObject> InteractionZonesDictionary => interactionZonesDictionary;
    public PatternData[] PatternDataArray => patternDataArray;
    public Dictionary<int, PatternData> PatternDataDictionary => patternDataDictionary;
    public MIDIData MIDIPrefab => MIDIDataPrefab;

    void Awake()
    {
        InitializeDictionaries();

        // Sort and prepare spawn events according to spawn time
        ghostSpawnEvents = channelGroups.SelectMany(channel => channel.GhostDataList).OrderBy(spawnEvent => spawnEvent.SpawnTime).ToList();
        ghostSpawnIndex = 0;
    }

    void Update()
    {
        if (ghostSpawnEvents == null || ghostSpawnEvents.Count == 0) return;
        if (ghostSpawnIndex >= ghostSpawnEvents.Count) return;

        float currentGlobalTime = GameManager.instance.GlobalTime;
        float adjustedSpawnTime = ghostSpawnEvents[ghostSpawnIndex].SpawnTime + GameManager.instance.StartDelay;

        if (currentGlobalTime >= adjustedSpawnTime)
        {
            SpawnGhost(ghostSpawnEvents[ghostSpawnIndex]);
            ghostSpawnIndex++;
        }
    }

    // === Initialization methods ===
    public void InitializeDictionaries()
    {
        spawnersDictionary = new()
        {
            {GhostMovement.GhostDirection.Left, spawners.FirstOrDefault(spawn => spawn.name == "RightSpawner")},
            {GhostMovement.GhostDirection.Right, spawners.FirstOrDefault(spawn => spawn.name == "LeftSpawner")}
        };

        ghostPoolDictionary = new()
        {
            {GhostSize.Small, ghostPools.FirstOrDefault(pool => pool.name == "SmallGhostPool")},
            {GhostSize.Medium,  ghostPools.FirstOrDefault(pool => pool.name == "MediumGhostPool")},
            {GhostSize.Big,  ghostPools.FirstOrDefault(pool => pool.name == "BigGhostPool")}
        };

        interactionZonesDictionary = new()
        {
            {GhostMovement.GhostDirection.Left, interactionZones.FirstOrDefault(zone => zone.name == "RightZone")},
            {GhostMovement.GhostDirection.Right, interactionZones.FirstOrDefault(zone => zone.name == "LeftZone")}
        };

        patternDataDictionary = new()
        {
            {1, patternDataArray.FirstOrDefault(pattern => pattern.name == "1NotePattern")},
            {2, patternDataArray.FirstOrDefault(pattern => pattern.name == "2NotePattern")},
            {3, patternDataArray.FirstOrDefault(pattern => pattern.name == "3NotePattern")},
            {4, patternDataArray.FirstOrDefault(pattern => pattern.name == "4NotePattern")},
            {5, patternDataArray.FirstOrDefault(pattern => pattern.name == "5NotePattern")}
        };
    }

    // === Spawn methods ===
    private void SpawnGhost(GhostSpawnData spawnData)
    {
        if (!ghostPoolDictionary[spawnData.Size].TryGetComponent<ObjectPool>(out var pool))
        {
            Debug.LogError($"[SpawnManager] A {pool.name} le hace falta el componente ObjectPool");
            return;
        }
        else
        {
            GameObject ghostSpawned = ghostPoolDictionary[spawnData.Size].GetComponent<ObjectPool>().GetObjectFromPool();
            InitializeNewGhost(ghostSpawned, spawnData);
        }
    }

    private void InitializeNewGhost(GameObject ghostSpawned, GhostSpawnData spawnData)
    {
        // Set position
        GameObject spawner = spawnersDictionary[spawnData.Direction];
        float height = spawner.GetComponent<SpawnerData>().GetHeight(spawnData.SpawnHeight);
        ghostSpawned.transform.position = spawner.transform.position + Vector3.up * height;

        // Set movement
        ghostSpawned.GetComponent<GhostMovement>().InitializeGhost(spawnData.Direction, spawnData.SpeedMovement);

        // Set notes pattern
        PatternGenerator pattern = ghostSpawned.GetComponent<PatternGenerator>();
        pattern.Pattern = spawnData.Pattern;
        pattern.CreateNotePattern();

        // Set despawner
        ghostSpawned.GetComponent<GhostDespawner>().InitializePendingNotes();
    }
}