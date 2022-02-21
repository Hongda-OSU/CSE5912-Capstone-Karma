using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class GrenadeThrower : MonoBehaviour
    {
        public GameObject Grenade;
        [SerializeField] private int grenadeCount;
        public Transform grenadeGenerationPoint;
        public float throwFroce = 15f;
        private Animator currentAnimator;

        void Start()
        {

        }


        void Update()
        {
            currentAnimator = WeaponManager.Instance.CarriedWeapon.GunAnimator;
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                currentAnimator.SetTrigger("ThrowGrenade");
                ThrowGrenade();
            }
        }

        private void ThrowGrenade()
        {
            GameObject grenade = Instantiate(Grenade, grenadeGenerationPoint.position, grenadeGenerationPoint.rotation);
            Rigidbody rb = grenade.GetComponent<Rigidbody>();
            rb.AddForce(grenadeGenerationPoint.forward * throwFroce, ForceMode.VelocityChange);
        }
    }
}
