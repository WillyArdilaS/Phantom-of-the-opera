using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GhostMovement))]
public class PatternGenerator : MonoBehaviour
{
    // === Ghost References ===
    private GhostMovement ghostMovement;

    // === Note settings ===
    [SerializeField] private PatternData patternData;
    [SerializeField, Tooltip("Vertical distance between the pattern and the ghost")] private float verticalOffset;
    [SerializeField, Space(10)] private GameObject[] notePrefabs;
    [SerializeField] private List<GameObject> notesList = new();

    // === Properties ===
    public PatternData Pattern { get => patternData; set => patternData = value; }
    public IReadOnlyList<GameObject> NotesList => notesList;

    void Awake()
    {
        ghostMovement = GetComponent<GhostMovement>();
    }

    void OnDisable()
    {
        foreach (var note in notesList)
        {
            if (note != null) Destroy(note);
        }

        notesList.Clear();
    }

    public void CreateNotePattern()
    {
        int randomIndex;

        /* 
            Formula for centering the entire pattern:
            totalWidth = (notesQuantity − 1) ∗ horizontalSpacing
            startXPos = totalWidth​ / 2 
        */
        int directionSign = ghostMovement.Direction == GhostMovement.GhostDirection.Right ? 1 : -1; // Mirror pattern horizontally depending on ghost direction
        float startXPos = (((patternData.NotesQuantity - 1) * patternData.HorizontalSpacing) / 2f) * directionSign;

        for (int i = 0; i < patternData.NotesQuantity; i++)
        {
            randomIndex = Random.Range(0, notePrefabs.Length);

            float posX = startXPos - (i * patternData.HorizontalSpacing * directionSign);
            Vector3 finalPos = new(posX, verticalOffset, 0); // Local position relative to the father

            // Prefab instantiation
            GameObject noteCreated = Instantiate(notePrefabs[randomIndex], transform);
            InitializeNote(noteCreated, finalPos);

            notesList.Add(noteCreated);
        }
    }

    private void InitializeNote(GameObject noteCreated, Vector3 localPos)
    {
        NoteManager noteManager = noteCreated.GetComponent<NoteManager>();

        noteManager.AssignKey(noteManager.Direction, ghostMovement.Direction);
        noteCreated.transform.localPosition = localPos;
    }
}