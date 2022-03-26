using UnityEngine;

namespace CSE5912.PolyGamers
{
    public class FPSMouseLook : MonoBehaviour
    {
        [Header("Transform")]
        [SerializeField] private Transform characterTransform;
        private Transform cameraTransform;

        [Header("Angle")]
        private Vector3 cameraRotation;
        public Vector2 MaxminAngle;

        [Header("Sensitivity")]
        public float MouseSensitivity;

        [Header("Recoil")]
        public AnimationCurve RecoilCurve;
        public Vector2 RecoilRange;
        public float RecoilScale = 1f;
        public float RecoilFactorAimed;
        public float RecoilFadeOutTime;
        private float currentRecoilTime;
        private Vector2 currentRecoil;

        public bool stopProcessingLook = false;

        // CameraShake 
        private CameraShake cameraShake;

        private static FPSMouseLook instance;
        public static FPSMouseLook Instance { get { return instance; } }

        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(gameObject);
            instance = this;
        }
        private void Start()
        {
            cameraTransform = transform;
            cameraShake = GetComponentInChildren<CameraShake>();
        }

        public void ProcessLook(Vector2 lookInput)
        {
            if (stopProcessingLook)
                return;

            var tmp_MouseX = lookInput.x;
            var tmp_MouseY = lookInput.y;

            cameraRotation.x -= tmp_MouseY * MouseSensitivity * Time.unscaledDeltaTime;
            cameraRotation.y += tmp_MouseX * MouseSensitivity * Time.unscaledDeltaTime;

            CalculateRecoilOffset();

            cameraRotation.x -= currentRecoil.x;
            if (UnityEngine.Random.value > 0.5f)
                cameraRotation.y += currentRecoil.y;
            else
                cameraRotation.y -= currentRecoil.y;

            cameraRotation.x = Mathf.Clamp(cameraRotation.x, MaxminAngle.x, MaxminAngle.y);

            characterTransform.rotation = Quaternion.Euler(0, cameraRotation.y, 0);
            cameraTransform.rotation = Quaternion.Euler(cameraRotation.x, cameraRotation.y, 0);
        }

        public void ResetLook()
        {
            cameraRotation = Vector2.zero;
            characterTransform.rotation = Quaternion.identity;
            cameraTransform.rotation = Quaternion.identity;
        }

        private void CalculateRecoilOffset()
        {
            currentRecoilTime += Time.unscaledDeltaTime;
            float tmp_RecoilFraction = currentRecoilTime / RecoilFadeOutTime;
            float tmp_RecoilValue = RecoilCurve.Evaluate(tmp_RecoilFraction);
            currentRecoil = Vector2.Lerp(Vector2.zero, currentRecoil, tmp_RecoilValue);
        }

        public void FiringWithRecoil()
        {
            currentRecoil += RecoilRange * RecoilScale;
            cameraShake.StartCameraSpring();
            currentRecoilTime = 0;
        }

        public void FiringWithRecoilAimed()
        {
            currentRecoil += RecoilRange * RecoilFactorAimed * RecoilScale;
            cameraShake.StartCameraSpringAimed();
            currentRecoilTime = 0;
        }
    }
}