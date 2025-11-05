using UnityEditor.Animations;
using UnityEngine;

public abstract class AbstractNoteDetector : MonoBehaviour
{
    [SerializeField] protected NoteManager activeNote;
    protected Animator detectorAnimator;

    protected abstract void CheckNoteInput();

    void OnTriggerEnter2D(Collider2D collision)
    {
        activeNote = collision.GetComponent<NoteManager>();
        if (activeNote != null && activeNote.State == NoteManager.NoteState.Waiting) activeNote.MarkAsHighlighted();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        NoteManager exitNote = collision.GetComponent<NoteManager>();
        if (exitNote.State == NoteManager.NoteState.Highlighted) exitNote.MarkAsFailed();

        if (exitNote == activeNote) activeNote = null;
    }
}