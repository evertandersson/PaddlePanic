using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Audio Events/New Audio Event", fileName = "Sound_")]
public class SoundEvent : AudioEvent
{
    public AudioClip[] clips;
    public RangedFloat volume;

    [MinMaxRange(0, 2)]  
    public RangedFloat pitch;
    

    public override void Play(AudioSource source)
    {
        if (clips.Length == 0)  return;
        source.clip = clips[Random.Range(0, clips.Length)];
        source.volume = Random.Range(volume.MinValue, volume.MaxValue);
        source.pitch = Random.Range(pitch.MinValue, pitch.MaxValue);
        source.Play();
    }

    public override void Stop(AudioSource source)
    {
        source.Stop();
    }
}