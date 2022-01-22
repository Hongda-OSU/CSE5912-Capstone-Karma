using UnityEngine.InputSystem;

public static class InputActionExtension
{
    // Helper method to check if an input action is pressed
    public static bool IsPressed(this InputAction inputAction)
    {
        return inputAction.ReadValue<float>() > 0f;
    }

    // Helper method to check if an input action is pressed each frame (continue)
    public static bool WasPressedThisFrame(this InputAction inputAction)
    {
        return inputAction.triggered && inputAction.ReadValue<float>() > 0f;
    }

    // Helper method to check if an input action is released this frame
    public static bool WasReleasedThisFrame(this InputAction inputAction)
    {
        return inputAction.triggered && inputAction.ReadValue<float>() == 0f;
    }
}