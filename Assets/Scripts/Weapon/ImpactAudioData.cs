using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PolyGamers
{
    [CreateAssetMenu(menuName = "FPS/Impact Audio Data")]
    public class ImpactAudioData : ScriptableObject
    {
        public List<ImpactTagsWithAudio> ImpactTagsWithAudios;
    }

    [System.Serializable]
    public class ImpactTagsWithAudio
    {
        public string Tag;
        public List<AudioClip> ImpactAudioClips;
    }
}