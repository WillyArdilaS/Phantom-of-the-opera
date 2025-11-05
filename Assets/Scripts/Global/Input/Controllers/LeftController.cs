using UnityEngine;
using UnityEngine.InputSystem;

public class LeftController : AbstractController
{
    // === Overridden abstract methods ===
    public override void ProcessInput(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            // Change the status of the key that has been pressed
            switch (ctx.control.name)
            {
                case "w":
                    keyState = KeyState.W;
                    break;
                case "a":
                    keyState = KeyState.A;
                    break;
                case "s":
                    keyState = KeyState.S;
                    break;
                case "d":
                    keyState = KeyState.D;
                    break;
            }

            InvokeKeyEvent();

            // Reset the key status after a moment
            if (resetKeyRoutine != null) StopCoroutine(resetKeyRoutine);
            resetKeyRoutine = StartCoroutine(ResetKeyState());
        }
    }
}