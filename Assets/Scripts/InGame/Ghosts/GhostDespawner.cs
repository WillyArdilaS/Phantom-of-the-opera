using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PatternGenerator))]
public class GhostDespawner : MonoBehaviour
{
    // === Notes references ===
    private PatternGenerator patternGenerator;
    private readonly List<NoteManager> pendingNotes = new();

    // === Despawn ===
    [SerializeField] private float despawnCooldown;

    // === Coroutines ===
    private Coroutine despawnRoutine;

    void Awake()
    {
        patternGenerator = GetComponent<PatternGenerator>();
    }

    void OnEnable()
    {
       if (pendingNotes.Count == 0) InitializePendingNotes();
    }

    void OnDisable()
    {
        foreach (var note in pendingNotes)
        {
            if (note != null) note.OnNoteResolved -= CheckDespawnCondition;
        }

        pendingNotes.Clear();
    }

    private void InitializePendingNotes()
    {
        for (int i = 0; i < patternGenerator.NotesList.Count; i++)
        {
            pendingNotes.Add(patternGenerator.NotesList[i].GetComponent<NoteManager>());
            pendingNotes[i].OnNoteResolved += CheckDespawnCondition;
        }
    }

    private void CheckDespawnCondition(NoteManager lastNote)
    {
        pendingNotes.Remove(lastNote);

        if (pendingNotes.Count == 0)
        {
            if (despawnRoutine != null) StopCoroutine(despawnRoutine);
            despawnRoutine = StartCoroutine(StartDespawn());
        }
    }

    private IEnumerator StartDespawn()
    {
        yield return new WaitForSeconds(despawnCooldown);
        gameObject.SetActive(false);
    }
}