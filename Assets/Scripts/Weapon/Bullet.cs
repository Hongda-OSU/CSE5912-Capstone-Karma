using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Bullet : MonoBehaviour
    {
        public float BulletSpeed;
        private Transform bulletTransform;
        // track bullet position
        private Vector3 prevPosition;
        // get impact component from Firearm class
        public GameObject ImpactPrefab;
        public ImpactAudioData impactAudioData;

        void Start()
        {
            bulletTransform = transform;
            prevPosition = bulletTransform.position;
        }
        void Update()
        {
            prevPosition = bulletTransform.position;

            bulletTransform.Translate(0, 0, BulletSpeed * Time.deltaTime);

            if (!Physics.Raycast(prevPosition,
                (bulletTransform.position - prevPosition).normalized,
                out RaycastHit hit,
                (bulletTransform.position - prevPosition).magnitude)) return;

            // TODO:
            CheckDamgeEenemy(hit);

            // impact prefab
            if (hit.transform.tag != "Enemy")
            {
                var tmp_BulletEffect =
                    Instantiate(ImpactPrefab,
                        hit.point,
                        Quaternion.LookRotation(hit.normal, Vector3.up));
                Destroy(tmp_BulletEffect, 1f);
            }

            // For impact Audio
            var tmp_TagsWithAudio = impactAudioData.ImpactTagsWithAudios.Find((audioData) =>
            {
                return audioData.Tag.Equals(hit.collider.tag);
            });
            if (tmp_TagsWithAudio == null) return;
            int tmp_AudioDataCount = tmp_TagsWithAudio.ImpactAudioClips.Count;
            AudioClip tmp_AudioClip = tmp_TagsWithAudio.ImpactAudioClips[UnityEngine.Random.Range(0, tmp_AudioDataCount)];
            AudioSource.PlayClipAtPoint(tmp_AudioClip, hit.point);
        }

        private void CheckDamgeEenemy(RaycastHit hit)
        {
            if (hit.transform.tag == "Enemy")
            {
                PlayerManager.Instance.PerformDamage(hit.transform.GetComponent<Enemy>(), hit.point);
            }
        }

    }
}
