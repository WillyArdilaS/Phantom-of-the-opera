using UnityEngine;

[RequireComponent(typeof(Animator))]
public class RightNoteDetector : AbstractNoteDetector
{
    private RightController rightController;

    void Awake()
    {
        detectorAnimator = GetComponent<Animator>();
        rightController = GlobalGameManager.instance.InputManager.GetComponent<RightController>();
        rightController.OnKeyPressed += CheckNoteInput;
    }

    void OnDestroy()
    {
        if (rightController != null) rightController.OnKeyPressed -= CheckNoteInput;
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

            activeNote.NoteAnimator.SetTrigger("t_isPlaying");
            detectorAnimator.SetTrigger("t_isPlaying");
        }
    }
}