using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class Bullet : MonoBehaviour
    {
        public float BulletSpeed;
        public bool Penetrable;
        private Transform bulletTransform;
        // track bullet position
        private Vector3 prevPosition;
        // get impact component from Firearm class
        public GameObject ImpactPrefab;
        public ImpactAudioData impactAudioData;

        public float damage;
        public Element.Type elementType;

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
            CheckTargetHit(hit);

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

        private void CheckTargetHit(RaycastHit hit)
        {
            if (!Penetrable)
                Destroy(gameObject);

            hit.transform.TryGetComponent(out IDamageable target);
            if (target == null) 
                return;


            if (target is Enemy enemy)
            {
                PlayerManager.Instance.HitByBullet = enemy;
                PlayerManager.Instance.PerformBulletDamage(enemy, hit.point);
                PlayerManager.Instance.StackDebuff(WeaponManager.Instance.CarriedWeapon.Element, enemy);
            }
            else if (target is Shield shield)
            {
                Damage damage = new Damage(this.damage, elementType, PlayerStats.Instance, shield);
                PlayerManager.Instance.PerformBulletDamage(shield, damage, hit.point);
            }
        }

    }
}
