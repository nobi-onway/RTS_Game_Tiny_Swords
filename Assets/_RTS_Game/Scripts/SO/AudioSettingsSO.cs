using UnityEngine;

[CreateAssetMenu(menuName = "SO/AudioSettingsSO", fileName = "AudioSettingsSO")]
public class AudioSettingsSO : ScriptableObject
{
    public AudioClip[] clips;
    public float Volume = 1.0f;
    public float Pitch = 1.0f;
    public bool Loop = false;
    public float SpatialBlend = 1.0f;
    public float MinDistance = 1.0f;
    public float MaxDistance = 15.0f;
    public EAudioPriority Priority = EAudioPriority.MEDIUM;
    public AudioRolloffMode RolloffMode = AudioRolloffMode.Linear;
}