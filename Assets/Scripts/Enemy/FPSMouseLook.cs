using UnityEngine;

public class FPSMouseLook : MonoBehaviour
{
    [Header("Transform")]
    [SerializeField] private Transform characterTransform;
    private Transform cameraTransform;
    [Header("Angle")]
    private Vector3 cameraRotation;
    public Vector2 MaxminAngle;
    [Header("Sensitivity")]
    public float MouseSensitivity;


    private void Start()
    {
        cameraTransform = transform;
    }

    void LateUpdate()
    {
        var tmp_MouseX = Input.GetAxis("Mouse X");
        var tmp_MouseY = Input.GetAxis("Mouse Y");

        cameraRotation.x -= tmp_MouseY * MouseSensitivity;
        cameraRotation.y += tmp_MouseX * MouseSensitivity;

        cameraRotation.x = Mathf.Clamp(cameraRotation.x, MaxminAngle.x, MaxminAngle.y);

        characterTransform.rotation = Quaternion.Euler(0, cameraRotation.y, 0);
        cameraTransform.rotation = Quaternion.Euler(cameraRotation.x, cameraRotation.y, 0);

    }
}