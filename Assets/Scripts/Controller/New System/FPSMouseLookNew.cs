using UnityEngine;

public class FPSMouseLookNew : MonoBehaviour
{
    public Camera cam;
    private float xRotation;

    public float MouseSensitivity;
    public Vector2 MaxminAngle;

    public void ProcessLook(Vector2 lookInput)
    {
        float mouseX = lookInput.x;
        float mouseY = lookInput.y;

        xRotation -= mouseY * MouseSensitivity * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, MaxminAngle.x, MaxminAngle.y);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up * (mouseX * Time.deltaTime) * MouseSensitivity);
    }
}