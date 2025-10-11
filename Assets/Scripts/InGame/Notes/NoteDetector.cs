using UnityEngine;

public class NoteDetector : MonoBehaviour
{
    [SerializeField] private NoteManager activeNote;

    void OnTriggerEnter2D(Collider2D collision)
    {
        activeNote = collision.GetComponent<NoteManager>();
        if (activeNote != null) activeNote.MarkAsHighlighted();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        NoteManager exitNote = collision.GetComponent<NoteManager>();
        if (exitNote.State == NoteManager.NoteState.Highlighted) exitNote.MarkAsFailed();

        if (exitNote == activeNote) activeNote = null;
    }
}