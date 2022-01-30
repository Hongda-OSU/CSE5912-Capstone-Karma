using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FPS/Footstep Audio Data")]
public class FootStepAudioData : ScriptableObject
{
    public List<FootStepAudio> FootStepAudios = new List<FootStepAudio>();
}

[System.Serializable]
public class FootStepAudio
{
    public string Tag;
    public List<AudioClip> AudioClips = new List<AudioClip>();
    public float Delay;
}