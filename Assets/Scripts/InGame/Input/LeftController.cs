using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class LeftController : MonoBehaviour
{
    // === Button states ===
    public enum LeftButtonState { None, W, A, S, D }
    [SerializeField] private LeftButtonState leftButton = LeftButtonState.None;
    private const float buttonHoldTime = 0.1f;

    // Coroutines
    private Coroutine resetButtonRoutine;

    public void ProcessLeftInput(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            // Change the status of the button that has been pressed
            switch (ctx.control.name)
            {
                case "w":
                    leftButton = LeftButtonState.W;
                    break;
                case "a":
                    leftButton = LeftButtonState.A;
                    break;
                case "s":
                    leftButton = LeftButtonState.S;
                    break;
                case "d":
                    leftButton = LeftButtonState.D;
                    break;
            }

            // Reset the button status after a moment
            if (resetButtonRoutine != null) StopCoroutine(resetButtonRoutine);
            resetButtonRoutine = StartCoroutine(ResetButtonState());
        }
    }

    public IEnumerator ResetButtonState()
    {
        yield return new WaitForSeconds(buttonHoldTime);
        leftButton = LeftButtonState.None;
    }
}