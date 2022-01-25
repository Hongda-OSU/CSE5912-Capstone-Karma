using UnityEngine;

public class LeanController : MonoBehaviour
{
    public Transform PeakLeft;
    public Transform PeakRight;
    public Transform Idle;
    public float lerpTime;

    void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            transform.position = Vector3.Lerp(transform.position, PeakLeft.position, lerpTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, PeakLeft.rotation, lerpTime);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, Idle.position, lerpTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, Idle.rotation, lerpTime);
        }

        if (Input.GetKey(KeyCode.E))
        {
            transform.position = Vector3.Lerp(transform.position, PeakRight.position, lerpTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, PeakRight.rotation, lerpTime);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, Idle.position, lerpTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, Idle.rotation, lerpTime);
        }


    }
}
