using UnityEngine;

public class LeftNoteDetector : AbstractNoteDetector
{
    private LeftController leftController;

    void Awake()
    {
        leftController = GlobalGameManager.instance.InputManager.GetComponent<LeftController>();
        leftController.OnKeyPressed += CheckNoteInput;
    }

    void OnDestroy()
    {
        if (leftController != null) leftController.OnKeyPressed -= CheckNoteInput;
    }

    // === Overridden abstract methods ===
    protected override void CheckNoteInput()
    {
        if (activeNote == null) return;

        if (activeNote.State == NoteManager.NoteState.Highlighted)
        {
            if (leftController.CurrentKey.ToString() == activeNote.RequiredKey.ToString())
            {
                activeNote.MarkAsSuccess();
            }
            else
            {
                activeNote.MarkAsFailed();
            }
        }
    }
}