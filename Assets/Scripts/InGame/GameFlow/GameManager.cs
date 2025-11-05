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
    [SerializeField, Tooltip("Delay time in seconds. It must be greater than the absolute value of the smallest spawnTime")] private float startDelay;
    private float globalTime;

    // === Audio ===
    [SerializeField] private AudioSource audioSource;
    private bool audioStarted = false;

    // === Properties ===
    public GameObject SpawnManager => spawnManager;
    public GameState State { get => gameState; set => gameState = value; }
    public AudioSource Audio => audioSource;
    public float StartDelay => startDelay;
    public float GlobalTime => globalTime;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            InitializeManagers();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        globalTime += Time.deltaTime;

        if (!audioStarted && globalTime >= startDelay)
        {
            audioSource.Play();
            audioStarted = true;
        }

        if (audioStarted && gameState == GameState.Playing && audioSource.time >= audioSource.clip.length)
        {
            OnLevelEnd();
        }
    }

    private void InitializeManagers()
    {
        if (spawnManager == null) spawnManager = transform.GetChild(0).gameObject;
    }

    private void OnLevelEnd()
    {
        gameState = GameState.Finishing;
        audioSource.Stop();

        // *Aquí luego va lo relacionado al final del nivel*
        // Ej: mostrar score final
    }
}