using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    // === Spawners ===
    [SerializeField] private GameObject[] spawners;
    private Dictionary<GhostMovement.GhostDirection, GameObject> spawnersDictionary;

    // === Spawn events management ===
    [SerializeField] private GhostSpawnData[] ghostSpawnEvents;
    private int ghostSpawnIndex = 0;

    // === Properties ===
    public Dictionary<GhostMovement.GhostDirection, GameObject> SpawnersDictionary => spawnersDictionary;

    void Awake()
    {
        // Initialize dictionaries
        spawnersDictionary = new()
        {
            {GhostMovement.GhostDirection.Left, spawners.FirstOrDefault(spawn => spawn.name == "RightSpawner")},
            {GhostMovement.GhostDirection.Right, spawners.FirstOrDefault(spawn => spawn.name == "LeftSpawner")}
        };

        // Initialize the ghostSpawnEvents and sort them by SpawnTime
        foreach (var data in ghostSpawnEvents)
        {
            data.Initialize();
        }
        ghostSpawnEvents = ghostSpawnEvents.OrderBy(spawnEvent => spawnEvent.SpawnTime).ToArray();
    }

    void Update()
    {
        if (ghostSpawnIndex >= ghostSpawnEvents.Count()) return;

        if (GameManager.instance.Audio.time >= ghostSpawnEvents[ghostSpawnIndex].SpawnTime)
        {
            SpawnGhost(ghostSpawnEvents[ghostSpawnIndex]);
            ghostSpawnIndex++;
        }
    }

    private void SpawnGhost(GhostSpawnData spawnData)
    {
        GameObject ghostSpawned = spawnData.GhostPoolDictionary[spawnData.Size].GetObjectFromPool();
        InitializeNewGhost(ghostSpawned, spawnData);
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
        pattern.InitializeNotePattern();

        // Set despawner
        ghostSpawned.GetComponent<GhostDespawner>().InitializePendingNotes();
    }
}