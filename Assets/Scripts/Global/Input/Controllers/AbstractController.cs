using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class AbstractController : MonoBehaviour
{
    // === Key states ===
    public enum KeyState { None, W, A, S, D, UpArrow, LeftArrow, DownArrow, RightArrow }
    [SerializeField] protected KeyState keyState = KeyState.None;
    private const float keyHoldTime = 0.1f;

    // === Coroutines ===
    protected Coroutine resetKeyRoutine;

    // === Events ===
    public event Action KeyPressed;

    // === Properties ===
   public KeyState CurrentKey => keyState;

    public abstract void ProcessInput(InputAction.CallbackContext ctx);

    protected void InvokeKeyEvent()
    {
        KeyPressed?.Invoke();
    }

    protected IEnumerator ResetKeyState()
    {
        yield return new WaitForSeconds(keyHoldTime);
        keyState = KeyState.None;
    }
}