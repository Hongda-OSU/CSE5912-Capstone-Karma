using UnityEngine;

namespace PolyGamers.Weapon
{
    [CreateAssetMenu(menuName = "FPS/Firearms Audio Data")]
    public class FirearmsAudioData : ScriptableObject
    {
        public AudioClip ShootingAudio;
        public AudioClip ReloadLeft;
        public AudioClip ReloadOutOf;
    }
}
