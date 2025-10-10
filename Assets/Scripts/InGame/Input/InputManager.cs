using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput), typeof(LeftController), typeof(RightController))]
public class InputManager : MonoBehaviour
{
    // === Input ===
    private PlayerInput playerInput;
    private InputActionMap leftMap;
    private InputActionMap rightMap;

    // === Player controllers ===
    private LeftController leftController;
    private RightController rightController;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        leftController = GetComponent<LeftController>();
        rightController = GetComponent<RightController>();

        leftMap = playerInput.actions.FindActionMap("LeftSide");
        rightMap = playerInput.actions.FindActionMap("RightSide");

        leftMap.Enable();
        rightMap.Enable();

        playerInput.onActionTriggered += OnActionTriggered;
    }

    void OnDestroy()
    {
        playerInput.onActionTriggered -= OnActionTriggered;
    }

    private void OnActionTriggered(InputAction.CallbackContext ctx)
    {
        // Process the input according to the corresponding map
        if (ctx.action.actionMap == leftMap)
        {
            leftController.ProcessLeftInput(ctx);
        }
        else if (ctx.action.actionMap == rightMap)
        {
            rightController.ProcessRightInput(ctx);
        }
    }
}