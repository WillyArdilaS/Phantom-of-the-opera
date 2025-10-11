using System.Collections.Generic;
using UnityEngine;

public class PatternGenerator : MonoBehaviour
{
    // === Note settings ===
    [SerializeField, Range(1, 5)] private int notesQuantity;
    [SerializeField, Tooltip("Horizontal space between one note and another within the pattern")] private float horizontalSpacing;
    [SerializeField, Tooltip("Vertical distance between the pattern and the ghost")] private float verticalOffset;
    [SerializeField] private GameObject[] notePrefabs;
    [SerializeField] private List<GameObject> notesList = new();

    void Awake()
    {
        CreateNotePattern();
    }

    void OnDisable()
    {
        notesList.Clear();
    }

    private void CreateNotePattern()
    {
        int randomIndex;
        
        /* 
            Formula for centering the entire pattern:
            totalWidth = (notesQuantity − 1) ∗ horizontalSpacing
            startXPos = −totalWidth​ / 2 
        */
        float startXPos = -((notesQuantity - 1) * horizontalSpacing) / 2f; 

        for (int i = 0; i < notesQuantity; i++)
        {
            randomIndex = Random.Range(0, notePrefabs.Length);

            float posX = startXPos + (i * horizontalSpacing); // Position of the first note + the required spacing
            Vector3 finalPos = new(posX, verticalOffset, 0); // Local position relative to the father

            // Prefab instantiation
            GameObject noteCreated = Instantiate(notePrefabs[randomIndex], transform);

            noteCreated.transform.localPosition = finalPos;
            notesList.Add(noteCreated);
        }
    }
}