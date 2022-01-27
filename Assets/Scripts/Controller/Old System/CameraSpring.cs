using UnityEngine;

public class CameraSpring : MonoBehaviour
{
    public float Frequency;
    public float Damp;
    public Vector2 MinRecoilRange;
    public Vector2 MaxRecoilRange;
    public float springSpeed;

    private CameraSpringUtility cameraSpringUtility;
    private Transform cameraSpringTransform;

    void Start()
    {
        cameraSpringUtility = new CameraSpringUtility(Frequency, Damp);
        cameraSpringTransform = transform;
    }

    void Update()
    {
        cameraSpringUtility.UpdateSpring(Time.deltaTime, Vector3.zero);
        cameraSpringTransform.localRotation = Quaternion.Slerp(cameraSpringTransform.localRotation,
            Quaternion.Euler(cameraSpringUtility.Values),
            Time.deltaTime * springSpeed);
    }

    public void StartCameraSpring()
    {
        cameraSpringUtility.Values = new Vector3(0,
            UnityEngine.Random.Range(MinRecoilRange.x, MaxRecoilRange.x),
            UnityEngine.Random.Range(MinRecoilRange.y, MaxRecoilRange.y));
    }
}
