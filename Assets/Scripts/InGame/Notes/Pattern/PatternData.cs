using UnityEngine;

[CreateAssetMenu(fileName = "New Pattern Data", menuName = "ScriptableObjects/PatternData")]
public class PatternData : ScriptableObject
{
    // === Data fields ===
    [SerializeField, Range(1, 5)] private int notesQuantity;
    [SerializeField, Tooltip("Horizontal space between one note and another within the pattern")] private float horizontalSpacing;

    // === Properties ===
    public int NotesQuantity { get => notesQuantity; set => notesQuantity = value; }
    public float HorizontalSpacing => horizontalSpacing;
}