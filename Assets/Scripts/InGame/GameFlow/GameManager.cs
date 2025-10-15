using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // === Singleton ===
    public static GameManager instance;

    // === Managers ===
    private GameObject spawnManager;

    // === States ===
    public enum GameState { Playing, InPause, Finishing }
    [SerializeField] private GameState gameState = GameState.Playing;

    // === Game start ===
    [SerializeField, Tooltip("Delay time in seconds")] private float startDelay;
    [SerializeField] private AudioSource audioSource;

    // === Coroutines ===
    private Coroutine startGameRoutine;

    // === Properties ===
    public GameObject SpawnManager => spawnManager;
    public GameState State { get => gameState; set => gameState = value; }
    public AudioSource Audio => audioSource;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            InitializeManagers();

            if (startGameRoutine != null) StopCoroutine(StartGame());
            startGameRoutine = StartCoroutine(StartGame());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (gameState == GameState.Playing && audioSource.time == audioSource.clip.length)
        {
            OnLevelEnd();
        }
    }

    private void InitializeManagers()
    {
        if (spawnManager == null) spawnManager = transform.GetChild(0).gameObject;
    }

    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(startDelay);
        if (audioSource != null) audioSource.Play();
    }

    private void OnLevelEnd()
    {
        gameState = GameState.Finishing;
        audioSource.Stop();

        // *Aquí luego va lo relacionado al final del nivel*
        // Ej: mostrar score final
    }
}