using UnityEngine;

[DefaultExecutionOrder(-1)]
public class GlobalGameManager : MonoBehaviour
{
    // === Singleton ===
    public static GlobalGameManager instance;

    // === Managers ===
    private GameObject inputManager;

    // === Properties ===
    public GameObject InputManager => inputManager;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeManagers();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeManagers()
    {
        if (inputManager == null) inputManager = transform.GetChild(0).gameObject;
    }
}