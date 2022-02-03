using System.Collections.Generic;
using UnityEngine;

namespace PolyGamers.Weapon
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