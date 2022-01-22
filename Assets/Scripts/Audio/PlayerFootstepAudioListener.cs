using UnityEngine;

public class PlayerFootstepAudioListener : MonoBehaviour
{
    public FootStepAudioData FootStepAudioData;
    public AudioSource FootStepAudioSource;
    public LayerMask LayerMask;

    private CharacterController characterController;
    private Transform footstepTransform;
    private float nextPlayTime;


    void Start()
    {
        characterController = GetComponent<CharacterController>();
        footstepTransform = transform;
    }

    void FixedUpdate()
    {
        if (characterController.isGrounded)
        {
            if (characterController.velocity.normalized.magnitude >= 0.1f)
            {
                nextPlayTime += Time.deltaTime;
                bool tmp_IsHit = Physics.Linecast(footstepTransform.position,
                    footstepTransform.position + Vector3.down * (characterController.height / 2 + characterController.skinWidth - characterController.center.y),
                    out RaycastHit hitInfo, LayerMask);
                if (tmp_IsHit)
                {
                    foreach (var tmp_AudioElement in FootStepAudioData.FootStepAudios)
                    {
                        if (hitInfo.collider.CompareTag(tmp_AudioElement.Tag))
                        {
                            if (nextPlayTime >= tmp_AudioElement.Delay)
                            {
                                int tmp_AudioCount = tmp_AudioElement.AudioClips.Count;
                                int tmp_AudioIndex = UnityEngine.Random.Range(0, tmp_AudioCount);
                                AudioClip tmp_FootstepAudioClip = tmp_AudioElement.AudioClips[tmp_AudioIndex];
                                FootStepAudioSource.clip = tmp_FootstepAudioClip;
                                FootStepAudioSource.Play();
                                nextPlayTime = 0f;
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
