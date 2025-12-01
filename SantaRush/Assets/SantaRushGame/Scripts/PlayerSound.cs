using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip jumpClip;
    public AudioClip EnemyDumpClip;
    public AudioClip ObstaclesDumpClip;

    public AudioClip OrnamentClip;
    public AudioClip ChristmasstockingClip;
    public AudioClip WreathClip;

    public AudioClip DeathClip;

    public void PlayJump()
    {
        if (jumpClip != null)
            audioSource.PlayOneShot(jumpClip);
    }

    public void EnemyDump()
    {
        if (EnemyDumpClip != null)
            audioSource.PlayOneShot(EnemyDumpClip);
    }

    public void ObstaclesDump()
    {
        if (ObstaclesDumpClip != null)
            audioSource.PlayOneShot(ObstaclesDumpClip);
    }

    public void ItemOrnament()
    {
        if (OrnamentClip != null)
            audioSource.PlayOneShot(OrnamentClip);
    }

    public void ItemChristmasstocking()
    {
        if (ChristmasstockingClip != null)
            audioSource.PlayOneShot(ChristmasstockingClip);
    }

    public void ItemWreath()
    {
        if (WreathClip != null)
            audioSource.PlayOneShot(WreathClip);
    }

    public void PlayDeath()
    {
        if (DeathClip != null)
            audioSource.PlayOneShot(DeathClip);
    }
}
