using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Bullet : MonoBehaviour
    {
        public float BulletSpeed;
        public GameObject ImpactPrefab;
        public ImpactAudioData impactAudioData;
        private Transform bulletTransform;
        private Vector3 prevPosition;

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
                out RaycastHit tmp_Hit,
                (bulletTransform.position - prevPosition).magnitude)) return;

            // TODO:
            CheckDamgeEenemy(tmp_Hit);


            //var tmp_BulletEffect =
            //    Instantiate(ImpactPrefab,
            //        tmp_Hit.point,
            //        Quaternion.LookRotation(tmp_Hit.normal, Vector3.up));
            //Destroy(tmp_BulletEffect, 0.3f);


            // For Audio
            var tmp_TagsWithAudio = impactAudioData.ImpactTagsWithAudios.Find((tmp_AudioData) =>
            {
                return tmp_AudioData.Tag.Equals(tmp_Hit.collider.tag);
            });
            if (tmp_TagsWithAudio == null) return;
            int tmp_AudioDataCount = tmp_TagsWithAudio.ImpactAudioClips.Count;
            AudioClip tmp_AudioClip = tmp_TagsWithAudio.ImpactAudioClips[UnityEngine.Random.Range(0, tmp_AudioDataCount)];
            AudioSource.PlayClipAtPoint(tmp_AudioClip, tmp_Hit.point);
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
