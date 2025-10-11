public interface IKeyMapping
{
    public enum Key { None, W, A, S, D, UpArrow, LeftArrow, DownArrow, RightArrow }

    public void AssignKey(NoteManager.NoteDirection noteDirection, GhostMovement.GhostDirection ghostDirection);
}