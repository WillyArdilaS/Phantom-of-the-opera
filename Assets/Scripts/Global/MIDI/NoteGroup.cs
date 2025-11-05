using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NoteGroup
{
    [SerializeField, HideInInspector] private int groupIndex;
    [SerializeField] private float groupStartTime;
    [SerializeField] private float groupEndTime;
    [SerializeField] private List<MIDINoteData> notes = new();

    // === Properties ===
    public int GroupIndex => groupIndex;
    public float GroupStartTime => groupStartTime;
    public float GroupEndTime { get => groupEndTime; set => groupEndTime = value; }
    public List<MIDINoteData> Notes => notes;

    // === Constructor ===
    public NoteGroup(int index, float start, float end)
    {
        groupIndex = index;
        groupStartTime = start;
        groupEndTime = end;
    }

    public void AddNote(MIDINoteData note)
    {
        notes.Add(note);
    }
}