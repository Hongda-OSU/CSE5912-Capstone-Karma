using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float Frequency;
    public float Damp;
    public Vector2 MinRecoilRange;
    public Vector2 MaxRecoilRange;
    public float SpringSpeed;
    public float SpringFactorAimed;

    private CameraShakeUtility cameraShakeUtility;
    private Transform cameraSpringTransform;

    void Start()
    {
        cameraShakeUtility = new CameraShakeUtility(Frequency, Damp);
        cameraSpringTransform = transform;
    }

    void Update()
    {
        cameraShakeUtility.UpdateSpring(Time.deltaTime, Vector3.zero);
        cameraSpringTransform.localRotation = Quaternion.Slerp(cameraSpringTransform.localRotation,
            Quaternion.Euler(cameraShakeUtility.Values),
            Time.deltaTime * SpringSpeed);
    }

    public void StartCameraSpring()
    {
        cameraShakeUtility.Values = new Vector3(0,
            UnityEngine.Random.Range(MinRecoilRange.x, MaxRecoilRange.x),
            UnityEngine.Random.Range(MinRecoilRange.y, MaxRecoilRange.y));
    }

    public void StartCameraSpringAimed()
    {
        cameraShakeUtility.Values = new Vector3(0,
            UnityEngine.Random.Range(MinRecoilRange.x, MaxRecoilRange.x) * SpringFactorAimed,
            UnityEngine.Random.Range(MinRecoilRange.y, MaxRecoilRange.y)) * SpringFactorAimed;
    }
}
