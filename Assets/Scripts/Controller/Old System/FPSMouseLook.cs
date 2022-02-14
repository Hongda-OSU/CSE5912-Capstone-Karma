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

    [Header("Recoil")]
    public AnimationCurve RecoilCurve;
    public Vector2 RecoilRange;
    public float RecoilFactorAimed;
    public float RecoilFadeOutTime;
    private float currentRecoilTime;
    private Vector2 currentRecoil;

    // CameraShake 
    private CameraShake cameraShake;

    private void Start()
    {
        cameraTransform = transform;
        cameraShake = GetComponentInChildren<CameraShake>();
    }

    void LateUpdate()
    {
        var tmp_MouseX = Input.GetAxis("Mouse X");
        var tmp_MouseY = Input.GetAxis("Mouse Y");

        cameraRotation.x -= tmp_MouseY * MouseSensitivity;
        cameraRotation.y += tmp_MouseX * MouseSensitivity;

        CalculateRecoilOffset();

        cameraRotation.x -= currentRecoil.x;
        if (UnityEngine.Random.value > 0.5f)
            cameraRotation.y += currentRecoil.y;
        else
            cameraRotation.y -= currentRecoil.y;

        cameraRotation.x = Mathf.Clamp(cameraRotation.x, MaxminAngle.x, MaxminAngle.y);

        characterTransform.rotation = Quaternion.Euler(0, cameraRotation.y, 0);
        cameraTransform.rotation = Quaternion.Euler(cameraRotation.x, cameraRotation.y, 0);


    }

    private void CalculateRecoilOffset()
    {
        currentRecoilTime += Time.deltaTime;
        float tmp_RecoilFraction = currentRecoilTime / RecoilFadeOutTime;
        float tmp_RecoilValue = RecoilCurve.Evaluate(tmp_RecoilFraction);
        currentRecoil = Vector2.Lerp(Vector2.zero, currentRecoil, tmp_RecoilValue);
    }

    public void FiringWithRecoil()
    {
        currentRecoil += RecoilRange;
        cameraShake.StartCameraSpring();
        currentRecoilTime = 0;
    }

    public void FiringWithRecoilAimed()
    {
        currentRecoil += RecoilRange * RecoilFactorAimed;
        cameraShake.StartCameraSpringAimed();
        currentRecoilTime = 0;
    }
}