
using UnityEngine;
using IndieMarc.Platformer;

[RequireComponent(typeof(PlayerCharacter))]
public class PlayerAudio : MonoBehaviour
{
    [Header("Footsteps")]
    public AudioSource footstepSource;
    public AudioClip footstepClip;
    public float movementThreshold = 0.1f;

    [Header("Jump")]
    public AudioClip jumpClip;

    private PlayerCharacter player;

    private void Awake()
    {
        player = GetComponent<PlayerCharacter>();

        if (footstepSource != null)
        {
            footstepSource.clip = footstepClip;
            footstepSource.loop = true;
            footstepSource.playOnAwake = false;
            footstepSource.spatialBlend = 0f;
        }
    }

    private void OnEnable()
    {
        if (player == null)
            player = GetComponent<PlayerCharacter>();

        if (player != null)
            player.onJump += PlayJump;
    }

    private void OnDisable()
    {
        if (player != null)
            player.onJump -= PlayJump;

        StopFootsteps();
    }

    private void Update()
    {
        if (player == null ||
            footstepSource == null ||
            footstepClip == null)
            return;

        bool moving =
            Mathf.Abs(player.GetMove().x) > movementThreshold;

        bool shouldPlay =
            player.IsGrounded() &&
            moving &&
            !player.IsDead() &&
            Time.timeScale > 0f;

        if (shouldPlay)
        {
            if (!footstepSource.isPlaying)
                footstepSource.Play();
        }
        else
        {
            StopFootsteps();
        }
    }

    private void PlayJump()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(jumpClip);
    }

    private void StopFootsteps()
    {
        if (footstepSource != null &&
            footstepSource.isPlaying)
        {
            footstepSource.Stop();
        }
    }
}
