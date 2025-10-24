using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class MIDIChannelData
{
    // === Channel ===
    private readonly int channelNumber;
    [SerializeField] private string channelName;

    // === Notes ===
    [SerializeField, HideInInspector] private List<MIDINoteData> rawNotes = new();
    [SerializeField] private List<NoteGroup> noteGroups = new();

    // === Properties ===
    public int ChannelNumber => channelNumber;
    public string ChannelName => channelName;
    public List<MIDINoteData> RawNotes => rawNotes;
    public List<NoteGroup> NoteGroups => noteGroups;

    // === Constructor ===
    public MIDIChannelData(int number)
    {
        channelNumber = number;
        channelName = $"Channel {number}";
    }

    public void ClearGroups() => noteGroups.Clear();

    public void GroupNotes(float timeWindow, float pauseThreshold)
    {
        if (rawNotes == null || rawNotes.Count == 0) return;

        noteGroups.Clear();

        // Sort by start time
        rawNotes.Sort((a, b) => a.startTime.CompareTo(b.startTime));

        // Create first group
        int groupIndex = 0;
        double currentWindowStart = rawNotes[0].startTime;
        NoteGroup currentGroup = new(groupIndex, (float)currentWindowStart, (float)(currentWindowStart + timeWindow));
        currentGroup.AddNote(rawNotes[0]);

        // Process all raw notes
        for (int i = 1; i < rawNotes.Count; i++)
        {
            MIDINoteData prevNote = rawNotes[i - 1];
            MIDINoteData currentNote = rawNotes[i];

            double timeGap = currentNote.startTime - (prevNote.startTime + prevNote.duration);
            double windowEnd = currentWindowStart + timeWindow;

            // Time window and pause treshold validations
            bool exceedsWindow = currentNote.startTime > windowEnd;
            bool exceedsPause = timeGap > pauseThreshold;

            // If the note goes outside the time window or the pause is too long, close the current group
            if (exceedsWindow || exceedsPause)
            {
                // Adjust the actual end time of the group according to its last note
                float lastNoteEnd = currentGroup.Notes.Max(note => (float)(note.startTime + note.duration));
                currentGroup.GroupEndTime = lastNoteEnd;

                noteGroups.Add(currentGroup);

                // Create new group
                groupIndex++;
                currentWindowStart = currentNote.startTime;
                currentGroup = new NoteGroup(groupIndex, (float)currentWindowStart, (float)(currentWindowStart + timeWindow));
            }

            // Add the current note to the current group
            currentGroup.AddNote(currentNote);
        }

        // Save last group
        if (currentGroup.Notes.Count > 0)
        {
            float lastNoteEnd = currentGroup.Notes.Max(n => (float)(n.startTime + n.duration));
            currentGroup.GroupEndTime = lastNoteEnd;

            noteGroups.Add(currentGroup);
        }
    }
}