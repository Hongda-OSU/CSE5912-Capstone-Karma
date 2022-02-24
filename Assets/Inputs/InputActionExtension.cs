using UnityEngine.InputSystem;

public static class InputActionExtension
{
    public static bool ContinuedShooting(this InputAction inputAction)
    {
         return inputAction.ReadValue<float>() > 0f;
    }

    public static bool FixedShooting(this InputAction inputAction)
    {
        return inputAction.triggered && inputAction.ReadValue<float>() > 0f;
    }

    public static bool StopShooting(this InputAction inputAction)
    {
        return inputAction.WasReleasedThisFrame();
    }
}