using UnityEngine;

namespace Assets.Scripts.Weapon
{
    [CreateAssetMenu(menuName = "FPS/Firearms Audio Data")]
    public class FirearmsAudioData : ScriptableObject
    {
        public AudioClip ShootingAudio;
        public AudioClip ReloadLeft;
        public AudioClip ReloadOutOf;


    }
}
