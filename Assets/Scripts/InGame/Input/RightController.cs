using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class RightController : MonoBehaviour
{
    // === Button states ===
    public enum RightButtonState { None, UpArrow, LeftArrow, DownArrow, RightArrow }
    [SerializeField] private RightButtonState rightButton = RightButtonState.None;
    private const float buttonHoldTime = 0.1f;

    // Coroutines
    private Coroutine resetButtonRoutine;

    public void ProcessRightInput(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            // Change the status of the button that has been pressed
            switch (ctx.control.name)
            {
                case "upArrow":
                    rightButton = RightButtonState.UpArrow;
                    break;
                case "leftArrow":
                    rightButton = RightButtonState.LeftArrow;
                    break;
                case "downArrow":
                    rightButton = RightButtonState.DownArrow;
                    break;
                case "rightArrow":
                    rightButton = RightButtonState.RightArrow;
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
        rightButton = RightButtonState.None;
    }
}