using System.Collections.Generic;
using UnityEngine;

public class SpawnerData : MonoBehaviour
{
    // === Heights ===
    public enum SpawnHeight { Low, Medium, High }
    [SerializeField] private float lowHeight;
    [SerializeField] private float mediumHeight;
    [SerializeField] private float highHeight;
    private Dictionary<SpawnHeight, float> spawnHeightDictionary;

    // === Properties ===
    public Dictionary<SpawnHeight, float> SpawnHeightDictionary => spawnHeightDictionary;

    void Awake()
    {
        spawnHeightDictionary = new()
        {
            {SpawnHeight.Low, lowHeight},
            {SpawnHeight.Medium, mediumHeight},
            {SpawnHeight.High, highHeight},
        };
    }

    public float GetHeight(SpawnHeight height)
    {
        if (!spawnHeightDictionary.ContainsKey(height))
        {
            Debug.LogWarning($"La altura {height} No se encontró en el SpawnerData de {gameObject.name}");
            return 0f;
        }
        return spawnHeightDictionary[height];
    }
}