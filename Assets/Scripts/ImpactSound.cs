using UnityEngine;

public class ImpactSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip impactSound;
    public float minVelocity = 1f; // минимальная скорость для звука

    private float lastPlayTime;
    public float cooldown = 0.2f;

    private void OnCollisionEnter(Collision collision)
    {
        if (Time.time - lastPlayTime < cooldown)
            return;

        float impact = collision.relativeVelocity.magnitude;

        if (impact > minVelocity)
        {
            audioSource.PlayOneShot(impactSound);
            lastPlayTime = Time.time;
        }
    }
}
