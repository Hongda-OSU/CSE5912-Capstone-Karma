using UnityEngine;

public class LaunchBulletTime : MonoBehaviour
{
    public float theTimeScale;

    public RadiaBlur radiaBlue;
    public ColorAdjustEffect cae;

    public AudioSource ass;
    public AudioClip clipIn;
    public AudioClip clipOut;

    public float t;
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.F))
        //{
        //    t = 0;
        //    ass.PlayOneShot(clipIn);
        //}

        //if (Input.GetKey(KeyCode.F))
        //{

        //    t += Time.deltaTime;

        //    Time.timeScale = Mathf.Lerp(Time.timeScale, 0.2f, t);

        //    radiaBlue.Level = Mathf.Lerp(radiaBlue.Level, 15, t);

        //    cae.saturation = Mathf.Lerp(cae.saturation, 0.5f, t);
        //}
        //if (Input.GetKeyUp(KeyCode.F))
        //{
        //    t = 2f;
        //    ass.PlayOneShot(clipOut);
        //    Time.timeScale = Mathf.Lerp(Time.timeScale, 1f, t);
        //    radiaBlue.Level = Mathf.Lerp(radiaBlue.Level, 1, t);
        //    cae.saturation = Mathf.Lerp(cae.saturation, 1f, t);

        //}



    }
}
