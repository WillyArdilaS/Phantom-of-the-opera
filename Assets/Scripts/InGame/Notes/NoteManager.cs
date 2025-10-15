using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class NoteManager : MonoBehaviour, IKeyMapping
{
    // === Key Mapping ===
    public enum NoteDirection { Up, Left, Down, Right }
    [SerializeField] private NoteDirection noteDirection;
    [SerializeField] private IKeyMapping.Key requiredKey;

    // === Damage ===
    [SerializeField, Space(10)] private float damage;

    // === States ===
    public enum NoteState { Waiting, Highlighted, Successful, Failed }
    [SerializeField, Space(10)] private NoteState noteState;

    // === Sprites ===
    [SerializeField, Space(10)] private Sprite[] noteSprites;
    private Dictionary<NoteState, Sprite> spritesDictionary;
    private SpriteRenderer spriteRend;

    // === Events ===
    public event Action<NoteManager> OnNoteResolved;

    // === Properties ===
    public IKeyMapping.Key RequiredKey { get => requiredKey; set => requiredKey = value; }
    public NoteDirection Direction => noteDirection;
    public NoteState State => noteState;

    void Awake()
    {
        spriteRend = GetComponent<SpriteRenderer>();

        spritesDictionary = new() 
        {
            {NoteState.Highlighted, noteSprites.FirstOrDefault(sprite => sprite.name == "HighlightedNote")},
            {NoteState.Successful, noteSprites.FirstOrDefault(sprite => sprite.name == "SuccessfulNote")},
            {NoteState.Failed, noteSprites.FirstOrDefault(sprite => sprite.name == "FailedNote")},
        };
    }

    // === Interface implementation ===
    public void AssignKey(NoteDirection noteDirection, GhostMovement.GhostDirection ghostDirection)
    {
        // Dictionaries for each ghost direction
        var leftKeys = new Dictionary<NoteDirection, IKeyMapping.Key>
        {
            { NoteDirection.Up, IKeyMapping.Key.W },
            { NoteDirection.Left, IKeyMapping.Key.A },
            { NoteDirection.Down, IKeyMapping.Key.S },
            { NoteDirection.Right, IKeyMapping.Key.D },
        };

        var rightKeys = new Dictionary<NoteDirection, IKeyMapping.Key>
        {
            { NoteDirection.Up, IKeyMapping.Key.UpArrow },
            { NoteDirection.Left, IKeyMapping.Key.LeftArrow },
            { NoteDirection.Down, IKeyMapping.Key.DownArrow },
            { NoteDirection.Right, IKeyMapping.Key.RightArrow },

        };

        // Select the dictionary according to the ghost direction 
        var keyMap = ghostDirection == GhostMovement.GhostDirection.Right ? leftKeys : rightKeys;

        // Assign the corresponding key
        if (keyMap.TryGetValue(noteDirection, out IKeyMapping.Key mappedKey))
        {
            requiredKey = mappedKey;
        }
        else
        {
            requiredKey = IKeyMapping.Key.None;
        }
    }

    // === Change note status mehods ===
    public void MarkAsHighlighted()
    {
        spriteRend.sprite = spritesDictionary[NoteState.Highlighted];
        noteState = NoteState.Highlighted;
    }

    public void MarkAsSuccess()
    {
        spriteRend.sprite = spritesDictionary[NoteState.Successful];
        noteState = NoteState.Successful;
        OnNoteResolved?.Invoke(this);
    }

    public void MarkAsFailed()
    {
        spriteRend.sprite = spritesDictionary[NoteState.Failed];
        noteState = NoteState.Failed;
        OnNoteResolved?.Invoke(this);
    }
}