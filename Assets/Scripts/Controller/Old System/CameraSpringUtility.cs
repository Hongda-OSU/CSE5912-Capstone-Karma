using UnityEngine;

public class CameraSpringUtility
{
    public Vector3 Values;
    private float frequency;
    private float damp;
    private Vector3 dampValues;

    public CameraSpringUtility(float frequency, float damp)
    {
        this.frequency = frequency;
        this.damp = damp;
    }

    public void UpdateSpring(float deltaTime, Vector3 target)
    {
        Values -= deltaTime * frequency * dampValues;
        dampValues = Vector3.Lerp(dampValues, Values - target, damp * deltaTime);
    }
}


