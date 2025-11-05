using UnityEngine;
using UnityEngine.InputSystem;

public class RightController : AbstractController
{
    // === Overridden abstract methods ===
    public override void ProcessInput(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            // Change the status of the key that has been pressed
            switch (ctx.control.name)
            {
                case "upArrow":
                    keyState = KeyState.UpArrow;
                    break;
                case "leftArrow":
                    keyState = KeyState.LeftArrow;
                    break;
                case "downArrow":
                    keyState = KeyState.DownArrow;
                    break;
                case "rightArrow":
                    keyState = KeyState.RightArrow;
                    break;
            }

            InvokeKeyEvent();

            // Reset the key status after a moment
            if (resetKeyRoutine != null) StopCoroutine(resetKeyRoutine);
            resetKeyRoutine = StartCoroutine(ResetKeyState());
        }
    }
}