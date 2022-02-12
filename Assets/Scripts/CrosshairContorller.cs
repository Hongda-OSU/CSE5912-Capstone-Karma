using PolyGamers.Weapon;
using UnityEngine;

public class CrosshairContorller : MonoBehaviour
{
    public CharacterController CharacterController;
    [SerializeField] private WeaponManager weaponManager;
    public RectTransform Reticle;
    public float OriginalSize;
    public float FiringSize;
    public float MaxSize;
    private float currentSize;

    void Start()
    {
        
    }

    void Update()
    {
        bool isMoving = CharacterController.velocity.magnitude > 0;
        if (isMoving)
            currentSize = Mathf.Lerp(currentSize, MaxSize, Time.deltaTime * 5);
        else if (weaponManager.isFiring || weaponManager.isAiming)
            currentSize = Mathf.Lerp(currentSize, FiringSize, Time.deltaTime * 10);
        else
            currentSize = Mathf.Lerp(currentSize, OriginalSize, Time.deltaTime * 5);
        Reticle.sizeDelta = new Vector2(currentSize, currentSize);
    }
}
