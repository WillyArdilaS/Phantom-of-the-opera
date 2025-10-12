using UnityEngine;

public class RightNoteDetector : AbstractNoteDetector
{
    private RightController rightController;

    void Awake()
    {
        rightController = GlobalGameManager.instance.InputManager.GetComponent<RightController>();
        rightController.KeyPressed += CheckNoteInput;
    }

    void OnDestroy()
    {
        if (rightController != null) rightController.KeyPressed -= CheckNoteInput;
    }

    // === Overridden abstract methods ===
    protected override void CheckNoteInput()
    {
        if (activeNote == null) return;

        if (activeNote.State == NoteManager.NoteState.Highlighted)
        {
            if (rightController.CurrentKey.ToString() == activeNote.RequiredKey.ToString())
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